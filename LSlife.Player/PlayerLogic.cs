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
	public static TaskSequence PerformTaskSeq(int index, Vehicle vehicle, Vector3 target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected Obj, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		switch (index)
		{
		case 0:
		{
			TaskSequence val2 = new TaskSequence();
			val2.AddTask.LeaveVehicle();
			val2.AddTask.Wander();
			return val2;
		}
		case 1:
		{
			TaskSequence val = new TaskSequence();
			val.AddTask.FollowNavMeshTo(((Entity)vehicle).Position + ((Entity)vehicle).RightVector * 2f * -1f);
			val.AddTask.EnterVehicle(vehicle, unchecked((VehicleSeat)(-1)));
			val.AddTask.CruiseWithVehicle(vehicle, 20f, (GTA.VehicleDrivingFlags)5);
			return val;
		}
		case 2:
		{
			Vector3 playerForward = ((Entity)player.Character).ForwardVector;
			Vector3 playerRight = ((Entity)player.Character).RightVector;
			TaskSequence driveToPlayerAndExit = new TaskSequence();
			driveToPlayerAndExit.AddTask.ClearAllImmediately();
			driveToPlayerAndExit.AddTask.DriveTo(vehicle, target, 10f, (GTA.VehicleDrivingFlags)5, 20f);
			if (player.Character.IsInVehicle())
			{
				driveToPlayerAndExit.AddTask.FollowNavMeshTo(playerPos + playerRight * 3f);
			}
			else
			{
				driveToPlayerAndExit.AddTask.FollowNavMeshTo(playerPos + playerForward * 2f);
			}
			return driveToPlayerAndExit;
		}
		default:
			return null;
		}
	}

	private int PedsInPlayersGroup()
	{
		return player.Character.PedGroup.ToList(false).Count;
	}
}
