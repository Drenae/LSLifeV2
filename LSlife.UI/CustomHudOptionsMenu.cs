using GTA;
using GTA.Native;

namespace LSlife;

// Réglages du HUD sans UIMenu, sliders ni MenuPool NativeUI.
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
        if (menu == null)
        {
            menu = new CustomMenu("HUD", "POSITION / OPTIONS");
            menu.Add("Stocks - position X", () => { });
            menu.Add("Stocks - position Y", () => { });
            menu.Add("Pression - position X", () => { });
            menu.Add("Pression - position Y", () => { });
            menu.Add("DEBUG - position X", () => { });
            menu.Add("DEBUG - position Y", () => { });
            menu.Add("Enregistrer", Save);
            menu.Add("Retour", Close);
        }
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

        if (menu.SelectedIndex <= 5)
        {
            int direction = repeat.Poll(110, 320);
            if (direction != 0)
            {
                switch (menu.SelectedIndex)
                {
                    case 0: Hud.Move(direction * 10, 0); break;
                    case 1: Hud.Move(0, direction * 10); break;
                    case 2: Hud.MoveBars(direction * 10, 0); break;
                    case 3: Hud.MoveBars(0, direction * 10); break;
                    case 4: Hud.MoveDebug(direction * 10, 0); break;
                    case 5: Hud.MoveDebug(0, direction * 10); break;
                }
            }
        }
        else repeat.Reset();

        Draw();
    }

    private static void Draw()
    {
        var layout = new CustomMenuLayout();
        layout.Header("HUD", "POSITION / OPTIONS");
        layout.Separator(0.005f);
        layout.TextPair("Coordonnees", "1920 x 1080");
        layout.Row("Stocks X", menu.SelectedIndex == 0, Hud.PositionX.ToString());
        layout.Row("Stocks Y", menu.SelectedIndex == 1, Hud.PositionY.ToString());
        layout.Row("Pression X", menu.SelectedIndex == 2, Hud.BarsPositionX.ToString());
        layout.Row("Pression Y", menu.SelectedIndex == 3, Hud.BarsPositionY.ToString());
        layout.Row("DEBUG X", menu.SelectedIndex == 4, Hud.DebugX.ToString());
        layout.Row("DEBUG Y", menu.SelectedIndex == 5, Hud.DebugY.ToString());
        layout.Block(0.032f, y =>
            CustomMenuRenderer.TextCenteredVertically("Gauche / droite : deplacer par 10 pixels",
                layout.Left + 0.019f, y, 0.032f, 0.245f, 175, 177, 182, 255));
        layout.Separator(0.004f);
        layout.Row("Enregistrer", menu.SelectedIndex == 6);
        layout.Row("Retour", menu.SelectedIndex == 7);
        layout.Render();
    }
}
