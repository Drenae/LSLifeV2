using System;
using GTA;

namespace LSlife;

internal enum WeaponDealerAction { Melee, Pistol, Armour, Dismiss }

// Gestion complete du vendeur d'armes, sans objets NativeUI.
internal static class WeaponDealerActions
{
	internal static void Execute(WeaponDealerAction item, int meleeIndex, int pistolIndex)
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
		if (item == WeaponDealerAction.Melee)
		{
			int total = LSL.meleePrice[meleeIndex] - LsFunctions.Discount(LSL.meleePrice[meleeIndex]);
			if (Game.Player.Money >= total)
			{
				WeaponHash weaponHash = default;
				Enum.TryParse<WeaponHash>(LSL.meleeWeps[meleeIndex], out weaponHash);
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
				Enum.TryParse<WeaponHash>(LSL.meleeWeps[meleeIndex], out weaponHash2);
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
		if (item == WeaponDealerAction.Pistol)
		{
			int total2 = LSL.pistolPrice[pistolIndex] - LsFunctions.Discount(LSL.pistolPrice[pistolIndex]);
			if (pistolIndex == 3)
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
				Enum.TryParse<WeaponHash>(LSL.pistolWeps[pistolIndex], out weaponHash3);
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
				Enum.TryParse<WeaponHash>(LSL.pistolWeps[pistolIndex], out weaponHash4);
				if (!Game.Player.Character.Weapons.HasWeapon(weaponHash4))
				{
					LSL.dealer1Debt += total2;
					Game.Player.Character.Weapons.Give(weaponHash4, 100, true, true);
					LsFunctions.UpdateSave();
				}
			}
		}
		if (item == WeaponDealerAction.Armour)
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
		if (item == WeaponDealerAction.Dismiss && (Entity)(object)LSL.weaponDealer1 != (Entity)null)
		{
			((Entity)LSL.weaponDealer1).CurrentBlip.Remove();
			CustomWeaponDealerMenu.Close();
			LSL.weaponDealer1.Task.WanderAround();
			((Entity)LSL.weaponDealer1).MarkAsNoLongerNeeded();
			LSL.weaponDealer1.LeaveGroup();
			LSL.weaponDealer1 = null;
			LSL.readyToDeal = false;
		}
	}

}
