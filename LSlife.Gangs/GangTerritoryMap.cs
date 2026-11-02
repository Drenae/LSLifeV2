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
        private int territoryMapLastOwnerRefresh;
        private readonly HashSet<string> territoryMapPendingZones =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> TerritoryMapExcludedZones =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "OCEANA", "SANAND", "ISHEIST", "ISHEIST2"
            };
        // Les marqueurs sont un apercu : les limites exactes des quartiers
        // doivent etre obtenues avant toute coloration de surface.
        private static string TerritoryMapDisplayName(string zone)
        {
            switch ((zone ?? "").ToUpperInvariant())
            {
                case "DAVIS": return "Davis";
                case "CHAMH": return "Chamberlain Hills";
                case "STRAW": return "Strawberry";
                case "RANCHO": return "Rancho";
                case "LAMESA": return "La Mesa";
                case "EBURO": return "El Burro Heights";
                case "EAST_V": return "East Vinewood";
                case "CHIL": return "Vinewood Hills";
                case "PBOX": return "Pillbox Hill";
                case "DOWNT": return "Downtown";
                case "MURRI": return "Murrieta Heights";
                case "CYPRE": return "Cypress Flats";
                case "TEXTI": return "Textile City";
                case "LMESA": return "La Mesa";
                default: return zone;
            }
        }


        private void LoadGangTerritoryMap()
        {
            ClearGangTerritoryMap();
            territoryMapPositions.Clear();
            territoryMapPendingZones.Clear();
            territoryMapLastOwnerRefresh = 0;
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
                    if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) ||
                        float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(z))
                        continue;
                    territoryMapPositions[id] = new Vector3(x, y, z);
                }
            }
            catch (Exception ex)
            {
                LsFunctions.DbugString("Carte territoires : chargement : " + ex.Message);
            }
        }

        private void RecordGangTerritoryVisit(string zone, Vector3 position)
        {
            if (string.IsNullOrWhiteSpace(zone) ||
                TerritoryMapExcludedZones.Contains(zone) ||
                territoryMapPositions.ContainsKey(zone) ||
                string.IsNullOrEmpty(GangPolitics.GetTerritoryOwner(zone))) return;
            territoryMapPositions[zone] = position;
            territoryMapPendingZones.Add(zone);
            SaveGangTerritoryMap();
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
                string temporaryPath = TerritoryMapPath + ".tmp";
                string backupPath = TerritoryMapPath + ".bak";
                new XDocument(root).Save(temporaryPath);
                if (File.Exists(TerritoryMapPath))
                {
                    File.Replace(temporaryPath, TerritoryMapPath, backupPath);
                }
                else
                {
                    File.Move(temporaryPath, TerritoryMapPath);
                }
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
            RecordGangTerritoryVisit(currentZone, player.Character.Position);
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
                blip.Name = TerritoryMapDisplayName(entry.Key) + " - " + owner;
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
