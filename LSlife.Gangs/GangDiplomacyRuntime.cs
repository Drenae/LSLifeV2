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
                    // Reutiliser le proprietaire enregistre au lieu du tirage aleatoire d'Area.
                    GangOrganization previousOwner = null;
                    foreach (string name in GangUtilities.LsLifeGangs.Values.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        GangOrganization candidate;
                        if (GangPolitics.TryGetGang(name, out candidate) &&
                            candidate.Territories.Contains(area.Name))
                        {
                            previousOwner = candidate;
                            break;
                        }
                    }
                    if (previousOwner != null)
                    {
                        if (!string.Equals(area.Gang.Name, previousOwner.Name, StringComparison.OrdinalIgnoreCase))
                            area.Gang = new Gang(previousOwner.Name, (int)area.GangPresance * 10000, 50, 20, area.Name);
                    }
                    else
                    {
                        GangOrganization newOwner = GangPolitics.RegisterGang(area.Gang.Name, area.Gang.Name);
                        newOwner.Territories.Add(area.Name);
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
            if (firstRun || Game.GameTime - lastGangDiplomacySave < 60000) return;
            lastGangDiplomacySave = Game.GameTime;
            try { GangPolitics.Save(GangDiplomacySavePath); }
            catch (Exception ex) { LsFunctions.DbugString("Diplomatie : sauvegarde impossible : " + ex.Message); }
        }
    }
}
