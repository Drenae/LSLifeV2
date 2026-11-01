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
        // Reputation du joueur distincte des relations diplomatiques entre gangs.
        // Aucun contact ni renseignement n'est offert automatiquement au chargement.
        private readonly Dictionary<string, int> playerReputation =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        internal const int ContactReputation = 25;
        internal const int TerritoryReputation = 35;
        internal const int MembersReputation = 45;
        internal const int RelationsReputation = 55;
        internal const int TreasuryReputation = 75;
        internal const int AllianceReputation = 60;

        internal int GetPlayerReputation(string gangId)
        {
            int value;
            return !string.IsNullOrWhiteSpace(gangId) && playerReputation.TryGetValue(gangId, out value) ? value : 0;
        }

        internal bool HasLeaderContact(string gangId)
        {
            return !string.IsNullOrWhiteSpace(gangId) && organizations.ContainsKey(gangId) &&
                GetPlayerReputation(gangId) >= ContactReputation;
        }

        internal void ChangePlayerReputation(string gangId, int change)
        {
            if (string.IsNullOrWhiteSpace(gangId) || !organizations.ContainsKey(gangId))
                throw new ArgumentException("Gang inconnu.");
            int current = GetPlayerReputation(gangId);
            int next = (int)Math.Max(0L, Math.Min(100L, (long)current + change));
            if (next == current) return;
            playerReputation[gangId] = next;
            MarkDirty();
        }

        private readonly Dictionary<string, GangRelation> relations = new Dictionary<string, GangRelation>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, GangOrganization> organizations = new Dictionary<string, GangOrganization>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, GangCartel> cartels = new Dictionary<string, GangCartel>(StringComparer.OrdinalIgnoreCase);
        internal IEnumerable<GangOrganization> Organizations { get { return organizations.Values.OrderBy(g => g.Name); } }
        internal IEnumerable<GangCartel> Cartels { get { return cartels.Values.OrderBy(g => g.Name); } }
        internal bool IsDirty { get; private set; }
        internal void MarkDirty() { IsDirty = true; }
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
            GangRelation relation = GetRelation(first, second);
            if (relation.Status != status) { relation.Status = status; MarkDirty(); }
        }

        internal void ChangeTrust(string first, string second, int change)
        {
            GangRelation relation = GetRelation(first, second);
            int next = (int)Math.Max(0L, Math.Min(100L, (long)relation.Trust + change));
            if (next != relation.Trust) { relation.Trust = next; MarkDirty(); }
        }

        internal void SetAgreement(string first, string second, GangAgreementType agreement, bool enabled)
        {
            if (!Enum.IsDefined(typeof(GangAgreementType), agreement)) throw new ArgumentOutOfRangeException("agreement");
            GangRelation relation = GetRelation(first, second);
            bool changed = enabled ? relation.Agreements.Add(agreement) : relation.Agreements.Remove(agreement);
            if (changed) MarkDirty();
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
                MarkDirty();
            }
            return gang;
        }

        internal bool TryGetGang(string id, out GangOrganization gang)
        {
            return organizations.TryGetValue(id, out gang);
        }

        internal string GetTerritoryOwner(string areaName)
        {
            if (string.IsNullOrWhiteSpace(areaName)) return null;
            foreach (GangOrganization gang in organizations.Values)
                if (gang.Territories.Contains(areaName)) return gang.Id;
            return null;
        }

        internal void SetTerritoryOwner(string areaName, string gangId)
        {
            if (string.IsNullOrWhiteSpace(areaName) || !organizations.ContainsKey(gangId))
                throw new ArgumentException("Territoire ou gang invalide.");
            bool changed = false;
            foreach (GangOrganization gang in organizations.Values)
                if (gang.Territories.Remove(areaName)) changed = true;
            if (organizations[gangId].Territories.Add(areaName)) changed = true;
            if (changed) MarkDirty();
        }

        internal bool AreAllied(string first, string second)
        {
            if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase)) return true;
            GangRelation relation = GetRelation(first, second);
            return relation.Status == GangRelationStatus.Allied ||
                relation.Agreements.Contains(GangAgreementType.MutualDefense);
        }

        // Les renseignements diplomatiques ne sont accessibles qu'au gang du joueur
        // et a ses allies directs. Les alliances de tiers restent confidentielles.
        internal bool CanViewGangIntelligence(string gangId)
        {
            if (string.IsNullOrWhiteSpace(PlayerGangId) || string.IsNullOrWhiteSpace(gangId) ||
                !organizations.ContainsKey(gangId)) return false;
            if (string.Equals(PlayerGangId, gangId, StringComparison.OrdinalIgnoreCase)) return true;
            return AreAllied(PlayerGangId, gangId);
        }

        internal bool CanViewRelationIntelligence(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(PlayerGangId)) return false;
            return (string.Equals(first, PlayerGangId, StringComparison.OrdinalIgnoreCase) &&
                    CanViewGangIntelligence(second)) ||
                   (string.Equals(second, PlayerGangId, StringComparison.OrdinalIgnoreCase) &&
                    CanViewGangIntelligence(first));
        }

        internal bool CanViewCartelIntelligence(GangCartel cartel)
        {
            return cartel != null && !string.IsNullOrWhiteSpace(PlayerGangId) &&
                cartel.Members.Contains(PlayerGangId);
        }

        internal bool CanNegotiate(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second) ||
                string.Equals(first, second, StringComparison.OrdinalIgnoreCase)) return false;
            GangRelation relation = GetRelation(first, second);
            return relation.Status != GangRelationStatus.Hostile && relation.Trust >= 30;
        }

        internal void BreakAlliance(string first, string second)
        {
            GangRelation relation = GetRelation(first, second);
            relation.Agreements.Clear();
            relation.Status = GangRelationStatus.Neutral;
            ChangeTrust(first, second, -20);
            MarkDirty();
        }

        internal bool ProposeAgreement(string first, string second, GangAgreementType agreement)
        {
            if (!CanNegotiate(first, second)) return false;
            GangRelation relation = GetRelation(first, second);
            int requiredTrust = agreement == GangAgreementType.MutualDefense ? 75 :
                agreement == GangAgreementType.ArmedSupport ? 65 :
                agreement == GangAgreementType.Trade ? 50 : 35;
            if (relation.Trust < requiredTrust || relation.Agreements.Contains(agreement)) return false;
            SetAgreement(first, second, agreement, true);
            if (agreement == GangAgreementType.MutualDefense)
                Set(first, second, GangRelationStatus.Allied);
            return true;
        }

        internal void CancelAgreement(string first, string second, GangAgreementType agreement)
        {
            GangRelation relation = GetRelation(first, second);
            if (!relation.Agreements.Contains(agreement)) return;
            SetAgreement(first, second, agreement, false);
            if (agreement == GangAgreementType.MutualDefense &&
                relation.Status == GangRelationStatus.Allied)
                Set(first, second, GangRelationStatus.Neutral);
            ChangeTrust(first, second, -10);
        }

        internal bool CanCreateCartel(string leaderGangId)
        {
            if (string.IsNullOrWhiteSpace(leaderGangId) || !organizations.ContainsKey(leaderGangId))
                return false;
            return !cartels.Values.Any(c => c.Members.Contains(leaderGangId));
        }

        internal bool CanJoinCartel(string cartelName, string gangId)
        {
            GangCartel cartel;
            if (string.IsNullOrWhiteSpace(gangId) || !organizations.ContainsKey(gangId) ||
                !cartels.TryGetValue(cartelName, out cartel) ||
                cartels.Values.Any(c => c.Members.Contains(gangId))) return false;
            return AreAllied(cartel.LeaderGangId, gangId) &&
                GetRelation(cartel.LeaderGangId, gangId).Trust >= 70;
        }

        internal void JoinGang(string id, PlayerGangRank rank)
        {
            if (!organizations.ContainsKey(id)) throw new ArgumentException("Gang inconnu.");
            if (rank == PlayerGangRank.Independent || !Enum.IsDefined(typeof(PlayerGangRank), rank))
                throw new ArgumentOutOfRangeException("rank");
            PlayerGangId = id;
            PlayerRank = rank;
            MarkDirty();
        }

        internal void LeaveGang() { PlayerGangId = null; PlayerRank = PlayerGangRank.Independent; MarkDirty(); }

        internal GangOrganization FoundPlayerGang(string id, string name)
        {
            if (!string.IsNullOrEmpty(PlayerGangId)) throw new InvalidOperationException("Le joueur appartient deja a un gang.");
            GangOrganization gang = RegisterGang(id, name);
            if (gang.PlayerFounded || gang.Members != 0 || !string.IsNullOrEmpty(gang.LeaderName))
                throw new InvalidOperationException("Organisation deja existante.");
            gang.PlayerFounded = true;
            MarkDirty();
            JoinGang(id, PlayerGangRank.Leader);
            return gang;
        }

        internal GangCartel CreateCartel(string name, string leaderGangId)
        {
            if (string.IsNullOrWhiteSpace(name) || !organizations.ContainsKey(leaderGangId))
                throw new ArgumentException("Cartel ou gang dirigeant invalide.");
            if (cartels.ContainsKey(name)) throw new InvalidOperationException("Cartel deja existant.");
            if (!CanCreateCartel(leaderGangId)) throw new InvalidOperationException("Ce gang appartient deja a un cartel.");
            GangCartel cartel = new GangCartel(name, leaderGangId);
            cartels.Add(name, cartel);
            MarkDirty();
            return cartel;
        }

        internal void AddCartelMember(string cartelName, string gangId)
        {
            GangCartel cartel;
            if (!cartels.TryGetValue(cartelName, out cartel) || !organizations.ContainsKey(gangId))
                throw new ArgumentException("Cartel ou gang inconnu.");
            if (!CanJoinCartel(cartelName, gangId))
                throw new InvalidOperationException("Adhesion refusee : alliance, confiance ou disponibilite insuffisante.");
            cartel.Members.Add(gangId);
            MarkDirty();
        }

        internal void RemoveCartelMember(string cartelName, string gangId)
        {
            GangCartel cartel;
            if (!cartels.TryGetValue(cartelName, out cartel)) throw new ArgumentException("Cartel inconnu.");
            if (string.Equals(cartel.LeaderGangId, gangId, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Le dirigeant doit dissoudre le cartel.");
            if (cartel.Members.Remove(gangId)) MarkDirty();
        }

        internal void DissolveCartel(string cartelName) { if (cartels.Remove(cartelName)) MarkDirty(); }

        internal void Save(string path)
        {
            XElement root = new XElement("gangDiplomacy", new XAttribute("version", 1),
                new XElement("player", new XAttribute("gang", PlayerGangId ?? ""),
                    new XAttribute("rank", PlayerRank)),
                new XElement("playerReputations", playerReputation.OrderBy(p => p.Key).Select(p =>
                    new XElement("reputation", new XAttribute("gang", p.Key), new XAttribute("value", p.Value)))),
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
            IsDirty = false;
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
            XElement reputations = doc.Root.Element("playerReputations");
            if (reputations != null) foreach (XElement element in reputations.Elements("reputation"))
            {
                string id = (string)element.Attribute("gang");
                if (!string.IsNullOrWhiteSpace(id) && loaded.organizations.ContainsKey(id))
                    loaded.playerReputation[id] = ReadInt(element, "value", 0, 0, 100);
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
            playerReputation.Clear();
            foreach (var pair in loaded.playerReputation) playerReputation.Add(pair.Key, pair.Value);
            organizations.Clear();
            foreach (var pair in loaded.organizations) organizations.Add(pair.Key, pair.Value);
            relations.Clear();
            foreach (var pair in loaded.relations) relations.Add(pair.Key, pair.Value);
            cartels.Clear();
            foreach (var pair in loaded.cartels) cartels.Add(pair.Key, pair.Value);
            PlayerGangId = loaded.PlayerGangId;
            PlayerRank = loaded.PlayerRank;
            IsDirty = false;
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
