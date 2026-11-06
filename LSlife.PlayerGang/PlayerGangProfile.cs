using System;
using System.Collections.Generic;
using System.Linq;

namespace LSlife;

// Identite du reseau du joueur. Ne derive pas de Gang (spawns et menaces rivaux).
internal sealed class PlayerGangProfile
{
    internal const int MaxNameLength = 24;
    internal static readonly string[] ColorKeys = { "gold", "blue", "red", "purple", "green", "orange", "cyan", "white" };
    internal Guid Id { get; }
    internal string Name { get; }
    internal string ColorKey { get; }
    internal string LeaderId => "player"; // Meme portee que les sauvegardes LS Life existantes.
    internal DateTime CreatedUtc { get; }
    internal IReadOnlyList<int> DealerIds { get; }
    internal IReadOnlyList<string> TerritoryIds { get; }

    internal PlayerGangProfile(Guid id, string name, string colorKey, DateTime createdUtc,
        IEnumerable<int> dealerIds, IEnumerable<string> territoryIds)
    {
        if (id == Guid.Empty) throw new ArgumentException("Identifiant de gang vide.");
        Name = ValidateName(name);
        if (!ColorKeys.Contains(colorKey)) throw new ArgumentException("Couleur de gang inconnue.");
        if (createdUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Date UTC requise.");
        var dealers = dealerIds.ToArray();
        var territories = territoryIds.ToArray();
        if (dealers.Any(idValue => idValue < 0) || dealers.Distinct().Count() != dealers.Length)
            throw new ArgumentException("References de dealers invalides ou dupliquees.");
        if (territories.Any(string.IsNullOrWhiteSpace) || territories.Distinct(StringComparer.Ordinal).Count() != territories.Length)
            throw new ArgumentException("References de quartiers invalides ou dupliquees.");
        Id = id;
        ColorKey = colorKey;
        CreatedUtc = createdUtc;
        DealerIds = Array.AsReadOnly(dealers);
        TerritoryIds = Array.AsReadOnly(territories);
    }

    internal static string ValidateName(string value)
    {
        string name = (value ?? "").Trim();
        if (name.Length < 2 || name.Length > MaxNameLength || !name.Any(char.IsLetterOrDigit) ||
            name.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '\''))
            throw new ArgumentException("Nom : 2 a 24 caracteres (lettres, chiffres, espaces, tirets, apostrophes).");
        return name;
    }

    internal PlayerGangProfile WithIdentity(string name, string colorKey) =>
        new PlayerGangProfile(Id, name, colorKey, CreatedUtc, DealerIds, TerritoryIds);
}
