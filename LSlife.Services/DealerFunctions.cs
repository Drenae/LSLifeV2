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
	public static int zeeDebtCap()
	{
		int initCap = 100000;
		if (LSL.hardMode)
		{
			initCap = 1000;
		}
		return initCap + (int)LSL.dealer1Rep;
	}

	public static int DealerDelay()
	{
		return 60000 * (10 - (int)LSL.areas[LSL.areaIndex].GangPresance);
	}

	public static string OrderMenuDesc()
	{
		// Résumé de facture utilisé par les commandes Zee.
		return "Facture : " + IntToMoney(LSL.NewOrder.Bill) + " $";
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

	public static void ContactAnswered(iFruitContact contact)
	{
		if (contact != null && contact.Name == "Zee")
		{
			// Le contact iFruit ouvre directement l'interface personnalisee.
			// Ne plus lire ni rafraichir les anciens widgets NativeUI.
			CustomZeeMenu.Toggle();
		}
		if (LSL.iFruit != null)
			LSL.iFruit.Close(1000);
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
			ped2.MoneyCarried = (uint)System.Math.Max(0L, (long)ped2.MoneyCarried - price);
			LSL.PlayerInventory[c.Drug.Name] -= c.amount;
			PlayTheSound();
			GTA.UI.Notification.PostTicker("Vendu : " + DrugDisplayName(c.Drug.Name) + " " + GramsToOz(c.amount) + ", il te reste " + GramsToOz(LSL.PlayerInventory[c.Drug.Name]) +". Réputation gagnée : ~g~" + repEarned + " ~s~, réputation totale ~g~" + LSL.areas[LSL.areaIndex].Reputation + "~s~ à " + LSL.areas[LSL.areaIndex].Name, false, false);
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
			ped3.MoneyCarried = (uint)System.Math.Max(0L, (long)ped3.MoneyCarried - price);
			LSL.PlayerInventory[c.Drug.Name] -= c.amount;
			PlayTheSound();
			GTA.UI.Notification.PostTicker("Vendu : " + DrugDisplayName(c.Drug.Name) + " " + GramsToOz(c.amount) + ", il te reste " + GramsToOz(LSL.PlayerInventory[c.Drug.Name]) +". Réputation gagnée : ~g~" + repEarned + " ~s~, réputation totale ~g~" + LSL.areas[LSL.areaIndex].Reputation + "~s~ à " + LSL.areas[LSL.areaIndex].Name, false, false);
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
				GTA.UI.Screen.ShowSubtitle("Hé, pour info, ~r~quelqu'un ~w~d'autre vend vers ~y~" + World.GetStreetName(((Entity)LSL.aDealer.dPed).Position) + "~w~.", 6000);
				AddBlip((Entity)(object)LSL.aDealer.dPed, (BlipColor)1, "Dealer", _friend: false, _edge: false);
				LSL.aDealer.Detected = true;
				tip = true;
			}
		}
		if (!tip && LSL.pigs.Count > 0 && !tip && !LSL.pigs[0].detected && RanRange(0, 5 + (int)LSL.areas[LSL.areaIndex].GangPresance) == 0)
		{
			GTA.UI.Screen.ShowSubtitle("J'ai entendu que les ~r~flics ~w~sont vers ~y~" + World.GetStreetName(((Entity)LSL.pigs[0].Ped).Position) + "~w~. Ils viennent par ici.", 6000);
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
				GTA.UI.Screen.ShowSubtitle("Tu recrutes du monde ?", 6000);
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
				GTA.UI.Notification.PostTicker(police.Count + " | " + Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (((Entity)pol).Handle)) + " | " + IsPolice(pol), false, false);
			}
			if (CanSee((Entity)(object)pol, (Entity)(object)Game.Player.Character))
			{
				SendTextMsg("Ils t'ont vu dealer. Tu vas attirer l'attention dans ce quartier.");
				LSL.areas[LSL.areaIndex].Heat++;
				Game.Player.Wanted.SetWantedLevel(1);
			Game.Player.Wanted.ApplyWantedLevelChangeNow();
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
				GTA.UI.Notification.PostTicker(police.Count + " | " + Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (((Entity)pol).Handle)) + " | " + IsPolice(pol), false, false);
			}
			position = ((Entity)pol).Position;
			if (position.DistanceTo(LSL.playerPos) < 40f && CanSee((Entity)(object)pol, (Entity)(object)Game.Player.Character))
			{
				SendTextMsg("Ils t'ont vu dealer. Tu vas attirer l'attention dans ce quartier.");
				LSL.areas[LSL.areaIndex].Heat += 200f;
				Game.Player.Wanted.SetWantedLevel(1);
			Game.Player.Wanted.ApplyWantedLevelChangeNow();
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
}
