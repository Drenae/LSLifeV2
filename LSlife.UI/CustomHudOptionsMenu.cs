using GTA;
using GTA.Native;

namespace LSlife;

// Menu de configuration des trois groupes HUD, sans NativeUI.
// Les lignes informatives et les separateurs ne sont pas selectionnables.
internal static class CustomHudOptionsMenu
{
    private static CustomMenu menu;
    private static bool waitForRelease;
    private static int lastFrame = -1;
    private static readonly CustomMenuShared.HorizontalRepeat repeat = new CustomMenuShared.HorizontalRepeat();

    public static bool Visible => menu != null && menu.Visible;

    public static void Open()
    {
        if (Visible) return;
        // Reconstruire pour tenir compte d'un changement de AREADEBUG a chaud.
        menu = new CustomMenu("HUD", "POSITION / OPTIONS");
        menu.Add("Stock X", () => { });
        menu.Add("Stock Y", () => { });
        menu.Add("Influence et pression X", () => { });
        menu.Add("Influence et pression Y", () => { });
        if (LSL.AreaDebug)
        {
            menu.Add("DEBUG X", () => { });
            menu.Add("DEBUG Y", () => { });
        }
        menu.Add("Enregistrer", Save);
        menu.Add("Retour", Close);
        menu.Open();
        waitForRelease = true;
        repeat.Reset();
    }

    private static void Save()
    {
        Hud.SavePosition();
        GTA.UI.ShowSubtitle("Position du HUD enregistree.", 2500);
        Close();
    }

    public static void Close()
    {
        if (!Visible) return;
        menu.Close();
        repeat.Reset();
        CustomZeeMenu.Toggle();
    }

    private static bool Pressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);

    private static bool JustPressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);

    public static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (frame == lastFrame) return;
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

        if (JustPressed(172)) { menu.Move(-1); repeat.Reset(); }
        else if (JustPressed(173)) { menu.Move(1); repeat.Reset(); }
        else if (JustPressed(202)) { Close(); return; }
        else if (JustPressed(201))
        {
            menu.Activate();
            if (!Visible) return;
        }

        int coordinateRows = LSL.AreaDebug ? 6 : 4;
        if (menu.SelectedIndex < coordinateRows)
        {
            // Premier appui : 1 pixel ; maintien : repetition automatique.
            int direction = repeat.Poll(28, 220);
            if (direction != 0)
            {
                switch (menu.SelectedIndex)
                {
                    case 0: Hud.Move(direction, 0); break;
                    case 1: Hud.Move(0, direction); break;
                    case 2: Hud.MoveBars(direction, 0); break;
                    case 3: Hud.MoveBars(0, direction); break;
                    case 4: Hud.MoveDebug(direction, 0); break;
                    case 5: Hud.MoveDebug(0, direction); break;
                }
            }
        }
        else repeat.Reset();

        Draw();
    }

    private static void Section(CustomMenuLayout layout, string title)
    {
        layout.Separator(0.005f);
        layout.Block(0.029f, y =>
            CustomMenuRenderer.TextCenteredVertically(title, layout.Left + 0.019f,
                y, 0.029f, 0.29f, 218, 176, 101, 255));
    }

    private static void Draw()
    {
        var layout = new CustomMenuLayout();
        layout.Header("HUD", "POSITION / OPTIONS");
        layout.TextPair("Taille ecran", "1920 x 1080", 0.032f);

        Section(layout, "STOCK");
        layout.Row("Position X", menu.SelectedIndex == 0, Hud.PositionX.ToString());
        layout.Row("Position Y", menu.SelectedIndex == 1, Hud.PositionY.ToString());

        Section(layout, "INFLUENCE ET PRESSION");
        layout.Row("Position X", menu.SelectedIndex == 2, Hud.BarsPositionX.ToString());
        layout.Row("Position Y", menu.SelectedIndex == 3, Hud.BarsPositionY.ToString());

        if (LSL.AreaDebug)
        {
            Section(layout, "DEBUG");
            layout.Row("Position X", menu.SelectedIndex == 4, Hud.DebugX.ToString());
            layout.Row("Position Y", menu.SelectedIndex == 5, Hud.DebugY.ToString());
        }

        layout.Separator(0.006f);
        layout.Block(0.029f, y =>
            CustomMenuRenderer.TextCenteredVertically("Maintenir gauche / droite : deplacer (1 px)",
                layout.Left + 0.019f, y, 0.029f, 0.245f, 175, 177, 182, 255));

        int saveIndex = LSL.AreaDebug ? 6 : 4;
        layout.Row("Enregistrer", menu.SelectedIndex == saveIndex);
        layout.Row("Retour", menu.SelectedIndex == saveIndex + 1);
        layout.Render();
    }
}
