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
        private readonly Dictionary<string, List<Blip>> territoryMapAreaBlips =
            new Dictionary<string, List<Blip>>(StringComparer.OrdinalIgnoreCase);
        private const int TerritoryMapMaxAreaBlips = 250;
        private int territoryMapLastCheck;
        private bool territoryMapSavePending;
        private int territoryMapLastSaveAttempt;
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
            territoryMapSavePending = false;

            // Recuperer le dernier fichier valide si une ecriture a ete interrompue.
            string[] candidates = { TerritoryMapPath, TerritoryMapPath + ".bak" };
            foreach (string candidate in candidates)
            {
                if (!File.Exists(candidate)) continue;
                try
                {
                    var loaded = new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);
                    XDocument document = XDocument.Load(candidate);
                    if (document.Root == null ||
                        document.Root.Name.LocalName != "discoveredTerritoryPositions")
                        throw new InvalidDataException("Format de carte inconnu.");
                    foreach (XElement zone in document.Root.Elements("zone"))
                    {
                        string id = (string)zone.Attribute("id");
                        float x, y, z;
                        if (string.IsNullOrWhiteSpace(id) ||
                            TerritoryMapExcludedZones.Contains(id) ||
                            !float.TryParse((string)zone.Attribute("x"), NumberStyles.Float,
                                CultureInfo.InvariantCulture, out x) ||
                            !float.TryParse((string)zone.Attribute("y"), NumberStyles.Float,
                                CultureInfo.InvariantCulture, out y) ||
                            !float.TryParse((string)zone.Attribute("z"), NumberStyles.Float,
                                CultureInfo.InvariantCulture, out z) ||
                            float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) ||
                            float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(z))
                            continue;
                        loaded[id] = new Vector3(x, y, z);
                    }
                    foreach (var entry in loaded)
                        territoryMapPositions.Add(entry.Key, entry.Value);
                    if (candidate != TerritoryMapPath)
                    {
                        LsFunctions.DbugString("Carte territoires : restauration depuis .bak.");
                        // Conserver le backup intact jusqu'a la prochaine ecriture.
                        territoryMapSavePending = true;
                    }
                    return;
                }
                catch (Exception ex)
                {
                    LsFunctions.DbugString("Carte territoires : lecture " +
                        candidate + " : " + ex.Message);
                }
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
            territoryMapSavePending = true;
            SaveGangTerritoryMap();
        }

        private void SaveGangTerritoryMap()
        {
            territoryMapLastSaveAttempt = Game.GameTime;
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
                    if (territoryMapSavePending && File.Exists(backupPath))
                    {
                        // La sauvegarde de secours peut etre la seule copie valide :
                        // ne jamais la remplacer par un fichier principal corrompu.
                        bool primaryValid = true;
                        try { XDocument.Load(TerritoryMapPath); }
                        catch { primaryValid = false; }
                        if (!primaryValid)
                        {
                            File.Delete(TerritoryMapPath);
                            File.Move(temporaryPath, TerritoryMapPath);
                        }
                        else File.Replace(temporaryPath, TerritoryMapPath, backupPath);
                    }
                    else File.Replace(temporaryPath, TerritoryMapPath, backupPath);
                }
                else
                {
                    File.Move(temporaryPath, TerritoryMapPath);
                }
                territoryMapSavePending = false;
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

        private void RemoveGangTerritoryArea(string zone)
        {
            List<Blip> blips;
            if (!territoryMapAreaBlips.TryGetValue(zone, out blips)) return;
            foreach (Blip blip in blips)
                if (blip != null && blip.Exists()) blip.Remove();
            territoryMapAreaBlips.Remove(zone);
        }

        private void AddGangTerritoryArea(string zone, string owner)
        {
            IList<GangTerritoryZoneBounds.Rectangle> bounds = GangTerritoryZoneBounds.Get(zone);
            if (bounds == null || bounds.Count == 0) return;
            int active = 0;
            foreach (List<Blip> existing in territoryMapAreaBlips.Values)
                active += existing.Count;
            if (active + bounds.Count > TerritoryMapMaxAreaBlips) return;

            var created = new List<Blip>();
            foreach (GangTerritoryZoneBounds.Rectangle rectangle in bounds)
            {
                float cx = rectangle.X + rectangle.Width / 2f;
                float cy = rectangle.Y + rectangle.Height / 2f;
                // GTA V Legacy : ADD_BLIP_FOR_AREA, disponible par son hash natif.
                Blip area = Function.Call<Blip>((Hash)0xCE5D0E5E315DB238UL,
                    cx, cy, 0f, rectangle.Width, rectangle.Height);
                if (area == null || !area.Exists()) continue;
                area.Color = (BlipColor)TerritoryMapColor(owner);
                Function.Call(Hash.SET_BLIP_ALPHA, area, 65);
                Function.Call(Hash.SET_BLIP_ROTATION, area, 0);
                // 3 : grande carte uniquement ; 2 afficherait aussi la minimap.
                Function.Call(Hash.SET_BLIP_DISPLAY, area, 3);
                created.Add(area);
            }
            if (created.Count > 0) territoryMapAreaBlips[zone] = created;
        }

        private void TickGangTerritoryMap()
        {
            if (firstRun || player == null || player.Character == null ||
                !player.Character.Exists() || Game.GameTime - territoryMapLastCheck < 1500) return;
            territoryMapLastCheck = Game.GameTime;
            if (territoryMapSavePending &&
                Game.GameTime - territoryMapLastSaveAttempt >= 10000)
                SaveGangTerritoryMap();
            string currentZone = World.GetZoneNameLabel(player.Character.Position);
            RecordGangTerritoryVisit(currentZone, player.Character.Position);
            // Rafraichissement complet limite pour eviter de parcourir toutes les
            // zones a chaque tick ; les nouvelles decouvertes restent immediates.
            bool fullRefresh = Game.GameTime - territoryMapLastOwnerRefresh >= 10000 ||
                territoryMapLastOwnerRefresh == 0;
            if (fullRefresh) territoryMapLastOwnerRefresh = Game.GameTime;
            foreach (var entry in territoryMapPositions)
            {
                if (!fullRefresh && !territoryMapPendingZones.Contains(entry.Key))
                    continue;
                string owner = GangPolitics.GetTerritoryOwner(entry.Key);
                if (string.IsNullOrEmpty(owner))
                {
                    Blip old;
                    if (territoryMapBlips.TryGetValue(entry.Key, out old) && old.Exists())
                        old.Remove();
                    territoryMapBlips.Remove(entry.Key);
                    RemoveGangTerritoryArea(entry.Key);
                    territoryMapOwners.Remove(entry.Key);
                    continue;
                }
                string previous;
                if (territoryMapBlips.ContainsKey(entry.Key) &&
                    territoryMapOwners.TryGetValue(entry.Key, out previous) && previous == owner &&
                    territoryMapBlips[entry.Key].Exists()) continue;
                RemoveGangTerritoryArea(entry.Key);
                Blip existing;
                if (territoryMapBlips.TryGetValue(entry.Key, out existing) && existing.Exists())
                    existing.Remove();
                Blip blip = World.CreateBlip(entry.Value);
                if (blip == null) continue;
                blip.Name = TerritoryMapDisplayName(entry.Key) + " - " + owner;
                blip.Color = (BlipColor)TerritoryMapColor(owner);
                // Display 2 = carte principale uniquement, pas minimap.
                Function.Call(Hash.SET_BLIP_DISPLAY, blip, 3);
                territoryMapBlips[entry.Key] = blip;
                AddGangTerritoryArea(entry.Key, owner);
                territoryMapOwners[entry.Key] = owner;
            }
            territoryMapPendingZones.Clear();
        }

        private void ClearGangTerritoryMap()
        {
            foreach (Blip blip in territoryMapBlips.Values)
                if (blip != null && blip.Exists()) blip.Remove();
            territoryMapBlips.Clear();
            foreach (List<Blip> group in territoryMapAreaBlips.Values)
                foreach (Blip area in group)
                    if (area != null && area.Exists()) area.Remove();
            territoryMapAreaBlips.Clear();
            territoryMapOwners.Clear();
        }
    }
}
