using System;
using GTA;
using GTA.Native;
using LSlife.Gangs;

namespace LSlife
{
    // Negociations uniquement devant le chef ; aucune action par telephone.
    internal static class CustomLeaderNegotiationMenu
    {
        private static CustomMenu menu;
        private static string gangId;
        private static bool waitForRelease;
        private static bool awaitingConfirmation;
        private static bool awaitingMembershipConfirmation;
        private static int lastFrame = -1;

        internal static bool Visible { get { return menu != null && menu.Visible; } }

        internal static void Open(string targetGangId)
        {
            if (Visible || string.IsNullOrWhiteSpace(targetGangId) ||
                CustomMenuShared.IsAnyGameplayMenuOpen()) return;
            gangId = targetGangId;
            awaitingConfirmation = false;
            awaitingMembershipConfirmation = false;
            Rebuild();
        }

        private static void Rebuild()
        {
            var state = LSL.DiplomacyState;
            GangOrganization gang;
            if (!state.TryGetGang(gangId, out gang)) { Close(); return; }
            if (menu != null) menu.Close();
            menu = new CustomMenu(gang.Name,
                awaitingMembershipConfirmation ? "ADHESION" :
                awaitingConfirmation ? "CONFIRMATION" : "NEGOCIATIONS");
            if (awaitingMembershipConfirmation)
            {
                menu.Add("Confirmer mon adhesion", ConfirmMembership);
                menu.Add("Annuler", () => { awaitingMembershipConfirmation = false; Rebuild(); });
            }
            else if (awaitingConfirmation)
            {
                menu.Add("Confirmer l'alliance", ConfirmAlliance);
                menu.Add("Annuler", () => { awaitingConfirmation = false; Rebuild(); });
            }
            else
            {
                menu.Add("Proposer une alliance", ProposeAlliance);
                if (string.IsNullOrWhiteSpace(state.PlayerGangId))
                    menu.Add("Rejoindre ce gang", RequestMembership);
                menu.Add("Quitter la rencontre", Close);
            }
            menu.Open();
            waitForRelease = true;
        }

        private static string AllianceRequirement()
        {
            var state = LSL.DiplomacyState;
            if (!state.HasMetLeader(gangId)) return "Rencontrez d'abord ce chef.";
            if (state.GetPlayerReputation(gangId) < GangDiplomacy.AllianceReputation)
                return "Reputation requise : 60/100.";
            if (string.IsNullOrWhiteSpace(state.PlayerGangId))
                return "Vous devez appartenir a un gang.";
            if (string.Equals(state.PlayerGangId, gangId, StringComparison.OrdinalIgnoreCase))
                return "Vous appartenez deja a ce gang.";
            if (state.AreAllied(state.PlayerGangId, gangId))
                return "Vos gangs sont deja allies.";
            if (!state.CanNegotiate(state.PlayerGangId, gangId))
                return "Confiance insuffisante ou relation hostile.";
            if (state.GetRelation(state.PlayerGangId, gangId).Trust < 75)
                return "Confiance diplomatique requise : 75/100.";
            return null;
        }

        private static void RequestMembership()
        {
            var state = LSL.DiplomacyState;
            if (!string.IsNullOrWhiteSpace(state.PlayerGangId))
            {
                GTA.UI.ShowSubtitle("Vous appartenez deja a un gang.", 3500);
                return;
            }
            if (!state.HasMetLeader(gangId) ||
                state.GetPlayerReputation(gangId) < GangDiplomacy.AllianceReputation)
            {
                GTA.UI.ShowSubtitle("Le chef exige une reputation de 60/100.", 4000);
                return;
            }
            awaitingMembershipConfirmation = true;
            Rebuild();
        }

        private static void ConfirmMembership()
        {
            var state = LSL.DiplomacyState;
            if (!string.IsNullOrWhiteSpace(state.PlayerGangId) ||
                !state.HasMetLeader(gangId) ||
                state.GetPlayerReputation(gangId) < GangDiplomacy.AllianceReputation)
            {
                GTA.UI.ShowSubtitle("Adhesion refusee : conditions non remplies.", 4000);
                awaitingMembershipConfirmation = false;
                Rebuild();
                return;
            }
            try
            {
                state.JoinGang(gangId, PlayerGangRank.Recruit);
                state.Save("scripts\\LSLife\\LSLife_GangDiplomacy.xml");
                GTA.UI.ShowSubtitle("Vous avez rejoint " + gangId + " comme recrue.", 5000);
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Adhesion gang : " + ex.Message);
                GTA.UI.ShowSubtitle("Adhesion non confirmee, sauvegarde a verifier.", 4500);
            }
            awaitingMembershipConfirmation = false;
            Rebuild();
        }

