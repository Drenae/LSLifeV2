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

    // Notification de réassort partagée avec le menu Zee personnalisé.
    internal static void NotifyZeeRestock()
    {
        if (!dealer1ReloadOrder) return;
        Enum.TryParse<Enums.DayGTA>(dealer1ReloadDay.ToString(), out var day);
        LsFunctions.TextMsg("ZEE", "Réassort",
            "La commande devrait arriver " + day + " à " + dealer1ReloadHour + ":00");
    }

	private void OrderDrugs_OnMenuOpen(UIMenu sender)
	{
		NotifyZeeRestock();
		if (!NewOrder.Placed)
		{
			PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(NewOrder.Bill) + " $";
			PlaceOrder.Description = LsFunctions.OrderMenuDesc();
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
		if (VehicleArmourTransfer.Transfer(false, takeArmourVeh.Index))
			RefreshLegacyVehicleArmourChoices();
	}

	private void PutArmourVeh_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (VehicleArmourTransfer.Transfer(true, putArmourVeh.Index))
			RefreshLegacyVehicleArmourChoices();
	}

	// Les anciennes listes ne sont actualisees que pour leur propre interface.
	private void RefreshLegacyVehicleArmourChoices()
	{
		putArmourVeh.Items = new List<object>();
		for (int i = 0; i <= playerArmours; i++) putArmourVeh.Items.Add(i);
		putArmourVeh.Index = 0;
		takeArmourVeh.Items = new List<object>();
		for (int i = 0; i <= pStashVehicle.drugCarArmour; i++) takeArmourVeh.Items.Add(i);
		takeArmourVeh.Index = 0;
	}

}
