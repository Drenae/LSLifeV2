using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;

namespace LSlife.Gangs;

internal class ThreatHandler
{
	public List<Threat> Threats = new List<Threat>();

	internal List<Threat> RespondingTo => Threats.Where((Threat t) => t.Responding).ToList();

	internal int ThreatCount => Threats.Count;

	internal void CheckForNewThreats(Gang _g)
	{
		if (Threats.Count <= 0)
		{
			return;
		}
		int freeMembers = _g.TotalMembers - GangUtilities.AllocatedMemberCount(_g);
		if (freeMembers <= 0)
		{
			return;
		}
		List<Threat> cThreats = ThreatsAtHome(_g);
		if (cThreats.Count == 0)
		{
			cThreats = Threats;
		}
		foreach (Threat t in cThreats)
		{
			if (!t.Responding && t.HasMembersForResponce(freeMembers))
			{
				RespondToThreat(t, t.Level);
				freeMembers -= t.Level;
			}
			if (freeMembers == 0)
			{
				break;
			}
		}
	}

	internal void RespondToThreats()
	{
	}

	internal List<Threat> ThreatsAtHome(Gang _g)
	{
		return Threats.Where((Threat t) => t.Area == _g.BaseArea).ToList();
	}

	private void RespondToThreat(Threat _threat, int _members)
	{
		_threat.SetResponding(_resp: true, _members);
	}

	internal void AddThreat(Area _area, Vector3 _pos, Ped _ped, int _lvl)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Threats.Add(new Threat(_area, _pos, _ped, _lvl));
	}

	internal void RemoveThreat(Threat _threat)
	{
		Threats.Remove(_threat);
	}
}
