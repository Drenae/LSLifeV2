using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.OldCustomers;
using LSlife.Shots;


namespace LSlife;

internal static partial class LsFunctions
{
	public static void LoadSpawnLocations()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		foreach (XElement element in XDocument.Load("scripts\\LSLife\\LSLife_Spawns.xml").Descendants())
		{
			if (!(element.Name == "pos"))
			{
				continue;
			}
			Vector3 position = Vector3.Zero;
			float heading = 0f;
			foreach (XElement ele in element.Descendants())
			{
				if (ele.Name == "x")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.X);
				}
				if (ele.Name == "y")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.Y);
				}
				if (ele.Name == "z")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out position.Z);
				}
				if (ele.Name == "h")
				{
					float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out heading);
				}
			}
			if (position != Vector3.Zero)
			{
				spawnPostions.Add(new DealerSpawnPostion(position, heading));
			}
		}
	}

	public static void LoadZoneData()
	{
		if (File.Exists("scripts\\LSLife\\zoneData\\LsLife_LSSA.xml"))
		{
			int i = 0;
			foreach (XElement zone in XDocument.Load("scripts\\LSLife\\zoneData\\LsLife_LSSA.xml").Descendants())
			{
				if (!(zone.Name == "area"))
				{
					continue;
				}
				i++;
				string zoneName = zone.FirstAttribute.Value;
				LSL.AreaType wealth = LSL.AreaType.Normal;
				LSL.jurisdictionType police = LSL.jurisdictionType.LSPD;
				foreach (XElement data in zone.Elements())
				{
					if (data.Name == "wealth")
					{
						Enum.TryParse<LSL.AreaType>(data.Value, out wealth);
					}
					if (data.Name == "police")
					{
						Enum.TryParse<LSL.jurisdictionType>(data.Value, out police);
					}
				}
				Tuple<LSL.AreaType, LSL.jurisdictionType> zData = new Tuple<LSL.AreaType, LSL.jurisdictionType>(wealth, police);
				LSL.zoneData.Add(zoneName, zData);
			}
			DbugString("Données de Los Santos chargées : " + i + " quartiers ajoutés");
		}
		else
		{
			GTA.UI.Notification.PostTicker("~r~Fichier LsLife_LSSA.xml introuvable", false, false);
		}
		if (!LSL.LCRWMAP)
		{
			return;
		}
		if (File.Exists("scripts\\LSLife\\zoneData\\LsLife_LCRW.xml"))
		{
			int i2 = 0;
			foreach (XElement zone2 in XDocument.Load("scripts\\LSLife\\zoneData\\LsLife_LCRW.xml").Descendants())
			{
				if (!(zone2.Name == "area"))
				{
					continue;
				}
				i2++;
				string zoneName2 = zone2.FirstAttribute.Value;
				LSL.AreaType wealth2 = LSL.AreaType.Normal;
				LSL.jurisdictionType police2 = LSL.jurisdictionType.LSPD;
				foreach (XElement data2 in zone2.Elements())
				{
					if (data2.Name == "wealth")
					{
						Enum.TryParse<LSL.AreaType>(data2.Value, out wealth2);
					}
					if (data2.Name == "police")
					{
						Enum.TryParse<LSL.jurisdictionType>(data2.Value, out police2);
					}
				}
				Tuple<LSL.AreaType, LSL.jurisdictionType> zData2 = new Tuple<LSL.AreaType, LSL.jurisdictionType>(wealth2, police2);
				LSL.zoneData.Add(zoneName2, zData2);
			}
			DbugString("Données de Liberty City Rewind chargées : " + i2 + " quartiers ajoutés");
		}
		else
		{
			GTA.UI.Notification.PostTicker("~r~Fichier LsLife_LCRW.xml introuvable", false, false);
		}
	}

	public static void SaveDealer()
	{
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "dealer1"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "weed")
				{
					e.Value = LSL.dealer1Weed.ToString();
				}
				if (e.Name == "crack")
				{
					e.Value = LSL.dealer1Crack.ToString();
				}
				if (e.Name == "cocain")
				{
					e.Value = LSL.dealer1Cocain.ToString();
				}
				if (e.Name == "debt")
				{
					e.Value = LSL.dealer1Debt.ToString();
				}
				if (e.Name == "money")
				{
					e.Value = LSL.dealer1Money.ToString();
				}
				if (e.Name == "rep")
				{
					e.Value = LSL.dealer1Rep.ToString();
				}
			}
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
	}

	public static void UpdateSave()
	{
		bool changed = false;
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "dealer1"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "debt" && e.Value != LSL.dealer1Debt.ToString())
				{
					e.Value = LSL.dealer1Debt.ToString();
					changed = true;
				}
				if (e.Name == "weed" && e.Value != LSL.dealer1Weed.ToString())
				{
					e.Value = LSL.dealer1Weed.ToString();
					changed = true;
				}
				if (e.Name == "crack" && e.Value != LSL.dealer1Crack.ToString())
				{
					e.Value = LSL.dealer1Crack.ToString();
					changed = true;
				}
				if (e.Name == "cocain" && e.Value != LSL.dealer1Cocain.ToString())
				{
					e.Value = LSL.dealer1Cocain.ToString();
					changed = true;
				}
				if (e.Name == "rep" && e.Value != LSL.dealer1Rep.ToString())
				{
					e.Value = LSL.dealer1Rep.ToString();
					changed = true;
				}
				if (e.Name == "money" && e.Value != LSL.dealer1Money.ToString())
				{
					e.Value = LSL.dealer1Money.ToString();
					changed = true;
				}
			}
		}
		if (changed)
		{
			LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
			if (LSL.DEBUG)
			{
				GTA.UI.Notification.PostTicker("Sauvegarde modifiée", false, false);
			}
		}
	}

	private static void SaveAreaData()
	{
		XDocument doc = ((!File.Exists("scripts\\LSLife\\LSLifeAreas.xml")) ? new XDocument(new XElement("areas")) : XDocument.Load("scripts\\LSLife\\LSLifeAreas.xml"));
		doc.Root.RemoveAll();
		foreach (Area area in LSL.areas)
		{
			XElement aXML = new XElement("area");
			XElement nameXML = new XElement("name", area.Name);
			XElement cStrXML = new XElement("cStr", area.CopPresance);
			XElement gStrXML = new XElement("gStr", area.GangPresance);
			XElement aHeat = new XElement("aHeat", area.Heat);
			XElement aRep = new XElement("aRep", area.Reputation);
			aXML.Add(nameXML);
			aXML.Add(cStrXML);
			aXML.Add(gStrXML);
			aXML.Add(aHeat);
			aXML.Add(aRep);
			doc.Root.Add(aXML);
		}
		doc.Save("scripts\\LSLife\\LSLifeAreas.xml");
	}

	public static Prop LoadWeedPlace()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		int doorHash = 464151082;
		Prop prop = null;
		Prop[] props = World.GetNearbyProps(((Entity)Game.Player.Character).Position, 50f);
		Vector3 pos = WeedPlaceLocation();
		if (props.Count() > 0)
		{
			Prop[] array = props;
			foreach (Prop p in array)
			{
				Model model = ((Entity)p).Model;
				if (model.Hash == doorHash)
				{
					prop = p;
					((Entity)p).IsPositionFrozen = true;
					if (((Entity)p).Position != pos)
					{
						((Entity)p).Position = pos;
					}
					if (LSL.DEBUG)
					{
						GTA.UI.Notification.PostTicker("Porte trouvée", false, false);
					}
					break;
				}
			}
		}
		if ((Entity)(object)prop == (Entity)null)
		{
			prop = Prop.Create(new Model(doorHash), pos, new Vector3(0f, 0f, 0f), false, false);
			if (LSL.DEBUG)
			{
				GTA.UI.Notification.PostTicker("Aucune porte trouvée, création d'une porte", false, false);
			}
		}
		return prop;
	}
}
