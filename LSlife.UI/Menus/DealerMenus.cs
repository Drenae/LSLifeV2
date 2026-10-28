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
	public static void OnZeeMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.buyWeapons) { CallWeaponDealer(); if (sender != null) sender.Visible = false; }
		else if (item == LSL.buyLift) { CallLift(); if (sender != null) sender.Visible = false; }
		else if (item == LSL.payDebt)
		{
			PayZeeDebt();
			LSL.payDebt.Text = "Rembourser la dette " + LsFunctions.IntToMoney(LSL.dealer1Debt) + " $";
		}
	}

	// Actions metier accessibles sans creer ni ouvrir de menu NativeUI.
	internal static void CallWeaponDealer() => ZeeContactActions.CallWeaponDealer();

	internal static void CallLift() => ZeeContactActions.CallLift();

	internal static void PayZeeDebt()
	{
		ZeeContactActions.PayZeeDebt();
	}

	public static void OnWepMenuSelect(UIMenu sender, UIMenuItem item, int index)
	{
		if (item == LSL.melee) WeaponDealerActions.Execute(WeaponDealerAction.Melee, LSL.melee.Index, LSL.pistol.Index);
		else if (item == LSL.pistol) WeaponDealerActions.Execute(WeaponDealerAction.Pistol, LSL.melee.Index, LSL.pistol.Index);
		else if (item == LSL.armour) WeaponDealerActions.Execute(WeaponDealerAction.Armour, LSL.melee.Index, LSL.pistol.Index);
		else if (item == LSL.dismiss)
		{
			LSL.wepMenu.Visible = false;
			WeaponDealerActions.Execute(WeaponDealerAction.Dismiss, LSL.melee.Index, LSL.pistol.Index);
		}
	}

	public static void HireDealerFollowSelect(UIMenu sender, UIMenuItem selectedItem)
	{
		LSL.hireDealerMainMenu.Visible = false;
		CancelDealerRecruitment();
	}

	internal static void CancelDealerRecruitment()
	{
		DealerRecruitmentActions.CancelDealerRecruitment();
	}

	public static void HireDealerOfferSelect(UIMenu sender, UIMenuItem selectedItem)
	{
		LSL.hireDealerMainMenu.Visible = false;
		SubmitDealerOffer(LSL.hireDealerOfferAmount.Value);
	}

	internal static void SubmitDealerOffer(int amount)
	{
		DealerRecruitmentActions.SubmitDealerOffer(amount);
	}

	public static void HireDealerOfferAmount_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.hireDealerMakeOffer.Text = "Offre : " + LsFunctions.IntToMoney(sender.Value) + " $";
	}
}
