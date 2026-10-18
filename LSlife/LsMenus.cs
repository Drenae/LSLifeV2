using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LSlife.OldCustomers;
using LSlife.Shots;
using NativeUI;

namespace LSlife;

internal static class LsMenus
{
	public static void OnstartDealSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (item != LSL.StartDeal)
		{
			return;
		}
		LSL.sellingWeed = LSL.sellWeed.Checked;
		LSL.sellingCrack = LSL.sellCrack.Checked;
		LSL.sellingCocaine = LSL.sellCocaine.Checked;
		if (!LSL.sellingWeed && !LSL.sellingCrack && !LSL.sellingCocaine)
		{
			LsFunctions.TextMsg("ZEE", "Travail", "Dis-moi ce que tu veux vendre.");
		}
		else if (!LSL.isDealing)
		{
			int total = LsFunctions.TotalDrugs(LSL.PlayerInventory) + LSL.HouseHandler.TotalStashDrugs();
			if (LSL.pStashVehicle != null)
			{
				total += LSL.pStashVehicle.TotalDrugs();
			}
			sender.Visible = !sender.Visible;
			if (LSL.playerHeat > 0f)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Tu attires trop l attention dans ce quartier.");
			}
			else if (total > 0)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "D accord, je fais passer le mot.");
				LSL.isDealing = true;
				LSL.lastStreet = World.GetStreetName(new Vector2(((Entity)Game.Player.Character).Position.X, ((Entity)Game.Player.Character).Position.Y));
				LSL.thisStreet = LSL.lastStreet;
				LSL.timeStartedDealing = Game.GameTime;
			}
			else
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Tu n as rien a vendre.");
			}
		}
		else if (LSL.isDealing)
		{
			LSL.isDealing = false;
			LsFunctions.TextMsg("ZEE", "Travail", "Tu n as rien a vendre.");
		}
	}

	public static void OnZeeMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (item == LSL.buyWeapons)
		{
			sender.Visible = !sender.Visible;
			if (LSL.playerHeat > 0f)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Non, tu attires trop l attention.");
			}
			else if ((Entity)(object)LSL.weaponDealer1 == (Entity)null)
			{
				LSL.callDealer(wepdealer: true);
				LsFunctions.TextMsg("ZEE", "Travail", "J envoie quelqu un te retrouver.");
			}
			else
			{
				LsFunctions.RemovePedFromWorld(LSL.weaponDealer1, clearTask: true);
				LSL.weaponDealer1 = null;
				LsFunctions.TextMsg("ZEE", "Travail", "D accord, je leur dis de rentrer.");
			}
		}
		if (item == LSL.buyLift)
		{
			if (LSL.wepDealer == null)
			{
				sender.Visible = !sender.Visible;
				LSL.wepDealer = new Dealer(new Model(unchecked((int)3882958867u)), ((Entity)Game.Player.Character).Position + ((Entity)Game.Player.Character).ForwardVector * 2f, new Model("INGOT"), LSL.playerGroup);
				if ((Entity)(object)LSL.wepDealer.ped != (Entity)null)
				{
					LsFunctions.TextMsg("ZEE", LSL.wepDealer.ped, "Travail", "J envoie quelqu un te retrouver.");
				}
			}
			else
			{
				LsFunctions.RemovePedFromWorld(LSL.wepDealer.ped, clearTask: true);
				Vehicle vehicle = LSL.wepDealer.vehicle;
				if (vehicle != null)
				{
					((Entity)vehicle).MarkAsNoLongerNeeded();
				}
				Vehicle vehicle2 = LSL.wepDealer.vehicle;
				if (vehicle2 != null)
				{
					Blip currentBlip = ((Entity)vehicle2).CurrentBlip;
					if (currentBlip != null)
					{
						currentBlip.Remove();
					}
				}
				LSL.wepDealer = null;
				LsFunctions.TextMsg("ZEE", "Travail", "D accord, je leur dis que tu ne peux pas venir.");
			}
		}
		if (item != LSL.payDebt)
		{
			return;
		}
		if (LSL.player.Money > 0)
		{
			int toPay = 0;
			toPay = ((LSL.dealer1Debt >= LSL.player.Money) ? LSL.player.Money : LSL.dealer1Debt);
			LSL.dealer1Debt -= toPay;
			Player player = LSL.player;
			player.Money -= toPay;
			LSL.dealer1Money += toPay;
			LsFunctions.Dealer1AddRep(LsFunctions.MoneyToRep(toPay, _debt: true));
			LSL.payDebt.Text = "Rembourser la dette : $" + LsFunctions.IntToMoney(LSL.dealer1Debt);
			LsFunctions.PlayTheSound();
			LsFunctions.SaveDealer();
			Script.Wait(500);
			if (LSL.dealer1Debt > 0)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Merci pour ce remboursement partiel.");
			}
			else
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Merci, ta dette est entierement remboursee.");
			}
		}
		else
		{
			LsFunctions.TextMsg("ZEE", "Travail", "Tu n as pas d argent.");
		}
	}

	public static void OnWepMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Invalid comparison between Unknown and I4
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		if ((object)item == LSL.melee)
		{
			int total = LSL.meleePrice[LSL.melee.Index] - LsFunctions.Discount(LSL.meleePrice[LSL.melee.Index]);
			if (Game.Player.Money >= total)
			{
				WeaponHash weaponHash = default;
				Enum.TryParse<WeaponHash>(LSL.meleeWeps[LSL.melee.Index], out weaponHash);
				if (!Game.Player.Character.Weapons.HasWeapon(weaponHash))
				{
					LsFunctions.HandOverGoods(LSL.weaponDealer1, LSL.player.Character);
					Player player = Game.Player;
					player.Money -= total;
					LsFunctions.PlayTheSound();
					LSL.dealer1Money += total;
					LSL.dealer1Rep += total / 100;
					Game.Player.Character.Weapons.Give(weaponHash, 0, true, true);
					LsFunctions.UpdateSave();
				}
			}
			else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total <= LsFunctions.zeeDebtCap())
			{
				WeaponHash weaponHash2 = default;
				Enum.TryParse<WeaponHash>(LSL.meleeWeps[LSL.melee.Index], out weaponHash2);
				if (!Game.Player.Character.Weapons.HasWeapon(weaponHash2))
				{
					LsFunctions.HandOverGoods(LSL.weaponDealer1, LSL.player.Character);
					LsFunctions.PlayTheSound();
					LSL.dealer1Debt += total;
					Game.Player.Character.Weapons.Give(weaponHash2, 0, true, true);
					LsFunctions.UpdateSave();
				}
			}
		}
		if ((object)item == LSL.pistol)
		{
			int total2 = LSL.pistolPrice[LSL.pistol.Index] - LsFunctions.Discount(LSL.pistolPrice[LSL.pistol.Index]);
			if (LSL.pistol.Index == 3)
			{
				if ((int)Game.Player.Character.Weapons.Current.Group == 416676503)
				{
					if (Game.Player.Money >= total2)
					{
						Player player2 = Game.Player;
						player2.Money -= total2;
						LSL.dealer1Money += total2;
						LSL.dealer1Rep += total2 / 100;
						Weapon current = Game.Player.Character.Weapons.Current;
						current.Ammo += 50;
						LsFunctions.UpdateSave();
					}
					else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total2 <= LsFunctions.zeeDebtCap())
					{
						LSL.dealer1Debt += total2;
						Weapon current2 = Game.Player.Character.Weapons.Current;
						current2.Ammo += 50;
						LsFunctions.UpdateSave();
					}
				}
				else if (Game.Player.Character.Weapons.HasWeapon((WeaponHash)453432689))
				{
					Game.Player.Character.Weapons.Select((WeaponHash)453432689);
					if (Game.Player.Money >= total2)
					{
						Player player3 = Game.Player;
						player3.Money -= total2;
						LSL.dealer1Money += total2;
						LSL.dealer1Rep += total2 / 100;
						Weapon current3 = Game.Player.Character.Weapons.Current;
						current3.Ammo += 50;
						LsFunctions.UpdateSave();
					}
					else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total2 <= LsFunctions.zeeDebtCap())
					{
						LSL.dealer1Debt += total2;
						Weapon current4 = Game.Player.Character.Weapons.Current;
						current4.Ammo += 50;
						LsFunctions.UpdateSave();
					}
				}
				else if (Game.Player.Character.Weapons.HasWeapon(unchecked((WeaponHash)(-1716589765))))
				{
					Game.Player.Character.Weapons.Select(unchecked((WeaponHash)(-1716589765)));
					if (Game.Player.Money >= total2)
					{
						Player player4 = Game.Player;
						player4.Money -= total2;
						LSL.dealer1Money += total2;
						LSL.dealer1Rep += total2 / 100;
						Weapon current5 = Game.Player.Character.Weapons.Current;
						current5.Ammo += 50;
						LsFunctions.UpdateSave();
					}
					else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total2 <= LsFunctions.zeeDebtCap())
					{
						LSL.dealer1Debt += total2;
						Weapon current6 = Game.Player.Character.Weapons.Current;
						current6.Ammo += 50;
						LsFunctions.UpdateSave();
					}
				}
				else if (Game.Player.Character.Weapons.HasWeapon(unchecked((WeaponHash)(-771403250))))
				{
					Game.Player.Character.Weapons.Select(unchecked((WeaponHash)(-771403250)));
					if (Game.Player.Money >= total2)
					{
						Player player5 = Game.Player;
						player5.Money -= total2;
						LSL.dealer1Money += total2;
						LSL.dealer1Rep += total2 / 100;
						Weapon current7 = Game.Player.Character.Weapons.Current;
						current7.Ammo += 50;
						LsFunctions.UpdateSave();
					}
					else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total2 <= LsFunctions.zeeDebtCap())
					{
						LSL.dealer1Debt += total2;
						Weapon current8 = Game.Player.Character.Weapons.Current;
						current8.Ammo += 50;
						LsFunctions.UpdateSave();
					}
				}
			}
			else if (Game.Player.Money >= total2)
			{
				WeaponHash weaponHash3 = default;
				Enum.TryParse<WeaponHash>(LSL.pistolWeps[LSL.pistol.Index], out weaponHash3);
				if (!Game.Player.Character.Weapons.HasWeapon(weaponHash3))
				{
					Player player6 = Game.Player;
					player6.Money -= total2;
					LSL.dealer1Money += total2;
					LSL.dealer1Rep += total2 / 100;
					Game.Player.Character.Weapons.Give(weaponHash3, 100, true, true);
					LsFunctions.UpdateSave();
				}
			}
			else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total2 <= LsFunctions.zeeDebtCap())
			{
				WeaponHash weaponHash4 = default;
				Enum.TryParse<WeaponHash>(LSL.pistolWeps[LSL.pistol.Index], out weaponHash4);
				if (!Game.Player.Character.Weapons.HasWeapon(weaponHash4))
				{
					LSL.dealer1Debt += total2;
					Game.Player.Character.Weapons.Give(weaponHash4, 100, true, true);
					LsFunctions.UpdateSave();
				}
			}
		}
		if (item == LSL.armour)
		{
			int total3 = 250 - LsFunctions.Discount(250);
			if (Game.Player.Money >= total3)
			{
				if (LsFunctions.IsPlayerArmoursMaxed())
				{
					UI.ShowSubtitle("Tu ne peux pas en transporter davantage", 4000);
				}
				else
				{
					Player player7 = Game.Player;
					player7.Money -= total3;
					LSL.dealer1Money += total3;
					LSL.dealer1Rep += total3 / 100;
					LsFunctions.GiveArmourToPlayer();
					LsFunctions.UpdateInventory();
				}
			}
			else if (LSL.dealer1Debt < LsFunctions.zeeDebtCap() && LSL.dealer1Debt + total3 <= LsFunctions.zeeDebtCap())
			{
				if (LsFunctions.IsPlayerArmoursMaxed())
				{
					UI.ShowSubtitle("Tu ne peux pas en transporter davantage", 4000);
				}
				else
				{
					LSL.dealer1Debt += total3;
					LSL.dealer1Rep += total3 / 100;
					LsFunctions.GiveArmourToPlayer();
					LsFunctions.UpdateInventory();
				}
			}
		}
		if (item == LSL.dismiss && (Entity)(object)LSL.weaponDealer1 != (Entity)null)
		{
			((Entity)LSL.weaponDealer1).CurrentBlip.Remove();
			LSL.wepMenu.Visible = !LSL.wepMenu.Visible;
			LSL.weaponDealer1.Task.WanderAround();
			((Entity)LSL.weaponDealer1).MarkAsNoLongerNeeded();
			LSL.weaponDealer1.LeaveGroup();
			LSL.weaponDealer1 = null;
			LSL.readyToDeal = false;
		}
	}

	public static void onWeedMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.WeedBuy && LSL.dealer1Weed > 0 && LSL.bushWeed.Value > 0)
		{
			int total = LSL.bushWeed.Value * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price));
			LSL.NewOrder.OrderWeed(LSL.bushWeed.Value, total);
			UIMenuSliderItem bushWeed = LSL.bushWeed;
			bushWeed.Maximum -= LSL.bushWeed.Value;
			LSL.dealer1Weed = LSL.bushWeed.Maximum;
			LSL.WeedBuy.Text = "Commander ~g~" + LSL.bushWeed.Value + " ~s~pour $" + LsFunctions.IntToMoney(total);
			LSL.PlaceOrder.Text = "Passer commande : $" + LsFunctions.IntToMoney(LSL.NewOrder.Bill);
			LsFunctions.DbugString("Weed Order changed");
			LsFunctions.UpdateOrderMenuDesc();
		}
	}

	public static void onCrackMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.CrackBuy && LSL.dealer1Crack > 0 && LSL.crack.Value > 0)
		{
			int total = LSL.crack.Value * (LSL.crack1Price - LsFunctions.Discount(LSL.crack1Price));
			LSL.NewOrder.OrderCrack(LSL.crack.Value, total);
			UIMenuSliderItem crack = LSL.crack;
			crack.Maximum -= LSL.crack.Value;
			LSL.dealer1Crack = LSL.crack.Maximum;
			LSL.CrackBuy.Text = "Commander ~g~" + LSL.crack.Value + " ~w~pour $~g~" + LsFunctions.IntToMoney(total);
			LSL.PlaceOrder.Text = "Passer commande : $" + LsFunctions.IntToMoney(LSL.NewOrder.Bill);
			LsFunctions.DbugString("Crack Order changed");
			LsFunctions.UpdateOrderMenuDesc();
		}
	}

	public static void onCocainMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.CocainBuy && LSL.dealer1Cocain > 0)
		{
			int total = LSL.cocaine.Value * (LSL.cocain1Price - LsFunctions.Discount(LSL.cocain1Price));
			LSL.NewOrder.OrderCocaine(LSL.cocaine.Value, total);
			UIMenuSliderItem cocaine = LSL.cocaine;
			cocaine.Maximum -= LSL.cocaine.Value;
			LSL.dealer1Cocain = LSL.cocaine.Maximum;
			LSL.CocainBuy.Text = "Commander ~g~" + LSL.cocaine.Value + " ~w~pour $~g~" + LsFunctions.IntToMoney(total);
			LSL.PlaceOrder.Text = "Passer commande : $" + LsFunctions.IntToMoney(LSL.NewOrder.Bill);
			LsFunctions.DbugString("Cocaïne Order changed");
			LsFunctions.UpdateOrderMenuDesc();
		}
	}

	public static void OnOrderDrugsMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.PlaceOrder && LSL.NewOrder != null && LSL.NewOrder.OrderValid())
		{
			LSL.NewOrder.Placed = true;
			LSL.callDealer(wepdealer: false);
			LSL.LsMenuPool.CloseAllMenus();
		}
	}

	public static void onPutVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c95: Invalid comparison between Unknown and I4
		//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
		if ((object)item == LSL.putWeedVeh)
		{
			LSL.pStashVehicle.drugCarWeed += LSL.putWeedVeh.Value;
			LSL.PlayerInventory["Weed"] -= LSL.putWeedVeh.Value;
			UIMenuSliderItem putWeedVeh = LSL.putWeedVeh;
			putWeedVeh.Maximum -= LSL.putWeedVeh.Value;
			LSL.takeWeedVeh.Maximum = LSL.pStashVehicle.drugCarWeed;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element in LSL.LSLifeSave.Descendants())
			{
				if (element.Name == "playerInventory")
				{
					foreach (XElement e in element.Elements())
					{
						if (e.Name == "weed")
						{
							e.Value = LSL.PlayerInventory["Weed"].ToString();
							break;
						}
					}
				}
				if (!(element.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e2 in element.Elements())
				{
					if (e2.Name == "weed")
					{
						e2.Value = LSL.pStashVehicle.drugCarWeed.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putWeedOzVeh)
		{
			LSL.pStashVehicle.drugCarWeedOz += LSL.putWeedOzVeh.Value;
			LsFunctions.pWeedOunce -= LSL.putWeedOzVeh.Value;
			UIMenuSliderItem putWeedOzVeh = LSL.putWeedOzVeh;
			putWeedOzVeh.Maximum -= LSL.putWeedOzVeh.Value;
			LSL.takeWeedOzVeh.Maximum = LSL.pStashVehicle.drugCarWeedOz;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element2 in LSL.LSLifeSave.Descendants())
			{
				if (element2.Name == "playerInventory")
				{
					foreach (XElement e3 in element2.Elements())
					{
						if (e3.Name == "weedOz")
						{
							e3.Value = LsFunctions.pWeedOunce.ToString();
							break;
						}
					}
				}
				if (!(element2.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e4 in element2.Elements())
				{
					if (e4.Name == "weedOz")
					{
						e4.Value = LSL.pStashVehicle.drugCarWeedOz.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putCrackVeh)
		{
			LSL.pStashVehicle.drugCarCrack += LSL.putCrackVeh.Value;
			LSL.PlayerInventory["Crack"] -= LSL.putCrackVeh.Value;
			UIMenuSliderItem putCrackVeh = LSL.putCrackVeh;
			putCrackVeh.Maximum -= LSL.putCrackVeh.Value;
			LSL.takeCrackVeh.Maximum = LSL.pStashVehicle.drugCarCrack;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element3 in LSL.LSLifeSave.Descendants())
			{
				if (element3.Name == "playerInventory")
				{
					foreach (XElement e5 in element3.Elements())
					{
						if (e5.Name == "crack")
						{
							e5.Value = LSL.PlayerInventory["Crack"].ToString();
							break;
						}
					}
				}
				if (!(element3.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e6 in element3.Elements())
				{
					if (e6.Name == "crack")
					{
						e6.Value = LSL.pStashVehicle.drugCarCrack.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putCrackOzVeh)
		{
			LSL.pStashVehicle.drugCarCrackOz += LSL.putCrackOzVeh.Value;
			LsFunctions.pCrackOunce -= LSL.putCrackOzVeh.Value;
			UIMenuSliderItem putCrackOzVeh = LSL.putCrackOzVeh;
			putCrackOzVeh.Maximum -= LSL.putCrackOzVeh.Value;
			LSL.takeCrackOzVeh.Maximum = LSL.pStashVehicle.drugCarCrackOz;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element4 in LSL.LSLifeSave.Descendants())
			{
				if (element4.Name == "playerInventory")
				{
					foreach (XElement e7 in element4.Elements())
					{
						if (e7.Name == "crackOz")
						{
							e7.Value = LsFunctions.pCrackOunce.ToString();
							break;
						}
					}
				}
				if (!(element4.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e8 in element4.Elements())
				{
					if (e8.Name == "crackOz")
					{
						e8.Value = LSL.pStashVehicle.drugCarCrackOz.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putCocainVeh)
		{
			LSL.pStashVehicle.drugCarCocain += LSL.putCocainVeh.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.putCocainVeh.Value;
			UIMenuSliderItem putCocainVeh = LSL.putCocainVeh;
			putCocainVeh.Maximum -= LSL.putCocainVeh.Value;
			LSL.takeCocainVeh.Maximum = LSL.pStashVehicle.drugCarCocain;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element5 in LSL.LSLifeSave.Descendants())
			{
				if (element5.Name == "playerInventory")
				{
					foreach (XElement e9 in element5.Elements())
					{
						if (e9.Name == "cocain")
						{
							e9.Value = LSL.PlayerInventory["Cocaine"].ToString();
							break;
						}
					}
				}
				if (!(element5.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e10 in element5.Elements())
				{
					if (e10.Name == "cocain")
					{
						e10.Value = LSL.pStashVehicle.drugCarCocain.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putCocainOzVeh)
		{
			LSL.pStashVehicle.drugCarCocainOz += LSL.putCocainOzVeh.Value;
			LsFunctions.pCocaineOunce -= LSL.putCocainOzVeh.Value;
			UIMenuSliderItem putCocainOzVeh = LSL.putCocainOzVeh;
			putCocainOzVeh.Maximum -= LSL.putCocainOzVeh.Value;
			LSL.takeCocainOzVeh.Maximum = LSL.pStashVehicle.drugCarCocainOz;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element6 in LSL.LSLifeSave.Descendants())
			{
				if (element6.Name == "playerInventory")
				{
					foreach (XElement e11 in element6.Elements())
					{
						if (e11.Name == "cocaineOz")
						{
							e11.Value = LsFunctions.pCocaineOunce.ToString();
							break;
						}
					}
				}
				if (!(element6.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e12 in element6.Elements())
				{
					if (e12.Name == "cocainOz")
					{
						e12.Value = LSL.pStashVehicle.drugCarCocainOz.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putMoneyVeh)
		{
			LSL.pStashVehicle.drugCarMoney += LSL.putMoneyVeh.Value;
			Player player = Game.Player;
			player.Money -= LSL.putMoneyVeh.Value;
			UIMenuSliderItem putMoneyVeh = LSL.putMoneyVeh;
			putMoneyVeh.Maximum -= LSL.putMoneyVeh.Value;
			LSL.takeMoneyVeh.Maximum = LSL.pStashVehicle.drugCarMoney;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element7 in LSL.LSLifeSave.Descendants())
			{
				if (!(element7.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e13 in element7.Elements())
				{
					if (e13.Name == "money")
					{
						e13.Value = LSL.pStashVehicle.drugCarMoney.ToString();
						break;
					}
				}
				break;
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.putWeaponVeh && LSL.putWeaponVeh.Items.Count > 0)
		{
			LSL.vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
			int ID = 0;
			int newID = 0;
			if (LSL.vehWeaponsXML.Descendants().Count() > 0)
			{
				foreach (XElement element8 in LSL.vehWeaponsXML.Descendants())
				{
					if (element8.Name == "weapon")
					{
						ID++;
						if (ID.ToString() != element8.FirstAttribute.Value)
						{
							newID = ID;
						}
					}
				}
			}
			XElement w = LSL.playerWeapons[LSL.putWeaponVeh.Index];
			if (w != null)
			{
				WeaponHash wep = default;
				Enum.TryParse(LSL.playerWeps[LSL.putWeaponVeh.Index], out wep);
				if ((int)wep != -1569615261)
				{
					w.SetAttributeValue("id", newID);
					LSL.vehWeaponsXML.Element("weapons")?.Add(w);
					LSL.vehWeaponsXML.Save("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
					Game.Player.Character.Weapons.Remove(wep);
					LSL.playerWeapons.Remove(w);
					LSL.playerWeps.RemoveAt(LSL.putWeaponVeh.Index);
					LSL.pStashVehicle.vehWeapons.Add(w);
					LSL.pStashVehicle.vehWeps.Add(LSL.putWeaponVeh.Items[LSL.putWeaponVeh.Index]);
				}
			}
		}
		LSL.pStashVehicle.UpdateCarStash();
	}

	public static void onTakeVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdd: Invalid comparison between Unknown and I4
		//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
		if ((object)item == LSL.takeWeedVeh)
		{
			LSL.PlayerInventory["Weed"] += LSL.takeWeedVeh.Value;
			LSL.pStashVehicle.drugCarWeed -= LSL.takeWeedVeh.Value;
			UIMenuSliderItem takeWeedVeh = LSL.takeWeedVeh;
			takeWeedVeh.Maximum -= LSL.takeWeedVeh.Value;
			LSL.putWeedVeh.Maximum = LSL.PlayerInventory["Weed"];
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element in LSL.LSLifeSave.Descendants())
			{
				if (element.Name == "playerInventory")
				{
					foreach (XElement e in element.Elements())
					{
						if (e.Name == "weed")
						{
							e.Value = LSL.PlayerInventory["Weed"].ToString();
							break;
						}
					}
				}
				if (!(element.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e2 in element.Elements())
				{
					if (e2.Name == "weed")
					{
						e2.Value = LSL.pStashVehicle.drugCarWeed.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeWeedOzVeh)
		{
			LsFunctions.pWeedOunce += LSL.takeWeedOzVeh.Value;
			LSL.pStashVehicle.drugCarWeedOz -= LSL.takeWeedOzVeh.Value;
			UIMenuSliderItem takeWeedOzVeh = LSL.takeWeedOzVeh;
			takeWeedOzVeh.Maximum -= LSL.takeWeedOzVeh.Value;
			LSL.putWeedOzVeh.Maximum = LsFunctions.pWeedOunce;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element2 in LSL.LSLifeSave.Descendants())
			{
				if (element2.Name == "playerInventory")
				{
					foreach (XElement e3 in element2.Elements())
					{
						if (e3.Name == "weedOz")
						{
							e3.Value = LsFunctions.pWeedOunce.ToString();
							break;
						}
					}
				}
				if (!(element2.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e4 in element2.Elements())
				{
					if (e4.Name == "weedOz")
					{
						e4.Value = LSL.pStashVehicle.drugCarWeedOz.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeCrackVeh)
		{
			LSL.PlayerInventory["Crack"] += LSL.takeCrackVeh.Value;
			LSL.pStashVehicle.drugCarCrack -= LSL.takeCrackVeh.Value;
			UIMenuSliderItem takeCrackVeh = LSL.takeCrackVeh;
			takeCrackVeh.Maximum -= LSL.takeCrackVeh.Value;
			LSL.putCrackVeh.Maximum = LSL.PlayerInventory["Crack"];
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element3 in LSL.LSLifeSave.Descendants())
			{
				if (element3.Name == "playerInventory")
				{
					foreach (XElement e5 in element3.Elements())
					{
						if (e5.Name == "crack")
						{
							e5.Value = LSL.PlayerInventory["Crack"].ToString();
							break;
						}
					}
				}
				if (!(element3.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e6 in element3.Elements())
				{
					if (e6.Name == "crack")
					{
						e6.Value = LSL.pStashVehicle.drugCarCrack.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeCrackOzVeh)
		{
			LsFunctions.pCrackOunce += LSL.takeCrackOzVeh.Value;
			LSL.pStashVehicle.drugCarCrackOz -= LSL.takeCrackOzVeh.Value;
			UIMenuSliderItem takeCrackOzVeh = LSL.takeCrackOzVeh;
			takeCrackOzVeh.Maximum -= LSL.takeCrackOzVeh.Value;
			LSL.putCrackOzVeh.Maximum = LsFunctions.pCrackOunce;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element4 in LSL.LSLifeSave.Descendants())
			{
				if (element4.Name == "playerInventory")
				{
					foreach (XElement e7 in element4.Elements())
					{
						if (e7.Name == "crackOz")
						{
							e7.Value = LsFunctions.pCrackOunce.ToString();
							break;
						}
					}
				}
				if (!(element4.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e8 in element4.Elements())
				{
					if (e8.Name == "crackOz")
					{
						e8.Value = LSL.pStashVehicle.drugCarCrackOz.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeCocainVeh)
		{
			LSL.PlayerInventory["Cocaine"] += LSL.takeCocainVeh.Value;
			LSL.pStashVehicle.drugCarCocain -= LSL.takeCocainVeh.Value;
			UIMenuSliderItem takeCocainVeh = LSL.takeCocainVeh;
			takeCocainVeh.Maximum -= LSL.takeCocainVeh.Value;
			LSL.putCocainStash.Maximum = LSL.PlayerInventory["Cocaine"];
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element5 in LSL.LSLifeSave.Descendants())
			{
				if (element5.Name == "playerInventory")
				{
					foreach (XElement e9 in element5.Elements())
					{
						if (e9.Name == "cocain")
						{
							e9.Value = LSL.PlayerInventory["Cocaine"].ToString();
							break;
						}
					}
				}
				if (!(element5.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e10 in element5.Elements())
				{
					if (e10.Name == "cocain")
					{
						e10.Value = LSL.pStashVehicle.drugCarCocain.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeCocainOzVeh)
		{
			LsFunctions.pCocaineOunce += LSL.takeCocainOzVeh.Value;
			LSL.pStashVehicle.drugCarCocainOz -= LSL.takeCocainOzVeh.Value;
			UIMenuSliderItem takeCocainOzVeh = LSL.takeCocainOzVeh;
			takeCocainOzVeh.Maximum -= LSL.takeCocainOzVeh.Value;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element6 in LSL.LSLifeSave.Descendants())
			{
				if (element6.Name == "playerInventory")
				{
					foreach (XElement e11 in element6.Elements())
					{
						if (e11.Name == "cocainOz")
						{
							e11.Value = LsFunctions.pCocaineOunce.ToString();
							break;
						}
					}
				}
				if (!(element6.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e12 in element6.Elements())
				{
					if (e12.Name == "cocainOz")
					{
						e12.Value = LSL.pStashVehicle.drugCarCocainOz.ToString();
						break;
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeMoneyVeh)
		{
			Player player = Game.Player;
			player.Money += LSL.takeMoneyVeh.Value;
			LSL.pStashVehicle.drugCarMoney -= LSL.takeMoneyVeh.Value;
			UIMenuSliderItem takeMoneyVeh = LSL.takeMoneyVeh;
			takeMoneyVeh.Maximum -= LSL.takeMoneyVeh.Value;
			LSL.putMoneyVeh.Maximum = Game.Player.Money;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element7 in LSL.LSLifeSave.Descendants())
			{
				if (!(element7.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e13 in element7.Elements())
				{
					if (e13.Name == "money")
					{
						e13.Value = LSL.pStashVehicle.drugCarMoney.ToString();
						break;
					}
				}
				break;
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if ((object)item == LSL.takeWeaponVeh && LSL.takeWeaponVeh.Items.Count > 0)
		{
			LSL.vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
			XElement w = LSL.pStashVehicle.vehWeapons[LSL.takeWeaponVeh.Index];
			XElement HASH = null;
			foreach (XElement e14 in w.Elements())
			{
				if (e14.Name == "HASH")
				{
					HASH = e14;
				}
			}
			if (HASH != null)
			{
				Enum.TryParse<WeaponHash>(HASH.Value, out WeaponHash wep);
				if ((int)wep != -1569615261)
				{
					foreach (XElement element8 in LSL.vehWeaponsXML.Descendants())
					{
						int ammo = 0;
						if (!(element8.Name == "HASH") || !(element8.Value == HASH.Value))
						{
							continue;
						}
						XElement rootElement = element8.Parent;
						foreach (XElement e15 in rootElement.Elements())
						{
							if (e15.Name == "AMMO")
							{
								int.TryParse(e15.Value, out ammo);
							}
						}
						Game.Player.Character.Weapons.Give(wep, ammo, true, false);
						Game.Player.Character.Weapons.Select(wep, true);
						foreach (XElement e16 in rootElement.Elements())
						{
							if (e16.Name == "COMPONENT")
							{
								Enum.TryParse<WeaponComponent>(e16.Value, out WeaponComponent wepcomp);
								Game.Player.Character.Weapons.Current.SetComponent(wepcomp, true);
							}
						}
						LSL.playerWeapons.Add(element8);
						LSL.playerWeps.Add(element8.Value);
						LSL.pStashVehicle.vehWeapons.Remove(w);
						rootElement.Remove();
						LSL.pStashVehicle.vehWeps.RemoveAt(LSL.takeWeaponVeh.Index);
						LSL.vehWeaponsXML.Save("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
						break;
					}
				}
			}
		}
		LSL.pStashVehicle.UpdateCarStash();
	}

	public static void onSliderChange(UIMenuSliderItem sender, int index)
	{
		if (sender == LSL.takeWeedVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take " + LsFunctions.GramsToOz(LSL.takeWeedVeh.Value);
		}
		else if (sender == LSL.putWeedVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put " + LsFunctions.GramsToOz(LSL.putWeedVeh.Value);
		}
		if (sender == LSL.takeWeedOzVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take ~r~" + LSL.takeWeedOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putWeedOzVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put ~r~" + LSL.putWeedOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.takeCrackVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take " + LsFunctions.GramsToOz(LSL.takeCrackVeh.Value);
		}
		else if (sender == LSL.putCrackVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put " + LsFunctions.GramsToOz(LSL.putCrackVeh.Value);
		}
		if (sender == LSL.takeCrackOzVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take ~r~" + LSL.takeCrackOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putCrackOzVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put ~r~" + LSL.putCrackOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.takeCocainVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take " + LsFunctions.GramsToOz(LSL.takeCocainVeh.Value);
		}
		else if (sender == LSL.putCocainVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put " + LsFunctions.GramsToOz(LSL.putCocainVeh.Value);
		}
		else if (sender == LSL.takeCocainOzVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take ~r~" + LSL.takeCocainOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putCocainOzVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put ~r~" + LSL.putCocainOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putMoneyVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put $" + LsFunctions.IntToMoney(LSL.putMoneyVeh.Value);
		}
		else if (sender == LSL.takeMoneyVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take $" + LsFunctions.IntToMoney(LSL.takeMoneyVeh.Value);
		}
		else if (sender == LSL.takeWeedStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Take " + LsFunctions.GramsToOz(LSL.takeWeedStash.Value);
		}
		else if (sender == LSL.putWeedStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put " + LsFunctions.GramsToOz(LSL.putWeedStash.Value);
		}
		else if (sender == LSL.takeCrackStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Take " + LsFunctions.GramsToOz(LSL.takeCrackStash.Value);
		}
		else if (sender == LSL.putCrackStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put " + LsFunctions.GramsToOz(LSL.putCrackStash.Value);
		}
		else if (sender == LSL.takeCocainStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Take " + LsFunctions.GramsToOz(LSL.takeCocainStash.Value);
		}
		else if (sender == LSL.putCocainStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put " + LsFunctions.GramsToOz(LSL.putCocainStash.Value);
		}
		else if (sender == LSL.takeMoneyStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Take $" + LsFunctions.IntToMoney(LSL.takeMoneyStash.Value);
		}
		else if (sender == LSL.putWeedOzStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put ~r~" + LSL.putWeedOzStash.Value + "~s~z";
		}
		else if (sender == LSL.putCrackOzStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put ~r~" + LSL.putCrackOzStash.Value + "~s~z";
		}
		else if (sender == LSL.putCocainOzStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put ~r~" + LsFunctions.pCocaineOunce + "~s~z";
		}
		else if (sender == LSL.putMoneyStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put $" + LsFunctions.IntToMoney(LSL.putMoneyStash.Value);
		}
		else if (sender == LSL.bushWeed)
		{
			int total = LSL.bushWeed.Value * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price));
			LSL.WeedBuy.Text = "Commander ~g~" + LSL.bushWeed.Value + " ~s~pour $" + LsFunctions.IntToMoney(total);
		}
		else if (sender == LSL.crack)
		{
			int total2 = LSL.crack.Value * (LSL.crack1Price - LsFunctions.Discount(LSL.crack1Price));
			LSL.CrackBuy.Text = "Commander ~g~" + LSL.crack.Value + " ~s~pour $" + LsFunctions.IntToMoney(total2);
		}
		else if (sender == LSL.cocaine)
		{
			int total3 = LSL.cocaine.Value * (LSL.cocain1Price - LsFunctions.Discount(LSL.cocain1Price));
			LSL.CocainBuy.Text = "Commander ~g~" + LSL.cocaine.Value + " ~s~pour $" + LsFunctions.IntToMoney(total3);
		}
	}

	public static void onListChange(UIMenuListItem sender, int index)
	{
		if (sender == LSL.takeWeaponStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Take " + sender.Items[LSL.takeWeaponStash.Index];
		}
		else if (sender == LSL.putWeaponStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Put " + sender.Items[LSL.putWeaponStash.Index];
		}
		else if (sender == LSL.takeWeaponVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Take " + sender.Items[LSL.takeWeaponVeh.Index];
		}
		else if (sender == LSL.putWeaponVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Put " + sender.Items[LSL.putWeaponVeh.Index];
		}
		else if (sender == LSL.stashPutMultiplier)
		{
			LSL.putWeedStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
			LSL.putCrackStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
			LSL.putCocainStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
			LSL.putMoneyStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
			LSL.putWeedOzStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
			LSL.putCrackOzStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
			LSL.putCocainOzStash.Multiplier = (int)LSL.multiplier[LSL.stashPutMultiplier.Index];
		}
		else if (sender == LSL.stashTakeMultiplier)
		{
			LSL.takeWeedStash.Multiplier = (int)LSL.multiplier[LSL.stashTakeMultiplier.Index];
			LSL.takeCrackStash.Multiplier = (int)LSL.multiplier[LSL.stashTakeMultiplier.Index];
			LSL.takeCocainStash.Multiplier = (int)LSL.multiplier[LSL.stashTakeMultiplier.Index];
			LSL.takeMoneyStash.Multiplier = (int)LSL.multiplier[LSL.stashTakeMultiplier.Index];
		}
		else if (sender == LSL.vehPutMultiplier)
		{
			LSL.putWeedVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
			LSL.putCrackVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
			LSL.putCocainVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
			LSL.putMoneyVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
			LSL.putWeedOzVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
			LSL.putCrackOzVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
			LSL.putCocainOzVeh.Multiplier = (int)LSL.multiplier[LSL.vehPutMultiplier.Index];
		}
		else if (sender == LSL.vehTakeMultiplier)
		{
			LSL.takeWeedVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
			LSL.takeCrackVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
			LSL.takeCocainVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
			LSL.takeMoneyVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
			LSL.takeWeedOzVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
			LSL.takeCrackOzVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
			LSL.takeCocainOzVeh.Multiplier = (int)LSL.multiplier[LSL.vehTakeMultiplier.Index];
		}
		else if (sender == LSL.melee)
		{
			((UIMenuItem)LSL.melee).Text = "Arme de mêlée $" + LsFunctions.IntToMoney(LSL.meleePrice[LSL.melee.Index] - LsFunctions.Discount(LSL.meleePrice[LSL.melee.Index]));
		}
		else if (sender == LSL.pistol)
		{
			((UIMenuItem)LSL.pistol).Text = "Pistolet $" + LsFunctions.IntToMoney(LSL.pistolPrice[LSL.pistol.Index] - LsFunctions.Discount(LSL.pistolPrice[LSL.pistol.Index]));
		}
	}

	public static void onStashMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Invalid comparison between Unknown and I4
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		if ((object)item == LSL.putWeedStash)
		{
			LSL.HouseHandler.CurrentHouse.Weed += LSL.putWeedStash.Value;
			LSL.PlayerInventory["Weed"] -= LSL.putWeedStash.Value;
		}
		else if ((object)item == LSL.putCrackStash)
		{
			LSL.HouseHandler.CurrentHouse.Crack += LSL.putCrackStash.Value;
			LSL.PlayerInventory["Crack"] -= LSL.putCrackStash.Value;
		}
		else if ((object)item == LSL.putCocainStash)
		{
			LSL.HouseHandler.CurrentHouse.Cocain += LSL.putCocainStash.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.putCocainStash.Value;
		}
		else if ((object)item == LSL.putMoneyStash)
		{
			LSL.HouseHandler.CurrentHouse.Money += LSL.putMoneyStash.Value;
			Player player = Game.Player;
			player.Money -= LSL.putMoneyStash.Value;
		}
		else if ((object)item == LSL.putWeedOzStash)
		{
			LSL.HouseHandler.CurrentHouse.Weed += LSL.putWeedOzStash.Value * 28;
			LsFunctions.pWeedOunce -= LSL.putWeedOzStash.Value;
		}
		else if ((object)item == LSL.putCrackOzStash)
		{
			LSL.HouseHandler.CurrentHouse.Crack += LSL.putCrackOzStash.Value * 28;
			LsFunctions.pCrackOunce -= LSL.putCrackOzStash.Value;
		}
		else if ((object)item == LSL.putCocainOzStash)
		{
			LSL.HouseHandler.CurrentHouse.Cocain += LSL.putCocainOzStash.Value * 28;
			LsFunctions.pCocaineOunce -= LSL.putCocainOzStash.Value;
		}
		else if ((object)item == LSL.putArmourStash)
		{
			int amount = LSL.putArmourStash.Index;
			if (amount > 0 && LSL.playerArmours > 0)
			{
				LSL.playerArmours -= amount;
				LSL.HouseHandler.CurrentHouse.Armor += amount;
				foreach (XElement ele in LSL.LSLifeSave.Descendants())
				{
					if (!(ele.Name == "playerInventory"))
					{
						continue;
					}
					foreach (XElement el in ele.Elements())
					{
						if (el.Name == "armour")
						{
							el.Value = LSL.playerArmours.ToString();
							break;
						}
					}
					break;
				}
				LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			}
		}
		else if ((object)item == LSL.putWeaponStash && LSL.putWeaponStash.Items.Count > 0)
		{
			int ID = 0;
			int newID = 0;
			if (LSL.HouseHandler.CurrentHouse.Weapons.Count > 0)
			{
				foreach (XElement element in LSL.HouseHandler.CurrentHouse.Weapons)
				{
					if (element.Name == "weapon")
					{
						ID++;
						if (ID.ToString() != element.FirstAttribute.Value)
						{
							newID = ID;
						}
					}
				}
			}
			XElement w = LSL.playerWeapons[LSL.putWeaponStash.Index];
			if (w != null)
			{
				WeaponHash wep = default;
				Enum.TryParse(LSL.playerWeps[LSL.putWeaponStash.Index], out wep);
				if ((int)wep != -1569615261)
				{
					w.SetAttributeValue("id", newID);
					LSL.HouseHandler.CurrentHouse.Weapons.Add(w);
					LSL.HouseHandler.CurrentHouse.Weps.Add(LSL.putWeaponStash.Items[LSL.putWeaponStash.Index]);
					Game.Player.Character.Weapons.Remove(wep);
					LSL.playerWeapons.Remove(w);
					LSL.playerWeps.RemoveAt(LSL.putWeaponStash.Index);
				}
			}
		}
		LsFunctions.UpdateInventory();
		LSL.HouseHandler.UpdateMenu(LSL.HouseHandler.CurrentHouse);
		LSL.HouseHandler.SaveHouse(LSL.HouseHandler.CurrentHouse);
	}

	public static void onPlayerMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Invalid comparison between Unknown and I4
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		if ((object)item == LSL.takeWeedStash)
		{
			LSL.PlayerInventory["Weed"] += LSL.takeWeedStash.Value;
			LSL.HouseHandler.CurrentHouse.Weed -= LSL.takeWeedStash.Value;
		}
		else if ((object)item == LSL.takeCrackStash)
		{
			LSL.PlayerInventory["Crack"] += LSL.takeCrackStash.Value;
			LSL.HouseHandler.CurrentHouse.Crack -= LSL.takeCrackStash.Value;
		}
		else if ((object)item == LSL.takeCocainStash)
		{
			LSL.PlayerInventory["Cocaine"] += LSL.takeCocainStash.Value;
			LSL.HouseHandler.CurrentHouse.Cocain -= LSL.takeCocainStash.Value;
		}
		else if ((object)item == LSL.takeMoneyStash)
		{
			Player player = Game.Player;
			player.Money += LSL.takeMoneyStash.Value;
			LSL.HouseHandler.CurrentHouse.Money -= LSL.takeMoneyStash.Value;
		}
		else if ((object)item == LSL.takeArmourStash)
		{
			int amount = LSL.takeArmourStash.Index;
			int max = 5;
			if (Game.Player.Character.Armor < 100)
			{
				max = 6;
			}
			if (amount > 0 && LSL.HouseHandler.CurrentHouse.Armor > 0 && amount + LSL.playerArmours <= max)
			{
				if (Game.Player.Character.Armor < 100)
				{
					Game.Player.Character.Armor = 100;
					amount--;
					LSL.HouseHandler.CurrentHouse.Armor--;
				}
				if (amount > 0)
				{
					LSL.playerArmours += amount;
					LSL.HouseHandler.CurrentHouse.Armor -= amount;
				}
				foreach (XElement ele in LSL.LSLifeSave.Descendants())
				{
					if (!(ele.Name == "playerInventory"))
					{
						continue;
					}
					foreach (XElement el in ele.Elements())
					{
						if (el.Name == "armour")
						{
							el.Value = LSL.playerArmours.ToString();
							break;
						}
					}
					break;
				}
				LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			}
		}
		else if ((object)item == LSL.takeWeaponStash && LSL.takeWeaponStash.Items.Count > 0)
		{
			XElement w = LSL.HouseHandler.CurrentHouse.Weapons[LSL.takeWeaponStash.Index];
			XElement HASH = null;
			foreach (XElement e in w.Elements())
			{
				if (e.Name == "HASH")
				{
					HASH = e;
				}
			}
			if (HASH != null)
			{
				Enum.TryParse<WeaponHash>(HASH.Value, out WeaponHash wep);
				if ((int)wep != -1569615261)
				{
					bool hasWeapon = false;
					int ammo = 0;
					foreach (XElement aEl in w.Descendants())
					{
						if (aEl.Name == "AMMO")
						{
							int.TryParse(aEl.Value, out ammo);
							break;
						}
					}
					if (!Game.Player.Character.Weapons.HasWeapon(wep))
					{
						Game.Player.Character.Weapons.Give(wep, ammo, true, true);
						Game.Player.Character.Weapons.Select(wep);
					}
					else
					{
						hasWeapon = true;
						Game.Player.Character.Weapons.Select(wep);
						Weapon current = Game.Player.Character.Weapons.Current;
						current.Ammo += ammo;
					}
					if (!hasWeapon)
					{
						LSL.playerWeapons.Add(w);
						LSL.playerWeps.Add(HASH.Value);
						foreach (XElement aEl2 in w.Descendants())
						{
							if (aEl2.Name == "COMPONENT")
							{
								Enum.TryParse<WeaponComponent>(aEl2.Value, out WeaponComponent wepcomp);
								Game.Player.Character.Weapons.Current.SetComponent(wepcomp, true);
							}
						}
					}
					LSL.HouseHandler.CurrentHouse.Weapons.Remove(w);
					LSL.HouseHandler.CurrentHouse.Weps.RemoveAt(LSL.takeWeaponStash.Index);
				}
			}
		}
		LsFunctions.UpdateInventory();
		LSL.HouseHandler.UpdateMenu(LSL.HouseHandler.CurrentHouse);
		LSL.HouseHandler.SaveHouse(LSL.HouseHandler.CurrentHouse);
	}

	public static void onCustomerMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		if (item == LSL.sell)
		{
			if (LSL.PlayerInventory[LSL.customerSellTo.Drug.Name] < LSL.customerSellTo.amount)
			{
				return;
			}
			foreach (Drug d in LSL.areas[LSL.areaIndex].Drugs)
			{
				if (d.Name == LSL.customerSellTo.Drug.Name)
				{
					d.Supplied = true;
				}
			}
			if (Game.Player.Character.IsInVehicle())
			{
				LsFunctions.MakeStreetSale(LSL.customerSellTo, inCar: true);
				LSL.customerMainMenu.Visible = !LSL.customerMainMenu.Visible;
				LSL.lastTimeDealDone = Game.GameTime;
			}
			else
			{
				LsFunctions.MakeStreetSale(LSL.customerSellTo, inCar: false);
				LSL.customerMainMenu.Visible = !LSL.customerMainMenu.Visible;
				LSL.lastTimeDealDone = Game.GameTime;
			}
		}
		else
		{
			if (item != LSL.turnAway)
			{
				return;
			}
			foreach (Drug d2 in LSL.areas[LSL.areaIndex].Drugs)
			{
				if (d2.Name == LSL.customerSellTo.Drug.Name)
				{
					d2.Supplied = false;
				}
			}
			LSL.customerMainMenu.Visible = !LSL.customerMainMenu.Visible;
			if (LSL.customerSellTo.FarAway)
			{
				ShotHandler.RemoveShot(ShotHandler.shots.First((Shot s) => s.customer == LSL.customerSellTo));
				LSL.lastTimeDealDone = Game.GameTime;
				OldCustomerHandler.AddCustomer(LSL.customerSellTo.Ped, LSL.customerSellTo.Vehicle, LSL.HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(LSL.customerSellTo.Drug.Name, LSL.customerSellTo.amount));
				return;
			}
			if ((Entity)(object)LSL.customerSellTo.Vehicle != (Entity)null && ((Entity)LSL.customerSellTo.Vehicle).Exists())
			{
				LSL.customerSellTo.Ped.Task.PerformSequence(LSL.PerformTaskSeq(1, LSL.customerSellTo.Vehicle, Vector3.Zero));
			}
			LSL.customers.Remove(LSL.customerSellTo);
			LSL.lastTimeDealDone = Game.GameTime;
			OldCustomerHandler.AddCustomer(LSL.customerSellTo.Ped, LSL.customerSellTo.Vehicle, LSL.HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(LSL.customerSellTo.Drug.Name, LSL.customerSellTo.amount));
			LSL.customerSellTo = null;
		}
	}

	public static void HireDealerFollowSelect(UIMenu sender, UIMenuItem selectedItem)
	{
		LSL.hireDealerMainMenu.Visible = false;
		if (LSL.aDealer != null)
		{
			LSL.aDealer.NewState = Enums.DStates.Normal;
		}
	}

	public static void HireDealerOfferSelect(UIMenu sender, UIMenuItem selectedItem)
	{
		LSL.hireDealerMainMenu.Visible = false;
		if (LSL.aDealer == null)
		{
			return;
		}
		LSL.aDealer.NewState = Enums.DStates.Normal;
		if (LSL.aDealer.LastOffer < LSL.hireDealerOfferAmount.Value)
		{
			if (LSL.aDealer.DecidePlayerOffer(LSL.hireDealerOfferAmount.Value))
			{
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (InputArgument)(LSL.aDealer.dPed), (InputArgument)("GENERIC_YES"), (InputArgument)("SPEECH_PARAMS_FORCE"));
				LsFunctions.ChangeAdealerToPdealer();
				UI.ShowSubtitle("D'accord, je travaillerai pour toi.", 4000);
				Player player = Game.Player;
				player.Money -= LSL.hireDealerOfferAmount.Value;
				LsFunctions.PlayTheSound();
				LSL.hireDealerOfferAmount.Maximum = Game.Player.Money;
				if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
				{
					LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
				}
			}
			else if (LSL.rnd.Next(0, 3) == 0)
			{
				LSL.aDealer.NewState = Enums.DStates.AttackPlayer;
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (InputArgument)(LSL.aDealer.dPed), (InputArgument)("GENERIC_CURSE_HIGH"), (InputArgument)("SPEECH_PARAMS_FORCE"));
				if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
				{
					LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
				}
			}
			else
			{
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (InputArgument)(LSL.aDealer.dPed), (InputArgument)("GENERIC_NO"), (InputArgument)("SPEECH_PARAMS_FORCE"));
				UI.ShowSubtitle("Tu plaisantes ? Je vaux bien plus que ça !", 4000);
				LSL.aDealer.SetLastOfferAmount(LSL.hireDealerOfferAmount.Value);
			}
		}
		else if (LSL.rnd.Next(0, 3) == 0)
		{
			LSL.aDealer.NewState = Enums.DStates.AttackPlayer;
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (InputArgument)(LSL.aDealer.dPed), (InputArgument)("GENERIC_CURSE_HIGH"), (InputArgument)("SPEECH_PARAMS_FORCE"));
			UI.ShowSubtitle("Non, je vais te tuer et prendre ton argent.", 4000);
			if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
			{
				LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
			}
		}
		else
		{
			UI.ShowSubtitle("Propose davantage et j'accepterai peut-être.", 4000);
		}
	}

	public static void HireDealerOfferAmount_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.hireDealerMakeOffer.Text = "Offre : $" + LsFunctions.IntToMoney(sender.Value);
	}

	public static void WArmour_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (LSL.playerArmours <= 0)
		{
			return;
		}
		LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
		PlayerDealer currentDealer = LSL.DealerHandler.currentDealer;
		if (currentDealer != null)
		{
			Ped ped = currentDealer.Ped;
			if (((ped != null) ? new int?(ped.Armor) : ((int?)null)) < 200)
			{
				LSL.DealerHandler.currentDealer.Ped.Armor = 200;
				LSL.DealerHandler.currentDealer.dArmour = LSL.DealerHandler.currentDealer.Ped.Armor;
				LSL.playerArmours--;
				if (LSL.playerArmours == 0)
				{
					selectedItem.Text = "Plus de gilet pare-balles";
				}
				LsFunctions.PlayTheSound();
				LsFunctions.UpdateInventory();
				LsFunctions.SetWorkerMenuHealth(LSL.DealerHandler.currentDealer.Ped);
				return;
			}
		}
		UI.ShowSubtitle("J'ai déjà assez de protection.", 4000);
	}

	public static void WWeapons_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Invalid comparison between Unknown and I4
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		if (LSL.DEBUG)
		{
			UI.Notify("Arme transférée");
		}
		if (LSL.wWeapons.Items.Count > 0)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			WeaponHash wep = default;
			if (Enum.TryParse(LSL.playerWeps[LSL.wWeapons.Index], out wep))
			{
				if ((int)wep == -1569615261)
				{
					return;
				}
				XElement w = LSL.playerWeapons[LSL.wWeapons.Index];
				if (w != null)
				{
					XDocument dealersDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
					if (dealersDoc.Descendants().Count() <= 0)
					{
						return;
					}
					{
						foreach (XElement element in dealersDoc.Descendants())
						{
							if (!(element.Name == "dealer"))
							{
								continue;
							}
							bool found = false;
							foreach (XElement e in element.Elements())
							{
								if (e.Name == "id" && e.Value == LSL.DealerHandler.currentDealer.ID.ToString())
								{
									found = true;
									break;
								}
							}
							if (!found)
							{
								continue;
							}
							bool hasWeapon = false;
							XElement dWeapon = null;
							foreach (XElement e2 in element.Descendants())
							{
								if (e2.Name == "HASH" && e2.Value == ((object)wep/*cast due to constrained. prefix*/).ToString())
								{
									hasWeapon = true;
									dWeapon = e2.Parent;
									break;
								}
							}
							if (hasWeapon)
							{
								dWeapon.ReplaceWith(w);
							}
							else
							{
								element.Add(w);
							}
							dealersDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
							int ammo = 1000;
							foreach (XElement e3 in w.Descendants())
							{
								if (e3.Name == "AMMO")
								{
									int.TryParse(e3.Value, out ammo);
									PlayerDealer currentDealer = LSL.DealerHandler.currentDealer;
									if (currentDealer != null)
									{
										currentDealer.Ped.Weapons.Give(wep, ammo, true, true);
									}
									PlayerDealer currentDealer2 = LSL.DealerHandler.currentDealer;
									if (currentDealer2 != null)
									{
										Ped ped = currentDealer2.Ped;
										if (ped != null)
										{
											ped.Weapons.Select(wep);
										}
									}
								}
								if (!(e3.Name == "COMPONENT") || !Enum.TryParse<WeaponComponent>(e3.Value, out WeaponComponent wepcomp))
								{
									continue;
								}
								PlayerDealer currentDealer3 = LSL.DealerHandler.currentDealer;
								if (currentDealer3 != null)
								{
									Ped ped2 = currentDealer3.Ped;
									if (ped2 != null)
									{
										ped2.Weapons.Current.SetComponent(wepcomp, true);
									}
								}
							}
							Game.Player.Character.Weapons.Remove(wep);
							LSL.playerWeapons.Remove(w);
							LSL.playerWeps.RemoveAt(LSL.wWeapons.Index);
							LSL.wWeapons.Items = LSL.playerWeps;
							LSL.wWeapons.Index = 0;
							if (LSL.DEBUG)
							{
								UI.Notify("Enregistrement de l'arme du dealer " + ((object)wep/*cast due to constrained. prefix*/).ToString() + " munitions :" + ammo);
							}
							break;
						}
						return;
					}
				}
				if (LSL.DEBUG)
				{
					UI.Notify("Aucune arme correspondante dans l'inventaire");
				}
			}
			else if (LSL.DEBUG)
			{
				UI.Notify("Identifiant d'arme introuvable : " + LSL.playerWeps[LSL.wWeapons.Index]);
			}
		}
		else if (LSL.DEBUG)
		{
			UI.Notify("Aucune arme trouvée");
		}
	}

	internal static void WOrder_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (LSL.DealerHandler.currentDealer != null)
		{
			Enum.TryParse<Enums.PdStates>(LSL.wOrder.Items[LSL.wOrder.Index].ToString(), out LSL.DealerHandler.currentDealer.NewState);
			LSL.wOrder.Items = LSL.DealerHandler.currentDealer.Orders();
			LSL.wOrder.Index = 0;
		}
	}

	public static void WConfirm_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int i = 0;
		using (List<PlayerDealer>.Enumerator enumerator = LSL.DealerHandler.dealers.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != LSL.DealerHandler.currentDealer)
			{
				i++;
			}
		}
		bool change = false;
		if (LSL.wWeed.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Weed"] += LSL.wWeed.Value;
			LSL.PlayerInventory["Weed"] -= LSL.wWeed.Value;
			LSL.wWeed.Value = 0;
			((UIMenuItem)LSL.wWeed).Text = "Cannabis " + LsFunctions.GramsToOz(LSL.wWeed.Value);
			change = true;
		}
		if (LSL.wCrack.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Crack"] += LSL.wCrack.Value;
			LSL.PlayerInventory["Crack"] -= LSL.wCrack.Value;
			LSL.wCrack.Value = 0;
			((UIMenuItem)LSL.wCrack).Text = "Crack " + LsFunctions.GramsToOz(LSL.wCrack.Value);
			change = true;
		}
		if (LSL.wCocaine.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Cocaine"] += LSL.wCocaine.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.wCocaine.Value;
			LSL.wCocaine.Value = 0;
			((UIMenuItem)LSL.wCocaine).Text = "Cocaïne " + LsFunctions.GramsToOz(LSL.wCocaine.Value);
			change = true;
		}
		if (change)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			LsFunctions.UpdateInventory();
			LSL.DealerHandler.dealers[i].SetDrugDesc();
			LSL.DealerHandler.needsSave = true;
		}
	}

	public static void WCollect_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int i = 0;
		using (List<PlayerDealer>.Enumerator enumerator = LSL.DealerHandler.dealers.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != LSL.DealerHandler.currentDealer)
			{
				i++;
			}
		}
		if (LSL.DealerHandler.dealers[i].Money >= 100)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			LSL.DealerHandler.dealers[i].GiveMoneyToPlayer();
			LSL.DealerHandler.dealers[i].SetDrugDesc();
			LSL.WorkerMainMenu.CurrentSelection = 3;
		}
	}

	public static void WCocaine_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		((UIMenuItem)LSL.wCocaine).Text = "Cocaïne " + LsFunctions.GramsToOz(sender.Value);
	}

	public static void WCrack_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		((UIMenuItem)LSL.wCrack).Text = "Crack " + LsFunctions.GramsToOz(sender.Value);
	}

	public static void WWeed_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		((UIMenuItem)LSL.wWeed).Text = "Cannabis " + LsFunctions.GramsToOz(sender.Value);
	}
}
