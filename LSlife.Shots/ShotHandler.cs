using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife.Shots;

internal static class ShotHandler
{
	public static List<Shot> shots = new List<Shot>();

	private const int shotCoolDown = 20000;

	private static int LastShotTime = 0;

	public static int currentShot = 99;

	private static bool routeSet = false;

	public static bool ShotOffers => shots.Where((Shot s) => s.shotOffered).Count() > 0;

	public static void Tick()
	{
		if (shots.Count > 0)
		{
			foreach (Shot s in shots.ToList())
			{
				if (s == null)
				{
					shots.Remove(s);
				}
				else
				{
					s.Tick();
				}
			}
			return;
		}
		currentShot = 0;
	}

	public static void OnSlowTick()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		if (!ShotOffers && shots.Where((Shot s) => s.accepted).Count() > 0)
		{
			float cDist = 1000f;
			int cIndex = 0;
			int handle = 0;
			for (int i = 0; i < shots.Count; i++)
			{
				Shot shot = shots[i];
				val = ((Entity)shots[i].customer.Ped).Position;
				shot.dist = val.DistanceTo(LSL.playerPos);
				if (shots[i].dist < cDist)
				{
					cDist = shots[i].dist;
					cIndex = i;
					handle = ((Entity)shots[i].customer.Ped).Handle;
				}
			}
			if (handle != currentShot)
			{
				currentShot = handle;
				((Entity)shots[cIndex].customer.Ped).CurrentBlip.ShowRoute = true;
			}
		}
		if (Game.GameTime <= LastShotTime + 20000 || ShotOffers)
		{
			return;
		}
		LastShotTime = Game.GameTime;
		int totalDrugs = LSL.TotalDrugsCarried() + LSL.HouseHandler.TotalStashDrugs();
		if (LSL.pStashVehicle != null)
		{
			totalDrugs += LSL.pStashVehicle.TotalDrugs();
		}
		if (LSL.PottentialWorker != null || totalDrugs <= 56 || shots.Count >= 10 || !((double)LSL.rnd.Next(0, 100) > 70.0 - LsFunctions.CurrentRepLvl(LSL.areas[LSL.areaIndex].Reputation) * 2.0))
		{
			return;
		}
		Vector3 spawnPos = LSL.GenerateSpawnPos(LSL.playerPos.Around(275f), LSL.Nodetype.Offroad, sidewalk: true);
		if (spawnPos == Vector3.Zero)
		{
			if (LSL.DEBUG)
			{
				UI.Notify("Nouvelle livraison créée : client trouvé.");
			}
			spawnPos = LSL.GenerateSpawnPos(LSL.playerPos.Around(200f), LSL.Nodetype.Offroad, sidewalk: true);
		}
		for (int j = 0; j < 100; j++)
		{
			if (!Function.Call<bool>((Hash)1322919935073282825L, (spawnPos.X), (spawnPos.Y), (spawnPos.Z)))
			{
				break;
			}
			spawnPos = LSL.GenerateSpawnPos(LSL.playerPos.Around(200f), LSL.Nodetype.Offroad, sidewalk: true);
			spawnPos = World.GetNextPositionOnSidewalk(spawnPos);
		}
		List<Ped> peds = LSL.lPeds.GetPeds;
		if (peds.ToList().Count <= 0)
		{
			return;
		}
		foreach (Ped p in peds)
		{
			if (p.IsPlayer || !p.IsHuman || LSL.rnd.Next(0, 10) <= 3)
			{
				continue;
			}
			Ped newPed = World.CreatePed(((Entity)p).Model, spawnPos);
			if (LSL.DEBUG)
			{
				val = spawnPos;
				string text = ((object)val/*cast due to constrained. prefix*/).ToString();
				val = ((Entity)newPed).Position;
				UI.Notify("Position livraison : " + text + " ; position client : " + ((object)val/*cast due to constrained. prefix*/).ToString());
			}
			if (!((Entity)(object)newPed != (Entity)null))
			{
				continue;
			}
			newPed.RelationshipGroup = p.RelationshipGroup;
			List<string> pDrugs = new List<string>();
			if (LSL.sellingWeed)
			{
				pDrugs.Add("Weed");
			}
			if (LSL.sellingCrack)
			{
				pDrugs.Add("Crack");
			}
			if (LSL.sellingCocaine)
			{
				pDrugs.Add("Cocaine");
			}
			Drug drug = null;
			switch (pDrugs[LSL.rnd.Next(0, pDrugs.Count)])
			{
			case "Weed":
				drug = LSL.areas[LSL.areaIndex].Drugs.Find((Drug d) => d.Name == "Weed");
				break;
			case "Crack":
				drug = LSL.areas[LSL.areaIndex].Drugs.Find((Drug d) => d.Name == "Crack");
				break;
			case "Cocaine":
				drug = LSL.areas[LSL.areaIndex].Drugs.Find((Drug d) => d.Name == "Cocaine");
				break;
			}
			AddShot(new Customer(newPed, spawnPos, farAway: true, null, drug, (int)LsFunctions.CurrentRepLvl(LSL.areas[LSL.areaIndex].Reputation)));
			break;
		}
	}

	private static void AddShot(Customer customer)
	{
		shots.Add(new Shot(customer));
	}

	public static void RemoveShot(Shot _s)
	{
		shots.Remove(_s);
	}
}
