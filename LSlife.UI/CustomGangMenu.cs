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
        private static CustomMenu territories;
        private static CustomMenu cartels;
        private static CustomMenu cartelDetail;
        private static string selectedCartel;
        private static int territoryPage;
        private static int cartelPage;
        private static int page;
        private static int relationPage;
        private static string selectedGang;
        private const int PageSize = 5;
        internal static bool IsGangMenu(CustomMenu menu)
        {
            return menu != null && (menu == overview || menu == detail || menu == relations ||
                menu == territories || menu == cartels || menu == cartelDetail);
        }

        internal static CustomMenu Open()
        {
            page = 0;
            return BuildOverview();
        }

        internal static CustomMenu Back(CustomMenu current)
        {
            if (current == relations || current == territories) return BuildDetail();
            if (current == cartelDetail) return BuildCartels();
            if (current == cartels) return BuildOverview();
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
                    relationPage = 0;
                    CustomZeeMenu.ShowGangSubmenu(BuildDetail());
                });
            }
            if (page > 0) overview.Add("Page precedente", () => { page--; CustomZeeMenu.ShowGangSubmenu(BuildOverview()); });
            if (page + 1 < pages) overview.Add("Page suivante", () => { page++; CustomZeeMenu.ShowGangSubmenu(BuildOverview()); });
            overview.Add("Cartels", () => { cartelPage = 0; CustomZeeMenu.ShowGangSubmenu(BuildCartels()); });
            overview.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(null));
            return overview;
        }

        private static CustomMenu BuildDetail()
        {
            GangOrganization gang;
            if (!LSL.DiplomacyState.TryGetGang(selectedGang, out gang)) return BuildOverview();
            detail = new CustomMenu(gang.Name, "TERRITOIRES / DIPLOMATIE");
            detail.Add("Territoires : " + gang.Territories.Count, () => { territoryPage = 0; CustomZeeMenu.ShowGangSubmenu(BuildTerritories()); });
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
            int pages = Math.Max(1, (gangs.Count + PageSize - 1) / PageSize);
            relationPage = Math.Max(0, Math.Min(relationPage, pages - 1));
            relations = new CustomMenu("RELATIONS", selectedGang + "  " + (relationPage + 1) + "/" + pages);
            foreach (GangOrganization gang in gangs.Skip(relationPage * PageSize).Take(PageSize))
            {
                string id = gang.Id;
                GangRelation relation = state.GetRelation(selectedGang, id);
                relations.Add(gang.Name + " : " + relation.Trust + "/100", () =>
                    GTA.UI.ShowSubtitle("Relation : " + state.Get(selectedGang, id) +
                        " | Confiance : " + state.GetRelation(selectedGang, id).Trust + "/100", 3500));
            }
            if (relationPage > 0) relations.Add("Page precedente", () => { relationPage--; CustomZeeMenu.ShowGangSubmenu(BuildRelations()); });
            if (relationPage + 1 < pages) relations.Add("Page suivante", () => { relationPage++; CustomZeeMenu.ShowGangSubmenu(BuildRelations()); });
            relations.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildDetail()));
            return relations;
        }
        private static CustomMenu BuildTerritories()
        {
            GangOrganization gang;
            if (!LSL.DiplomacyState.TryGetGang(selectedGang, out gang)) return BuildOverview();
            var zones = gang.Territories.OrderBy(t => t).ToList();
            int pages = Math.Max(1, (zones.Count + PageSize - 1) / PageSize);
            territoryPage = Math.Max(0, Math.Min(territoryPage, pages - 1));
            territories = new CustomMenu("TERRITOIRES", gang.Name + "  " + (territoryPage + 1) + "/" + pages);
            foreach (string zone in zones.Skip(territoryPage * PageSize).Take(PageSize))
                territories.Add(zone, () => GTA.UI.ShowSubtitle("Territoire : " + zone, 2500));
            if (zones.Count == 0) territories.Add("Aucun territoire", () => { });
            if (territoryPage > 0) territories.Add("Page precedente", () => { territoryPage--; CustomZeeMenu.ShowGangSubmenu(BuildTerritories()); });
            if (territoryPage + 1 < pages) territories.Add("Page suivante", () => { territoryPage++; CustomZeeMenu.ShowGangSubmenu(BuildTerritories()); });
            territories.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildDetail()));
            return territories;
        }

        private static CustomMenu BuildCartels()
        {
            var groups = LSL.DiplomacyState.Cartels.ToList();
            int pages = Math.Max(1, (groups.Count + PageSize - 1) / PageSize);
            cartelPage = Math.Max(0, Math.Min(cartelPage, pages - 1));
            cartels = new CustomMenu("CARTELS", "ORGANISATIONS  " + (cartelPage + 1) + "/" + pages);
            foreach (GangCartel cartel in groups.Skip(cartelPage * PageSize).Take(PageSize))
            {
                string name = cartel.Name;
                cartels.Add(name + " (" + cartel.Members.Count + " gangs)", () =>
                {
                    selectedCartel = name;
                    CustomZeeMenu.ShowGangSubmenu(BuildCartelDetail());
                });
            }
            if (groups.Count == 0) cartels.Add("Aucun cartel forme", () => { });
            if (cartelPage > 0) cartels.Add("Page precedente", () => { cartelPage--; CustomZeeMenu.ShowGangSubmenu(BuildCartels()); });
            if (cartelPage + 1 < pages) cartels.Add("Page suivante", () => { cartelPage++; CustomZeeMenu.ShowGangSubmenu(BuildCartels()); });
            cartels.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildOverview()));
            return cartels;
        }

        private static CustomMenu BuildCartelDetail()
        {
            GangCartel cartel = LSL.DiplomacyState.Cartels.FirstOrDefault(c =>
                string.Equals(c.Name, selectedCartel, StringComparison.OrdinalIgnoreCase));
            if (cartel == null) return BuildCartels();
            cartelDetail = new CustomMenu(cartel.Name, "COMPOSITION DU CARTEL");
            cartelDetail.Add("Direction : " + cartel.LeaderGangId, () => { });
            foreach (string member in cartel.Members.OrderBy(m => m).Take(5))
                cartelDetail.Add(member, () => GTA.UI.ShowSubtitle("Membre du cartel : " + member, 2500));
            cartelDetail.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildCartels()));
            return cartelDetail;
        }

    }
}
