using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
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

public class LSL : Script
{
	public enum PedVariationData
	{
		PED_VARIATION_FACE,
		PED_VARIATION_HEAD,
		PED_VARIATION_HAIR,
		PED_VARIATION_TORSO,
		PED_VARIATION_LEGS,
		PED_VARIATION_HANDS,
		PED_VARIATION_FEET,
		PED_VARIATION_EYES,
		PED_VARIATION_ACCESSORIES,
		PED_VARIATION_TASKS,
		PED_VARIATION_TEXTURES,
		PED_VARIATION_TORSO2
	}

	public enum AreaType
	{
		Poor,
		Normal,
		Rich
	}

	public enum jurisdictionType
	{
		LSPD,
		LSSD,
		BCSO,
		SAPR
	}

	public class Door
	{
		private readonly bool DEBUG = true;

		public Prop Prop;

		private bool Locked;

		public Door(Prop prop)
		{
			Prop = prop;
			((Entity)Prop).IsInvincible = true;
			UseLock();
		}

		public void UseLock()
		{
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			Model model;
			if (Locked)
			{
				Locked = false;
				Script.Wait(10);
				Function.Call((Hash)3423943577717663365L, (InputArgument)("BIG_SCORE_GOLD_CAGE_LOCK"), (InputArgument)(0));
				Game.PlaySound("0x08053C9C", "BIG_SCORE_GOLD_CAGE_LOCK");
				model = ((Entity)Prop).Model;
				Function.Call(unchecked((Hash)(-7272475972733243984L)), (InputArgument)(model.Hash), (InputArgument)(((Entity)Prop).Position.X), (InputArgument)(((Entity)Prop).Position.Y), (InputArgument)(((Entity)Prop).Position.Z), (InputArgument)(false), (InputArgument)(0f), (InputArgument)(50f), (InputArgument)(0f));
				if (DEBUG)
				{
					model = ((Entity)Prop).Model;
					UI.Notify(model.Hash + " Porte déverrouillée " + ((object)((Entity)Prop).Position/*cast due to constrained. prefix*/).ToString());
				}
			}
			else
			{
				Locked = true;
				Script.Wait(10);
				Game.PlaySound("Bar_Lower_And_Lock", "DLC_IND_ROLLERCOASTER_SOUNDS");
				model = ((Entity)Prop).Model;
				Function.Call(unchecked((Hash)(-7272475972733243984L)), (InputArgument)(model.Hash), (InputArgument)(((Entity)Prop).Position.X), (InputArgument)(((Entity)Prop).Position.Y), (InputArgument)(((Entity)Prop).Position.Z), (InputArgument)(true), (InputArgument)(0f), (InputArgument)(50f), (InputArgument)(0f));
				if (DEBUG)
				{
					model = ((Entity)Prop).Model;
					UI.Notify(model.Hash + " Porte verrouillée " + ((object)((Entity)Prop).Position/*cast due to constrained. prefix*/).ToString());
				}
			}
		}
	}

	public enum Nodetype
	{
		AnyRoad,
		Road,
		Offroad,
		Water
	}

	public enum PathnodeFlags
	{
		Slow = 1,
		Two = 2,
		Intersection = 4,
		Eight = 8,
		SlowTraffic = 12,
		ThirtyTwo = 32,
		Freeway = 64,
		FourWayIntersection = 128,
		BigIntersectionLeft = 512
	}

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

	public static int playerRelationship = Function.Call<int>(unchecked((Hash)(-3292914402564945716L)), (InputArgument)("PLAYER"));

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

	public static UIMenuSliderItem putWeedGramsStash;

	public static UIMenuSliderItem takeWeedGramsStash;

	public static UIMenuSliderItem putCrackGramsStash;

	public static UIMenuSliderItem takeCrackGramsStash;

	public static UIMenuSliderItem putCocainGramsStash;

	public static UIMenuSliderItem takeCocainGramsStash;

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

	public static UIMenuSliderItem putWeedGramsVeh;

	public static UIMenuSliderItem takeWeedGramsVeh;

	public static UIMenuSliderItem putCrackGramsVeh;

	public static UIMenuSliderItem takeCrackGramsVeh;

	public static UIMenuSliderItem putCocainGramsVeh;

	public static UIMenuSliderItem takeCocainGramsVeh;

	public static UIMenuSliderItem putMoneyVeh;

	public static UIMenuSliderItem takeMoneyVeh;

	public static UIMenuListItem putArmourVeh;

	public static UIMenuListItem takeArmourVeh;

	public static UIMenuListItem putWeaponVeh;

	public static UIMenuListItem takeWeaponVeh;

	public static UIMenuListItem vehPutMultiplier;

	public static UIMenuListItem vehTakeMultiplier;

	public static readonly List<dynamic> multiplier = new List<object> { 1, 5, 10, 25, 50, 100, 250, 500, 1000 };

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

	public static int TotalDrugsCarried()
	{
		return LsFunctions.TotalDrugs(PlayerInventory);
	}

	public LSL()
	{
		Setup();
		((Script)this).Aborted += OnAbort;
		((Script)this).Tick += onTick;
		((Script)this).KeyDown += onKeyDown;
		this.Interval = 3;
	}

