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

internal static partial class LsMenus
{
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
				LsFunctions.TextMsg("ZEE", "Travail", "Non, tu attires trop l'attention.");
			}
			else if ((Entity)(object)LSL.weaponDealer1 == (Entity)null)
			{
				LSL.callDealer(wepdealer: true);
				LsFunctions.TextMsg("ZEE", "Travail", "J'envoie quelqu'un te rejoindre.");
			}
			else
			{
				LsFunctions.RemovePedFromWorld(LSL.weaponDealer1, clearTask: true);
				LSL.weaponDealer1 = null;
				LsFunctions.TextMsg("ZEE", "Travail", "OK, je leur dis de rentrer.");
			}
		}
		if (item == LSL.buyLift)
		{
			if (LSL.wepDealer == null)
			{
				sender.Visible = !sender.Visible;
				LSL.wepDealer = new Dealer(new Model(unchecked((PedHash)(-412008429))), ((Entity)Game.Player.Character).Position + ((Entity)Game.Player.Character).ForwardVector * 2f, new Model("INGOT"), LSL.playerGroup);
				if ((Entity)(object)LSL.wepDealer.ped != (Entity)null)
				{
					LsFunctions.TextMsg("ZEE", LSL.wepDealer.ped, "Travail", "J'envoie quelqu'un te rejoindre.");
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
				LsFunctions.TextMsg("ZEE", "Travail", "OK, je leur dis que tu ne peux pas venir.");
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
			LSL.payDebt.Text = "Rembourser la dette " + LsFunctions.IntToMoney(LSL.dealer1Debt) + " $";
			LsFunctions.PlayTheSound();
			LsFunctions.SaveDealer();
			Script.Wait(500);
			if (LSL.dealer1Debt > 0)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Merci d'avoir remboursé une partie de ta dette.");
			}
			else
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Merci, ta dette est entièrement remboursée.");
			}
		}
		else
		{
			LsFunctions.TextMsg("ZEE", "Travail", "Tu n'as pas d'argent.");
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
		if (item == LSL.melee)
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
		if (item == LSL.pistol)
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
					UI.ShowSubtitle("Tu ne peux plus rien transporter.", 4000);
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
					UI.ShowSubtitle("Tu ne peux plus rien transporter.", 4000);
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
			LSL.wepMenu.Visible = false;
			CustomWeaponDealerMenu.Close();
			LSL.weaponDealer1.Task.WanderAround();
			((Entity)LSL.weaponDealer1).MarkAsNoLongerNeeded();
			LSL.weaponDealer1.LeaveGroup();
			LSL.weaponDealer1 = null;
			LSL.readyToDeal = false;
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
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_YES"), ("SPEECH_PARAMS_FORCE"));
				LsFunctions.ChangeAdealerToPdealer();
				UI.ShowSubtitle("Ouais, je veux bien bosser pour toi.", 4000);
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
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_CURSE_HIGH"), ("SPEECH_PARAMS_FORCE"));
				if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
				{
					LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
				}
			}
			else
			{
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_NO"), ("SPEECH_PARAMS_FORCE"));
				UI.ShowSubtitle("Tu plaisantes ? Je vaux bien plus que ça !", 4000);
				LSL.aDealer.SetLastOfferAmount(LSL.hireDealerOfferAmount.Value);
			}
		}
		else if (LSL.rnd.Next(0, 3) == 0)
		{
			LSL.aDealer.NewState = Enums.DStates.AttackPlayer;
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_CURSE_HIGH"), ("SPEECH_PARAMS_FORCE"));
			UI.ShowSubtitle("Nan, je vais plutôt te buter et prendre ton fric.", 4000);
			if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
			{
				LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
			}
		}
		else
		{
			UI.ShowSubtitle("Propose plus et je réfléchirai.", 4000);
		}
	}

	public static void HireDealerOfferAmount_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.hireDealerMakeOffer.Text = "Offre : " + LsFunctions.IntToMoney(sender.Value) + " $";
	}
}
