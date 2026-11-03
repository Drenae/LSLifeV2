using GTA;
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
    private static System.Action back;
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
        ShowRoot();
    }

    private static void ShowRoot()
    {
        var gang = PlayerGangOrganization.Current;
        root = new CustomMenu(ContactName, "");
        root.Add("Gangs", ShowGangs);
        root.Add("Cartel", null, () => false);
        root.Add("", null, () => false);
        if (gang == null)
            root.Add("Créer mon gang", ShowFounding);
        else
            root.Add("Gérer mon gang", ShowOrganization);
        root.Add("", null, () => false);
        root.Add("Fermer", Close);
        Open(root, Close);
    }

    private static void ShowGangs()
    {
        var menu = new CustomMenu("Gangs", "");
        foreach (string gang in GangDiscovery.KnownGangs())
            menu.Add(gang, () => ShowGang(gang));

        if (menu.Entries.Count == 0)
            menu.Add("Aucun gang découvert", null, () => false);
        menu.Add("Retour", ShowRoot);
        Open(menu, ShowRoot);
    }

    private static void ShowGang(string gang)
    {
        var menu = new CustomMenu(gang, "FICHE");
        menu.Add("Chef : identité inconnue", null, () => false);
        menu.Add("Territoires de référence", null, () => false);
        foreach (string zone in GangDiscovery.Territories(gang))
            menu.Add(zone, null, () => false);
        menu.Add("Retour", ShowGangs);
        Open(menu, ShowGangs);
    }

    private static void ShowFounding()
    {
        var menu = new CustomMenu("Créer mon gang", "");
        menu.Add("Nom : " + PlayerGangOrganization.DefaultName, null, () => false);
        menu.Add("Créer et devenir chef", () =>
        {
            if (PlayerGangOrganization.Create()) ShowOrganization();
        }, () => PlayerGangOrganization.CanWrite);
        menu.Add("Annuler", ShowRoot);
        Open(menu, ShowRoot);
    }

    private static void ShowOrganization()
    {
        var gang = PlayerGangOrganization.Current;
        var menu = new CustomMenu(gang.Name, "MON GANG");
        menu.Add("Trésorerie : " + gang.Balance + " $", null, () => false);
        menu.Add("Effectif : " + (1 + gang.Dealers + gang.Soldiers), null, () => false);
        menu.Add("Verser 5 000 $", Deposit);
        menu.Add("Recrutement", ShowRecruitment);
        menu.Add("Quitter mon gang", ShowLeaveConfirmation);
        menu.Add("Retour", ShowRoot);
        Open(menu, ShowRoot);
    }

    private static void Deposit()
    {
        const int amount = 5000;
        var player = Game.Player;
        if (player.Money < amount)
        {
            GTA.UI.ShowSubtitle("Fonds personnels insuffisants.", 3500);
            return;
        }

        if (!PlayerGangOrganization.Deposit(amount)) return;
        player.Money -= amount;
        ShowOrganization();
    }

    private static void ShowRecruitment()
    {
        var gang = PlayerGangOrganization.Current;
        var menu = new CustomMenu("Recrutement", "");
        menu.Add("Dealers : " + gang.Dealers, null, () => false);
        menu.Add("Soldats : " + gang.Soldiers, null, () => false);
        menu.Add("Recruter un dealer : 10 000 $", () => Recruit(false));
        menu.Add("Recruter un soldat : 15 000 $", () => Recruit(true));
        menu.Add("Retour", ShowOrganization);
        Open(menu, ShowOrganization);
    }

    private static void Recruit(bool soldier)
    {
        if (PlayerGangOrganization.Recruit(soldier)) ShowRecruitment();
    }

    private static void ShowLeaveConfirmation()
    {
        var menu = new CustomMenu("Quitter mon gang", "");
        menu.Add("La caisse et les effectifs seront supprimés", null, () => false);
        menu.Add("Annuler", ShowOrganization);
        menu.Add("Confirmer le départ", () =>
        {
            if (PlayerGangOrganization.Leave()) ShowRoot();
        });
        Open(menu, ShowOrganization);
    }

    private static void Open(CustomMenu target, System.Action returnAction)
    {
        activeMenu?.Close();
        activeMenu = target;
        back = returnAction;
        activeMenu.Open();
    }

    private static void Close()
    {
        activeMenu?.Close();
        activeMenu = null;
        back = null;
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
            back();

        if (!Visible) return;
        var layout = new CustomMenuLayout();
        layout.Header(activeMenu.Title, activeMenu.Subtitle);
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
