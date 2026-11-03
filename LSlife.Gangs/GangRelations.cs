using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using GTA;

namespace LSlife;

internal enum GangAgreement { None, Pact, Alliance }

internal static class GangRelations
{
    private const string SavePath = @"scripts\LSLife\LSLife_GangRelations.xml";
    private static readonly Dictionary<string, Relation> relations = new Dictionary<string, Relation>();
    private static bool loaded;
    private static bool readFailed;

    internal sealed class Relation
    {
        internal int Trust { get; }
        internal bool Introduced { get; }
        internal GangAgreement Agreement { get; }

        internal Relation(int trust, bool introduced, GangAgreement agreement)
        {
            Trust = trust;
            Introduced = introduced;
            Agreement = agreement;
        }
    }

    internal static Relation Get(string gang)
    {
        Load();
        return relations.TryGetValue(gang, out var relation)
            ? relation : new Relation(0, false, GangAgreement.None);
    }

    internal static string Label(GangAgreement agreement)
    {
        switch (agreement)
        {
            case GangAgreement.Pact: return "Non-agression";
            case GangAgreement.Alliance: return "Alliance";
            default: return "Aucun accord";
        }
    }

    internal static bool Introduce(string gang)
    {
        var relation = Get(gang);
        return !relation.Introduced && Save(gang,
            new Relation(Math.Min(100, relation.Trust + 10), true, relation.Agreement));
    }

    internal static bool Support(string gang, Player player)
    {
        const int cost = 5000;
        var relation = Get(gang);
        if (!relation.Introduced || relation.Trust >= 100) return false;
        if (player.Money < cost) return Reject("Fonds personnels insuffisants.");
        if (!Save(gang, new Relation(Math.Min(100, relation.Trust + 10), true, relation.Agreement)))
            return false;

        player.Money -= cost;
        return true;
    }

    internal static bool CanNegotiate(Relation relation, GangAgreement agreement)
    {
        switch (agreement)
        {
            case GangAgreement.Pact:
                return relation.Agreement == GangAgreement.None && relation.Trust >= 25;
            case GangAgreement.Alliance:
                return relation.Agreement == GangAgreement.Pact && relation.Trust >= 60;
            default:
                return false;
        }
    }

    internal static bool Negotiate(string gang, GangAgreement agreement)
    {
        var relation = Get(gang);
        return CanNegotiate(relation, agreement) &&
            Save(gang, new Relation(relation.Trust, relation.Introduced, agreement));
    }

    internal static bool Break(string gang)
    {
        var relation = Get(gang);
        return relation.Agreement != GangAgreement.None && Save(gang,
            new Relation(Math.Max(0, relation.Trust - 20), relation.Introduced, GangAgreement.None));
    }

    private static void Load()
    {
        if (loaded) return;
        loaded = true;
        try
        {
            if (!File.Exists(SavePath)) return;
            var root = XElement.Load(SavePath);
            if (root.Name != "GangRelations") throw new FormatException("Racine inattendue.");
            foreach (var entry in root.Elements("Relation"))
            {
                string gang = (string)entry.Attribute("gang");
                int trust = (int)entry.Attribute("trust");
                bool introduced = (bool)entry.Attribute("introduced");
                if (string.IsNullOrWhiteSpace(gang) || trust < 0 || trust > 100 ||
                    !Enum.TryParse((string)entry.Attribute("agreement"), out GangAgreement agreement) ||
                    !Enum.IsDefined(typeof(GangAgreement), agreement) || relations.ContainsKey(gang))
                    throw new FormatException("Relation de gang invalide.");
                relations.Add(gang, new Relation(trust, introduced, agreement));
            }
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException ||
            ex is XmlException || ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
        {
            relations.Clear();
            readFailed = true;
            LsFunctions.DbugString("Chargement de la diplomatie : " + ex.Message);
        }
    }

    private static bool Save(string gang, Relation relation)
    {
        if (readFailed) return Reject("Sauvegarde diplomatique illisible.");
        string temporaryPath = SavePath + ".tmp";
        try
        {
            var snapshot = new Dictionary<string, Relation>(relations) { [gang] = relation };
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            new XElement("GangRelations", snapshot.Select(pair =>
                new XElement("Relation", new XAttribute("gang", pair.Key),
                    new XAttribute("trust", pair.Value.Trust),
                    new XAttribute("introduced", pair.Value.Introduced),
                    new XAttribute("agreement", pair.Value.Agreement.ToString())))).Save(temporaryPath);
            if (File.Exists(SavePath)) File.Replace(temporaryPath, SavePath, null);
            else File.Move(temporaryPath, SavePath);
            relations[gang] = relation;
            return true;
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            LsFunctions.DbugString("Sauvegarde de la diplomatie : " + ex.Message);
            return Reject("Impossible de sauvegarder l'accord ou la confiance.");
        }
    }

    private static bool Reject(string message)
    {
        GTA.UI.ShowSubtitle(message, 3500);
        return false;
    }
}
