using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Control = GTA.Control;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.Gangs;
using LSlife.HeatBars;
using LSlife.OldCustomers;
using LSlife.Shots;
using NativeUI;


namespace LSlife;

public partial class LSL
{
	private void DrugStockMenu_OnMenuOpen(UIMenu sender)
	{
		if (sender == weedMenu)
		{
			((UIText)weedMenu.Subtitle).Caption = "Stock disponible : " + dealer1Weed + " oz";
		}
		else if (sender == crackMenu)
		{
			((UIText)crackMenu.Subtitle).Caption = "Stock disponible : " + dealer1Crack + " oz";
		}
		else if (sender == cocainMenu)
		{
			((UIText)cocainMenu.Subtitle).Caption = "Stock disponible : " + dealer1Cocain + " oz";
		}
	}

	private void OrderDrugs_OnMenuOpen(UIMenu sender)
	{
		if (dealer1ReloadOrder)
		{
			Enum.TryParse<Enums.DayGTA>(dealer1ReloadDay.ToString(), out var mDay);
			LsFunctions.TextMsg("ZEE", "Réassort", "La commande devrait arriver " + mDay.ToString() + " à " + dealer1ReloadHour + ":00");
		}
		if (!NewOrder.Placed)
		{
			PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(NewOrder.Bill) + " $";
			LsFunctions.UpdateOrderMenuDesc();
		}
	}

	private void DMultiplier_OnListChanged(UIMenuListItem sender, int newIndex)
	{
		int mlti = (int)sender.Items[sender.Index];
		bushWeed.Multiplier = mlti;
		crack.Multiplier = mlti;
		cocaine.Multiplier = mlti;
	}

	private void WMultiplier_OnListChanged(UIMenuListItem sender, int newIndex)
	{
		wWeed.Multiplier = multiplier[wMultiplier.Index];
		wCrack.Multiplier = multiplier[wMultiplier.Index];
		wCocaine.Multiplier = multiplier[wMultiplier.Index];
	}

	private void TakeArmourVeh_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int amount = takeArmourVeh.Index;
		int max = 5;
		if (player.Character.Armor < 100)
		{
			max = 6;
		}
		if (amount <= 0 || playerArmours + amount > max)
		{
			return;
		}
		if (player.Character.Armor < 100)
		{
			player.Character.Armor = 100;
			amount--;
		}
		playerArmours += amount;
		pStashVehicle.drugCarArmour -= amount;
		foreach (XElement ele in LSLifeSave.Descendants())
		{
			if (ele.Name == "drugCarStash")
			{
				foreach (XElement el in ele.Elements())
				{
					if (el.Name == "armour")
					{
						el.Value = pStashVehicle.drugCarArmour.ToString();
						break;
					}
				}
			}
			if (!(ele.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement el2 in ele.Elements())
			{
				if (el2.Name == "armour")
				{
					el2.Value = playerArmours.ToString();
					break;
				}
			}
		}
		LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		putArmourVeh.Items = new List<object>();
		for (int t = 0; t < playerArmours + 1; t++)
		{
			putArmourVeh.Items.Add(t);
		}
		takeArmourVeh.Items = new List<object>();
		for (int i = 0; i < pStashVehicle.drugCarArmour + 1; i++)
		{
			takeArmourVeh.Items.Add(i);
		}
	}

	private void PutArmourVeh_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int amount = putArmourVeh.Index;
		if (amount <= 0 || playerArmours <= 0)
		{
			return;
		}
		playerArmours -= amount;
		pStashVehicle.drugCarArmour += amount;
		foreach (XElement ele in LSLifeSave.Descendants())
		{
			if (ele.Name == "drugCarStash")
			{
				foreach (XElement el in ele.Elements())
				{
					if (el.Name == "armour")
					{
						el.Value = pStashVehicle.drugCarArmour.ToString();
						break;
					}
				}
			}
			if (!(ele.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement el2 in ele.Elements())
			{
				if (el2.Name == "armour")
				{
					el2.Value = playerArmours.ToString();
					break;
				}
			}
		}
		LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		putArmourVeh.Index = 0;
		putArmourVeh.Items = new List<object>();
		for (int t = 0; t < playerArmours + 1; t++)
		{
			putArmourVeh.Items.Add(t);
		}
		takeArmourVeh.Items = new List<object>();
		for (int i = 0; i < pStashVehicle.drugCarArmour + 1; i++)
		{
			takeArmourVeh.Items.Add(i);
		}
	}
}
