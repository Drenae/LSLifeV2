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
	public static List<Drug> LsDrugs()
	{
		List<Drug> Drugs = new List<Drug>();
		if (LSL.hardMode)
		{
			Drugs.Add(new Drug("Weed", 10, LSL.AreaType.Normal));
			Drugs.Add(new Drug("Crack", 20, LSL.AreaType.Poor));
			Drugs.Add(new Drug("Cocaine", 100, LSL.AreaType.Rich));
		}
		else
		{
			Drugs.Add(new Drug("Weed", 50, LSL.AreaType.Normal));
			Drugs.Add(new Drug("Crack", 200, LSL.AreaType.Poor));
			Drugs.Add(new Drug("Cocaine", 400, LSL.AreaType.Rich));
		}
		return Drugs;
	}

	public static string DrugDisplayName(string name)
	{
		if (name == "Weed") return "cannabis";
		if (name == "Cocaine") return "cocaïne";
		if (name == "Crack") return "crack";
		return name;
	}

	public static decimal TotalDrugGrams(Dictionary<string, int> drugs)
	{
		decimal grams = 0;
		foreach (int quantity in drugs.Values)
		{
			grams += quantity;
		}
		return grams;
	}

	public static void UpdateInventory()
	{
		bool saveChanged = false;
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "playerInventory"))
			{
				continue;
			}
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "weed")
				{
					if (e.Value != LSL.PlayerInventory["Weed"].ToString())
					{
						e.Value = LSL.PlayerInventory["Weed"].ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "crack")
				{
					if (e.Value != LSL.PlayerInventory["Crack"].ToString())
					{
						e.Value = LSL.PlayerInventory["Crack"].ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "cocain")
				{
					if (e.Value != LSL.PlayerInventory["Cocaine"].ToString())
					{
						e.Value = LSL.PlayerInventory["Cocaine"].ToString();
						saveChanged = true;
					}
				}
				else if (e.Name == "weedOz")
				{
					saveChanged |= WriteStockGrams(e, playerBulkWeedGrams);
				}
				else if (e.Name == "crackOz")
				{
					saveChanged |= WriteStockGrams(e, playerBulkCrackGrams);
				}
				else if (e.Name == "cocaineOz")
				{
					saveChanged |= WriteStockGrams(e, playerBulkCocaineGrams);
				}
				else if (e.Name == "armour" && e.Value != LSL.playerArmours.ToString())
				{
					e.Value = LSL.playerArmours.ToString();
					saveChanged = true;
				}
			}
			break;
		}
		if (saveChanged)
		{
			XmlSaveFile.Save(LSL.LSLifeSave, "scripts\\LSLife\\LSLifeSave.xml");
		}
	}

	internal const decimal GramsPerOunce = 28.349523125m;
	private static readonly CultureInfo WeightCulture = CultureInfo.GetCultureInfo("fr-FR");

	public static string FormatWeight(decimal grams)
	{
		return Math.Abs(grams) >= 1000m
			? (grams / 1000m).ToString("0.###", WeightCulture) + " kg"
			: grams.ToString("0.##", WeightCulture) + " g";
	}

	public static string FormatOunceWeight(int ounces) => FormatWeight(OuncesToGrams(ounces));

	internal static decimal OuncesToGrams(int ounces) => ounces * GramsPerOunce;

	internal static int WholeGrams(decimal grams) => (int)Math.Min(int.MaxValue, Math.Max(0m, grams));

	// The attribute is authoritative. Keep whole historical units in the element
	// text so older builds can still read the whole packages.
	internal static decimal ReadStockGrams(XElement element)
	{
		XAttribute grams = element.Attribute("grams");
		if (grams != null)
			return (decimal)grams;

		return int.TryParse(element.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int ounces)
			? OuncesToGrams(ounces) : 0m;
	}

	internal static bool WriteStockGrams(XElement element, decimal grams)
	{
		string legacy = decimal.Truncate(grams / GramsPerOunce).ToString(CultureInfo.InvariantCulture);
		string metric = grams.ToString(CultureInfo.InvariantCulture);
		if (element.Value == legacy && (string)element.Attribute("grams") == metric)
			return false;

		element.Value = legacy;
		element.SetAttributeValue("grams", metric);
		return true;
	}
}
