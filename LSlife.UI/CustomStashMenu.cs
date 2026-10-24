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
    private static bool vehicle;
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
            houseMenu.Add("Deposer", () => OpenLegacy(LSL.stashPutMenu));
            houseMenu.Add("Retirer", () => OpenLegacy(LSL.stashTakeMenu));
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
            vehicleMenu.Add("Deposer", () => OpenLegacy(LSL.vehStashPutMenu));
            vehicleMenu.Add("Retirer", () => OpenLegacy(LSL.vehStashTakeMenu));
        }
        activeMenu = vehicleMenu;
        activeMenu.Open();
        waitForRelease = true;
    }

    // Conserver provisoirement les sous-menus et transferts historiques NativeUI.
    private static void OpenLegacy(NativeUI.UIMenu target)
    {
        if (target == null) return;
        Close();
        target.CurrentSelection = 0;
        target.Visible = true;
    }

    public static void Close()
    {
        if (activeMenu != null) activeMenu.Close();
        activeMenu = null;
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
            Draw();
            return;
        }
        if (JustPressed(172)) activeMenu.Move(-1);
        else if (JustPressed(173)) activeMenu.Move(1);
        else if (JustPressed(201)) activeMenu.Activate();
        else if (JustPressed(202)) { Close(); return; }
        Draw();
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
        layout.Separator(0.006f);
        layout.Block(0.031f, y =>
            CustomMenuRenderer.TextCenteredVertically("STOCK ENTREPOSE",
                layout.Left + 0.019f, y, 0.031f, 0.28f, 226, 206, 167, 255));
        if (vehicle)
        {
            // Stock voiture en grammes et en onces : inventaires distincts.
            var car = LSL.pStashVehicle;
            if (car == null) { Close(); return; }
            layout.TextPair("Cannabis", car.drugCarWeed + "g  /  " + car.drugCarWeedOz + "oz");
            layout.TextPair("Crack", car.drugCarCrack + "g  /  " + car.drugCarCrackOz + "oz");
            layout.TextPair("Cocaine", car.drugCarCocain + "g  /  " + car.drugCarCocainOz + "oz");
            layout.TextPair("Argent", LsFunctions.IntToMoney(car.drugCarMoney) + " $");
        }
        else
        {
            var house = LSL.HouseHandler.CurrentHouse;
            if (house == null) { Close(); return; }
            layout.TextPair("Cannabis", house.Weed + "g");
            layout.TextPair("Crack", house.Crack + "g");
            layout.TextPair("Cocaine", house.Cocain + "g");
            layout.TextPair("Argent", LsFunctions.IntToMoney(house.Money) + " $");
        }
        layout.Render();
    }
}