	private void Setup()
	{
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Expected I4, but got Unknown
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Expected I4, but got Unknown
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Expected Obj, but got Unknown
		//IL_0955: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Expected Obj, but got Unknown
		//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Expected Obj, but got Unknown
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d3: Expected Obj, but got Unknown
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e3: Expected Obj, but got Unknown
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Expected Obj, but got Unknown
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Expected Obj, but got Unknown
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Expected I4, but got Unknown
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Expected I4, but got Unknown
		//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Expected Obj, but got Unknown
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Expected Obj, but got Unknown
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afa: Expected Obj, but got Unknown
		//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Expected Obj, but got Unknown
		//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Expected Obj, but got Unknown
		//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Expected Obj, but got Unknown
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b85: Expected Obj, but got Unknown
		//IL_0bad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Expected Obj, but got Unknown
		//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Expected Obj, but got Unknown
		//IL_0bff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c09: Expected Obj, but got Unknown
		//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1f: Expected Obj, but got Unknown
		//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c35: Expected Obj, but got Unknown
		//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c49: Expected Obj, but got Unknown
		//IL_0c7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c88: Expected Obj, but got Unknown
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbb: Expected Obj, but got Unknown
		//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cee: Expected Obj, but got Unknown
		//IL_0d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1c: Expected Obj, but got Unknown
		//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2b: Expected Obj, but got Unknown
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Expected Obj, but got Unknown
		//IL_0d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5e: Expected Obj, but got Unknown
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6d: Expected Obj, but got Unknown
		//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d82: Expected Obj, but got Unknown
		//IL_0e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1f: Expected Obj, but got Unknown
		//IL_0e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e35: Expected Obj, but got Unknown
		//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4b: Expected Obj, but got Unknown
		//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e61: Expected Obj, but got Unknown
		//IL_0e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e77: Expected Obj, but got Unknown
		//IL_0e83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8d: Expected Obj, but got Unknown
		//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea3: Expected Obj, but got Unknown
		//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb9: Expected Obj, but got Unknown
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecf: Expected Obj, but got Unknown
		//IL_0f15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1f: Expected Obj, but got Unknown
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Expected Obj, but got Unknown
		//IL_0f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f95: Expected Obj, but got Unknown
		//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcd: Expected Obj, but got Unknown
		//IL_0fe1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0feb: Expected Obj, but got Unknown
		//IL_1006: Unknown result type (might be due to invalid IL or missing references)
		//IL_1010: Expected Obj, but got Unknown
		//IL_101c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1026: Expected Obj, but got Unknown
		//IL_1032: Unknown result type (might be due to invalid IL or missing references)
		//IL_103c: Expected Obj, but got Unknown
		//IL_1048: Unknown result type (might be due to invalid IL or missing references)
		//IL_1052: Expected Obj, but got Unknown
		//IL_105f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Expected Obj, but got Unknown
		//IL_10bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c6: Expected Obj, but got Unknown
		//IL_111c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1126: Expected Obj, but got Unknown
		//IL_1141: Unknown result type (might be due to invalid IL or missing references)
		//IL_114b: Expected Obj, but got Unknown
		//IL_1157: Unknown result type (might be due to invalid IL or missing references)
		//IL_1161: Expected Obj, but got Unknown
		//IL_11b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11be: Expected Obj, but got Unknown
		//IL_1214: Unknown result type (might be due to invalid IL or missing references)
		//IL_121e: Expected Obj, but got Unknown
		//IL_1239: Unknown result type (might be due to invalid IL or missing references)
		//IL_1243: Expected Obj, but got Unknown
		//IL_124f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1259: Expected Obj, but got Unknown
		//IL_12ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b6: Expected Obj, but got Unknown
		//IL_130c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1316: Expected Obj, but got Unknown
		//IL_1331: Unknown result type (might be due to invalid IL or missing references)
		//IL_133b: Expected Obj, but got Unknown
		//IL_1347: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Expected Obj, but got Unknown
		//IL_1361: Unknown result type (might be due to invalid IL or missing references)
		//IL_136b: Expected Obj, but got Unknown
		//IL_1386: Unknown result type (might be due to invalid IL or missing references)
		//IL_1390: Expected Obj, but got Unknown
		//IL_139a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a4: Expected Obj, but got Unknown
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1436: Expected Obj, but got Unknown
		//IL_146e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1478: Expected Obj, but got Unknown
		//IL_14a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b3: Expected Obj, but got Unknown
		//IL_14bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c9: Expected Obj, but got Unknown
		//IL_14ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d8: Expected Obj, but got Unknown
		//IL_1510: Unknown result type (might be due to invalid IL or missing references)
		//IL_151a: Expected Obj, but got Unknown
		//IL_154b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1555: Expected Obj, but got Unknown
		//IL_1561: Unknown result type (might be due to invalid IL or missing references)
		//IL_156b: Expected Obj, but got Unknown
		//IL_1570: Unknown result type (might be due to invalid IL or missing references)
		//IL_157a: Expected Obj, but got Unknown
		//IL_15b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bc: Expected Obj, but got Unknown
		//IL_15ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f7: Expected Obj, but got Unknown
		//IL_1603: Unknown result type (might be due to invalid IL or missing references)
		//IL_160d: Expected Obj, but got Unknown
		//IL_1612: Unknown result type (might be due to invalid IL or missing references)
		//IL_161c: Expected Obj, but got Unknown
		//IL_1651: Unknown result type (might be due to invalid IL or missing references)
		//IL_165b: Expected Obj, but got Unknown
		//IL_1660: Unknown result type (might be due to invalid IL or missing references)
		//IL_166a: Expected Obj, but got Unknown
		//IL_169f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a9: Expected Obj, but got Unknown
		//IL_16ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b8: Expected Obj, but got Unknown
		//IL_16ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f7: Expected Obj, but got Unknown
		//IL_16fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1706: Expected Obj, but got Unknown
		//IL_173a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1744: Expected Obj, but got Unknown
		//IL_1776: Unknown result type (might be due to invalid IL or missing references)
		//IL_1780: Expected Obj, but got Unknown
		//IL_178c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1796: Expected Obj, but got Unknown
		//IL_17a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ab: Expected Obj, but got Unknown
		//IL_17c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d0: Expected Obj, but got Unknown
		//IL_17db: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e5: Expected Obj, but got Unknown
		//IL_1800: Unknown result type (might be due to invalid IL or missing references)
		//IL_180a: Expected Obj, but got Unknown
		//IL_193b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1945: Expected Obj, but got Unknown
		//IL_1981: Unknown result type (might be due to invalid IL or missing references)
		//IL_198b: Expected Obj, but got Unknown
		//IL_1a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a31: Expected Obj, but got Unknown
		//IL_1a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a47: Expected Obj, but got Unknown
		//IL_1a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5b: Expected Obj, but got Unknown
		//IL_19b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_19bc: Expected Obj, but got Unknown
		//IL_19d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e1: Expected Obj, but got Unknown
		//IL_19ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f6: Expected Obj, but got Unknown
		//IL_1a11: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1b: Expected Obj, but got Unknown
		//IL_1afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b06: Expected Obj, but got Unknown
		//IL_1b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b48: Expected Obj, but got Unknown
		//IL_1b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b83: Expected Obj, but got Unknown
		//IL_1b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b99: Expected Obj, but got Unknown
		//IL_1bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc1: Expected Obj, but got Unknown
		//IL_1bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c03: Expected Obj, but got Unknown
		//IL_1c34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c3e: Expected Obj, but got Unknown
		//IL_1c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c54: Expected Obj, but got Unknown
		//IL_1c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7c: Expected Obj, but got Unknown
		//IL_1cb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cbe: Expected Obj, but got Unknown
		//IL_1cef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf9: Expected Obj, but got Unknown
		//IL_1d05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0f: Expected Obj, but got Unknown
		//IL_1d28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d32: Expected Obj, but got Unknown
		//IL_1d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6a: Expected Obj, but got Unknown
		//IL_1d9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da5: Expected Obj, but got Unknown
		//IL_1db1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbb: Expected Obj, but got Unknown
		//IL_1dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dde: Expected Obj, but got Unknown
		//IL_1e0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e16: Expected Obj, but got Unknown
		//IL_1e47: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e51: Expected Obj, but got Unknown
		//IL_1e5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e67: Expected Obj, but got Unknown
		//IL_1e80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e8a: Expected Obj, but got Unknown
		//IL_1eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec2: Expected Obj, but got Unknown
		//IL_1ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1efd: Expected Obj, but got Unknown
		//IL_1f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f13: Expected Obj, but got Unknown
		//IL_1f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f36: Expected Obj, but got Unknown
		//IL_1f6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f74: Expected Obj, but got Unknown
		//IL_1fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb0: Expected Obj, but got Unknown
		//IL_1fbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc6: Expected Obj, but got Unknown
		//IL_1fd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fdb: Expected Obj, but got Unknown
		//IL_1ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2000: Expected Obj, but got Unknown
		//IL_200b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2015: Expected Obj, but got Unknown
		//IL_2030: Unknown result type (might be due to invalid IL or missing references)
		//IL_203a: Expected Obj, but got Unknown
		//IL_2050: Unknown result type (might be due to invalid IL or missing references)
		//IL_205a: Expected Obj, but got Unknown
		//IL_2075: Unknown result type (might be due to invalid IL or missing references)
		//IL_207f: Expected Obj, but got Unknown
		//IL_2087: Unknown result type (might be due to invalid IL or missing references)
		//IL_2091: Expected Obj, but got Unknown
		//IL_20ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_20b6: Expected Obj, but got Unknown
		//IL_20e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ed: Expected Obj, but got Unknown
		//IL_2108: Unknown result type (might be due to invalid IL or missing references)
		//IL_2112: Expected Obj, but got Unknown
		//IL_2165: Unknown result type (might be due to invalid IL or missing references)
		//IL_216f: Expected Obj, but got Unknown
		//IL_217b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2185: Expected Obj, but got Unknown
		//IL_218f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2199: Expected Obj, but got Unknown
		//IL_212a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2134: Expected Obj, but got Unknown
		//IL_214f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2159: Expected Obj, but got Unknown
		//IL_21c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cd: Expected Obj, but got Unknown
		//IL_21e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_21eb: Expected Obj, but got Unknown
		//IL_2206: Unknown result type (might be due to invalid IL or missing references)
		//IL_2210: Expected Obj, but got Unknown
		//IL_2225: Unknown result type (might be due to invalid IL or missing references)
		//IL_222f: Expected Obj, but got Unknown
		//IL_2234: Unknown result type (might be due to invalid IL or missing references)
		//IL_223e: Expected Obj, but got Unknown
		//IL_224a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2254: Expected Obj, but got Unknown
		lsDebug = Function.Call<uint>(unchecked((Hash)(-3292914402564945716L)), (InputArgument)("lsdebug"));
		areaDebug = Function.Call<uint>(unchecked((Hash)(-3292914402564945716L)), (InputArgument)("areadebug"));
		spawnP = Function.Call<uint>(unchecked((Hash)(-3292914402564945716L)), (InputArgument)("spawnP"));
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
				bool foundWeedGrams = false;
				bool foundCrackGrams = false;
				bool foundCocaineGrams = false;
				foreach (XElement e in element.Elements())
				{
					int number;
					if (e.Name == "weed")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory.Add("Weed", number);
					}
					if (e.Name == "weedGrams")
					{
						int.TryParse(e.Value, out LsFunctions.pWeedGrams);
						foundWeedGrams = true;
					}
					if (e.Name == "crack")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory.Add("Crack", number);
					}
					if (e.Name == "crackGrams")
					{
						int.TryParse(e.Value, out LsFunctions.pCrackGrams);
						foundCrackGrams = true;
					}
					if (e.Name == "cocain")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory.Add("Cocaine", number);
					}
					if (e.Name == "cocaineGrams")
					{
						int.TryParse(e.Value, out LsFunctions.pCocaineGrams);
						foundCocaineGrams = true;
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
				if (!foundWeedGrams) element.Add(new XElement("weedGrams", LsFunctions.pWeedGrams.ToString()));
				if (!foundCrackGrams) element.Add(new XElement("crackGrams", LsFunctions.pCrackGrams.ToString()));
				if (!foundCocaineGrams) element.Add(new XElement("cocaineGrams", LsFunctions.pCocaineGrams.ToString()));
				if (!foundWeedGrams || !foundCrackGrams || !foundCocaineGrams)
				{
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
			if (!Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (InputArgument)(player.Character), (InputArgument)((int)weapon)))
			{
				continue;
			}
			int ammunitionInWeapon = Function.Call<int>((Hash)97480644549409105L, (InputArgument)(player.Character), (InputArgument)((int)weapon));
			XElement parentElement = new XElement("weapon");
			parentElement.SetAttributeValue("id", i);
			XElement weaponNameElement = new XElement("HASH", (object)weapon);
			_ = DEBUG;
			XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
			parentElement.Add(weaponNameElement);
			parentElement.Add(weaponAmmoElement);
			foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
			{
				if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (InputArgument)(player.Character), (InputArgument)((int)weapon), (InputArgument)((int)component)))
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
		zeeMenu = new UIMenu("Rafael Mendoza", "Que veux-tu ?");
		if (UseSprites)
		{
			zeeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		LsMenuPool.Add(zeeMenu);
		startDealing = LsMenuPool.AddSubMenu(zeeMenu, "Vente de drogue");
		if (UseSprites)
		{
			startDealing.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		sellWeed = new UIMenuCheckboxItem("Vendre du cannabis", true);
		sellCrack = new UIMenuCheckboxItem("Vendre du crack", true);
		sellCocaine = new UIMenuCheckboxItem("Vendre de la cocaïne", true);
		StartDeal = new UIMenuItem("Commencer la vente");
		startDealing.AddItem((UIMenuItem)(object)sellWeed);
		startDealing.AddItem((UIMenuItem)(object)sellCrack);
		startDealing.AddItem((UIMenuItem)(object)sellCocaine);
		startDealing.AddItem(StartDeal);
		startDealing.OnItemSelect += LsMenus.OnstartDealSelect;
		OrderDrugs = LsMenuPool.AddSubMenu(zeeMenu, "Commander de la drogue");
		// LSL V2 FR : élargir ce menu NativeUI pour conserver les libellés complets.
		OrderDrugs.SetMenuWidthOffset(140);
		if (UseSprites)
		{
			OrderDrugs.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		OrderDrugs.BannerSprite.Color = Color.Black;
		OrderDrugs.Title.DropShadow = true;
		OrderDrugs.Title.Outline = true;
		((UIText)OrderDrugs.Title).Shadow = true;
		OrderDrugs.OnMenuOpen += OrderDrugs_OnMenuOpen;
		buyWeapons = new UIMenuItem("Acheter des armes");
		zeeMenu.AddItem(buyWeapons);
		buyLift = new UIMenuItem("Demander un trajet");
		zeeMenu.AddItem(buyLift);
		payDebt = new UIMenuItem("Rembourser la dette");
		zeeMenu.AddItem(payDebt);
		zeeMenu.OnItemSelect += LsMenus.OnZeeMenuSelect;
		hireDealerMainMenu = new UIMenu("Recruter un dealer", "Choisir une option");
		if (UseSprites)
		{
			hireDealerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_dealer.png");
		}
		LsMenuPool.Add(hireDealerMainMenu);
		hireDealerOfferAmount = new UIMenuSliderItem("Montant proposé");
		hireDealerOfferAmount.Maximum = player.Money;
		hireDealerOfferAmount.Multiplier = 1000;
		hireDealerMakeOffer = new UIMenuItem("Faire une offre");
		hireDealerFollowButton = new UIMenuItem("Annuler l’offre");
		hireDealerMainMenu.AddItem((UIMenuItem)(object)hireDealerOfferAmount);
		hireDealerMainMenu.AddItem(hireDealerMakeOffer);
		hireDealerMainMenu.AddItem(hireDealerFollowButton);
		hireDealerOfferAmount.OnSliderChanged += LsMenus.HireDealerOfferAmount_OnSliderChanged;
		hireDealerMakeOffer.Activated += LsMenus.HireDealerOfferSelect;
		hireDealerFollowButton.Activated += LsMenus.HireDealerFollowSelect;
		WorkerMainMenu = new UIMenu("Employé", "Donner des ressources au dealer");
		if (UseSprites)
		{
			WorkerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_worker.png");
		}
		LsMenuPool.Add(WorkerMainMenu);
		wWeed = new UIMenuSliderItem("Cannabis " + LsFunctions.FormatMetric(0));
		wWeed.Maximum = PlayerInventory["Weed"];
		wCrack = new UIMenuSliderItem("Crack " + LsFunctions.FormatMetric(0));
		wCrack.Maximum = PlayerInventory["Crack"];
		wCocaine = new UIMenuSliderItem("Cocaïne " + LsFunctions.FormatMetric(0));
		wCocaine.Maximum = PlayerInventory["Cocaine"];
		wMultiplier = new UIMenuListItem("Multiplier", (List<object>)multiplier, 0);
		wConfirm = new UIMenuItem("Confirmer");
		wCollect = new UIMenuItem("Récupérer");
		List<object> orders = new List<object> { "Nothing" };
		wOrder = new UIMenuListItem("Commandes", orders, 0);
		wArmour = new UIMenuItem("Donner un gilet pare-balles");
		wWeapons = new UIMenuListItem("Donner une arme", (List<object>)playerWeps, 0);
		WorkerMainMenu.AddItem((UIMenuItem)(object)wWeed);
		WorkerMainMenu.AddItem((UIMenuItem)(object)wCrack);
		WorkerMainMenu.AddItem((UIMenuItem)(object)wCocaine);
		WorkerMainMenu.AddItem((UIMenuItem)(object)wMultiplier);
		WorkerMainMenu.AddItem(wConfirm);
		WorkerMainMenu.AddItem(wCollect);
		WorkerMainMenu.AddItem((UIMenuItem)(object)wOrder);
		WorkerMainMenu.AddItem(wArmour);
		WorkerMainMenu.AddItem((UIMenuItem)(object)wWeapons);
		wWeed.OnSliderChanged += LsMenus.WWeed_OnSliderChanged;
		wCrack.OnSliderChanged += LsMenus.WCrack_OnSliderChanged;
		wCocaine.OnSliderChanged += LsMenus.WCocaine_OnSliderChanged;
		wMultiplier.OnListChanged += WMultiplier_OnListChanged;
		wConfirm.Activated += LsMenus.WConfirm_Activated;
		wCollect.Activated += LsMenus.WCollect_Activated;
		((UIMenuItem)wOrder).Activated += LsMenus.WOrder_Activated;
		wArmour.Activated += LsMenus.WArmour_Activated;
		((UIMenuItem)wWeapons).Activated += LsMenus.WWeapons_Activated;
		int mWprice = meleePrice[0] - LsFunctions.Discount(meleePrice[0]);
		int pWprice = pistolPrice[0] - LsFunctions.Discount(pistolPrice[0]);
		wepMenu = new UIMenu("Armes", "Que veux-tu ?");
		if (UseSprites)
		{
			wepMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_dealer.png");
		}
		LsMenuPool.Add(wepMenu);
		melee = new UIMenuListItem("Arme de mêlée $" + LsFunctions.IntToMoney(mWprice), (List<object>)meleeWeps, 0);
		wepMenu.AddItem((UIMenuItem)(object)melee);
		pistol = new UIMenuListItem("Pistolet $" + LsFunctions.IntToMoney(pWprice), (List<object>)pistolWeps, 0);
		wepMenu.AddItem((UIMenuItem)(object)pistol);
		armour = new UIMenuItem("Gilet pare-balles $" + LsFunctions.IntToMoney(250 - LsFunctions.Discount(250)));
		wepMenu.AddItem(armour);
		dismiss = new UIMenuItem("Renvoyer");
		wepMenu.AddItem(dismiss);
		wepMenu.OnItemSelect += LsMenus.OnWepMenuSelect;
		melee.OnListChanged += LsMenus.onListChange;
		pistol.OnListChanged += LsMenus.onListChange;
		OrderMultipliers = new UIMenuListItem("Multiplier", (List<object>)multiplier, 0);
		OrderMultipliers.OnListChanged += DMultiplier_OnListChanged;
		weedMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Cannabis");
		// LSL V2 FR : élargir ce menu NativeUI pour conserver les libellés complets.
		weedMenu.SetMenuWidthOffset(140);
		if (UseSprites)
		{
			weedMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		bushWeed = new UIMenuSliderItem("Cannabis (prix/100 g) $" + LsFunctions.IntToMoney(LsFunctions.PriceForGrams(100, weed1Price - LsFunctions.Discount(weed1Price))));
		weedMenu.AddItem((UIMenuItem)(object)bushWeed);
		bushWeed.Maximum = dealer1Weed;
		bushWeed.Multiplier = 1;
		weedMenu.AddItem((UIMenuItem)(object)OrderMultipliers);
		WeedBuy = new UIMenuItem("Commander : " + LsFunctions.FormatMetric(bushWeed.Value));
		weedMenu.AddItem(WeedBuy);
		bushWeed.OnSliderChanged += LsMenus.onSliderChange;
		weedMenu.OnItemSelect += LsMenus.onWeedMenuSelect;
		crackMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Crack");
		// LSL V2 FR : élargir ce menu NativeUI pour conserver les libellés complets.
		crackMenu.SetMenuWidthOffset(140);
		if (UseSprites)
		{
			crackMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		crack = new UIMenuSliderItem("Crack (prix/100 g) $" + LsFunctions.IntToMoney(LsFunctions.PriceForGrams(100, crack1Price - LsFunctions.Discount(crack1Price))));
		crackMenu.AddItem((UIMenuItem)(object)crack);
		crack.Maximum = dealer1Crack;
		crack.Multiplier = 1;
		crackMenu.AddItem((UIMenuItem)(object)OrderMultipliers);
		CrackBuy = new UIMenuItem("Commander : " + LsFunctions.FormatMetric(crack.Value));
		crackMenu.AddItem(CrackBuy);
		crack.OnSliderChanged += LsMenus.onSliderChange;
		crackMenu.OnItemSelect += LsMenus.onCrackMenuSelect;
		cocainMenu = LsMenuPool.AddSubMenu(OrderDrugs, "Cocaïne");
		// LSL V2 FR : élargir ce menu NativeUI pour conserver les libellés complets.
		cocainMenu.SetMenuWidthOffset(140);
		if (UseSprites)
		{
			cocainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_zee.png");
		}
		cocaine = new UIMenuSliderItem("Cocaïne (prix/100 g) $" + LsFunctions.IntToMoney(LsFunctions.PriceForGrams(100, cocain1Price - LsFunctions.Discount(cocain1Price))));
		cocainMenu.AddItem((UIMenuItem)(object)cocaine);
		cocaine.Maximum = dealer1Cocain;
		cocaine.Multiplier = 1;
		cocainMenu.AddItem((UIMenuItem)(object)OrderMultipliers);
		CocainBuy = new UIMenuItem("Commander : " + LsFunctions.FormatMetric(cocaine.Value));
		cocainMenu.AddItem(CocainBuy);
		cocaine.OnSliderChanged += LsMenus.onSliderChange;
		cocainMenu.OnItemSelect += LsMenus.onCocainMenuSelect;
		PlaceOrder = new UIMenuItem("Passer commande $" + LsFunctions.IntToMoney(0));
		OrderDrugs.AddItem(PlaceOrder);
		OrderDrugs.OnItemSelect += LsMenus.OnOrderDrugsMenuSelect;
		stashMainMenu = new UIMenu("Planque", "Choisir une option");
		if (UseSprites)
		{
			stashMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		LsMenuPool.Add(stashMainMenu);
		stashTakeMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Retirer");
		if (UseSprites)
		{
			stashTakeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		stashPutMenu = LsMenuPool.AddSubMenu(stashMainMenu, "Déposer");
		if (UseSprites)
		{
			stashPutMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		putWeedStash = new UIMenuSliderItem("Cannabis");
		stashPutMenu.AddItem((UIMenuItem)(object)putWeedStash);
		putWeedStash.Maximum = PlayerInventory["Weed"];
		putWeedStash.Multiplier = 1;
		takeWeedStash = new UIMenuSliderItem("Cannabis");
		stashTakeMenu.AddItem((UIMenuItem)(object)takeWeedStash);
		takeWeedStash.Maximum = 0;
		takeWeedStash.Multiplier = 1;
		putWeedStash.OnSliderChanged += LsMenus.onSliderChange;
		takeWeedStash.OnSliderChanged += LsMenus.onSliderChange;
		putCrackStash = new UIMenuSliderItem("Crack");
		stashPutMenu.AddItem((UIMenuItem)(object)putCrackStash);
		putCrackStash.Maximum = PlayerInventory["Crack"];
		putCrackStash.Multiplier = 1;
		takeCrackStash = new UIMenuSliderItem("Crack");
		stashTakeMenu.AddItem((UIMenuItem)(object)takeCrackStash);
		takeCrackStash.Maximum = 0;
		takeCrackStash.Multiplier = 1;
		putCrackStash.OnSliderChanged += LsMenus.onSliderChange;
		takeCrackStash.OnSliderChanged += LsMenus.onSliderChange;
		putCocainStash = new UIMenuSliderItem("Cocaïne");
		stashPutMenu.AddItem((UIMenuItem)(object)putCocainStash);
		putCocainStash.Maximum = PlayerInventory["Cocaine"];
		putCocainStash.Multiplier = 1;
		takeCocainStash = new UIMenuSliderItem("Cocaïne");
		stashTakeMenu.AddItem((UIMenuItem)(object)takeCocainStash);
		takeCocainStash.Maximum = 0;
		takeCocainStash.Multiplier = 1;
		putCocainStash.OnSliderChanged += LsMenus.onSliderChange;
		takeCocainStash.OnSliderChanged += LsMenus.onSliderChange;
		putWeedGramsStash = new UIMenuSliderItem("Cannabis non préparé");
		stashPutMenu.AddItem((UIMenuItem)(object)putWeedGramsStash);
		putWeedGramsStash.Maximum = LsFunctions.pWeedGrams;
		putWeedGramsStash.Multiplier = 1;
		putWeedGramsStash.OnSliderChanged += LsMenus.onSliderChange;
		putCrackGramsStash = new UIMenuSliderItem("Crack non préparé");
		stashPutMenu.AddItem((UIMenuItem)(object)putCrackGramsStash);
		putCrackGramsStash.Maximum = LsFunctions.pCrackGrams;
		putCrackGramsStash.Multiplier = 1;
		putCrackStash.OnSliderChanged += LsMenus.onSliderChange;
		putCocainGramsStash = new UIMenuSliderItem("Cocaïne non préparée");
		stashPutMenu.AddItem((UIMenuItem)(object)putCocainGramsStash);
		putCocainGramsStash.Maximum = LsFunctions.pCocaineGrams;
		putCocainStash.Multiplier = 1;
		putCocainStash.OnSliderChanged += LsMenus.onSliderChange;
		putMoneyStash = new UIMenuSliderItem("Argent $");
		stashPutMenu.AddItem((UIMenuItem)(object)putMoneyStash);
		putMoneyStash.Maximum = player.Money;
		putMoneyStash.Multiplier = 100;
		takeMoneyStash = new UIMenuSliderItem("Argent $");
		stashTakeMenu.AddItem((UIMenuItem)(object)takeMoneyStash);
		takeMoneyStash.Maximum = 0;
		takeMoneyStash.Multiplier = 100;
		putMoneyStash.OnSliderChanged += LsMenus.onSliderChange;
		takeMoneyStash.OnSliderChanged += LsMenus.onSliderChange;
		stashPutMultiplier = new UIMenuListItem("Multiplier", (List<object>)multiplier, 0);
		stashPutMenu.AddItem((UIMenuItem)(object)stashPutMultiplier);
		stashPutMultiplier.OnListChanged += LsMenus.onListChange;
		stashTakeMultiplier = new UIMenuListItem("Multiplier", (List<object>)multiplier, 0);
		stashTakeMenu.AddItem((UIMenuItem)(object)stashTakeMultiplier);
		stashTakeMultiplier.OnListChanged += LsMenus.onListChange;
		InventoryText.Innit();
		stashMainMenu.FormatDescriptions = true;
		string mWeed = "Cannabis. ";
		string mCrack = "Crack. ";
		string mCocaine = "Cocaïne. ";
		string mGrams = "Grammes de ";
		string mOunce = "Produit non préparé : ";
		string mSell = "Prêt à vendre ou à confier à un dealer.";
		string mStash = "À déposer dans une planque avant la vente.";
		((UIMenuItem)takeWeedStash).Description = mGrams + mWeed + mSell;
		((UIMenuItem)takeCrackStash).Description = mGrams + mCrack + mSell;
		((UIMenuItem)takeCocainStash).Description = mGrams + mCocaine + mSell;
		((UIMenuItem)putWeedStash).Description = mGrams + mWeed + mSell;
		((UIMenuItem)putCrackStash).Description = mGrams + mCrack + mSell;
		((UIMenuItem)putCocainStash).Description = mGrams + mCocaine + mSell;
		((UIMenuItem)putWeedGramsStash).Description = mOunce + mWeed + mStash;
		((UIMenuItem)putCrackGramsStash).Description = mOunce + mCrack + mStash;
		((UIMenuItem)putCocainGramsStash).Description = mOunce + mCocaine + mStash;
		List<object> pArm = new List<object>();
		for (int t = 0; t < playerArmours + 1; t++)
		{
			pArm.Add(t);
		}
		putArmourStash = new UIMenuListItem("Déposer un gilet pare-balles", pArm, 0);
		stashPutMenu.AddItem((UIMenuItem)(object)putArmourStash);
		List<object> sArm = new List<object>();
		for (int j = 0; j < 1; j++)
		{
			sArm.Add(j);
		}
		takeArmourStash = new UIMenuListItem("Prendre un gilet pare-balles", sArm, 0);
		stashTakeMenu.AddItem((UIMenuItem)(object)takeArmourStash);
		if (playerWeapons.Count > 0)
		{
			putWeaponStash = new UIMenuListItem("Déposer une arme : ", (List<object>)playerWeps, 0);
			stashPutMenu.AddItem((UIMenuItem)(object)putWeaponStash);
			putWeaponStash.OnListChanged += LsMenus.onListChange;
			takeWeaponStash = new UIMenuListItem("Prendre une arme :", (List<object>)playerWeps, 0);
			stashTakeMenu.AddItem((UIMenuItem)(object)takeWeaponStash);
			takeWeaponStash.OnListChanged += LsMenus.onListChange;
		}
		stashPutMenu.OnItemSelect += LsMenus.onStashMenuSelect;
		stashTakeMenu.OnItemSelect += LsMenus.onPlayerMenuSelect;
		vehStashMainMenu = new UIMenu("Stockage du véhicule", "Choisir");
		if (UseSprites)
		{
			vehStashMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		LsMenuPool.Add(vehStashMainMenu);
		vehStashTakeMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Retirer");
		if (UseSprites)
		{
			vehStashTakeMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		vehStashPutMenu = LsMenuPool.AddSubMenu(vehStashMainMenu, "Déposer");
		if (UseSprites)
		{
			vehStashPutMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_stash.png");
		}
		putWeedVeh = new UIMenuSliderItem("Cannabis " + LsFunctions.FormatMetric(PlayerInventory["Weed"]));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putWeedVeh);
		putWeedVeh.Maximum = PlayerInventory["Weed"];
		putWeedVeh.Multiplier = 1;
		takeWeedVeh = new UIMenuSliderItem("Cannabis");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeWeedVeh);
		takeWeedVeh.Maximum = 0;
		takeWeedVeh.Multiplier = 1;
		putWeedVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeWeedVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCrackVeh = new UIMenuSliderItem("Crack " + LsFunctions.FormatMetric(PlayerInventory["Crack"]));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putCrackVeh);
		putCrackVeh.Maximum = PlayerInventory["Crack"];
		putCrackVeh.Multiplier = 1;
		takeCrackVeh = new UIMenuSliderItem("Crack");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeCrackVeh);
		takeCrackVeh.Maximum = 0;
		takeCrackVeh.Multiplier = 1;
		putCrackVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCrackVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCocainVeh = new UIMenuSliderItem("Cocaïne " + LsFunctions.FormatMetric(PlayerInventory["Cocaine"]));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putCocainVeh);
		putCocainVeh.Maximum = PlayerInventory["Cocaine"];
		putCocainVeh.Multiplier = 1;
		takeCocainVeh = new UIMenuSliderItem("Cocaïne");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeCocainVeh);
		takeCocainVeh.Maximum = 0;
		takeCocainVeh.Multiplier = 1;
		putCocainVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCocainVeh.OnSliderChanged += LsMenus.onSliderChange;
		putWeedGramsVeh = new UIMenuSliderItem("Cannabis non préparé " + LsFunctions.FormatMetric(LsFunctions.pWeedGrams));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putWeedGramsVeh);
		putWeedGramsVeh.Maximum = LsFunctions.pWeedGrams;
		putWeedGramsVeh.Multiplier = 1;
		takeWeedGramsVeh = new UIMenuSliderItem("Cannabis non préparé");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeWeedGramsVeh);
		takeWeedGramsVeh.Maximum = 0;
		takeWeedGramsVeh.Multiplier = 1;
		putWeedGramsVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeWeedGramsVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCrackGramsVeh = new UIMenuSliderItem("Crack non préparé " + LsFunctions.FormatMetric(LsFunctions.pCrackGrams));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putCrackGramsVeh);
		putCrackGramsVeh.Maximum = LsFunctions.pCrackGrams;
		putCrackGramsVeh.Multiplier = 1;
		takeCrackGramsVeh = new UIMenuSliderItem("Crack non préparé");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeCrackGramsVeh);
		takeCrackGramsVeh.Maximum = 0;
		takeCrackGramsVeh.Multiplier = 1;
		putCrackGramsVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCrackGramsVeh.OnSliderChanged += LsMenus.onSliderChange;
		putCocainGramsVeh = new UIMenuSliderItem("Cocaïne non préparée " + LsFunctions.FormatMetric(LsFunctions.pCocaineGrams));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putCocainGramsVeh);
		putCocainGramsVeh.Maximum = LsFunctions.pCocaineGrams;
		putCocainGramsVeh.Multiplier = 1;
		takeCocainGramsVeh = new UIMenuSliderItem("Cocaïne");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeCocainGramsVeh);
		takeCocainGramsVeh.Maximum = 0;
		takeCocainGramsVeh.Multiplier = 1;
		putCocainGramsVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeCocainGramsVeh.OnSliderChanged += LsMenus.onSliderChange;
		putMoneyVeh = new UIMenuSliderItem("Argent $" + LsFunctions.IntToMoney(player.Money));
		vehStashPutMenu.AddItem((UIMenuItem)(object)putMoneyVeh);
		putMoneyVeh.Maximum = player.Money;
		putMoneyVeh.Multiplier = 100;
		takeMoneyVeh = new UIMenuSliderItem("Argent $");
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeMoneyVeh);
		takeMoneyVeh.Maximum = 0;
		takeMoneyVeh.Multiplier = 100;
		putMoneyVeh.OnSliderChanged += LsMenus.onSliderChange;
		takeMoneyVeh.OnSliderChanged += LsMenus.onSliderChange;
		vehPutMultiplier = new UIMenuListItem("Multiplier", (List<object>)multiplier, 0);
		vehStashPutMenu.AddItem((UIMenuItem)(object)vehPutMultiplier);
		vehPutMultiplier.OnListChanged += LsMenus.onListChange;
		vehTakeMultiplier = new UIMenuListItem("Multiplier", (List<object>)multiplier, 0);
		vehStashTakeMenu.AddItem((UIMenuItem)(object)vehTakeMultiplier);
		vehTakeMultiplier.OnListChanged += LsMenus.onListChange;
		takeWeaponVeh = new UIMenuListItem("Prendre une arme :", new List<object> { "Aucune arme" }, 0);
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeWeaponVeh);
		takeWeaponVeh.OnListChanged += LsMenus.onListChange;
		putArmourVeh = new UIMenuListItem("Déposer un gilet pare-balles", pArm, 0);
		vehStashPutMenu.AddItem((UIMenuItem)(object)putArmourVeh);
		((UIMenuItem)putArmourVeh).Activated += PutArmourVeh_Activated;
		List<object> vArm = new List<object>();
		for (int k = 0; k < 1; k++)
		{
			vArm.Add(k);
		}
		takeArmourVeh = new UIMenuListItem("Prendre un gilet pare-balles", vArm, 0);
		vehStashTakeMenu.AddItem((UIMenuItem)(object)takeArmourVeh);
		((UIMenuItem)takeArmourVeh).Activated += TakeArmourVeh_Activated;
		if (playerWeapons.Count > 0)
		{
			putWeaponVeh = new UIMenuListItem("Déposer une arme : ", (List<object>)playerWeps, 0);
			vehStashPutMenu.AddItem((UIMenuItem)(object)putWeaponVeh);
			putWeaponVeh.OnListChanged += LsMenus.onListChange;
		}
		vehStashPutMenu.OnItemSelect += LsMenus.onPutVehicleMenuSelect;
		vehStashTakeMenu.OnItemSelect += LsMenus.onTakeVehicleMenuSelect;
		customerMainMenu = new UIMenu("Client", "Choisir une option");
		if (UseSprites)
		{
			customerMainMenu.SetBannerType("scripts\\LSLife\\sprites\\banner_customer.png");
		}
		LsMenuPool.Add(customerMainMenu);
		sell = new UIMenuItem("Drogue");
		customerMainMenu.AddItem(sell);
		turnAway = new UIMenuItem("Refuser la vente");
		customerMainMenu.AddItem(turnAway);
		customerMainMenu.OnItemSelect += LsMenus.onCustomerMenuSelect;
		LsMenuPool.FormatDescriptions = true;
		LsMenuPool.RefreshIndex();
		iFruit = new CustomiFruit();
		contactZee = new iFruitContact("Zee");
		contactZee.Answered += LsFunctions.ContactAnswered;
		contactZee.DialTimeout = 1000;
		contactZee.Active = true;
		contactZee.Icon = ContactIcon.Blank;
		((List<iFruitContact>)(object)iFruit.Contacts).Add(contactZee);
		DealerHandler = new PlayerDealerHandler();
		HouseHandler = new StashHouseHandler();
		LsFunctions.LoadSpawnLocations();
		day = Function.Call<int>(unchecked((Hash)(-2777906517671992481L)));
		hour = Function.Call<int>((Hash)2675767815307398015L);
		SlowTickTime = Game.GameTime;
		randomAttackTimer = Game.GameTime;
		reloadTimer = Game.GameTime;
	}

	private void OrderDrugs_OnMenuOpen(UIMenu sender)
	{
		if (dealer1ReloadOrder)
		{
			Enum.TryParse<Enums.DayGTA>(dealer1ReloadDay.ToString(), out var mDay);
			LsFunctions.TextMsg("ZEE", "Livraison", "La commande devrait arriver le " + mDay.ToString() + " à " + dealer1ReloadHour + ":00");
		}
		if (!NewOrder.Placed)
		{
			PlaceOrder.Text = "Passer commande $" + LsFunctions.IntToMoney(NewOrder.Bill);
			LsFunctions.UpdateOrderMenuDesc();
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
		int amount = takeArmourVeh.Index;
		int max = 5;
		if (player.Character.Armor < 100)
		{
			max = 6;
		}
		if (amount <= 0 || playerArmours + amount > max)
		{
			return;
		}
		if (player.Character.Armor < 100)
		{
			player.Character.Armor = 100;
			amount--;
		}
		playerArmours += amount;
		pStashVehicle.drugCarArmour -= amount;
		foreach (XElement ele in LSLifeSave.Descendants())
		{
			if (ele.Name == "drugCarStash")
			{
				foreach (XElement el in ele.Elements())
				{
					if (el.Name == "armour")
					{
						el.Value = pStashVehicle.drugCarArmour.ToString();
						break;
					}
				}
			}
			if (!(ele.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement el2 in ele.Elements())
			{
				if (el2.Name == "armour")
				{
					el2.Value = playerArmours.ToString();
					break;
				}
			}
		}
		LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		putArmourVeh.Items = new List<object>();
		for (int t = 0; t < playerArmours + 1; t++)
		{
			putArmourVeh.Items.Add(t);
		}
		takeArmourVeh.Items = new List<object>();
		for (int i = 0; i < pStashVehicle.drugCarArmour + 1; i++)
		{
			takeArmourVeh.Items.Add(i);
		}
	}

	private void PutArmourVeh_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int amount = putArmourVeh.Index;
		if (amount <= 0 || playerArmours <= 0)
		{
			return;
		}
		playerArmours -= amount;
		pStashVehicle.drugCarArmour += amount;
		foreach (XElement ele in LSLifeSave.Descendants())
		{
			if (ele.Name == "drugCarStash")
			{
				foreach (XElement el in ele.Elements())
				{
					if (el.Name == "armour")
					{
						el.Value = pStashVehicle.drugCarArmour.ToString();
						break;
					}
				}
			}
			if (!(ele.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement el2 in ele.Elements())
			{
				if (el2.Name == "armour")
				{
					el2.Value = playerArmours.ToString();
					break;
				}
			}
		}
		LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		putArmourVeh.Index = 0;
		putArmourVeh.Items = new List<object>();
		for (int t = 0; t < playerArmours + 1; t++)
		{
			putArmourVeh.Items.Add(t);
		}
		takeArmourVeh.Items = new List<object>();
		for (int i = 0; i < pStashVehicle.drugCarArmour + 1; i++)
		{
			takeArmourVeh.Items.Add(i);
		}
	}

	private void onTick(object sender, EventArgs e)
	{
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected Obj, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5b: Expected I4, but got Unknown
		//IL_1cd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b26: Expected I4, but got Unknown
		//IL_1b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4a: Expected I4, but got Unknown
		//IL_1f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e73: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Expected Obj, but got Unknown
		//IL_2508: Unknown result type (might be due to invalid IL or missing references)
		//IL_250d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2516: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_24ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1983: Unknown result type (might be due to invalid IL or missing references)
		//IL_1988: Unknown result type (might be due to invalid IL or missing references)
		//IL_198c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_2534: Unknown result type (might be due to invalid IL or missing references)
		//IL_2543: Unknown result type (might be due to invalid IL or missing references)
		//IL_2548: Unknown result type (might be due to invalid IL or missing references)
		//IL_254d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2558: Unknown result type (might be due to invalid IL or missing references)
		//IL_2564: Unknown result type (might be due to invalid IL or missing references)
		//IL_2570: Unknown result type (might be due to invalid IL or missing references)
		//IL_25a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_25aa: Expected Obj, but got Unknown
		//IL_25c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_25c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_25cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_25d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_25f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_25fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2607: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2f: Expected I4, but got Unknown
		//IL_1c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c36: Expected I4, but got Unknown
		//IL_158a: Unknown result type (might be due to invalid IL or missing references)
		//IL_158f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1593: Unknown result type (might be due to invalid IL or missing references)
		//IL_2636: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c47: Unknown result type (might be due to invalid IL or missing references)
		//IL_2674: Unknown result type (might be due to invalid IL or missing references)
		//IL_2679: Unknown result type (might be due to invalid IL or missing references)
		//IL_2682: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c24: Unknown result type (might be due to invalid IL or missing references)
		//IL_26a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_26b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_2701: Unknown result type (might be due to invalid IL or missing references)
		//IL_2708: Expected Obj, but got Unknown
		//IL_2721: Unknown result type (might be due to invalid IL or missing references)
		//IL_2726: Unknown result type (might be due to invalid IL or missing references)
		//IL_2741: Unknown result type (might be due to invalid IL or missing references)
		//IL_275c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2761: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_23de: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_293b: Unknown result type (might be due to invalid IL or missing references)
		//IL_29a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2806: Unknown result type (might be due to invalid IL or missing references)
		//IL_280b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2810: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ae9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2af0: Invalid comparison between Unknown and I4
		//IL_2824: Unknown result type (might be due to invalid IL or missing references)
		//IL_2829: Unknown result type (might be due to invalid IL or missing references)
		//IL_282e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2875: Unknown result type (might be due to invalid IL or missing references)
		//IL_2898: Unknown result type (might be due to invalid IL or missing references)
		//IL_3822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1064: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Unknown result type (might be due to invalid IL or missing references)
		//IL_106d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1100: Unknown result type (might be due to invalid IL or missing references)
		//IL_1105: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b86: Unknown result type (might be due to invalid IL or missing references)
		//IL_119f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_117b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1181: Invalid comparison between Unknown and I4
		//IL_1113: Unknown result type (might be due to invalid IL or missing references)
		//IL_1118: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c31: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_3abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_30f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_31be: Unknown result type (might be due to invalid IL or missing references)
		//IL_31c5: Invalid comparison between Unknown and I4
		//IL_31d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fef: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_400d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4012: Unknown result type (might be due to invalid IL or missing references)
		if (Game.IsLoading)
		{
			return;
		}
		if (firstRun && (Entity)(object)player.Character != (Entity)null)
		{
			if (File.Exists("scripts\\LSLife\\LSLifeAreas.xml"))
			{
				LoadAreaData();
			}
			UI.Notify("~g~LsLife: " + vNumber);
			UI.Notify("~g~LsLife: " + areas.Count + " zones chargées");
			lPeds = new LsLPeds();
			World.SetRelationshipBetweenGroups((Relationship)3, playerRelationship, CustomerRel);
			World.SetRelationshipBetweenGroups((Relationship)3, playerRelationship, rival_nutral);
			World.SetRelationshipBetweenGroups((Relationship)5, playerRelationship, rival_enemy);
			World.SetRelationshipBetweenGroups((Relationship)3, rival_nutral, CustomerRel);
			World.SetRelationshipBetweenGroups((Relationship)3, rival_nutral, playerRelationship);
			World.SetRelationshipBetweenGroups((Relationship)1, rival_nutral, rival_enemy);
			World.SetRelationshipBetweenGroups((Relationship)1, rival_enemy, CustomerRel);
			World.SetRelationshipBetweenGroups((Relationship)5, rival_enemy, playerRelationship);
			World.SetRelationshipBetweenGroups((Relationship)1, rival_enemy, rival_nutral);
			World.SetRelationshipBetweenGroups((Relationship)0, CustomerRel, playerRelationship);
			World.SetRelationshipBetweenGroups((Relationship)0, CustomerRel, rival_nutral);
			World.SetRelationshipBetweenGroups((Relationship)0, CustomerRel, rival_enemy);
			playerHeat = -100f;
			Function.Call((Hash)2750315038012726912L, (InputArgument)(player.Character), (InputArgument)(9), (InputArgument)(0), (InputArgument)(0), (InputArgument)(0));
			playerHasBag = false;
			canSprint = true;
			playerArea = World.GetZoneNameLabel(playerPos);
			if (areas.Where((Area a) => a.Name != playerArea).Count() == 0)
			{
				if (zoneData.ContainsKey(playerArea))
				{
					areas.Add(new Area(playerArea, LsFunctions.LsDrugs(), zoneData[playerArea].Item1));
				}
				else
				{
					UI.Notify("~r~LsLife - Erreur de type de zone : " + playerArea);
				}
			}
			Area cArea = areas.Find((Area a) => a.Name == playerArea);
			if (cArea != null)
			{
				areaIndex = areas.IndexOf(cArea);
			}
			firstRun = false;
		}
		if (DEBUG)
		{
			string debugString = "0";
			if (Game.GameTime < areas[areaIndex].DemandBuff())
			{
				debugString = (areas[areaIndex].DemandBuff() - Game.GameTime).ToString();
			}
			myUIText = new UIText(debugString, new Point(10, 10), 0.4f, Color.WhiteSmoke, (GTA.Font)0, false);
			myUIText.Draw();
		}
		if (Prison.Started && Prison.CurrentState > Prison.PrisonStates.ApproachBarrier)
		{
			Function.Call(unchecked((Hash)(-7077668788463384353L)), (InputArgument)("restrictedareas"));
			Function.Call(unchecked((Hash)(-7077668788463384353L)), (InputArgument)("re_prison"));
		}
		if (LsMenuPool != null)
		{
			LsMenuPool.ProcessMenus();
		}
		iFruit.Update();
		if (Game.IsControlPressed(2, (GTA.Control)76) && Game.IsControlJustPressed(2, (GTA.Control)26))
		{
			DropDrugs.Drop();
		}
		Vector3 position;
		if ((Entity)(object)player.Character != (Entity)null && ((Entity)player.Character).IsAlive && player.IsPlaying)
		{
			playerPos = ((Entity)player.Character).Position;
			if (DEBUG)
			{
				PedDebugger.OnTick();
			}
			if (DealerHandler != null)
			{
				DealerHandler.OnTick();
			}
			if (HouseHandler != null)
			{
				HouseHandler.Ontick();
			}
			if (ZeesDealers != null)
			{
				ZeesDealers.OnTick();
			}
			HandlePotentialWorker();
			if (showInventory)
			{
				InventoryText.InventoryGui();
				if (AreaDebug && !LsMenuPool.IsAnyMenuOpen())
				{
					areaInfoUI();
					ZeeInfoUI();
					PlayerUI();
					GangUI();
				}
				player.GetTargetedEntity();
				if (!AreaHeatBar.Displayed)
				{
					AreaHeatBar.Toggle();
				}
				if (!PlayerHeatBar.Displayed)
				{
					PlayerHeatBar.Toggle();
				}
				BarHandler.Tick();
			}
			else
			{
				if (AreaHeatBar.Displayed)
				{
					AreaHeatBar.Toggle();
				}
				if (PlayerHeatBar.Displayed)
				{
					PlayerHeatBar.Toggle();
				}
			}
			if (pigs.Count > 0 && pigs[0].AskPlayerToComply)
			{
				if ((float)Game.GameTime > displayHelpTimer + 3000f && !LsMenuPool.IsAnyMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("~INPUT_CONTEXT~ pour obtempérer.");
				}
				if (Game.IsControlJustPressed(2, (GTA.Control)51))
				{
					PlayerComplied = true;
					TaskSequence giveUP = new TaskSequence();
					if ((Entity)(object)player.Character.CurrentVehicle != (Entity)null)
					{
						giveUP.AddTask.LeaveVehicle();
					}
					giveUP.AddTask.TurnTo(((Entity)pigs[0].Ped).ForwardVector + playerPos, 1000);
					giveUP.AddTask.HandsUp(20000);
					player.Character.Task.PerformSequence(giveUP);
				}
			}
			HandleAreaDealers();
			if (!LsMenuPool.IsAnyMenuOpen() && (Entity)(object)weaponDealer1 != (Entity)null && readyToDeal && Game.IsControlJustPressed(2, (GTA.Control)51) && LsMenuPool != null && !LsMenuPool.IsAnyMenuOpen())
			{
				wepMenu.Visible = !wepMenu.Visible;
			}
			if (pStashVehicle != null)
			{
				pStashVehicle.OnTick();
			}
			if ((float)Game.GameTime > ratTimer + 1000f)
			{
				if (isDealing)
				{
					timeDealing = Game.GameTime - timeStartedDealing;
				}
				else
				{
					timeDealing = 0;
				}
				thisStreet = World.GetStreetName(new Vector2(playerPos.X, playerPos.Y));
				ratTimer = Game.GameTime;
				if (thisStreet != lastStreet)
				{
					lastStreet = thisStreet;
					if (playerHeat - 10f >= -100f)
					{
						playerHeat -= 10f;
					}
					else
					{
						playerHeat = -100f;
					}
					if (isDealing)
					{
						timeStartedDealing = Game.GameTime;
					}
				}
				else if ((float)Game.GameTime > lastTimeDealDone + 10000f)
				{
					if (playerHeat > -100f && player.WantedLevel == 0)
					{
						if (isDealing)
						{
							if (playerHeat < 100f)
							{
								playerHeat += timeDealing / 180000;
							}
							if (playerHeat > 100f)
							{
								playerHeat = 100f;
							}
						}
						float repMod = (float)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation) * 0.1f;
						if (repMod < 0f)
						{
							repMod = 0f;
						}
						LsFunctions.DbugString(repMod.ToString() ?? "");
						playerHeat -= 1f + repMod;
						if (playerHeat < -100f)
						{
							playerHeat = -100f;
						}
					}
				}
				else if (playerHeat < 100f)
				{
					float heatThisTick = (float)((double)(timeDealing / 60000) + ((double)areas[areaIndex].Heat * 0.1 + (double)areas[areaIndex].CopPresance) * 0.5 + (double)PedsCanSeeDeal * 0.2 - Math.Min(10.0, LsFunctions.CurrentRepLvl((int)areas[areaIndex].Reputation)));
					heatThisTick = Math.Max(heatThisTick, 1f);
					heatThisTick = Math.Min(10f, heatThisTick);
					heatThisTick *= 0.5f;
					float newRatChance = playerHeat + heatThisTick;
					if (newRatChance > 100f)
					{
						playerHeat = 100f;
					}
					else if (newRatChance < -100f)
					{
						playerHeat = -100f;
					}
					else
					{
						playerHeat = newRatChance;
					}
					float repMod2 = (float)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation) * 0.1f;
					float mod = 1f;
					if (IsNight())
					{
						mod = 0.5f;
					}
					mod = (float)PedsCanSeeDeal * mod;
					if (mod < repMod2)
					{
						repMod2 = mod;
					}
					mod -= repMod2;
					if (mod < 0f)
					{
						mod = 0f;
					}
					areas[areaIndex].Heat += mod * 0.5f;
					if (areas[areaIndex].Heat > 1000f)
					{
						areas[areaIndex].Heat = 1000f;
					}
				}
			}
			HandleOldCutomers();
			if (isDealing)
			{
				if (removedStuff)
				{
					removedStuff = false;
				}
				if (customers.Count < 5 && ShotHandler.shots.Count < 1 && (float)Game.GameTime > lastTimePedSelected + 1000f)
				{
					lastTimePedSelected = Game.GameTime;
					if (rnd.Next(0, 100) > 60 && areas[areaIndex].NeedCustomer())
					{
						if (HouseHandler.CurrentHouse != null && HouseHandler.CurrentHouse.inside)
						{
							foreach (Drug drug in areas[areaIndex].Drugs)
							{
								if (customers.Count >= 3)
								{
									break;
								}
								switch (drug.Name)
								{
								case "Weed":
									if (!sellingWeed)
									{
										drug.Supplied = false;
										drug.SelectCustomer = false;
									}
									else
									{
										if (!drug.SelectCustomer)
										{
											break;
										}
										Random random2 = new Random();
										int rIntMax2 = (int)(10f * drug.Demand);
										if (drug.Demand > 1f)
										{
											rIntMax2 = 10;
										}
										if (random2.Next(0, 15 - rIntMax2) == 0)
										{
											PedHash hash2 = unchecked((PedHash)(-1954728090));
											if (LsFunctions.cHashes.Count > 0)
											{
												hash2 = LsFunctions.cHashes[rnd.Next(0, LsFunctions.cHashes.Count)];
											}
											Ped cPed2 = World.CreatePed(LsFunctions.RequestModel(hash2), HouseHandler.CurrentHouse.exitPos);
											if ((Entity)(object)cPed2 != (Entity)null)
											{
												customers.Add(new Customer(cPed2, playerPos, farAway: false, null, drug, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
											}
										}
									}
									break;
								case "Crack":
									if (!sellingCrack)
									{
										drug.Supplied = false;
										drug.SelectCustomer = false;
									}
									else
									{
										if (!drug.SelectCustomer)
										{
											break;
										}
										Random random3 = new Random();
										int rIntMax3 = (int)(10f * drug.Demand);
										if (drug.Demand > 1f)
										{
											rIntMax3 = 10;
										}
										if (random3.Next(0, 15 - rIntMax3) == 0)
										{
											PedHash hash3 = unchecked((PedHash)(-1954728090));
											if (LsFunctions.cHashes.Count > 0)
											{
												hash3 = LsFunctions.cHashes[rnd.Next(0, LsFunctions.cHashes.Count)];
											}
											Ped cPed3 = World.CreatePed(LsFunctions.RequestModel(hash3), HouseHandler.CurrentHouse.exitPos);
											if ((Entity)(object)cPed3 != (Entity)null)
											{
												customers.Add(new Customer(cPed3, playerPos, farAway: false, null, drug, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
											}
										}
									}
									break;
								case "Cocaine":
									if (!sellingCocaine)
									{
										drug.Supplied = false;
										drug.SelectCustomer = false;
									}
									else
									{
										if (!drug.SelectCustomer)
										{
											break;
										}
										Random random = new Random();
										int rIntMax = (int)(10f * drug.Demand);
										if (drug.Demand > 1f)
										{
											rIntMax = 10;
										}
										if (random.Next(0, 15 - rIntMax) == 0)
										{
											PedHash hash = unchecked((PedHash)(-1954728090));
											if (LsFunctions.cHashes.Count > 0)
											{
												hash = LsFunctions.cHashes[rnd.Next(0, LsFunctions.cHashes.Count)];
											}
											Ped cPed = World.CreatePed(LsFunctions.RequestModel(hash), HouseHandler.CurrentHouse.exitPos);
											if ((Entity)(object)cPed != (Entity)null)
											{
												customers.Add(new Customer(cPed, playerPos, farAway: false, null, drug, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
											}
										}
									}
									break;
								}
							}
						}
						else
						{
							List<Ped> peds = new List<Ped>();
							foreach (Ped p in lPeds.GetPeds)
							{
								if (!CheckIfPedUsed(p, _allowPolice: true, _allowVehicle: true))
								{
									peds.Add(p);
								}
							}
							if (peds.Count > 0)
							{
								areas[areaIndex].GetCustomer(peds);
								foreach (Drug drug2 in areas[areaIndex].Drugs)
								{
									bool chooseCustomer = true;
									switch (drug2.Name)
									{
									case "Weed":
										if (!sellingWeed)
										{
											drug2.Supplied = false;
											chooseCustomer = false;
											drug2.SelectCustomer = false;
											drug2.Customer = null;
										}
										break;
									case "Crack":
										if (!sellingCrack)
										{
											drug2.Supplied = false;
											chooseCustomer = false;
											drug2.SelectCustomer = false;
											drug2.Customer = null;
										}
										break;
									case "Cocaine":
										if (!sellingCocaine)
										{
											drug2.Supplied = false;
											chooseCustomer = false;
											drug2.SelectCustomer = false;
											drug2.Customer = null;
										}
										break;
									}
									if (!chooseCustomer || !drug2.SelectCustomer)
									{
										continue;
									}
									Ped p2 = drug2.Customer;
									drug2.Customer = null;
									drug2.SelectCustomer = false;
									if ((Entity)(object)p2 != (Entity)null)
									{
										if (p2.IsInVehicle() && (Entity)(object)p2.CurrentVehicle.Driver == (Entity)(object)p2)
										{
											Vehicle vehicle = p2.CurrentVehicle;
											customers.Add(new Customer(p2, playerPos, farAway: false, vehicle, drug2, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
										}
										else
										{
											customers.Add(new Customer(p2, playerPos, farAway: false, null, drug2, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
										}
									}
								}
							}
						}
					}
				}
				if (customers.Count > 0)
				{
					if (Game.GameTime > lastCustomerCheckTimer + 370)
					{
						foreach (Customer c in customers.ToList())
						{
							if (c == null)
							{
								continue;
							}
							if (((Entity)c.Ped).Exists() && ((Entity)c.Ped).IsAlive && !c.Ped.IsPlayer)
							{
								position = ((Entity)c.Ped).Position;
								if (position.DistanceTo(playerPos) > 80f)
								{
									LsFunctions.DbugString("Client supprimé : trop éloigné");
									customers.Remove(c);
									LsFunctions.RemovePedFromWorld(c.Ped, clearTask: true);
									break;
								}
								if (!(c.Ped.CurrentPedGroup != player.Character.CurrentPedGroup))
								{
									continue;
								}
								position = ((Entity)c.Ped).Position;
								if (position.DistanceTo(playerPos) < 3f)
								{
									c.Ped.Task.ClearAll();
									Function.Call(unchecked((Hash)(-6974808092771143243L)), (InputArgument)(((Entity)c.Ped).Handle), (InputArgument)(playerGroup));
									Function.Call(unchecked((Hash)(-8213159594590722974L)), (InputArgument)(c.Ped), (InputArgument)("GENERIC_HI"), (InputArgument)("SPEECH_PARAMS_STANDARD"));
									continue;
								}
								if (c.Ped.IsInVehicle())
								{
									if (c.Target != World.GetNextPositionOnStreet(playerPos))
									{
										c.Target = World.GetNextPositionOnStreet(playerPos);
										c.Ped.Task.PerformSequence(PerformTaskSeq(2, c.Ped.CurrentVehicle, c.Target));
									}
									lastCustomerCheckTimer = Game.GameTime;
									continue;
								}
								if ((Entity)(object)c.Ped.LastVehicle != (Entity)null && (int)c.Ped.LastVehicle.LockStatus != 3)
								{
									c.Ped.LastVehicle.LockStatus = (VehicleLockStatus)3;
								}
								Vector3 playerForward = ((Entity)player.Character).ForwardVector;
								Vector3 playerRight = ((Entity)player.Character).RightVector;
								if (player.Character.IsInVehicle())
								{
									MoveCustomer(c, playerPos + playerRight * 2f);
								}
								else
								{
									MoveCustomer(c, playerPos + playerForward * 2f);
								}
								lastCustomerCheckTimer = Game.GameTime;
							}
							else
							{
								customers.Remove(c);
								LsFunctions.RemovePedFromWorld(c.Ped, clearTask: true);
							}
						}
					}
					Customer nCustomer = null;
					nCustomer = ((!player.Character.IsInVehicle()) ? FindClosestCustomer() : FindClosestCustomerInVehicle());
					if (nCustomer != null && !LsMenuPool.IsAnyMenuOpen())
					{
						if (player.Character.IsInVehicle() && nCustomer.Ped.IsInVehicle() && player.Character.CurrentVehicle.Speed < 1f)
						{
							if ((float)Game.GameTime > displayHelpTimer + 3000f && !ShotHandler.ShotOffers && PottentialWorker == null)
							{
								Function.Call((Hash)7023634693725934496L);
								displayHelpTimer = Game.GameTime;
								LsFunctions.DisplayHelpText("~INPUT_SPRINT~ pour vendre depuis le véhicule.");
							}
							if (Game.IsControlJustPressed(2, (GTA.Control)21))
							{
								Function.Call((Hash)7023634693725934496L);
								customerSellTo = nCustomer;
								sell.Text = "Vendre " + LsFunctions.FormatMetric(customerSellTo.amount) + " de " + customerSellTo.Drug.Name + "?";
								((UIText)customerMainMenu.Subtitle).Caption = "Tu possèdes " + LsFunctions.FormatMetric(PlayerInventory[customerSellTo.Drug.Name]) + " de " + customerSellTo.Drug.Name;
								customerMainMenu.Visible = !customerMainMenu.Visible;
								customerMainMenu.CurrentSelection = 0;
							}
						}
						if (!player.Character.IsInVehicle() && player.Character.IsStopped && !player.Character.IsInCombat)
						{
							if ((float)Game.GameTime > displayHelpTimer + 3000f && !ShotHandler.ShotOffers && PottentialWorker == null)
							{
								Function.Call((Hash)7023634693725934496L);
								displayHelpTimer = Game.GameTime;
								LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour vendre à cette personne.");
							}
							if (Game.IsControlJustPressed(2, (GTA.Control)21) && !LsMenuPool.IsAnyMenuOpen())
							{
								Function.Call((Hash)7023634693725934496L);
								customerSellTo = nCustomer;
								sell.Text = "Vendre " + LsFunctions.FormatMetric(customerSellTo.amount) + " de " + customerSellTo.Drug.Name + "?";
								((UIText)customerMainMenu.Subtitle).Caption = "Tu possèdes " + LsFunctions.FormatMetric(PlayerInventory[customerSellTo.Drug.Name]) + " de " + customerSellTo.Drug.Name;
								customerMainMenu.Visible = !customerMainMenu.Visible;
								customerMainMenu.CurrentSelection = 0;
							}
						}
					}
				}
			}
			if (wepDealer != null)
			{
				wepDealer.SetSpeed();
				if (!player.Character.IsInVehicle())
				{
					position = ((Entity)wepDealer.vehicle).Position;
					if (position.DistanceTo(playerPos) < 3f)
					{
						if ((float)Game.GameTime > displayHelpTimer + 3000f)
						{
							Function.Call((Hash)7023634693725934496L);
							displayHelpTimer = Game.GameTime;
							LsFunctions.DisplayHelpText("Appuyez sur ~INPUT_VEH_EXIT~ pour entrer.");
						}
						if (Game.IsControlJustPressed(2, (GTA.Control)75))
						{
							Function.Call((Hash)7023634693725934496L);
							player.Character.Task.EnterVehicle(wepDealer.vehicle, (VehicleSeat)0);
						}
					}
				}
				else if (player.Character.IsInVehicle() && ((Entity)player.Character.CurrentVehicle).Handle == ((Entity)wepDealer.vehicle).Handle)
				{
					if (Game.IsControlJustPressed(2, (GTA.Control)52))
					{
						player.Character.Task.LeaveVehicle(player.Character.CurrentVehicle, true);
						Script.Wait(5500);
						wepDealer.ped.CurrentVehicle.SoundHorn(15);
						Script.Wait(15);
						wepDealer.ped.CurrentVehicle.SoundHorn(12);
						((Entity)wepDealer.ped).IsPersistent = false;
						((Entity)wepDealer.vehicle).IsPersistent = false;
						((Entity)wepDealer.vehicle).CurrentBlip.Remove();
						Function.Call((Hash)5188501456062479616L, (InputArgument)(((Entity)wepDealer.ped).Handle), (InputArgument)(((Entity)wepDealer.vehicle).Handle), (InputArgument)(20f), (InputArgument)(786603));
						wepDealer = null;
					}
					if (Game.IsControlJustPressed(2, (GTA.Control)51))
					{
						if (wepDealer.state == Dealer.States.Driving)
						{
							wepDealer.state = Dealer.States.Faster;
						}
						if (wepDealer.state == Dealer.States.Fast)
						{
							wepDealer.state = Dealer.States.Slower;
						}
					}
				}
			}
			ShotHandler.Tick();
		}
		if (Game.GameTime > oneSecDelay + 10001)
		{
			oneSecDelay = Game.GameTime;
			if (player.IsPlaying)
			{
				if (!dealer1Angry && (double)dealer1Debt > (double)LsFunctions.zeeDebtCap() + (double)LsFunctions.zeeDebtCap() * 0.5)
				{
					if (DEBUG)
					{
						UI.Notify("Fournisseur en colère");
					}
					dealer1Angry = true;
				}
				if (driveByHandler.AmountOfDriveBy(_agg: true) == 0)
				{
					if (isDealing && areas[areaIndex].GangPresance > 4f && timeDealing > 240000 && rnd.Next(0, 60) == 0)
					{
						if (DEBUG)
						{
							UI.Notify("Fusillade dans un quartier pauvre");
						}
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
					if (dealer1Angry && driveByHandler.AmountOfDriveBy(_agg: true) == 0 && (Entity)(object)drugDealer1 == (Entity)null && rnd.Next(0, 10) == 0)
					{
						if (DEBUG)
						{
							UI.Notify("Attaque du fournisseur");
						}
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
						LsFunctions.TextMsg("ZEE", "Dette", "Rembourse ta dette et je les rappellerai.");
					}
				}
				if (entitiesCleanUp.Count > 0)
				{
					foreach (Entity ent in entitiesCleanUp.ToList())
					{
						if (ent == (Entity)null || !ent.Exists())
						{
							entitiesCleanUp.Remove(ent);
						}
						else
						{
							if (!ent.Exists())
							{
								continue;
							}
							if (ent.CurrentBlip != (Blip)null || ent.CurrentBlip.Exists())
							{
								ent.CurrentBlip.Remove();
							}
							if (ent == (Entity)(object)attacker)
							{
								entitiesCleanUp.Remove(ent);
								if (DEBUG)
								{
									UI.Notify("Attaquant supprimé après l'arrêt");
								}
								continue;
							}
							position = ent.Position;
							if (position.DistanceTo(playerPos) > 200f && !ent.IsOnScreen)
							{
								entitiesCleanUp.Remove(ent);
								if (DEBUG)
								{
									UI.Notify("Suppression : " + (object)ent);
								}
								ent.Delete();
							}
						}
					}
				}
			}
		}
		Model model;
		if (Game.GameTime > areaCheckTimer + 999)
		{
			newWepAmount = 0;
			foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
			{
				if (Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (InputArgument)(player.Character), (InputArgument)((int)weapon)))
				{
					newWepAmount++;
					if (newWepAmount > playerWeapons.Count)
					{
						break;
					}
				}
			}
			if (newWepAmount != playerWeps.Count)
			{
				if (DEBUG)
				{
					UI.Notify("Changement d'arme du joueur détecté");
				}
				playerWeps.Clear();
				playerWeapons.Clear();
				int i = 0;
				foreach (WeaponHash weapon2 in Enum.GetValues(typeof(WeaponHash)))
				{
					bool flag = Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (InputArgument)(player.Character), (InputArgument)((int)weapon2));
					int ammunitionInWeapon = Function.Call<int>((Hash)97480644549409105L, (InputArgument)(player.Character), (InputArgument)((int)weapon2));
					if (!flag)
					{
						continue;
					}
					XElement parentElement = new XElement("weapon");
					parentElement.SetAttributeValue("id", i);
					XElement weaponNameElement = new XElement("HASH", (object)weapon2);
					if (DEBUG)
					{
						UI.Notify("Ajout de " + ((object)weapon2/*cast due to constrained. prefix*/).ToString() + " à la liste des armes du joueur.");
					}
					XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
					parentElement.Add(weaponNameElement);
					parentElement.Add(weaponAmmoElement);
					foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
					{
						if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (InputArgument)(player.Character), (InputArgument)((int)weapon2), (InputArgument)((int)component)))
						{
							XElement weaponComponentElement = new XElement("COMPONENT", (object)component);
							parentElement.Add(weaponComponentElement);
						}
					}
					playerWeapons.Add(parentElement);
					playerWeps.Add(((object)weapon2/*cast due to constrained. prefix*/).ToString());
					i++;
				}
			}
			areaCheckTimer = Game.GameTime;
			string newArea = World.GetZoneNameLabel(playerPos);
			if (aDealer != null && playerArea != aDealer.Area && playerPos.DistanceTo(((Entity)aDealer.dPed).Position) > 300f)
			{
				if ((Entity)(object)aDealer.cPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: false);
				}
				LsFunctions.RemovePedFromWorld(aDealer.dPed, clearTask: false);
				aDealer = null;
			}
			if (playerArea != newArea)
			{
				playerArea = newArea;
				if (areas.FindAll((Area a) => a.Name == playerArea).Count == 0)
				{
					if (zoneData.ContainsKey(playerArea))
					{
						areas.Add(new Area(playerArea, LsFunctions.LsDrugs(), zoneData[playerArea].Item1));
					}
					else
					{
						UI.Notify("~r~LsLife - Erreur de type de zone : " + playerArea);
					}
					int aIndex = 0;
					foreach (Area area2 in areas)
					{
						if (area2.Name == playerArea)
						{
							areaIndex = aIndex;
							break;
						}
						aIndex++;
					}
				}
				else
				{
					string area1 = ((HouseHandler.CurrentHouse == null || !HouseHandler.CurrentHouse.inside) ? playerArea : World.GetZoneNameLabel(HouseHandler.CurrentHouse.entrancePos));
					int aIndex2 = 0;
					foreach (Area area3 in areas)
					{
						if (area3.Name == area1)
						{
							areaIndex = aIndex2;
							break;
						}
						aIndex2++;
					}
				}
			}
			else if (aDealer == null && areas[areaIndex].CanSpawnDealer())
			{
				LsFunctions.DbugString("demandBuff 0");
				if (areas[areaIndex].GangPresance > 0f && areas[areaIndex].Name != World.GetZoneNameLabel(HouseHandler.Houses[0].entrancePos))
				{
					List<Ped> aPeds = lPeds.GetPeds.FindAll((Ped r) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						//IL_002f: Unknown result type (might be due to invalid IL or missing references)
						if (World.GetZoneNameLabel(((Entity)r).Position) == areas[areaIndex].Name)
						{
							Vector3 position2 = ((Entity)r).Position;
							return position2.DistanceTo(playerPos) > 20f;
						}
						return false;
					});
					if (aPeds.Count > 0)
					{
						foreach (Ped p3 in aPeds)
						{
							if (!CheckIfPedUsed(p3, _allowPolice: false, _allowVehicle: false))
							{
								aDealer = new AreaDealer(p3);
								if (DEBUG)
								{
									model = ((Entity)p3).Model;
									UI.Notify("Dealer de quartier créé avec ~g~" + ((object)model/*cast due to constrained. prefix*/).ToString());
								}
								break;
							}
						}
					}
				}
			}
			else
			{
				LsFunctions.DbugString("Impossible de créer un dealer mort : " + areas[areaIndex].DealerKilled);
			}
		}
		if (Game.GameTime <= SlowTickTime + 375)
		{
			return;
		}
		SlowTickTime = Game.GameTime;
		player = Game.Player;
		playerGroup = player.Character.CurrentPedGroup;
		if (Function.Call<bool>((Hash)6160435850588389544L, (InputArgument)(lsDebug)))
		{
			DEBUG = !DEBUG;
		}
		else if (Function.Call<bool>((Hash)6160435850588389544L, (InputArgument)(areaDebug)))
		{
			AreaDebug = !AreaDebug;
		}
		else if (Function.Call<bool>((Hash)6160435850588389544L, (InputArgument)(spawnP)))
		{
			LsFunctions.SpawnDriveBy(player.Character, _agressive: false);
		}
		if (lPeds != null)
		{
			lPeds.Ontick();
		}
		if (DealerHandler != null)
		{
			DealerHandler?.OnSlowTick();
		}
		if (pickupHandler != null)
		{
			pickupHandler?.OnSlowTick();
		}
		if (driveByHandler != null)
		{
			driveByHandler.OnTick();
		}
		if (ZeesDealers != null)
		{
			ZeesDealers.OnSlowTick();
			if (ZeesDealers.Kill)
			{
				ZeesDealers = null;
			}
		}
		if (aDealer != null)
		{
			if ((Entity)(object)aDealer.cPed != (Entity)null && ((Entity)aDealer.cPed).IsDead)
			{
				Ped dKiller = null;
				if (aDealer.dPed.GetKiller() is Ped)
				{
					Entity killer = aDealer.dPed.GetKiller();
					dKiller = (Ped)(object)((killer is Ped) ? killer : null);
				}
				if ((Entity)(object)dKiller != (Entity)null && dKiller.CurrentPedGroup == player.Character.CurrentPedGroup)
				{
					aDealer.NewState = Enums.DStates.AttackPlayer;
					if (driveByHandler.AmountOfDriveBy(_agg: true) == 0 && rnd.Next(0, 3) == 0)
					{
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
				}
				LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: false);
				aDealer.cPed = null;
			}
			if (aDealer != null && ((Entity)aDealer.dPed).IsDead)
			{
				areas[areaIndex].KillDealer();
				if (aDealer.dPed.WasKilledByTakedown && LsFunctions.CanSee((Entity)(object)aDealer.dPed, (Entity)(object)player.Character) && rnd.Next(0, 3) == 0)
				{
					if (driveByHandler.AmountOfDriveBy(_agg: true) == 0 && rnd.Next(0, 3) == 0)
					{
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
					if (DEBUG)
					{
						UI.Notify("Meurtre du dealer repéré");
					}
				}
				Ped dKiller2 = null;
				if (aDealer.dPed.GetKiller() is Ped)
				{
					Entity killer2 = aDealer.dPed.GetKiller();
					dKiller2 = (Ped)(object)((killer2 is Ped) ? killer2 : null);
				}
				if ((Entity)(object)dKiller2 != (Entity)null && dKiller2.CurrentPedGroup == player.Character.CurrentPedGroup)
				{
					int drugCap = 28 * ((int)areas[areaIndex].GangPresance + 1);
					if (drugCap < 28)
					{
						drugCap = 28;
					}
					int moneyCap = 1000 * ((int)areas[areaIndex].GangPresance + 1);
					if (moneyCap < 1000)
					{
						moneyCap = 1000;
					}
					pickupHandler.AddBag(new DroppedBag("A ~r~rival dealer~s~ dropped a ~g~bag.", LsFunctions.RanRange(1, drugCap), LsFunctions.RanRange(1, drugCap), LsFunctions.RanRange(1, drugCap), LsFunctions.RanRange(100, moneyCap), LsFunctions.PedWeapons(aDealer.dPed), ((Entity)aDealer.dPed).Position));
				}
				if ((Entity)(object)aDealer.cPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: true);
					aDealer.cPed = null;
				}
				if ((Entity)(object)aDealer.dPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.dPed, clearTask: true);
					aDealer.dPed = null;
				}
				aDealer = null;
			}
			if (aDealer != null && (Entity)(object)aDealer.dPed != (Entity)null && ((Entity)aDealer.dPed).Exists() && ((Entity)aDealer.dPed).IsAlive)
			{
				aDealer.DoState();
				aDealer.CheckState();
			}
		}
		if (wepDealer != null && ((Entity)wepDealer.ped).IsAlive)
		{
			if ((Entity)(object)player.Character.CurrentVehicle != (Entity)(object)wepDealer.vehicle)
			{
				position = ((Entity)wepDealer.ped).Position;
				if (position.DistanceTo(playerPos) < 10f)
				{
					goto IL_252a;
				}
			}
			position = ((Entity)wepDealer.ped).Position;
			if (position.DistanceTo(wepDealer.destination) < 10f)
			{
				goto IL_252a;
			}
			goto IL_25d6;
		}
		goto IL_29de;
		IL_252a:
		Vector3 pos = ((Entity)wepDealer.vehicle).Position + ((Entity)wepDealer.vehicle).Velocity;
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (InputArgument)(pos.X), (InputArgument)(pos.Y), (InputArgument)(pos.Z), (InputArgument)(1), (InputArgument)(1), (InputArgument)(1f), (InputArgument)(0f));
		OutputArgument outArgA = new OutputArgument();
		Function.Call((Hash)8084282276450675138L, (InputArgument)(NodeID), (InputArgument)(object)outArgA);
		Vector3 parkPos = outArgA.GetResult<Vector3>();
		wepDealer.destination = parkPos;
		goto IL_25d6;
		IL_29de:
		if (dealer1Angry && dealer1Debt < 170000)
		{
			dealer1Angry = false;
		}
		LsFunctions.DateTimeUpdate();
		if (attackerPickupSpawned && !attackerPickup.ObjectExists())
		{
			attackerPickup.Delete();
			attackerPickupSpawned = false;
			WeaponHash weaponHash = unchecked((WeaponHash)(-1569615261));
			int ammo = 0;
			WeaponHash currentWep = player.Character.Weapons.Current.Hash;
			foreach (XElement element in attackerWeapons)
			{
				if (!(element.Name == "weapon"))
				{
					continue;
				}
				foreach (XElement wep in element.Elements())
				{
					if (wep.Name == "HASH")
					{
						Enum.TryParse<WeaponHash>(wep.Value, out weaponHash);
					}
					if (wep.Name == "AMMO")
					{
						int.TryParse(wep.Value, out ammo);
					}
					if ((int)weaponHash == -1569615261)
					{
						continue;
					}
					if (ammo < 100)
					{
						ammo = 100;
					}
					player.Character.Weapons.Give(weaponHash, ammo, true, true);
					player.Character.Weapons.Select(weaponHash, true);
					foreach (XElement c2 in element.Elements())
					{
						if (c2.Name == "COMPONENT")
						{
							Enum.TryParse<WeaponComponent>(c2.Value, out WeaponComponent wepcomp);
							player.Character.Weapons.Current.SetComponent(wepcomp, true);
						}
					}
					if (DEBUG)
					{
						UI.Notify("Donne : " + ((object)weaponHash/*cast due to constrained. prefix*/).ToString() + " munitions = " + ammo);
					}
				}
			}
			attackerWeapons.Clear();
			player.Character.Weapons.Select(currentWep);
			PlayerInventory["Weed"] += attackerWeed;
			PlayerInventory["Crack"] += attackerCrack;
			PlayerInventory["Cocaine"] += attackerCocaine;
			Player val = player;
			val.Money += attackerMoney;
			attackerWeed = 0;
			attackerCrack = 0;
			attackerCocaine = 0;
			LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element2 in LSLifeSave.Descendants())
			{
				if (!(element2.Name == "playerInventory"))
				{
					continue;
				}
				foreach (XElement drug3 in element2.Elements())
				{
					if (drug3.Name == "weed")
					{
						drug3.Value = PlayerInventory["Weed"].ToString();
					}
					if (drug3.Name == "crack")
					{
						drug3.Value = PlayerInventory["Crack"].ToString();
					}
					if (drug3.Name == "cocain")
					{
						drug3.Value = PlayerInventory["Cocaine"].ToString();
					}
				}
			}
			LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if (areas.Count > 0 && playerWanted && areas[areaIndex].Heat < 5000f)
		{
			Area areaToAddHeat = ((HouseHandler.CurrentHouse == null || !HouseHandler.CurrentHouse.inside) ? areas[areaIndex] : areas.Find((Area a) =>
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				return a.Name == World.GetZoneNameLabel(HouseHandler.CurrentHouse.entrancePos);
			}));
			areaToAddHeat.Heat += (float)player.WantedLevel * 0.05f;
		}
		if (player.IsPlaying && !notFined)
		{
			notFined = true;
		}
		if ((Entity)(object)player.Character != (Entity)null && !((Entity)player.Character).IsAlive && !playerDead)
		{
			driveByHandler.CleanDriveBys();
			playerDead = true;
			if (isDealing)
			{
				isDealing = false;
			}
			Entity killer3 = player.Character.GetKiller();
			if (killer3 != (Entity)null)
			{
				Ped p4 = (Ped)(object)((killer3 is Ped) ? killer3 : null);
				if (p4 != null && (Entity)(object)p4 != (Entity)null && p4.IsHuman && !p4.IsPlayer && !LsFunctions.IsPolice(p4) && (Entity)(object)attacker == (Entity)null)
				{
					LsFunctions.TextMsg("ZEE", "info", "J'ai retrouvé cette personne et envoyé sa position.");
					attacker = p4;
					((Entity)attacker).IsPersistent = true;
					attackerMoney = player.Money - (int)((double)player.Money * 0.1);
					Player val2 = player;
					val2.Money -= player.Money;
					if (angryPeds.Contains(attacker))
					{
						angryPeds.Remove(attacker);
					}
					if (driveByHandler.GetDriveBies.Count > 0)
					{
						foreach (DriveBy driveBy in driveByHandler.GetDriveBies)
						{
							if (driveBy.Peds.Count > 0 && driveBy.Peds.Contains(attacker))
							{
								driveBy.Peds.Remove(attacker);
								break;
							}
						}
					}
					if ((Entity)(object)aDealer?.dPed == (Entity)(object)attacker)
					{
						aDealer = null;
					}
					attacker.BlockPermanentEvents = true;
					((Entity)attacker).IsPersistent = true;
					LsFunctions.AddBlip((Entity)(object)attacker, (BlipColor)1, "Braqueur", _friend: false, _edge: false);
					WeaponHash weaponHash2 = unchecked((WeaponHash)(-1569615261));
					int ammo2 = 0;
					player.Character.Weapons.RemoveAll();
					attacker.Armor = 100;
					foreach (XElement element3 in playerWeapons)
					{
						if (!(element3.Name == "weapon"))
						{
							continue;
						}
						attackerWeapons.Add(element3);
						foreach (XElement wep2 in element3.Elements())
						{
							if (wep2.Name == "HASH")
							{
								Enum.TryParse<WeaponHash>(wep2.Value, out weaponHash2);
							}
							if (wep2.Name == "AMMO")
							{
								int.TryParse(wep2.Value, out ammo2);
							}
							if ((int)weaponHash2 != -1569615261)
							{
								attacker.Weapons.Give(weaponHash2, ammo2, true, true);
							}
						}
					}
					playerWeapons.Clear();
					playerWeps.Clear();
					attacker.Weapons.Select(attacker.Weapons.BestWeapon);
					attacker.DropsWeaponsOnDeath = false;
					if (DEBUG)
					{
						UI.Notify("Le tueur est équipé de " + attacker.Weapons.Current.Name + " et porte un gilet pare-balles");
					}
					attackerWeed = PlayerInventory["Weed"];
					attackerCrack = PlayerInventory["Crack"];
					attackerCocaine = PlayerInventory["Cocaine"];
					foreach (string drug4 in PlayerInventory.Keys.ToList())
					{
						PlayerInventory[drug4] = 0;
					}
				}
			}
		}
		if ((Entity)(object)player.Character != (Entity)null && !player.IsPlaying)
		{
			if (isDealing)
			{
				isDealing = false;
			}
			if (notFined && playerWanted)
			{
				notFined = false;
				playerWanted = false;
				int AmountOfDrugs = TotalDrugsCarried();
				if (pStashVehicle != null && pStashVehicle.drugCarWanted && pStashVehicle.TotalDrugs() > 0)
				{
					AmountOfDrugs += pStashVehicle.TotalDrugs();
					pStashVehicle.ImpoundCar();
				}
				if ((Entity)(object)attacker == (Entity)null && AmountOfDrugs > 0)
				{
					LsFunctions.SendTextMsg("Ils ont pris $" + LsFunctions.IntToMoney(player.Money) + ", toutes tes drogues et tes armes.");
					player.Character.Weapons.RemoveAll();
					Player val3 = player;
					val3.Money -= player.Money;
					if (PlayerInventory.Keys.ToList().Count > 0)
					{
						foreach (string drug5 in PlayerInventory.Keys.ToList())
						{
							PlayerInventory[drug5] = 0;
						}
						LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
						foreach (XElement element4 in LSLifeSave.Descendants())
						{
							if (!(element4.Name == "playerInventory"))
							{
								continue;
							}
							foreach (XElement pe in element4.Elements())
							{
								if (pe.Name == "weed")
								{
									pe.Value = "0";
								}
								if (pe.Name == "crack")
								{
									pe.Value = "0";
								}
								if (pe.Name == "cocain")
								{
									pe.Value = "0";
								}
							}
						}
						LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
					}
				}
			}
			else if (playerDead)
			{
				playerDead = false;
				if ((Entity)(object)attacker == (Entity)null && TotalDrugsCarried() > 0)
				{
					player.Character.Weapons.RemoveAll();
					LsFunctions.SendTextMsg("La police a confisqué $" + LsFunctions.IntToMoney(player.Money) + ", toutes tes drogues et tes armes.");
					Player val4 = player;
					val4.Money -= player.Money;
					if (PlayerInventory.Keys.ToList().Count > 0)
					{
						foreach (string drug6 in PlayerInventory.Keys.ToList())
						{
							PlayerInventory[drug6] = 0;
						}
						LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
						foreach (XElement element5 in LSLifeSave.Descendants())
						{
							if (!(element5.Name == "playerInventory"))
							{
								continue;
							}
							foreach (XElement pe2 in element5.Elements())
							{
								if (pe2.Name == "weed")
								{
									pe2.Value = "0";
								}
								if (pe2.Name == "crack")
								{
									pe2.Value = "0";
								}
								if (pe2.Name == "cocain")
								{
									pe2.Value = "0";
								}
							}
						}
						LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
					}
				}
			}
		}
		if (angryPeds.Count > 0)
		{
			int pIndex = 0;
			foreach (Ped p5 in angryPeds)
			{
				pIndex++;
				if ((Entity)(object)p5 == (Entity)null)
				{
					angryPeds.RemoveAt(pIndex);
					break;
				}
				if (!((Entity)(object)p5 != (Entity)null))
				{
					continue;
				}
				if (((Entity)p5).IsDead)
				{
					LsFunctions.RemovePedFromWorld(p5, clearTask: false);
					angryPeds.Remove(p5);
					if (areas[areaIndex].GangPresance > 0f)
					{
						areas[areaIndex].GangPresance--;
					}
					break;
				}
				if (playerPos.DistanceTo(((Entity)p5).Position) > 100f)
				{
					LsFunctions.RemovePedFromWorld(p5, clearTask: false);
					angryPeds.Remove(p5);
					break;
				}
				if (!p5.IsInCombatAgainst(player.Character) || !p5.IsInMeleeCombat)
				{
					p5.Task.FightAgainst(player.Character);
					if (!((Entity)p5).IsPersistent)
					{
						((Entity)p5).IsPersistent = true;
					}
					p5.BlockPermanentEvents = true;
					if (((Entity)p5).CurrentBlip.Exists())
					{
						((Entity)p5).CurrentBlip.Name = "Braqueur";
						((Entity)p5).CurrentBlip.IsFriendly = false;
					}
				}
			}
		}
		if (!((Entity)(object)player.Character != (Entity)null) || !((Entity)player.Character).IsAlive || !player.IsPlaying)
		{
			return;
		}
		if (pigs.Count == 0)
		{
			if (PlayerComplied)
			{
				PlayerComplied = false;
			}
			if (playerHeat > 50f && rnd.Next(0, 100) == 0)
			{
				SpawnPigs();
			}
			if (playerHeat > 95f && rnd.Next(0, 10) == 0)
			{
				SpawnPigs();
			}
		}
		else
		{
			HandlePigs();
		}
		if (pStashVehicle == null)
		{
			pStashVehicle = new StashVehicle();
		}
		else
		{
			pStashVehicle.OnDelayedTick();
		}
		if (Prison.Started)
		{
			if (Function.Call<bool>((Hash)5315141875575487047L) && !Prison.Ready)
			{
				Function.Call(unchecked((Hash)(-8272491852216909662L)), (InputArgument)(player.Handle), (InputArgument)(false), (InputArgument)(256));
				UI.Notify("PRÊT");
				Prison.Ready = true;
			}
			Prison.PrisonTick();
		}
		if ((Entity)(object)attacker != (Entity)null)
		{
			if (((Entity)attacker).CurrentBlip == (Blip)null || !((Entity)attacker).CurrentBlip.Exists())
			{
				LsFunctions.AddBlip((Entity)(object)attacker, (BlipColor)1, "Braqueur", _friend: false, _edge: false);
			}
			if (((Entity)attacker).IsDead)
			{
				LsFunctions.RemovePedFromWorld(attacker, clearTask: true);
				if (!attackerPickupSpawned)
				{
					attackerAttacking = false;
					attackerPickupSpawned = true;
					attackerPickup = World.CreatePickup((PickupType)341217064, ((Entity)attacker).Position, Vector3.Zero, new Model("prop_big_bag_01"), 100);
				}
				attacker = null;
			}
			else
			{
				if (!((Entity)attacker).IsPersistent)
				{
					((Entity)attacker).IsPersistent = true;
				}
				position = ((Entity)attacker).Position;
				float distance = position.DistanceTo(playerPos);
				if (!attackerAttacking && Game.GameTime > attackCheckTime + 1000)
				{
					attackCheckTime = Game.GameTime;
					if (distance < 60f && LsFunctions.CanSee((Entity)(object)attacker, (Entity)(object)player.Character))
					{
						int EnemyRelationShipGroup = Function.Call<int>(unchecked((Hash)(-3292914402564945716L)), (InputArgument)("HATES_PLAYER"));
						Function.Call(unchecked((Hash)(-4032282232082276974L)), (InputArgument)(attacker), (InputArgument)(EnemyRelationShipGroup));
						attackerAttacking = true;
						attacker.Task.ClearAll();
						attacker.Task.FightAgainst(player.Character);
					}
					else
					{
						attacker.Task.ClearAll();
						attacker.Task.WanderAround();
						attackerAttacking = false;
					}
				}
			}
		}
		bool sprintChange = false;
		if (!playerHasBag)
		{
			if (!canSprint)
			{
				canSprint = true;
				sprintChange = true;
			}
			if (TotalDrugsCarried() / 28 + (playerWeps.Count - 1) + player.Money / 50000 > 35)
			{
				LsFunctions.PedBag(player.Character, give: true);
				playerHasBag = true;
			}
		}
		if (playerHasBag)
		{
			if (!canSprint)
			{
				if ((Entity)(object)player.Character.CurrentVehicle != (Entity)null)
				{
					model = ((Entity)player.Character.CurrentVehicle).Model;
					if (model.IsBicycle)
					{
						canSprint = true;
						sprintChange = true;
					}
				}
			}
			else if ((Entity)(object)player.Character.CurrentVehicle == (Entity)null)
			{
				canSprint = false;
				sprintChange = true;
			}
			if (TotalDrugsCarried() / 28 + (playerWeps.Count - 1) + player.Money / 50000 <= 35)
			{
				LsFunctions.PedBag(player.Character, give: false);
				playerHasBag = false;
			}
		}
		if (sprintChange)
		{
			if (canSprint)
			{
				Function.Call(unchecked((Hash)(-6909787959634350604L)), (InputArgument)(player), (InputArgument)(true));
			}
			else
			{
				Function.Call(unchecked((Hash)(-6909787959634350604L)), (InputArgument)(player), (InputArgument)(false));
			}
		}
		if (player.WantedLevel > 0 && !playerWanted)
		{
			playerWanted = true;
		}
		if (playerWanted && player.WantedLevel == 0)
		{
			playerWanted = false;
		}
		if ((Entity)(object)drugDealer1 != (Entity)null && ((Entity)drugDealer1).IsDead)
		{
			LsFunctions.RemovePedFromWorld(drugDealer1, clearTask: true);
			drugDealer1 = null;
		}
		if ((Entity)(object)weaponDealer1 != (Entity)null && ((Entity)weaponDealer1).IsDead)
		{
			LsFunctions.RemovePedFromWorld(weaponDealer1, clearTask: true);
			weaponDealer1 = null;
		}
		if (player.WantedLevel > 0 && isDealing)
		{
			isDealing = false;
		}
		if (!isDealing && !removedStuff)
		{
			removedStuff = true;
			if (ShotHandler.shots.Count > 0)
			{
				foreach (Shot s in ShotHandler.shots)
				{
					if (s.customer != null)
					{
						LsFunctions.RemovePedFromWorld(s.customer.Ped, clearTask: true);
					}
				}
				ShotHandler.shots.Clear();
			}
			if (customers.Count > 0)
			{
				foreach (Customer c3 in customers)
				{
					if (c3 == null)
					{
						continue;
					}
					foreach (Drug d in areas[areaIndex].Drugs)
					{
						if (d.Name == c3.Drug.Name)
						{
							d.Supplied = false;
						}
					}
					if (((Entity)c3.Ped).Exists())
					{
						OldCustomerHandler.AddCustomer(c3.Ped, c3.Vehicle, HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(c3.Drug.Name, c3.amount));
					}
				}
				customers.Clear();
			}
			if (angryPeds.Count > 0)
			{
				foreach (Ped p6 in angryPeds)
				{
					if ((Entity)(object)p6 != (Entity)null && ((Entity)p6).Exists())
					{
						LsFunctions.RemovePedFromWorld(p6, clearTask: true);
					}
				}
				angryPeds.Clear();
			}
		}
		if (isDealing)
		{
			ShotHandler.OnSlowTick();
		}
		if (!((Entity)(object)weaponDealer1 != (Entity)null) || !((Entity)weaponDealer1).IsAlive)
		{
			return;
		}
		float distanceToPlayer = World.GetDistance(((Entity)weaponDealer1).Position, playerPos);
		if (distanceToPlayer > 15f && weaponDealer1.CurrentPedGroup != player.Character.CurrentPedGroup)
		{
			if (Game.GameTime > dealerTimer + 2000 && playerPos != pedTarget)
			{
				dealerTimer = Game.GameTime;
				pedTarget = playerPos;
				weaponDealer1.Task.GoTo((Entity)(object)player.Character);
			}
		}
		else if (weaponDealer1.CurrentPedGroup != player.Character.CurrentPedGroup)
		{
			weaponDealer1.Task.ClearAll();
			Function.Call(unchecked((Hash)(-6974808092771143243L)), (InputArgument)(((Entity)weaponDealer1).Handle), (InputArgument)(playerGroup));
		}
		if (!ShotHandler.ShotOffers && weaponDealer1.IsStopped && distanceToPlayer <= 3f && player.Character.IsStopped && LsFunctions.IsCustomerNear() && PottentialWorker == null)
		{
			if (lPeds.GetPeds.FindAll((Ped r) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position2 = ((Entity)r).Position;
				return position2.DistanceTo(playerPos) < 10f;
			}).Count() - PedsInPlayersGroup() > 3)
			{
				if ((float)Game.GameTime > displayHelpTimer + 5000f)
				{
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Trouve un endroit plus calme.");
				}
				readyToDeal = false;
				if (wepMenu.Visible)
				{
					wepMenu.Visible = !wepMenu.Visible;
				}
			}
			else if (!readyToDeal && !LsMenuPool.IsAnyMenuOpen())
			{
				if ((float)Game.GameTime > displayHelpTimer + 5000f)
				{
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Appuyez sur ~INPUT_CONTEXT~ pour interagir");
				}
				readyToDeal = true;
			}
		}
		else
		{
			readyToDeal = false;
		}
		if ((Entity)(object)weaponDealer1 != (Entity)null && !((Entity)weaponDealer1).IsAlive)
		{
			((Entity)weaponDealer1).IsPersistent = false;
			((Entity)weaponDealer1).CurrentBlip.Remove();
			((Entity)weaponDealer1).MarkAsNoLongerNeeded();
			LsFunctions.SendTextMsg("Le dealer est mort !");
			weaponDealer1 = null;
			readyToDeal = false;
		}
		return;
		IL_25d6:
		if (wepDealer.state == Dealer.States.Driving || wepDealer.state == Dealer.States.Fast)
		{
			position = ((Entity)wepDealer.vehicle).Position;
			if (position.DistanceTo(wepDealer.destination) < 80f)
			{
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 10f, 786603);
				wepDealer.state = Dealer.States.Ariving;
			}
		}
		if (wepDealer.state == Dealer.States.Ariving)
		{
			position = ((Entity)wepDealer.vehicle).Position;
			if (position.DistanceTo(wepDealer.destination) < 20f)
			{
				wepDealer.doneTutorial = false;
				Vector3 pos2 = wepDealer.destination;
				int NodeID2 = Function.Call<int>((Hash)2510518586829603349L, (InputArgument)(pos2.X), (InputArgument)(pos2.Y), (InputArgument)(pos2.Z), (InputArgument)(1), (InputArgument)(1), (InputArgument)(1f), (InputArgument)(0f));
				OutputArgument outArgA2 = new OutputArgument();
				Function.Call((Hash)8084282276450675138L, (InputArgument)(NodeID2), (InputArgument)(object)outArgA2);
				Vector3 parkPos2 = outArgA2.GetResult<Vector3>();
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, parkPos2, 2f, 6f, 786603);
				wepDealer.destination = Vector3.Zero;
				wepDealer.state = Dealer.States.Arrived;
			}
		}
		if (wepDealer.state == Dealer.States.Arrived && player.Character.IsInVehicle() && ((Entity)player.Character.CurrentVehicle).Handle == ((Entity)wepDealer.vehicle).Handle)
		{
			if (!wepDealer.doneTutorial)
			{
				Function.Call((Hash)7023634693725934496L);
				wepDealer.doneTutorial = true;
				displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuyez sur ~INPUT_CONTEXT~ pour interagir ou ~INPUT_CONTEXT_SECONDARY~ pour renvoyer.");
			}
			if (Game.IsWaypointActive && wepDealer.destination != World.GetNextPositionOnStreet(World.GetWaypointPosition()))
			{
				wepDealer.destination = World.GetNextPositionOnStreet(World.GetWaypointPosition());
				((Entity)wepDealer.vehicle).IsInvincible = true;
				wepDealer.ped.Task.ClearAll();
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1f, 10f, 786603);
				UI.ShowSubtitle("D'accord, nous allons vers ~y~" + World.GetStreetName(wepDealer.destination));
				wepDealer.state = Dealer.States.Leaving;
			}
		}
		if (wepDealer.state == Dealer.States.Leaving)
		{
			Script.Wait(5000);
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1f, 40f, 786603);
			wepDealer.state = Dealer.States.Driving;
		}
		if (wepDealer.state == Dealer.States.Faster)
		{
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 50f, 6);
			wepDealer.state = Dealer.States.Fast;
			UI.ShowSubtitle("D'accord, j'accélère.", 2000);
			wepDealer.canSpeed = true;
		}
		if (wepDealer.state == Dealer.States.Slower)
		{
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 20f, 786603);
			wepDealer.state = Dealer.States.Driving;
			UI.ShowSubtitle("D'accord, je ralentis.", 2000);
			wepDealer.canSpeed = false;
		}
		goto IL_29de;
	}

	private bool IsNight()
	{
		int hour = World.CurrentDayTime.Hours;
		if (hour <= 20)
		{
			return hour < 5;
		}
		return true;
	}

	private static void HandleOldCutomers()
	{
		OldCustomerHandler.OnTick();
	}

	private void HandlePotentialWorker()
	{
		if (PottentialWorker != null)
		{
			PottentialWorker.OnTick();
			if (PottentialWorker.Destroy)
			{
				PottentialWorker = null;
			}
		}
	}

	private static void LoadAreaData()
	{
		foreach (XElement aEle in XDocument.Load("scripts\\LSLife\\LSLifeAreas.xml").Descendants())
		{
			if (!(aEle.Name == "area"))
			{
				continue;
			}
			string name = "";
			int cStr = -1;
			int gstr = -1;
			int heat = -1;
			float rep = -1f;
			foreach (XElement aEl in aEle.Elements())
			{
				if (aEl.Name == "name")
				{
					name = aEl.Value;
				}
				if (aEl.Name == "cStr")
				{
					int.TryParse(aEl.Value, out cStr);
				}
				if (aEl.Name == "gStr")
				{
					int.TryParse(aEl.Value, out gstr);
				}
				if (aEl.Name == "aHeat")
				{
					int.TryParse(aEl.Value, out heat);
				}
				if (aEl.Name == "aRep")
				{
					float.TryParse(aEl.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out rep);
				}
			}
			if (name != "" && cStr != -1 && gstr != -1)
			{
				AreaType type = AreaType.Normal;
				if (zoneData.ContainsKey(name))
				{
					type = zoneData[name].Item1;
				}
				else
				{
					UI.Notify("~r~LsLife - Erreur de type de zone : " + name);
				}
				areas.Add(new Area(name, LsFunctions.LsDrugs(), type, cStr, gstr, heat, rep));
			}
		}
	}

	private void SavedDetected()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Function.Call<bool>((Hash)7576188390707304864L);
		bool SaveHud = UI.IsHudComponentActive((HudComponent)17);
		bool StatSave = Function.Call<bool>(unchecked((Hash)(-4920555325479993693L)));
		bool Paused = Function.Call<bool>(unchecked((Hash)(-5763681587332382773L)));
		bool sPaused = Function.Call<bool>(unchecked((Hash)(-1572002141673951856L)));
		bool Busy = Function.Call<bool>(unchecked((Hash)(-5573887678010054197L)));
		if (flag | SaveHud | StatSave | Paused | sPaused | Busy)
		{
			pSaved = true;
		}
		new UIText(pSaved.ToString(), new Point(Game.ScreenResolution.Width / 2, Game.ScreenResolution.Height / 2), 1f).Draw();
	}

	private Customer FindClosestCustomer()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float nDistance = 100f;
		Customer cCustomer = null;
		foreach (Customer c in customers.Where((Customer r) => r.Ped.CurrentPedGroup == playerGroup))
		{
			Vector3 position = ((Entity)c.Ped).Position;
			float cDistance = position.DistanceTo(playerPos + ((Entity)player.Character).ForwardVector);
			if (cDistance < 3f && cDistance < nDistance)
			{
				nDistance = cDistance;
				cCustomer = c;
			}
		}
		return cCustomer;
	}

	private Customer FindClosestCustomerInVehicle()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float nDistance = 100f;
		Customer cCustomer = null;
		foreach (Customer c in customers.Where((Customer r) => r.Ped.CurrentPedGroup == playerGroup && (Entity)(object)r.Ped.CurrentVehicle == (Entity)(object)player.Character.CurrentVehicle))
		{
			Vector3 position = ((Entity)c.Ped).Position;
			float cDistance = position.DistanceTo(playerPos);
			if (cDistance < nDistance)
			{
				nDistance = cDistance;
				cCustomer = c;
			}
		}
		return cCustomer;
	}

	private void areaInfoUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected Obj, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected Obj, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected Obj, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected Obj, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected Obj, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected Obj, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected Obj, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Expected Obj, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Expected Obj, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Expected Obj, but got Unknown
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Expected Obj, but got Unknown
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Expected Obj, but got Unknown
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Expected Obj, but got Unknown
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Expected Obj, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Expected Obj, but got Unknown
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Expected Obj, but got Unknown
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		Point point = new Point(10, 10);
		Size size = new Size(80, 184);
		UIContainer val = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--AREA--", new Point(2, 2), 0.2f);
		UIText name = new UIText("name:" + areas[areaIndex].Name, new Point(2, 12), 0.2f);
		UIText type = new UIText("type:" + areas[areaIndex].AreaType, new Point(2, 22), 0.2f);
		UIText cop = new UIText("cStr:" + areas[areaIndex].CopPresance, new Point(2, 32), 0.2f);
		UIText gang = new UIText("gStr:" + areas[areaIndex].GangPresance, new Point(2, 42), 0.2f);
		UIText heat = new UIText("heat:" + areas[areaIndex].Heat, new Point(2, 52), 0.2f);
		UIText dealers = new UIText("nDealer:" + areas[areaIndex].Dealers(), new Point(2, 62), 0.2f);
		UIText demandMet = new UIText("dMet:" + areas[areaIndex].DemandMet(), new Point(2, 72), 0.2f);
		UIText aRep = new UIText("aRep:" + areas[areaIndex].Reputation + " Lvl :" + (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation), new Point(2, 82), 0.2f);
		UIText drug0H = new UIText("--" + areas[areaIndex].Drugs[0].Name + "--", new Point(2, 92), 0.2f);
		UIText drug0D = new UIText("Dem:" + areas[areaIndex].Drugs[0].Demand, new Point(2, 102), 0.2f);
		UIText drug0S = new UIText("Sup:" + areas[areaIndex].Drugs[0].Supplied, new Point(2, 112), 0.2f);
		UIText drug1H = new UIText("--" + areas[areaIndex].Drugs[1].Name + "--", new Point(2, 122), 0.2f);
		UIText drug1D = new UIText("Dem:" + areas[areaIndex].Drugs[1].Demand, new Point(2, 132), 0.2f);
		UIText drug1S = new UIText("Sup:" + areas[areaIndex].Drugs[1].Supplied, new Point(2, 142), 0.2f);
		UIText drug2H = new UIText("--" + areas[areaIndex].Drugs[2].Name + "--", new Point(2, 152), 0.2f);
		UIText drug2D = new UIText("Dem:" + areas[areaIndex].Drugs[2].Demand, new Point(2, 162), 0.2f);
		UIText drug2S = new UIText("Sup:" + areas[areaIndex].Drugs[2].Supplied, new Point(2, 172), 0.2f);
		val.Items.Add((UIElement)(object)aHeader);
		val.Items.Add((UIElement)(object)name);
		val.Items.Add((UIElement)(object)type);
		val.Items.Add((UIElement)(object)cop);
		val.Items.Add((UIElement)(object)gang);
		val.Items.Add((UIElement)(object)heat);
		val.Items.Add((UIElement)(object)dealers);
		val.Items.Add((UIElement)(object)demandMet);
		val.Items.Add((UIElement)(object)aRep);
		val.Items.Add((UIElement)(object)drug0H);
		val.Items.Add((UIElement)(object)drug0D);
		val.Items.Add((UIElement)(object)drug0S);
		val.Items.Add((UIElement)(object)drug1H);
		val.Items.Add((UIElement)(object)drug1D);
		val.Items.Add((UIElement)(object)drug1S);
		val.Items.Add((UIElement)(object)drug2H);
		val.Items.Add((UIElement)(object)drug2D);
		val.Items.Add((UIElement)(object)drug2S);
		((UIRectangle)val).Draw();
	}

	private void ZeeInfoUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected Obj, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected Obj, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected Obj, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected Obj, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected Obj, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected Obj, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		Point point = new Point(10, 194);
		Size size = new Size(80, 104);
		UIContainer val = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--Zee--", new Point(2, 2), 0.2f);
		UIText money = new UIText("money:" + dealer1Money, new Point(2, 12), 0.2f);
		UIText reload = new UIText("reload:" + dealer1ReloadOrder, new Point(2, 22), 0.2f);
		UIText reloaded = new UIText("reloaded:" + dealer1Reloaded, new Point(2, 32), 0.2f);
		UIText weed = new UIText("weed:" + dealer1Weed, new Point(2, 42), 0.2f);
		UIText crack = new UIText("crack:" + dealer1Crack, new Point(2, 52), 0.2f);
		UIText coke = new UIText("coke:" + dealer1Cocain, new Point(2, 62), 0.2f);
		val.Items.Add((UIElement)(object)aHeader);
		val.Items.Add((UIElement)(object)money);
		val.Items.Add((UIElement)(object)reload);
		val.Items.Add((UIElement)(object)reloaded);
		val.Items.Add((UIElement)(object)weed);
		val.Items.Add((UIElement)(object)crack);
		val.Items.Add((UIElement)(object)coke);
		((UIRectangle)val).Draw();
	}

	private void PlayerUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected Obj, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected Obj, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected Obj, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected Obj, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected Obj, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected Obj, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected Obj, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected Obj, but got Unknown
		Point point = new Point(10, 294);
		Size size = new Size(80, 100);
		UIContainer cont = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--Player--", new Point(2, 2), 0.2f);
		UIText heat = new UIText("heat:" + playerHeat, new Point(2, 12), 0.2f);
		UIText pDealing = new UIText("dealing:" + isDealing, new Point(2, 22), 0.2f);
		UIText tPeds = new UIText("tPeds:" + lPeds.GetPeds.Count, new Point(2, 32), 0.2f);
		UIText oPeds = new UIText("oPeds:" + OldCustomerHandler.AmountOf(), new Point(2, 42), 0.2f);
		cont.Items.Add((UIElement)(object)aHeader);
		cont.Items.Add((UIElement)(object)heat);
		cont.Items.Add((UIElement)(object)pDealing);
		cont.Items.Add((UIElement)(object)tPeds);
		cont.Items.Add((UIElement)(object)oPeds);
		if (wepDealer != null)
		{
			UIText dSpeed = new UIText("dSpeed:" + wepDealer.maxSpeed + "|" + wepDealer.ped.CurrentVehicle.Speed, new Point(2, 52), 0.2f);
			cont.Items.Add((UIElement)(object)dSpeed);
			UIText cSpeed = new UIText("cSpeed:" + wepDealer.canSpeed, new Point(2, 62), 0.2f);
			cont.Items.Add((UIElement)(object)cSpeed);
			UIText sSpeed = new UIText("sSpeed:" + wepDealer.shouldSpeed, new Point(2, 72), 0.2f);
			cont.Items.Add((UIElement)(object)sSpeed);
		}
		int drvAmount = driveByHandler.GetDriveBies.Count;
		if (drvAmount > 0)
		{
			UIText drvAmou = new UIText("drvAmount:" + drvAmount, new Point(2, 82), 0.2f);
			cont.Items.Add((UIElement)(object)drvAmou);
		}
		((UIRectangle)cont).Draw();
	}

	private void GangUI()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected Obj, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected Obj, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected Obj, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected Obj, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected Obj, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected Obj, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected Obj, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		Gang gang = areas[areaIndex].Gang;
		Point point = new Point(10, 394);
		Size size = new Size(80, 100);
		UIContainer val = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--Gang--", new Point(2, 2), 0.2f);
		UIText gName = new UIText("name:" + gang.Name, new Point(2, 12), 0.2f);
		UIText gMoney = new UIText("money:" + gang.Money, new Point(2, 22), 0.2f);
		UIText gMax = new UIText("max:" + gang.MaxMembers, new Point(2, 32), 0.2f);
		UIText gTotal = new UIText("total:" + gang.TotalMembers, new Point(2, 42), 0.2f);
		UIText gThreat = new UIText("threats:" + gang.ThreatHandler.ThreatCount, new Point(2, 52), 0.2f);
		UIText gFree = new UIText("free:" + GangUtilities.AllocatedMemberCount(gang), new Point(2, 62), 0.2f);
		UIText night = new UIText("night:" + IsNight(), new Point(2, 72), 0.2f);
		UIText shots = new UIText("shots: " + ShotHandler.shots.Count, new Point(2, 82), 0.2f);
		val.Items.Add((UIElement)(object)aHeader);
		val.Items.Add((UIElement)(object)gName);
		val.Items.Add((UIElement)(object)gMoney);
		val.Items.Add((UIElement)(object)gMax);
		val.Items.Add((UIElement)(object)gTotal);
		val.Items.Add((UIElement)(object)gFree);
		val.Items.Add((UIElement)(object)gThreat);
		val.Items.Add((UIElement)(object)night);
		val.Items.Add((UIElement)(object)shots);
		((UIRectangle)val).Draw();
	}

	private static void HandlePigs()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (pigs.Count <= 0)
		{
			return;
		}
		foreach (Pig pig in pigs.ToList())
		{
			if ((Entity)(object)pig.Ped != (Entity)null && ((Entity)pig.Ped).Exists())
			{
				if (((Entity)pig.Ped).IsAlive)
				{
					Vector3 position = ((Entity)pig.Ped).Position;
					if (!(position.DistanceTo(playerPos) > 1050f))
					{
						goto IL_008c;
					}
				}
				pig.DeletePig();
				if (DEBUG)
				{
					UI.Notify("Policier créé supprimé");
				}
				goto IL_008c;
			}
			pigs.Remove(pig);
			continue;
			IL_008c:
			pig.DoState();
			pig.CheckState();
		}
	}

	private static void SpawnPigs()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Vehicle pigCar = World.CreateVehicle((int)(Enums.GetPoliceVehType()), GenerateSpawnPos(playerPos.Around(200f), Nodetype.Road, sidewalk: false));
		if ((Entity)(object)pigCar != (Entity)null)
		{
			if (DEBUG)
			{
				UI.Notify("Policier créé");
				((Entity)pigCar).AddBlip();
				((Entity)pigCar).CurrentBlip.Name = "Police Car";
			}
			Function.Call(unchecked((Hash)(-823518585696636035L)), (InputArgument)(((Entity)pigCar).Handle), (InputArgument)(true));
			pigs.Add(new Pig(pigCar.CreatePedOnSeat((VehicleSeat)(-1), (int)(Enums.GetPolicePedType())), _leader: true));
			pigs.Add(new Pig(pigCar.CreatePedOnSeat((VehicleSeat)0, (int)(Enums.GetPolicePedType())), _leader: false));
		}
		if (pigs.Count <= 0)
		{
			return;
		}
		foreach (Pig p in pigs)
		{
			if ((Entity)(object)p.Ped != (Entity)null)
			{
				Function.Call(unchecked((Hash)(-4970914549772615683L)), (InputArgument)(p.Ped), (InputArgument)(true));
				if (areas[areaIndex].CopPresance < 10)
				{
					areas[areaIndex].CopPresance++;
				}
			}
		}
	}

	private void onKeyDown(object sender, KeyEventArgs e)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (e.KeyCode == SetDrugCar && ((Entity)player.Character).IsAlive)
		{
			if (player.Character.IsInVehicle())
			{
				if (pStashVehicle != null && (Entity)(object)pStashVehicle.drugCar != (Entity)null)
				{
					LsFunctions.SendTextMsg(pStashVehicle.drugCar.DisplayName + " n'est plus un véhicule de stockage.");
					pStashVehicle.RemoveOldCar();
					pStashVehicle = null;
				}
				else
				{
					Model model = ((Entity)player.Character.CurrentVehicle).Model;
					if (!model.IsCar)
					{
						model = ((Entity)player.Character.CurrentVehicle).Model;
						if (!model.IsBike)
						{
							model = ((Entity)player.Character.CurrentVehicle).Model;
							if (!model.IsBoat)
							{
								model = ((Entity)player.Character.CurrentVehicle).Model;
								if (!model.IsHelicopter)
								{
									model = ((Entity)player.Character.CurrentVehicle).Model;
									if (!model.IsPlane)
									{
										LsFunctions.SendTextMsg("Ce véhicule ne peut pas servir de cachette.");
										goto IL_0180;
									}
								}
							}
						}
					}
					pStashVehicle = new StashVehicle();
					pStashVehicle.VehicleSave(player.Character.CurrentVehicle);
					if ((Entity)(object)pStashVehicle.drugCar != (Entity)null)
					{
						LsFunctions.SendTextMsg(pStashVehicle.drugCar.DisplayName + " est maintenant ton véhicule de stockage.");
					}
				}
			}
			else
			{
				DropDrugs.Drop();
			}
		}
		goto IL_0180;
		IL_0180:
		if (e.KeyCode == StopDealing && ((Entity)player.Character).IsAlive)
		{
			if (isDealing)
			{
				isDealing = false;
				LsFunctions.SendTextMsg("Vente de rue annulée");
			}
			else if (!isDealing)
			{
				LsFunctions.SendTextMsg("Aucune vente de rue en cours.");
			}
		}
		if (e.KeyCode == Keys.J && !started)
		{
			if (DEBUG)
			{
				if (player.Character.IsInVehicle())
				{
					LsFunctions.PlayTheSound();
					Vector3 vPos = ((Entity)player.Character.CurrentVehicle).Position;
					float vHead = ((Entity)player.Character.CurrentVehicle).Heading;
					XDocument xDocument = XDocument.Load("scripts\\LSLife\\LSLife_Spawns.xml");
					XElement ePos = new XElement("pos");
					ePos.Add(new XElement("x", vPos.X));
					ePos.Add(new XElement("y", vPos.Y));
					ePos.Add(new XElement("z", vPos.Z));
					ePos.Add(new XElement("h", vHead));
					xDocument.Root.Add(ePos);
					xDocument.Save("scripts\\LSLife\\LSLife_Spawns.xml");
					UI.Notify("Position ajoutée au fichier XML");
				}
				else
				{
					UI.Notify("Tu dois être dans un véhicule pour ajouter une position");
				}
			}
			if (DEBUG)
			{
				player.Character.IsInVehicle();
			}
		}
		if (e.KeyCode != MenuKey || !(player != (Player)null) || !player.IsAlive)
		{
			return;
		}
		if (DEBUG && (Entity)(object)player.Character != (Entity)null)
		{
			string toPaste = playerPos.X + "f, " + playerPos.Y + "f, " + playerPos.Z + "f";
			Thread thread = new Thread(() =>
			{
				Clipboard.SetText(toPaste);
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			thread.Abort();
		}
		showInventory = !showInventory;
		Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
	}

	public static void callDealer(bool wepdealer)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (wepdealer)
		{
			Vector3 dealer1pos = World.GetNextPositionOnSidewalk(GenerateSpawnPos(playerPos.Around(60f), Nodetype.AnyRoad, sidewalk: true));
			weaponDealer1 = World.CreatePed(new Model(unchecked((int)4058522530u)), GenerateSpawnPos(dealer1pos, Nodetype.AnyRoad, sidewalk: true));
			while ((Entity)(object)weaponDealer1 == (Entity)null)
			{
				Script.Wait(0);
			}
			pedTarget = playerPos;
			if ((Entity)(object)weaponDealer1 != (Entity)null)
			{
				weaponDealer1.RelationshipGroup = playerRelationship;
				LsFunctions.AddBlip((Entity)(object)weaponDealer1, (BlipColor)3, "Vendeur de Rafael Mendoza", _friend: true, _edge: true);
				weaponDealer1.CanSwitchWeapons = true;
				weaponDealer1.Weapons.Give((WeaponHash)453432689, 9999, false, true);
				weaponDealer1.Armor = 100;
				Function.Call(unchecked((Hash)(-4079649877180351064L)), (InputArgument)(weaponDealer1), (InputArgument)(100));
				readyToDeal = false;
				Script.Wait(100);
				weaponDealer1.Task.GoTo(pedTarget);
				dealerTimer = Game.GameTime;
				LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
				foreach (XElement element in LSLifeSave.Descendants())
				{
					if (!(element.Name == "dealer1"))
					{
						continue;
					}
					foreach (XElement e in element.Elements())
					{
						if (e.Name == "debt")
						{
							int.TryParse(e.Value, out dealer1Debt);
						}
						if (e.Name == "weed")
						{
							int.TryParse(e.Value, out dealer1Weed);
						}
						if (e.Name == "crack")
						{
							int.TryParse(e.Value, out dealer1Crack);
						}
						if (e.Name == "cocain")
						{
							int.TryParse(e.Value, out dealer1Cocain);
						}
						if (e.Name == "money")
						{
							int.TryParse(e.Value, out dealer1Money);
						}
						if (e.Name == "reloaded")
						{
							bool.TryParse(e.Value, out dealer1Reloaded);
						}
					}
				}
				bushWeed.Maximum = dealer1Weed;
				crack.Maximum = dealer1Crack;
				cocaine.Maximum = dealer1Cocain;
			}
			else
			{
				LsFunctions.SendTextMsg("~r~Impossible de faire apparaître le vendeur d'armes");
			}
		}
		else if (ZeesDealers == null && NewOrder != null)
		{
			ZeesDealers = new DrugDealer(NewOrder);
			if (ZeesDealers != null)
			{
				PlaceOrder.Text = "Annuler la commande";
			}
		}
		else
		{
			LsFunctions.TextMsg("ZEE", "Commande", "Je leur dirai que tu ne viens pas.");
			ZeesDealers.Dismiss();
			if (NewOrder != null)
			{
				NewOrder.PutBackDrugs();
				NewOrder.ResetOrder();
			}
			LsMenuPool.CloseAllMenus();
		}
	}

	public static TaskSequence PerformTaskSeq(int index, Vehicle vehicle, Vector3 target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected Obj, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		switch (index)
		{
		case 0:
		{
			TaskSequence val2 = new TaskSequence();
			val2.AddTask.LeaveVehicle();
			val2.AddTask.WanderAround();
			return val2;
		}
		case 1:
		{
			TaskSequence val = new TaskSequence();
			val.AddTask.GoTo(((Entity)vehicle).Position + ((Entity)vehicle).RightVector * 2f * -1f);
			val.AddTask.EnterVehicle(vehicle, (VehicleSeat)(-1));
			val.AddTask.CruiseWithVehicle(vehicle, 20f, 5);
			return val;
		}
		case 2:
		{
			Vector3 playerForward = ((Entity)player.Character).ForwardVector;
			Vector3 playerRight = ((Entity)player.Character).RightVector;
			TaskSequence driveToPlayerAndExit = new TaskSequence();
			driveToPlayerAndExit.AddTask.ClearAllImmediately();
			driveToPlayerAndExit.AddTask.DriveTo(vehicle, target, 10f, 20f, 5);
			if (player.Character.IsInVehicle())
			{
				driveToPlayerAndExit.AddTask.GoTo(playerPos + playerRight * 3f);
			}
			else
			{
				driveToPlayerAndExit.AddTask.GoTo(playerPos + playerForward * 2f);
			}
			return driveToPlayerAndExit;
		}
		default:
			return null;
		}
	}

	public static bool CheckIfPedUsed(Ped _ped, bool _allowPolice, bool _allowVehicle)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		Model model = ((Entity)_ped).Model;
		if (model.Hash != 1885233650)
		{
			model = ((Entity)_ped).Model;
			if (model.Hash != -1667301416)
			{
				if (!((Entity)_ped).Exists())
				{
					return true;
				}
				if (_ped.IsPlayer)
				{
					return true;
				}
				if (!_allowVehicle && _ped.IsInVehicle())
				{
					return true;
				}
				if (((Entity)_ped).Alpha == 0)
				{
					return true;
				}
				if (!_ped.IsHuman)
				{
					return true;
				}
				if (((Entity)_ped).IsDead)
				{
					return true;
				}
				if (!((Entity)_ped).IsAlive)
				{
					return true;
				}
				if (LsFunctions.IsInInterior((Entity)(object)_ped))
				{
					return true;
				}
				if (LsFunctions.IsUnderGround((Entity)(object)_ped))
				{
					return true;
				}
				if (((Entity)_ped).IsInAir)
				{
					return true;
				}
				if (!_ped.IsInVehicle() && ((Entity)_ped).Position.Z > World.GetGroundHeight(((Entity)_ped).Position + ((Entity)_ped).ForwardVector * 2f) + 2f)
				{
					return true;
				}
				if (player.Character.IsInVehicle() && _ped.IsInVehicle() && (Entity)(object)_ped.CurrentVehicle == (Entity)(object)player.Character.CurrentVehicle)
				{
					return true;
				}
				if (Game.Player.Character.CurrentPedGroup == _ped.CurrentPedGroup)
				{
					return true;
				}
				if (_ped.RelationshipGroup == player.Character.RelationshipGroup)
				{
					return true;
				}
				if ((Entity)(object)drugDealer1 != (Entity)null && (Entity)(object)drugDealer1 == (Entity)(object)_ped)
				{
					return true;
				}
				if ((Entity)(object)weaponDealer1 != (Entity)null && (Entity)(object)weaponDealer1 == (Entity)(object)_ped)
				{
					return true;
				}
				if (wepDealer != null && (Entity)(object)wepDealer.ped == (Entity)(object)_ped)
				{
					return true;
				}
				if (customers.Count > 0 && customers.FindAll((Customer r) => (Entity)(object)r.Ped == (Entity)(object)_ped).Count != 0)
				{
					return true;
				}
				if (OldCustomerHandler.IsPedOldCustomer(_ped))
				{
					return true;
				}
				if (ShotHandler.shots.Count > 0 && ShotHandler.shots.Where((Shot s) => (Entity)(object)s.customer.Ped == (Entity)(object)_ped).Count() != 0)
				{
					return true;
				}
				if (angryPeds.Count > 0 && angryPeds.FindAll((Ped r) => (Entity)(object)r == (Entity)(object)_ped).Count != 0)
				{
					return true;
				}
				if (pigs.Count > 0 && pigs.FindAll((Pig r) => (Entity)(object)r.Ped == (Entity)(object)_ped).Count != 0)
				{
					return true;
				}
				if (aDealer != null)
				{
					if ((Entity)(object)aDealer.dPed != (Entity)null && (Entity)(object)aDealer.dPed == (Entity)(object)_ped)
					{
						return true;
					}
					if ((Entity)(object)aDealer.cPed != (Entity)null && (Entity)(object)aDealer.cPed == (Entity)(object)_ped)
					{
						return true;
					}
					if (aDealer.LastCustomer == ((Entity)_ped).Handle)
					{
						return true;
					}
				}
				if (ZeesDealers != null && ((Entity)(object)ZeesDealers.lieut == (Entity)(object)_ped || (Entity)(object)ZeesDealers.goon == (Entity)(object)_ped))
				{
					return true;
				}
				if ((Entity)(object)attacker != (Entity)null && (Entity)(object)_ped == (Entity)(object)attacker)
				{
					return true;
				}
				if (DealerHandler.dealers.Count > 0)
				{
					foreach (PlayerDealer d in DealerHandler.dealers)
					{
						if (d != null)
						{
							if ((Entity)(object)d.Ped != (Entity)null && (Entity)(object)d.Ped == (Entity)(object)_ped)
							{
								return true;
							}
							if ((Entity)(object)d.cPed != (Entity)null && (Entity)(object)d.cPed == (Entity)(object)_ped)
							{
								return true;
							}
							if ((Entity)(object)d.lPed == (Entity)(object)_ped)
							{
								return true;
							}
						}
					}
				}
				if (driveByHandler.GetDriveBies.Count > 0)
				{
					foreach (DriveBy getDriveBy in driveByHandler.GetDriveBies)
					{
						if (getDriveBy.Peds.Contains(_ped))
						{
							return true;
						}
					}
				}
				if (!Function.Call<bool>((Hash)5176593366545292405L, (InputArgument)(_ped)))
				{
					return true;
				}
				int type = Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (InputArgument)(((Entity)_ped).Handle));
				if (type == 24 || type == 26)
				{
					return true;
				}
				if (!_allowPolice && LsFunctions.IsPolice(_ped))
				{
					return true;
				}
				return false;
			}
		}
		return true;
	}

	public void HandleAreaDealers()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (aDealer != null && aDealer.NewState == Enums.DStates.TalkToPlayer && aDealer.CurrentState == Enums.DStates.TalkToPlayer && player.Character.IsStopped)
		{
			Vector3 position = ((Entity)aDealer.dPed).Position;
			if (position.DistanceTo(playerPos) < 2f)
			{
				aDealer.PromtPlayerToTalk();
			}
		}
	}

	public static void MoveCustomer(Customer c, Vector3 target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (c.Target != target)
		{
			c.Target = target;
		}
		if (c.Target == target)
		{
			Vector3 position = ((Entity)c.Ped).Position;
			if (position.DistanceTo(c.Target) > 20f || Function.Call<bool>((Hash)1322919935073282825L, (InputArgument)(((Entity)c.Ped).Position.X), (InputArgument)(((Entity)c.Ped).Position.Y), (InputArgument)(((Entity)c.Ped).Position.Z)))
			{
				c.Ped.Task.RunTo(c.Target);
			}
			else
			{
				c.Ped.Task.GoTo(c.Target);
			}
		}
	}

	private static float MaxSpeed()
	{
		return 0f;
	}

	public static Vector3 GenerateSpawnPos(Vector3 desiredPos, Nodetype roadtype, bool sidewalk)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected Obj, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 finalpos = Vector3.Zero;
		bool ForceOffroad = false;
		OutputArgument outArgA = new OutputArgument();
		int NodeNumber = 1;
		int type = 0;
		if (roadtype == Nodetype.AnyRoad)
		{
			type = 1;
		}
		if (roadtype == Nodetype.Road)
		{
			type = 0;
		}
		if (roadtype == Nodetype.Offroad)
		{
			type = 1;
			ForceOffroad = true;
		}
		if (roadtype == Nodetype.Water)
		{
			type = 3;
		}
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (InputArgument)(desiredPos.X), (InputArgument)(desiredPos.Y), (InputArgument)(desiredPos.Z), (InputArgument)(NodeNumber), (InputArgument)(type), (InputArgument)(200f), (InputArgument)(200f));
		if (ForceOffroad)
		{
			while (!Function.Call<bool>((Hash)5715191804072465017L, (InputArgument)(NodeID)))
			{
				NodeID = Function.Call<int>((Hash)2510518586829603349L, (InputArgument)(desiredPos.X), (InputArgument)(desiredPos.Y), (InputArgument)(desiredPos.Z), (InputArgument)(NodeNumber), (InputArgument)(type), (InputArgument)(200f), (InputArgument)(200f));
				NodeNumber++;
			}
		}
		Function.Call((Hash)8084282276450675138L, (InputArgument)(NodeID), (InputArgument)(object)outArgA);
		finalpos = outArgA.GetResult<Vector3>();
		if (sidewalk)
		{
			finalpos = World.GetNextPositionOnSidewalk(finalpos);
		}
		return finalpos;
	}

	public Vector3 ClosetNodePos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected Obj, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		_ = Vector3.Zero;
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (InputArgument)(pos.X), (InputArgument)(pos.Y), (InputArgument)(pos.Z), (InputArgument)(1), (InputArgument)(1), (InputArgument)(200f), (InputArgument)(200f));
		OutputArgument outArgA = new OutputArgument();
		Function.Call((Hash)8084282276450675138L, (InputArgument)(NodeID), (InputArgument)(object)outArgA);
		return outArgA.GetResult<Vector3>();
	}

	private int PedsInPlayersGroup()
	{
		return player.Character.CurrentPedGroup.ToList(false).Count;
	}

	public static List<PathnodeFlags> GetRoadFlags(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<PathnodeFlags> pathnodesflags = new List<PathnodeFlags>();
		OutputArgument outArgA = new OutputArgument();
		OutputArgument outArgB = new OutputArgument();
		if (Function.Call<bool>((Hash)389656384451763932L, (InputArgument)(pos.X), (InputArgument)(pos.Y), (InputArgument)(pos.Z), (InputArgument)(object)outArgA, (InputArgument)(object)outArgB))
		{
			outArgA.GetResult<int>();
			int flags = outArgB.GetResult<int>();
			foreach (PathnodeFlags flag in Enum.GetValues(typeof(PathnodeFlags)).Cast<PathnodeFlags>())
			{
				if (((uint)flag & (uint)flags) != 0)
				{
					pathnodesflags.Add(flag);
				}
			}
		}
		return pathnodesflags;
	}

	public static bool RoadHasFlag(Vector3 pos, PathnodeFlags flag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		OutputArgument outArgA = new OutputArgument();
		OutputArgument outArgB = new OutputArgument();
		if (Function.Call<bool>((Hash)389656384451763932L, (InputArgument)(pos.X), (InputArgument)(pos.Y), (InputArgument)(pos.Z), (InputArgument)(object)outArgA, (InputArgument)(object)outArgB))
		{
			outArgA.GetResult<int>();
			if (((uint)outArgB.GetResult<int>() & (uint)flag) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IntHasFlag(int number, int flag)
	{
		if ((number & flag) != 0)
		{
			return true;
		}
		return false;
	}

	private void OnAbort(object sender, EventArgs e)
	{
if (ShotHandler.shots.Count > 0)
		{
			foreach (Shot s in ShotHandler.shots)
			{
				if (s.customer != null)
				{
					LsFunctions.RemovePedFromWorld(s.customer.Ped, clearTask: true);
				}
			}
			ShotHandler.shots.Clear();
		}
		StashHouseHandler houseHandler = HouseHandler;
		if (houseHandler != null && houseHandler.Houses.Count > 0)
		{
			foreach (StashHouse house in HouseHandler.Houses)
			{
				house?.RemoveBlip();
			}
		}
		if ((Entity)(object)drugDealer1 != (Entity)null)
		{
			Blip currentBlip = ((Entity)drugDealer1).CurrentBlip;
			if (currentBlip != null)
			{
				currentBlip.Remove();
			}
			((Entity)drugDealer1).Delete();
		}
		if ((Entity)(object)weaponDealer1 != (Entity)null)
		{
			Blip currentBlip2 = ((Entity)weaponDealer1).CurrentBlip;
			if (currentBlip2 != null)
			{
				currentBlip2.Remove();
			}
			((Entity)weaponDealer1).Delete();
		}
		if (customers.Count >= 1)
		{
			foreach (Customer c in customers.ToList())
			{
				if (c != null)
				{
					Ped ped = c.Ped;
					Blip currentBlip3 = ((Entity)ped).CurrentBlip;
					if (currentBlip3 != null)
					{
						currentBlip3.Remove();
					}
					((Entity)ped).Delete();
				}
			}
		}
		if (pStashVehicle != null && (Entity)(object)pStashVehicle.drugCar != (Entity)null)
		{
			if ((Entity)(object)pStashVehicle.drugCar != (Entity)null && ((Entity)pStashVehicle.drugCar).Exists())
			{
				((Entity)pStashVehicle.drugCar).CurrentBlip.Remove();
				((Entity)pStashVehicle.drugCar).Delete();
			}
			Blip drugCarBlip = pStashVehicle.drugCarBlip;
			if (drugCarBlip != null)
			{
				drugCarBlip.Remove();
			}
		}
		if (wepDealer != null)
		{
			if ((Entity)(object)wepDealer.vehicle != (Entity)null)
			{
				((Entity)wepDealer.vehicle).CurrentBlip.Remove();
				((Entity)wepDealer.vehicle).Delete();
			}
			if ((Entity)(object)wepDealer.ped != (Entity)null)
			{
				((Entity)wepDealer.ped).Delete();
			}
		}
		if (aDealer != null)
		{
			if ((Entity)(object)aDealer.dPed != (Entity)null)
			{
				Blip currentBlip4 = ((Entity)aDealer.dPed).CurrentBlip;
				if (currentBlip4 != null)
				{
					currentBlip4.Remove();
				}
				((Entity)aDealer.dPed).Delete();
			}
			if ((Entity)(object)aDealer.cPed != (Entity)null)
			{
				Blip currentBlip5 = ((Entity)aDealer.cPed).CurrentBlip;
				if (currentBlip5 != null)
				{
					currentBlip5.Remove();
				}
				((Entity)aDealer.cPed).Delete();
			}
		}
		if (DealerHandler.dealers.Count > 0)
		{
			foreach (PlayerDealer pd in DealerHandler.dealers)
			{
				if ((Entity)(object)pd.Ped != (Entity)null)
				{
					LsFunctions.NotImmuneToPlayer(pd.Ped);
					Ped cPed = pd.cPed;
					if (cPed != null)
					{
						Blip currentBlip6 = ((Entity)cPed).CurrentBlip;
						if (currentBlip6 != null)
						{
							currentBlip6.Remove();
						}
					}
					Ped cPed2 = pd.cPed;
					if (cPed2 != null)
					{
						((Entity)cPed2).Delete();
					}
					Ped ped2 = pd.Ped;
					if (ped2 != null)
					{
						Blip currentBlip7 = ((Entity)ped2).CurrentBlip;
						if (currentBlip7 != null)
						{
							currentBlip7.Remove();
						}
					}
					((Entity)pd.Ped).Delete();
				}
				else
				{
					pd.RemoveLocationBlip();
				}
			}
		}
		World.RemoveRelationshipGroup(rival_nutral);
		World.RemoveRelationshipGroup(rival_enemy);
		World.RemoveRelationshipGroup(CustomerRel);
		driveByHandler.CleanDriveBys();
		if (pigs.Count > 0)
		{
			foreach (Pig pig in pigs)
			{
				Ped ped3 = pig.Ped;
				if (ped3 != null)
				{
					Blip currentBlip8 = ((Entity)ped3).CurrentBlip;
					if (currentBlip8 != null)
					{
						currentBlip8.Remove();
					}
				}
				Ped ped4 = pig.Ped;
				if (ped4 != null)
				{
					((Entity)ped4).Delete();
				}
				Vehicle vehicle = pig.Vehicle;
				if (vehicle != null)
				{
					Blip currentBlip9 = ((Entity)vehicle).CurrentBlip;
					if (currentBlip9 != null)
					{
						currentBlip9.Remove();
					}
				}
				((Entity)pig.Vehicle).Delete();
			}
		}
		pickupHandler.RemoveBags();
		OldCustomerHandler.ClearOldCustomers();
	}

	static LSL()
	{
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
	}
}
