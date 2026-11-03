using GTA.Native;

namespace LSlife;

internal static class CustomGangNetworkMenu
{
    internal const string ContactName = "Réseau Gangs";

    private static readonly int[] BlockedControls =
    {
        27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264
    };

    private static CustomMenu root;
    private static CustomMenu gangs;
    private static CustomMenu activeMenu;

    internal static bool Visible => activeMenu != null && activeMenu.Visible;

    internal static void Toggle()
    {
        if (Visible)
        {
            Close();
            return;
        }

        if (CustomMenuShared.IsAnyGameplayMenuOpen()) return;
        if (root == null) Create();
        Open(root);
    }

    private static void Create()
    {
        root = new CustomMenu(ContactName, "");
        gangs = new CustomMenu("Gangs", "");
        gangs.Add("Aucun gang découvert", null, () => false);
        gangs.Add("Retour", () => Open(root));

        root.Add("Gangs", () => Open(gangs));
        root.Add("Cartel", null, () => false);
        root.Add("", null, () => false);
        root.Add("Créer mon gang", null, () => false);
        root.Add("", null, () => false);
        root.Add("Fermer", Close);
    }

    private static void Open(CustomMenu target)
    {
        activeMenu?.Close();
        activeMenu = target;
        activeMenu.Open();
    }

    private static void Close()
    {
        activeMenu?.Close();
        activeMenu = null;
    }

    internal static void Tick()
    {
        if (!Visible) return;

        foreach (int control in BlockedControls)
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }

        if (DisabledJustPressed(172)) activeMenu.Move(-1);
        else if (DisabledJustPressed(173)) activeMenu.Move(1);
        else if (DisabledJustPressed(201)) activeMenu.Activate();
        else if (DisabledJustPressed(202))
        {
            if (activeMenu == root) Close();
            else Open(root);
        }

        if (!Visible) return;
        var layout = new CustomMenuLayout();
        layout.Header(activeMenu.Title);
        for (int index = 0; index < activeMenu.Entries.Count; index++)
        {
            var entry = activeMenu.Entries[index];
            if (entry.Label.Length == 0) layout.Separator();
            else layout.Row(entry.Label, index == activeMenu.SelectedIndex, enabled: entry.Enabled);
        }
        layout.Render();
    }

    private static bool DisabledJustPressed(int control)
    {
        return Function.Call<bool>(unchecked((Hash)0x91AEF906BCA88877UL), 2, control);
    }
}
