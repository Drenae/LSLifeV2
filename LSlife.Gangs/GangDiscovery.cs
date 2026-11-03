using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using GTA;

namespace LSlife;

internal static class GangDiscovery
{
    private const string SavePath = @"scripts\LSLife\LSLife_GangDiscovery.xml";

    // Correspondances de l'ancienne branche. Elles ne modifient pas Area.Gang.
    private static readonly Dictionary<string, string> ZoneGangs =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DAVIS", "Ballas" },
            { "CHAMH", "Families" },
            { "STRAW", "Families" },
            { "RANCHO", "Vagos" },
            { "LAMESA", "Marabunta Grande" },
            { "EBURO", "Marabunta Grande" },
            { "EAST_V", "Marabunta Grande" }
        };

    private static readonly HashSet<string> discovered = new HashSet<string>(StringComparer.Ordinal);
    private static bool loaded;
    private static bool loadFailed;
    private static int nextCheck;

    internal static IEnumerable<string> KnownGangs()
    {
        Load();
        return discovered.OrderBy(gang => gang, StringComparer.Ordinal);
    }

    internal static string ReferenceOwner(string zone) =>
        zone != null && ZoneGangs.TryGetValue(zone, out string gang) ? gang : null;

    internal static IEnumerable<string> Territories(string gang) =>
        ZoneGangs.Where(zone => zone.Value == gang).Select(zone => zone.Key);

    internal static void Tick(Player player)
    {
        int now = Game.GameTime;
        if (unchecked(now - nextCheck) < 0) return;
        nextCheck = unchecked(now + 1000);

        if (player == null || !player.IsPlaying) return;
        Ped ped = player.Character;
        if (ped == null || !ped.Exists()) return;
        string zone = World.GetZoneNameLabel(((Entity)ped).Position);
        if (string.IsNullOrEmpty(zone) || !ZoneGangs.TryGetValue(zone, out string gang)) return;

        Load();
        if (loadFailed || discovered.Contains(gang)) return;

        try
        {
            string temporaryPath = SavePath + ".tmp";
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            new XElement("GangDiscovery", discovered.Concat(new[] { gang })
                .Select(name => new XElement("Gang", name))).Save(temporaryPath);
            if (File.Exists(SavePath)) File.Replace(temporaryPath, SavePath, null);
            else File.Move(temporaryPath, SavePath);
            discovered.Add(gang);
            GTA.UI.ShowSubtitle("Gang découvert : " + gang, 3500);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            nextCheck = unchecked(now + 30000);
            LsFunctions.DbugString("Sauvegarde des gangs découverts : " + ex.Message);
        }
    }

    private static void Load()
    {
        if (loaded) return;
        loaded = true;
        try
        {
            if (!File.Exists(SavePath)) return;
            var root = XElement.Load(SavePath);
            if (root.Name != "GangDiscovery") throw new FormatException("Racine inattendue.");
            foreach (var gang in root.Elements("Gang"))
                if (ZoneGangs.ContainsValue(gang.Value)) discovered.Add(gang.Value);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException ||
            ex is XmlException || ex is FormatException)
        {
            loadFailed = true;
            LsFunctions.DbugString("Chargement des gangs découverts : " + ex.Message);
        }
    }
}
