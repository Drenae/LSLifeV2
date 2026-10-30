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

	public static void SetWorkerMenuHealth(Ped p)
	{
		// Health and armor are displayed in CustomWorkerMenu.
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
			foreach (WeaponComponentHash component in Enum.GetValues(typeof(WeaponComponentHash)))
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
						GTA.UI.Notification.PostTicker("Arme donnée au personnage : " + ((object)weapon/*cast due to constrained. prefix*/).ToString());
					}
				}
				if (ele.Name == "COMPONENT" && !hasWeapon && Enum.TryParse<WeaponComponentHash>(ele.Value, out WeaponComponentHash wepcomp) && _ped != null)
				{
					_ped.Weapons.Current.Components[wepcomp].Active = true;
				}
			}
		}
		_ped.Weapons.Select(cWeapon);
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
			p.Task.FollowNavMeshTo(target);
		}
	}

	private static void CheckIfPartolsCanSee()
	{
		if (LSL.driveByHandler.GetDriveBies.Where((DriveBy r) => CanSee((Entity)(object)r.Leader, (Entity)(object)LSL.player.Character)).Count() > 0)
		{
			LSL.driveByHandler.SetAgressiveTarget(_agg: true, LSL.player.Character);
			GTA.UI.Notification.PostTicker("Vente repérée depuis un véhicule");
		}
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
