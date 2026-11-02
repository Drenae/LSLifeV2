using System;
using GTA;
using LSlife.Gangs;

namespace LSlife
{
    // Premiere phase de conquete : occupation persistante sur place.
    // Aucun PNJ ni combat artificiel ; les affrontements seront ajoutes separement.
    internal static class GangTerritoryCapture
    {
        private const int CaptureDuration = 90000;
        private static string zone;
        private static string attackingGang;
        private static string originalOwner;
        private static int startedAt;
        private static int lastNotice;
        private const int RetryDelay = 120000;
        private static readonly System.Collections.Generic.Dictionary<string, int> retryAfter =
            new System.Collections.Generic.Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        internal static bool IsActive { get { return !string.IsNullOrEmpty(zone); } }

        internal static string Requirement(string target)
        {
            var state = LSL.DiplomacyState;
            if (string.IsNullOrWhiteSpace(target)) return "Quartier GTA inconnu.";
            if (string.IsNullOrWhiteSpace(state.PlayerGangId))
                return "Vous devez appartenir a un gang.";
            if (state.PlayerRank != PlayerGangRank.Leader)
                return "Seul le chef peut lancer une prise de territoire.";
            if (IsActive) return "Une prise de territoire est deja en cours.";
            int next;
            if (retryAfter.TryGetValue(target, out next) && Game.GameTime < next)
                return "Quartier en alerte : patientez " + Math.Max(1, (next - Game.GameTime + 999) / 1000) + " s.";
            string owner = state.GetTerritoryOwner(target);
            if (string.Equals(owner, state.PlayerGangId, StringComparison.OrdinalIgnoreCase))
                return "Ce quartier appartient deja a votre gang.";
            if (!string.IsNullOrWhiteSpace(owner) && state.AreAllied(state.PlayerGangId, owner))
                return "Impossible de conquerir le territoire d'un allie.";
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists() || player.IsDead || player.IsInVehicle())
                return "Vous devez etre vivant et a pied dans le quartier.";
            return null;
        }

        internal static string Start(string target)
        {
            string refusal = Requirement(target);
            if (refusal != null) return refusal;
            if (!string.Equals(target, World.GetZoneNameLabel(Game.Player.Character.Position),
                StringComparison.OrdinalIgnoreCase))
                return "Vous avez quitte le quartier.";
            zone = target;
            attackingGang = LSL.DiplomacyState.PlayerGangId;
            originalOwner = LSL.DiplomacyState.GetTerritoryOwner(target);
            startedAt = Game.GameTime;
            lastNotice = startedAt;
            return null;
        }

        internal static void Cancel()
        {
            zone = null;
            attackingGang = null;
            originalOwner = null;
            startedAt = 0;
            lastNotice = 0;
        }

        internal static void Tick()
        {
            if (!IsActive) return;
            var state = LSL.DiplomacyState;
            Ped player = Game.Player.Character;
            if (player == null || !player.Exists() || player.IsDead || player.IsInVehicle() ||
                state.PlayerRank != PlayerGangRank.Leader ||
                !string.Equals(state.PlayerGangId, attackingGang, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(World.GetZoneNameLabel(player.Position), zone, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(state.GetTerritoryOwner(zone), originalOwner, StringComparison.OrdinalIgnoreCase))
            {
                retryAfter[zone] = Game.GameTime + RetryDelay;
                GTA.UI.ShowSubtitle("Prise de territoire echouee : quartier en alerte 2 minutes.", 4500);
                Cancel();
                return;
            }
            int elapsed = Game.GameTime - startedAt;
            if (elapsed < CaptureDuration)
            {
                if (Game.GameTime - lastNotice >= 15000)
                {
                    lastNotice = Game.GameTime;
                    GTA.UI.ShowSubtitle("Controle de " + zone + " : " +
                        Math.Min(100, elapsed * 100 / CaptureDuration) + "%.", 2500);
                }
                return;
            }
            string conquered = zone;
            try
            {
                state.SetTerritoryOwner(conquered, attackingGang);
                GangOrganization newOwner;
                if (state.TryGetGang(attackingGang, out newOwner))
                {
                    foreach (Area area in LSL.areas)
                    {
                        if (area == null || !string.Equals(area.Name, conquered, StringComparison.OrdinalIgnoreCase) || area.Gang == null) continue;
                        area.Gang = new Gang(newOwner.Name, (int)area.GangPresance * 10000, 50, 20, area.Name);
                    }
                }
                state.Save("scripts\\LSLife\\LSLife_GangDiplomacy.xml");
                GTA.UI.ShowSubtitle("Territoire conquis : " + conquered + ".", 5000);
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Conquete : sauvegarde impossible : " + ex.Message);
                GTA.UI.ShowSubtitle("Territoire modifie, sauvegarde a verifier.", 4500);
            }
            Cancel();
        }
    }
}
