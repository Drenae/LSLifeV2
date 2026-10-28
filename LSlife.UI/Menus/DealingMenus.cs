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
		if (item != LSL.WeedBuy || !ZeeDrugOrderActions.AddWeed(LSL.bushWeed.Value)) return;
		LSL.bushWeed.Maximum = LSL.dealer1Weed;
		LSL.WeedBuy.Text = "Commander ~g~" + LSL.bushWeed.Value + " ~s~pour " +
			LsFunctions.IntToMoney(LSL.bushWeed.Value * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price))) + " $";
		RefreshLegacyOrderMenu();
	}

	public static void onCrackMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item != LSL.CrackBuy || !ZeeDrugOrderActions.AddCrack(LSL.crack.Value)) return;
		LSL.crack.Maximum = LSL.dealer1Crack;
		LSL.CrackBuy.Text = "Commander ~g~" + LSL.crack.Value + " ~w~pour " +
			LsFunctions.IntToMoney(LSL.crack.Value * (LSL.crack1Price - LsFunctions.Discount(LSL.crack1Price))) + " $";
		RefreshLegacyOrderMenu();
	}

	public static void onCocainMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item != LSL.CocainBuy || !ZeeDrugOrderActions.AddCocaine(LSL.cocaine.Value)) return;
		LSL.cocaine.Maximum = LSL.dealer1Cocain;
		LSL.CocainBuy.Text = "Commander ~g~" + LSL.cocaine.Value + " ~w~pour " +
			LsFunctions.IntToMoney(LSL.cocaine.Value * (LSL.cocain1Price - LsFunctions.Discount(LSL.cocain1Price))) + " $";
		RefreshLegacyOrderMenu();
	}

	internal static void RefreshLegacyOrderMenu()
	{
		LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
		LSL.PlaceOrder.Description = LsFunctions.OrderMenuDesc();
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
		return CustomerTransactionActions.HandleCustomerTransaction(sale);
	}

}
