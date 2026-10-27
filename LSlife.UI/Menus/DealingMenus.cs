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
	public static void OnstartDealSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item != LSL.StartDeal) return;
		StartDealing(LSL.sellWeed.Checked, LSL.sellCrack.Checked,
			LSL.sellCocaine.Checked, LSL.allowDeliveries.Checked);
		if (sender != null && LSL.isDealing) sender.Visible = false;
	}

	// Traitement metier independant de NativeUI, partage avec CustomZeeMenu.
	internal static void StartDealing(bool weed, bool crack, bool cocaine, bool deliveries)
	{
		StreetDealingActions.StartDealing(weed, crack, cocaine, deliveries);
	}

	public static void onWeedMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.WeedBuy) ZeeDrugOrderActions.AddWeed();
	}

	public static void onCrackMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.CrackBuy) ZeeDrugOrderActions.AddCrack();
	}

	public static void onCocainMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.CocainBuy) ZeeDrugOrderActions.AddCocaine();
	}

	public static void OnOrderDrugsMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item != LSL.PlaceOrder) return;
		if (PlaceDrugOrder()) LSL.LsMenuPool.CloseAllMenus();
	}

	// Retour explicite pour que chaque interface gere sa propre fermeture.
	internal static bool PlaceDrugOrder()
	{
		return StreetDealingActions.PlaceDrugOrder();
	}

	public static void onCustomerMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item != LSL.sell && item != LSL.turnAway) return;
		if (HandleCustomerTransaction(item == LSL.sell))
			LSL.customerMainMenu.Visible = !LSL.customerMainMenu.Visible;
	}

	// Same gameplay, independent of NativeUI item and menu visibility.
	internal static bool HandleCustomerTransaction(bool sale)
	{
		if (sale && LSL.customerSellTo != null &&
			LSL.PlayerInventory[LSL.customerSellTo.Drug.Name] < LSL.customerSellTo.amount)
			LSL.sell.Enabled = false;
		return CustomerTransactionActions.HandleCustomerTransaction(sale);
	}

}
