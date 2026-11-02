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
        private static CustomMenu foundingConfirmation;
        private static CustomMenu territoryControl;
        private static CustomMenu captureConfirmation;
        private const string PlayerFoundedGangId = "PLAYER_ORGANIZATION";
        private static CustomMenu detail;
        private static CustomMenu relations;
        private static CustomMenu relationDetail;
        private static CustomMenu agreements;
        private static string selectedPartner;
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
            return menu != null && (menu == foundingConfirmation || menu == territoryControl || menu == captureConfirmation || menu == overview || menu == detail || menu == relations ||
                menu == territories || menu == cartels || menu == cartelDetail ||
                menu == relationDetail || menu == agreements);
        }

        // Point d'entree destine au contact telephonique d'un chef.
        // L'appelant doit verifier l'acquisition du contact avant d'ouvrir ce menu.
        internal static CustomMenu OpenContact(string gangId)
        {
            if (!LSL.DiplomacyState.HasLeaderContact(gangId)) return null;
            selectedGang = gangId;
            relationPage = 0;
            territoryPage = 0;
            return BuildDetail();
        }

        internal static CustomMenu Open()
        {
            page = 0;
            return BuildOverview();
        }

        internal static CustomMenu Back(CustomMenu current)
        {
            if (current == foundingConfirmation || current == territoryControl) return BuildOverview();
            if (current == captureConfirmation) return BuildTerritoryControl();
            if (current == agreements) return BuildRelationDetail();
            if (current == relationDetail) return BuildRelations();
            if (current == relations || current == territories) return BuildDetail();
            if (current == cartelDetail) return BuildCartels();
            if (current == cartels) return BuildOverview();
            if (current == detail) return null;
            return null;
        }

        private static CustomMenu BuildOverview()
        {
            var state = LSL.DiplomacyState;
            var gangs = state.Organizations.Where(g => state.CanViewGangIntelligence(g.Id)).ToList();
            int pages = Math.Max(1, (gangs.Count + PageSize - 1) / PageSize);
            page = Math.Max(0, Math.Min(page, pages - 1));
            overview = new CustomMenu("GANGS", "ORGANISATIONS  " + (page + 1) + "/" + pages);
            if (gangs.Count == 0) overview.Add("Aucun gang allie", () =>
                GTA.UI.ShowSubtitle("Rencontrez des chefs de gang pour etablir des alliances.", 3500));
            foreach (GangOrganization gang in gangs.Skip(page * PageSize).Take(PageSize))
            {
                string id = gang.Id;
                overview.Add(gang.Name + (state.CanViewGangIntelligence(gang.Id) ? " (" + gang.Territories.Count + " zones)" : " - Inconnu"), () =>
                {
                    selectedGang = id;
                    relationPage = 0;
                    CustomZeeMenu.ShowGangSubmenu(BuildDetail());
                });
            }
            if (page > 0) overview.Add("Page precedente", () => { page--; CustomZeeMenu.ShowGangSubmenu(BuildOverview()); });
            if (page + 1 < pages) overview.Add("Page suivante", () => { page++; CustomZeeMenu.ShowGangSubmenu(BuildOverview()); });
            if (string.IsNullOrWhiteSpace(state.PlayerGangId))
                overview.Add("Fonder mon organisation", () =>
                    CustomZeeMenu.ShowGangSubmenu(BuildFoundingConfirmation()));
            else
            {
                GangOrganization mine;
                if (state.TryGetGang(state.PlayerGangId, out mine))
                    overview.Add("Mon gang : " + mine.Name, () =>
                        GTA.UI.ShowSubtitle("Rang : " + state.PlayerRank +
                            " | Territoires : " + mine.Territories.Count, 4000));
                overview.Add("Controle des territoires", () =>
                    CustomZeeMenu.ShowGangSubmenu(BuildTerritoryControl()));
            }
            overview.Add("Cartels", () => { cartelPage = 0; CustomZeeMenu.ShowGangSubmenu(BuildCartels()); });
            overview.Add("Fermer", () => CustomZeeMenu.Close());
            return overview;
        }


        private static CustomMenu BuildTerritoryControl()
        {
            var state = LSL.DiplomacyState;
            string zone = World.GetZoneNameLabel(Game.Player.Character.Position);
            string owner = state.GetTerritoryOwner(zone);
            GangOrganization ownerGang;
            string ownerName = string.IsNullOrEmpty(owner) ? "Sans proprietaire" :
                state.TryGetGang(owner, out ownerGang) ? ownerGang.Name : owner;
            territoryControl = new CustomMenu("TERRITOIRES", "QUARTIER ACTUEL");
            territoryControl.Add("Quartier : " + zone, null, () => false);
            territoryControl.Add("Proprietaire : " + ownerName, null, () => false);
            territoryControl.Add("Mes territoires : " +
                state.Organizations.Where(g => string.Equals(g.Id, state.PlayerGangId,
                    StringComparison.OrdinalIgnoreCase)).Sum(g => g.Territories.Count),
                null, () => false);
            if (GangTerritoryCapture.IsActive)
                territoryControl.Add("Abandonner la prise en cours", () =>
                {
                    GangTerritoryCapture.Cancel();
                    CustomZeeMenu.ShowGangSubmenu(BuildTerritoryControl());
                });
            else
                territoryControl.Add("Prendre le controle du quartier", () =>
                {
                    string refusal = GangTerritoryCapture.Requirement(zone);
                    if (refusal != null) { GTA.UI.ShowSubtitle(refusal, 4500); return; }
                    CustomZeeMenu.ShowGangSubmenu(BuildCaptureConfirmation());
                });
            territoryControl.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildOverview()));
            return territoryControl;
        }

        private static CustomMenu BuildCaptureConfirmation()
        {
            string zone = World.GetZoneNameLabel(Game.Player.Character.Position);
            captureConfirmation = new CustomMenu("TERRITOIRES", "CONFIRMATION");
            captureConfirmation.Add("Quartier : " + zone, null, () => false);
            captureConfirmation.Add("Lancer la prise de controle", () =>
            {
                string refusal = GangTerritoryCapture.Start(zone);
                GTA.UI.ShowSubtitle(refusal ?? "Restez dans le quartier pendant 90 secondes, a pied.", 5000);
                CustomZeeMenu.ShowGangSubmenu(BuildTerritoryControl());
            });
            captureConfirmation.Add("Annuler", () =>
                CustomZeeMenu.ShowGangSubmenu(BuildTerritoryControl()));
            return captureConfirmation;
        }

        private static CustomMenu BuildFoundingConfirmation()
        {
            foundingConfirmation = new CustomMenu("MON ORGANISATION", "CREATION");
            foundingConfirmation.Add("Nom : Mon organisation", null, () => false);
            foundingConfirmation.Add("Creer et devenir chef", () =>
            {
                var state = LSL.DiplomacyState;
                if (!string.IsNullOrWhiteSpace(state.PlayerGangId))
                {
                    GTA.UI.ShowSubtitle("Vous appartenez deja a un gang.", 3500);
                    CustomZeeMenu.ShowGangSubmenu(BuildOverview());
                    return;
                }
                GangOrganization existing;
                if (state.TryGetGang(PlayerFoundedGangId, out existing))
                {
                    GTA.UI.ShowSubtitle("Cette organisation existe deja.", 3500);
                    CustomZeeMenu.ShowGangSubmenu(BuildOverview());
                    return;
                }
                try
                {
                    state.FoundPlayerGang(PlayerFoundedGangId, "Mon organisation");
                    state.Save("scripts\\LSLife\\LSLife_GangDiplomacy.xml");
                    GTA.UI.ShowSubtitle("Organisation creee. Vous en etes le chef.", 5000);
                }
                catch (Exception ex)
                {
                    LsFunctions.DbugString("Creation organisation : " + ex.Message);
                    GTA.UI.ShowSubtitle("Creation impossible ou sauvegarde non confirmee.", 4000);
                }
                CustomZeeMenu.ShowGangSubmenu(BuildOverview());
            });
            foundingConfirmation.Add("Annuler", () =>
                CustomZeeMenu.ShowGangSubmenu(BuildOverview()));
            return foundingConfirmation;
        }

        private static CustomMenu BuildDetail()
        {
            GangOrganization gang;
            if (!LSL.DiplomacyState.TryGetGang(selectedGang, out gang)) return null;
            var state = LSL.DiplomacyState;
            detail = new CustomMenu(gang.Name, "CONTACT");
            if (!state.HasLeaderContact(gang.Id))
            {
                detail.Add("Renseignements indisponibles", () =>
                    GTA.UI.ShowSubtitle("Rapprochez-vous de ce gang pour obtenir des informations.", 3500));
                detail.Add("Raccrocher", () => CustomZeeMenu.Close());
                return detail;
            }
            int reputation = state.GetPlayerReputation(gang.Id);
            // Les renseignements confidentiels restent invisibles tant que le chef
            // n'a pas ete rencontre. La rencontre physique sera geree separement.
            detail.Add("Demander un rendez-vous", () =>
            {
                LSL.RequestLeaderMeeting(gang.Id);
                CustomZeeMenu.Close();
            });
            if (state.HasMetLeader(gang.Id) && reputation >= GangDiplomacy.TerritoryReputation)
                detail.Add("Territoires", () => CustomZeeMenu.ShowGangSubmenu(BuildTerritories()));
            if (state.HasMetLeader(gang.Id) && reputation >= GangDiplomacy.RelationsReputation)
                detail.Add("Relations", () => CustomZeeMenu.ShowGangSubmenu(BuildRelations()));
            detail.Add("INFORMATIONS", null, () => false);
            detail.Add("Dirigeant : " + (string.IsNullOrWhiteSpace(gang.LeaderName) ?
                "Identite inconnue" : gang.LeaderName), null, () => false);
            detail.Add("Confiance : " + reputation + "/100", null, () => false);
            if (state.HasMetLeader(gang.Id))
                detail.Add("Rencontre : chef deja rencontre", null, () => false);
            detail.Add("Raccrocher", () => CustomZeeMenu.Close());
            return detail;
        }

        private static CustomMenu BuildRelations()
        {
            var state = LSL.DiplomacyState;
            if (!state.CanViewLeaderInformation(selectedGang, GangDiplomacy.RelationsReputation)) return BuildDetail();
            // Ne jamais divulguer la diplomatie entre deux organisations tierces.
            // Un appel ne permet de consulter que la relation du chef avec le gang du joueur.
            var gangs = state.Organizations.Where(g =>
                !string.IsNullOrWhiteSpace(state.PlayerGangId) &&
                string.Equals(g.Id, state.PlayerGangId, StringComparison.OrdinalIgnoreCase) &&
                state.CanViewRelationIntelligence(selectedGang, g.Id)).ToList();
            int pages = Math.Max(1, (gangs.Count + PageSize - 1) / PageSize);
            relationPage = Math.Max(0, Math.Min(relationPage, pages - 1));
            relations = new CustomMenu("RELATIONS", selectedGang + "  " + (relationPage + 1) + "/" + pages);
            if (gangs.Count == 0) relations.Add("Relations confidentielles", () =>
                GTA.UI.ShowSubtitle("Les relations avec les autres gangs ne sont pas encore connues.", 3000));
            foreach (GangOrganization gang in gangs.Skip(relationPage * PageSize).Take(PageSize))
            {
                string id = gang.Id;
                bool known = state.CanViewRelationIntelligence(selectedGang, id);
                GangRelation relation = known ? state.GetRelation(selectedGang, id) : null;
                relations.Add(gang.Name + (known ? " : " + relation.Trust + "/100" : " : Confidentiel"), () =>
                {
                    if (!state.CanViewRelationIntelligence(selectedGang, id))
                    {
                        GTA.UI.ShowSubtitle("Les relations entre gangs tiers restent confidentielles.", 3500);
                        return;
                    }
                    selectedPartner = id;
                    CustomZeeMenu.ShowGangSubmenu(BuildRelationDetail());
                });
            }
            if (relationPage > 0) relations.Add("Page precedente", () => { relationPage--; CustomZeeMenu.ShowGangSubmenu(BuildRelations()); });
            if (relationPage + 1 < pages) relations.Add("Page suivante", () => { relationPage++; CustomZeeMenu.ShowGangSubmenu(BuildRelations()); });
            relations.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildDetail()));
            return relations;
        }
        private static CustomMenu BuildTerritories()
        {
            GangOrganization gang;
            if (!LSL.DiplomacyState.TryGetGang(selectedGang, out gang)) return null;
            if (!LSL.DiplomacyState.CanViewLeaderInformation(selectedGang, GangDiplomacy.TerritoryReputation)) return BuildDetail();
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
            var state = LSL.DiplomacyState;
            var groups = state.Cartels.Where(state.CanViewCartelIntelligence).ToList();
            int pages = Math.Max(1, (groups.Count + PageSize - 1) / PageSize);
            cartelPage = Math.Max(0, Math.Min(cartelPage, pages - 1));
            cartels = new CustomMenu("CARTELS", "ORGANISATIONS  " + (cartelPage + 1) + "/" + pages);
            foreach (GangCartel cartel in groups.Skip(cartelPage * PageSize).Take(PageSize))
            {
                string name = cartel.Name;
                bool known = state.CanViewCartelIntelligence(cartel);
                cartels.Add(known ? name + " (" + cartel.Members.Count + " gangs)" : "Cartel - Informations confidentielles", () =>
                {
                    if (!state.CanViewCartelIntelligence(cartel))
                    {
                        GTA.UI.ShowSubtitle("Vous ne faites pas partie de ce cartel.", 3000);
                        return;
                    }
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
            if (cartel == null || !LSL.DiplomacyState.CanViewCartelIntelligence(cartel)) return BuildCartels();
            cartelDetail = new CustomMenu(cartel.Name, "COMPOSITION DU CARTEL");
            cartelDetail.Add("Direction : " + cartel.LeaderGangId, () => { });
            foreach (string member in cartel.Members.OrderBy(m => m).Take(5))
                cartelDetail.Add(member, () => GTA.UI.ShowSubtitle("Membre du cartel : " + member, 2500));
            cartelDetail.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildCartels()));
            return cartelDetail;
        }

        private static string RelationLabel(GangRelationStatus status)
        {
            switch (status)
            {
                case GangRelationStatus.Hostile: return "Hostile";
                case GangRelationStatus.NonAggression: return "Non-agression";
                case GangRelationStatus.Allied: return "Allie";
                default: return "Neutre";
            }
        }

        private static string AgreementLabel(GangAgreementType agreement)
        {
            switch (agreement)
            {
                case GangAgreementType.NonAggression: return "Non-agression";
                case GangAgreementType.Trade: return "Commerce";
                case GangAgreementType.ArmedSupport: return "Soutien arme";
                case GangAgreementType.MutualDefense: return "Defense mutuelle";
                default: return agreement.ToString();
            }
        }

        private static CustomMenu BuildRelationDetail()
        {
            var state = LSL.DiplomacyState;
            GangOrganization partner;
            if (!state.TryGetGang(selectedPartner, out partner)) return BuildRelations();
            if (!state.CanViewRelationIntelligence(selectedGang, selectedPartner)) return BuildRelations();
            GangRelation relation = state.GetRelation(selectedGang, selectedPartner);
            relationDetail = new CustomMenu("DIPLOMATIE", partner.Name);
            relationDetail.Add("Statut : " + RelationLabel(relation.Status), () => { });
            relationDetail.Add("Confiance : " + relation.Trust + "/100", () => { });
            relationDetail.Add("Accords : " + relation.Agreements.Count,
                () => CustomZeeMenu.ShowGangSubmenu(BuildAgreements()));
            relationDetail.Add(state.CanNegotiate(selectedGang, selectedPartner) ?
                "Negociation possible" : "Negociation indisponible",
                () => GTA.UI.ShowSubtitle("Les negociations necessitent une rencontre avec le dirigeant.", 3500));
            relationDetail.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildRelations()));
            return relationDetail;
        }

        private static CustomMenu BuildAgreements()
        {
            if (!LSL.DiplomacyState.CanViewRelationIntelligence(selectedGang, selectedPartner)) return BuildRelations();
            var relation = LSL.DiplomacyState.GetRelation(selectedGang, selectedPartner);
            agreements = new CustomMenu("ACCORDS", selectedGang + " / " + selectedPartner);
            foreach (GangAgreementType type in Enum.GetValues(typeof(GangAgreementType)))
            {
                bool active = relation.Agreements.Contains(type);
                string name = AgreementLabel(type);
                agreements.Add(name + " : " + (active ? "Actif" : "Inactif"), () =>
                    GTA.UI.ShowSubtitle(name + " : " + (active ? "accord actif" : "aucun accord"), 2500));
            }
            agreements.Add("Retour", () => CustomZeeMenu.ShowGangSubmenu(BuildRelationDetail()));
            return agreements;
        }

    }
}
