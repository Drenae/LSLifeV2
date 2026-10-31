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

// Méthodes déplacées sans modification de leur logique.
public partial class LSL
{
	private void areaInfoUI()
	{
		var area = areas[areaIndex];
		HudRender.DebugPanel("QUARTIER", Hud.DebugX, Hud.DebugY, 340, new[]
		{
			"nom : " + area.Name,
			"type : " + area.AreaType,
			"cStr : " + area.CopPresance,
			"gStr : " + area.GangPresance,
			"pression : " + area.Heat,
			"nDealer : " + area.Dealers(),
			"dMet : " + area.DemandMet(),
			"aRep : " + area.Reputation + " Lvl : " + (int)LsFunctions.CurrentRepLvl(area.Reputation),
			"--" + LsFunctions.DrugDisplayName(area.Drugs[0].Name) + "--",
			"Dem : " + area.Drugs[0].Demand,
			"Sup : " + area.Drugs[0].Supplied,
			"--" + LsFunctions.DrugDisplayName(area.Drugs[1].Name) + "--",
			"Dem : " + area.Drugs[1].Demand,
			"Sup : " + area.Drugs[1].Supplied,
			"--" + LsFunctions.DrugDisplayName(area.Drugs[2].Name) + "--",
			"Dem : " + area.Drugs[2].Demand,
			"Sup : " + area.Drugs[2].Supplied
		});
	}

	private void ZeeInfoUI()
	{
		HudRender.DebugPanel("ZEE", Hud.DebugX, Hud.DebugY + 344, 340, new[]
		{
			"argent : " + dealer1Money,
			"reload : " + dealer1ReloadOrder,
			"reloaded : " + dealer1Reloaded,
			"cannabis : " + dealer1Weed,
			"crack : " + dealer1Crack,
			"cocaine : " + dealer1Cocain
		});
	}

	private void PlayerUI()
	{
		var lines = new List<string>
		{
			"pression : " + playerHeat,
			"vente : " + isDealing,
			"tPeds : " + lPeds.GetPeds.Count,
			"oPeds : " + OldCustomerHandler.AmountOf()
		};
		if (wepDealer != null)
		{
			lines.Add("dSpeed : " + wepDealer.maxSpeed + "|" +
				(wepDealer.ped.CurrentVehicle != null ? wepDealer.ped.CurrentVehicle.Speed : 0f));
			lines.Add("cSpeed : " + wepDealer.canSpeed);
			lines.Add("sSpeed : " + wepDealer.shouldSpeed);
		}
		int drvAmount = driveByHandler.GetDriveBies.Count;
		if (drvAmount > 0) lines.Add("drvAmount : " + drvAmount);
		HudRender.DebugPanel("JOUEUR", Hud.DebugX, Hud.DebugY + 492, 340, lines.ToArray());
	}

	private void GangUI()
	{
		Gang gang = areas[areaIndex].Gang;
		HudRender.DebugPanel("GANG", Hud.DebugX, Hud.DebugY + 688, 340, new[]
		{
			"nom : " + gang.Name,
			"argent : " + gang.Money,
			"max : " + gang.MaxMembers,
			"total : " + gang.TotalMembers,
			"threats : " + gang.ThreatHandler.ThreatCount,
			"free : " + GangUtilities.AllocatedMemberCount(gang),
			"nuit : " + IsNight(),
			"shots : " + ShotHandler.shots.Count
		});
	}
}

internal static class Hud
{
	private static int posX = TopHudDefaultX;
	private static int posY = TopHudDefaultY;
	private static int debugX = 10;
	private static int debugY = 10;
	public static bool DebugEnabled { get; set; }
	public static int BarsPositionX => barsX;
	public static int BarsPositionY => barsY;
	public static int DebugX => debugX;
	public static int DebugY => debugY;
	public static void MoveBars(int dx, int dy)
	{
		barsX = System.Math.Max(0, System.Math.Min(MaxX - BarsWidth, barsX + dx));
		barsY = System.Math.Max(0, System.Math.Min(MaxY - BarsRowHeight * 2, barsY + dy));
	}
	public static void MoveDebug(int dx, int dy)
	{
		debugX = System.Math.Max(0, System.Math.Min(MaxX - 340, debugX + dx));
		debugY = System.Math.Max(0, System.Math.Min(MaxY - 864, debugY + dy));
	}

	private const int MaxX = 1920;
	private const int MaxY = 1080;

	public static int PositionX => posX;
	public static int PositionY => posY;

