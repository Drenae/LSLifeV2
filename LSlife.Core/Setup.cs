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
	private void Setup()
	{
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Expected I4, but got Unknown
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Expected I4, but got Unknown
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Expected I4, but got Unknown
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Expected I4, but got Unknown
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		lsDebug = Function.Call<uint>(unchecked((Hash)(-3292914402564945716L)), ("lsdebug"));
		areaDebug = Function.Call<uint>(unchecked((Hash)(-3292914402564945716L)), ("areadebug"));
		spawnP = Function.Call<uint>(unchecked((Hash)(-3292914402564945716L)), ("spawnP"));
		player = Game.Player;
		player.Character.CurrentPedGroup.SeparationRange = 100f;
		iniSettings = ScriptSettings.Load("scripts\\LSlife.ini");
		DEBUG = iniSettings.GetValue<bool>("OPTIONS", "DEBUG", false);
		UseSprites = iniSettings.GetValue<bool>("OPTIONS", "SPRITES", true);
		hardMode = iniSettings.GetValue<bool>("OPTIONS", "HARD", false);
		DrawMarkers = iniSettings.GetValue<bool>("OPTIONS", "MARKERS", true);
		AreaDebug = iniSettings.GetValue<bool>("OPTIONS", "AREADEBUG", false);
		LCRWMAP = iniSettings.GetValue<bool>("OPTIONS", "LCRWMAP", false);
		if (hardMode)
		{
			weed1Price = 180;
			crack1Price = 400;
			cocain1Price = 1300;
		}
		LsFunctions.LoadZoneData();
		LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSLifeSave.Descendants())
		{
			if (element.Name == "playerInventory")
			{
				bool foundArmour = false;
				bool foundOz = false;
				foreach (XElement e in element.Elements())
				{
					int number;
					if (e.Name == "weed")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory.Add("Weed", number);
					}
					if (e.Name == "weedOz")
					{
						int.TryParse(e.Value, out LsFunctions.pWeedOunce);
						foundOz = true;
					}
					if (e.Name == "crack")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory.Add("Crack", number);
					}
					if (e.Name == "crackOz")
					{
						int.TryParse(e.Value, out LsFunctions.pCrackOunce);
						foundOz = true;
					}
					if (e.Name == "cocain")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory.Add("Cocaine", number);
					}
					if (e.Name == "cocaineOz")
					{
						int.TryParse(e.Value, out LsFunctions.pCocaineOunce);
						foundOz = true;
					}
					if (e.Name == "armour")
					{
						int.TryParse(e.Value, out playerArmours);
						foundArmour = true;
					}
				}
				if (!foundArmour)
				{
					element.Add(new XElement("armour", playerArmours.ToString()));
					LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
				}
				if (!foundOz)
				{
					element.Add(new XElement("weedOz", LsFunctions.pWeedOunce.ToString()));
					element.Add(new XElement("crackOz", LsFunctions.pCrackOunce.ToString()));
					element.Add(new XElement("cocaineOz", LsFunctions.pCocaineOunce.ToString()));
					LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
				}
			}
			if (!(element.Name == "dealer1"))
			{
				continue;
			}
			bool foundRep = false;
			foreach (XElement item in element.Elements())
			{
				if (item.Name == "rep")
				{
					foundRep = true;
					break;
				}
			}
			if (!foundRep)
			{
				XElement dealerRep = new XElement("rep");
				dealerRep.Value = "1";
				element.Add(dealerRep);
				LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			}
			foreach (XElement e2 in element.Elements())
			{
				if (e2.Name == "debt")
				{
					int.TryParse(e2.Value, out dealer1Debt);
					if (dealer1Debt > 200000)
					{
						dealer1Debt = 200000;
					}
					else
					{
						dealer1Angry = false;
						if (dealer1Debt < 0)
						{
							dealer1Debt = 0;
						}
					}
				}
				if (e2.Name == "weed")
				{
					int.TryParse(e2.Value, out dealer1Weed);
				}
				if (e2.Name == "crack")
				{
					int.TryParse(e2.Value, out dealer1Crack);
				}
				if (e2.Name == "cocain")
				{
					int.TryParse(e2.Value, out dealer1Cocain);
				}
				if (e2.Name == "money")
				{
					int.TryParse(e2.Value, out var tempInt);
					if (tempInt > 10000000)
					{
						dealer1Money = 10000000;
					}
					else
					{
						dealer1Money = tempInt;
					}
				}
				if (e2.Name == "reloaded")
				{
					bool.TryParse(e2.Value, out dealer1Reloaded);
				}
				if (e2.Name == "rep")
				{
					double.TryParse(e2.Value, out dealer1Rep);
				}
			}
		}
		VehicleXml = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
		Enum.TryParse<Keys>(iniSettings.GetValue("KEYS", "MAIN", "F10"), out var mainMenuKey);
		MenuKey = mainMenuKey;
		Enum.TryParse<Keys>(iniSettings.GetValue("KEYS", "STOPDEALING", "F11"), out var mainStopDealing);
		StopDealing = mainStopDealing;
		Enum.TryParse<Keys>(iniSettings.GetValue("KEYS", "SETDRUGCAR", "F12"), out var mainSetDrugCar);
		SetDrugCar = mainSetDrugCar;
		EnableBlips = true;
		stashWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Weapons.xml");
		foreach (XElement element2 in stashWeaponsXML.Descendants())
		{
			if (!(element2.Name == "weapon"))
			{
				continue;
			}
			foreach (XElement item2 in element2.Elements())
			{
				_ = item2.Name == "HASH";
			}
		}
		vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
		int i = 0;
		foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
		{
			if (!Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (player.Character), ((int)weapon)))
			{
				continue;
			}
			int ammunitionInWeapon = Function.Call<int>((Hash)97480644549409105L, (player.Character), ((int)weapon));
			XElement parentElement = new XElement("weapon");
			parentElement.SetAttributeValue("id", i);
			XElement weaponNameElement = new XElement("HASH", (object)weapon);
			_ = DEBUG;
			XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
			parentElement.Add(weaponNameElement);
			parentElement.Add(weaponAmmoElement);
			foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
			{
				if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (player.Character), ((int)weapon), ((int)component)))
				{
					XElement weaponComponentElement = new XElement("COMPONENT", (object)component);
					parentElement.Add(weaponComponentElement);
				}
			}
			playerWeapons.Add(parentElement);
			playerWeps.Add(((object)weapon/*cast due to constrained. prefix*/).ToString());
			i++;
		}
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
		((UIText)OrderDrugs.Title).Shadow = true;
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
		// iFruit est initialise apres le chargement de GTA dans Tick.
		// Ne pas creer le telephone pendant le constructeur du script.
		DealerHandler = new PlayerDealerHandler();
		HouseHandler = new StashHouseHandler();
		LsFunctions.LoadSpawnLocations();
		day = Function.Call<int>(unchecked((Hash)(-2777906517671992481L)));
		hour = Function.Call<int>((Hash)2675767815307398015L);
		SlowTickTime = Game.GameTime;
		randomAttackTimer = Game.GameTime;
		reloadTimer = Game.GameTime;
	}
}
