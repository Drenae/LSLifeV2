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
        private Ped representative;
        private Blip representativeBlip;
        private string representativeGangId;
        private int representativeSpawnTime;
        private int representativeLastService;

        private void TickGangEncounters()
        {
            TickGangRepresentative();
        }

        private void InitializeGangDiplomacy()
        {
            try
            {
                GangPolitics.Load(GangDiplomacySavePath);
                foreach (Area area in areas)
                {
                    if (area.Gang == null) continue;
                    // Migration unique : les quartiers natifs reprennent leur gang
                    // historique. Les prochaines modifications de territoire restent
                    // ensuite sous controle du gameplay et de la sauvegarde.
                    string nativeGang;
                    if (!GangPolitics.NativeTerritoriesMigrated &&
                        GangUtilities.TryGetNativeGang(area.Name, out nativeGang))
                    {
                        GangPolitics.RegisterGang(nativeGang, nativeGang);
                        GangPolitics.SetTerritoryOwner(area.Name, nativeGang);
                    }
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
                GangPolitics.CompleteNativeTerritoryMigration();
                // Les gangs historiques peuvent etre enregistres sans fiches detaillees.
                // Ne pas inventer de nom de dirigeant, de tresorerie ou de membres :
                // ces informations seront alimentees par les rencontres et le gameplay.
                // Les zones sont initialisees avec un gang aleatoire dans Area.Innit().
                // Persister IMMEDIATEMENT leur premier proprietaire, sinon un Reload()
                // avant la sauvegarde differee de 60s change aleatoirement le gang.
                if (GangPolitics.IsDirty)
                    GangPolitics.Save(GangDiplomacySavePath);
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
