using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace LSlife;

internal sealed class PlayerGangOrganization
{
    internal const string DefaultName = "Mon organisation";
    private const string SavePath = @"scripts\LSLife\LSLife_PlayerGang.xml";
    private const int MaxBalance = 999999999;
    private const int MaxMembers = 100;

    private static bool loaded;
    private static bool loadFailed;
    private static PlayerGangOrganization current;

    internal string Name { get; }
    internal int Balance { get; }
    internal int Dealers { get; }
    internal int Soldiers { get; }
    internal IReadOnlyList<string> Territories { get; }

    private PlayerGangOrganization(string name, int balance, int dealers, int soldiers, string[] territories = null)
    {
        Name = name;
        Balance = balance;
        Dealers = dealers;
        Soldiers = soldiers;
        Territories = Array.AsReadOnly(territories ?? new string[0]);
    }

    internal static PlayerGangOrganization Current
    {
        get
        {
            if (!loaded) Load();
            return current;
        }
    }

    internal static bool CanWrite => !loadFailed;

    internal static bool Create() =>
        Current == null && Save(new PlayerGangOrganization(DefaultName, 0, 0, 0));

    internal static bool Leave() => Current != null && Save(null);

    internal static bool Deposit(int amount)
    {
        var gang = Current;
        if (gang == null || amount <= 0 || amount > MaxBalance - gang.Balance)
            return Reject("Dépôt impossible.");

        return Save(new PlayerGangOrganization(gang.Name, gang.Balance + amount, gang.Dealers, gang.Soldiers, gang.Territories.ToArray()));
    }

    internal static bool Recruit(bool soldier)
    {
        var gang = Current;
        int cost = soldier ? 15000 : 10000;
        if (gang == null || gang.Balance < cost || 1 + gang.Dealers + gang.Soldiers >= MaxMembers)
            return Reject("Trésorerie insuffisante ou effectif maximal atteint.");

        return Save(new PlayerGangOrganization(gang.Name, gang.Balance - cost,
            gang.Dealers + (soldier ? 0 : 1), gang.Soldiers + (soldier ? 1 : 0), gang.Territories.ToArray()));
    }

    internal static bool Capture(string zone, int reward)
    {
        var gang = Current;
        if (gang == null || gang.Territories.Contains(zone)) return false;
        return Save(new PlayerGangOrganization(gang.Name, Math.Min(MaxBalance, gang.Balance + reward),
            gang.Dealers, gang.Soldiers, gang.Territories.Concat(new[] { zone }).ToArray()));
    }

    internal static void CollectTerritoryRevenue(int amountPerZone)
    {
        var gang = Current;
        if (gang == null || gang.Territories.Count == 0 || gang.Balance == MaxBalance) return;
        int balance = Math.Min(MaxBalance, gang.Balance + gang.Territories.Count * amountPerZone);
        Save(new PlayerGangOrganization(gang.Name, balance, gang.Dealers, gang.Soldiers, gang.Territories.ToArray()));
    }

    private static void Load()
    {
        loaded = true;
        try
        {
            if (!File.Exists(SavePath)) return;
            var root = XElement.Load(SavePath);
            if (root.Name != "PlayerGang") throw new FormatException("Racine inattendue.");
            var gang = root.Element("Organization");
            if (gang == null) return;

            string name = (string)gang.Attribute("name");
            int balance = (int)gang.Attribute("balance");
            int dealers = (int)gang.Attribute("dealers");
            int soldiers = (int)gang.Attribute("soldiers");
            if (string.IsNullOrWhiteSpace(name) || balance < 0 || balance > MaxBalance ||
                dealers < 0 || soldiers < 0 || 1L + dealers + soldiers > MaxMembers)
                throw new FormatException("Données de gang invalides.");

            current = new PlayerGangOrganization(name, balance, dealers, soldiers,
                gang.Elements("Territory").Select(zone => zone.Value)
                    .Where(zone => GangDiscovery.ReferenceOwner(zone) != null).Distinct().ToArray());
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException ||
            ex is XmlException || ex is FormatException || ex is ArgumentNullException || ex is OverflowException)
        {
            loadFailed = true;
            LsFunctions.DbugString("Chargement du gang : " + ex.Message);
            Reject("Sauvegarde du gang illisible.");
        }
    }

    private static bool Save(PlayerGangOrganization gang)
    {
        if (loadFailed) return Reject("Sauvegarde du gang illisible.");

        string temporaryPath = SavePath + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            var root = new XElement("PlayerGang");
            if (gang != null)
                root.Add(new XElement("Organization", new XAttribute("name", gang.Name),
                    new XAttribute("balance", gang.Balance), new XAttribute("dealers", gang.Dealers),
                    new XAttribute("soldiers", gang.Soldiers),
                    gang.Territories.Select(zone => new XElement("Territory", zone))));

            root.Save(temporaryPath);
            if (File.Exists(SavePath)) File.Replace(temporaryPath, SavePath, null);
            else File.Move(temporaryPath, SavePath);
            current = gang;
            return true;
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            LsFunctions.DbugString("Sauvegarde du gang : " + ex.Message);
            return Reject("Impossible de sauvegarder le gang.");
        }
    }

    private static bool Reject(string message)
    {
        GTA.UI.ShowSubtitle(message, 3500);
        return false;
    }
}
