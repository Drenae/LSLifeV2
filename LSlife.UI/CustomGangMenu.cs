using System;
using System.Linq;
using GTA;
using LSlife.Gangs;

namespace LSlife
{
    // Consultation des gangs depuis le menu Zee : aucune alliance automatique.
    internal static class CustomGangMenu
    {
        private static CustomMenu overview;
        private static CustomMenu detail;
        private static CustomMenu relations;
        private static int page;
        private static string selectedGang;
        private const int PageSize = 5;
        internal static bool IsGangMenu(CustomMenu menu)
        {
            return menu != null && (menu == overview || menu == detail || menu == relations);
        }

        internal static CustomMenu Open()
        {
            page = 0;
            return BuildOverview();
        }

        internal static CustomMenu Back(CustomMenu current)
        {
            if (current == relations) return BuildDetail();
            if (current == detail) return BuildOverview();
            return null;
        }

        private static CustomMenu BuildOverview()
        {
            var state = LSL.DiplomacyState;
            var gangs = state.Organizations.ToList();
            int pages = Math.Max(1, (gangs.Count + PageSize - 1) / PageSize);
            page = Math.Max(0, Math.Min(page, pages - 1));
            overview = new CustomMenu("GANGS", "ORGANISATIONS  " + (page + 1) + "/" + pages);
            foreach (GangOrganization gang in gangs.Skip(page * PageSize).Take(PageSize))
            {
                string id = gang.Id;
                overview.Add(gang.Name + " (" + gang.Territories.Count + " zones)", () =>
                {
                    selectedGang = id;
                    CustomZeeMenu.ShowGangSubmenu(BuildDetail());
                });
            }
            if (page > 0) overview.Add("Page precedente", () => { page--; CustomZeeMenu.ShowGangSubmenu(BuildOverview()); });
            if (page + 1 < pages) overview.Add("Page suivante", () => { page++; CustomZeeMenu.ShowGangSubmenu(BuildOverview()); });
            overview.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(null));
            return overview;
        }

        private static CustomMenu BuildDetail()
        {
            GangOrganization gang;
            if (!LSL.DiplomacyState.TryGetGang(selectedGang, out gang)) return BuildOverview();
            detail = new CustomMenu(gang.Name, "TERRITOIRES / DIPLOMATIE");
            detail.Add("Territoires : " + gang.Territories.Count, () => { });
            detail.Add("Membres : " + gang.Members, () => { });
            detail.Add("Dirigeant : " + (string.IsNullOrEmpty(gang.LeaderName) ? "Non defini" : gang.LeaderName), () => { });
            detail.Add("Relations", () => CustomZeeMenu.ShowGangSubmenu(BuildRelations()));
            detail.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildOverview()));
            return detail;
        }

        private static CustomMenu BuildRelations()
        {
            var state = LSL.DiplomacyState;
            var gangs = state.Organizations.Where(g => !string.Equals(g.Id, selectedGang, StringComparison.OrdinalIgnoreCase)).ToList();
            relations = new CustomMenu("RELATIONS", selectedGang);
            foreach (GangOrganization gang in gangs.Take(5))
            {
                string id = gang.Id;
                GangRelation relation = state.GetRelation(selectedGang, id);
                relations.Add(gang.Name + " : " + relation.Trust + "/100", () =>
                    GTA.UI.ShowSubtitle("Relation : " + state.Get(selectedGang, id) +
                        " | Confiance : " + state.GetRelation(selectedGang, id).Trust + "/100", 3500));
            }
            relations.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildDetail()));
            return relations;
        }
    }
}
