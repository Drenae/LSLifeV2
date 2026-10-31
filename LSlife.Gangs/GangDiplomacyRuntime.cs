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
                    GangOrganization gang = GangPolitics.RegisterGang(area.Gang.Name, area.Gang.Name);
                    gang.Territories.Add(area.Name);
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
            if (firstRun || Game.GameTime - lastGangDiplomacySave < 60000) return;
            lastGangDiplomacySave = Game.GameTime;
            try { GangPolitics.Save(GangDiplomacySavePath); }
            catch (Exception ex) { LsFunctions.DbugString("Diplomatie : sauvegarde impossible : " + ex.Message); }
        }
    }
}
