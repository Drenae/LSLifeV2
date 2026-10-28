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
	public static void onPlayerMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Invalid comparison between Unknown and I4
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		if (item == LSL.takeWeedStash)
		{
			LSL.PlayerInventory["Weed"] += LSL.takeWeedStash.Value;
			LSL.HouseHandler.CurrentHouse.Weed -= LSL.takeWeedStash.Value;
		}
		else if (item == LSL.takeCrackStash)
		{
			LSL.PlayerInventory["Crack"] += LSL.takeCrackStash.Value;
			LSL.HouseHandler.CurrentHouse.Crack -= LSL.takeCrackStash.Value;
		}
		else if (item == LSL.takeCocainStash)
		{
			LSL.PlayerInventory["Cocaine"] += LSL.takeCocainStash.Value;
			LSL.HouseHandler.CurrentHouse.Cocain -= LSL.takeCocainStash.Value;
		}
		else if (item == LSL.takeMoneyStash)
		{
			Player player = Game.Player;
			player.Money += LSL.takeMoneyStash.Value;
			LSL.HouseHandler.CurrentHouse.Money -= LSL.takeMoneyStash.Value;
		}
		else if (item == LSL.takeArmourStash)
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
		else if (item == LSL.takeWeaponStash && LSL.takeWeaponStash.Items.Count > 0)
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
		LSL.HouseHandler.SaveHouse(LSL.HouseHandler.CurrentHouse);
	}
}
