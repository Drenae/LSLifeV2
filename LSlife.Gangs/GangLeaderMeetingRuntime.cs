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
            if (!string.IsNullOrEmpty(requestedLeaderGangId) ||
                !string.IsNullOrEmpty(ActiveLeaderMeetingGangId))
            {
                GTA.UI.ShowSubtitle("Un rendez-vous avec un chef est deja en cours.", 4000);
                return;
            }
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
            leaderMeetingDrivingAway = false;
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
                // Reutilise les emplacements de livraison de Zee, deja
                // selectionnes pour des rencontres en voiture hors du centre.
                var possibleLocations = LsFunctions.spawnPostions.FindAll(s =>
                    s.Position.DistanceTo(player.Character.Position) >= 400f &&
                    s.Position.DistanceTo(player.Character.Position) <= 1800f);
                if (possibleLocations.Count == 0)
                {
                    GTA.UI.ShowSubtitle("Aucun lieu de rendez-vous disponible dans ce secteur.", 4000);
                    return;
                }
                DealerSpawnPostion meetingLocation =
                    possibleLocations[rnd.Next(possibleLocations.Count)];
                Vector3 position = meetingLocation.Position;
                // Les positions Zee sont deja au niveau de la chaussee.
                // Ne pas les deplacer verticalement avec un second calcul de sol.
                string modelName;
                Model model = GangUtilities.TryGetRepresentativeModel(gang.Id, out modelName)
                    ? new Model(modelName)
                    : LsFunctions.RequestModel(unchecked((PedHash)(-1954728090)));
                // Comme Zee : vehicule stationne avec deux occupants.
                Vector3 carPosition = meetingLocation.Position;
                leaderMeetingCar = World.CreateVehicle(
                    LsFunctions.RequestModel((VehicleHash)1177543287),
                    carPosition, meetingLocation.Heading);
                if (leaderMeetingCar == null || !leaderMeetingCar.Exists()) return;
                leaderMeetingCar.IsPersistent = true;
                leaderMeetingPed = leaderMeetingCar.CreatePedOnSeat((VehicleSeat)0, model);
                leaderMeetingEscort = leaderMeetingCar.CreatePedOnSeat(
                    unchecked((VehicleSeat)(-1)), model);
                if (leaderMeetingPed == null || !leaderMeetingPed.Exists()) return;
                pendingLeaderGangId = gang.Id;
                leaderMeetingPed.IsPersistent = true;
                leaderMeetingPed.BlockPermanentEvents = true;
                if (leaderMeetingEscort != null) leaderMeetingEscort.BlockPermanentEvents = true;
                leaderMeetingBlip = World.CreateBlip(carPosition);
                leaderMeetingBlip.Name = "Rendez-vous : " + gang.Name;
                leaderMeetingBlip.Color = (BlipColor)2;
                leaderMeetingBlip.IsShortRange = false;
                leaderMeetingBlip.ShowRoute = true;
                leaderMeetingExpires = Game.GameTime + (DEBUG ? 300000 : 600000);
            }

            if (string.IsNullOrEmpty(pendingLeaderGangId)) return;
            if (leaderMeetingPed == null || !leaderMeetingPed.Exists() ||
                !leaderMeetingPed.IsAlive || leaderMeetingCar == null ||
                !leaderMeetingCar.Exists() || leaderMeetingEscort == null ||
                !leaderMeetingEscort.Exists() || !leaderMeetingEscort.IsAlive ||
                (!leaderMeetingFinished && Game.GameTime >= leaderMeetingExpires))
            {
                ClearLeaderMeeting();
                return;
            }
            if (leaderMeetingFinished)
            {
                if (Game.GameTime > leaderMeetingDepartureTime + 45000 ||
                    (leaderMeetingDrivingAway && leaderMeetingCar != null &&
                     leaderMeetingCar.Position.DistanceTo(player.Character.Position) > 150f))
                {
                    ClearLeaderMeeting();
                    return;
                }
                if (!leaderMeetingDrivingAway && leaderMeetingCar != null && leaderMeetingEscort != null &&
                    leaderMeetingEscort.IsInVehicle() && leaderMeetingPed.IsInVehicle())
                {
                    Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER,
                        leaderMeetingEscort, leaderMeetingCar, 17f, 786603);
                    leaderMeetingDrivingAway = true;
                }
                return;
            }
            if (!leaderMeetingExited && leaderMeetingCar != null &&
                leaderMeetingCar.Position.DistanceTo(player.Character.Position) < 18f)
            {
                leaderMeetingExited = true;
                Function.Call(Hash.TASK_LEAVE_VEHICLE, leaderMeetingPed, leaderMeetingCar, 0);
                if (leaderMeetingEscort != null)
                    Function.Call(Hash.TASK_LEAVE_VEHICLE, leaderMeetingEscort, leaderMeetingCar, 0);
            }
            if (!leaderMeetingExited || leaderMeetingPed.IsInVehicle()) return;
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
            leaderMeetingFinished = true;
            leaderMeetingDepartureTime = Game.GameTime;
            if (leaderMeetingPed != null && leaderMeetingCar != null)
                leaderMeetingPed.Task.EnterVehicle(leaderMeetingCar, (VehicleSeat)0);
            if (leaderMeetingEscort != null && leaderMeetingCar != null)
                leaderMeetingEscort.Task.EnterVehicle(leaderMeetingCar, unchecked((VehicleSeat)(-1)));
        }
    }
}
