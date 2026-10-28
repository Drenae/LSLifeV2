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
	public static void onSliderChange(UIMenuSliderItem sender, int index)
	{
		if (sender == LSL.takeWeedVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.GramsToOz(LSL.takeWeedVeh.Value);
		}
		else if (sender == LSL.putWeedVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.GramsToOz(LSL.putWeedVeh.Value);
		}
		if (sender == LSL.takeWeedOzVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer ~r~" + LSL.takeWeedOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putWeedOzVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer ~r~" + LSL.putWeedOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.takeCrackVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.GramsToOz(LSL.takeCrackVeh.Value);
		}
		else if (sender == LSL.putCrackVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.GramsToOz(LSL.putCrackVeh.Value);
		}
		if (sender == LSL.takeCrackOzVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer ~r~" + LSL.takeCrackOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putCrackOzVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer ~r~" + LSL.putCrackOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.takeCocainVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.GramsToOz(LSL.takeCocainVeh.Value);
		}
		else if (sender == LSL.putCocainVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.GramsToOz(LSL.putCocainVeh.Value);
		}
		else if (sender == LSL.takeCocainOzVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer ~r~" + LSL.takeCocainOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putCocainOzVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer ~r~" + LSL.putCocainOzVeh.Value + "~s~z";
		}
		else if (sender == LSL.putMoneyVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.IntToMoney(LSL.putMoneyVeh.Value) + " $";
		}
		else if (sender == LSL.takeMoneyVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.IntToMoney(LSL.takeMoneyVeh.Value) + " $";
		}
		else if (sender == LSL.takeWeedStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.GramsToOz(LSL.takeWeedStash.Value);
		}
		else if (sender == LSL.putWeedStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.GramsToOz(LSL.putWeedStash.Value);
		}
		else if (sender == LSL.takeCrackStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.GramsToOz(LSL.takeCrackStash.Value);
		}
		else if (sender == LSL.putCrackStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.GramsToOz(LSL.putCrackStash.Value);
		}
		else if (sender == LSL.takeCocainStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.GramsToOz(LSL.takeCocainStash.Value);
		}
		else if (sender == LSL.putCocainStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.GramsToOz(LSL.putCocainStash.Value);
		}
		else if (sender == LSL.takeMoneyStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Retirer " + LsFunctions.IntToMoney(LSL.takeMoneyStash.Value) + " $";
		}
		else if (sender == LSL.putWeedOzStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer ~r~" + LSL.putWeedOzStash.Value + "~s~z";
		}
		else if (sender == LSL.putCrackOzStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer ~r~" + LSL.putCrackOzStash.Value + "~s~z";
		}
		else if (sender == LSL.putCocainOzStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer ~r~" + LsFunctions.pCocaineOunce + "~s~z";
		}
		else if (sender == LSL.putMoneyStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer " + LsFunctions.IntToMoney(LSL.putMoneyStash.Value) + " $";
		}
		else if (sender == LSL.bushWeed)
		{
			int total = LSL.bushWeed.Value * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price));
			LSL.WeedBuy.Text = "Commander ~g~" + LSL.bushWeed.Value + " ~s~pour " + LsFunctions.IntToMoney(total) + " $";
		}
		else if (sender == LSL.crack)
		{
			int total2 = LSL.crack.Value * (LSL.crack1Price - LsFunctions.Discount(LSL.crack1Price));
			LSL.CrackBuy.Text = "Commander ~g~" + LSL.crack.Value + " ~s~pour " + LsFunctions.IntToMoney(total2) + " $";
		}
		else if (sender == LSL.cocaine)
		{
			int total3 = LSL.cocaine.Value * (LSL.cocain1Price - LsFunctions.Discount(LSL.cocain1Price));
			LSL.CocainBuy.Text = "Commander ~g~" + LSL.cocaine.Value + " ~s~pour " + LsFunctions.IntToMoney(total3) + " $";
		}
	}

	public static void onListChange(UIMenuListItem sender, int index)
	{
		if (sender == LSL.takeWeaponStash)
		{
			((UIText)LSL.stashTakeMenu.Subtitle).Caption = "Retirer " + sender.Items[LSL.takeWeaponStash.Index];
		}
		else if (sender == LSL.putWeaponStash)
		{
			((UIText)LSL.stashPutMenu.Subtitle).Caption = "Déposer " + sender.Items[LSL.putWeaponStash.Index];
		}
		else if (sender == LSL.takeWeaponVeh)
		{
			((UIText)LSL.vehStashTakeMenu.Subtitle).Caption = "Retirer " + sender.Items[LSL.takeWeaponVeh.Index];
		}
		else if (sender == LSL.putWeaponVeh)
		{
			((UIText)LSL.vehStashPutMenu.Subtitle).Caption = "Déposer " + sender.Items[LSL.putWeaponVeh.Index];
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
			LSL.melee.Text = "Arme blanche " + LsFunctions.IntToMoney(LSL.meleePrice[LSL.melee.Index] - LsFunctions.Discount(LSL.meleePrice[LSL.melee.Index])) + " $";
		}
		else if (sender == LSL.pistol)
		{
			LSL.pistol.Text = "Pistolet " + LsFunctions.IntToMoney(LSL.pistolPrice[LSL.pistol.Index] - LsFunctions.Discount(LSL.pistolPrice[LSL.pistol.Index])) + " $";
		}
	}

	public static void onStashMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Invalid comparison between Unknown and I4
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		if (item == LSL.putWeedStash)
		{
			LSL.HouseHandler.CurrentHouse.Weed += LSL.putWeedStash.Value;
			LSL.PlayerInventory["Weed"] -= LSL.putWeedStash.Value;
		}
		else if (item == LSL.putCrackStash)
		{
			LSL.HouseHandler.CurrentHouse.Crack += LSL.putCrackStash.Value;
			LSL.PlayerInventory["Crack"] -= LSL.putCrackStash.Value;
		}
		else if (item == LSL.putCocainStash)
		{
			LSL.HouseHandler.CurrentHouse.Cocain += LSL.putCocainStash.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.putCocainStash.Value;
		}
		else if (item == LSL.putMoneyStash)
		{
			LSL.HouseHandler.CurrentHouse.Money += LSL.putMoneyStash.Value;
			Player player = Game.Player;
			player.Money -= LSL.putMoneyStash.Value;
		}
		else if (item == LSL.putWeedOzStash)
		{
			LSL.HouseHandler.CurrentHouse.Weed += LSL.putWeedOzStash.Value * 28;
			LsFunctions.pWeedOunce -= LSL.putWeedOzStash.Value;
		}
		else if (item == LSL.putCrackOzStash)
		{
			LSL.HouseHandler.CurrentHouse.Crack += LSL.putCrackOzStash.Value * 28;
			LsFunctions.pCrackOunce -= LSL.putCrackOzStash.Value;
		}
		else if (item == LSL.putCocainOzStash)
		{
			LSL.HouseHandler.CurrentHouse.Cocain += LSL.putCocainOzStash.Value * 28;
			LsFunctions.pCocaineOunce -= LSL.putCocainOzStash.Value;
		}
		else if (item == LSL.putArmourStash)
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
		else if (item == LSL.putWeaponStash && LSL.putWeaponStash.Items.Count > 0)
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
		LSL.HouseHandler.SaveHouse(LSL.HouseHandler.CurrentHouse);
	}
}
