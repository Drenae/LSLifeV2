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

public partial class LSL
{
	private void Setup()
	{
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
				bool saveInventoryMigration = false;
				foreach (XElement e in element.Elements())
				{
					int number;
					if (e.Name == "weed")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory["Weed"] = number;
					}
					if (e.Name == "weedOz")
						int.TryParse(e.Value, out LsFunctions.pWeedOunce);
					if (e.Name == "crack")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory["Crack"] = number;
					}
					if (e.Name == "crackOz")
						int.TryParse(e.Value, out LsFunctions.pCrackOunce);
					if (e.Name == "cocain")
					{
						int.TryParse(e.Value, out number);
						PlayerInventory["Cocaine"] = number;
					}
					if (e.Name == "cocaineOz")
						int.TryParse(e.Value, out LsFunctions.pCocaineOunce);
					if (e.Name == "armour")
						int.TryParse(e.Value, out playerArmours);
				}
				if (element.Element("armour") == null)
				{
					element.Add(new XElement("armour", playerArmours));
					saveInventoryMigration = true;
				}
				if (element.Element("weedOz") == null)
				{
					element.Add(new XElement("weedOz", LsFunctions.pWeedOunce));
					saveInventoryMigration = true;
				}
				if (element.Element("crackOz") == null)
				{
					element.Add(new XElement("crackOz", LsFunctions.pCrackOunce));
					saveInventoryMigration = true;
				}
				if (element.Element("cocaineOz") == null)
				{
					element.Add(new XElement("cocaineOz", LsFunctions.pCocaineOunce));
					saveInventoryMigration = true;
				}
				if (saveInventoryMigration)
					LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
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
		InitializeLegacyMenus();
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
