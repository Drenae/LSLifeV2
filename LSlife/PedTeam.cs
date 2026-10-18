using System.Collections.Generic;
using GTA;

namespace LSlife;

public class PedTeam
{
	private PedGroup DefenceTeam = new PedGroup();

	private List<PlayerDealer> Peds = new List<PlayerDealer>();

	private Vehicle Vehicle;

	private int MaxTeamSize = 1;

	public string Area { get; private set; } = "";

	public PedTeam(PlayerDealer _dealer, string _area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		DefenceTeam.Add(_dealer.Ped, true);
		Peds.Add(_dealer);
		Area = _area;
	}

	public void AddMember(PlayerDealer _dealer)
	{
		if (Peds.Count < MaxTeamSize)
		{
			DefenceTeam.Add(_dealer.Ped, false);
			Peds.Add(_dealer);
		}
		else
		{
			LsFunctions.SendTextMsg("Nombre maximal de membres atteint.");
		}
	}
}
