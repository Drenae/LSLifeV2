using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class DriveBy
{
	public List<Ped> Peds = new List<Ped>();

	private Vehicle Vehicle;

	private VehicleHash vehicleHash;

	private VehicleClass vehicleClass;

	private int pedamount;

	private bool enterState;

	public bool Dead;

	private PedGroup Group = new PedGroup();

	internal Ped Leader;

	internal bool Agressive;

	internal int timeSeen;

	private Vector3 LastKnownPosition;

	private Ped Target;

	private int time;

	private Vector3 lastSpawn;

	public Enums.DriveBySates DriveBySate { get; private set; }

	public Vector3 DefendPosition
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public DriveBy(VehicleClass _class, int _pedAmount, Ped _target, bool _agressive)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		DefendPosition = LSL.playerPos;
		LastKnownPosition = LSL.playerPos;
		vehicleClass = _class;
		pedamount = _pedAmount;
		Target = _target;
		Agressive = _agressive;
		SetVehicleClass();
		SpawnVehicle();
		SpawnPeds();
		_ = Peds.Count;
		_ = 0;
	}

	public void OnTick()
	{
		CheckPeds();
		CheckVehicle();
		DoStuff();
	}

	internal void SetNewTarget(Ped _ped)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Target = _ped;
		LastKnownPosition = ((Entity)_ped).Position;
		DefendPosition = LastKnownPosition;
	}

	internal void ChangeBlip(bool _agg)
	{
		if (_agg)
		{
			if (!Agressive)
			{
				((Entity)Vehicle).AttachedBlip.Color = (BlipColor)1;
				((Entity)Vehicle).AttachedBlip.Name = "Driveby Vehicle";
			}
		}
		else if (Agressive)
		{
			((Entity)Vehicle).AttachedBlip.Color = (BlipColor)61;
			((Entity)Vehicle).AttachedBlip.Name = "Patrol";
		}
	}

	private void DoStuff()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		if (!Dead && LSL.playerDead)
		{
			goto IL_0047;
		}
		Vector3 position;
		if ((Entity)(object)Target != (Entity)null)
		{
			Ped target = Target;
			if (target != null)
			{
				position = ((Entity)target).Position;
				if (position.DistanceTo(DefendPosition) > 500f)
				{
					goto IL_0047;
				}
			}
		}
		goto IL_0055;
		IL_0055:
		if ((Entity)(object)Leader != (Entity)null && ((Entity)Leader).Exists() && (Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists())
		{
			if (Agressive)
			{
				CheckOtherDrivebys();
				switch (DriveBySate)
				{
				case Enums.DriveBySates.VehDrive:
				{
					if (enterState)
					{
						enterState = false;
						if ((Entity)(object)Leader != (Entity)null && ((Entity)Leader).Exists() && (Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists())
						{
							Leader.Task.DriveTo(Vehicle, LastKnownPosition, 10f, (GTA.VehicleDrivingFlags)(6), 17f);
						}
					}
					bool seen = false;
					foreach (Ped ped in Peds)
					{
						if (LsFunctions.CanSee((Entity)(object)ped, (Entity)(object)Target))
						{
							seen = true;
							timeSeen = Game.GameTime;
							break;
						}
					}
					if (seen)
					{
						LastKnownPosition = LSL.playerPos;
						DriveBySate = Enums.DriveBySates.VehAttack;
						enterState = true;
					}
					else if ((Entity)(object)Leader != (Entity)null && ((Entity)Leader).Exists() && (Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists())
					{
						position = ((Entity)Leader).Position;
						if (position.DistanceTo(LastKnownPosition) < 10f)
						{
							DriveBySate = Enums.DriveBySates.VehPatrol;
							enterState = true;
						}
					}
					break;
				}
				case Enums.DriveBySates.VehPatrol:
					if (enterState)
					{
						enterState = false;
						time = Game.GameTime;
						if ((Entity)(object)Leader != (Entity)null && ((Entity)Leader).Exists() && (Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists())
						{
							Ped leader2 = Leader;
							if (leader2 != null)
							{
								leader2.Task.CruiseWithVehicle(Vehicle, 13f, (GTA.VehicleDrivingFlags)(5));
							}
						}
					}
					if (LsFunctions.CanSee((Entity)(object)Leader, (Entity)(object)Target))
					{
						timeSeen = Game.GameTime;
						LastKnownPosition = LSL.playerPos;
						DriveBySate = Enums.DriveBySates.VehAttack;
						enterState = true;
						break;
					}
					if ((Entity)(object)Leader != (Entity)null && ((Entity)Leader).Exists() && (Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists())
					{
						position = ((Entity)Vehicle).Position;
						if (position.DistanceTo(LastKnownPosition) > 300f)
						{
							DriveBySate = Enums.DriveBySates.VehDrive;
							enterState = true;
							break;
						}
					}
					if (Game.GameTime > time + 120000)
					{
						DriveBySate = Enums.DriveBySates.CleanDriveBy;
						enterState = true;
					}
					break;
				case Enums.DriveBySates.VehAttack:
					if (enterState)
					{
						enterState = false;
						Ped leader = Leader;
						if (leader != null)
						{
							leader.Task.VehicleChase(Target);
						}
					}
					if (!LsFunctions.CanSee((Entity)(object)Leader, (Entity)(object)Target))
					{
						timeSeen = Game.GameTime;
						LastKnownPosition = LSL.playerPos;
						DriveBySate = Enums.DriveBySates.VehDrive;
						enterState = true;
					}
					break;
				}
			}
			else
			{
				switch (DriveBySate)
				{
				case Enums.DriveBySates.VehDrive:
					if (enterState)
					{
						enterState = false;
						Leader.Task.DriveTo(Vehicle, LastKnownPosition, 10f, (GTA.VehicleDrivingFlags)(6), 17f);
					}
					position = ((Entity)Leader).Position;
					if (position.DistanceTo(LastKnownPosition) < 20f)
					{
						DriveBySate = Enums.DriveBySates.VehPatrol;
						enterState = true;
					}
					break;
				case Enums.DriveBySates.VehPatrol:
					if (enterState)
					{
						enterState = false;
						time = Game.GameTime;
						Leader.Task.CruiseWithVehicle(Vehicle, 13f, (GTA.VehicleDrivingFlags)(5));
					}
					else
					{
						position = ((Entity)Vehicle).Position;
						if (position.DistanceTo(LastKnownPosition) > 300f)
						{
							DriveBySate = Enums.DriveBySates.VehDrive;
							enterState = true;
						}
					}
					if (Game.GameTime > time + 120000)
					{
						DriveBySate = Enums.DriveBySates.CleanDriveBy;
						enterState = true;
					}
					break;
				}
			}
		}
		if (enterState && DriveBySate == Enums.DriveBySates.CleanDriveBy)
		{
			LsFunctions.DbugString("DriveByCleaned");
			enterState = false;
			Dead = true;
			CleanDriveBy();
		}
		return;
		IL_0047:
		DriveBySate = Enums.DriveBySates.CleanDriveBy;
		enterState = true;
		goto IL_0055;
	}

	private void CheckOtherDrivebys()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		foreach (DriveBy d in LSL.driveByHandler.GetDriveBies.Where((DriveBy r) => r.timeSeen > timeSeen))
		{
			if (DriveBySate == Enums.DriveBySates.VehPatrol || (DriveBySate == Enums.DriveBySates.VehDrive && d.timeSeen > timeSeen))
			{
				timeSeen = d.timeSeen;
				LastKnownPosition = d.LastKnownPosition;
				DriveBySate = Enums.DriveBySates.VehDrive;
				enterState = true;
				LsFunctions.DbugString("Driveby updated position from team");
			}
		}
	}

	private void CheckVehicle()
	{
		if ((Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists() && Peds.Count == 0)
		{
			RemoveVehicle();
		}
	}

	private void RemoveVehicle()
	{
		((Entity)Vehicle).AttachedBlip.Delete();
		((Entity)Vehicle).MarkAsNoLongerNeeded();
		LSL.entitiesCleanUp.Add((Entity)(object)Vehicle);
		Vehicle = null;
	}

	public void CleanDriveBy()
	{
		if (Peds.Count > 0)
		{
			foreach (Ped p in Peds.ToList())
			{
				RemovePed(p);
			}
		}
		if ((Entity)(object)Vehicle != (Entity)null)
		{
			RemoveVehicle();
		}
		Group.Dispose();
		LSL.driveByHandler.GetDriveBies.Remove(this);
	}

	private void RemovePed(Ped p)
	{
		if ((Entity)(object)p != (Entity)(object)LSL.attacker)
		{
			LsFunctions.RemovePedFromWorld(p, clearTask: true);
		}
		Peds.Remove(p);
	}

	private void CheckPeds()
	{
		if (Peds.Count > 0)
		{
			foreach (Ped p in Peds.ToList())
			{
				if ((Entity)(object)p != (Entity)null && ((Entity)p).Exists())
				{
					if (!p.IsInVehicle() && !((Entity)p).AttachedBlip.Exists())
					{
						LsFunctions.AddBlip((Entity)(object)p, (BlipColor)1, "Driveby Ped", _friend: false, _edge: false);
					}
					if (((Entity)p).IsDead)
					{
						if (LSL.areas[LSL.areaIndex].GangPresance > 0f)
						{
							LSL.areas[LSL.areaIndex].GangPresance--;
						}
						LsFunctions.RemovePedFromWorld(p, clearTask: true);
						Peds.Remove(p);
					}
				}
				else
				{
					Peds.Remove(p);
				}
			}
		}
		if (Peds.Count == 0)
		{
			DriveBySate = Enums.DriveBySates.CleanDriveBy;
			enterState = true;
		}
		else
		{
			if (!((Entity)(object)Leader != (Entity)(object)Peds.First()))
			{
				return;
			}
			Leader = Peds.First();
			foreach (Ped p2 in Peds)
			{
				if (p2.PedGroup == Group)
				{
					Group.Remove(p2);
				}
				if ((Entity)(object)p2 == (Entity)(object)Leader)
				{
					Group.Add(p2, true);
				}
				else
				{
					Group.Add(p2, false);
				}
			}
		}
	}

	private void SpawnVehicle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 defendPosition = DefendPosition;
		Vector3 spawnPos = LSL.GenerateSpawnPos(defendPosition.Around(300f), LSL.Nodetype.Road, sidewalk: false);
		while (spawnPos == lastSpawn || spawnPos.DistanceTo(LSL.playerPos) < 200f)
		{
			defendPosition = DefendPosition;
			spawnPos = LSL.GenerateSpawnPos(defendPosition.Around(300f), LSL.Nodetype.Road, sidewalk: false);
		}
		lastSpawn = spawnPos;
		Vehicle = World.CreateVehicle(LsFunctions.RequestModel(vehicleHash), spawnPos);
		int time = Game.GameTime;
		while ((Entity)(object)Vehicle == (Entity)null && Game.GameTime < time + 1000)
		{
			Script.Wait(0);
		}
		if ((Entity)(object)Vehicle != (Entity)null)
		{
			if (Agressive)
			{
				LsFunctions.AddBlip((Entity)(object)Vehicle, (BlipColor)1, "Driveby Vehicle", _friend: false, _edge: false);
			}
			((Entity)Vehicle).IsPersistent = true;
		}
		else if (LSL.DEBUG)
		{
			UI.Notify("~r~Impossible de faire apparaître l'attaque en voiture.");
		}
	}

	private void SpawnPeds()
	{
		SpawnPed(unchecked((VehicleSeat)(-1)), _leader: true);
		if (pedamount <= 0)
		{
			return;
		}
		SpawnPed((VehicleSeat)0, _leader: false);
		if (pedamount > 1)
		{
			SpawnPed((VehicleSeat)2, _leader: false);
			if (pedamount > 2)
			{
				SpawnPed((VehicleSeat)1, _leader: false);
			}
		}
	}

	private void SpawnPed(VehicleSeat _seat, bool _leader)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected I4, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Ped p = Vehicle.CreateRandomPedOnSeat(_seat);
		if (!((Entity)(object)p != (Entity)null) || !((Entity)p).Exists())
		{
			return;
		}
		((Entity)p).IsPersistent = true;
		if (Agressive)
		{
			p.RelationshipGroup = LSL.rival_enemy;
		}
		else
		{
			p.RelationshipGroup = LSL.rival_nutral;
		}
		LsFunctions.SetRandomCombatStats(p);
		p.FiringPattern = unchecked((FiringPattern)(-753768974));
		p.AlwaysKeepTask = true;
		p.CanSwitchWeapons = true;
		Function.Call(unchecked((Hash)(-4079649877180351064L)), (p), (100));
		LsFunctions.SetRivalWeapons(p);
		if (_leader)
		{
			Leader = p;
			p.AlwaysKeepTask = true;
			DrivingStyle style = (DrivingStyle)786603;
			if (Agressive)
			{
				style = (DrivingStyle)6;
			}
			p.Task.DriveTo(Vehicle, LastKnownPosition, 10f, (GTA.VehicleDrivingFlags)((int), 20f)style);
		}
		else if (Agressive)
		{
			p.Task.VehicleShootAtPed(Target);
		}
		Peds.Add(p);
	}

	private void SetVehicleClass()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Vehicle[] vehs = World.GetNearbyVehicles(Target, 200f);
		vehicleHash = (VehicleHash)142944341;
		if (vehs.ToList().Count <= 0)
		{
			return;
		}
		Vehicle[] array = vehs;
		foreach (Vehicle v in array)
		{
			if (v.ClassType == vehicleClass)
			{
				Model model = ((Entity)v).Model;
				vehicleHash = (VehicleHash)model.Hash;
				if (LSL.DEBUG)
				{
					UI.Notify("foundSUV");
				}
				break;
			}
		}
	}
}
