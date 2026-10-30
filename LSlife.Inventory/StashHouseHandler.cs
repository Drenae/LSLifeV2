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
			GTA.UI.Notification.PostTicker("Planques chargées : " + Houses.Count, false, false);
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
			GTA.UI.Notification.PostTicker("~r~Fichier StashHouses.xml manquant", false, false);
			return;
		}
		XDocument StashDoc = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
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
			int weed = 0;
			int crack = 0;
			int cocain = 0;
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
					LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
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
							ele5.Value = weed.ToString();
						}
						else
						{
							int.TryParse(ele5.Value, out weed);
							if (LSL.DEBUG)
							{
								GTA.UI.Notification.PostTicker("~y~Weed : " + weed, false, false);
							}
						}
					}
					if (ele5.Name == "crack")
					{
						if (found)
						{
							ele5.Value = crack.ToString();
						}
						else
						{
							int.TryParse(ele5.Value, out crack);
						}
					}
					if (ele5.Name == "cocain")
					{
						if (found)
						{
							ele5.Value = cocain.ToString();
						}
						else
						{
							int.TryParse(ele5.Value, out cocain);
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
				StashDoc.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
			}
			Houses.Add(new StashHouse(id, entrance, exit, stash, health, purchased, price, inside, weed, crack, cocain, money, armour, sWeapons, sWeps));
		}
	}

	private void CreateStashHouseXML()
	{
	}

	public void SaveHouse(StashHouse _house)
	{
		XDocument StashDoc = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
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
						ele.Value = _house.Weed.ToString();
					}
					if (ele.Name == "crack")
					{
						ele.Value = _house.Crack.ToString();
					}
					if (ele.Name == "cocain")
					{
						ele.Value = _house.Cocain.ToString();
					}
					if (ele.Name == "money")
					{
						ele.Value = _house.Money.ToString();
					}
					if (ele.Name == "armour")
					{
						ele.Value = _house.Armor.ToString();
					}
					XElement wEle = new XElement("weapons");
					foreach (XElement wep in _house.Weapons)
					{
						wEle.Add(wep);
					}
					if (ele.Name == "weapons")
					{
						ele.Remove();
						el.Add(wEle);
						break;
					}
				}
			}
			StashDoc.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
		}
	}

	public int TotalStashDrugs()
	{
		int drugs = 0;
		foreach (StashHouse house in Houses)
		{
			drugs += house.Weed + house.Crack + house.Cocain;
		}
		return drugs;
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
