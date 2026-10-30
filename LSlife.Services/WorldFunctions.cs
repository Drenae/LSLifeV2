using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.OldCustomers;
using LSlife.Shots;


namespace LSlife;

internal static partial class LsFunctions
{
	public static Vector3 RoadSideFromSpawn(Vector3 spawnPos, Vector3 directionPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 directionFromSpawn = directionPos - spawnPos;
		directionFromSpawn = directionFromSpawn.Normalized;
		Vector3 newSpawnPos = spawnPos + directionFromSpawn;
		Vector3 oldSpawnPos = newSpawnPos;
		Vector3 finalSpawnPos = Vector3.Zero;
		for (int i = 0; i < 16; i++)
		{
			newSpawnPos.Z = GetGroundHeightCompat(newSpawnPos);
			if (Function.Call<bool>((Hash)1322919935073282825L, (newSpawnPos.X), (newSpawnPos.Y), (newSpawnPos.Z)) || newSpawnPos.DistanceTo(spawnPos) < newSpawnPos.DistanceTo(directionPos))
			{
				oldSpawnPos = newSpawnPos;
				newSpawnPos += directionFromSpawn * 0.5f;
				continue;
			}
			finalSpawnPos = oldSpawnPos;
			finalSpawnPos.Z = GetGroundHeightCompat(finalSpawnPos);
			return finalSpawnPos;
		}
		return finalSpawnPos;
	}

	public static Vector2 World3DToScreen2d(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		OutputArgument x2dp = new OutputArgument();
		OutputArgument y2dp = new OutputArgument();
		Function.Call((Hash)3812348786834502901L, (pos.X), (pos.Y), (pos.Z), (InputArgument)(object)x2dp, (InputArgument)(object)y2dp);
		return new Vector2(x2dp.GetResult<float>(), y2dp.GetResult<float>());
	}

	public static bool ShouldSpeed(Vector3 vPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Vector3 closestRoadPos = GetClosestRoadPos(vPos);
		return closestRoadPos.DistanceTo(vPos) > 2.4f;
	}