        private static void ProposeAlliance()
        {
            string refusal = AllianceRequirement();
            if (refusal != null)
            {
                GTA.UI.ShowSubtitle(refusal, 4500);
                return;
            }
            awaitingConfirmation = true;
            Rebuild();
        }

        private static void ConfirmAlliance()
        {
            string refusal = AllianceRequirement();
            if (refusal != null)
            {
                GTA.UI.ShowSubtitle(refusal, 4500);
                awaitingConfirmation = false;
                Rebuild();
                return;
            }
            var state = LSL.DiplomacyState;
            string playerGang = state.PlayerGangId;
            // Revalider au moment du clic pour ne pas negocier un accord tiers.
            if (!state.CanRequestAlliance(gangId) ||
                !state.ProposeAgreement(playerGang, gangId, GangAgreementType.MutualDefense))
            {
                GTA.UI.ShowSubtitle("Le chef refuse cette alliance.", 4000);
                return;
            }
            try
            {
                state.Save("scripts\\LSLife\\LSLife_GangDiplomacy.xml");
                GTA.UI.ShowSubtitle("Alliance conclue avec " + gangId + ".", 5000);
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Alliance : sauvegarde impossible : " + ex.Message);
                GTA.UI.ShowSubtitle("Alliance conclue, sauvegarde a verifier.", 4500);
            }
            awaitingConfirmation = false;
            Rebuild();
        }

        internal static void Close()
        {
            if (menu != null) menu.Close();
            gangId = null;
            awaitingConfirmation = false;
            awaitingMembershipConfirmation = false;
        }

        internal static void TickOncePerFrame()
        {
            if (!Visible || lastFrame == Game.GameTime) return;
            lastFrame = Game.GameTime;
            foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
            {
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
            }
            if (waitForRelease)
            {
                if (!Pressed(201) && !Pressed(202)) waitForRelease = false;
            }
            else if (JustPressed(172)) menu.Move(-1);
            else if (JustPressed(173)) menu.Move(1);
            else if (JustPressed(201)) menu.Activate();
            else if (JustPressed(202)) Close();
            if (!Visible) return;
            var state = LSL.DiplomacyState;
            var layout = new CustomMenuLayout();
            layout.Header("CHEF DE GANG", "NEGOCIATIONS");
            layout.Separator(0.004f);
            layout.TextPair("Gang", gangId);
            if (!string.IsNullOrWhiteSpace(state.PlayerGangId))
            {
                GangOrganization mine;
                if (state.TryGetGang(state.PlayerGangId, out mine))
                    layout.TextPair("Votre gang", mine.Name);
            }
            layout.TextPair("Reputation", state.GetPlayerReputation(gangId) + "/100");
            if (!string.IsNullOrWhiteSpace(state.PlayerGangId) &&
                !string.Equals(state.PlayerGangId, gangId, StringComparison.OrdinalIgnoreCase))
                layout.TextPair("Confiance", state.GetRelation(state.PlayerGangId, gangId).Trust + "/100");
            layout.Separator(0.004f);
            if (awaitingMembershipConfirmation)
            {
                layout.Row("Confirmer mon adhesion", menu.SelectedIndex == 0);
                layout.Row("Annuler", menu.SelectedIndex == 1);
            }
            else if (awaitingConfirmation)
            {
                layout.Row("Confirmer l'alliance", menu.SelectedIndex == 0);
                layout.Row("Annuler", menu.SelectedIndex == 1);
            }
            else
            {
                layout.Row("Proposer une alliance", menu.SelectedIndex == 0,
                    AllianceRequirement() == null ? "Disponible" : "Verrouille");
                int quitIndex = 1;
                if (string.IsNullOrWhiteSpace(state.PlayerGangId))
                {
                    layout.Row("Rejoindre ce gang", menu.SelectedIndex == 1,
                        state.GetPlayerReputation(gangId) >= GangDiplomacy.AllianceReputation
                            ? "Disponible" : "60 requis");
                    quitIndex = 2;
                }
                layout.Row("Quitter", menu.SelectedIndex == quitIndex);
            }
            layout.Render();
        }

        private static bool Pressed(int control)
        {
            return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);
        }

        private static bool JustPressed(int control)
        {
            return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);
        }
    }
}
