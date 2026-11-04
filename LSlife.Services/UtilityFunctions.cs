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
	public static float StringToFloat(string _float)
	{
		if (float.TryParse(_float, NumberStyles.Float, new CultureInfo("de-DE"), out var final))
		{
			return final;
		}
		return float.Parse(_float, NumberStyles.Float, CultureInfo.InvariantCulture);
	}

	public static int RandomInt(int start, int end)
	{
		return new Random().Next(start, end);
	}

	public static float Lerp(float start, float end, float time)
	{
		return (1f - time) * start + time * end;
	}

	public static int RanRange(int _min, int _max)
	{
		int max = ((_max >= _min) ? _max : (_min + 1));
		return LSL.rnd.Next(_min, max);
	}

	public static void DateTimeUpdate()
	{
		if (Function.Call<int>((Hash)2675767815307398015L) != LSL.hour)
		{
			HoursPassed = HowManyHoursPassed();
			TimeChanged = true;
			LSL.hour = Function.Call<int>((Hash)2675767815307398015L);
			if ((LSL.dealerWeedGrams == 0 || LSL.dealerCrackGrams == 0 || LSL.dealerCocaineGrams == 0) && !LSL.dealer1ReloadOrder && !LSL.dealer1Reloaded)
			{
				LSL.dealer1ReloadOrder = true;
				int waitTime = 20;
				waitTime = ((waitTime - (int)CurrentRepLvl(LSL.dealer1Rep) / 2 > 5) ? (waitTime - (int)CurrentRepLvl(LSL.dealer1Rep) / 2) : 5);
				int timeTodeliver = 0;
				bool nextday = false;
				if (LSL.hour + waitTime > 24)
				{
					timeTodeliver = LSL.hour + waitTime - 24;
					nextday = true;
				}
				else
				{
					timeTodeliver = LSL.hour + waitTime;
					nextday = false;
				}
				LSL.dealer1ReloadHour = timeTodeliver;
				if (nextday)
				{
					if (LSL.day == 6)
					{
						LSL.dealer1ReloadDay = 0;
					}
					else
					{
						LSL.dealer1ReloadDay = LSL.day + 1;
					}
				}
				else
				{
					LSL.dealer1ReloadDay = LSL.day;
				}
				Enum.TryParse<Enums.DayGTA>(LSL.dealer1ReloadDay.ToString(), out var mDay);
				if (LSL.DEBUG)
				{
					UI.Notify(LSL.dealer1ReloadHour.ToString() ?? "");
				}
				string amPm = "AM";
				if (timeTodeliver > 12)
				{
					timeTodeliver -= 12;
					amPm = "PM";
				}
				TextMsg("ZEE", "Réassort", "Réassort commandé, livraison prévue " + mDay.ToString() + " à " + timeTodeliver + ":00" + amPm);
			}
			int timeReduction = 0;
			timeReduction = ((!(CurrentRepLvl(LSL.dealer1Rep) > 36.0)) ? ((int)CurrentRepLvl(LSL.dealer1Rep) * 10000) : 360000);
			if (!LSL.dealer1ReloadOrder && LSL.dealer1Reloaded && Game.GameTime > LSL.reloadTimer + (LSL.reloadTimerInterval - timeReduction))
			{
				LSL.dealer1Reloaded = false;
				foreach (XElement element in LSL.LSLifeSave.Descendants())
				{
					if (!(element.Name == "dealer1"))
					{
						continue;
					}
					foreach (XElement drugCarEle in element.Elements())
					{
						if (drugCarEle.Name == "reloaded")
						{
							drugCarEle.Value = LSL.dealer1Reloaded.ToString();
							XmlSaveFile.Save(LSL.LSLifeSave, "scripts\\LSLife\\LSLifeSave.xml");
							break;
						}
					}
				}
			}
			int dDay = 0;
			dDay = ((LSL.dealer1ReloadDay + 1 != 7) ? (LSL.dealer1ReloadDay + 1) : 0);
			if (LSL.day == LSL.dealer1ReloadDay && !LSL.dealer1Reloaded && LSL.dealer1ReloadOrder && LSL.hour >= LSL.dealer1ReloadHour)
			{
				LSL.dealer1ReloadOrder = false;
				LSL.dealer1Reloaded = true;
				LSL.reloadTimer = Game.GameTime;
				if (LSL.dealer1Debt > 0)
				{
					float intrest = 0.05f;
					float intrestAmount = (float)LSL.dealer1Debt * intrest;
					if (intrestAmount < 0f)
					{
						intrestAmount = 0f;
					}
					LSL.dealer1Debt += (int)intrestAmount;
					if (LSL.dealer1Debt < 0)
					{
						LSL.dealer1Debt = 0;
					}
					TextMsg("ZEE", "Dette", "J'ai ajouté " + IntToMoney((int)intrestAmount) + " $" + " à ta dette. Elle s'élève maintenant à " + IntToMoney(LSL.dealer1Debt) + " $");
				}
				if (LSL.hardMode)
				{
					if (LSL.dealer1Money < 1000)
					{
						LSL.dealer1Money = 5000;
					}
				}
				else if (LSL.dealer1Money < 10000)
				{
					LSL.dealer1Money = 50000;
				}
				float weedMoney = 0f;
				float crackMoney = 0f;
				float cocainMoney = 0f;
				if (LSL.dealerWeedGrams < OuncesToGrams(40))
				{
					weedMoney = (float)LSL.dealer1Money * 0.75f;
					LSL.dealer1Money -= (int)weedMoney;
				}
				if (LSL.dealerCrackGrams < OuncesToGrams(40))
				{
					crackMoney = (float)LSL.dealer1Money * 0.6f;
					LSL.dealer1Money -= (int)crackMoney;
				}
				if (LSL.dealerCocaineGrams < OuncesToGrams(40))
				{
					cocainMoney = LSL.dealer1Money;
					LSL.dealer1Money -= (int)cocainMoney;
				}
				int dWeed = LSL.weed1Price / 2 - Discount(LSL.weed1Price / 2);
				int dCrack = LSL.crack1Price / 2 - Discount(LSL.crack1Price / 2);
				int dCocaine = LSL.cocain1Price / 2 - Discount(LSL.cocain1Price / 2);
				int finalWeed = (int)weedMoney / dWeed;
				int finalCrack = (int)crackMoney / dCrack;
				int finalCocain = (int)cocainMoney / dCocaine;
				LSL.dealerWeedGrams += OuncesToGrams(finalWeed);
				LSL.dealerCrackGrams += OuncesToGrams(finalCrack);
				LSL.dealerCocaineGrams += OuncesToGrams(finalCocain);
				weedMoney -= (float)(finalWeed * dWeed);
				crackMoney -= (float)(finalCrack * dCrack);
				cocainMoney -= (float)(finalCocain * dCocaine);
				LSL.dealer1Money += (int)weedMoney + (int)crackMoney + (int)cocainMoney;
				TextMsg("ZEE", "Réapprovisionnement", "Cannabis : " + FormatOunceWeight(finalWeed) + ", crack : " + FormatOunceWeight(finalCrack) + ", cocaïne : " + FormatOunceWeight(finalCocain) + ".");
				LSL.LSLifeSave = XmlSaveFile.Load("scripts\\LSLife\\LSLifeSave.xml");
				UpdateSave();
			}
			if (LSL.day == dDay && !LSL.dealer1Reloaded && LSL.dealer1ReloadOrder)
			{
				LSL.dealer1ReloadOrder = false;
				LSL.dealer1Reloaded = true;
				LSL.reloadTimer = Game.GameTime;
				if (LSL.dealer1Debt > 0)
				{
					float intrest2 = 0.05f;
					float intrestAmount2 = (float)LSL.dealer1Debt * intrest2;
					LSL.dealer1Debt += (int)intrestAmount2;
					if (LSL.dealer1Debt < 0)
					{
						LSL.dealer1Debt = 0;
					}
					TextMsg("ZEE", "Dette", "J'ai ajouté " + IntToMoney((int)intrestAmount2) + " $" + " à ta dette. Elle s'élève maintenant à " + IntToMoney(LSL.dealer1Debt) + " $");
				}
				if (LSL.dealer1Money < 10000)
				{
					LSL.dealer1Money = 50000;
				}
				float weedMoney2 = 0f;
				float crackMoney2 = 0f;
				float cocainMoney2 = 0f;
				if (LSL.dealerWeedGrams < OuncesToGrams(40))
				{
					weedMoney2 = (float)LSL.dealer1Money * 0.75f;
					LSL.dealer1Money -= (int)weedMoney2;
				}
				if (LSL.dealerCrackGrams < OuncesToGrams(40))
				{
					crackMoney2 = (float)LSL.dealer1Money * 0.6f;
					LSL.dealer1Money -= (int)crackMoney2;
				}
				if (LSL.dealerCocaineGrams < OuncesToGrams(40))
				{
					cocainMoney2 = LSL.dealer1Money;
					LSL.dealer1Money -= (int)cocainMoney2;
				}
				int dWeed2 = LSL.weed1Price / 2 - Discount(LSL.weed1Price / 2);
				int dCrack2 = LSL.crack1Price / 2 - Discount(LSL.crack1Price / 2);
				int dCocaine2 = LSL.cocain1Price / 2 - Discount(LSL.cocain1Price / 2);
				int finalWeed2 = (int)weedMoney2 / dWeed2;
				int finalCrack2 = (int)crackMoney2 / dCrack2;
				int finalCocain2 = (int)cocainMoney2 / dCocaine2;
				LSL.dealerWeedGrams += OuncesToGrams(finalWeed2);
				LSL.dealerCrackGrams += OuncesToGrams(finalCrack2);
				LSL.dealerCocaineGrams += OuncesToGrams(finalCocain2);
				weedMoney2 -= (float)(finalWeed2 * dWeed2);
				crackMoney2 -= (float)(finalCrack2 * dCrack2);
				cocainMoney2 -= (float)(finalCocain2 * dCocaine2);
				LSL.dealer1Money += (int)weedMoney2 + (int)crackMoney2 + (int)cocainMoney2;
				TextMsg("ZEE", "Réapprovisionnement", "Cannabis : " + FormatOunceWeight(finalWeed2) + ", crack : " + FormatOunceWeight(finalCrack2) + ", cocaïne : " + FormatOunceWeight(finalCocain2) + ".");
				if (LSL.DEBUG)
				{
					UI.Notify("Cannabis : " + finalWeed2 + " crack : " + finalCrack2 + " cocaïne : " + finalCocain2);
				}
				LSL.LSLifeSave = XmlSaveFile.Load("scripts\\LSLife\\LSLifeSave.xml");
				UpdateSave();
			}
			if (LSL.areas.Count > 0)
			{
				DbugString("HoursPassed = " + HoursPassed);
				for (int i = 0; i < HoursPassed; i++)
				{
					foreach (Area area in LSL.areas)
					{
						if (area.Name == LSL.areas[LSL.areaIndex].Name)
						{
							if (!LSL.playerWanted)
							{
								area.HeatDecay();
							}
						}
						else
						{
							area.HeatDecay();
						}
					}
					DoGangGrowth();
				}
				if (LSL.hour == 12 || LSL.hour == 0 || LSL.hour == 6 || LSL.hour == 18)
				{
					SaveAreaData();
				}
			}
		}
		else
		{
			TimeChanged = false;
		}
		if (Function.Call<int>(unchecked((Hash)(-2777906517671992481L))) == LSL.day)
		{
			return;
		}
		LSL.day = Function.Call<int>(unchecked((Hash)(-2777906517671992481L)));
		LSL.LSLifeSave = XmlSaveFile.Load("scripts\\LSLife\\LSLifeSave.xml");
		if (LSL.dealer1Money > 1200000)
		{
			LSL.dealer1Money = 1200000;
		}
		if (LSL.dealerWeedGrams > 0)
		{
			decimal sold = LSL.dealerWeedGrams <= GramsPerOunce ? LSL.dealerWeedGrams : LSL.dealerWeedGrams / 2m;
			LSL.dealerWeedGrams -= sold;
			int basePrice = LSL.weed1Price / 2;
			LSL.dealer1Money += (int)decimal.Round(sold * (basePrice + Discount(basePrice)) / GramsPerOunce);
			foreach (XElement element2 in LSL.LSLifeSave.Descendants())
			{
				if (!(element2.Name == "dealer1"))
				{
					continue;
				}
				foreach (XElement drugCarEle2 in element2.Elements())
				{
					if (drugCarEle2.Name == "weed")
					{
						LsFunctions.WriteStockGrams(drugCarEle2, LSL.dealerWeedGrams);
					}
					if (drugCarEle2.Name == "money")
					{
						drugCarEle2.Value = LSL.dealer1Money.ToString();
					}
				}
			}
		}
		if (LSL.dealerCrackGrams > 0)
		{
			decimal sold = LSL.dealerCrackGrams <= GramsPerOunce ? LSL.dealerCrackGrams : LSL.dealerCrackGrams / 2m;
			LSL.dealerCrackGrams -= sold;
			int basePrice = LSL.crack1Price / 2;
			LSL.dealer1Money += (int)decimal.Round(sold * (basePrice + Discount(basePrice)) / GramsPerOunce);
			foreach (XElement element3 in LSL.LSLifeSave.Descendants())
			{
				if (!(element3.Name == "dealer1"))
				{
					continue;
				}
				foreach (XElement drugCarEle3 in element3.Elements())
				{
					if (drugCarEle3.Name == "crack")
					{
						LsFunctions.WriteStockGrams(drugCarEle3, LSL.dealerCrackGrams);
					}
					if (drugCarEle3.Name == "money")
					{
						drugCarEle3.Value = LSL.dealer1Money.ToString();
					}
				}
			}
		}
		if (LSL.dealerCocaineGrams > 0)
		{
			decimal sold = LSL.dealerCocaineGrams <= GramsPerOunce ? LSL.dealerCocaineGrams : LSL.dealerCocaineGrams / 2m;
			LSL.dealerCocaineGrams -= sold;
			int basePrice = LSL.cocain1Price / 2;
			LSL.dealer1Money += (int)decimal.Round(sold * (basePrice + Discount(basePrice)) / GramsPerOunce);
			foreach (XElement element4 in LSL.LSLifeSave.Descendants())
			{
				if (!(element4.Name == "dealer1"))
				{
					continue;
				}
				foreach (XElement drugCarEle4 in element4.Elements())
				{
					if (drugCarEle4.Name == "cocain")
					{
						LsFunctions.WriteStockGrams(drugCarEle4, LSL.dealerCocaineGrams);
					}
					if (drugCarEle4.Name == "money")
					{
						drugCarEle4.Value = LSL.dealer1Money.ToString();
					}
				}
			}
		}
		XmlSaveFile.Save(LSL.LSLifeSave, "scripts\\LSLife\\LSLifeSave.xml");
		UpdateSave();
	}

	public static int HowManyHoursPassed()
	{
		int currentHour = Function.Call<int>((Hash)2675767815307398015L);
		int hoursPassed = 0;
		if (lastHour != currentHour)
		{
			hoursPassed = ((currentHour <= lastHour) ? (24 - lastHour + currentHour) : (currentHour - lastHour));
			lastHour = currentHour;
			if (LSL.DEBUG)
			{
				UI.Notify("Heures écoulées : " + hoursPassed);
			}
		}
		return hoursPassed;
	}

	public static Model RequestModel(PedHash _hash)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		int time = Game.GameTime;
		Model Model = new Model(_hash);
		Model.Request(1000);
		if (Model.IsInCdImage && Model.IsValid)
		{
			while (!Model.IsLoaded)
			{
				Script.Yield();
			}
			DbugString("modelTime = " + (time - Game.GameTime));
			return Model;
		}
		DbugString("modelInvalid");
		Model.MarkAsNoLongerNeeded();
		return Model;
	}

	public static Model RequestModel(VehicleHash _hash)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Model Model = new Model(_hash);
		Model.Request(1000);
		if (Model.IsInCdImage && Model.IsValid)
		{
			while (!Model.IsLoaded)
			{
				Script.Yield();
			}
			return Model;
		}
		Model.MarkAsNoLongerNeeded();
		return Model;
	}
}
