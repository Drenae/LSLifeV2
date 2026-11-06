using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace LSlife;

// Publication en memoire seulement APRES ecriture atomique reussie.
// Un XML invalide bloque les mutations ; il n'est jamais remplace par un gang vide.
internal sealed class PlayerGangStore
{
    internal const string DefaultPath = "scripts\\LSLife\\LSLife_PlayerGang.xml";
    private readonly string path;
    private XDocument baseline;
    internal PlayerGangProfile Current { get; private set; }
    internal string LastError { get; private set; }
    internal bool IsBlocked { get; private set; }

    internal PlayerGangStore(string path) { this.path = path; }

    internal void Load()
    {
        Current = null;
        baseline = null;
        LastError = null;
        IsBlocked = false;
        try
        {
            if (!File.Exists(path))
            {
                // File.Exists retourne aussi false lorsque l'acces est refuse :
                // la lecture ci-dessous distingue une vraie absence d'une erreur.
                try { baseline = XmlSaveFile.Load(path); }
                catch (IOException error) when (error.InnerException is FileNotFoundException)
                {
                    if (File.Exists(path + ".bak"))
                        throw new IOException("XML actif absent, copie .bak presente : restauration manuelle requise.");
                    return;
                }
            }
            else baseline = XmlSaveFile.Load(path);
            Current = Parse(baseline);
        }
        catch (Exception error) when (IsDataError(error))
        {
            IsBlocked = true;
            LastError = error.Message;
        }
    }

    internal bool Create(string name, string colorKey)
    {
        if (IsBlocked || Current != null) return false;
        try
        {
            var profile = new PlayerGangProfile(Guid.NewGuid(), name, colorKey, DateTime.UtcNow,
                new int[0], new string[0]);
            return Commit(profile);
        }
        catch (Exception error) when (IsDataError(error)) { LastError = error.Message; return false; }
    }

    internal bool UpdateIdentity(string name, string colorKey)
    {
        if (IsBlocked || Current == null) return false;
        try { return Commit(Current.WithIdentity(name, colorKey)); }
        catch (Exception error) when (IsDataError(error)) { LastError = error.Message; return false; }
    }

    private bool Commit(PlayerGangProfile profile)
    {
        if (baseline == null)
        {
            if (File.Exists(path) || File.Exists(path + ".bak"))
                throw new IOException("Un fichier de gang est apparu. Recharger avant toute modification.");
        }
        else if (!XNode.DeepEquals(baseline, XmlSaveFile.Load(path)))
            throw new IOException("Le XML a ete modifie ailleurs. Recharger avant toute modification.");

        // Conserver les extensions XML inconnues lors d'un changement d'identite.
        var document = baseline == null
            ? new XDocument(new XElement("playerGang", new XAttribute("version", 1),
                new XElement("id", profile.Id.ToString("D")), new XElement("leaderId", profile.LeaderId),
                new XElement("createdUtc", profile.CreatedUtc.ToString("o", CultureInfo.InvariantCulture)),
                new XElement("members"), new XElement("territories")))
            : new XDocument(baseline);
        document.Root.SetElementValue("name", profile.Name);
        document.Root.SetElementValue("color", profile.ColorKey);
        Parse(document); // Validation du document reel avant toute ecriture.
        XmlSaveFile.Save(document, path);
        baseline = document;
        Current = profile;
        LastError = null;
        return true;
    }

    internal static PlayerGangProfile Parse(XDocument document)
    {
        XElement root = document.Root;
        if (root == null || root.Name != "playerGang" || (string)root.Attribute("version") != "1")
            throw new InvalidDataException("Format ou version du gang non pris en charge.");
        if (Required(root, "leaderId").Value != "player")
            throw new InvalidDataException("Chef du gang invalide.");
        return new PlayerGangProfile(Guid.Parse(Required(root, "id").Value),
            Required(root, "name").Value, Required(root, "color").Value,
            DateTime.ParseExact(Required(root, "createdUtc").Value, "o", CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind),
            Required(root, "members").Elements("dealer").Select(e => int.Parse(Required(e, "id").Value, CultureInfo.InvariantCulture)),
            Required(root, "territories").Elements("territory").Select(e => Required(e, "zone").Value));
    }

    private static XElement Required(XElement parent, string name)
    {
        var matches = parent.Elements(name).ToArray();
        if (matches.Length != 1) throw new InvalidDataException("Champ XML manquant ou duplique : " + name);
        return matches[0];
    }

    private static bool IsDataError(Exception error) => error is IOException || error is UnauthorizedAccessException ||
        error is XmlException || error is InvalidDataException || error is ArgumentException || error is FormatException || error is OverflowException;
}
