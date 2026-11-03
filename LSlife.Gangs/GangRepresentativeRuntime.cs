using System;
using GTA;
using GTA.Math;
using GTA.Native;
using LSlife.Gangs;

namespace LSlife
{
    public partial class LSL
    {
        private void ClearRepresentative()
        {
            if (representativeBlip != null && representativeBlip.Exists()) representativeBlip.Remove();
            representativeBlip = null;
            // PNJ du monde ouvert : ne jamais le supprimer ni modifier son apparence.
            if (representative != null && representative.Exists() && representative.IsAlive &&
                representativeGreetingActive)
                representative.Task.WanderAround();
            representativeGreetingActive = false;
            representative = null;
            representativeGangId = null;
            representativeZone = null;
        }

        private void TickGangRepresentative()
        {
            if (firstRun || player == null || player.Character == null ||
                !player.Character.Exists() || !player.Character.IsAlive) return;
            if (areaIndex < 0 || areaIndex >= areas.Count) return;
            Area area = areas[areaIndex];
            GangOrganization gang;
            if (area == null || area.Gang == null || !GangPolitics.TryGetGang(area.Gang.Name, out gang))
            {
                ClearRepresentative();
                return;
            }
            if (representative != null && (!representative.Exists() || !representative.IsAlive ||
                representativeGangId != gang.Id ||
                !string.Equals(representativeZone, area.Name, StringComparison.OrdinalIgnoreCase) ||
                representative.Position.DistanceTo(player.Character.Position) > 85f))
                ClearRepresentative();

            if (representative == null)
            {
                if (Game.GameTime - representativeSpawnTime < 30000) return;
                representativeSpawnTime = Game.GameTime;
                // Reutiliser un habitant GTA existant : aucun spawn, aucun changement
                // de modele, aucun gel de position, aucune suppression du PNJ.
                string gangModel;
                if (!GangUtilities.TryGetRepresentativeModel(gang.Name, out gangModel)) return;
                int expectedModel = new Model(gangModel).Hash;
                Ped nearest = null;
                float best = 55f;
                foreach (Ped candidate in World.GetNearbyPeds(player.Character, 55f))
                {
                    if (candidate == null || !candidate.Exists() || !candidate.IsAlive ||
                        candidate == player.Character || candidate.IsInVehicle() ||
                        candidate.IsPersistent || candidate.Model.Hash != expectedModel ||
                        (aDealer != null && candidate == aDealer.dPed)) continue;
                    if (!string.Equals(World.GetZoneNameLabel(candidate.Position),
                        area.Name, StringComparison.OrdinalIgnoreCase)) continue;
                    float distance = candidate.Position.DistanceTo(player.Character.Position);
                    if (distance < 12f || distance >= best) continue;
                    best = distance;
                    nearest = candidate;
                }
                if (nearest == null) return;
                representative = nearest;
                representativeGangId = gang.Id;
                representativeZone = area.Name;
            }

            UpdateRepresentativeDebug(gang.Name);
            float playerDistance = representative.Position.DistanceTo(player.Character.Position);
            if (representativeGreetingActive && (playerDistance > 7f || player.Character.IsInVehicle()))
            {
                representativeGreetingActive = false;
                if (representative.Exists() && representative.IsAlive)
                    representative.Task.WanderAround();
            }
            if (!representativeGreetingActive && playerDistance < 4.5f &&
                !player.Character.IsInVehicle() && !representative.IsInCombatAgainst(player.Character))
            {
                representativeGreetingActive = true;
                representative.Task.Wait(20000);
                representative.Task.TurnTo(player.Character);
            }
            if (player.Character.IsInVehicle() || CustomMenuShared.IsAnyGameplayMenuOpen() ||
                representative.Position.DistanceTo(player.Character.Position) > 3f) return;
            // Tests DEBUG : 3 secondes au lieu de 2 minutes en partie normale.
            int cooldown = DEBUG ? 3000 : 120000;
            int lastService;
            if (representativeLastService.TryGetValue(gang.Id, out lastService) &&
                Game.GameTime - lastService < cooldown) return;
            if (!representative.IsHeadtracking(player.Character))
                representative.Task.LookAt(player.Character);
            if ((float)Game.GameTime > displayHelpTimer + 4000f)
            {
                displayHelpTimer = Game.GameTime;
                LsFunctions.DisplayHelpText("~INPUT_CONTEXT~ : parler au gerant de " + gang.Name);
            }
            if (!Game.IsControlJustPressed(2, (GTA.Control)51)) return;
            representativeLastService[gang.Id] = Game.GameTime;
            GangPolitics.ChangePlayerReputation(gang.Id, 5);
            GTA.UI.ShowSubtitle(gang.Name + " : confiance " +
                GangPolitics.GetPlayerReputation(gang.Id) + "/100 (+5).", 3500);
            try { GangPolitics.Save(GangDiplomacySavePath); lastGangDiplomacySave = Game.GameTime; }
            catch (Exception ex) { LsFunctions.DbugString("Diplomatie : sauvegarde : " + ex.Message); }
        }

        private void UpdateRepresentativeDebug(string gangName)
        {
            if (!DEBUG)
            {
                if (representativeBlip != null && representativeBlip.Exists()) representativeBlip.Remove();
                representativeBlip = null;
                return;
            }
            if (representativeBlip != null && representativeBlip.Exists()) return;
            representativeBlip = representative.AddBlip();
            representativeBlip.Name = "Gerant " + gangName + " (DEBUG)";
            representativeBlip.Color = (BlipColor)2;
            representativeBlip.IsShortRange = false;
        }
    }
}
