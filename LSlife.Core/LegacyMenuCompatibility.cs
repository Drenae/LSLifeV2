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


    private void InitializeLegacyMenus()
    {
		LsMenuPool = new MenuPool();
		Color.FromArgb(114, 204, 114);
		zeeMenu = new UIMenu("Zee", "Tu veux quoi ?");
		LsMenuPool.Add(zeeMenu);
		startDealing = LsMenuPool.AddSubMenu(zeeMenu, "Dealer");
		sellWeed = new UIMenuCheckboxItem("Vendre du cannabis", check: true);
		sellCrack = new UIMenuCheckboxItem("Vendre du crack", check: true);
		sellCocaine = new UIMenuCheckboxItem("Vendre de la cocaïne", check: true);
		allowDeliveries = new UIMenuCheckboxItem("Livraisons", check: true);
		StartDeal = new UIMenuItem("Commencer à dealer");
		startDealing.AddItem(sellWeed);
		startDealing.AddItem(sellCrack);
		startDealing.AddItem(sellCocaine);
		startDealing.AddItem(allowDeliveries);
		startDealing.AddItem(StartDeal);
		startDealing.OnItemSelect += LsMenus.OnstartDealSelect;
		OrderDrugs = LsMenuPool.AddSubMenu(zeeMenu, "Commander de la drogue");
		OrderDrugs.BannerSprite.Color = Color.Black;
		OrderDrugs.Title.DropShadow = true;
		OrderDrugs.Title.Outline = true;
		((GTA.UIText)OrderDrugs.Title).Shadow = true;
		OrderDrugs.OnMenuOpen += OrderDrugs_OnMenuOpen;
		buyWeapons = new UIMenuItem("Acheter des armes");
		zeeMenu.AddItem(buyWeapons);
		buyLift = new UIMenuItem("Demander à être déposé");
		zeeMenu.AddItem(buyLift);
		payDebt = new UIMenuItem("Rembourser la dette");
		zeeMenu.AddItem(payDebt);
		zeeMenu.OnItemSelect += LsMenus.OnZeeMenuSelect;
		hireDealerMainMenu = new UIMenu("Recruter un dealer", "Choisis une option");
		LsMenuPool.Add(hireDealerMainMenu);
		hireDealerOfferAmount = new UIMenuSliderItem("Montant proposé");
		hireDealerOfferAmount.Maximum = player.Money;
		hireDealerOfferAmount.Multiplier = 1000;
		hireDealerMakeOffer = new UIMenuItem("Faire une offre");
		hireDealerFollowButton = new UIMenuItem("Annuler l'offre");
		hireDealerMainMenu.AddItem(hireDealerOfferAmount);
		hireDealerMainMenu.AddItem(hireDealerMakeOffer);
		hireDealerMainMenu.AddItem(hireDealerFollowButton);
		hireDealerOfferAmount.OnSliderChanged += LsMenus.HireDealerOfferAmount_OnSliderChanged;
		hireDealerMakeOffer.Activated += LsMenus.HireDealerOfferSelect;
		hireDealerFollowButton.Activated += LsMenus.HireDealerFollowSelect;
		WorkerMainMenu = new UIMenu("Employé", "Donner du stock au dealer");
		LsMenuPool.Add(WorkerMainMenu);
		wWeed = new UIMenuSliderItem("Cannabis " + LsFunctions.GramsToOz(0));
		wWeed.Maximum = PlayerInventory["Weed"];
		wCrack = new UIMenuSliderItem("Crack " + LsFunctions.GramsToOz(0));
		wCrack.Maximum = PlayerInventory["Crack"];
		wCocaine = new UIMenuSliderItem("Cocaïne " + LsFunctions.GramsToOz(0));
		wCocaine.Maximum = PlayerInventory["Cocaine"];
		wMultiplier = new UIMenuListItem("Multiplicateur", multiplier, 0);
		wConfirm = new UIMenuItem("Confirmer");
		wCollect = new UIMenuItem("Récupérer");
		List<object> orders = new List<object> { "Rien" };
		wOrder = new UIMenuListItem("Commandes", orders, 0);
		wArmour = new UIMenuItem("Donner un gilet");
		wWeapons = new UIMenuListItem("Donner une arme", playerWeps, 0);
		WorkerMainMenu.AddItem(wWeed);
		WorkerMainMenu.AddItem(wCrack);
		WorkerMainMenu.AddItem(wCocaine);
		WorkerMainMenu.AddItem(wMultiplier);
		WorkerMainMenu.AddItem(wConfirm);
		WorkerMainMenu.AddItem(wCollect);
		WorkerMainMenu.AddItem(wOrder);
		WorkerMainMenu.AddItem(wArmour);
		WorkerMainMenu.AddItem(wWeapons);
		wWeed.OnSliderChanged += LsMenus.WWeed_OnSliderChanged;
		wCrack.OnSliderChanged += LsMenus.WCrack_OnSliderChanged;
		wCocaine.OnSliderChanged += LsMenus.WCocaine_OnSliderChanged;
		wMultiplier.OnListChanged += WMultiplier_OnListChanged;
		wConfirm.Activated += LsMenus.WConfirm_Activated;
		wCollect.Activated += LsMenus.WCollect_Activated;
		wOrder.Activated += LsMenus.WOrder_Activated;
		wArmour.Activated += LsMenus.WArmour_Activated;
		wWeapons.Activated += LsMenus.WWeapons_Activated;
		int mWprice = meleePrice[0] - LsFunctions.Discount(meleePrice[0]);
		int pWprice = pistolPrice[0] - LsFunctions.Discount(pistolPrice[0]);
		wepMenu = new UIMenu("Armes", "Tu veux quoi ?");
		LsMenuPool.Add(wepMenu);
		melee = new UIMenuListItem("Arme blanche " + LsFunctions.IntToMoney(mWprice) + " $", meleeWeps, 0);
		wepMenu.AddItem(melee);
		pistol = new UIMenuListItem("Pistolet " + LsFunctions.IntToMoney(pWprice) + " $", pistolWeps, 0);
		wepMenu.AddItem(pistol);
		armour = new UIMenuItem("Gilet " + LsFunctions.IntToMoney(250 - LsFunctions.Discount(250)) + " $");
		wepMenu.AddItem(armour);
		dismiss = new UIMenuItem("Renvoyer");
		wepMenu.AddItem(dismiss);
		wepMenu.OnItemSelect += LsMenus.OnWepMenuSelect;
		melee.OnListChanged += LsMenus.onListChange;
		pistol.OnListChanged += LsMenus.onListChange;
		OrderMultipliers = new UIMenuListItem("Multiplicateur", multiplier, 0);
		OrderMultipliers.OnListChanged += DMultiplier_OnListChanged;
		weedMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Cannabis");
		bushWeed = new UIMenuSliderItem("Cannabis " + LsFunctions.IntToMoney(weed1Price - LsFunctions.Discount(weed1Price)) + " $/oz");
		weedMenu.AddItem(bushWeed);
		bushWeed.Maximum = dealer1Weed;
		bushWeed.Multiplier = 1;
		weedMenu.AddItem(OrderMultipliers);
		WeedBuy = new UIMenuItem("Commander : " + bushWeed.Value);
		weedMenu.AddItem(WeedBuy);
		bushWeed.OnSliderChanged += LsMenus.onSliderChange;
		weedMenu.OnItemSelect += LsMenus.onWeedMenuSelect;
		weedMenu.OnMenuOpen += DrugStockMenu_OnMenuOpen;
		crackMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Crack");
		crack = new UIMenuSliderItem("Crack " + LsFunctions.IntToMoney(crack1Price - LsFunctions.Discount(crack1Price)) + " $/oz");
		crackMenu.AddItem(crack);
		crack.Maximum = dealer1Crack;
		crack.Multiplier = 1;
		crackMenu.AddItem(OrderMultipliers);
		CrackBuy = new UIMenuItem("Acheter : " + crack.Value);
		crackMenu.AddItem(CrackBuy);
		crack.OnSliderChanged += LsMenus.onSliderChange;
		crackMenu.OnItemSelect += LsMenus.onCrackMenuSelect;
		crackMenu.OnMenuOpen += DrugStockMenu_OnMenuOpen;
		cocainMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Cocaïne");
		cocaine = new UIMenuSliderItem("Cocaïne " + LsFunctions.IntToMoney(cocain1Price - LsFunctions.Discount(cocain1Price)) + " $/oz");
		cocainMenu.AddItem(cocaine);
		cocaine.Maximum = dealer1Cocain;
		cocaine.Multiplier = 1;
		cocainMenu.AddItem(OrderMultipliers);
		CocainBuy = new UIMenuItem("Acheter : " + cocaine.Value);
		cocainMenu.AddItem(CocainBuy);
		cocaine.OnSliderChanged += LsMenus.onSliderChange;
		cocainMenu.OnItemSelect += LsMenus.onCocainMenuSelect;
		cocainMenu.OnMenuOpen += DrugStockMenu_OnMenuOpen;
		OrderSummaryMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Récapitulatif");
		OrderSummaryWeed = new UIMenuItem("Cannabis");
		OrderSummaryCrack = new UIMenuItem("Crack");
		OrderSummaryCocaine = new UIMenuItem("Cocaïne");
		OrderSummaryBill = new UIMenuItem("Facture");
		OrderSummaryDebt = new UIMenuItem("Dette");
		OrderSummaryCredit = new UIMenuItem("Plafond de crédit");
		OrderSummaryMenu.AddItem(OrderSummaryWeed);
		OrderSummaryMenu.AddItem(OrderSummaryCrack);
		OrderSummaryMenu.AddItem(OrderSummaryCocaine);
		OrderSummaryMenu.AddItem(OrderSummaryBill);
		OrderSummaryMenu.AddItem(OrderSummaryDebt);
		OrderSummaryMenu.AddItem(OrderSummaryCredit);
		OrderSummaryMenu.OnMenuOpen += (UIMenu sender) => LsFunctions.UpdateOrderMenuDesc();
		PlaceOrder = new UIMenuItem("Commander pour " + LsFunctions.IntToMoney(0) + " $");
		OrderDrugs.AddItem(PlaceOrder);
		OrderDrugs.OnItemSelect += LsMenus.OnOrderDrugsMenuSelect;
		stashMainMenu = new UIMenu("Stockage de la planque", "Choisis une option.");
		LsMenuPool.Add(stashMainMenu);
		stashTakeMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Retirer");
		stashPutMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Déposer");
		putWeedStash = new UIMenuSliderItem("Cannabis");
		stashPutMenu.AddItem(putWeedStash);
		putWeedStash.Maximum = PlayerInventory["Weed"];
		putWeedStash.Multiplier = 1;
		takeWeedStash = new UIMenuSliderItem("Cannabis");
		stashTakeMenu.AddItem(takeWeedStash);
		takeWeedStash.Maximum = 0;
		takeWeedStash.Multiplier = 1;
		putWeedStash.OnSliderChanged += LsMenus.onSliderChange;
		takeWeedStash.OnSliderChanged += LsMenus.onSliderChange;
		putCrackStash = new UIMenuSliderItem("Crack");
		stashPutMenu.AddItem(putCrackStash);
		putCrackStash.Maximum = PlayerInventory["Crack"];
		putCrackStash.Multiplier = 1;
		takeCrackStash = new UIMenuSliderItem("Crack");
		stashTakeMenu.AddItem(takeCrackStash);
		takeCrackStash.Maximum = 0;
		takeCrackStash.Multiplier = 1;
		putCrackStash.OnSliderChanged += LsMenus.onSliderChange;
		takeCrackStash.OnSliderChanged += LsMenus.onSliderChange;
		putCocainStash = new UIMenuSliderItem("Cocaïne");
		stashPutMenu.AddItem(putCocainStash);
		putCocainStash.Maximum = PlayerInventory["Cocaine"];
		putCocainStash.Multiplier = 1;
		takeCocainStash = new UIMenuSliderItem("Cocaïne");
		stashTakeMenu.AddItem(takeCocainStash);
		takeCocainStash.Maximum = 0;
		takeCocainStash.Multiplier = 1;
		putCocainStash.OnSliderChanged += LsMenus.onSliderChange;
		takeCocainStash.OnSliderChanged += LsMenus.onSliderChange;
		putWeedOzStash = new UIMenuSliderItem("Cannabis oz");
		stashPutMenu.AddItem(putWeedOzStash);
		putWeedOzStash.Maximum = LsFunctions.pWeedOunce;
		putWeedOzStash.Multiplier = 1;
		putWeedOzStash.OnSliderChanged += LsMenus.onSliderChange;
		putCrackOzStash = new UIMenuSliderItem("Crack oz");
		stashPutMenu.AddItem(putCrackOzStash);
		putCrackOzStash.Maximum = LsFunctions.pCrackOunce;
		putCrackOzStash.Multiplier = 1;
		putCrackStash.OnSliderChanged += LsMenus.onSliderChange;
		putCocainOzStash = new UIMenuSliderItem("Cocaïne oz");
		stashPutMenu.AddItem(putCocainOzStash);
		putCocainOzStash.Maximum = LsFunctions.pCocaineOunce;
		putCocainStash.Multiplier = 1;
		putCocainStash.OnSliderChanged += LsMenus.onSliderChange;
		putMoneyStash = new UIMenuSliderItem("Argent");
		stashPutMenu.AddItem(putMoneyStash);
		putMoneyStash.Maximum = player.Money;
		putMoneyStash.Multiplier = 100;
		takeMoneyStash = new UIMenuSliderItem("Argent");
		stashTakeMenu.AddItem(takeMoneyStash);
		takeMoneyStash.Maximum = 0;
		takeMoneyStash.Multiplier = 100;
		putMoneyStash.OnSliderChanged += LsMenus.onSliderChange;
		takeMoneyStash.OnSliderChanged += LsMenus.onSliderChange;
		stashPutMultiplier = new UIMenuListItem("Multiplicateur", multiplier, 0);
		stashPutMenu.AddItem(stashPutMultiplier);
		stashPutMultiplier.OnListChanged += LsMenus.onListChange;
		stashTakeMultiplier = new UIMenuListItem("Multiplicateur", multiplier, 0);
		stashTakeMenu.AddItem(stashTakeMultiplier);
		stashTakeMultiplier.OnListChanged += LsMenus.onListChange;
		Hud.Innit();
		stashMainMenu.FormatDescriptions = true;
		string mWeed = "cannabis. ";
		string mCrack = "Crack. ";
		string mCocaine = "cocaïne. ";
		string mGrams = "Grammes de ";
		string mOunce = "Onces de ";
		string mSell = "Prêt à vendre dans la rue ou à confier à un dealer.";
		string mStash = "À déposer dans une planque avant la vente.";
		takeWeedStash.Description = mGrams + mWeed + mSell;
		takeCrackStash.Description = mGrams + mCrack + mSell;
		takeCocainStash.Description = mGrams + mCocaine + mSell;
		putWeedStash.Description = mGrams + mWeed + mSell;
		putCrackStash.Description = mGrams + mCrack + mSell;
		putCocainStash.Description = mGrams + mCocaine + mSell;
		putWeedOzStash.Description = mOunce + mWeed + mStash;
		putCrackOzStash.Description = mOunce + mCrack + mStash;
		putCocainOzStash.Description = mOunce + mCocaine + mStash;
		List<object> pArm = new List<object>();
		for (int t = 0; t < playerArmours + 1; t++)
		{
			pArm.Add(t);
		}
		putArmourStash = new UIMenuListItem("Déposer un gilet", pArm, 0);
		stashPutMenu.AddItem(putArmourStash);
		List<object> sArm = new List<object>();
		for (int j = 0; j < 1; j++)
		{
			sArm.Add(j);
		}
		takeArmourStash = new UIMenuListItem("Retirer un gilet", sArm, 0);
		stashTakeMenu.AddItem(takeArmourStash);
		if (playerWeapons.Count > 0)
		{
			putWeaponStash = new UIMenuListItem("Déposer une arme : ", playerWeps, 0);
			stashPutMenu.AddItem(putWeaponStash);
			putWeaponStash.OnListChanged += LsMenus.onListChange;
			takeWeaponStash = new UIMenuListItem("Retirer une arme :", playerWeps, 0);
			stashTakeMenu.AddItem(takeWeaponStash);
			takeWeaponStash.OnListChanged += LsMenus.onListChange;
		}
		stashPutMenu.OnItemSelect += LsMenus.onStashMenuSelect;
		stashTakeMenu.OnItemSelect += LsMenus.onPlayerMenuSelect;
		vehStashMainMenu = new UIMenu("Stockage du véhicule", "Choisir");
		LsMenuPool.Add(vehStashMainMenu);
		vehStashTakeMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Retirer");
		vehStashPutMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Déposer");
		putWeedVeh = new UIMenuSliderItem("Cannabis " + LsFunctions.GramsToOz(PlayerInventory["Weed"]));
		vehStashPutMenu.AddItem(putWeedVeh);
		putWeedVeh.Maximum = PlayerInventory["Weed"];
		putWeedVeh.Multiplier = 1;
		takeWeedVeh = new UIMenuSliderItem("Cannabis drugCarWeedg");
		vehStashTakeMenu.AddItem(takeWeedVeh);
		takeWeedVeh.Maximum = 0;
		takeWeedVeh.Multiplier = 1;
		putWeedVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeWeedVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCrackVeh = new UIMenuSliderItem("Crack " + LsFunctions.GramsToOz(PlayerInventory["Crack"]));
		vehStashPutMenu.AddItem(putCrackVeh);
		putCrackVeh.Maximum = PlayerInventory["Crack"];
		putCrackVeh.Multiplier = 1;
		takeCrackVeh = new UIMenuSliderItem("Crack drugCarCrackg");
		vehStashTakeMenu.AddItem(takeCrackVeh);
		takeCrackVeh.Maximum = 0;
		takeCrackVeh.Multiplier = 1;
		putCrackVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCrackVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCocainVeh = new UIMenuSliderItem("Cocaïne " + LsFunctions.GramsToOz(PlayerInventory["Cocaine"]));
		vehStashPutMenu.AddItem(putCocainVeh);
		putCocainVeh.Maximum = PlayerInventory["Cocaine"];
		putCocainVeh.Multiplier = 1;
		takeCocainVeh = new UIMenuSliderItem("Cocaïne drugCarCocaing");
		vehStashTakeMenu.AddItem(takeCocainVeh);
		takeCocainVeh.Maximum = 0;
		takeCocainVeh.Multiplier = 1;
		putCocainVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCocainVeh.OnSliderChanged += LsMenus.onSliderChange;
		putWeedOzVeh = new UIMenuSliderItem("Cannabis oz~g~" + LsFunctions.pWeedOunce + "~s~oz");
		vehStashPutMenu.AddItem(putWeedOzVeh);
		putWeedOzVeh.Maximum = LsFunctions.pWeedOunce;
		putWeedOzVeh.Multiplier = 1;
		takeWeedOzVeh = new UIMenuSliderItem("Cannabis ozdrugCarWeedoz");
		vehStashTakeMenu.AddItem(takeWeedOzVeh);
		takeWeedOzVeh.Maximum = 0;
		takeWeedOzVeh.Multiplier = 1;
		putWeedOzVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeWeedOzVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCrackOzVeh = new UIMenuSliderItem("Crack oz~g~" + LsFunctions.pCrackOunce + "~s~oz");
		vehStashPutMenu.AddItem(putCrackOzVeh);
		putCrackOzVeh.Maximum = LsFunctions.pCrackOunce;
		putCrackOzVeh.Multiplier = 1;
		takeCrackOzVeh = new UIMenuSliderItem("Crack ozdrugCarCrackoz");
		vehStashTakeMenu.AddItem(takeCrackOzVeh);
		takeCrackOzVeh.Maximum = 0;
		takeCrackOzVeh.Multiplier = 1;
		putCrackOzVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCrackOzVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCocainOzVeh = new UIMenuSliderItem("Cocaïne oz~g~" + LsFunctions.pCocaineOunce + "~s~oz");
		vehStashPutMenu.AddItem(putCocainOzVeh);
		putCocainOzVeh.Maximum = LsFunctions.pCocaineOunce;
		putCocainOzVeh.Multiplier = 1;
		takeCocainOzVeh = new UIMenuSliderItem("Cocaïne ozdrugCarCocaing");
		vehStashTakeMenu.AddItem(takeCocainOzVeh);
		takeCocainOzVeh.Maximum = 0;
		takeCocainOzVeh.Multiplier = 1;
		putCocainOzVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCocainOzVeh.OnSliderChanged += LsMenus.onSliderChange;
		putMoneyVeh = new UIMenuSliderItem("Argent " + LsFunctions.IntToMoney(player.Money) + " $");
		vehStashPutMenu.AddItem(putMoneyVeh);
		putMoneyVeh.Maximum = player.Money;
		putMoneyVeh.Multiplier = 100;
		takeMoneyVeh = new UIMenuSliderItem("Argent");
		vehStashTakeMenu.AddItem(takeMoneyVeh);
		takeMoneyVeh.Maximum = 0;
		takeMoneyVeh.Multiplier = 100;
		putMoneyVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeMoneyVeh.OnSliderChanged += LsMenus.onSliderChange;
		vehPutMultiplier = new UIMenuListItem("Multiplicateur", multiplier, 0);
		vehStashPutMenu.AddItem(vehPutMultiplier);
		vehPutMultiplier.OnListChanged += LsMenus.onListChange;
		vehTakeMultiplier = new UIMenuListItem("Multiplicateur", multiplier, 0);
		vehStashTakeMenu.AddItem(vehTakeMultiplier);
		vehTakeMultiplier.OnListChanged += LsMenus.onListChange;
		takeWeaponVeh = new UIMenuListItem("Retirer une arme :", new List<object> { "EROaR!" }, 0);
		vehStashTakeMenu.AddItem(takeWeaponVeh);
		takeWeaponVeh.OnListChanged += LsMenus.onListChange;
		putArmourVeh = new UIMenuListItem("Déposer un gilet", pArm, 0);
		vehStashPutMenu.AddItem(putArmourVeh);
		putArmourVeh.Activated += PutArmourVeh_Activated;
		List<object> vArm = new List<object>();
		for (int k = 0; k < 1; k++)
		{
			vArm.Add(k);
		}
		takeArmourVeh = new UIMenuListItem("Retirer un gilet", vArm, 0);
		vehStashTakeMenu.AddItem(takeArmourVeh);
		takeArmourVeh.Activated += TakeArmourVeh_Activated;
		if (playerWeapons.Count > 0)
		{
			putWeaponVeh = new UIMenuListItem("Déposer une arme : ", playerWeps, 0);
			vehStashPutMenu.AddItem(putWeaponVeh);
			putWeaponVeh.OnListChanged += LsMenus.onListChange;
		}
		vehStashPutMenu.OnItemSelect += LsMenus.onPutVehicleMenuSelect;
		vehStashTakeMenu.OnItemSelect += LsMenus.onTakeVehicleMenuSelect;
		customerMainMenu = new UIMenu("Client", "Choisis une option");
		LsMenuPool.Add(customerMainMenu);
		sell = new UIMenuItem("Drogue");
		customerMainMenu.AddItem(sell);
		turnAway = new UIMenuItem("À plus");
		customerMainMenu.AddItem(turnAway);
		customerMainMenu.OnItemSelect += LsMenus.onCustomerMenuSelect;
		// Bandeaux d'origine conserves en attendant la refonte complete du theme.
		// Centralise ici pour conserver la navigation et les actions existantes.
		if (UseSprites)
		{
			zeeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			startDealing.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			OrderDrugs.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			hireDealerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_dealer.png");
		}
		if (UseSprites)
		{
			WorkerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_worker.png");
		}
		if (UseSprites)
		{
			wepMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_dealer.png");
		}
		if (UseSprites)
		{
			weedMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			crackMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			cocainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			OrderSummaryMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		if (UseSprites)
		{
			stashMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		if (UseSprites)
		{
			stashTakeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		if (UseSprites)
		{
			stashPutMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		if (UseSprites)
		{
			vehStashMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		if (UseSprites)
		{
			vehStashTakeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		if (UseSprites)
		{
			vehStashPutMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		if (UseSprites)
		{
			customerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_customer.png");
		}
		
		LsMenuPool.FormatDescriptions = true;
		LsMenuPool.RefreshIndex();
    }
}
