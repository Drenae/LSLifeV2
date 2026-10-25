using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Interface de stockage : deux contextes distincts, aucun transfert de donnees ici.
// L'ouverture depuis les interactions et la migration des sous-menus sont a raccorder.
internal static class CustomStashMenu
{
    private static CustomMenu houseMenu;
    private static CustomMenu vehicleMenu;
    private static CustomMenu activeMenu;
    private static bool vehicle, transfer, put;
    private static int stepIndex;
    private static readonly int[] steps = { 1, 5, 10, 25, 50, 100 };
    private static bool waitForRelease;
    private static int lastFrame = -1;

    public static bool Visible => activeMenu != null && activeMenu.Visible;

    public static void OpenHouse()
    {
        if (Visible || LSL.LsMenuPool.IsAnyMenuOpen() ||
            LSL.HouseHandler.CurrentHouse == null) return;
        vehicle = false;
        if (houseMenu == null)
        {
            houseMenu = new CustomMenu("PLANQUE", "");
            houseMenu.Add("Deposer", () => OpenTransfer(false, true));
            houseMenu.Add("Retirer", () => OpenTransfer(false, false));
        }
        activeMenu = houseMenu;
        activeMenu.Open();
        waitForRelease = true;
    }

    public static void OpenVehicle()
    {
        if (Visible || LSL.LsMenuPool.IsAnyMenuOpen() || LSL.pStashVehicle == null) return;
        vehicle = true;
        if (vehicleMenu == null)
        {
            vehicleMenu = new CustomMenu("COFFRE", "");
            vehicleMenu.Add("Deposer", () => OpenTransfer(true, true));
            vehicleMenu.Add("Retirer", () => OpenTransfer(true, false));
        }
        activeMenu = vehicleMenu;
        activeMenu.Open();
        waitForRelease = true;
    }


