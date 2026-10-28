using System;
using System.Collections.Generic;
using System.Drawing;
using NativeUI;

namespace LSlife;

// Couche de compatibilite temporaire : a supprimer lorsque les derniers consommateurs
// des anciens champs et evenements auront migre vers les menus personnalises.
public partial class LSL
{
	public static MenuPool LsMenuPool;

	public static UIMenu hireDealerMainMenu;

	public static UIMenuSliderItem hireDealerOfferAmount;

	public static UIMenuItem hireDealerMakeOffer;

	public static UIMenuItem hireDealerFollowButton;

	public static UIMenu WorkerMainMenu;

	public static UIMenuSliderItem wWeed;

	public static UIMenuSliderItem wCrack;

	public static UIMenuSliderItem wCocaine;

	public static UIMenuListItem wMultiplier;

	public static UIMenuItem wConfirm;

	public static UIMenuItem wCollect;

	public static UIMenuListItem wOrder;

	public static UIMenuItem wArmour;

	public static UIMenuListItem wWeapons;

	public static UIMenu zeeMenu;

	public static UIMenu startDealing;

	public static UIMenuCheckboxItem sellWeed;

	public static UIMenuCheckboxItem sellCrack;

	public static UIMenuCheckboxItem sellCocaine;

	public static UIMenuCheckboxItem allowDeliveries;

	public static bool deliveriesEnabled = true;

	public static UIMenuItem StartDeal;

	public static UIMenu Options;

	public static UIMenuSliderItem InventoryPosX;

	public static UIMenuSliderItem InventoryPosY;

	public static UIMenu OrderDrugs;

	private UIMenuListItem OrderMultipliers;

	public static UIMenu weedMenu;

	public static UIMenuSliderItem bushWeed;

	public static UIMenu crackMenu;

	public static UIMenuSliderItem crack;

	public static UIMenu cocainMenu;

	public static UIMenuSliderItem cocaine;

	public static UIMenuItem WeedBuy;

	public static UIMenuItem CrackBuy;

	public static UIMenuItem CocainBuy;

	public static UIMenuItem PlaceOrder;

	public static UIMenu OrderSummaryMenu;

	public static UIMenuItem OrderSummaryWeed;

	public static UIMenuItem OrderSummaryCrack;

	public static UIMenuItem OrderSummaryCocaine;

	public static UIMenuItem OrderSummaryBill;

	public static UIMenuItem OrderSummaryDebt;

	public static UIMenuItem OrderSummaryCredit;

	public static UIMenuItem buyWeapons;

	public static UIMenuItem buyLift;

	public static UIMenuItem payDebt;

	public static UIMenu wepMenu;

	public static UIMenuListItem melee;

	public static List<dynamic> meleeWeps = new List<object> { "Bat", "Knife", "Dagger" };

	public static List<int> meleePrice = new List<int> { 5, 100, 2000 };

	public static UIMenuListItem pistol;

	public static List<dynamic> pistolWeps = new List<object> { "Pistol", "Pistol50", "HeavyPistol", "Ammo" };

	public static List<int> pistolPrice = new List<int> { 350, 3750, 4000, 50 };

	public static UIMenuListItem shotgun;

	public static List<dynamic> shotgunWeps = new List<object> { "SawnOffShotgun", "PumpShotgun" };

	public static UIMenuListItem assaultRifle;

	public static List<dynamic> assaultRifleWeps = new List<object> { "AssaultRifle", "CarbineRifle" };

	public static UIMenuListItem smg;

	public static List<dynamic> smgWeps = new List<object> { "MicroSMG", "SMG" };

	public static UIMenuListItem mg;

	public static List<dynamic> mgWeps = new List<object> { "MG" };

	public static UIMenuListItem sniper;

	public static List<dynamic> sniperWeps = new List<object> { "SniperRifle" };

	public static UIMenuListItem heavy;

	public static List<dynamic> heavyWeps = new List<object> { "RPG" };

	public static UIMenuListItem thrown;

	public static List<dynamic> thrownWeps = new List<object> { "Grenade" };

	public static UIMenuItem armour;

	public static UIMenuItem dismiss;

	public static UIMenu stashMainMenu;

	public static UIMenu stashTakeMenu;

	public static UIMenu stashPutMenu;

	public static UIMenuSliderItem putWeedStash;

	public static UIMenuSliderItem takeWeedStash;

	public static UIMenuSliderItem putCrackStash;

	public static UIMenuSliderItem takeCrackStash;

	public static UIMenuSliderItem putCocainStash;

	public static UIMenuSliderItem takeCocainStash;

	public static UIMenuSliderItem putWeedOzStash;

	public static UIMenuSliderItem takeWeedOzStash;

	public static UIMenuSliderItem putCrackOzStash;

	public static UIMenuSliderItem takeCrackOzStash;

	public static UIMenuSliderItem putCocainOzStash;

	public static UIMenuSliderItem takeCocainOzStash;

	public static UIMenuSliderItem putMoneyStash;

	public static UIMenuSliderItem takeMoneyStash;

	public static UIMenuListItem putArmourStash;

	public static UIMenuListItem takeArmourStash;

	public static UIMenuListItem putWeaponStash;

	public static UIMenuListItem takeWeaponStash;

	public static UIMenuListItem stashPutMultiplier;

	public static UIMenuListItem stashTakeMultiplier;

	public static UIMenu vehStashMainMenu;

	public static UIMenu vehStashTakeMenu;

	public static UIMenu vehStashPutMenu;

	public static UIMenuSliderItem putWeedVeh;

	public static UIMenuSliderItem takeWeedVeh;

	public static UIMenuSliderItem putCrackVeh;

	public static UIMenuSliderItem takeCrackVeh;

	public static UIMenuSliderItem putCocainVeh;

	public static UIMenuSliderItem takeCocainVeh;

	public static UIMenuSliderItem putWeedOzVeh;

	public static UIMenuSliderItem takeWeedOzVeh;

	public static UIMenuSliderItem putCrackOzVeh;

	public static UIMenuSliderItem takeCrackOzVeh;

	public static UIMenuSliderItem putCocainOzVeh;

	public static UIMenuSliderItem takeCocainOzVeh;

	public static UIMenuSliderItem putMoneyVeh;

	public static UIMenuSliderItem takeMoneyVeh;

	public static UIMenuListItem putArmourVeh;

	public static UIMenuListItem takeArmourVeh;

	public static UIMenuListItem putWeaponVeh;

	public static UIMenuListItem takeWeaponVeh;

	public static UIMenuListItem vehPutMultiplier;

	public static UIMenuListItem vehTakeMultiplier;

	public static readonly List<dynamic> multiplier = new List<object> { 1, 10, 28, 100, 280, 1000, 2800, 10000, 100000 };

	public static UIMenu customerMainMenu;

	public static UIMenuItem sell;

	public static UIMenuItem turnAway;



}
