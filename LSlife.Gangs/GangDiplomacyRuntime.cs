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
