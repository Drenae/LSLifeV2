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
        private static int lastFrame = -1;

        internal static bool Visible { get { return menu != null && menu.Visible; } }

        internal static void Open(string targetGangId)
        {
            if (Visible || string.IsNullOrWhiteSpace(targetGangId) ||
                CustomMenuShared.IsAnyGameplayMenuOpen()) return;
            gangId = targetGangId;
            Rebuild();
        }

        private static void Rebuild()
        {
            var state = LSL.DiplomacyState;
            GangOrganization gang;
            if (!state.TryGetGang(gangId, out gang)) { Close(); return; }
            if (menu != null) menu.Close();
            menu = new CustomMenu(gang.Name, "NEGOCIATIONS");
            menu.Add("Proposer une alliance", ProposeAlliance);
            menu.Add("Quitter la rencontre", Close);
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

        private static void ProposeAlliance()
        {
            string refusal = AllianceRequirement();
            if (refusal != null)
            {
                GTA.UI.ShowSubtitle(refusal, 4500);
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
            Rebuild();
        }

        internal static void Close()
        {
            if (menu != null) menu.Close();
            gangId = null;
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
            layout.TextPair("Reputation", state.GetPlayerReputation(gangId) + "/100");
            if (!string.IsNullOrWhiteSpace(state.PlayerGangId) &&
                !string.Equals(state.PlayerGangId, gangId, StringComparison.OrdinalIgnoreCase))
                layout.TextPair("Confiance", state.GetRelation(state.PlayerGangId, gangId).Trust + "/100");
            layout.Separator(0.004f);
            layout.Row("Proposer une alliance", menu.SelectedIndex == 0,
                AllianceRequirement() == null ? "Disponible" : "Verrouille");
            layout.Row("Quitter", menu.SelectedIndex == 1);
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
