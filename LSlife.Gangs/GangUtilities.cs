using System.Collections.Generic;

namespace LSlife.Gangs;

internal static class GangUtilities
{
	private const int memberCost = 10000;

	public const int moneyCap = 999999999;

	public const int memberCap = 100;

	public static Dictionary<int, string> LsLifeGangs = new Dictionary<int, string>
	{
		{ 0, "Armenian Mob" },
		{ 1, "Aztecas" },
		{ 2, "Ballas" },
		{ 3, "Families" },
		{ 4, "Kkangpae" },
		{ 5, "The Lost MC" },
		{ 6, "Madrazo Cartel" },
		{ 7, "Marabunta Grande" },
		{ 8, "Madrazo Cartel" },
		{ 9, "Marabunta Grande" },
		{ 10, "Triads" },
		{ 11, "Vagos" }
	};

	public static bool CanGangGrow(Gang _g)
	{
		if (10000 <= _g.Money)
		{
			return _g.MaxMembers > _g.TotalMembers;
		}
		return false;
	}

	public static void GrowGang(Gang _g)
	{
		_g.TotalMembers++;
		_g.Money -= 10000;
	}

	public static int AllocatedMemberCount(Gang _g)
	{
		int mems = 0;
		foreach (Threat t in _g.ThreatHandler.RespondingTo)
		{
			mems += t.Allocated;
		}
		return mems;
	}
}
