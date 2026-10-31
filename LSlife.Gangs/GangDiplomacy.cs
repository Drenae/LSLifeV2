using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LSlife.Gangs
{
    internal enum GangRelationStatus { Hostile, Neutral, NonAggression, Allied }
    internal enum GangAgreementType { NonAggression, Trade, ArmedSupport, MutualDefense }
    internal enum PlayerGangRank { Independent, Recruit, Member, Lieutenant, RightHand, Leader }

    internal sealed class GangRelation
    {
        internal string First;
        internal string Second;
        internal GangRelationStatus Status = GangRelationStatus.Neutral;
        internal int Trust = 50;
        internal readonly HashSet<GangAgreementType> Agreements = new HashSet<GangAgreementType>();

        internal GangRelation(string first, string second) { First = first; Second = second; }
    }

    internal sealed class GangOrganization
    {
        internal string Id;
        internal string Name;
        internal string LeaderName;
        internal int Money;
        internal int Members;
        internal bool PlayerFounded;
        internal readonly HashSet<string> Territories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        internal GangOrganization(string id, string name) { Id = id; Name = name; }
    }

    internal sealed class GangCartel
    {
        internal string Name;
        internal string LeaderGangId;
        internal readonly HashSet<string> Members = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        internal GangCartel(string name, string leader)
        {
            Name = name;
            LeaderGangId = leader;
            Members.Add(leader);
        }
    }

    // Etat autonome : aucune modification des sauvegardes existantes ni des groupes PNJ GTA.
    internal sealed class GangDiplomacy
    {
        private readonly Dictionary<string, GangRelation> relations = new Dictionary<string, GangRelation>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, GangOrganization> organizations = new Dictionary<string, GangOrganization>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, GangCartel> cartels = new Dictionary<string, GangCartel>(StringComparer.OrdinalIgnoreCase);
        internal string PlayerGangId;
        internal PlayerGangRank PlayerRank = PlayerGangRank.Independent;

        private static string Key(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second) ||
                string.Equals(first, second, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Paire de gangs invalide.");
            return string.Compare(first, second, StringComparison.OrdinalIgnoreCase) < 0
                ? first.ToUpperInvariant() + "|" + second.ToUpperInvariant()
                : second.ToUpperInvariant() + "|" + first.ToUpperInvariant();
        }

        internal GangRelation GetRelation(string first, string second)
        {
            string key = Key(first, second);
            GangRelation relation;
            if (!relations.TryGetValue(key, out relation))
            {
                relation = new GangRelation(first, second);
                relations.Add(key, relation);
            }
            return relation;
        }

        internal GangRelationStatus Get(string first, string second) { return GetRelation(first, second).Status; }

        internal void Set(string first, string second, GangRelationStatus status)
        {
            if (!Enum.IsDefined(typeof(GangRelationStatus), status)) throw new ArgumentOutOfRangeException("status");
            GetRelation(first, second).Status = status;
        }

        internal void ChangeTrust(string first, string second, int change)
        {
            GangRelation relation = GetRelation(first, second);
            relation.Trust = (int)Math.Max(0L, Math.Min(100L, (long)relation.Trust + change));
        }

        internal void SetAgreement(string first, string second, GangAgreementType agreement, bool enabled)
        {
            if (!Enum.IsDefined(typeof(GangAgreementType), agreement)) throw new ArgumentOutOfRangeException("agreement");
            GangRelation relation = GetRelation(first, second);
            if (enabled) relation.Agreements.Add(agreement);
            else relation.Agreements.Remove(agreement);
            if (agreement == GangAgreementType.NonAggression)
            {
                if (enabled && relation.Status == GangRelationStatus.Neutral) relation.Status = GangRelationStatus.NonAggression;
                if (!enabled && relation.Status == GangRelationStatus.NonAggression) relation.Status = GangRelationStatus.Neutral;
            }
        }

        internal GangOrganization RegisterGang(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Identite de gang invalide.");
            GangOrganization gang;
            if (!organizations.TryGetValue(id, out gang))
            {
                gang = new GangOrganization(id, name);
                organizations.Add(id, gang);
            }
            return gang;
        }

        internal bool TryGetGang(string id, out GangOrganization gang)
        {
            return organizations.TryGetValue(id, out gang);
        }

        internal void JoinGang(string id, PlayerGangRank rank)
        {
            if (!organizations.ContainsKey(id)) throw new ArgumentException("Gang inconnu.");
            if (rank == PlayerGangRank.Independent || !Enum.IsDefined(typeof(PlayerGangRank), rank))
                throw new ArgumentOutOfRangeException("rank");
            PlayerGangId = id;
            PlayerRank = rank;
        }

        internal void LeaveGang() { PlayerGangId = null; PlayerRank = PlayerGangRank.Independent; }

        internal GangOrganization FoundPlayerGang(string id, string name)
        {
            if (!string.IsNullOrEmpty(PlayerGangId)) throw new InvalidOperationException("Le joueur appartient deja a un gang.");
            GangOrganization gang = RegisterGang(id, name);
            if (gang.PlayerFounded || gang.Members != 0 || !string.IsNullOrEmpty(gang.LeaderName))
                throw new InvalidOperationException("Organisation deja existante.");
            gang.PlayerFounded = true;
            JoinGang(id, PlayerGangRank.Leader);
            return gang;
        }

        internal GangCartel CreateCartel(string name, string leaderGangId)
        {
            if (string.IsNullOrWhiteSpace(name) || !organizations.ContainsKey(leaderGangId))
                throw new ArgumentException("Cartel ou gang dirigeant invalide.");
            if (cartels.ContainsKey(name)) throw new InvalidOperationException("Cartel deja existant.");
            GangCartel cartel = new GangCartel(name, leaderGangId);
            cartels.Add(name, cartel);
            return cartel;
        }

        internal void AddCartelMember(string cartelName, string gangId)
        {
            GangCartel cartel;
            if (!cartels.TryGetValue(cartelName, out cartel) || !organizations.ContainsKey(gangId))
                throw new ArgumentException("Cartel ou gang inconnu.");
            if (cartels.Values.Any(c => c.Members.Contains(gangId)))
                throw new InvalidOperationException("Ce gang appartient deja a un cartel.");
            cartel.Members.Add(gangId);
        }

        internal void RemoveCartelMember(string cartelName, string gangId)
        {
            GangCartel cartel;
            if (!cartels.TryGetValue(cartelName, out cartel)) throw new ArgumentException("Cartel inconnu.");
            if (string.Equals(cartel.LeaderGangId, gangId, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Le dirigeant doit dissoudre le cartel.");
            cartel.Members.Remove(gangId);
        }

        internal void DissolveCartel(string cartelName) { cartels.Remove(cartelName); }

        internal void Save(string path)
        {
            XElement root = new XElement("gangDiplomacy", new XAttribute("version", 1),
                new XElement("player", new XAttribute("gang", PlayerGangId ?? ""),
                    new XAttribute("rank", PlayerRank)),
                new XElement("organizations", organizations.Values.OrderBy(g => g.Id).Select(g =>
                    new XElement("gang", new XAttribute("id", g.Id), new XAttribute("name", g.Name),
                        new XAttribute("leader", g.LeaderName ?? ""), new XAttribute("money", g.Money),
                        new XAttribute("members", g.Members), new XAttribute("playerFounded", g.PlayerFounded),
                        g.Territories.OrderBy(t => t).Select(t => new XElement("territory", t))))),
                new XElement("relations", relations.Values.OrderBy(r => Key(r.First, r.Second)).Select(r =>
                    new XElement("relation", new XAttribute("first", r.First), new XAttribute("second", r.Second),
                        new XAttribute("status", r.Status), new XAttribute("trust", r.Trust),
                        r.Agreements.OrderBy(a => a).Select(a => new XElement("agreement", a))))),
                new XElement("cartels", cartels.Values.OrderBy(c => c.Name).Select(c =>
                    new XElement("cartel", new XAttribute("name", c.Name),
                        new XAttribute("leader", c.LeaderGangId),
                        c.Members.OrderBy(m => m).Select(m => new XElement("member", m))))));
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            string temporary = path + ".tmp";
            new XDocument(root).Save(temporary);
            if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
            else File.Move(temporary, path);
        }

        internal void Load(string path)
        {
            if (!File.Exists(path)) return;
            XDocument doc = XDocument.Load(path);
            if (doc.Root == null || doc.Root.Name != "gangDiplomacy")
                throw new InvalidDataException("Sauvegarde diplomatique invalide.");

            // Construire le nouvel etat avant de remplacer l'ancien.
            GangDiplomacy loaded = new GangDiplomacy();
            XElement orgs = doc.Root.Element("organizations");
            if (orgs != null) foreach (XElement element in orgs.Elements("gang"))
            {
                string id = (string)element.Attribute("id");
                string name = (string)element.Attribute("name");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)) continue;
                GangOrganization gang = loaded.RegisterGang(id, name);
                gang.LeaderName = (string)element.Attribute("leader");
                gang.Money = ReadInt(element, "money", 0, 0, 999999999);
                gang.Members = ReadInt(element, "members", 0, 0, 100000);
                gang.PlayerFounded = (bool?)element.Attribute("playerFounded") ?? false;
                foreach (XElement territory in element.Elements("territory"))
                    if (!string.IsNullOrWhiteSpace(territory.Value)) gang.Territories.Add(territory.Value);
            }
            XElement rels = doc.Root.Element("relations");
            if (rels != null) foreach (XElement element in rels.Elements("relation"))
            {
                string first = (string)element.Attribute("first");
                string second = (string)element.Attribute("second");
                if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second) ||
                    string.Equals(first, second, StringComparison.OrdinalIgnoreCase)) continue;
                GangRelation relation = loaded.GetRelation(first, second);
                GangRelationStatus status;
                if (Enum.TryParse((string)element.Attribute("status"), true, out status) &&
                    Enum.IsDefined(typeof(GangRelationStatus), status)) relation.Status = status;
                relation.Trust = ReadInt(element, "trust", 50, 0, 100);
                foreach (XElement agreement in element.Elements("agreement"))
                {
                    GangAgreementType type;
                    if (Enum.TryParse(agreement.Value, true, out type) &&
                        Enum.IsDefined(typeof(GangAgreementType), type)) relation.Agreements.Add(type);
                }
            }
            XElement cs = doc.Root.Element("cartels");
            if (cs != null) foreach (XElement element in cs.Elements("cartel"))
            {
                string name = (string)element.Attribute("name");
                string leader = (string)element.Attribute("leader");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(leader) ||
                    !loaded.organizations.ContainsKey(leader) || loaded.cartels.ContainsKey(name)) continue;
                GangCartel cartel = loaded.CreateCartel(name, leader);
                foreach (XElement member in element.Elements("member"))
                    if (loaded.organizations.ContainsKey(member.Value)) cartel.Members.Add(member.Value);
            }
            XElement player = doc.Root.Element("player");
            if (player != null)
            {
                string id = (string)player.Attribute("gang");
                PlayerGangRank rank;
                if (!string.IsNullOrEmpty(id) && loaded.organizations.ContainsKey(id) &&
                    Enum.TryParse((string)player.Attribute("rank"), true, out rank) &&
                    Enum.IsDefined(typeof(PlayerGangRank), rank) && rank != PlayerGangRank.Independent)
                    loaded.JoinGang(id, rank);
            }
            organizations.Clear();
            foreach (var pair in loaded.organizations) organizations.Add(pair.Key, pair.Value);
            relations.Clear();
            foreach (var pair in loaded.relations) relations.Add(pair.Key, pair.Value);
            cartels.Clear();
            foreach (var pair in loaded.cartels) cartels.Add(pair.Key, pair.Value);
            PlayerGangId = loaded.PlayerGangId;
            PlayerRank = loaded.PlayerRank;
        }

        private static int ReadInt(XElement element, string attribute, int fallback, int min, int max)
        {
            int value;
            if (!int.TryParse((string)element.Attribute(attribute), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out value)) return fallback;
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
