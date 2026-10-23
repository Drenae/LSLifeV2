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
	public const int pMaxDrugs = 7168;

	public static int pWeedOunce = 0;

	public static int pCrackOunce = 0;

	public static int pCocaineOunce = 0;

	public static List<PedHash> cHashes = new List<PedHash>();

	public static List<DealerSpawnPostion> spawnPostions = new List<DealerSpawnPostion>();

	public static List<PedHash> LikleyCustomers = new List<PedHash>();

	public static Dictionary<string, PedHash> PedModels = new Dictionary<string, PedHash> { 
	{
		"ZEE",
		unchecked((PedHash)(-236444766))
	} };

	private static List<int> AllowedInteriors = new List<int> { 118018, 271617, 92674, 275201, 233729 };

	private static int lastHour;

	internal static bool TimeChanged = false;

	public static int HoursPassed { get; private set; }


}
