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
	private static float MaxSpeed()
	{
		return 0f;
	}

	public static Vector3 GenerateSpawnPos(Vector3 desiredPos, Nodetype roadtype, bool sidewalk)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected Obj, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 finalpos = Vector3.Zero;
		bool ForceOffroad = false;
		OutputArgument outArgA = new OutputArgument();
		int NodeNumber = 1;
		int type = 0;
		if (roadtype == Nodetype.AnyRoad)
		{
			type = 1;
		}
		if (roadtype == Nodetype.Road)
		{
			type = 0;
		}
		if (roadtype == Nodetype.Offroad)
		{
			type = 1;
			ForceOffroad = true;
		}
		if (roadtype == Nodetype.Water)
		{
			type = 3;
		}
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (desiredPos.X), (desiredPos.Y), (desiredPos.Z), (NodeNumber), (type), (200f), (200f));
		if (ForceOffroad)
		{
			while (!Function.Call<bool>((Hash)5715191804072465017L, (NodeID)))
			{
				NodeID = Function.Call<int>((Hash)2510518586829603349L, (desiredPos.X), (desiredPos.Y), (desiredPos.Z), (NodeNumber), (type), (200f), (200f));
				NodeNumber++;
			}
		}
		Function.Call((Hash)8084282276450675138L, (NodeID), outArgA);
		finalpos = outArgA.GetResult<Vector3>();
		if (sidewalk)
		{
			finalpos = World.GetNextPositionOnSidewalk(finalpos);
		}
		return finalpos;
	}

	public Vector3 ClosetNodePos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		_ = Vector3.Zero;
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (pos.X), (pos.Y), (pos.Z), (1), (1), (200f), (200f));
		OutputArgument outArgA = new OutputArgument();
		Function.Call((Hash)8084282276450675138L, (NodeID), outArgA);
		return outArgA.GetResult<Vector3>();
	}

	public static List<PathnodeFlags> GetRoadFlags(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<PathnodeFlags> pathnodesflags = new List<PathnodeFlags>();
		OutputArgument outArgA = new OutputArgument();
		OutputArgument outArgB = new OutputArgument();
		if (Function.Call<bool>((Hash)389656384451763932L, (pos.X), (pos.Y), (pos.Z), outArgA, outArgB))
		{
			outArgA.GetResult<int>();
			int flags = outArgB.GetResult<int>();
			foreach (PathnodeFlags flag in Enum.GetValues(typeof(PathnodeFlags)).Cast<PathnodeFlags>())
			{
				if (((uint)flag & (uint)flags) != 0)
				{
					pathnodesflags.Add(flag);
				}
			}
		}
		return pathnodesflags;
	}

	public static bool RoadHasFlag(Vector3 pos, PathnodeFlags flag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		OutputArgument outArgA = new OutputArgument();
		OutputArgument outArgB = new OutputArgument();
		if (Function.Call<bool>((Hash)389656384451763932L, (pos.X), (pos.Y), (pos.Z), outArgA, outArgB))
		{
			outArgA.GetResult<int>();
			if (((uint)outArgB.GetResult<int>() & (uint)flag) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IntHasFlag(int number, int flag)
	{
		if ((number & flag) != 0)
		{
			return true;
		}
		return false;
	}
}
