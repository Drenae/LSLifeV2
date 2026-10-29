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

	public static PlayerDealerHandler DealerHandler;

	public static StashHouseHandler HouseHandler;

	public static PickupHandler pickupHandler = new PickupHandler();

	public static DriveByHandler driveByHandler = new DriveByHandler();

	public static LsLPeds lPeds;

	public static GTA.RelationshipGroup CustomerRel = World.AddRelationshipGroup("LSCUSTOMER");

	public static GTA.RelationshipGroup rival_nutral = World.AddRelationshipGroup("RIVAL_N");

	public static GTA.RelationshipGroup rival_enemy = World.AddRelationshipGroup("RIVAL_E");

	public static int playerRelationship = Function.Call<int>(unchecked((Hash)(-3292914402564945716L)), ("PLAYER"));

	public PedDebugger PedDebugger = new PedDebugger();

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

	public static GTA.UI.TextElement myUIText;

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
