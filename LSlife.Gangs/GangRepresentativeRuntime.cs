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
            if (representative != null && representative.Exists()) representative.Delete();
            representative = null;
            representativeGangId = null;
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
                representative.Position.DistanceTo(player.Character.Position) > 150f))
                ClearRepresentative();

            if (representative == null)
            {
                if (Game.GameTime - representativeSpawnTime < 10000) return;
                representativeSpawnTime = Game.GameTime;
                Vector3 pos = World.GetNextPositionOnSidewalk(
                    player.Character.Position + player.Character.ForwardVector * 25f);
                if (pos.DistanceTo(player.Character.Position) < 10f) return;
                representative = World.CreatePed(
                    LsFunctions.RequestModel(unchecked((PedHash)(-1954728090))), pos);
                if (representative == null || !representative.Exists())
                {
                    representative = null;
                    return;
                }
                representativeGangId = gang.Id;
                representative.IsPersistent = true;
                // Un interlocuteur doit rester accessible : l'IA ne peut plus l'envoyer sur la route.
                representative.FreezePosition = true;
            }

            UpdateRepresentativeDebug(gang.Name);
            if (player.Character.IsInVehicle() || CustomMenuShared.IsAnyGameplayMenuOpen() ||
                representative.Position.DistanceTo(player.Character.Position) > 3f) return;
            // Tests DEBUG : 3 secondes au lieu de 2 minutes en partie normale.
            int cooldown = DEBUG ? 3000 : 120000;
            if (representativeLastService != 0 &&
                Game.GameTime - representativeLastService < cooldown) return;
            if (!representative.IsHeadtracking(player.Character))
                representative.Task.LookAt(player.Character);
            if ((float)Game.GameTime > displayHelpTimer + 4000f)
            {
                displayHelpTimer = Game.GameTime;
                LsFunctions.DisplayHelpText("~INPUT_CONTEXT~ : parler au representant de " + gang.Name);
            }
            if (!Game.IsControlJustPressed(2, (GTA.Control)51)) return;
            representativeLastService = Game.GameTime;
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
            representativeBlip.Name = "Representant " + gangName + " (DEBUG)";
            representativeBlip.Color = (BlipColor)2;
            representativeBlip.IsShortRange = false;
        }
    }
}
