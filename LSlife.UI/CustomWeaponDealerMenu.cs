using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Interface du marchand d'armes. Les achats conservent le traitement historique
// (prix, remises, credit, dette, reputation et sauvegarde).
internal static class CustomWeaponDealerMenu
{
    private static CustomMenu menu;
    private static bool waitForRelease;
    private static int lastFrame = -1;
    private static int meleeChoice;
    private static int pistolChoice;
    private static readonly CustomMenuShared.HorizontalRepeat repeat = new CustomMenuShared.HorizontalRepeat();

    public static bool Visible => menu != null && menu.Visible;

    public static void Open()
    {
        if (Visible || LSL.weaponDealer1 == null || !LSL.readyToDeal ||
            LSL.LsMenuPool == null || LSL.LsMenuPool.IsAnyMenuOpen())
            return;
        if (menu == null)
        {
            menu = new CustomMenu("ARMES", "MARCHAND");
            menu.Add("Arme blanche", () => Purchase(WeaponDealerAction.Melee));
            menu.Add("Pistolet", () => Purchase(WeaponDealerAction.Pistol));
            menu.Add("Gilet pare-balles", () => Purchase(WeaponDealerAction.Armour));
            menu.Add("Renvoyer", Dismiss);
        }
        menu.Open();
        waitForRelease = true;
        repeat.Reset();
    }

    public static void Close()
    {
        if (menu != null) menu.Close();
        repeat.Reset();
    }

    private static void Purchase(WeaponDealerAction item)
    {
        if (LSL.weaponDealer1 == null) { Close(); return; }

        bool isMelee = item == WeaponDealerAction.Melee;
        bool isPistol = item == WeaponDealerAction.Pistol;
        bool isAmmo = isPistol && pistolChoice == 3;
        if (isMelee || (isPistol && !isAmmo))
        {
            string weaponName = isMelee ? LSL.meleeWeps[meleeChoice] : LSL.pistolWeps[pistolChoice];
            WeaponHash weapon;
            if (!Enum.TryParse<WeaponHash>(weaponName, true, out weapon))
            {
                GTA.UI.ShowSubtitle("Arme indisponible : " + weaponName, 3000);
                return;
            }
            if (Game.Player.Character.Weapons.HasWeapon(weapon))
            {
                GTA.UI.ShowSubtitle("Tu possedes deja cette arme.", 2500);
                return;
            }
        }

        int moneyBefore = Game.Player.Money;
        int debtBefore = LSL.dealer1Debt;
        int armourBefore = Game.Player.Character.Armor;
        int ammoBefore = Game.Player.Character.Weapons.Current.Ammo;
        // La logique historique conserve le prix, le credit et la sauvegarde.
        WeaponDealerActions.Execute(item, meleeChoice, pistolChoice);
        bool paid = Game.Player.Money < moneyBefore;
        bool credit = LSL.dealer1Debt > debtBefore;
        if (paid || credit)
        {
            LsFunctions.PlayTheSound();
            GTA.UI.ShowSubtitle(credit ? "Achat effectue a credit." : "Achat effectue.", 2500);
        }
        else if (item == WeaponDealerAction.Armour && Game.Player.Character.Armor != armourBefore)
        {
            GTA.UI.ShowSubtitle("Gilet pare-balles obtenu.", 2500);
        }
        else if (isAmmo && Game.Player.Character.Weapons.Current.Ammo != ammoBefore)
        {
            GTA.UI.ShowSubtitle("Munitions obtenues.", 2500);
        }
        else
        {
            GTA.UI.ShowSubtitle("Achat impossible : fonds insuffisants, plafond de credit ou inventaire plein.", 3500);
        }
    }

    private static void Dismiss()
    {
        if (LSL.weaponDealer1 == null) { Close(); return; }
        // Le gestionnaire historique ferme wepMenu : il doit aussi fermer
        // cette interface lors du renvoi du marchand.
        WeaponDealerActions.Execute(WeaponDealerAction.Dismiss, meleeChoice, pistolChoice);
        Close();
    }

    private static void ChangeChoice(int direction)
    {
        if (menu.SelectedIndex == 0 && LSL.melee != null && LSL.meleeWeps.Count > 0)
            meleeChoice = (meleeChoice + direction + LSL.meleeWeps.Count) % LSL.meleeWeps.Count;
        else if (menu.SelectedIndex == 1 && LSL.pistol != null && LSL.pistolWeps.Count > 0)
            pistolChoice = (pistolChoice + direction + LSL.pistolWeps.Count) % LSL.pistolWeps.Count;
    }

    private static bool Pressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);
    private static bool JustPressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);

    public static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (lastFrame == frame) return;
        lastFrame = frame;
        if (!Visible) return;
        if (LSL.weaponDealer1 == null || !LSL.readyToDeal) { Close(); return; }

        foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }
        if (waitForRelease)
        {
            if (!Pressed(201) && !Pressed(202) && !Pressed(51)) waitForRelease = false;
            Draw();
            return;
        }
        if (JustPressed(172)) { menu.Move(-1); repeat.Reset(); }
        else if (JustPressed(173)) { menu.Move(1); repeat.Reset(); }
        else if (JustPressed(202)) { Close(); return; }
        else if (JustPressed(201))
        {
            menu.Activate();
            if (!Visible) return;
        }
        int direction = repeat.Poll(110, 320);
        if (direction != 0) ChangeChoice(direction);
        if (Visible) Draw();
    }

    private static int Price(int index, bool melee)
    {
        var prices = melee ? LSL.meleePrice : LSL.pistolPrice;
        if (index < 0 || index >= prices.Count) return 0;
        return prices[index] - LsFunctions.Discount(prices[index]);
    }

    private static string Choice(int index, bool melee)
    {
        var weapons = melee ? LSL.meleeWeps : LSL.pistolWeps;
        if (index < 0 || index >= weapons.Count) return "";
        if (!melee && index == 3) return "Munitions (+50)";
        return CustomMenuShared.FrenchWeaponName(weapons[index].ToString());
    }

    private static void Draw()
    {
        var layout = new CustomMenuLayout();
        layout.Header("ARMES", "MARCHAND", titleScale: 0.54f);
        layout.Separator(0.004f);
        layout.Row("Arme blanche", menu.SelectedIndex == 0,
            LsFunctions.IntToMoney(Price(meleeChoice, true)) + " $");
        layout.Row("< " + Choice(meleeChoice, true) + " >", false, height: 0.030f);
        layout.Row("Pistolet", menu.SelectedIndex == 1,
            LsFunctions.IntToMoney(Price(pistolChoice, false)) + " $");
        layout.Row("< " + Choice(pistolChoice, false) + " >", false, height: 0.030f);
        layout.Row("Gilet pare-balles", menu.SelectedIndex == 2,
            LsFunctions.IntToMoney(250 - LsFunctions.Discount(250)) + " $");
        layout.Separator(0.005f);
        layout.Row("Renvoyer", menu.SelectedIndex == 3);
        layout.Render();
    }
}
