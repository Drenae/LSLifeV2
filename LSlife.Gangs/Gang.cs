using System.Collections.Generic;
using GTA;

namespace LSlife.Gangs;

public class Gang
{
	internal string Name;

	internal int Money = 100000;

	internal int MaxMembers;

	internal int TotalMembers;

	internal ThreatHandler ThreatHandler = new ThreatHandler();

	internal Area BaseArea;

	internal List<AreaDealer> AreaDealers = new List<AreaDealer>();

	public Gang(string _name, int _money, int _maxMem, int _totalMem, string _baseArea)
	{
		Name = _name;
		Money = _money;
		SetMaxMembers(_maxMem);
		SetTotalMembers(_totalMem);
		BaseArea = LsFunctions.GetAreaFromString(_baseArea);
	}

	private void OnTick()
	{
		ThreatHandler.CheckForNewThreats(this);
	}

	private void SetTotalMembers(int _totalMem)
	{
		if (_totalMem > MaxMembers)
		{
			TotalMembers = MaxMembers;
		}
		else
		{
			TotalMembers = _totalMem;
		}
	}

	private void SetMaxMembers(int _maxMem)
	{
		if (_maxMem > 100)
		{
			MaxMembers = 100;
		}
		else
		{
			MaxMembers = _maxMem;
		}
	}

	private void Patrol(Ped _target)
	{
		LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: false);
	}

	private void DriveBy(Ped _target)
	{
		LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
	}
}
