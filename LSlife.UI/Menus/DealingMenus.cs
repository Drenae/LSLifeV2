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
		if (item == LSL.WeedBuy && LSL.dealer1Weed > 0 && LSL.bushWeed.Value > 0)
		{
			int total = LSL.bushWeed.Value * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price));
			LSL.NewOrder.OrderWeed(LSL.bushWeed.Value, total);
			LSL.bushWeed.Maximum -= LSL.bushWeed.Value;
			LSL.dealer1Weed = LSL.bushWeed.Maximum;
			LSL.WeedBuy.Text = "Commander ~g~" + LSL.bushWeed.Value + " ~s~pour " + LsFunctions.IntToMoney(total) + " $";
			LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
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
			LSL.crack.Maximum -= LSL.crack.Value;
			LSL.dealer1Crack = LSL.crack.Maximum;
			LSL.CrackBuy.Text = "Commander ~g~" + LSL.crack.Value + " ~w~pour " + LsFunctions.IntToMoney(total) + " $";
			LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
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
			LSL.cocaine.Maximum -= LSL.cocaine.Value;
			LSL.dealer1Cocain = LSL.cocaine.Maximum;
			LSL.CocainBuy.Text = "Commander ~g~" + LSL.cocaine.Value + " ~w~pour " + LsFunctions.IntToMoney(total) + " $";
			LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
			LsFunctions.DbugString("Cocaine Order changed");
			LsFunctions.UpdateOrderMenuDesc();
		}
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
		if (LSL.customerSellTo == null) return false;
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		if (sale)
		{
			if (LSL.PlayerInventory[LSL.customerSellTo.Drug.Name] < LSL.customerSellTo.amount)
			{
				LSL.sell.Enabled = false;
				LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
				return false;
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
				LSL.lastTimeDealDone = Game.GameTime;
			}
			else
			{
				LsFunctions.MakeStreetSale(LSL.customerSellTo, inCar: false);
				LSL.lastTimeDealDone = Game.GameTime;
			}
		}
		else
		{
			foreach (Drug d2 in LSL.areas[LSL.areaIndex].Drugs)
			{
				if (d2.Name == LSL.customerSellTo.Drug.Name)
				{
					d2.Supplied = false;
				}
			}
			if (LSL.customerSellTo.FarAway)
			{
				ShotHandler.RemoveShot(ShotHandler.shots.First((Shot s) => s.customer == LSL.customerSellTo));
				LSL.lastTimeDealDone = Game.GameTime;
				OldCustomerHandler.AddCustomer(LSL.customerSellTo.Ped, LSL.customerSellTo.Vehicle, LSL.HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(LSL.customerSellTo.Drug.Name, LSL.customerSellTo.amount));
				return true;
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
		return true;
	}

}
