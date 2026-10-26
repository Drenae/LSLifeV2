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
	public static void WArmour_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (LSL.playerArmours <= 0)
		{
			return;
		}
		LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
		PlayerDealer currentDealer = LSL.DealerHandler.currentDealer;
		if (currentDealer != null)
		{
			Ped ped = currentDealer.Ped;
			if (((ped != null) ? new int?(ped.Armor) : ((int?)null)) < 200)
			{
				LSL.DealerHandler.currentDealer.Ped.Armor = 200;
				LSL.DealerHandler.currentDealer.dArmour = LSL.DealerHandler.currentDealer.Ped.Armor;
				LSL.playerArmours--;
				if (LSL.playerArmours == 0)
				{
					selectedItem.Text = "Plus de gilet disponible";
				}
				LsFunctions.PlayTheSound();
				LsFunctions.UpdateInventory();
				LsFunctions.SetWorkerMenuHealth(LSL.DealerHandler.currentDealer.Ped);
				return;
			}
		}
		UI.ShowSubtitle("J'ai déjà assez de protection.", 4000);
	}

		public static void WWeapons_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		GiveWeaponToWorker(LSL.wWeapons.Index);
	}

	public static void GiveWeaponToWorker(int index)
	{
		WorkerWeaponTransfer.GiveWeaponToWorker(index);
	}

	internal static void WOrder_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (LSL.DealerHandler.currentDealer != null)
		{
			Enum.TryParse<Enums.PdStates>(LSL.wOrder.Items[LSL.wOrder.Index].ToString(), out LSL.DealerHandler.currentDealer.NewState);
			LSL.wOrder.Items = LSL.DealerHandler.currentDealer.Orders();
			LSL.wOrder.Index = 0;
		}
	}

	public static void WConfirm_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int i = 0;
		using (List<PlayerDealer>.Enumerator enumerator = LSL.DealerHandler.dealers.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != LSL.DealerHandler.currentDealer)
			{
				i++;
			}
		}
		bool change = false;
		if (LSL.wWeed.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Weed"] += LSL.wWeed.Value;
			LSL.PlayerInventory["Weed"] -= LSL.wWeed.Value;
			LSL.wWeed.Value = 0;
			LSL.wWeed.Text = "Cannabis " + LsFunctions.GramsToOz(LSL.wWeed.Value);
			change = true;
		}
		if (LSL.wCrack.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Crack"] += LSL.wCrack.Value;
			LSL.PlayerInventory["Crack"] -= LSL.wCrack.Value;
			LSL.wCrack.Value = 0;
			LSL.wCrack.Text = "Crack " + LsFunctions.GramsToOz(LSL.wCrack.Value);
			change = true;
		}
		if (LSL.wCocaine.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Cocaine"] += LSL.wCocaine.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.wCocaine.Value;
			LSL.wCocaine.Value = 0;
			LSL.wCocaine.Text = "Cocaïne " + LsFunctions.GramsToOz(LSL.wCocaine.Value);
			change = true;
		}
		if (change)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			LsFunctions.UpdateInventory();
			LSL.DealerHandler.dealers[i].SetDrugDesc();
			LSL.DealerHandler.needsSave = true;
		}
	}

	public static void WCollect_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int i = 0;
		using (List<PlayerDealer>.Enumerator enumerator = LSL.DealerHandler.dealers.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != LSL.DealerHandler.currentDealer)
			{
				i++;
			}
		}
		if (LSL.DealerHandler.dealers[i].Money >= 100)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			LSL.DealerHandler.dealers[i].GiveMoneyToPlayer();
			LSL.DealerHandler.dealers[i].SetDrugDesc();
			LSL.WorkerMainMenu.CurrentSelection = 3;
		}
	}

	public static void WCocaine_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.wCocaine.Text = "Cocaïne " + LsFunctions.GramsToOz(sender.Value);
	}

	public static void WCrack_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.wCrack.Text = "Crack " + LsFunctions.GramsToOz(sender.Value);
	}

	public static void WWeed_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.wWeed.Text = "Cannabis " + LsFunctions.GramsToOz(sender.Value);
	}
}
