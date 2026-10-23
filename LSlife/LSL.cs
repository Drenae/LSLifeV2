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

public partial class LSL : Script
{
	private string vNumber = "0.2.46a";

	public static bool hardMode = false;

	public static bool DEBUG = true;

	public static bool UseSprites = true;

	public static bool DrawMarkers = true;

	public static bool started = false;

	public static bool EnableBlips;

	public static bool AreaDebug;

	public static bool LCRWMAP = false;

	public static ScriptSettings iniSettings;

	public static HirePed PottentialWorker = null;

	public static MenuPool LsMenuPool;

	public static PlayerDealerHandler DealerHandler;

	public static StashHouseHandler HouseHandler;

	public static PickupHandler pickupHandler = new PickupHandler();

	public static DriveByHandler driveByHandler = new DriveByHandler();

	public static LsLPeds lPeds;

	public static int CustomerRel = World.AddRelationshipGroup("LSCUSTOMER");

	public static int rival_nutral = World.AddRelationshipGroup("RIVAL_N");

	public static int rival_enemy = World.AddRelationshipGroup("RIVAL_E");

	public static int playerRelationship = Function.Call<int>(unchecked((Hash)(-3292914402564945716L)), ("PLAYER"));

	public PedDebugger PedDebugger = new PedDebugger();

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

	public static Keys MenuKey;

	public static Keys StopDealing;

	public static Keys SetDrugCar;

	public static XDocument vehWeaponsXML;

	public static XDocument stashWeaponsXML;

	public static XDocument LSLifeSave;

	public static XDocument VehicleXml;

	public static float displayHelpTimer = Game.GameTime;

	public static int day;

	public static int hour;

	public static int SlowTickTime;

	public static CustomiFruit iFruit;

	public static iFruitContact contactZee;

	public static DrugDealer ZeesDealers = null;

	public static DrugOrder NewOrder = new DrugOrder();

	public static Ped drugDealer1;

	public static Ped weaponDealer1;

	public static int dealerTimer;

	public static bool readyToDeal;

	public static bool dealer1Angry;

	public static int weed1Price = 500;

	public static int crack1Price = 2500;

	public static int cocain1Price = 5000;

	public static int dealer1Weed;

	public static int dealer1Crack;

	public static int dealer1Cocain;

	public static int dealer1Money;

	public static int dealer1ReloadDay;

	public static bool dealer1ReloadOrder = false;

	public static int dealer1ReloadHour = 20;

	public static bool dealer1Reloaded = false;

	public static int dealer1Debt;

	public static double dealer1Rep;

	public static int reloadTimer;

	public static int reloadTimerInterval = 600000;

	public static List<Pig> pigs = new List<Pig>();

	public static AreaDealer aDealer = null;

	public static StashVehicle pStashVehicle = null;

	public static Vector3 playerPos = Vector3.Zero;

	public static Player player = Game.Player;

	public static bool removedStuff = true;

	public static bool isDealing = false;

	public static bool chilling = false;

	public static int areaCheckTimer;

	public static Dictionary<string, Tuple<AreaType, jurisdictionType>> zoneData = new Dictionary<string, Tuple<AreaType, jurisdictionType>>();

	public static List<Area> areas = new List<Area>();

	public static int areaIndex = 0;

	public static string playerArea = "SanAnd";

	public static Random rnd = new Random();

	public static Dictionary<string, int> PlayerInventory = new Dictionary<string, int>();

	public static int playerArmours = 0;

	public static bool playerWanted = false;

	public static bool notFined = true;

	public static bool playerDead = false;

	public static List<dynamic> playerWeps = new List<object>();

	public static int newWepAmount = 0;

	public static List<XElement> playerWeapons = new List<XElement>();

	public static bool playerHasBag = false;

	public static bool canSprint = true;

	public static int randomAttackTimer;

	public static List<Entity> entitiesCleanUp = new List<Entity>();

	public static bool sellingWeed = false;

	public static bool sellingCrack = false;

	public static bool sellingCocaine = false;

	public static PedGroup playerGroup;

	public static int timeStartedDealing;

	public static int timeDealing = 0;

	public static Vector3 pedTarget;

	public static Ped attacker;

	public static bool attackerAttacking = false;

	public static bool attackerPickupSpawned = false;

	public static Pickup attackerPickup;

	public static int attackCheckTime = 0;

	public static List<XElement> attackerWeapons = new List<XElement>();

	public int attackerWeed;

	public int attackerCrack;

	public int attackerCocaine;

	public int attackerMoney;

	public static List<Ped> angryPeds = new List<Ped>();

	public static List<Customer> customers = new List<Customer>();

	public static string thisStreet;

	public static string lastStreet;

	public static float lastTimePedSelected = Game.GameTime;

	public static float lastTimeDealDone = Game.GameTime;

	public static float lastAngryTime = Game.GameTime;

	public static float ratTimer = 0f;

	public static float playerHeat;

	public static int rat;

	public static int lastCustomerCheckTimer;

	public static Customer customerSellTo;

	public static int oneSecDelay;

	public static bool firstRun = true;

	public static Dealer wepDealer;

	public static UIText myUIText;

	public static int PedsCanSeeDeal = 0;

	private bool showInventory;

	private bool pSaved;

	private uint lsDebug;

	private uint areaDebug;

	private uint spawnP;

	public static bool PlayerComplied { get; private set; }

	public LSL()
	{
		Setup();
		((Script)this).Aborted += OnAbort;
		((Script)this).Tick += onTick;
		((Script)this).KeyDown += onKeyDown;
		Interval = 3;
	}

}
