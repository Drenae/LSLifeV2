using System;
using GTA;
using GTA.Native;

namespace LSlife;

internal static class CustomPlayerGangMenu
{
    private static CustomMenu menu;
    private static bool returnToZee;
    private static bool waitForRelease;
    private static bool editing;
    private static bool enteringName;
    private static int lastFrame = -1;
    private static string draftName = "";
    private static int draftColor;
    private static readonly string[] ColorNames = { "Or", "Bleu", "Rouge", "Violet", "Vert", "Orange", "Cyan", "Blanc" };
    private static readonly int[,] Colors = {
        {226,180,94}, {85,150,235}, {225,85,85}, {178,110,225},
        {100,200,125}, {240,155,70}, {80,205,210}, {230,230,232}
    };
    internal static PlayerGangStore Store { get; private set; }
    internal static bool Visible => menu != null && menu.Visible;

    internal static void Initialize()
    {
        Store = new PlayerGangStore(PlayerGangStore.DefaultPath);
        Store.Load();
    }

    internal static void Open(bool fromZee = false)
    {
        if (Visible || CustomMenuShared.IsAnyGameplayMenuOpen()) return;
        returnToZee = fromZee;
        ShowHome();
    }

    internal static void Close(bool returnToParent = true)
    {
        menu?.Close();
        if (returnToParent && returnToZee) CustomZeeMenu.Toggle();
    }

    private static void SetMenu(CustomMenu next)
    {
        menu?.Close();
        menu = next;
        menu.Open();
        waitForRelease = true;
    }

    private static void ShowHome()
    {
        editing = false;
        var next = new CustomMenu("MON GANG", "ORGANISATION DU JOUEUR");
        if (Store.IsBlocked)
            next.Add("Recharger le fichier XML", Reload);
        else if (Store.Current == null)
            next.Add("Creer mon gang", BeginEdit);
        else
        {
            next.Add("Modifier l'identite", BeginEdit);
            next.Add("Afficher l'identifiant complet", () => UI.Notify(Store.Current.Id.ToString("D")));
        }
        next.Add("Retour", () => Close());
        SetMenu(next);
    }

    private static void Reload()
    {
        Store.Load();
        if (Store.IsBlocked) ReportError();
        ShowHome();
    }

    private static void BeginEdit()
    {
        editing = true;
        draftName = Store.Current?.Name ?? "";
        draftColor = Store.Current == null ? 0 : Array.IndexOf(PlayerGangProfile.ColorKeys, Store.Current.ColorKey);
        var next = new CustomMenu("MON GANG", Store.Current == null ? "CREATION" : "IDENTITE");
        next.Add("Nom du gang", EnterName);
        next.Add("Couleur distinctive", () => draftColor = (draftColor + 1) % ColorNames.Length);
        next.Add(Store.Current == null ? "Creer le gang" : "Enregistrer", Save);
        next.Add("Annuler", ShowHome);
        SetMenu(next);
    }

    private static void EnterName()
    {
        enteringName = true;
        try
        {
            string result = Game.GetUserInput(draftName, PlayerGangProfile.MaxNameLength);
            if (result == null) return;
            try { draftName = PlayerGangProfile.ValidateName(result); }
            catch (ArgumentException error) { UI.Notify(error.Message); }
        }
        finally { enteringName = false; waitForRelease = true; }
    }

    private static void Save()
    {
        bool created = Store.Current == null;
        bool saved = created ? Store.Create(draftName, PlayerGangProfile.ColorKeys[draftColor])
            : Store.UpdateIdentity(draftName, PlayerGangProfile.ColorKeys[draftColor]);
        waitForRelease = true;
        if (!saved) { ReportError(); return; }
        UI.Notify(created ? "~g~Gang cree et enregistre." : "~g~Identite enregistree.");
        ShowHome();
    }

    private static void ReportError()
    {
        UI.Notify("~r~Gang : " + Store.LastError);
        UI.Notify("Fichier : scripts/LSLife/LSLife_PlayerGang.xml. Aucune modification appliquee.");
    }

    private static bool Pressed(int control) => Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);
    private static bool JustPressed(int control) => Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);

    internal static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (frame == lastFrame) return;
        lastFrame = frame;
        if (!Visible || enteringName) return;
        if (!Game.Player.IsAlive) { Close(false); return; }
        if (Game.IsPaused) { waitForRelease = true; return; }
        foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }
        if (waitForRelease)
        {
            if (!Pressed(201) && !Pressed(202) && !Pressed(174) && !Pressed(175)) waitForRelease = false;
            Draw();
            return;
        }
        if (JustPressed(172)) menu.Move(-1);
        else if (JustPressed(173)) menu.Move(1);
        else if (JustPressed(202)) { if (editing) ShowHome(); else Close(); }
        else if (editing && menu.SelectedIndex == 1 && (JustPressed(174) || JustPressed(175)))
            draftColor = (draftColor + (JustPressed(174) ? -1 : 1) + ColorNames.Length) % ColorNames.Length;
        else if (JustPressed(201)) menu.Activate();
        if (Visible) Draw();
    }

    private static void Draw()
    {
        var layout = new CustomMenuLayout();
        layout.Header("MON GANG", menu.Subtitle);
        if (Store.IsBlocked)
        {
            layout.TextPair("Sauvegarde", "Lecture impossible");
            layout.TextPair("Action", "Verifier le XML / .bak");
        }
        else if (editing || Store.Current != null)
        {
            var profile = Store.Current;
            string name = editing ? draftName : profile.Name;
            int color = editing ? draftColor : Array.IndexOf(PlayerGangProfile.ColorKeys, profile.ColorKey);
            layout.TextPair("Chef", "Vous");
            layout.Row(string.IsNullOrEmpty(name) ? "Nom a renseigner" : name, false, height: 0.037f);
            layout.ColoredTextPair("Couleur", ColorNames[color], Colors[color,0], Colors[color,1], Colors[color,2]);
            if (!editing)
            {
                layout.TextPair("Identifiant", profile.Id.ToString("N").Substring(0, 12));
                layout.TextPair("Membres", "Vous (chef)");
                layout.TextPair("Territoires", "Integration : lot 2");
                layout.TextPair("Dealers", "Rattachement : lot 3");
                layout.TextPair("Tresorerie", "Integration : lot 4");
            }
        }
        else
        {
            layout.TextPair("Organisation", "Aucun gang cree");
            layout.TextPair("Creation", "Sans batiment requis");
        }
        layout.Separator();
        for (int i = 0; i < menu.Entries.Count; i++)
            layout.Row(menu.Entries[i].Label, menu.SelectedIndex == i,
                editing && i == 1 ? "< " + ColorNames[draftColor] + " >" : null);
        layout.Render();
    }
}