    private static void OpenTransfer(bool car, bool deposit)
    {
        vehicle = car; put = deposit; transfer = true; stepIndex = 0;
        var menu = new CustomMenu(deposit ? "DEPOSER" : "RETIRER", "");
        menu.Add("Multiplicateur", () => Change(1));
        for (int n = 0; n < 3; n++) { int k = n; menu.Add(DrugName(k), () => Commit(k)); }
        if (deposit || car)
            for (int n = 3; n < 6; n++) { int k = n; menu.Add(DrugName(k), () => Commit(k)); }
        menu.Add("Argent", () => Commit(6));
        menu.Add(deposit ? "Deposer un gilet" : "Retirer un gilet", () => Commit(7));
        menu.Add(deposit ? "Deposer une arme" : "Retirer une arme", () => Commit(8));
        activeMenu.Close(); activeMenu = menu; menu.Open(); waitForRelease = true;
        Refresh();
    }
    private static string DrugName(int k) => k % 3 == 0 ? "Cannabis" : k % 3 == 1 ? "Crack" : "Cocaine";
    private static int Kind(int row)
    {
        if (row == 0) return -1;
        if (row <= 3) return row - 1;
        if (put || vehicle) return row <= 6 ? row - 1 : row - 1;
        return row == 4 ? 6 : row == 5 ? 7 : 8;
    }
    private static NativeUI.UIMenuSliderItem Slider(int k)
    {
        if (vehicle)
        {
            var a = put
                ? new[] { LSL.putWeedVeh, LSL.putCrackVeh, LSL.putCocainVeh, LSL.putWeedOzVeh, LSL.putCrackOzVeh, LSL.putCocainOzVeh, LSL.putMoneyVeh }
                : new[] { LSL.takeWeedVeh, LSL.takeCrackVeh, LSL.takeCocainVeh, LSL.takeWeedOzVeh, LSL.takeCrackOzVeh, LSL.takeCocainOzVeh, LSL.takeMoneyVeh };
            return a[k];
        }
        if (put)
        {
            var a = new[] { LSL.putWeedStash, LSL.putCrackStash, LSL.putCocainStash, LSL.putWeedOzStash, LSL.putCrackOzStash, LSL.putCocainOzStash, LSL.putMoneyStash };
            return a[k];
        }
        return k == 0 ? LSL.takeWeedStash : k == 1 ? LSL.takeCrackStash : k == 2 ? LSL.takeCocainStash : LSL.takeMoneyStash;
    }
    private static NativeUI.UIMenuListItem List(int k)
    {
        if (vehicle) return k == 7 ? (put ? LSL.putArmourVeh : LSL.takeArmourVeh) : (put ? LSL.putWeaponVeh : LSL.takeWeaponVeh);
        return k == 7 ? (put ? LSL.putArmourStash : LSL.takeArmourStash) : (put ? LSL.putWeaponStash : LSL.takeWeaponStash);
    }
    private static void Refresh()
    {
        if (vehicle) { if (LSL.pStashVehicle != null) LSL.pStashVehicle.UpdateCarStash(); }
        else if (LSL.HouseHandler.CurrentHouse != null) LSL.HouseHandler.UpdateMenu(LSL.HouseHandler.CurrentHouse);
    }
    private static void Change(int direction)
    {
        int k = Kind(activeMenu.SelectedIndex);
        if (k == -1) { stepIndex = (stepIndex + direction + steps.Length) % steps.Length; return; }
        if (k <= 6)
        {
            var a = Slider(k);
            a.Value = Math.Max(0, Math.Min(a.Maximum, a.Value + direction * steps[stepIndex]));
        }
        else
        {
            var a = List(k);
            if (a.Items.Count > 0) a.Index = (a.Index + direction + a.Items.Count) % a.Items.Count;
        }
    }
    private static void Commit(int k)
    {
        if (k <= 6 || k == 8)
        {
            NativeUI.UIMenuItem item = k <= 6 ? (NativeUI.UIMenuItem)Slider(k) : List(k);
            if (k <= 6 && Slider(k).Value <= 0) return;
            if (k == 8 && List(k).Items.Count == 0) return;
            if (vehicle)
            {
                if (put) LsMenus.onPutVehicleMenuSelect(LSL.vehStashPutMenu, item, 0);
                else LsMenus.onTakeVehicleMenuSelect(LSL.vehStashTakeMenu, item, 0);
            }
            else if (put) LsMenus.onStashMenuSelect(LSL.stashPutMenu, item, 0);
            else TakeHouse(k);
            if (k <= 6) Slider(k).Value = 0;
        }
        else
        {
            int amount = List(7).Index;
            if (amount <= 0) return;
            if (vehicle)
            {
                var car = LSL.pStashVehicle;
                if (car == null) return;
                amount = Math.Min(amount, put ? LSL.playerArmours : car.drugCarArmour);
                if (put) { LSL.playerArmours -= amount; car.drugCarArmour += amount; }
                else { car.drugCarArmour -= amount; LSL.playerArmours += amount; }
                car.UpdateCarStash();
            }
            else
            {
                var house = LSL.HouseHandler.CurrentHouse;
                if (house == null) return;
                amount = Math.Min(amount, put ? LSL.playerArmours : house.Armor);
                if (put) { LSL.playerArmours -= amount; house.Armor += amount; }
                else { house.Armor -= amount; LSL.playerArmours += amount; }
                LSL.HouseHandler.SaveHouse(house);
            }
            LsFunctions.UpdateInventory();
        }
        Refresh();
    }
    private static void TakeHouse(int k)
    {
        var house = LSL.HouseHandler.CurrentHouse;
        if (house == null) return;
        if (k <= 2)
        {
            int amount = Slider(k).Value;
            if (k == 0) { amount = Math.Min(amount, house.Weed); house.Weed -= amount; LSL.PlayerInventory["Weed"] += amount; }
            if (k == 1) { amount = Math.Min(amount, house.Crack); house.Crack -= amount; LSL.PlayerInventory["Crack"] += amount; }
            if (k == 2) { amount = Math.Min(amount, house.Cocain); house.Cocain -= amount; LSL.PlayerInventory["Cocaine"] += amount; }
        }
        else if (k == 6)
        {
            int amount = Math.Min(Slider(k).Value, house.Money);
            house.Money -= amount; Game.Player.Money += amount;
        }
        else if (k == 8 && house.Weapons.Count > 0)
        {
            int index = List(8).Index;
            if (index < 0 || index >= house.Weapons.Count) return;
            var w = house.Weapons[index];
            var hash = w.Element("HASH");
            GTA.Native.WeaponHash weapon;
            if (hash == null || !Enum.TryParse(hash.Value, out weapon)) return;
            int ammo = 0; int.TryParse((string)w.Element("AMMO"), out ammo);
            Game.Player.Character.Weapons.Give(weapon, ammo, true, false);
            LSL.playerWeapons.Add(w); LSL.playerWeps.Add(hash.Value);
            house.Weapons.RemoveAt(index); house.Weps.RemoveAt(index);
        }
        LsFunctions.UpdateInventory();
        LSL.HouseHandler.SaveHouse(house);
    }
    private static string Value(int k)
    {
        if (k == -1) return "< x" + steps[stepIndex] + " >";
        if (k <= 6)
        {
            int value = Slider(k).Value;
            return k <= 2 ? LsFunctions.GramsToOz(value) : k <= 5 ? value + " oz" : LsFunctions.IntToMoney(value) + " $";
        }
        var item = List(k);
        return item.Items.Count == 0 ? "Aucun" : k == 7 ? "< " + item.Index + " >" : "< " + item.Items[item.Index] + " >";
    }
    private static void Section(CustomMenuLayout layout, string title)
    {
        layout.Block(0.028f, y => CustomMenuRenderer.TextCenteredVertically(title, layout.Left + 0.019f, y, 0.028f, 0.28f, 226, 206, 167, 255));
    }
    private static void DrawTransfer()
    {
        var layout = new CustomMenuLayout();
        layout.Header(activeMenu.Title, vehicle ? "COFFRE" : "PLANQUE", titleScale: 0.57f);
        for (int i = 0; i < activeMenu.Entries.Count; i++)
        {
            int k = Kind(i);
            if (i == 1 || k == 3 || k == 6 || k == 7) layout.Separator(0.004f);
            if (i == 1 && (put || vehicle)) Section(layout, "EN VRAC");
            if (k == 3) Section(layout, "CONDITIONNE");
            string label = k >= 0 && k <= 5 ? DrugName(k) : activeMenu.Entries[i].Label;
            layout.Row(label, activeMenu.SelectedIndex == i, Value(k), 0.040f, redValue: k >= 3 && k <= 5);
            if (k >= 0 && k <= 6)
            {
                var slider = Slider(k);
                float ratio = slider.Maximum > 0 ? (float)slider.Value / slider.Maximum : 0f;
                int red = k >= 3 && k <= 5 ? 226 : 111;
                int green = k >= 3 && k <= 5 ? 85 : 216;
                layout.Block(0.005f, y => CustomMenuRenderer.Rect(layout.Left + 0.020f + ratio * 0.105f,
                    y + 0.002f, Math.Max(0.001f, ratio * 0.21f), 0.003f, red, green, k >= 3 && k <= 5 ? 85 : 139, 230));
            }
        }
        layout.Render();
    }
    public static void Close()
    {
        if (activeMenu != null) activeMenu.Close();
        activeMenu = null;
        transfer = false;
    }

