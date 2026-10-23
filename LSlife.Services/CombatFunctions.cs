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
	public static bool IsPlayerArmoursMaxed()
	{
		if (Game.Player.Character.Armor >= 100 && LSL.playerArmours == 5)
		{
			return true;
		}
		return false;
	}

	public static void GiveArmourToPlayer()
	{
		if (Game.Player.Character.Armor < 100)
		{
			Game.Player.Character.Armor = 200;
		}
		else if (LSL.playerArmours < 5)
		{
			LSL.playerArmours++;
		}
	}

	public static int AmountOfPedsForDriveBy(int _gangStr)
	{
		if (_gangStr <= 1)
		{
			return 1;
		}
		if (_gangStr <= 4)
		{
			return 2;
		}
		if (_gangStr <= 7)
		{
			return 3;
		}
		return 4;
	}

	public static void DoGangGrowth()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (LSL.areas.Count <= 0)
		{
			return;
		}
		foreach (Area area in LSL.areas)
		{
			int gChance = 20;
			if (area.Name == World.GetZoneNameLabel(LSL.HouseHandler.Houses[0].entrancePos))
			{
				gChance *= 2;
				area.GangPresance = 0f;
			}
			if (RanRange(0, gChance) == 0 && area.GangPresance < 10f && !area.DemandMet())
			{
				area.GangPresance++;
			}
		}
	}

	public static void SpawnDriveBy(Ped _target, bool _agressive)
	{
		int amount = (int)LSL.areas[LSL.areaIndex].GangPresance;
		if (_agressive)
		{
			if (!LSL.hardMode)
			{
				amount = (int)((double)amount * 0.5);
			}
			if (amount < 1)
			{
				amount = 1;
			}
			for (int i = 0; i < amount; i++)
			{
				LSL.driveByHandler.AddDriveBy((VehicleClass)2, RanRange(0, AmountOfPedsForDriveBy(amount)), _target, _agressive);
			}
		}
		else
		{
			LSL.driveByHandler.AddDriveBy((VehicleClass)2, RanRange(0, AmountOfPedsForDriveBy(amount)), _target, _agressive);
		}
	}
}
