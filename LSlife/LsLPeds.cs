using System.Collections.Generic;
using System.Linq;
using GTA;

namespace LSlife;

public class LsLPeds
{
	private int ptime;

	private int pdelay = 7000;

	public List<Ped> GetPeds { get; private set; }

	public LsLPeds()
	{
		GetPeds = (from p in World.GetNearbyPeds(Game.Player.Character, 200f)
			where !LSL.CheckIfPedUsed(p, _allowPolice: true, _allowVehicle: true)
			select p).ToList();
		ptime = Game.GameTime;
	}

	public void Ontick()
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (Game.GameTime > ptime + pdelay)
		{
			ptime = Game.GameTime;
			foreach (Ped p in new List<Ped>(World.GetNearbyPeds(Game.Player.Character, 200f)))
			{
				if (!LSL.CheckIfPedUsed(p, _allowPolice: true, _allowVehicle: true) && !GetPeds.Contains(p))
				{
					GetPeds.Add(p);
				}
			}
		}
		foreach (Ped p2 in GetPeds.ToList())
		{
			if ((Entity)(object)p2 == (Entity)null || !((Entity)p2).Exists() || LSL.playerPos.DistanceTo(((Entity)p2).Position) > 200f || LSL.CheckIfPedUsed(p2, _allowPolice: true, _allowVehicle: true))
			{
				GetPeds.Remove(p2);
			}
		}
	}
}
