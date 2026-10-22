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

internal static class LsFunctions
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

	public static List<Drug> LsDrugs()
	{
		List<Drug> Drugs = new List<Drug>();
		if (LSL.hardMode)
		{
			Drugs.Add(new Drug("Weed", 10, LSL.AreaType.Normal));
			Drugs.Add(new Drug("Crack", 20, LSL.AreaType.Poor));
			Drugs.Add(new Drug("Cocaine", 100, LSL.AreaType.Rich));
		}
		else
		{
			Drugs.Add(new Drug("Weed", 50, LSL.AreaType.Normal));
			Drugs.Add(new Drug("Crack", 200, LSL.AreaType.Poor));
			Drugs.Add(new Drug("Cocaine", 400, LSL.AreaType.Rich));
		}
		return Drugs;
	}

	public static float StringToFloat(string _float)
	{
		if (float.TryParse(_float, NumberStyles.Float, new CultureInfo("de-DE"), out var final))
		{
			return final;
		}
		return float.Parse(_float, NumberStyles.Float, CultureInfo.InvariantCulture);
	}

	public static void LoadSpawnLocations()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		foreach (XElement element in XDocument.Load("scripts\\LSLife\\LSLife_Spawns.xml").Descendants())
		{
			if (!(element.Name == "pos"))
			{
				continue;
			}
			Vector3 position = Vector3.Zero;
			float heading = 0f;
			foreach (XElement ele in element.Descendants())
			{
				if (ele.Name == "x")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.X);
				}
				if (ele.Name == "y")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.Y);
				}
				if (ele.Name == "z")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.Z);
				}
				if (ele.Name == "h")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out heading);
				}
			}
			if (position != Vector3.Zero)
			{
				spawnPostions.Add(new DealerSpawnPostion(position, heading));
			}
		}
	}

	public static void LoadZoneData()
	{
		if (File.Exists("scripts\\LSLife\\zoneData\\LsLife_LSSA.xml"))
		{
			int i = 0;
			foreach (XElement zone in XDocument.Load("scripts\\LSLife\\zoneData\\LsLife_LSSA.xml").Descendants())
			{
				if (!(zone.Name == "area"))
				{
					continue;
				}
				i++;
				string zoneName = zone.FirstAttribute.Value;
				LSL.AreaType wealth = LSL.AreaType.Normal;
				LSL.jurisdictionType police = LSL.jurisdictionType.LSPD;
				foreach (XElement data in zone.Elements())
				{
					if (data.Name == "wealth")
					{
						Enum.TryParse<LSL.AreaType>(data.Value, out wealth);
					}
					if (data.Name == "police")
					{
						Enum.TryParse<LSL.jurisdictionType>(data.Value, out police);
					}
				}
				Tuple<LSL.AreaType, LSL.jurisdictionType> zData = new Tuple<LSL.AreaType, LSL.jurisdictionType>(wealth, police);
				LSL.zoneData.Add(zoneName, zData);
			}
			DbugString("Données de Los Santos chargées : " + i + " quartiers ajoutés");
		}
		else
		{
			UI.Notify("~r~Fichier LsLife_LSSA.xml introuvable");
		}
		if (!LSL.LCRWMAP)
		{
			return;
		}
		if (File.Exists("scripts\\LSLife\\zoneData\\LsLife_LCRW.xml"))
		{
			int i2 = 0;
			foreach (XElement zone2 in XDocument.Load("scripts\\LSLife\\zoneData\\LsLife_LCRW.xml").Descendants())
			{
				if (!(zone2.Name == "area"))
				{
					continue;
				}
				i2++;
				string zoneName2 = zone2.FirstAttribute.Value;
				LSL.AreaType wealth2 = LSL.AreaType.Normal;
				LSL.jurisdictionType police2 = LSL.jurisdictionType.LSPD;
				foreach (XElement data2 in zone2.Elements())
				{
					if (data2.Name == "wealth")
					{
						Enum.TryParse<LSL.AreaType>(data2.Value, out wealth2);
					}
					if (data2.Name == "police")
					{
						Enum.TryParse<LSL.jurisdictionType>(data2.Value, out police2);
					}
				}
				Tuple<LSL.AreaType, LSL.jurisdictionType> zData2 = new Tuple<LSL.AreaType, LSL.jurisdictionType>(wealth2, police2);
				LSL.zoneData.Add(zoneName2, zData2);
			}
			DbugString("Données de Liberty City Rewind chargées : " + i2 + " quartiers ajoutés");
		}
		else
		{
			UI.Notify("~r~Fichier LsLife_LCRW.xml introuvable");
		}
	}

	public static int zeeDebtCap()
	{
		int initCap = 100000;
		if (LSL.hardMode)
		{
			initCap = 1000;
		}
		return initCap + (int)LSL.dealer1Rep;
	}

	public static void SaveDealer()
	{
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "dealer1"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "weed")
				{
					e.Value = LSL.dealer1Weed.ToString();
				}
				if (e.Name == "crack")
				{
					e.Value = LSL.dealer1Crack.ToString();
				}
				if (e.Name == "cocain")
				{
					e.Value = LSL.dealer1Cocain.ToString();
				}
				if (e.Name == "debt")
				{
					e.Value = LSL.dealer1Debt.ToString();
				}
				if (e.Name == "money")
				{
					e.Value = LSL.dealer1Money.ToString();
				}
				if (e.Name == "rep")
				{
					e.Value = LSL.dealer1Rep.ToString();
				}
			}
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
	}

	public static bool IsPlayerArmoursMaxed()
	{
		if (Game.Player.Character.Armor >= 100 && LSL.playerArmours == 5)
		{
			return true;
		}
		return false;
	}

	public static int DealerDelay()
	{
		return 60000 * (10 - (int)LSL.areas[LSL.areaIndex].GangPresance);
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

	public static int RandomInt(int start, int end)
	{
		return new Random().Next(start, end);
	}

	public static string DrugDisplayName(string name)
	{
		if (name == "Weed") return "cannabis";
		if (name == "Cocaine") return "cocaïne";
		if (name == "Crack") return "crack";
		return name;
	}

	public static string OrderMenuDesc()
	{
		return string.Join("\n", new string[]
		{
			"Cannabis : ~g~" + LSL.NewOrder.weed + "~s~ oz",
			"Crack : ~g~" + LSL.NewOrder.crack + "~s~ oz",
			"Cocaïne : ~g~" + LSL.NewOrder.cocaine + "~s~ oz",
			"Facture : " + IntToMoney(LSL.NewOrder.Bill) + " $",
			"Dette : " + IntToMoney(LSL.dealer1Debt) + " $",
			"Crédit : " + IntToMoney(zeeDebtCap()) + " $"
		});
	}

	public static void UpdateOrderMenuDesc()
	{
		string desc = OrderMenuDesc();
		LSL.PlaceOrder.Description = desc;
		LSL.bushWeed.Description = desc;
		LSL.crack.Description = desc;
		LSL.cocaine.Description = desc;
		LSL.WeedBuy.Description = desc;
		LSL.CrackBuy.Description = desc;
		LSL.CocainBuy.Description = desc;
		LSL.LsMenuPool.FormatDescriptions = true;
		LSL.LsMenuPool.RefreshIndex();
	}

	public static Pig GetPig(Ped _ped)
	{
		if (LSL.pigs.Count > 0)
		{
			List<Pig> tPigs = LSL.pigs.Where((Pig p) => (Entity)(object)p.Ped == (Entity)(object)_ped).ToList();
			if (tPigs.Count() > 0)
			{
				Pig pig = tPigs.First();
				if (pig != null)
				{
					return pig;
				}
			}
		}
		return null;
	}

	public static bool CanSee(Entity _ent, Entity _targetEnt)
	{
		return Function.Call<bool>((Hash)173335856089985402L, (_ent), (_targetEnt));
	}

	public static bool CanHearPlayer(Ped _ped)
	{
		return Function.Call<bool>(unchecked((Hash)(-966241770451121623L)), (Game.Player.Character), (_ped));
	}

	public static bool IsPolice(Ped p)
	{
		bool cop = false;
		foreach (Enums.LawEnforcePed lawEnforce in Enum.GetValues(typeof(Enums.LawEnforcePed)))
		{
			if (Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (((Entity)p).Handle)) == (int)lawEnforce)
			{
				cop = true;
				break;
			}
		}
		return cop;
	}

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
			newSpawnPos.Z = World.GetGroundHeight(newSpawnPos);
			if (Function.Call<bool>((Hash)1322919935073282825L, (newSpawnPos.X), (newSpawnPos.Y), (newSpawnPos.Z)) || newSpawnPos.DistanceTo(spawnPos) < newSpawnPos.DistanceTo(directionPos))
			{
				oldSpawnPos = newSpawnPos;
				newSpawnPos += directionFromSpawn * 0.5f;
				continue;
			}
			finalSpawnPos = oldSpawnPos;
			finalSpawnPos.Z = World.GetGroundHeight(finalSpawnPos);
			return finalSpawnPos;
		}
		return finalSpawnPos;
	}

	public static void SetImmuneToPlayer(Ped ped)
	{
	}

	public static void NotImmuneToPlayer(Ped ped)
	{
		Function.Call(unchecked((Hash)(-2148903261299834599L)), (ped), (true), (LSL.playerRelationship));
		ped.CanBeTargetted = true;
		ped.CanBeShotInVehicle = true;
		Function.Call((Hash)7400957295976475254L, (ped), (LSL.player), (true));
		Function.Call((Hash)7202820943940500141L, (ped), (true));
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

	public static float Lerp(float start, float end, float time)
	{
		return (1f - time) * start + time * end;
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

	public static void SetWorkerMenuHealth(Ped p)
	{
		((UIText)LSL.WorkerMainMenu.Subtitle).Caption = "Santé : " + ((Entity)p).Health + " Protection :" + p.Armor;
	}

	public static void SetRandomCombatStats(Ped p)
	{
		int gStr = (int)LSL.areas[LSL.areaIndex].GangPresance;
		if (gStr < 0)
		{
			gStr = 0;
		}
		p.Armor = RanRange(0, 1 + gStr * 10);
		p.ShootRate = RanRange(0, 1 + gStr * 10);
		p.Accuracy = RanRange(0, 1 + gStr * 10);
	}

	public static void SetRivalWeapons(Ped p)
	{
		switch (RanRange(0, (int)(1f + LSL.areas[LSL.areaIndex].GangPresance * 0.5f)))
		{
		case 5:
			p.Weapons.Give(unchecked((WeaponHash)(-1074790547)), 500, false, true);
			p.Weapons.Give((WeaponHash)324215364, 500, true, true);
			break;
		case 4:
			p.Weapons.Give((WeaponHash)1649403952, 500, false, true);
			p.Weapons.Give(unchecked((WeaponHash)(-1121678507)), 500, true, true);
			break;
		case 3:
			p.Weapons.Give((WeaponHash)1649403952, 500, false, true);
			p.Weapons.Give(unchecked((WeaponHash)(-619010992)), 500, true, true);
			break;
		case 2:
			p.Weapons.Give((WeaponHash)487013001, 500, false, true);
			p.Weapons.Give(unchecked((WeaponHash)(-619010992)), 500, true, true);
			break;
		case 1:
			p.Weapons.Give((WeaponHash)2017895192, 500, false, true);
			p.Weapons.Give((WeaponHash)453432689, 500, true, true);
			break;
		case 0:
			p.Weapons.Give((WeaponHash)453432689, 500, true, true);
			break;
		}
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

	public static void SendTextMsg(string str)
	{
		Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
		UI.Notify(str);
	}

	public static void TextMsg(string _name, string _title, string _msg)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!HeadShotHandler.headShots.ContainsKey(_name))
		{
			Ped ped = World.CreatePed(RequestModel(PedModels[_name]), new Vector3(0f, 0f, 0f));
			while ((Entity)(object)ped == (Entity)null)
			{
				Script.Yield();
			}
			HeadShotHandler.MugShot(ped, _name);
			((Entity)ped).Delete();
		}
		Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
		Function.Call((Hash)2316831480196236324L, ("CELL_EMAIL_BCON"));
		Function.Call((Hash)7789129354908300458L, (_msg));
		Function.Call((Hash)2075484565200204495L, (HeadShotHandler.headShots[_name].Texture), (HeadShotHandler.headShots[_name].Texture), (true), (1), (_title), (_name));
		Script.Yield();
	}

	public static void TextMsg(string _name, Ped _ped, string _title, string _msg)
	{
		if (!HeadShotHandler.headShots.ContainsKey(_name))
		{
			HeadShotHandler.MugShot(_ped, _name);
		}
		else if (HeadShotHandler.headShots.Where((KeyValuePair<string, HeadShot> r) =>
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			PedHash pedHash = r.Value.PedHash;
			Model model = ((Entity)_ped).Model;
			return (int)pedHash == model.Hash;
		}).Count() == 0)
		{
			HeadShotHandler.DeleteHeadShot(_name);
			HeadShotHandler.MugShot(_ped, _name);
		}
		Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
		Function.Call((Hash)2316831480196236324L, ("CELL_EMAIL_BCON"));
		Function.Call((Hash)7789129354908300458L, (_msg));
		Function.Call((Hash)2075484565200204495L, (HeadShotHandler.headShots[_name].Texture), (HeadShotHandler.headShots[_name].Texture), (true), (1), (_title), (_name));
		Script.Yield();
	}

	public static int TotalDrugs(Dictionary<string, int> _drugs)
	{
		int totalDrugs = 0;
		foreach (KeyValuePair<string, int> _drug in _drugs)
		{
			totalDrugs += _drugs[_drug.Key];
		}
		return totalDrugs + (pWeedOunce * 28 + pCrackOunce * 28 + pCocaineOunce * 28);
	}

	public static void PedBag(Ped ped, bool give)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Model model = ((Entity)ped).Model;
		int num;
		if (model.Hash != 1885233650)
		{
			model = ((Entity)ped).Model;
			num = ((model.Hash == -1667301416) ? 1 : 0);
		}
		else
		{
			num = 1;
		}
		bool FreeMode = (byte)num != 0;
		if (give)
		{
			if (FreeMode)
			{
				Function.Call((Hash)2750315038012726912L, (ped), (5), (81), (0), (1));
			}
			else
			{
				Function.Call((Hash)2750315038012726912L, (ped), (9), (1), (0), (0));
			}
		}
		else if (FreeMode)
		{
			Function.Call((Hash)2750315038012726912L, (ped), (5), (0), (0), (1));
		}
		else
		{
			Function.Call((Hash)2750315038012726912L, (ped), (9), (0), (0), (0));
		}
	}

	public static void DisplayHelpText(string text)
	{
		InputArgument[] arguments = new InputArgument[1] { ("STRING") };
		Function.Call(unchecked((Hash)(-8860350453193909743L)), arguments);
		InputArgument[] argumentArray2 = new InputArgument[1] { (text) };
		Function.Call((Hash)7789129354908300458L, argumentArray2);
		InputArgument[] argumentArray3 = new InputArgument[4]
		{
			(0),
			(0),
			(1),
			(-1)
		};
		Function.Call((Hash)2562546386151446694L, argumentArray3);
	}

	public static int Discount(int price)
	{
		int discount = 0;
		if (CurrentRepLvl(LSL.dealer1Rep) >= 1.0)
		{
			discount = price / 100 * (int)CurrentRepLvl(LSL.dealer1Rep);
		}
		return discount;
	}

	public static double CurrentRepLvl(double rep)
	{
		if (rep < 0.0)
		{
			return 0.0;
		}
		return 0.03999999910593033 * Math.Sqrt(rep);
	}

	public static void AddBlip(Entity _ent, BlipColor _col, string _name, bool _friend, bool _edge)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!_ent.CurrentBlip.Exists())
		{
			_ent.AddBlip();
		}
		Blip currentBlip = _ent.CurrentBlip;
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
		Blip currentBlip = ((Entity)ped).CurrentBlip;
		if (currentBlip != null)
		{
			currentBlip.Remove();
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

	public static int RanRange(int _min, int _max)
	{
		int max = ((_max >= _min) ? _max : (_min + 1));
		return LSL.rnd.Next(_min, max);
	}

	public static void PlayTheSound()
	{
		Game.PlaySound("LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
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

	public static bool IsUnderGround(Entity _entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = _entity.Position;
		float eZ = position.Z;
		if (World.GetGroundHeight(position) > eZ)
		{
			return true;
		}
		return false;
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

	public static void UpdateSave()
	{
		bool changed = false;
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "dealer1"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "debt" && e.Value != LSL.dealer1Debt.ToString())
				{
					e.Value = LSL.dealer1Debt.ToString();
					changed = true;
				}
				if (e.Name == "weed" && e.Value != LSL.dealer1Weed.ToString())
				{
					e.Value = LSL.dealer1Weed.ToString();
					changed = true;
				}
				if (e.Name == "crack" && e.Value != LSL.dealer1Crack.ToString())
				{
					e.Value = LSL.dealer1Crack.ToString();
					changed = true;
				}
				if (e.Name == "cocain" && e.Value != LSL.dealer1Cocain.ToString())
				{
					e.Value = LSL.dealer1Cocain.ToString();
					changed = true;
				}
				if (e.Name == "rep" && e.Value != LSL.dealer1Rep.ToString())
				{
					e.Value = LSL.dealer1Rep.ToString();
					changed = true;
				}
				if (e.Name == "money" && e.Value != LSL.dealer1Money.ToString())
				{
					e.Value = LSL.dealer1Money.ToString();
					changed = true;
				}
			}
		}
		if (changed)
		{
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			if (LSL.DEBUG)
			{
				UI.Notify("Sauvegarde modifiée");
			}
		}
	}

	public static List<XElement> PedWeapons(Ped _ped)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected I4, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected I4, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected I4, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		List<XElement> weps = new List<XElement>();
		foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
		{
			if (!Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (_ped), ((int)weapon)))
			{
				continue;
			}
			int ammunitionInWeapon = Function.Call<int>((Hash)97480644549409105L, (_ped), ((int)weapon));
			if (ammunitionInWeapon < 500)
			{
				ammunitionInWeapon = 500;
			}
			XElement parentElement = new XElement("weapon");
			XElement weaponNameElement = new XElement("HASH", (object)weapon);
			XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
			parentElement.Add(weaponNameElement);
			parentElement.Add(weaponAmmoElement);
			foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
			{
				if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (_ped), ((int)weapon), ((int)component)))
				{
					XElement weaponComponentElement = new XElement("COMPONENT", (object)component);
					parentElement.Add(weaponComponentElement);
				}
			}
			weps.Add(parentElement);
		}
		return weps;
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

	public static void GivePedWeapons(Ped _ped, List<XElement> _weapons)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (_weapons.Count <= 0)
		{
			return;
		}
		WeaponHash cWeapon = _ped.Weapons.Current.Hash;
		foreach (XElement _weapon in _weapons)
		{
			bool hasWeapon = false;
			WeaponHash weapon = unchecked((WeaponHash)(-1569615261));
			foreach (XElement ele in _weapon.Elements())
			{
				if (ele.Name == "HASH")
				{
					Enum.TryParse<WeaponHash>(ele.Value, out weapon);
				}
				if (ele.Name == "AMMO")
				{
					int.TryParse(ele.Value, out var ammo);
					if (ammo < 500)
					{
						ammo = 500;
					}
					if (!_ped.Weapons.HasWeapon(weapon))
					{
						if (_ped != null)
						{
							_ped.Weapons.Give(weapon, ammo, true, true);
						}
					}
					else
					{
						hasWeapon = true;
					}
					if (_ped != null)
					{
						_ped.Weapons.Select(weapon);
					}
					Weapon current2 = _ped.Weapons.Current;
					current2.Ammo += ammo;
					if (LSL.DEBUG)
					{
						UI.Notify("Arme donnée au personnage : " + ((object)weapon/*cast due to constrained. prefix*/).ToString());
					}
				}
				if (ele.Name == "COMPONENT" && !hasWeapon && Enum.TryParse<WeaponComponent>(ele.Value, out WeaponComponent wepcomp) && _ped != null)
				{
					_ped.Weapons.Current.SetComponent(wepcomp, true);
				}
			}
		}
		_ped.Weapons.Select(cWeapon);
	}

	public static void UpdateInventory()
	{
		bool saveChanged = false;
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "weed")
				{
					if (e.Value != LSL.PlayerInventory["Weed"].ToString())
					{
						e.Value = LSL.PlayerInventory["Weed"].ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "crack")
				{
					if (e.Value != LSL.PlayerInventory["Crack"].ToString())
					{
						e.Value = LSL.PlayerInventory["Crack"].ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "cocain")
				{
					if (e.Value != LSL.PlayerInventory["Cocaine"].ToString())
					{
						e.Value = LSL.PlayerInventory["Cocaine"].ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "weedOz")
				{
					if (e.Value != pWeedOunce.ToString())
					{
						e.Value = pWeedOunce.ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "crackOz")
				{
					if (e.Value != pCrackOunce.ToString())
					{
						e.Value = pCrackOunce.ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "cocaineOz")
				{
					if (e.Value != pCocaineOunce.ToString())
					{
						e.Value = pCocaineOunce.ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "armour" && e.Value != LSL.playerArmours.ToString())
				{
					e.Value = LSL.playerArmours.ToString();
					saveChanged = true;
				}
			}
			break;
		}
		if (saveChanged)
		{
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
	}

	public static void DateTimeUpdate()
	{
		if (Function.Call<int>((Hash)2675767815307398015L) != LSL.hour)
		{
			HoursPassed = HowManyHoursPassed();
			TimeChanged = true;
			LSL.hour = Function.Call<int>((Hash)2675767815307398015L);
			if ((LSL.dealer1Weed == 0 || LSL.dealer1Crack == 0 || LSL.dealer1Cocain == 0) && !LSL.dealer1ReloadOrder && !LSL.dealer1Reloaded)
			{
				LSL.dealer1ReloadOrder = true;
				int waitTime = 20;
				waitTime = ((waitTime - (int)CurrentRepLvl(LSL.dealer1Rep) / 2 > 5) ? (waitTime - (int)CurrentRepLvl(LSL.dealer1Rep) / 2) : 5);
				int timeTodeliver = 0;
				bool nextday = false;
				if (LSL.hour + waitTime > 24)
				{
					timeTodeliver = LSL.hour + waitTime - 24;
					nextday = true;
				}
				else
				{
					timeTodeliver = LSL.hour + waitTime;
					nextday = false;
				}
				LSL.dealer1ReloadHour = timeTodeliver;
				if (nextday)
				{
					if (LSL.day == 6)
					{
						LSL.dealer1ReloadDay = 0;
					}
					else
					{
						LSL.dealer1ReloadDay = LSL.day + 1;
					}
				}
				else
				{
					LSL.dealer1ReloadDay = LSL.day;
				}
				Enum.TryParse<Enums.DayGTA>(LSL.dealer1ReloadDay.ToString(), out var mDay);
				if (LSL.DEBUG)
				{
					UI.Notify(LSL.dealer1ReloadHour.ToString() ?? "");
				}
				string amPm = "AM";
				if (timeTodeliver > 12)
				{
					timeTodeliver -= 12;
					amPm = "PM";
				}
				TextMsg("ZEE", "Réassort", "Réassort commandé, livraison prévue " + mDay.ToString() + " à " + timeTodeliver + ":00" + amPm);
			}
			int timeReduction = 0;
			timeReduction = ((!(CurrentRepLvl(LSL.dealer1Rep) > 36.0)) ? ((int)CurrentRepLvl(LSL.dealer1Rep) * 10000) : 360000);
			if (!LSL.dealer1ReloadOrder && LSL.dealer1Reloaded && Game.GameTime > LSL.reloadTimer + (LSL.reloadTimerInterval - timeReduction))
			{
				LSL.dealer1Reloaded = false;
				foreach (XElement element in LSL.LSLifeSave.Descendants())
				{
					if (!(element.Name == "dealer1"))
					{
						continue;
					}
					foreach (XElement drugCarEle in element.Elements())
					{
						if (drugCarEle.Name == "reloaded")
						{
							drugCarEle.Value = LSL.dealer1Reloaded.ToString();
							LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
							break;
						}
					}
				}
			}
			int dDay = 0;
			dDay = ((LSL.dealer1ReloadDay + 1 != 7) ? (LSL.dealer1ReloadDay + 1) : 0);
			if (LSL.day == LSL.dealer1ReloadDay && !LSL.dealer1Reloaded && LSL.dealer1ReloadOrder && LSL.hour >= LSL.dealer1ReloadHour)
			{
				LSL.dealer1ReloadOrder = false;
				LSL.dealer1Reloaded = true;
				LSL.reloadTimer = Game.GameTime;
				if (LSL.dealer1Debt > 0)
				{
					float intrest = 0.05f;
					float intrestAmount = (float)LSL.dealer1Debt * intrest;
					if (intrestAmount < 0f)
					{
						intrestAmount = 0f;
					}
					LSL.dealer1Debt += (int)intrestAmount;
					if (LSL.dealer1Debt < 0)
					{
						LSL.dealer1Debt = 0;
					}
					TextMsg("ZEE", "Dette", "J'ai ajouté $" + IntToMoney((int)intrestAmount) + " à ta dette. Elle s'élève maintenant à $" + IntToMoney(LSL.dealer1Debt));
				}
				if (LSL.hardMode)
				{
					if (LSL.dealer1Money < 1000)
					{
						LSL.dealer1Money = 5000;
					}
				}
				else if (LSL.dealer1Money < 10000)
				{
					LSL.dealer1Money = 50000;
				}
				float weedMoney = 0f;
				float crackMoney = 0f;
				float cocainMoney = 0f;
				if (LSL.dealer1Weed < 40)
				{
					weedMoney = (float)LSL.dealer1Money * 0.75f;
					LSL.dealer1Money -= (int)weedMoney;
				}
				if (LSL.dealer1Crack < 40)
				{
					crackMoney = (float)LSL.dealer1Money * 0.6f;
					LSL.dealer1Money -= (int)crackMoney;
				}
				if (LSL.dealer1Cocain < 40)
				{
					cocainMoney = LSL.dealer1Money;
					LSL.dealer1Money -= (int)cocainMoney;
				}
				int dWeed = LSL.weed1Price / 2 - Discount(LSL.weed1Price / 2);
				int dCrack = LSL.crack1Price / 2 - Discount(LSL.crack1Price / 2);
				int dCocaine = LSL.cocain1Price / 2 - Discount(LSL.cocain1Price / 2);
				int finalWeed = (int)weedMoney / dWeed;
				int finalCrack = (int)crackMoney / dCrack;
				int finalCocain = (int)cocainMoney / dCocaine;
				LSL.dealer1Weed += finalWeed;
				LSL.dealer1Crack += finalCrack;
				LSL.dealer1Cocain += finalCocain;
				weedMoney -= (float)(finalWeed * dWeed);
				crackMoney -= (float)(finalCrack * dCrack);
				cocainMoney -= (float)(finalCocain * dCocaine);
				LSL.dealer1Money += (int)weedMoney + (int)crackMoney + (int)cocainMoney;
				TextMsg("ZEE", "Réapprovisionnement", "Cannabis : " + finalWeed + "oz, crack : " + finalCrack + "oz, cocaïne : " + finalCocain + "oz.");
				LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
				UpdateSave();
			}
			if (LSL.day == dDay && !LSL.dealer1Reloaded && LSL.dealer1ReloadOrder)
			{
				LSL.dealer1ReloadOrder = false;
				LSL.dealer1Reloaded = true;
				LSL.reloadTimer = Game.GameTime;
				if (LSL.dealer1Debt > 0)
				{
					float intrest2 = 0.05f;
					float intrestAmount2 = (float)LSL.dealer1Debt * intrest2;
					LSL.dealer1Debt += (int)intrestAmount2;
					if (LSL.dealer1Debt < 0)
					{
						LSL.dealer1Debt = 0;
					}
					TextMsg("ZEE", "Dette", "J'ai ajouté $" + IntToMoney((int)intrestAmount2) + " à ta dette. Elle s'élève maintenant à $" + IntToMoney(LSL.dealer1Debt));
				}
				if (LSL.dealer1Money < 10000)
				{
					LSL.dealer1Money = 50000;
				}
				float weedMoney2 = 0f;
				float crackMoney2 = 0f;
				float cocainMoney2 = 0f;
				if (LSL.dealer1Weed < 40)
				{
					weedMoney2 = (float)LSL.dealer1Money * 0.75f;
					LSL.dealer1Money -= (int)weedMoney2;
				}
				if (LSL.dealer1Crack < 40)
				{
					crackMoney2 = (float)LSL.dealer1Money * 0.6f;
					LSL.dealer1Money -= (int)crackMoney2;
				}
				if (LSL.dealer1Cocain < 40)
				{
					cocainMoney2 = LSL.dealer1Money;
					LSL.dealer1Money -= (int)cocainMoney2;
				}
				int dWeed2 = LSL.weed1Price / 2 - Discount(LSL.weed1Price / 2);
				int dCrack2 = LSL.crack1Price / 2 - Discount(LSL.crack1Price / 2);
				int dCocaine2 = LSL.cocain1Price / 2 - Discount(LSL.cocain1Price / 2);
				int finalWeed2 = (int)weedMoney2 / dWeed2;
				int finalCrack2 = (int)crackMoney2 / dCrack2;
				int finalCocain2 = (int)cocainMoney2 / dCocaine2;
				LSL.dealer1Weed += finalWeed2;
				LSL.dealer1Crack += finalCrack2;
				LSL.dealer1Cocain += finalCocain2;
				weedMoney2 -= (float)(finalWeed2 * dWeed2);
				crackMoney2 -= (float)(finalCrack2 * dCrack2);
				cocainMoney2 -= (float)(finalCocain2 * dCocaine2);
				LSL.dealer1Money += (int)weedMoney2 + (int)crackMoney2 + (int)cocainMoney2;
				TextMsg("ZEE", "Réapprovisionnement", "Cannabis : " + finalWeed2 + "oz, crack : " + finalCrack2 + "oz, cocaïne : " + finalCocain2 + "oz.");
				if (LSL.DEBUG)
				{
					UI.Notify("Cannabis : " + finalWeed2 + " crack : " + finalCrack2 + " cocaïne : " + finalCocain2);
				}
				LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
				UpdateSave();
			}
			if (LSL.areas.Count > 0)
			{
				DbugString("HoursPassed = " + HoursPassed);
				for (int i = 0; i < HoursPassed; i++)
				{
					foreach (Area area in LSL.areas)
					{
						if (area.Name == LSL.areas[LSL.areaIndex].Name)
						{
							if (!LSL.playerWanted)
							{
								area.HeatDecay();
							}
						}
						else
						{
							area.HeatDecay();
						}
					}
					DoGangGrowth();
				}
				if (LSL.hour == 12 || LSL.hour == 0 || LSL.hour == 6 || LSL.hour == 18)
				{
					SaveAreaData();
				}
			}
		}
		else
		{
			TimeChanged = false;
		}
		if (Function.Call<int>(unchecked((Hash)(-2777906517671992481L))) == LSL.day)
		{
			return;
		}
		LSL.day = Function.Call<int>(unchecked((Hash)(-2777906517671992481L)));
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		if (LSL.dealer1Money > 1200000)
		{
			LSL.dealer1Money = 1200000;
		}
		if (LSL.dealer1Weed > 0)
		{
			if (LSL.dealer1Weed == 1)
			{
				LSL.dealer1Weed = 0;
				LSL.dealer1Money += LSL.weed1Price / 2 + Discount(LSL.weed1Price / 2);
			}
			else
			{
				LSL.dealer1Weed -= LSL.dealer1Weed / 2;
				LSL.dealer1Money += (LSL.weed1Price / 2 + Discount(LSL.weed1Price / 2)) * LSL.dealer1Weed;
			}
			foreach (XElement element2 in LSL.LSLifeSave.Descendants())
			{
				if (!(element2.Name == "dealer1"))
				{
					continue;
				}
				foreach (XElement drugCarEle2 in element2.Elements())
				{
					if (drugCarEle2.Name == "weed")
					{
						drugCarEle2.Value = LSL.dealer1Weed.ToString();
					}
					if (drugCarEle2.Name == "money")
					{
						drugCarEle2.Value = LSL.dealer1Money.ToString();
					}
				}
			}
		}
		if (LSL.dealer1Crack > 0)
		{
			if (LSL.dealer1Crack == 1)
			{
				LSL.dealer1Crack = 0;
				LSL.dealer1Money += LSL.crack1Price / 2 + Discount(LSL.crack1Price / 2);
			}
			else
			{
				LSL.dealer1Crack -= LSL.dealer1Crack / 2;
				LSL.dealer1Money += (LSL.crack1Price / 2 + Discount(LSL.crack1Price / 2)) * LSL.dealer1Crack;
			}
			foreach (XElement element3 in LSL.LSLifeSave.Descendants())
			{
				if (!(element3.Name == "dealer1"))
				{
					continue;
				}
				foreach (XElement drugCarEle3 in element3.Elements())
				{
					if (drugCarEle3.Name == "crack")
					{
						drugCarEle3.Value = LSL.dealer1Crack.ToString();
					}
					if (drugCarEle3.Name == "money")
					{
						drugCarEle3.Value = LSL.dealer1Money.ToString();
					}
				}
			}
		}
		if (LSL.dealer1Cocain > 0)
		{
			if (LSL.dealer1Cocain == 1)
			{
				LSL.dealer1Cocain = 0;
				LSL.dealer1Money += LSL.cocain1Price / 2 + Discount(LSL.cocain1Price / 2);
			}
			else
			{
				LSL.dealer1Cocain -= LSL.dealer1Cocain / 2;
				LSL.dealer1Money += (LSL.cocain1Price / 2 + Discount(LSL.cocain1Price / 2)) * LSL.dealer1Cocain;
			}
			foreach (XElement element4 in LSL.LSLifeSave.Descendants())
			{
				if (!(element4.Name == "dealer1"))
				{
					continue;
				}
				foreach (XElement drugCarEle4 in element4.Elements())
				{
					if (drugCarEle4.Name == "cocain")
					{
						drugCarEle4.Value = LSL.dealer1Cocain.ToString();
					}
					if (drugCarEle4.Name == "money")
					{
						drugCarEle4.Value = LSL.dealer1Money.ToString();
					}
				}
			}
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		UpdateSave();
	}

	private static void SaveAreaData()
	{
		XDocument doc = ((!File.Exists("scripts\\LSLife\\LSLifeAreas.xml")) ? new XDocument(new XElement("areas")) : XDocument.Load("scripts\\LSLife\\LSLifeAreas.xml"));
		doc.Root.RemoveAll();
		foreach (Area area in LSL.areas)
		{
			XElement aXML = new XElement("area");
			XElement nameXML = new XElement("name", area.Name);
			XElement cStrXML = new XElement("cStr", area.CopPresance);
			XElement gStrXML = new XElement("gStr", area.GangPresance);
			XElement aHeat = new XElement("aHeat", area.Heat);
			XElement aRep = new XElement("aRep", area.Reputation);
			aXML.Add(nameXML);
			aXML.Add(cStrXML);
			aXML.Add(gStrXML);
			aXML.Add(aHeat);
			aXML.Add(aRep);
			doc.Root.Add(aXML);
		}
		doc.Save("scripts\\LSLife\\LSLifeAreas.xml");
	}

	public static Vector3 WeedPlaceLocation()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(610.726f, -3059.5977f, 6.258467f);
	}

	public static int HowManyHoursPassed()
	{
		int currentHour = Function.Call<int>((Hash)2675767815307398015L);
		int hoursPassed = 0;
		if (lastHour != currentHour)
		{
			hoursPassed = ((currentHour <= lastHour) ? (24 - lastHour + currentHour) : (currentHour - lastHour));
			lastHour = currentHour;
			if (LSL.DEBUG)
			{
				UI.Notify("Heures écoulées : " + hoursPassed);
			}
		}
		return hoursPassed;
	}

	public static Prop LoadWeedPlace()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		int doorHash = 464151082;
		Prop prop = null;
		Prop[] props = World.GetNearbyProps(((Entity)Game.Player.Character).Position, 50f);
		Vector3 pos = WeedPlaceLocation();
		if (props.Count() > 0)
		{
			Prop[] array = props;
			foreach (Prop p in array)
			{
				Model model = ((Entity)p).Model;
				if (model.Hash == doorHash)
				{
					prop = p;
					((Entity)p).FreezePosition = true;
					if (((Entity)p).Position != pos)
					{
						((Entity)p).Position = pos;
					}
					if (LSL.DEBUG)
					{
						UI.Notify("Porte trouvée");
					}
					break;
				}
			}
		}
		if ((Entity)(object)prop == (Entity)null)
		{
			prop = World.CreateProp(new Model(doorHash), pos, new Vector3(0f, 0f, 0f), false, false);
			if (LSL.DEBUG)
			{
				UI.Notify("Aucune porte trouvée, création d'une porte");
			}
		}
		return prop;
	}

	public static Vector3 AdjustToGround(Vector3 _vector3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 finalPos = _vector3;
		finalPos.Z = World.GetGroundHeight(finalPos);
		return finalPos;
	}

	public static void ContactAnswered(iFruitContact contact)
	{
		if (contact.Name == "Zee" && LSL.LsMenuPool != null)
		{
			if (LSL.isDealing)
			{
				LSL.StartDeal.Text = "Arrêter de dealer";
			}
			else
			{
				LSL.StartDeal.Text = " Commencer à dealer";
			}
			LSL.zeeMenu.Visible = !LSL.zeeMenu.Visible;
			UpdateDealerInventory();
			LSL.zeeMenu.CurrentSelection = 0;
		}
		LSL.iFruit.Close(1000);
	}

	private static void UpdateDealerInventory()
	{
		((UIText)LSL.zeeMenu.Subtitle).Caption = "Réputation : " + IntToMoney((int)LSL.dealer1Rep) + " Niveau :~g~" + (int)CurrentRepLvl(LSL.dealer1Rep);
		LSL.payDebt.Text = "Rembourser la dette $" + IntToMoney(LSL.dealer1Debt);
		LSL.bushWeed.Maximum = LSL.dealer1Weed;
		LSL.bushWeed.Value = 0;
		LSL.crack.Maximum = LSL.dealer1Crack;
		LSL.crack.Value = 0;
		LSL.cocaine.Maximum = LSL.dealer1Cocain;
		LSL.cocaine.Value = 0;
	}

	public static void MovePed(Ped p, Vector3 target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Entity)p).Position;
		if (position.DistanceTo(target) > 20f || Function.Call<bool>((Hash)1322919935073282825L, (target.X), (target.Y), (target.Z)))
		{
			p.Task.RunTo(target);
		}
		else
		{
			p.Task.GoTo(target);
		}
	}

	public static void ChangeAdealerToPdealer()
	{
		if (LSL.aDealer != null && (Entity)(object)LSL.aDealer.dPed != (Entity)null)
		{
			LSL.DealerHandler.AddDealer(new PlayerDealer(LSL.aDealer.dPed));
			LSL.aDealer = null;
			LSL.areas[LSL.areaIndex].KillDealer();
		}
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

	public static void MakeStreetSale(Customer c, bool inCar)
	{
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Unknown result type (might be due to invalid IL or missing references)
		List<Ped> police = new List<Ped>();
		Ped p = c.Ped;
		int price = c.amount * c.Drug.PricePerG;
		float divder = (float)c.amount / 7f;
		if (divder >= 1f)
		{
			price = c.amount * c.Drug.PricePerG - (int)divder * c.Drug.PricePerG;
		}
		LSL.PedsCanSeeDeal = 0;
		float repEarned = (float)price * 0.01f;
		LSL.areas[LSL.areaIndex].Reputation += repEarned;
		foreach (Ped ped in LSL.lPeds.GetPeds.Where((Ped r) => ((Entity)r).IsAlive))
		{
			if (IsPolice(ped))
			{
				police.Add(ped);
				ped.Task.LookAt((Entity)(object)Game.Player.Character, 10000);
			}
			if (!ped.IsPlayer && LSL.customers.FindAll((Customer s) => (Entity)(object)s.Ped == (Entity)(object)ped).Count == 0 && !OldCustomerHandler.IsPedOldCustomer(ped) && CanSee((Entity)(object)ped, (Entity)(object)Game.Player.Character))
			{
				LSL.PedsCanSeeDeal++;
			}
		}
		if (!inCar)
		{
			Game.Player.Character.Task.TurnTo((Entity)(object)p);
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (p), ("GENERIC_HOWS_IT_GOING"), ("SPEECH_PARAMS_STANDARD"), (0));
			Script.Wait(1000);
			p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8f, 3000, (AnimationFlags)16);
			Game.Player.Character.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8f, 3000, (AnimationFlags)16);
			Script.Wait(2000);
			Player player = Game.Player;
			player.Money += price;
			Ped ped2 = c.Ped;
			ped2.Money -= price;
			LSL.PlayerInventory[c.Drug.Name] -= c.amount;
			PlayTheSound();
			UI.Notify("Vendu : " + DrugDisplayName(c.Drug.Name) + " " + GramsToOz(c.amount) + ", il te reste " + GramsToOz(LSL.PlayerInventory[c.Drug.Name]) +". Réputation gagnée : ~g~" + repEarned + " ~s~, réputation totale ~g~" + LSL.areas[LSL.areaIndex].Reputation + "~s~ à " + LSL.areas[LSL.areaIndex].Name);
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (p), ("GENERIC_THANKS"), ("SPEECH_PARAMS_STANDARD"), (0));
			if ((Entity)(object)c.Vehicle != (Entity)null && ((Entity)c.Vehicle).Exists())
			{
				c.Ped.Task.PerformSequence(LSL.PerformTaskSeq(1, c.Vehicle, Vector3.Zero));
			}
			if (c.FarAway)
			{
				ShotHandler.RemoveShot(ShotHandler.shots.First((Shot s) => s.customer == c));
			}
			else
			{
				LSL.customers.Remove(c);
			}
		}
		else
		{
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (p), ("GENERIC_HOWS_IT_GOING"), ("SPEECH_PARAMS_STANDARD"), (0));
			Script.Wait(1000);
			p.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8f, 3000, (AnimationFlags)16);
			Script.Wait(3000);
			Player player2 = Game.Player;
			player2.Money += price;
			Ped ped3 = c.Ped;
			ped3.Money -= price;
			LSL.PlayerInventory[c.Drug.Name] -= c.amount;
			PlayTheSound();
			UI.Notify("Vendu : " + DrugDisplayName(c.Drug.Name) + " " + GramsToOz(c.amount) + ", il te reste " + GramsToOz(LSL.PlayerInventory[c.Drug.Name]) +". Réputation gagnée : ~g~" + repEarned + " ~s~, réputation totale ~g~" + LSL.areas[LSL.areaIndex].Reputation + "~s~ à " + LSL.areas[LSL.areaIndex].Name);
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (p), ("GENERIC_THANKS"), ("SPEECH_PARAMS_STANDARD"), (0));
			if ((Entity)(object)c.Vehicle != (Entity)null && ((Entity)c.Vehicle).Exists())
			{
				c.Ped.Task.PerformSequence(LSL.PerformTaskSeq(1, c.Vehicle, Vector3.Zero));
			}
			else
			{
				c.Ped.Task.PerformSequence(LSL.PerformTaskSeq(0, null, Vector3.Zero));
			}
			if (c.FarAway)
			{
				ShotHandler.RemoveShot(ShotHandler.shots.First((Shot s) => s.customer == c));
			}
			else
			{
				LSL.customers.Remove(c);
			}
		}
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "weed" && c.Drug.Name == "Weed")
				{
					e.Value = LSL.PlayerInventory["Weed"].ToString();
				}
				if (e.Name == "crack" && c.Drug.Name == "Crack")
				{
					e.Value = LSL.PlayerInventory["Crack"].ToString();
				}
				if (e.Name == "cocain" && c.Drug.Name == "Cocaine")
				{
					e.Value = LSL.PlayerInventory["Cocaine"].ToString();
				}
			}
			break;
		}
		bool tip = false;
		if (LSL.aDealer != null && !LSL.aDealer.Detected)
		{
			double gangStr = LSL.areas[LSL.areaIndex].GangPresance * 1.5f;
			if (RanRange(0, 8 + (int)gangStr) == 0)
			{
				UI.ShowSubtitle("Hé, pour info, ~r~quelqu'un ~w~d'autre vend vers ~y~" + World.GetStreetName(((Entity)LSL.aDealer.dPed).Position) + "~w~.", 6000);
				AddBlip((Entity)(object)LSL.aDealer.dPed, (BlipColor)1, "Dealer", _friend: false, _edge: false);
				LSL.aDealer.Detected = true;
				tip = true;
			}
		}
		if (!tip && LSL.pigs.Count > 0 && !tip && !LSL.pigs[0].detected && RanRange(0, 5 + (int)LSL.areas[LSL.areaIndex].GangPresance) == 0)
		{
			UI.ShowSubtitle("J'ai entendu que les ~r~flics ~w~sont vers ~y~" + World.GetStreetName(((Entity)LSL.pigs[0].Ped).Position) + "~w~. Ils viennent par ici.", 6000);
			LSL.pigs[0].detected = true;
			tip = true;
		}
		bool Hire = false;
		if (!tip && (int)CurrentRepLvl(LSL.dealer1Rep) > LSL.DealerHandler.dealers.Count)
		{
			int level = RanRange(0, 110 - Math.Min((int)CurrentRepLvl(LSL.areas[LSL.areaIndex].Reputation), 100));
			if (!tip && LSL.PottentialWorker == null && level == 0)
			{
				Hire = true;
				LSL.PottentialWorker = new HirePed(c.Ped);
				UI.ShowSubtitle("Tu recrutes du monde ?", 6000);
				tip = true;
			}
		}
		if (!Hire)
		{
			OldCustomerHandler.AddCustomer(p, c.Vehicle, LSL.HouseHandler.Inside(), !tip, new KeyValuePair<string, int>(c.Drug.Name, c.amount));
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		foreach (Ped pol in police)
		{
			if (LSL.DEBUG)
			{
				UI.Notify(police.Count + " | " + Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (((Entity)pol).Handle)) + " | " + IsPolice(pol));
			}
			if (CanSee((Entity)(object)pol, (Entity)(object)Game.Player.Character))
			{
				SendTextMsg("Ils t'ont vu dealer. Tu vas attirer l'attention dans ce quartier.");
				LSL.areas[LSL.areaIndex].Heat++;
				Game.Player.WantedLevel = 1;
				break;
			}
		}
		if (LSL.aDealer != null && (Entity)(object)LSL.aDealer.dPed != (Entity)null && CanSee((Entity)(object)LSL.aDealer.dPed, (Entity)(object)Game.Player.Character) && LSL.rnd.Next(0, 2) == 0 && LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0)
		{
			SpawnDriveBy(LSL.player.Character, _agressive: true);
		}
		CheckIfPartolsCanSee();
		LSL.customerSellTo = null;
	}

	public static void HandOverGoods(Ped _from, Ped _too)
	{
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		List<Ped> police = new List<Ped>();
		LSL.PedsCanSeeDeal = 0;
		foreach (Ped ped in LSL.lPeds.GetPeds)
		{
			if (((Entity)ped).IsAlive)
			{
				if (IsPolice(ped))
				{
					police.Add(ped);
					ped.Task.LookAt((Entity)(object)Game.Player.Character, 10000);
				}
				if (!ped.IsPlayer && LSL.customers.FindAll((Customer s) => (Entity)(object)s.Ped == (Entity)(object)ped).Count == 0 && !OldCustomerHandler.IsPedOldCustomer(ped) && CanSee((Entity)(object)ped, (Entity)(object)Game.Player.Character))
				{
					LSL.PedsCanSeeDeal++;
				}
			}
		}
		if (!_from.IsInVehicle() || !_too.IsInVehicle())
		{
			_from.Task.TurnTo((Entity)(object)_too);
			_too.Task.TurnTo((Entity)(object)_from);
			Script.Wait(1000);
		}
		_too.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8f, 2000, (AnimationFlags)16);
		_from.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8f, 2000, (AnimationFlags)16);
		Script.Wait(2000);
		Vector3 position;
		foreach (Ped pol in police)
		{
			if (LSL.DEBUG)
			{
				UI.Notify(police.Count + " | " + Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (((Entity)pol).Handle)) + " | " + IsPolice(pol));
			}
			position = ((Entity)pol).Position;
			if (position.DistanceTo(LSL.playerPos) < 40f && CanSee((Entity)(object)pol, (Entity)(object)Game.Player.Character))
			{
				SendTextMsg("Ils t'ont vu dealer. Tu vas attirer l'attention dans ce quartier.");
				LSL.areas[LSL.areaIndex].Heat += 200f;
				Game.Player.WantedLevel = 1;
				break;
			}
		}
		if ((Entity)(object)_from == (Entity)(object)LSL.player.Character || (Entity)(object)_too == (Entity)(object)LSL.player.Character)
		{
			LSL.player.Character.Task.ClearLookAt();
			LSL.player.Character.Task.ClearAll();
		}
		if (LSL.aDealer != null && (Entity)(object)LSL.aDealer.dPed != (Entity)null)
		{
			position = ((Entity)LSL.aDealer.dPed).Position;
			if (position.DistanceTo(LSL.playerPos) < 40f && CanSee((Entity)(object)LSL.aDealer.dPed, (Entity)(object)Game.Player.Character) && LSL.rnd.Next(0, 2) == 0 && LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0)
			{
				DbugString("Attaque en voiture déclenchée : joueur surpris en pleine transaction.");
				SpawnDriveBy(LSL.player.Character, _agressive: true);
			}
		}
		CheckIfPartolsCanSee();
		LSL.lastTimeDealDone = Game.GameTime;
	}

	private static void CheckIfPartolsCanSee()
	{
		if (LSL.driveByHandler.GetDriveBies.Where((DriveBy r) => CanSee((Entity)(object)r.Leader, (Entity)(object)LSL.player.Character)).Count() > 0)
		{
			LSL.driveByHandler.SetAgressiveTarget(_agg: true, LSL.player.Character);
			UI.Notify("Vente repérée depuis un véhicule");
		}
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

	public static int MoneyToRep(int _money, bool _debt)
	{
		if (_debt)
		{
			return (int)((double)(_money / 100) * 0.5);
		}
		return _money / 100;
	}

	public static void Dealer1AddRep(int _rep)
	{
		if (_rep > 0)
		{
			LSL.dealer1Rep += _rep;
		}
	}

	public static Model RequestModel(PedHash _hash)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		int time = Game.GameTime;
		Model Model = new Model(_hash);
		Model.Request(1000);
		if (Model.IsInCdImage && Model.IsValid)
		{
			while (!Model.IsLoaded)
			{
				Script.Yield();
			}
			DbugString("modelTime = " + (time - Game.GameTime));
			return Model;
		}
		DbugString("modelInvalid");
		Model.MarkAsNoLongerNeeded();
		return Model;
	}

	public static Model RequestModel(VehicleHash _hash)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Model Model = new Model(_hash);
		Model.Request(1000);
		if (Model.IsInCdImage && Model.IsValid)
		{
			while (!Model.IsLoaded)
			{
				Script.Yield();
			}
			return Model;
		}
		Model.MarkAsNoLongerNeeded();
		return Model;
	}

	public static string GramsToOz(int _grams)
	{
		if (_grams >= 28)
		{
			int oz = _grams / 28;
			int g = _grams - oz * 28;
			return "~g~" + oz + "~s~oz, ~g~" + g + "~s~g";
		}
		return "~g~" + _grams + "~s~g";
	}

	public static void AddWeedOz(int _amount)
	{
		pWeedOunce += _amount;
	}

	public static void AddCrackOz(int _amount)
	{
		pCrackOunce += _amount;
	}

	public static void AddCocaineOz(int _amount)
	{
		pCocaineOunce += _amount;
	}

	public static void DbugString(string _string)
	{
		if (LSL.DEBUG)
		{
			UI.Notify(_string);
		}
	}

	public static string IntToMoney(int _money)
	{
		return "~g~" + _money.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", " ") + "~s~";
	}

	public static bool zeeClose()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position;
		if ((Entity)(object)LSL.drugDealer1 != (Entity)null)
		{
			position = ((Entity)LSL.drugDealer1).Position;
			if (position.DistanceTo(LSL.playerPos) < 4f)
			{
				return true;
			}
		}
		if ((Entity)(object)LSL.weaponDealer1 != (Entity)null)
		{
			position = ((Entity)LSL.weaponDealer1).Position;
			if (position.DistanceTo(LSL.playerPos) < 4f)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsCustomerNear()
	{
		if (LSL.customers.FindAll((Customer r) =>
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = ((Entity)r.Ped).Position;
			return position.DistanceTo(LSL.playerPos) < 20f;
		}).Count == 0)
		{
			return ShotHandler.shots.FindAll((Shot r) =>
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Entity)r.customer.Ped).Position;
				return position.DistanceTo(LSL.playerPos) < 20f;
			}).Count == 0;
		}
		return false;
	}

	public static Dictionary<int, Tuple<int, int>> GetPedVariationData(Ped _ped)
	{
		List<int> list = new List<int>
		{
			0, 1, 2, 3, 4, 5, 6, 8, 9, 10,
			11
		};
		Dictionary<int, Tuple<int, int>> pedData = new Dictionary<int, Tuple<int, int>>();
		foreach (int id in list)
		{
			int item = Function.Call<int>((Hash)7490462606036423932L, (_ped), (id));
			int textureID = Function.Call<int>((Hash)334205219021784294L, (_ped), (id));
			Tuple<int, int> data = new Tuple<int, int>(item, textureID);
			pedData.Add(id, data);
		}
		if (pedData.Count > 0)
		{
			return pedData;
		}
		return null;
	}

	public static void SetPedVariation(Ped _ped, Dictionary<int, Tuple<int, int>> _pData)
	{
		foreach (int id in new List<int>
		{
			0, 1, 2, 3, 4, 5, 6, 8, 9, 10,
			11
		})
		{
			Function.Call((Hash)2750315038012726912L, (_ped), (id), (_pData[id].Item1), (_pData[id].Item2), (0));
		}
	}
}
