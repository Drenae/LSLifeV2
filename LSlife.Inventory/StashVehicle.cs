using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class StashVehicle
{
	public bool drugCarWanted;

	public bool drugCarDestroyed = true;

	private bool drugCarSaved = true;

	public Blip drugCarBlip;

	public int drugCarWeed;

	public int drugCarWeedOz;

	public int drugCarCrack;

	public int drugCarCrackOz;

	public int drugCarCocain;

	public int drugCarCocainOz;

	public int drugCarMoney;

	public int drugCarArmour;

	public Vehicle drugCar;

	public Vector3 CurrentPosition;

	private string reg;

	private int drugCarHash = -1;

	public List<XElement> vehWeapons = new List<XElement>();

	public List<dynamic> vehWeps = new List<object>();

	public StashVehicle()
	{
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "drugCarStash"))
			{
				continue;
			}
			bool armourFound = false;
			bool foundOz = false;
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "weed")
				{
					int.TryParse(e.Value, out drugCarWeed);
				}
				if (e.Name == "weedOz")
				{
					int.TryParse(e.Value, out drugCarWeedOz);
					foundOz = true;
				}
				if (e.Name == "crack")
				{
					int.TryParse(e.Value, out drugCarCrack);
				}
				if (e.Name == "crackOz")
				{
					int.TryParse(e.Value, out drugCarCrackOz);
					foundOz = true;
				}
				if (e.Name == "cocain")
				{
					int.TryParse(e.Value, out drugCarCocain);
				}
				if (e.Name == "cocainOz")
				{
					int.TryParse(e.Value, out drugCarCocainOz);
					foundOz = true;
				}
				if (e.Name == "money")
				{
					int.TryParse(e.Value, out drugCarMoney);
				}
				if (e.Name == "armour")
				{
					armourFound = true;
					int.TryParse(e.Value, out drugCarArmour);
				}
			}
			if (!armourFound)
			{
				element.Add(new XElement("armour", 0));
				LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			}
			if (!foundOz)
			{
				element.Add(new XElement("weedOz", 0));
				element.Add(new XElement("crackOz", 0));
				element.Add(new XElement("cocainOz", 0));
				LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			}
		}
		foreach (XElement element2 in LSL.VehicleXml.Descendants())
		{
			if (!(element2.Name == "Vehicle"))
			{
				continue;
			}
			foreach (XElement e2 in element2.Elements())
			{
				if (e2.Name == "Destroyed")
				{
					bool.TryParse(e2.Value, out drugCarDestroyed);
				}
				if (e2.Name == "Hash")
				{
					int.TryParse(e2.Value, out drugCarHash);
				}
				if (e2.Name == "PosX")
				{
					float.TryParse(e2.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out CurrentPosition.X);
					if (CurrentPosition.X == 0f)
					{
						float.TryParse(e2.Value, out CurrentPosition.X);
					}
				}
				if (e2.Name == "PosY")
				{
					float.TryParse(e2.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out CurrentPosition.Y);
					if (CurrentPosition.Y == 0f)
					{
						float.TryParse(e2.Value, out CurrentPosition.Y);
					}
				}
				if (e2.Name == "PosZ")
				{
					float.TryParse(e2.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out CurrentPosition.Z);
					if (CurrentPosition.Z == 0f)
					{
						float.TryParse(e2.Value, out CurrentPosition.Z);
					}
				}
				if (e2.Name == "NumPlate")
				{
					reg = e2.Value;
				}
			}
		}
		foreach (XElement element3 in LSL.vehWeaponsXML.Descendants())
		{
			if (!(element3.Name == "weapon"))
			{
				continue;
			}
			foreach (XElement e3 in element3.Elements())
			{
				if (e3.Name == "HASH")
				{
					vehWeapons.Add(element3);
					vehWeps.Add(e3.Value);
				}
			}
		}
	}

	internal void AddDrugs()
	{
		drugCarWeed += LSL.PlayerInventory["Weed"];
		LSL.PlayerInventory["Weed"] = 0;
		drugCarCrack += LSL.PlayerInventory["Crack"];
		LSL.PlayerInventory["Crack"] = 0;
		drugCarCocain += LSL.PlayerInventory["Cocaine"];
		LSL.PlayerInventory["Cocaine"] = 0;
		drugCarWeedOz += LsFunctions.pWeedOunce;
		LsFunctions.pWeedOunce = 0;
		drugCarCrackOz += LsFunctions.pCrackOunce;
		LsFunctions.pCrackOunce = 0;
		drugCarCocainOz += LsFunctions.pCocaineOunce;
		LsFunctions.pCocaineOunce = 0;
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (element.Name == "playerInventory")
			{
				foreach (XElement e in element.Elements())
				{
					if (e.Name == "weed")
					{
						e.Value = LSL.PlayerInventory["Weed"].ToString();
					}
					else if (e.Name == "crack")
					{
						e.Value = LSL.PlayerInventory["Crack"].ToString();
					}
					else if (e.Name == "cocain")
					{
						e.Value = LSL.PlayerInventory["Cocaine"].ToString();
					}
					else if (e.Name == "weedOz")
					{
						e.Value = LsFunctions.pWeedOunce.ToString();
					}
					else if (e.Name == "crackOz")
					{
						e.Value = LsFunctions.pCrackOunce.ToString();
					}
					else if (e.Name == "cocaineOz")
					{
						e.Value = LsFunctions.pCocaineOunce.ToString();
					}
				}
			}
			else
			{
				if (!(element.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement e2 in element.Elements())
				{
					if (e2.Name == "weed")
					{
						e2.Value = drugCarWeed.ToString();
					}
					else if (e2.Name == "crack")
					{
						e2.Value = drugCarCrack.ToString();
					}
					else if (e2.Name == "cocain")
					{
						e2.Value = drugCarCocain.ToString();
					}
					else if (e2.Name == "weedOz")
					{
						e2.Value = drugCarWeedOz.ToString();
					}
					else if (e2.Name == "crackOz")
					{
						e2.Value = drugCarCrackOz.ToString();
					}
					else if (e2.Name == "cocaineOz")
					{
						e2.Value = drugCarCocainOz.ToString();
					}
				}
			}
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		UpdateCarStash();
	}

	public Vector3 StashPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Entity)drugCar).Position;
		Vector3 val = ((Entity)drugCar).ForwardVector * -1f;
		Model model = ((Entity)drugCar).Model;
		return Vector3.Add(position, Vector3.Multiply(val, VehicleCompat.HalfLength(model) + 0.5f));
	}

	public void OnTick()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!((Entity)(object)drugCar != (Entity)null) || !((Entity)drugCar).Exists())
		{
			return;
		}
		Vector3 backPos = StashPos();
		if (LSL.DrawMarkers && (Entity)(object)drugCar.Driver != (Entity)(object)LSL.player.Character && LSL.playerPos.DistanceTo(((Entity)drugCar).Position) < 10f)
		{
			World.DrawMarker((MarkerType)0, backPos, Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
		}
		if (!((Entity)drugCar).AttachedBlip.IsShortRange)
		{
			((Entity)drugCar).AttachedBlip.IsShortRange = true;
			((Entity)drugCar).AttachedBlip.Sprite = (BlipSprite)225;
			((Entity)drugCar).AttachedBlip.Name = "Drug Car";
			((Entity)drugCar).AttachedBlip.Color = (BlipColor)2;
		}
		if (LSL.player.Character.IsInVehicle())
		{
			return;
		}
		Vector3 position = ((Entity)LSL.player.Character).Position;
		if (!(position.DistanceTo(backPos) <= 1f))
		{
			return;
		}
		bool trunkFound = false;
		if (((Entity)drugCar).HasBone("boot"))
		{
			trunkFound = true;
		}
		if (trunkFound)
		{
			if (!VehicleCompat.IsDoorOpen(drugCar, 5))
			{
				if ((float)Game.GameTime > LSL.displayHelpTimer + 5000f && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
				{
					LSL.displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour ouvrir le coffre.");
				}
				if (Game.IsControlJustPressed((Control)21) && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
				{
					VehicleCompat.OpenDoor(drugCar, 5, false, false);
					UpdateCarStash();
					CustomStashMenu.OpenVehicle();
				}
			}
			else
			{
				if ((float)Game.GameTime > LSL.displayHelpTimer + 5000f && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
				{
					LSL.displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour fermer le coffre.");
				}
				if (Game.IsControlJustPressed((Control)21) && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
				{
					VehicleCompat.CloseDoor(drugCar, 5, false);
				}
			}
		}
		else if (!VehicleCompat.IsDoorOpen(drugCar, 2) && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
			{
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour ouvrir les portes arrière.");
			}
			if (Game.IsControlJustPressed((Control)21) && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
			{
				VehicleCompat.OpenDoor(drugCar, 2, true, false);
				VehicleCompat.OpenDoor(drugCar, 3, true, false);
				UpdateCarStash();
				CustomStashMenu.OpenVehicle();
			}
		}
		else
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
			{
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour fermer les portes arrière.");
			}
			if (Game.IsControlJustPressed((Control)21) && !CustomMenuShared.IsAnyGameplayMenuOpen() && LSL.player.Character.IsStopped)
			{
				VehicleCompat.CloseDoor(drugCar, 2, false);
				VehicleCompat.CloseDoor(drugCar, 3, false);
			}
		}
	}

	public void OnDelayedTick()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		if (!drugCarDestroyed && ((Entity)(object)drugCar == (Entity)null || !((Entity)drugCar).Exists()))
		{
			if (LSL.playerPos.DistanceTo(CurrentPosition) <= 200f)
			{
				if (drugCarBlip != (Blip)null)
				{
					drugCarBlip.Delete();
					drugCarBlip = null;
				}
				Vehicle[] allVehicles = World.GetAllVehicles();
				foreach (Vehicle veh in allVehicles)
				{
					Vector3 position = ((Entity)veh).Position;
					if (!(position.DistanceTo(CurrentPosition) < 6f) || !(veh.NumberPlate == reg))
					{
						continue;
					}
					Model model = ((Entity)veh).Model;
					if (model.Hash == drugCarHash)
					{
						if (((Entity)veh).AttachedBlip != (Blip)null)
						{
							((Entity)veh).AttachedBlip.Delete();
						}
						drugCar = veh;
						((Entity)drugCar).IsPersistent = true;
						drugCar.EngineHealth = 1000f;
						((Entity)drugCar).Health = 1000;
						drugCar.Repair();
						if (((Entity)drugCar).AttachedBlip != (Blip)null)
						{
							((Entity)drugCar).AttachedBlip.Delete();
						}
						((Entity)drugCar).AddBlip();
						((Entity)drugCar).AttachedBlip.IsShortRange = true;
						((Entity)drugCar).AttachedBlip.IsFriendly = true;
						((Entity)drugCar).AttachedBlip.Sprite = (BlipSprite)225;
						((Entity)drugCar).AttachedBlip.Name = "Drug Car";
						((Entity)drugCar).AttachedBlip.Color = (BlipColor)2;
					}
				}
				if ((Entity)(object)drugCar == (Entity)null || !((Entity)drugCar).Exists())
				{
					if (LSL.DEBUG)
					{
						UI.Notify("Tentative de chargement du véhicule");
					}
					VehicleLoad();
				}
			}
			else if (drugCarBlip == (Blip)null)
			{
				drugCarBlip = World.CreateBlip(CurrentPosition);
				drugCarBlip.IsShortRange = true;
				drugCarBlip.IsFriendly = true;
				drugCarBlip.Sprite = (BlipSprite)225;
				drugCarBlip.Name = "Drug Car";
				drugCarBlip.Color = (BlipColor)2;
			}
		}
		if (!((Entity)(object)drugCar != (Entity)null) || !((Entity)drugCar).Exists())
		{
			return;
		}
		if (((Entity)drugCar).AttachedBlip.Exists() && (Entity)(object)drugCar.Driver == (Entity)(object)LSL.player.Character)
		{
			((Entity)drugCar).AttachedBlip.Delete();
		}
		if (LSL.player.Character.IsInVehicle())
		{
			if (LSL.playerWanted && ((Entity)drugCar).Handle == ((Entity)LSL.player.Character.CurrentVehicle).Handle && !drugCarWanted)
			{
				drugCarWanted = true;
			}
			if (drugCarSaved && (Entity)(object)drugCar.Driver == (Entity)(object)LSL.player.Character)
			{
				CurrentPosition = ((Entity)drugCar).Position;
				drugCarSaved = false;
				if (((Entity)drugCar).IsInvincible)
				{
					((Entity)drugCar).IsInvincible = false;
				}
			}
		}
		if (!LSL.playerWanted && drugCarWanted)
		{
			drugCarWanted = false;
		}
		if (!drugCarSaved && CurrentPosition != ((Entity)drugCar).Position && !LSL.player.Character.IsInVehicle())
		{
			CurrentPosition = ((Entity)drugCar).Position;
			VehicleSave(drugCar);
			drugCarSaved = true;
		}
		if (((Entity)drugCar).IsDead || drugCar.EngineHealth <= -500f)
		{
			LsFunctions.DbugString("Vehicle Destroyed");
			drugCarWanted = false;
			((Entity)drugCar).IsPersistent = false;
			((Entity)drugCar).MarkAsNoLongerNeeded();
			((Entity)drugCar).AttachedBlip.Delete();
			drugCar = null;
			drugCarDestroyed = true;
			LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element in LSL.LSLifeSave.Descendants())
			{
				if (!(element.Name == "drugCarStash"))
				{
					continue;
				}
				foreach (XElement carStashEle in element.Elements())
				{
					if (carStashEle.Name == "weed")
					{
						carStashEle.Value = "0";
					}
					if (carStashEle.Name == "crack")
					{
						carStashEle.Value = "0";
					}
					if (carStashEle.Name == "cocain")
					{
						carStashEle.Value = "0";
					}
					if (carStashEle.Name == "money")
					{
						carStashEle.Value = "0";
					}
				}
			}
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			XDocument veh2 = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
			foreach (XElement element2 in veh2.Descendants())
			{
				if (element2.Name == "Destroyed")
				{
					element2.Value = drugCarDestroyed.ToString();
				}
			}
			veh2.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
		}
		if (drugCarDestroyed || !((Entity)(object)drugCar != (Entity)null) || !(LSL.playerPos.DistanceTo(CurrentPosition) > 200f) || !((Entity)drugCar).Exists())
		{
			return;
		}
		if (LSL.player.Character.IsInVehicle() && (Entity)(object)LSL.player.Character.CurrentVehicle != (Entity)(object)drugCar)
		{
			if (drugCarBlip == (Blip)null)
			{
				drugCarBlip = World.CreateBlip(((Entity)drugCar).Position);
				drugCarBlip.IsShortRange = true;
				drugCarBlip.IsFriendly = true;
				drugCarBlip.Sprite = (BlipSprite)225;
				drugCarBlip.Name = "Drug Car";
				drugCarBlip.Color = (BlipColor)2;
			}
			((Entity)drugCar).AttachedBlip.Delete();
			((Entity)drugCar).Delete();
			drugCar = null;
		}
		if (!LSL.player.Character.IsInVehicle())
		{
			if (drugCarBlip == (Blip)null)
			{
				drugCarBlip = World.CreateBlip(((Entity)drugCar).Position);
				drugCarBlip.IsShortRange = true;
				drugCarBlip.IsFriendly = true;
				drugCarBlip.Sprite = (BlipSprite)225;
				drugCarBlip.Name = "Drug Car";
				drugCarBlip.Color = (BlipColor)2;
			}
			((Entity)drugCar).AttachedBlip.Delete();
			((Entity)drugCar).Delete();
			drugCar = null;
		}
	}

	public int TotalDrugs()
	{
		return drugCarWeed + drugCarCrack + drugCarCocain;
	}

	public void SpawnCar()
	{
		if ((Entity)(object)drugCar == (Entity)null)
		{
			VehicleLoad();
		}
	}

	public void RemoveOldCar()
	{
		drugCarDestroyed = true;
		if (drugCarBlip != (Blip)null)
		{
			drugCarBlip.Delete();
		}
		((Entity)drugCar).IsPersistent = false;
		((Entity)drugCar).MarkAsNoLongerNeeded();
		if (((Entity)drugCar).AttachedBlip != (Blip)null)
		{
			((Entity)drugCar).AttachedBlip.Delete();
		}
		XDocument veh = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
		foreach (XElement element in veh.Descendants())
		{
			if (element.Name == "Destroyed")
			{
				element.Value = drugCarDestroyed.ToString();
			}
		}
		veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
		drugCar = null;
	}

	public void ImpoundCar()
	{
		if (!drugCarWanted || !((Entity)(object)drugCar != (Entity)null))
		{
			return;
		}
		LsFunctions.SendTextMsg("Le véhicule de stockage a été confisqué et son contenu détruit.");
		drugCarDestroyed = true;
		XDocument veh = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
		foreach (XElement element in veh.Descendants())
		{
			if (element.Name == "Destroyed")
			{
				element.Value = drugCarDestroyed.ToString();
				break;
			}
		}
		veh.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
		drugCarWanted = false;
		((Entity)drugCar).IsPersistent = false;
		((Entity)drugCar).MarkAsNoLongerNeeded();
		((Entity)drugCar).AttachedBlip.Delete();
		((Entity)drugCar).Delete();
		drugCar = null;
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element2 in LSL.LSLifeSave.Descendants())
		{
			if (element2.Name == "drugCar")
			{
				foreach (XElement drugCarEle in element2.Elements())
				{
					if (drugCarEle.Name == "HASH")
					{
						drugCarEle.Value = "";
						break;
					}
				}
			}
			if (!(element2.Name == "drugCarStash"))
			{
				continue;
			}
			foreach (XElement carStashEle in element2.Elements())
			{
				if (carStashEle.Name == "weed")
				{
					carStashEle.Value = "0";
				}
				if (carStashEle.Name == "crack")
				{
					carStashEle.Value = "0";
				}
				if (carStashEle.Name == "cocain")
				{
					carStashEle.Value = "0";
				}
				if (carStashEle.Name == "money")
				{
					carStashEle.Value = "0";
				}
			}
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
	}

	private void VehicleLoad()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		string drugCarName = "";
		float drugCarHead = 0f;
		VehicleColor drugCarPriCol = (VehicleColor)140;
		VehicleColor drugCarSecCol = (VehicleColor)140;
		int health = 1000;
		int engineHealth = 1000;
		VehicleWheelType wheelType = (VehicleWheelType)5;
		VehicleColor rimCol = (VehicleColor)117;
		bool canTiresBurst = false;
		NumberPlateType plateType = (NumberPlateType)0;
		string plate = "";
		VehicleWindowTint tint = (VehicleWindowTint)0;
		float dirt = 0f;
		float fuel = 0f;
		int livery = -1;
		bool customTire = false;
		XDocument vehiclesXML = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
		foreach (XElement veh in vehiclesXML.Descendants())
		{
			if (!(veh.Name == "Vehicle"))
			{
				continue;
			}
			foreach (XElement e in veh.Elements())
			{
				if (e.Name == "Name")
				{
					drugCarName = e.Value;
				}
				if (e.Name == "Hash")
				{
					int.TryParse(e.Value, out drugCarHash);
				}
				if (e.Name == "PosX")
				{
					float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var x);
					if (x == 0f)
					{
						float.TryParse(e.Value, out x);
					}
					if (x != 0f)
					{
						CurrentPosition.X = x;
					}
				}
				if (e.Name == "PosY")
				{
					float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var y);
					if (y == 0f)
					{
						float.TryParse(e.Value, out y);
					}
					if (y != 0f)
					{
						CurrentPosition.Y = y;
					}
				}
				if (e.Name == "PosZ")
				{
					float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var z);
					if (z == 0f)
					{
						float.TryParse(e.Value, out z);
					}
					if (z != 0f)
					{
						CurrentPosition.Z = z;
					}
				}
				if (e.Name == "Heading")
				{
					float.TryParse(e.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out drugCarHead);
					if (drugCarHead == 0f)
					{
						float.TryParse(e.Value, out drugCarHead);
					}
				}
				if (e.Name == "PrimaryCol")
				{
					Enum.TryParse<VehicleColor>(e.Value, out drugCarPriCol);
				}
				if (e.Name == "SecondaryCol")
				{
					Enum.TryParse<VehicleColor>(e.Value, out drugCarSecCol);
				}
				if (e.Name == "Health")
				{
					int.TryParse(e.Value, out health);
					if (health < 100)
					{
						health = 1000;
					}
				}
				if (e.Name == "EngineHealth")
				{
					int.TryParse(e.Value, out engineHealth);
					if (engineHealth < 100)
					{
						engineHealth = 1000;
					}
				}
				if (e.Name == "WheelType")
				{
					Enum.TryParse<VehicleWheelType>(e.Value, out wheelType);
				}
				if (e.Name == "RimCol")
				{
					Enum.TryParse<VehicleColor>(e.Value, out rimCol);
				}
				if (e.Name == "NumPlateType")
				{
					Enum.TryParse<NumberPlateType>(e.Value, out plateType);
				}
				if (e.Name == "NumPlate")
				{
					plate = (reg = e.Value);
				}
				if (e.Name == "WindowTint")
				{
					Enum.TryParse<VehicleWindowTint>(e.Value, out tint);
				}
				if (e.Name == "DirtLevel")
				{
					float.TryParse(e.Value, out dirt);
				}
				if (e.Name == "CanTiresBurst")
				{
					bool.TryParse(e.Value, out canTiresBurst);
				}
				if (e.Name == "Fuel")
				{
					float.TryParse(e.Value, out fuel);
				}
				if (e.Name == "Livery")
				{
					int.TryParse(e.Value, out livery);
				}
				if (e.Name == "CustomTire")
				{
					bool.TryParse(e.Value, out customTire);
				}
			}
		}
		Model model = new Model(drugCarName);
		List<Vehicle> vehicles = World.GetAllVehicles().ToList().FindAll((Vehicle v) =>
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (v.NumberPlate == reg)
			{
				Model model2 = ((Entity)v).Model;
				return model2.Hash == drugCarHash;
			}
			return false;
		});
		if (LSL.DEBUG)
		{
			UI.Notify(vehicles.Count.ToString() ?? "");
			Vector3 currentPosition = CurrentPosition;
			UI.Notify("~r~Coordonnées attendues : " + ((object)currentPosition/*cast due to constrained. prefix*/).ToString());
		}
		if (vehicles.Count == 0)
		{
			if (model.IsInCdImage)
			{
				drugCar = World.CreateVehicle(new Model(drugCarName), CurrentPosition, drugCarHead);
			}
			else
			{
				model = new Model(drugCarHash);
				if (model.IsInCdImage)
				{
					drugCar = World.CreateVehicle(model, CurrentPosition, drugCarHead);
				}
			}
		}
		else
		{
			drugCar = vehicles[0];
		}
		Script.Wait(5);
		if (LSL.DEBUG)
		{
			string text = drugCarName;
			Vector3 currentPosition = CurrentPosition;
			UI.Notify(text + " | " + ((object)currentPosition/*cast due to constrained. prefix*/).ToString());
		}
		if ((Entity)(object)drugCar != (Entity)null)
		{
			drugCar.InstallModKit();
			drugCar.PrimaryColor = drugCarPriCol;
			drugCar.SecondaryColor = drugCarSecCol;
			((Entity)drugCar).Health = health;
			drugCar.EngineHealth = engineHealth;
			drugCar.WheelType = wheelType;
			drugCar.EngineRunning = false;
			((Entity)drugCar).IsInvincible = false;
			drugCar.CanTiresBurst = canTiresBurst;
			drugCar.NumberPlateType = plateType;
			drugCar.NumberPlate = plate;
			drugCar.WindowTint = tint;
			drugCar.DirtLevel = dirt;
			drugCar.FuelLevel = fuel;
			drugCar.Livery = livery;
			((Entity)drugCar).IsPersistent = true;
			drugCar.RimColor = rimCol;
			drugCar.Repair();
			if (((Entity)drugCar).AttachedBlip != (Blip)null)
			{
				((Entity)drugCar).AttachedBlip.Delete();
			}
			((Entity)drugCar).AddBlip();
			((Entity)drugCar).AttachedBlip.IsShortRange = true;
			((Entity)drugCar).AttachedBlip.IsFriendly = true;
			((Entity)drugCar).AttachedBlip.Sprite = (BlipSprite)225;
			((Entity)drugCar).AttachedBlip.Name = "Drug Car";
			((Entity)drugCar).AttachedBlip.Color = (BlipColor)2;
			{
				foreach (XElement veh2 in vehiclesXML.Descendants())
				{
					if (veh2.Name == "Mods")
					{
						foreach (XElement m in veh2.Elements())
						{
							if (m.Name == "FrontWheels")
							{
								Enum.TryParse<VehicleMod>(m.Name.ToString(), out VehicleMod mod);
								int.TryParse(m.Value, out var val);
								drugCar.SetMod(mod, val, customTire);
								drugCar.RimColor = rimCol;
							}
							else
							{
								Enum.TryParse<VehicleMod>(m.Name.ToString(), out VehicleMod mod2);
								int.TryParse(m.Value, out var val2);
								drugCar.SetMod(mod2, val2, false);
							}
						}
					}
					if (!(veh2.Name == "ToggleMods"))
					{
						continue;
					}
					foreach (XElement item in veh2.Elements())
					{
						Enum.TryParse<VehicleToggleMod>(item.Name.ToString(), out VehicleToggleMod toggleMod);
						bool.TryParse(item.Value, out var toggle);
						drugCar.ToggleMod(toggleMod, toggle);
					}
				}
				return;
			}
		}
		UI.Notify("~r~Échec du chargement du véhicule");
	}

	public void VehicleSave(Vehicle veh)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		Model model = ((Entity)veh).Model;
		if (!model.IsCar)
		{
			model = ((Entity)veh).Model;
			if (!model.IsBike)
			{
				model = ((Entity)veh).Model;
				if (!model.IsBoat)
				{
					model = ((Entity)veh).Model;
					if (!model.IsHelicopter)
					{
						model = ((Entity)veh).Model;
						if (!model.IsPlane)
						{
							return;
						}
					}
				}
			}
		}
		XDocument vehiclesXML = XDocument.Load("scripts\\LSLife\\LSLife_Vehicles.xml");
		XElement rootElement = vehiclesXML.Root;
		rootElement.RemoveAll();
		XElement parentElement = new XElement("Vehicle");
		XElement modsElement = new XElement("Mods");
		XElement toggleModsElement = new XElement("ToggleMods");
		XElement neonsElement = new XElement("Neons");
		parentElement.Add(new XElement("Destroyed", "false"));
		parentElement.Add(new XElement("Handle", ((Entity)veh).Handle.ToString()));
		parentElement.Add(new XElement("Name", veh.DisplayName));
		XName name = "Hash";
		model = ((Entity)veh).Model;
		parentElement.Add(new XElement(name, model.Hash));
		parentElement.Add(new XElement("PosX", ((Entity)veh).Position.X));
		parentElement.Add(new XElement("PosY", ((Entity)veh).Position.Y));
		parentElement.Add(new XElement("PosZ", ((Entity)veh).Position.Z));
		parentElement.Add(new XElement("Heading", ((Entity)veh).Heading));
		parentElement.Add(new XElement("PrimaryCol", veh.PrimaryColor));
		parentElement.Add(new XElement("SecondaryCol", veh.SecondaryColor));
		parentElement.Add(new XElement("Health", veh.BodyHealth));
		parentElement.Add(new XElement("EngineHealth", veh.EngineHealth));
		parentElement.Add(new XElement("WheelType", veh.WheelType));
		parentElement.Add(new XElement("RimCol", veh.RimColor));
		parentElement.Add(new XElement("SmokeCol", veh.TireSmokeColor));
		parentElement.Add(new XElement("NeonCol", veh.NeonLightsColor));
		parentElement.Add(new XElement("TrimCol", veh.TrimColor));
		parentElement.Add(new XElement("DashCol", veh.DashboardColor));
		parentElement.Add(new XElement("NumPlateType", veh.NumberPlateType));
		parentElement.Add(new XElement("NumPlate", veh.NumberPlate));
		parentElement.Add(new XElement("WindowTint", veh.WindowTint));
		parentElement.Add(new XElement("DirtLevel", veh.DirtLevel));
		parentElement.Add(new XElement("CanTiresBurst", veh.CanTiresBurst));
		parentElement.Add(new XElement("Fuel", veh.FuelLevel));
		parentElement.Add(new XElement("Livery", veh.Livery));
		if (Function.Call<bool>(unchecked((Hash)(-5507252750051666468L)), (((Entity)veh).Handle), (23)))
		{
			parentElement.Add(new XElement("CustomTire", true));
		}
		else
		{
			parentElement.Add(new XElement("CustomTire", false));
		}
		foreach (VehicleMod mod in Enum.GetValues(typeof(VehicleMod)))
		{
			modsElement.Add(new XElement(((object)mod/*cast due to constrained. prefix*/).ToString(), veh.GetMod(mod)));
		}
		foreach (VehicleToggleMod mod2 in Enum.GetValues(typeof(VehicleToggleMod)))
		{
			toggleModsElement.Add(new XElement(((object)mod2/*cast due to constrained. prefix*/).ToString(), veh.IsToggleModOn(mod2)));
		}
		foreach (VehicleNeonLight neon in Enum.GetValues(typeof(VehicleNeonLight)))
		{
			neonsElement.Add(new XElement(((object)neon/*cast due to constrained. prefix*/).ToString(), veh.IsNeonLightsOn(neon)));
		}
		parentElement.Add(modsElement);
		parentElement.Add(toggleModsElement);
		parentElement.Add(neonsElement);
		rootElement.Add(parentElement);
		vehiclesXML.Save("scripts\\LSLife\\LSLife_Vehicles.xml");
		drugCar = veh;
		((Entity)drugCar).IsPersistent = true;
		if (((Entity)drugCar).AttachedBlip != (Blip)null)
		{
			((Entity)drugCar).AttachedBlip.Delete();
		}
		((Entity)drugCar).AddBlip();
		((Entity)drugCar).AttachedBlip.IsShortRange = true;
		((Entity)drugCar).AttachedBlip.IsFriendly = true;
		((Entity)drugCar).AttachedBlip.Sprite = (BlipSprite)225;
		((Entity)drugCar).AttachedBlip.Name = "Drug Car";
		((Entity)drugCar).AttachedBlip.Color = (BlipColor)2;
		drugCarDestroyed = false;
		CurrentPosition = ((Entity)drugCar).Position;
	}

	public void UpdateCarStash()
	{
		LSL.playerWeapons.Clear();
		LSL.playerWeps.Clear();
		int i = 0;
		foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
		{
			bool flag = Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (LSL.player.Character), ((int)weapon));
			int ammunitionInWeapon = Function.Call<int>((Hash)97480644549409105L, (LSL.player.Character), ((int)weapon));
			if (!flag)
			{
				continue;
			}
			XElement parentElement = new XElement("weapon");
			parentElement.SetAttributeValue("id", i);
			XElement weaponNameElement = new XElement("HASH", (object)weapon);
			XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
			parentElement.Add(weaponNameElement);
			parentElement.Add(weaponAmmoElement);
			foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
			{
				if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (LSL.player.Character), ((int)weapon), ((int)component)))
				{
					XElement weaponComponentElement = new XElement("COMPONENT", (object)component);
					parentElement.Add(weaponComponentElement);
				}
			}
			LSL.playerWeapons.Add(parentElement);
			LSL.playerWeps.Add(((object)weapon/*cast due to constrained. prefix*/).ToString());
			i++;
		}
		vehWeapons.Clear();
		vehWeps.Clear();
		LSL.vehWeaponsXML = XDocument.Load("scripts\\LSLife\\LsLife_Vehicle_Weapons.xml");
		foreach (XElement element in LSL.vehWeaponsXML.Descendants())
		{
			if (!(element.Name == "weapon"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "HASH")
				{
					vehWeapons.Add(element);
					vehWeps.Add(e.Value);
				}
			}
		}
	}
}
