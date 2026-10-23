using System;
using System.Drawing;
using GTA;
using GTA.Native;

namespace LSlife;

// Prototype pilote : rendu GTA natif, sans controles graphiques NativeUI.
// Les sous-menus et leurs traitements existants restent sous NativeUI.
internal static class CustomZeeMenu
{
    private static CustomMenu menu;
    private static bool returningFromSubmenu;
    private static bool submenuSeen;

    public static bool Visible => menu != null && menu.Visible;

    public static void Toggle()
    {
        if (Visible) { menu.Close(); return; }
        if (LSL.LsMenuPool.IsAnyMenuOpen()) return;
        if (menu == null) Create();
        menu.Subtitle = "REPUTATION  " + (int)LSL.dealer1Rep +
                        "    |    NIVEAU  " + (int)LsFunctions.CurrentRepLvl(LSL.dealer1Rep);
        menu.Open();
    }

    private static void Create()
    {
        menu = new CustomMenu("ZEE", "");
        menu.Add("Dealer", () => OpenSubmenu(LSL.startDealing));
        menu.Add("Commander de la drogue", () => OpenSubmenu(LSL.OrderDrugs));
        menu.Add("Acheter des armes", () => RunExisting(LSL.buyWeapons));
        menu.Add("Demander a etre depose", () => RunExisting(LSL.buyLift));
        menu.Add("Rembourser la dette", () => RunExisting(LSL.payDebt));
    }

    private static void OpenSubmenu(NativeUI.UIMenu target)
    {
        menu.Close();
        returningFromSubmenu = true;
        submenuSeen = false;
        target.Visible = true;
        target.CurrentSelection = 0;
    }

    private static void RunExisting(NativeUI.UIMenuItem item)
    {
        // Reutilise les traitements metier du menu Zee original.
        LSL.zeeMenu.Visible = true;
        try { LsMenus.OnZeeMenuSelect(LSL.zeeMenu, item, 0); }
        finally { LSL.zeeMenu.Visible = false; }
        menu.Close();
    }

    public static void Tick()
    {
        if (returningFromSubmenu)
        {
            if (LSL.LsMenuPool.IsAnyMenuOpen()) submenuSeen = true;
            else if (submenuSeen)
            {
                returningFromSubmenu = false;
                submenuSeen = false;
                // Retour au menu pilote apres fermeture du sous-menu NativeUI.
                if (menu != null) menu.Open();
            }
        }

        if (!Visible) return;

        if (Game.IsControlJustPressed(2, (Control)172)) menu.Move(-1);
        if (Game.IsControlJustPressed(2, (Control)173)) menu.Move(1);
        if (Game.IsControlJustPressed(2, (Control)201)) menu.Activate();
        if (Game.IsControlJustPressed(2, (Control)202)) menu.Close();

        for (int control = 172; control <= 173; control++)
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
        Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 201, true);
        Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 202, true);

        if (Visible) Draw();
    }

    private static void Draw()
    {
        const float x = 0.145f;
        const float width = 0.245f;
        const float top = 0.19f;
        Rect(x, top, width, 0.078f, 22, 29, 33, 235);
        Rect(x, top + 0.038f, width, 0.003f, 72, 174, 135, 255);
        Text(menu.Title, x - width / 2 + 0.014f, top - 0.028f, 0.51f, 255, 255, 255, 255);
        Rect(x, top + 0.066f, width, 0.052f, 31, 38, 43, 230);
        Text(menu.Subtitle, x - width / 2 + 0.014f, top + 0.050f, 0.26f, 202, 218, 211, 255);

        for (int i = 0; i < menu.Entries.Count; i++)
        {
            float y = top + 0.118f + i * 0.046f;
            bool selected = i == menu.SelectedIndex;
            Rect(x, y, width, 0.045f, selected ? 48 : 21,
                selected ? 69 : 27, selected ? 62 : 32, selected ? 245 : 215);
            if (selected) Rect(x - width / 2 + 0.002f, y, 0.004f, 0.045f, 72, 174, 135, 255);
            Text(menu.Entries[i].Label, x - width / 2 + 0.014f, y - 0.015f,
                0.31f, 245, 247, 246, 255);
        }

        float footer = top + 0.118f + menu.Entries.Count * 0.046f + 0.022f;
        Rect(x, footer, width, 0.041f, 22, 29, 33, 235);
        Text("VALIDER : ENTREE     RETOUR : ECHAP", x - width / 2 + 0.014f,
            footer - 0.012f, 0.23f, 181, 195, 190, 255);
    }

    private static void Rect(float x, float y, float w, float h, int r, int g, int b, int a)
    {
        Function.Call(Hash.DRAW_RECT, x, y, w, h, r, g, b, a);
    }

    private static void Text(string value, float x, float y, float scale, int r, int g, int b, int a)
    {
        Function.Call(Hash.SET_TEXT_FONT, 0);
        Function.Call(Hash.SET_TEXT_SCALE, 0f, scale);
        Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
        Function.Call(Hash.SET_TEXT_OUTLINE);
        Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
        Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, value);
        Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y);
    }
}
