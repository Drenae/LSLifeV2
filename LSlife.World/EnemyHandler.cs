using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

internal class EnemyHandler
{
	private List<Vector3> SpawnLocations = new List<Vector3>
	{
		new Vector3(-131.695f, -1509.14f, 34.10052f),
		new Vector3(-161.0194f, -1515.375f, 33.68655f),
		new Vector3(-172.2392f, -1492.894f, 32.55872f)
	};

	private Vector3 ClosePos;

	private List<Ped> Enemies = new List<Ped>();

	private int MaxPeds = 15;

	private int TimeBetweenSpawns = 500;

	private int Time;

	private List<PedHash> pedHashes = new List<PedHash>
	{
		unchecked((PedHash)(-1676424299)),
		unchecked((PedHash)(-1863364300))
	};

	private Random Rand = new Random();

	public bool Started { get; private set; }

	public EnemyHandler()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
	}

	public void Ontick()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (Started && Game.GameTime > Time + TimeBetweenSpawns && Enemies.Count < MaxPeds)
		{
			int index = -1;
			bool posFound = false;
			ClosePos = SpawnLocations[0];
			foreach (Vector3 spawnLocation in SpawnLocations)
			{
				Vector3 v = spawnLocation;
				if (v.DistanceTo(((Entity)Game.Player.Character).Position) < ClosePos.DistanceTo(((Entity)Game.Player.Character).Position))
				{
					ClosePos = v;
				}
			}
			while (!posFound)
			{
				index = Rand.Next(0, SpawnLocations.Count);
				if (SpawnLocations[index] != ClosePos)
				{
					posFound = true;
				}
			}
			int pedIndex = Rand.Next(0, 2);
			Ped enemy = Ped.Create(new Model(pedHashes[pedIndex]), SpawnLocations[index]);
			while ((Entity)(object)enemy == (Entity)null)
			{
				Script.Wait(0);
			}
			switch (pedIndex)
			{
			case 0:
				enemy.Weapons.Give(unchecked((WeaponHash)(-2084633992)), 1000, true, true);
				Function.Call((Hash)5592521861259579479L, (enemy), (2));
				break;
			case 1:
				enemy.Weapons.Give((WeaponHash)1119849093, 10000, true, true);
				enemy.CanSufferCriticalHits = false;
				((Entity)enemy).Health = 4000;
				Function.Call((Hash)5592521861259579479L, (enemy), (3));
				break;
			}
			Function.Call(unchecked((Hash)(-4079649877180351064L)), (enemy), (100));
			enemy.FiringPattern = unchecked((FiringPattern)(-957453492));
			enemy.CanWrithe = false;
			enemy.Armor = 100;
			enemy.KeepTaskWhenMarkedAsNoLongerNeeded = true;
			enemy.Task.Combat(Game.Player.Character);
			((Entity)enemy).AddBlip();
			((Entity)enemy).AttachedBlip.IsFriendly = false;
			Enemies.Add(enemy);
			Time = Game.GameTime;
		}
		CheckPeds();
	}

	public void Start()
	{
		Started = true;
	}

	public void Stop()
	{
		Started = false;
	}

	private void CheckPeds()
	{
		if (Enemies.Count <= 0)
		{
			return;
		}
		foreach (Ped p in Enemies.ToList())
		{
			if ((Entity)(object)p == (Entity)null || !((Entity)p).Exists())
			{
				Enemies.Remove(p);
			}
			else if (((Entity)p).IsDead)
			{
				((Entity)p).AttachedBlip.Delete();
				Enemies.Remove(p);
			}
		}
	}

	private void RemoveEnemies()
	{
		foreach (Ped p in Enemies.ToList())
		{
			((Entity)p).AttachedBlip.Delete();
			Enemies.Remove(p);
		}
	}
}
