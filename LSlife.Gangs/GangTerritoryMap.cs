using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LSlife.Gangs;

namespace LSlife
{
    public partial class LSL
    {
        // Phase 1 : points de quartiers decouverts, carte pause uniquement.
        // Une position de visite n'est PAS une frontiere geographique.
        private const string TerritoryMapPath = "scripts\\LSLife\\LSLife_TerritoryMap.xml";
        private readonly Dictionary<string, Vector3> territoryMapPositions =
            new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Blip> territoryMapBlips =
            new Dictionary<string, Blip>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> territoryMapOwners =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private int territoryMapLastCheck;

        private void LoadGangTerritoryMap()
        {
            ClearGangTerritoryMap();
            territoryMapPositions.Clear();
            if (!File.Exists(TerritoryMapPath)) return;
            try
            {
                foreach (XElement zone in XDocument.Load(TerritoryMapPath).Descendants("zone"))
                {
                    string id = (string)zone.Attribute("id");
                    float x, y, z;
                    if (string.IsNullOrWhiteSpace(id) ||
                        !float.TryParse((string)zone.Attribute("x"), NumberStyles.Float,
                            CultureInfo.InvariantCulture, out x) ||
                        !float.TryParse((string)zone.Attribute("y"), NumberStyles.Float,
                            CultureInfo.InvariantCulture, out y) ||
                        !float.TryParse((string)zone.Attribute("z"), NumberStyles.Float,
                            CultureInfo.InvariantCulture, out z)) continue;
                    territoryMapPositions[id] = new Vector3(x, y, z);
                }
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Carte territoires : chargement : " + ex.Message);
            }
        }

        private void SaveGangTerritoryMap()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(TerritoryMapPath));
                var root = new XElement("discoveredTerritoryPositions",
                    new XAttribute("version", 1));
                foreach (var entry in territoryMapPositions)
                {
                    Vector3 v = entry.Value;
                    root.Add(new XElement("zone", new XAttribute("id", entry.Key),
                        new XAttribute("x", v.X.ToString("R", CultureInfo.InvariantCulture)),
                        new XAttribute("y", v.Y.ToString("R", CultureInfo.InvariantCulture)),
                        new XAttribute("z", v.Z.ToString("R", CultureInfo.InvariantCulture))));
                }
                new XDocument(root).Save(TerritoryMapPath);
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Carte territoires : sauvegarde : " + ex.Message);
            }
        }

        private static int TerritoryMapColor(string gangId)
        {
            switch ((gangId ?? "").ToLowerInvariant())
            {
                case "ballas": return 27;
                case "families": return 25;
                case "vagos": return 46;
                case "marabunta grande": return 3;
                case "aztecas": return 26;
                case "the lost mc": return 40;
                default: return 4;
            }
        }

        private void TickGangTerritoryMap()
        {
            if (firstRun || player == null || player.Character == null ||
                !player.Character.Exists() || Game.GameTime - territoryMapLastCheck < 1500) return;
            territoryMapLastCheck = Game.GameTime;
            string currentZone = World.GetZoneNameLabel(player.Character.Position);
            if (!string.IsNullOrWhiteSpace(currentZone) &&
                !territoryMapPositions.ContainsKey(currentZone))
            {
                // On ne conserve que les quartiers connus par la diplomatie.
                if (!string.IsNullOrEmpty(GangPolitics.GetTerritoryOwner(currentZone)))
                {
                    territoryMapPositions[currentZone] = player.Character.Position;
                    SaveGangTerritoryMap();
                }
            }
            foreach (var entry in territoryMapPositions)
            {
                string owner = GangPolitics.GetTerritoryOwner(entry.Key);
                if (string.IsNullOrEmpty(owner))
                {
                    Blip old;
                    if (territoryMapBlips.TryGetValue(entry.Key, out old) && old.Exists())
                        old.Remove();
                    territoryMapBlips.Remove(entry.Key);
                    territoryMapOwners.Remove(entry.Key);
                    continue;
                }
                string previous;
                if (territoryMapBlips.ContainsKey(entry.Key) &&
                    territoryMapOwners.TryGetValue(entry.Key, out previous) && previous == owner &&
                    territoryMapBlips[entry.Key].Exists()) continue;
                Blip existing;
                if (territoryMapBlips.TryGetValue(entry.Key, out existing) && existing.Exists())
                    existing.Remove();
                Blip blip = World.CreateBlip(entry.Value);
                if (blip == null) continue;
                blip.Name = entry.Key + " - " + owner;
                blip.Color = (BlipColor)TerritoryMapColor(owner);
                // Display 2 = carte principale uniquement, pas minimap.
                Function.Call(Hash.SET_BLIP_DISPLAY, blip, 2);
                territoryMapBlips[entry.Key] = blip;
                territoryMapOwners[entry.Key] = owner;
            }
        }

        private void ClearGangTerritoryMap()
        {
            foreach (Blip blip in territoryMapBlips.Values)
                if (blip != null && blip.Exists()) blip.Remove();
            territoryMapBlips.Clear();
            territoryMapOwners.Clear();
        }
    }
}