    public static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (lastFrame == frame) return;
        lastFrame = frame;
        if (!Visible) return;
        foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }
        if (waitForRelease)
        {
            if (!Pressed(201) && !Pressed(202)) waitForRelease = false;
            if (transfer) DrawTransfer(); else Draw();
            return;
        }
        if (transfer && (JustPressed(174) || JustPressed(175))) Change(JustPressed(175) ? 1 : -1);
        else if (JustPressed(172)) activeMenu.Move(-1);
        else if (JustPressed(173)) activeMenu.Move(1);
        else if (JustPressed(201))
        {
            activeMenu.Activate();
            // L'action peut fermer le menu pour ouvrir un sous-menu NativeUI.
            // Ne jamais dessiner activeMenu apres cette transition.
            if (!Visible) return;
        }
        else if (JustPressed(202))
        {
            if (transfer)
            {
                transfer = false; activeMenu.Close();
                activeMenu = vehicle ? vehicleMenu : houseMenu;
                activeMenu.Open(); waitForRelease = true;
            }
            else Close();
            return;
        }
        if (Visible) { if (transfer) DrawTransfer(); else Draw(); }
    }

    private static bool Pressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);

    private static bool JustPressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);

    private static void Draw()
    {
        var layout = new CustomMenuLayout();
        layout.Header(activeMenu.Title, titleScale: 0.62f);
        layout.Separator(0.004f);
        for (int i = 0; i < activeMenu.Entries.Count; i++)
            layout.Row(activeMenu.Entries[i].Label, activeMenu.SelectedIndex == i);

        if (vehicle)
        {
            // Deux stocks distincts : produits en grammes et lots en onces.
            // Les donnees et la logique metier restent strictement inchangees.
            var car = LSL.pStashVehicle;
            if (car == null) { Close(); return; }
            layout.Separator(0.004f);
            layout.Block(0.029f, y =>
                CustomMenuRenderer.TextCenteredVertically("STOCK EN VRAC",
                    layout.Left + 0.019f, y, 0.029f, 0.28f, 226, 206, 167, 255));
            layout.TextPair("Cannabis", LsFunctions.GramsToOz(car.drugCarWeed));
            layout.TextPair("Crack", LsFunctions.GramsToOz(car.drugCarCrack));
            layout.TextPair("Cocaine", LsFunctions.GramsToOz(car.drugCarCocain));
            layout.Separator(0.005f);
            layout.Block(0.029f, y =>
                CustomMenuRenderer.TextCenteredVertically("STOCK CONDITIONNE",
                    layout.Left + 0.019f, y, 0.029f, 0.28f, 226, 206, 167, 255));
            layout.ColoredTextPair("Cannabis", car.drugCarWeedOz + " oz", 226, 85, 85);
            layout.ColoredTextPair("Crack", car.drugCarCrackOz + " oz", 226, 85, 85);
            layout.ColoredTextPair("Cocaine", car.drugCarCocainOz + " oz", 226, 85, 85);
            layout.Separator(0.005f);
            layout.TextPair("Argent", LsFunctions.IntToMoney(car.drugCarMoney) + " $");
        }
        else
        {
            layout.Separator(0.006f);
            var house = LSL.HouseHandler.CurrentHouse;
            if (house == null) { Close(); return; }
            layout.TextPair("Cannabis", LsFunctions.GramsToOz(house.Weed));
            layout.TextPair("Crack", LsFunctions.GramsToOz(house.Crack));
            layout.TextPair("Cocaine", LsFunctions.GramsToOz(house.Cocain));
            layout.TextPair("Argent", LsFunctions.IntToMoney(house.Money) + " $");
        }
        layout.Render();
    }
}
