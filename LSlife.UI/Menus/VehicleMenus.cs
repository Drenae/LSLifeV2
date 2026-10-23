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
	public static void onPutVehicleMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		//IL_0c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c95: Invalid comparison between Unknown and I4
		//IL_0cf1: Unknown result type (might be due to invalid IL or missing references)
		if (item == LSL.putWeedVeh)
		{
			LSL.pStashVehicle.drugCarWeed += LSL.putWeedVeh.Value;
			LSL.PlayerInventory["Weed"] -= LSL.putWeedVeh.Value;
			LSL.putWeedVeh.Maximum -= LSL.putWeedVeh.Value;
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
		if (item == LSL.putWeedOzVeh)
		{
			LSL.pStashVehicle.drugCarWeedOz += LSL.putWeedOzVeh.Value;
			LsFunctions.pWeedOunce -= LSL.putWeedOzVeh.Value;
			LSL.putWeedOzVeh.Maximum -= LSL.putWeedOzVeh.Value;
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
		if (item == LSL.putCrackVeh)
		{
			LSL.pStashVehicle.drugCarCrack += LSL.putCrackVeh.Value;
			LSL.PlayerInventory["Crack"] -= LSL.putCrackVeh.Value;
			LSL.putCrackVeh.Maximum -= LSL.putCrackVeh.Value;
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
		if (item == LSL.putCrackOzVeh)
		{
			LSL.pStashVehicle.drugCarCrackOz += LSL.putCrackOzVeh.Value;
			LsFunctions.pCrackOunce -= LSL.putCrackOzVeh.Value;
			LSL.putCrackOzVeh.Maximum -= LSL.putCrackOzVeh.Value;
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
		if (item == LSL.putCocainVeh)
		{
			LSL.pStashVehicle.drugCarCocain += LSL.putCocainVeh.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.putCocainVeh.Value;
			LSL.putCocainVeh.Maximum -= LSL.putCocainVeh.Value;
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
		if (item == LSL.putCocainOzVeh)
		{
			LSL.pStashVehicle.drugCarCocainOz += LSL.putCocainOzVeh.Value;
			LsFunctions.pCocaineOunce -= LSL.putCocainOzVeh.Value;
			LSL.putCocainOzVeh.Maximum -= LSL.putCocainOzVeh.Value;
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
		if (item == LSL.putMoneyVeh)
		{
			LSL.pStashVehicle.drugCarMoney += LSL.putMoneyVeh.Value;
			Player player = Game.Player;
			player.Money -= LSL.putMoneyVeh.Value;
			LSL.putMoneyVeh.Maximum -= LSL.putMoneyVeh.Value;
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
		if (item == LSL.putWeaponVeh && LSL.putWeaponVeh.Items.Count > 0)
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
		if (item == LSL.takeWeedVeh)
		{
			LSL.PlayerInventory["Weed"] += LSL.takeWeedVeh.Value;
			LSL.pStashVehicle.drugCarWeed -= LSL.takeWeedVeh.Value;
			LSL.takeWeedVeh.Maximum -= LSL.takeWeedVeh.Value;
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
		if (item == LSL.takeWeedOzVeh)
		{
			LsFunctions.pWeedOunce += LSL.takeWeedOzVeh.Value;
			LSL.pStashVehicle.drugCarWeedOz -= LSL.takeWeedOzVeh.Value;
			LSL.takeWeedOzVeh.Maximum -= LSL.takeWeedOzVeh.Value;
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
		if (item == LSL.takeCrackVeh)
		{
			LSL.PlayerInventory["Crack"] += LSL.takeCrackVeh.Value;
			LSL.pStashVehicle.drugCarCrack -= LSL.takeCrackVeh.Value;
			LSL.takeCrackVeh.Maximum -= LSL.takeCrackVeh.Value;
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
		if (item == LSL.takeCrackOzVeh)
		{
			LsFunctions.pCrackOunce += LSL.takeCrackOzVeh.Value;
			LSL.pStashVehicle.drugCarCrackOz -= LSL.takeCrackOzVeh.Value;
			LSL.takeCrackOzVeh.Maximum -= LSL.takeCrackOzVeh.Value;
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
		if (item == LSL.takeCocainVeh)
		{
			LSL.PlayerInventory["Cocaine"] += LSL.takeCocainVeh.Value;
			LSL.pStashVehicle.drugCarCocain -= LSL.takeCocainVeh.Value;
			LSL.takeCocainVeh.Maximum -= LSL.takeCocainVeh.Value;
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
		if (item == LSL.takeCocainOzVeh)
		{
			LsFunctions.pCocaineOunce += LSL.takeCocainOzVeh.Value;
			LSL.pStashVehicle.drugCarCocainOz -= LSL.takeCocainOzVeh.Value;
			LSL.takeCocainOzVeh.Maximum -= LSL.takeCocainOzVeh.Value;
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
		if (item == LSL.takeMoneyVeh)
		{
			Player player = Game.Player;
			player.Money += LSL.takeMoneyVeh.Value;
			LSL.pStashVehicle.drugCarMoney -= LSL.takeMoneyVeh.Value;
			LSL.takeMoneyVeh.Maximum -= LSL.takeMoneyVeh.Value;
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
		if (item == LSL.takeWeaponVeh && LSL.takeWeaponVeh.Items.Count > 0)
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
}
