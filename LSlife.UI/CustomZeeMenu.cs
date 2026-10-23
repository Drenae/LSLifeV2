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

        // Bloquer d'abord les commandes GTA (dont l'ouverture du telephone).
        // Lire ensuite les commandes desactivees pour conserver la navigation.
        foreach (int control in new[] { 27, 172, 173, 201, 202 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }

        if (DisabledJustPressed(172)) menu.Move(-1);
        else if (DisabledJustPressed(173)) menu.Move(1);
        else if (DisabledJustPressed(201)) menu.Activate();
        else if (DisabledJustPressed(202)) menu.Close();

        if (Visible) Draw();
    }

    private static bool DisabledJustPressed(int control)
    {
        // IS_DISABLED_CONTROL_JUST_PRESSED : compatible avec les controles
        // desactives et avec la version SHVDN2 du projet.
        return Function.Call<bool>(unchecked((Hash)0x91AEF906BCA88877UL), 2, control);
    }

    // Theme PaperNest : anthracite profond, or champagne et typographie sobre.
    private static void Draw()
    {
        const float x = 0.149f;
        const float width = 0.268f;
        const float top = 0.035f;
        const float left = x - width / 2f;

        // Panneau principal et filet dore (rectangles fins, sans textures externes).
        float height = 0.183f + menu.Entries.Count * 0.052f;
        Rect(x, top + height / 2f, width, height, 13, 16, 20, 238);
        Rect(x, top, width, 0.002f, 194, 156, 88, 255);
        Rect(x, top + height, width, 0.002f, 194, 156, 88, 255);
        Rect(left, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        Rect(left + width, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);

        // Bandeau de marque.
        Rect(x, top + 0.046f, width - 0.004f, 0.090f, 22, 25, 29, 240);
        Rect(left + 0.008f, top + 0.046f, 0.003f, 0.062f, 194, 156, 88, 255);
        TextCenteredVertically("ZEE", left + 0.023f, top, 0.090f, 0.67f, 234, 207, 151, 255);
        TextCenteredVertically("CONTACT  /  SERVICES", left + 0.116f, top, 0.090f, 0.25f, 170, 159, 138, 255);
        Rect(x, top + 0.091f, width - 0.019f, 0.0015f, 171, 133, 73, 235);

        // Informations de progression.
        Rect(x, top + 0.117f, width - 0.013f, 0.045f, 28, 30, 33, 232);
        TextCenteredVertically(menu.Subtitle, left + 0.016f, top + 0.0945f, 0.045f, 0.275f, 226, 206, 167, 255);

        // Liste : selection lumineuse mais sans surcharge.
        for (int i = 0; i < menu.Entries.Count; i++)
        {
            float y = top + 0.162f + i * 0.052f;
            bool selected = i == menu.SelectedIndex;
            Rect(x, y + 0.026f, width - 0.012f, 0.049f,
                selected ? 65 : 25, selected ? 55 : 28,
                selected ? 40 : 32, selected ? 248 : 229);
            if (selected)
            {
                Rect(left + 0.008f, y + 0.026f, 0.0035f, 0.047f, 218, 176, 101, 255);
                Rect(x, y + 0.001f, width - 0.012f, 0.001f, 172, 137, 78, 225);
                Rect(x, y + 0.050f, width - 0.012f, 0.001f, 172, 137, 78, 225);
            }
            TextCenteredVertically(menu.Entries[i].Label, left + 0.019f, y + 0.0015f, 0.049f,
                0.315f, selected ? 255 : 226, selected ? 247 : 227,
                selected ? 226 : 229, 255);
            if (i < 2)
                TextCenteredVertically(">", left + width - 0.027f, y + 0.0015f, 0.049f, 0.33f,
                    204, 168, 102, 255);
        }

    }

    // Centre le texte dans une zone verticale en utilisant la hauteur
    // reelle du caractere calculee par GTA pour la police et l'echelle.
    // Les dimensions des zones peuvent changer sans ajuster les offsets.
    private static void TextCenteredVertically(string value, float x, float zoneTop,
        float zoneHeight, float scale, int r, int g, int b, int a)
    {
        const int font = 0;
        float textHeight = Function.Call<float>(
            unchecked((Hash)0xDB88A37483346780UL), scale, font);
        // GTA ancre DRAW_TEXT au sommet de la ligne de police.
        // Utiliser une compensation negative pour remonter le rendu visuel.
        float textY = zoneTop + (zoneHeight - textHeight) / 2f
                    - textHeight * 0.15f;
        Text(value, x, textY, scale, r, g, b, a);
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
        Function.Call(unchecked((Hash)0x25FBB336DF1804CBUL), "STRING");
        Function.Call(unchecked((Hash)0x6C188BE134E074AAUL), value);
        Function.Call(unchecked((Hash)0xCD015E5BB0D96A57UL), x, y);
    }
}
