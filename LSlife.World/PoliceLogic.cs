using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Control = GTA.Control;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.Gangs;
using LSlife.HeatBars;
using LSlife.OldCustomers;
using LSlife.Shots;

namespace LSlife;

public partial class LSL
{
	private static void HandlePigs()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (pigs.Count <= 0)
		{
			return;
		}
		foreach (Pig pig in pigs.ToList())
		{
			if ((Entity)(object)pig.Ped != (Entity)null && ((Entity)pig.Ped).Exists())
			{
				if (((Entity)pig.Ped).IsAlive)
				{
					Vector3 position = ((Entity)pig.Ped).Position;
					if (!(position.DistanceTo(playerPos) > 1050f))
					{
						goto IL_008c;
					}
				}
				pig.DeletePig();
				if (DEBUG)
				{
					UI.Notify("Policier généré supprimé");
				}
				goto IL_008c;
			}
			pigs.Remove(pig);
			continue;
			IL_008c:
			pig.DoState();
			pig.CheckState();
		}
	}

	private static void SpawnPigs()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Vehicle pigCar = World.CreateVehicle(new Model(Enums.GetPoliceVehType()), GenerateSpawnPos(playerPos.Around(200f), Nodetype.Road, sidewalk: false));
		if ((Entity)(object)pigCar != (Entity)null)
		{
			if (DEBUG)
			{
				UI.Notify("Policier généré");
				((Entity)pigCar).AddBlip();
				((Entity)pigCar).CurrentBlip.Name = "Voiture de police";
			}
			Function.Call(unchecked((Hash)(-823518585696636035L)), (((Entity)pigCar).Handle), (true));
			pigs.Add(new Pig(pigCar.CreatePedOnSeat(unchecked((VehicleSeat)(-1)), new Model(Enums.GetPolicePedType())), _leader: true));
			pigs.Add(new Pig(pigCar.CreatePedOnSeat((VehicleSeat)0, new Model(Enums.GetPolicePedType())), _leader: false));
		}
		if (pigs.Count <= 0)
		{
			return;
		}
		foreach (Pig p in pigs)
		{
			if ((Entity)(object)p.Ped != (Entity)null)
			{
				Function.Call(unchecked((Hash)(-4970914549772615683L)), (p.Ped), (true));
				if (areas[areaIndex].CopPresance < 10)
				{
					areas[areaIndex].CopPresance++;
				}
			}
		}
	}
}
