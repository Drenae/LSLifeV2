using System;
using GTA;
using GTA.Math;
using GTA.Native;
using LSlife.Gangs;

namespace LSlife
{
    public partial class LSL
    {
        private static string requestedLeaderGangId;

        internal static void RequestLeaderMeeting(string gangId)
        {
            if (!GangPolitics.HasLeaderContact(gangId)) return;
            requestedLeaderGangId = gangId;
            GTA.UI.ShowSubtitle("Rendez-vous demande. Rejoignez le marqueur du chef.", 4000);
        }

        private void ClearLeaderMeeting()
        {
            if (leaderMeetingBlip != null && leaderMeetingBlip.Exists())
                leaderMeetingBlip.Remove();
            leaderMeetingBlip = null;
            if (leaderMeetingPed != null && leaderMeetingPed.Exists())
                leaderMeetingPed.Delete();
            leaderMeetingPed = null;
            if (leaderMeetingEscort != null && leaderMeetingEscort.Exists()) leaderMeetingEscort.Delete();
            if (leaderMeetingCar != null && leaderMeetingCar.Exists()) leaderMeetingCar.Delete();
            leaderMeetingEscort = null;
            leaderMeetingCar = null;
            leaderMeetingExited = false;
            leaderMeetingFinished = false;
            leaderMeetingDepartureTime = 0;
            pendingLeaderGangId = null;
        }

        private void TickLeaderMeeting()
        {
            if (firstRun || player == null || player.Character == null ||
                !player.Character.Exists() || !player.Character.IsAlive) return;

            if (!string.IsNullOrEmpty(requestedLeaderGangId))
            {
                string requested = requestedLeaderGangId;
                requestedLeaderGangId = null;
                ClearLeaderMeeting();
                GangOrganization gang;
                if (!GangPolitics.TryGetGang(requested, out gang)) return;
                Vector3 position = World.GetNextPositionOnSidewalk(
                    player.Character.Position + player.Character.ForwardVector * 40f);
                if (position.DistanceTo(player.Character.Position) < 12f) return;
                var groundZ = new OutputArgument();
                if (!Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD,
                    position.X, position.Y, position.Z + 50f, groundZ, false)) return;
                position.Z = groundZ.GetResult<float>();
                string modelName;
                Model model = GangUtilities.TryGetRepresentativeModel(gang.Id, out modelName)
                    ? new Model(modelName)
                    : LsFunctions.RequestModel(unchecked((PedHash)(-1954728090)));
                leaderMeetingPed = World.CreatePed(model, position);
                if (leaderMeetingPed == null || !leaderMeetingPed.Exists()) return;
                pendingLeaderGangId = gang.Id;
                leaderMeetingPed.IsPersistent = true;
                leaderMeetingPed.FreezePosition = true;
                leaderMeetingBlip = leaderMeetingPed.AddBlip();
                leaderMeetingBlip.Name = "Rendez-vous : " + gang.Name;
                leaderMeetingBlip.Color = (BlipColor)2;
                leaderMeetingBlip.IsShortRange = false;
                leaderMeetingExpires = Game.GameTime + (DEBUG ? 300000 : 600000);
            }

            if (string.IsNullOrEmpty(pendingLeaderGangId)) return;
            if (leaderMeetingPed == null || !leaderMeetingPed.Exists() ||
                !leaderMeetingPed.IsAlive || Game.GameTime >= leaderMeetingExpires)
            {
                ClearLeaderMeeting();
                return;
            }
            if (player.Character.IsInVehicle() || CustomMenuShared.IsAnyGameplayMenuOpen() ||
                leaderMeetingPed.Position.DistanceTo(player.Character.Position) > 3f) return;
            leaderMeetingPed.Task.LookAt(player.Character);
            if ((float)Game.GameTime > displayHelpTimer + 4000f)
            {
                displayHelpTimer = Game.GameTime;
                LsFunctions.DisplayHelpText("~INPUT_CONTEXT~ : rencontrer le chef");
            }
            if (!Game.IsControlJustPressed(2, (GTA.Control)51)) return;
            string gangId = pendingLeaderGangId;
            GangPolitics.MarkLeaderMet(gangId);
            GTA.UI.ShowSubtitle("Premiere rencontre avec le chef de " + gangId + " confirmee.", 4500);
            try { GangPolitics.Save(GangDiplomacySavePath); lastGangDiplomacySave = Game.GameTime; }
            catch (Exception ex) { LsFunctions.DbugString("Rencontre du chef : " + ex.Message); }
            ClearLeaderMeeting();
        }
    }
}
