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
    private static readonly int[] selected = new int[9];
    private static readonly int[] quantity = new int[7];
    private static readonly int[] steps = { 1, 5, 10, 25, 50, 100 };
    private static bool waitForRelease;
    private static int lastFrame = -1;
    private static readonly CustomMenuShared.HorizontalRepeat horizontalRepeat = new CustomMenuShared.HorizontalRepeat();

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
        Array.Clear(selected, 0, selected.Length);
        Array.Clear(quantity, 0, quantity.Length);
        horizontalRepeat.Reset();
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
    private static int MaxQuantity(int k)
    {
        if (k == 6) return Math.Max(0, vehicle
            ? (put ? Game.Player.Money : LSL.pStashVehicle.drugCarMoney)
            : (put ? Game.Player.Money : LSL.HouseHandler.CurrentHouse.Money));
        if (put)
        {
            switch (k)
            {
                case 0: return LSL.PlayerInventory["Weed"];
                case 1: return LSL.PlayerInventory["Crack"];
                case 2: return LSL.PlayerInventory["Cocaine"];
                case 3: return LsFunctions.pWeedOunce;
                case 4: return LsFunctions.pCrackOunce;
                case 5: return LsFunctions.pCocaineOunce;
            }
        }
        if (vehicle)
        {
            var car = LSL.pStashVehicle;
            switch (k)
            {
                case 0: return car.drugCarWeed;
                case 1: return car.drugCarCrack;
                case 2: return car.drugCarCocain;
                case 3: return car.drugCarWeedOz;
                case 4: return car.drugCarCrackOz;
                case 5: return car.drugCarCocainOz;
            }
        }
        else
        {
            var house = LSL.HouseHandler.CurrentHouse;
            switch (k)
            {
                case 0: return house.Weed;
                case 1: return house.Crack;
                case 2: return house.Cocain;
            }
        }
        return 0;
    }
    private static int Count(int k)
    {
        if (k == 7)
            return 1 + Math.Max(0, vehicle
                ? (put ? LSL.playerArmours : LSL.pStashVehicle.drugCarArmour)
                : (put ? LSL.playerArmours : LSL.HouseHandler.CurrentHouse.Armor));
        return vehicle
            ? (put ? LSL.playerWeps.Count : LSL.pStashVehicle.vehWeps.Count)
            : (put ? LSL.playerWeps.Count : LSL.HouseHandler.CurrentHouse.Weps.Count);
    }

    private static string WeaponName(int i)
    {
        if (put) return LSL.playerWeps[i];
        return vehicle ? LSL.pStashVehicle.vehWeps[i].ToString()
            : LSL.HouseHandler.CurrentHouse.Weps[i].ToString();
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
            long next = (long)quantity[k] + (long)direction * steps[stepIndex];
            quantity[k] = (int)Math.Max(0L, Math.Min((long)Math.Max(0, MaxQuantity(k)), next));
        }
        else
        {
            int count = Count(k);
            if (count > 0) selected[k] = (selected[k] + direction + count) % count;
        }
    }
    private static void Commit(int k)
    {
        if (k <= 6 || k == 8)
        {
            if (k <= 6 && quantity[k] <= 0) return;
            if (k == 8 && Count(k) == 0) return;
            if (vehicle)
            {
                if (k <= 6) VehicleStashTransfer.Transfer(put, k, quantity[k]);
                else VehicleWeaponTransfer.Transfer(put, selected[8]);
            }
            else if (put) HouseStashTransfer.Deposit(k, k <= 6 ? quantity[k] : selected[k]);
            else TakeHouse(k);
            if (k <= 6) quantity[k] = 0;
        }
        else
        {
            int amount = selected[7];
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
        if (k >= 7) selected[k] = Math.Min(selected[k], Math.Max(0, Count(k) - 1));
    }
    private static void TakeHouse(int k)
    {
        HouseStashTransfer.Withdraw(k, k <= 6 ? quantity[k] : selected[k]);
    }
    private static string Value(int k)
    {
        if (k == -1) return "< x" + steps[stepIndex] + " >";
        if (k <= 6)
        {
            int value = Math.Min(quantity[k], Math.Max(0, MaxQuantity(k)));
            return k <= 2 ? LsFunctions.GramsToOz(value).Replace(", ", " ").Replace(",", "")
                : k <= 5 ? value + "oz"
                : LsFunctions.IntToMoney(value) + "$";
        }
        int count = Count(k);
        if (count == 0) return "Aucun";
        int index = Math.Min(selected[k], count - 1);
        return k == 7 ? "< " + index + " >" : "< " + CustomMenuShared.FrenchWeaponName(WeaponName(index)) + " >";
    }
    private static string AvailableValue(int k)
    {
        if (k < 0 || k > 6) return null;
        int maximum = Math.Max(0, MaxQuantity(k));
        return k <= 2 ? "/ " + LsFunctions.GramsToOz(maximum).Replace(", ", " ").Replace(",", "")
            : k <= 5 ? "/ " + maximum + "oz"
            : "/ " + LsFunctions.IntToMoney(maximum) + "$";
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
            bool selected = activeMenu.SelectedIndex == i;
            bool available = k < 0 || (k <= 6 ? MaxQuantity(k) > 0 :
                (k == 7 ? Count(k) > 1 : Count(k) > 0));
            // Valeurs dessinees ensemble, alignees a droite : aucune colonne flottante.
            // Les codes GTA ~c~ gardent la disponibilite en gris.
            bool isQuantity = k >= 0 && k <= 6;
            layout.Row(label, selected, isQuantity ? null : Value(k),
                0.040f, enabled: available, redValue: k >= 3 && k <= 5,
                dimValue: !available, emphasizeValue: selected && available);
            if (isQuantity)
                layout.Block(0f, y => CustomMenuRenderer.TextTwoColorsRight(
                    Value(k), AvailableValue(k).Substring(2), layout.Right - 0.019f,
                    y - 0.040f, 0.040f, 0.245f,
                    k >= 3 && k <= 5 ? 226 : 111,
                    k >= 3 && k <= 5 ? 85 : 216,
                    k >= 3 && k <= 5 ? 85 : 139, !available));
            if (k >= 0 && k <= 6)
            {
                // Barre dans la ligne, comme le menu Approvisionner de l'employe.
                int maximum = Math.Max(0, MaxQuantity(k));
                float fraction = maximum > 0 ? Math.Min(1f, (float)quantity[k] / maximum) : 0f;
                bool red = k >= 3 && k <= 5;
                layout.Block(0f, y =>
                {
                    const float barWidth = 0.043f;
                    float barLeft = layout.Left + 0.101f;
                    float barY = y - 0.040f / 2f;
                    CustomMenuRenderer.Rect(barLeft + barWidth / 2f, barY, barWidth, 0.008f,
                        !available ? 47 : selected ? 108 : 66,
                        !available ? 48 : selected ? 112 : 68,
                        !available ? 50 : selected ? 115 : 70, 255);
                    if (fraction > 0f)
                        CustomMenuRenderer.Rect(barLeft + barWidth * fraction / 2f, barY,
                            barWidth * fraction, 0.008f,
                            red ? (selected ? 255 : 226) : (selected ? 155 : 111),
                            red ? (selected ? 115 : 85) : (selected ? 255 : 216),
                            red ? (selected ? 115 : 85) : (selected ? 175 : 139), 255);
                });
            }
        }
        layout.Render();
    }
    public static void Close()
    {
        if (activeMenu != null) activeMenu.Close();
        activeMenu = null;
        transfer = false;
        horizontalRepeat.Reset();
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
        int horizontal = transfer ? horizontalRepeat.Poll(65) : 0;
        if (transfer && horizontal != 0) Change(horizontal);
        else if (JustPressed(172)) { horizontalRepeat.Reset(); activeMenu.Move(-1); }
        else if (JustPressed(173)) { horizontalRepeat.Reset(); activeMenu.Move(1); }
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