	public static float GetRoadSpeed(Vector3 pos, bool canSpeed, bool _shouldSpeed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		List<LSL.PathnodeFlags> roadFlags = LSL.GetRoadFlags(pos);
		bool shouldSpeed = false;
		float speed = 8f;
		if (canSpeed)
		{
			shouldSpeed = _shouldSpeed;
		}
		foreach (LSL.PathnodeFlags item in roadFlags)
		{
			switch (item)
			{
			case LSL.PathnodeFlags.Two:
				speed = 16f;
				if (shouldSpeed)
				{
					speed = 35f;
				}
				break;
			case LSL.PathnodeFlags.Freeway:
				speed = 35f;
				if (shouldSpeed)
				{
					speed = 50f;
				}
				break;
			case LSL.PathnodeFlags.Slow:
				speed = 9f;
				if (shouldSpeed)
				{
					speed = 30f;
				}
				break;
			case LSL.PathnodeFlags.Eight:
				speed = 16f;
				break;
			case LSL.PathnodeFlags.SlowTraffic:
				speed = 14f;
				if (shouldSpeed)
				{
					speed = 16f;
				}
				break;
			case LSL.PathnodeFlags.FourWayIntersection:
				speed = 11f;
				if (shouldSpeed)
				{
					speed = 13f;
				}
				break;
			case LSL.PathnodeFlags.Intersection:
				if (shouldSpeed)
				{
					speed = 13f;
				}
				speed = 11f;
				break;
			default:
				speed = 8f;
				if (shouldSpeed)
				{
					speed = 13f;
				}
				break;
			}
		}
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (pos.X), (pos.Y), (pos.Z), (1), (1), (200f), (200f));
		if (Function.Call<bool>((Hash)5715191804072465017L, (NodeID)))
		{
			speed = ((!((speed * 0.5f > 9f) & shouldSpeed)) ? 9f : (speed * 0.5f));
		}
		return speed;
	}

	public static Vector3 GetClosestRoadPos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		OutputArgument outArgA = new OutputArgument();
		if (Function.Call<bool>((Hash)2596914974566212883L, (pos.X), (pos.Y), (pos.Z), (InputArgument)(object)outArgA, (1), (3f), (0)))
		{
			return outArgA.GetResult<Vector3>();
		}
		return pos;
	}

	public static void AddBlip(Entity _ent, BlipColor _col, string _name, bool _friend, bool _edge)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (_ent.AttachedBlip == null || !_ent.AttachedBlip.Exists())
		{
			_ent.AddBlip();
		}
		Blip currentBlip = _ent.AttachedBlip;
		currentBlip.IsFriendly = _friend;
		currentBlip.IsShortRange = _edge;
		currentBlip.Color = _col;
		currentBlip.Name = _name;
	}

	public static void RemovePedFromWorld(Ped ped, bool clearTask)
	{
		if ((Entity)(object)ped.LastVehicle == (Entity)(object)LSL.player.Character.LastVehicle)
		{
			Function.Call(unchecked((Hash)(-4932030039357269829L)), (ped));
		}
		Blip currentBlip = ((Entity)ped).AttachedBlip;
		if (currentBlip != null)
		{
			currentBlip.Delete();
		}
		ped.LeaveGroup();
		if (clearTask)
		{
			ped.Task.ClearAll();
		}
		ped.BlockPermanentEvents = false;
		((Entity)ped).IsPersistent = false;
		((Entity)ped).MarkAsNoLongerNeeded();
		LSL.entitiesCleanUp.Add((Entity)(object)ped);
	}

	public static bool IsInInterior(Entity _entity)
	{
		int interior = Function.Call<int>((Hash)2380075781929936571L, (_entity));
		foreach (int allowedInterior in AllowedInteriors)
		{
			if (allowedInterior == interior)
			{
				return false;
			}
		}
		if (interior != 0)
		{
			return true;
		}
		return false;
	}

	private static float GetGroundHeightCompat(Vector3 position)
	{
		return World.GetGroundHeight(position, out float height) ? height : position.Z;
	}

	public static bool IsUnderGround(Entity _entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = _entity.Position;
		float eZ = position.Z;
		if (GetGroundHeightCompat(position) > eZ)
		{
			return true;
		}
		return false;
	}

	public static int IsNearStash()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		int whatNear = 0;
		if (LSL.pStashVehicle != null && (Entity)(object)LSL.pStashVehicle.drugCar != (Entity)null && LSL.playerPos.DistanceTo(LSL.pStashVehicle.StashPos()) < 3f)
		{
			whatNear = 1;
		}
		if (LSL.HouseHandler.CurrentHouse != null && LSL.HouseHandler.CurrentHouse.inside && LSL.HouseHandler.CurrentHouse.stashPos.DistanceTo(LSL.playerPos) < 3f)
		{
			whatNear = 2;
		}
		return whatNear;
	}

	public static Vector3 WeedPlaceLocation()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(610.726f, -3059.5977f, 6.258467f);
	}

	public static Vector3 AdjustToGround(Vector3 _vector3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 finalPos = _vector3;
		finalPos.Z = GetGroundHeightCompat(finalPos);
		return finalPos;
	}

	public static int GetLargeStreetAmount()
	{
		List<int> amounts = new List<int> { 7, 14, 28 };
		int amount = amounts[RanRange(0, amounts.Count)];
		int rep = (int)CurrentRepLvl(LSL.areas[LSL.areaIndex].Reputation);
		if (rep > 0)
		{
			int mod = 1;
			for (int i = 0; i < rep; i++)
			{
				if (LSL.rnd.Next(0, 10) == 0)
				{
					mod++;
				}
			}
			amount *= mod;
		}
		return amount;
	}

	public static Area GetAreaFromString(string _name)
	{
		Area a = null;
		if (LSL.areas.Count > 0)
		{
			a = LSL.areas.Find((Area b) => b.Name == _name);
		}
		return a;
	}
}
