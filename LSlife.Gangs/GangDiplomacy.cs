using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LSlife.Gangs
{
    internal enum GangRelationStatus { Hostile, Neutral, NonAggression, Allied }

    internal sealed class GangDiplomacy
    {
        private readonly Dictionary<string, GangRelationStatus> relations = new Dictionary<string, GangRelationStatus>(StringComparer.OrdinalIgnoreCase);

        private static string Key(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second) || string.Equals(first, second, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid gang pair");
            return string.Compare(first, second, StringComparison.OrdinalIgnoreCase) < 0 ? first + "|" + second : second + "|" + first;
        }

        internal GangRelationStatus Get(string first, string second)
        {
            GangRelationStatus status;
            return relations.TryGetValue(Key(first, second), out status) ? status : GangRelationStatus.Neutral;
        }

        internal void Set(string first, string second, GangRelationStatus status)
        {
            if (!Enum.IsDefined(typeof(GangRelationStatus), status)) throw new ArgumentOutOfRangeException("status");
            relations[Key(first, second)] = status;
        }
    }
}