	public static void Innit()
	{
		LoadInvConfig();
		posX = System.Math.Max(0, System.Math.Min(MaxX, posX));
		posY = System.Math.Max(25, System.Math.Min(MaxY - 164, posY));
		barsX = System.Math.Max(0, System.Math.Min(MaxX - BarsWidth, barsX));
		barsY = System.Math.Max(0, System.Math.Min(MaxY - BarsRowHeight * 2, barsY));
		debugX = System.Math.Max(0, System.Math.Min(MaxX - 340, debugX));
		debugY = System.Math.Max(0, System.Math.Min(MaxY - 864, debugY));
	}

	public static void Move(int dx, int dy)
	{
		posX = System.Math.Max(0, System.Math.Min(MaxX, posX + dx));
		posY = System.Math.Max(25, System.Math.Min(MaxY - 164, posY + dy));
	}

	private static void LoadInvConfig()
	{
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "config"))
			{
				continue;
			}
			{
				foreach (XElement e in element.Elements())
				{
					if (e.Name == "invPosX")
					{
						int.TryParse(e.Value, out posX);
					}
					else if (e.Name == "invPosY")
					{
						int.TryParse(e.Value, out posY);
					}
					else if (e.Name == "heatPosX") int.TryParse(e.Value, out barsX);
					else if (e.Name == "heatPosY") int.TryParse(e.Value, out barsY);
					else if (e.Name == "debugPosX") int.TryParse(e.Value, out debugX);
					else if (e.Name == "debugPosY") int.TryParse(e.Value, out debugY);
				}
				// Migrer une seule fois les anciennes coordonnées (bord droit).
				if (element.Element("hudPosVersion") == null)
				{
					posX = System.Math.Max(0, posX - 365);
				}
				break;
			}
		}
	}

	public static void SavePosition()
	{
		XElement config = LSL.LSLifeSave.Descendants("config").FirstOrDefault();
		if (config == null)
		{
			config = new XElement("config");
			LSL.LSLifeSave.Root.Add(config);
		}
		SetConfig(config, "invPosX", posX);
		SetConfig(config, "invPosY", posY);
		SetConfig(config, "heatPosX", barsX);
		SetConfig(config, "heatPosY", barsY);
		SetConfig(config, "debugPosX", debugX);
		SetConfig(config, "debugPosY", debugY);
		SetConfig(config, "hudPosVersion", 2);
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
	}

	private static void SetConfig(XElement config, string key, int value)
	{
		XElement item = config.Element(key);
		if (item == null)
		{
			item = new XElement(key);
			config.Add(item);
		}
		item.Value = value.ToString();
	}

	private static void DrawHudText(string caption, int x, int y, float scale, Color color)
	{
		HudRender.Text(caption, x, y, scale, color.R, color.G, color.B, color.A, centered: true);
	}

	// Position par défaut du HUD supérieur (coin supérieur droit).
	// Les curseurs du menu Options peuvent aussi modifier ces coordonnées et les sauvegarder.
	private const int TopHudDefaultX = 1580;
	private const int TopHudDefaultY = 100;

	// Apparence harmonisee avec les menus personnalises : anthracite, or discret.
	// Les deux panneaux conservent leurs emplacements et leurs dimensions.
	private static int barsX = 1580;
	private static int barsY = 940;
	private const int BarsWidth = 365;
	private const int BarsRowHeight = 43;
	private const int BarsProgressHeight = 12;
	private const int BarsProgressWidth = 230;
	private const int BarsProgressOffsetX = 120;
	private static readonly Color HudBackground = Color.FromArgb(190, 11, 13, 16);
	private static readonly Color HudSeparator = Color.FromArgb(155, 130, 103, 52);
	private static readonly Color BarsTrack = Color.FromArgb(225, 53, 56, 60);
	private static readonly Color AccentGold = Color.FromArgb(225, 187, 154, 82);

	// Options visuelles : valeurs prudentes pour cette premiere version.
	// Les stocks restent affiches par defaut, comme dans l'ancien HUD.
	private const bool CompactWhenNotSelling = false;
	private static bool ShowReputation = false;
	private static bool ShowDealerCapacity = false;

	public static void Draw()
	{
		InventoryGui();
		DrawHeatBars();
	}

	private static Color PressureColor(float value)
	{
		if (value >= 0.70f) return Color.FromArgb(235, 213, 83, 78);
		if (value >= 0.35f) return Color.FromArgb(235, 225, 160, 70);
		return Color.FromArgb(235, 123, 187, 117);
	}

	private static void DrawHeatBars()
	{
		// Trois mesures distinctes : pression joueur, pression quartier et reputation quartier.
		const int rows = 3;
		HudRender.Rect(barsX, barsY - BarsRowHeight, BarsWidth, BarsRowHeight * rows, 11, 13, 16, 190);
		HudRender.Rect(barsX, barsY - BarsRowHeight, 3, BarsRowHeight * rows, 187, 154, 82, 225);
		HudRender.Rect(barsX + 12, barsY, BarsWidth - 24, 1, 130, 103, 52, 155);
		HudRender.Rect(barsX + 12, barsY + BarsRowHeight, BarsWidth - 24, 1, 130, 103, 52, 155);
		DrawBarRow("Joueur", PlayerHeatBar.GetPercentage(), barsY - BarsRowHeight);
		DrawBarRow("Quartier", AreaHeatBar.GetPercentage(), barsY);
		DrawBarRow("Réputation", AreaHeatBar.GetReputationPercentage(), barsY + BarsRowHeight);
	}

	private static void DrawBarRow(string label, float percentage, int y)
	{
		float progress = System.Math.Max(0f, System.Math.Min(1f, percentage));
		int progressX = barsX + BarsProgressOffsetX;
		int progressY = y + (BarsRowHeight - BarsProgressHeight) / 2;
		DrawHudText(label, barsX + 58, y + 10, 0.30f, Color.WhiteSmoke);
		HudRender.Progress(progressX, progressY, BarsProgressWidth, BarsProgressHeight, progress);

	}

	public static void Hide()
	{
		// Aucun élément persistant : les rectangles cessent d'être dessinés.
	}

	public static void InventoryGui()
	{
		// Coordonnees et sauvegarde existantes conservees (1920 x 1080).
		const int width = 365;
		bool compact = CompactWhenNotSelling && !LSL.isDealing;
		int extraRows = (ShowReputation ? 1 : 0) + (ShowDealerCapacity ? 1 : 0);
		int height = (compact ? 91 : 164) + extraRows * 21;
		int left = posX;
		int top = System.Math.Max(0, posY - 25);
		HudRender.Rect(left, top, width, height, 11, 13, 16, 190);
		HudRender.Rect(left, top, 3, height, 187, 154, 82, 225);
		HudRender.Rect(left + 12, top + 36, width - 24, 1, 130, 103, 52, 155);
		DrawHudText("ARGENT", left + 60, top + 8, 0.30f, Color.White);
		DrawHudText(LsFunctions.IntToMoney(LSL.player.Money) + " $",
			left + 290, top + 8, 0.30f, Color.LightGreen);

		if (!compact)
		{
			string[] names = { "Cannabis", "Crack", "Cocaïne" };
			string[] ready =
			{
				LsFunctions.GramsToOz(LSL.PlayerInventory["Weed"]) ?? "",
				LsFunctions.GramsToOz(LSL.PlayerInventory["Crack"]) ?? "",
				LsFunctions.GramsToOz(LSL.PlayerInventory["Cocaine"]) ?? ""
			};
			string[] raw =
			{
				LsFunctions.pWeedOunce + " oz",
				LsFunctions.pCrackOunce + " oz",
				LsFunctions.pCocaineOunce + " oz"
			};
			for (int i = 0; i < 3; i++)
			{
				int x = left + 75 + i * 109;
				DrawHudText(names[i], x, top + 45, 0.30f, Color.WhiteSmoke);
				DrawHudText(ready[i], x, top + 70, 0.30f, Color.LightGreen);
				DrawHudText(raw[i], x, top + 100, 0.30f, Color.LightCoral);
			}
		}
		int footerY = compact ? top + 47 : top + 125;
		if (ShowReputation)
		{
			DrawHudText("REPUTATION", left + 80, footerY + 1, 0.27f, Color.WhiteSmoke);
			DrawHudText("Niveau " + (int)LsFunctions.CurrentRepLvl(LSL.dealer1Rep),
				left + 290, footerY + 1, 0.27f, Color.WhiteSmoke);
			footerY += 21;
		}
		if (ShowDealerCapacity)
		{
			DrawHudText("DEALERS", left + 80, footerY + 1, 0.27f, Color.WhiteSmoke);
			DrawHudText(LSL.DealerHandler.dealers.Count + " / " +
				(int)LsFunctions.CurrentRepLvl(LSL.dealer1Rep),
				left + 290, footerY + 1, 0.27f, Color.WhiteSmoke);
			footerY += 21;
		}
		HudRender.Rect(left + 12, footerY, width - 24, 1, 130, 103, 52, 155);
		DrawHudText("MODE VENTE", left + 80, footerY + 5, 0.30f, Color.WhiteSmoke);
		DrawHudText(LSL.isDealing ? "ACTIVÉ" : "DÉSACTIVÉ",
			left + 290, footerY + 5, 0.30f,
			LSL.isDealing ? Color.LightGreen : Color.LightCoral);
		GTA.Native.Function.Call(GTA.Native.Hash.HIDE_HUD_COMPONENT_THIS_FRAME, 3);
	}

}
