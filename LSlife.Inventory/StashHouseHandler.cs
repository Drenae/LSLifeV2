using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;

namespace LSlife;

public class StashHouseHandler
{
	public StashHouse CurrentHouse;

	public List<StashHouse> Houses { get; private set; } = new List<StashHouse>();

	public StashHouseHandler()
	{
		LoadHouses();
		if (LSL.DEBUG)
		{
			UI.Notify("Planques chargées : " + Houses.Count);
		}
	}

	public void Ontick()
	{
		foreach (StashHouse item in Houses.Where((StashHouse r) =>
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = r.entrancePos;
			if (!(val.DistanceTo2D(LSL.playerPos) < 30f))
			{
				val = r.exitPos;
				return val.DistanceTo(LSL.playerPos) < 30f;
			}
			return true;
		}))
		{
			item.OnTick();
		}
	}

	private void LoadHouses()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		if (!File.Exists("scripts\\LSLife\\LSLife_StashHouses.xml"))
		{
			UI.Notify("~r~Fichier StashHouses.xml manquant");
			return;
		}
		XDocument StashDoc = XmlSaveFile.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
		foreach (XElement e in StashDoc.Descendants())
		{
			if (!(e.Name == "house"))
			{
				continue;
			}
			int id = 0;
			int.TryParse(e.FirstAttribute.Value, out id);
			Vector3 entrance = Vector3.Zero;
			Vector3 exit = Vector3.Zero;
			Vector3 stash = Vector3.Zero;
			Vector3 health = Vector3.Zero;
			int price = 0;
			bool purchased = false;
			decimal weed = 0;
			decimal crack = 0;
			decimal cocain = 0;
			int armour = 0;
			int money = 0;
			bool inside = false;
			bool found = false;
			foreach (XElement lEle in LSL.LSLifeSave.Descendants())
			{
				if (lEle.Name == "playerStash")
				{
					found = true;
					lEle.Remove();
					XmlSaveFile.Save(LSL.LSLifeSave, "scripts\\LSLife\\LSLifeSave.xml");
					break;
				}
			}
			List<XElement> sWeapons = new List<XElement>();
			List<object> sWeps = new List<object>();
			foreach (XElement el in e.Elements())
			{
				if (el.Name == "entrancePos")
				{
					foreach (XElement ele in el.Elements())
					{
						if (ele.Name == "X")
						{
							float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out entrance.X);
						}
						if (ele.Name == "Y")
						{
							float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out entrance.Y);
						}
						if (ele.Name == "Z")
						{
							float.TryParse(ele.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out entrance.Z);
						}
					}
				}
				if (el.Name == "exitPos")
				{
					foreach (XElement ele2 in el.Elements())
					{
						if (ele2.Name == "X")
						{
							float.TryParse(ele2.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exit.X);
						}
						if (ele2.Name == "Y")
						{
							float.TryParse(ele2.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exit.Y);
						}
						if (ele2.Name == "Z")
						{
							float.TryParse(ele2.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out exit.Z);
						}
					}
				}
				if (el.Name == "stashPos")
				{
					foreach (XElement ele3 in el.Elements())
					{
						if (ele3.Name == "X")
						{
							float.TryParse(ele3.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out stash.X);
						}
						if (ele3.Name == "Y")
						{
							float.TryParse(ele3.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out stash.Y);
						}
						if (ele3.Name == "Z")
						{
							float.TryParse(ele3.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out stash.Z);
						}
					}
				}
				if (el.Name == "healthPos")
				{
					foreach (XElement ele4 in el.Elements())
					{
						if (ele4.Name == "X")
						{
							float.TryParse(ele4.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out health.X);
						}
						if (ele4.Name == "Y")
						{
							float.TryParse(ele4.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out health.Y);
						}
						if (ele4.Name == "Z")
						{
							float.TryParse(ele4.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out health.Z);
						}
					}
				}
				if (el.Name == "price")
				{
					int.TryParse(el.Value, out price);
				}
				if (el.Name == "purchased")
				{
					bool.TryParse(el.Value, out purchased);
				}
				if (el.Name == "inside")
				{
					bool.TryParse(el.Value, out inside);
				}
				if (!(el.Name == "stash"))
				{
					continue;
				}
				foreach (XElement ele5 in el.Elements())
				{
					if (ele5.Name == "weed")
					{
						if (found)
						{
							WriteStashWeight(ele5, weed);
						}
						else
						{
							weed = ReadStashWeight(ele5);
							if (LSL.DEBUG)
							{
								UI.Notify("~y~Weed : " + weed);
							}
						}
					}
					if (ele5.Name == "crack")
					{
						if (found)
						{
							WriteStashWeight(ele5, crack);
						}
						else
						{
							crack = ReadStashWeight(ele5);
						}
					}
					if (ele5.Name == "cocain")
					{
						if (found)
						{
							WriteStashWeight(ele5, cocain);
						}
						else
						{
							cocain = ReadStashWeight(ele5);
						}
					}
					if (ele5.Name == "money")
					{
						if (found)
						{
							ele5.Value = money.ToString();
						}
						else
						{
							int.TryParse(ele5.Value, out money);
						}
					}
					if (ele5.Name == "armour")
					{
						if (found)
						{
							ele5.Value = armour.ToString();
						}
						else
						{
							int.TryParse(ele5.Value, out armour);
						}
					}
					if (!(ele5.Name == "weapons"))
					{
						continue;
					}
					foreach (XElement wepEl in ele5.Elements())
					{
						if (!(wepEl.Name == "weapon"))
						{
							continue;
						}
						foreach (XElement element in wepEl.Elements())
						{
							if (element.Name == "HASH")
							{
								sWeapons.Add(wepEl);
								sWeps.Add(element.Value);
								break;
							}
						}
					}
				}
			}
			if (found)
			{
				XmlSaveFile.Save(StashDoc, "scripts\\LSLife\\LSLife_StashHouses.xml");
			}
			Houses.Add(new StashHouse(id, entrance, exit, stash, health, purchased, price, inside, weed, crack, cocain, money, armour, sWeapons, sWeps));
		}
	}

	private void CreateStashHouseXML()
	{
	}

	public void SaveHouse(StashHouse _house)
	{
		XDocument StashDoc = XmlSaveFile.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
		foreach (XElement e in StashDoc.Descendants())
		{
			if (!(e.Name == "house") || !(e.FirstAttribute.Value == _house.id.ToString()))
			{
				continue;
			}
			foreach (XElement el in e.Elements())
			{
				if (!(el.Name == "stash"))
				{
					continue;
				}
				foreach (XElement ele in el.Elements())
				{
					if (ele.Name == "weed")
					{
						WriteStashWeight(ele, _house.Weed);
					}
					if (ele.Name == "crack")
					{
						WriteStashWeight(ele, _house.Crack);
					}
					if (ele.Name == "cocain")
					{
						WriteStashWeight(ele, _house.Cocain);
					}
					if (ele.Name == "money")
					{
						ele.Value = _house.Money.ToString();
					}
					if (ele.Name == "armour")
					{
						ele.Value = _house.Armor.ToString();
					}
					if (ele.Name == "weapons")
					{
						ele.Elements("weapon").Remove();
						ele.Add(_house.Weapons.Select(weapon => new XElement(weapon)));
					}
				}
			}
			XmlSaveFile.Save(StashDoc, "scripts\\LSLife\\LSLife_StashHouses.xml");
		}
	}

	// Keep the legacy whole-gram value readable by earlier versions.
	private const string FractionalGramsAttribute = "fractionalGrams";

	private static decimal ReadStashWeight(XElement element)
	{
		decimal.TryParse(element.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out decimal whole);
		decimal.TryParse((string)element.Attribute(FractionalGramsAttribute), NumberStyles.AllowDecimalPoint,
			CultureInfo.InvariantCulture, out decimal fraction);
		return fraction > 0m && fraction < 1m ? whole + fraction : whole;
	}

	private static void WriteStashWeight(XElement element, decimal grams)
	{
		decimal whole = Math.Floor(grams);
		decimal fraction = grams - whole;
		element.Value = whole.ToString(CultureInfo.InvariantCulture);
		element.SetAttributeValue(FractionalGramsAttribute,
			fraction == 0m ? null : fraction.ToString(CultureInfo.InvariantCulture));
	}

	public int TotalStashDrugs()
	{
		long drugs = 0;
		foreach (StashHouse house in Houses)
		{
			drugs += (long)LsFunctions.WholeGrams(house.Weed) +
				LsFunctions.WholeGrams(house.Crack) + LsFunctions.WholeGrams(house.Cocain);
		}
		return (int)Math.Min(int.MaxValue, drugs);
	}


	internal bool Inside()
	{
		if (CurrentHouse == null)
		{
			return false;
		}
		return CurrentHouse.inside;
	}
}
