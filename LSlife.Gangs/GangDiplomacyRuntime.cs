using System;
using System.IO;
using System.Linq;
using GTA;
using LSlife.Gangs;

namespace LSlife
{
    public partial class LSL
    {
        private static readonly GangDiplomacy GangPolitics = new GangDiplomacy();
        internal static GangDiplomacy DiplomacyState { get { return GangPolitics; } }
        private const string GangDiplomacySavePath = "scripts\\LSLife\\LSLife_GangDiplomacy.xml";
        private int lastGangDiplomacySave;
        private readonly System.Collections.Generic.Dictionary<string, int> lastGangServiceByGang =
            new System.Collections.Generic.Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private const int GangServiceCooldownMs = 120000;

        // Interactions diplomatiques separees du recrutement des dealers.
        // Aucun recrutement, changement d'employeur ou menu d'embauche n'est modifie.
        private void TickGangEncounters()
        {
            if (firstRun || player == null || player.Character == null ||
                !player.Character.Exists() || !player.Character.IsAlive ||
                player.Character.IsInVehicle() || CustomMenuShared.IsAnyGameplayMenuOpen())
                return;
            if (areaIndex < 0 || areaIndex >= areas.Count) return;
            Area area = areas[areaIndex];
            if (area == null || area.Gang == null) return;
            GangOrganization gang;
            if (!GangPolitics.TryGetGang(area.Gang.Name, out gang)) return;
            AreaDealer intermediary = area.Gang.AreaDealers.FirstOrDefault(d =>
                d != null && d.dPed != null && d.dPed.Exists() && d.dPed.IsAlive &&
                (d.CurrentState == Enums.DStates.TalkToPlayer ||
                 d.CurrentState == Enums.DStates.LookAtPlayer ||
                 d.CurrentState == Enums.DStates.StopForPlayer) &&
                !d.dPed.IsInVehicle() &&
                d.dPed.Position.DistanceTo(player.Character.Position) < 2.5f);
            if (intermediary == null) return;

            // Sprint est reserve a l'embauche; ne jamais intercepter cette commande.
            if (Game.IsControlPressed(2, (GTA.Control)21)) return;
            int last;
            if (lastGangServiceByGang.TryGetValue(gang.Id, out last) &&
                Game.GameTime >= last && Game.GameTime - last < GangServiceCooldownMs)
                return;
            // L'aide d'embauche partage l'affichage GTA : ne pas l'ecraser a chaque frame.
            // La touche E est lue a CHAQUE tick (JustPressed dure une seule frame).
            if ((float)Game.GameTime > displayHelpTimer + 4000f)
            {
                displayHelpTimer = Game.GameTime;
                LsFunctions.DisplayHelpText("~INPUT_SPRINT~ : recrutement  |  ~INPUT_CONTEXT~ : service pour " + gang.Name);
            }
            if (!Game.IsControlJustPressed(2, (GTA.Control)51)) return;
            lastGangServiceByGang[gang.Id] = Game.GameTime;
            GangPolitics.ChangePlayerReputation(gang.Id, 5);
            GTA.UI.ShowSubtitle(gang.Name + " : confiance " +
                GangPolitics.GetPlayerReputation(gang.Id) + "/100 (+5).", 3500);
            try { GangPolitics.Save(GangDiplomacySavePath); lastGangDiplomacySave = Game.GameTime; }
            catch (Exception ex) { LsFunctions.DbugString("Diplomatie : sauvegarde service : " + ex.Message); }
        }

        private void InitializeGangDiplomacy()
        {
            try
            {
                GangPolitics.Load(GangDiplomacySavePath);
                foreach (Area area in areas)
                {
                    if (area.Gang == null) continue;
                    string ownerId = GangPolitics.GetTerritoryOwner(area.Name);
                    if (!string.IsNullOrEmpty(ownerId))
                    {
                        GangOrganization owner;
                        if (GangPolitics.TryGetGang(ownerId, out owner) &&
                            !string.Equals(area.Gang.Name, owner.Name, StringComparison.OrdinalIgnoreCase))
                            area.Gang = new Gang(owner.Name, (int)area.GangPresance * 10000, 50, 20, area.Name);
                    }
                    else
                    {
                        GangPolitics.RegisterGang(area.Gang.Name, area.Gang.Name);
                        GangPolitics.SetTerritoryOwner(area.Name, area.Gang.Name);
                    }
                }
                // Les gangs historiques peuvent etre enregistres sans fiches detaillees.
                // Ne pas inventer de nom de dirigeant, de tresorerie ou de membres :
                // ces informations seront alimentees par les rencontres et le gameplay.
                lastGangDiplomacySave = Game.GameTime;
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Diplomatie : chargement impossible : " + ex.Message);
            }
        }

        private void TickGangDiplomacy()
        {
            if (firstRun || !GangPolitics.IsDirty || Game.GameTime - lastGangDiplomacySave < 60000) return;
            lastGangDiplomacySave = Game.GameTime;
            try { GangPolitics.Save(GangDiplomacySavePath); }
            catch (Exception ex) { LsFunctions.DbugString("Diplomatie : sauvegarde impossible : " + ex.Message); }
        }
    }
}
