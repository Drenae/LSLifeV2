using System.Drawing;
using System.Xml.Linq;
using GTA;
using NativeUI;

namespace LSlife;

internal static class InventoryText
{
	public static UIMenu Options;

	public static UIMenuSliderItem InventoryPosX;

	public static UIMenuSliderItem InventoryPosY;

	private static UIMenuItem Save;

	private static int posX = 1900;

	private static int posY = 100;

	private const int MaxX = 1920;

	private const int MaxY = 1080;

	public static void Innit()
	{
		PrepareHudSprite();
		LoadInvConfig();
		Options = LSL.LsMenuPool.AddSubMenu(LSL.zeeMenu, "Options");
		InventoryPosX = new UIMenuSliderItem("PosX: ~g~" + posX);
		InventoryPosX.Maximum = 1920;
		InventoryPosX.Value = posX;
		InventoryPosY = new UIMenuSliderItem("PosY: ~g~" + posY);
		InventoryPosY.Maximum = 1080;
		InventoryPosY.Value = posY;
		Save = new UIMenuItem("Enregistrer les réglages");
		Options.AddItem(InventoryPosX);
		Options.AddItem(InventoryPosY);
		Options.AddItem(Save);
		InventoryPosX.OnSliderChanged += InventoryPos_OnSliderChanged;
		InventoryPosY.OnSliderChanged += InventoryPos_OnSliderChanged;
		Save.Activated += Save_Activated;
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
				}
				break;
			}
		}
	}

	private static void Save_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		bool found = false;
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (!(element.Name == "config"))
			{
				continue;
			}
			found = true;
			foreach (XElement e in element.Elements())
			{
				if (e.Name == "invPosX")
				{
					e.Value = posX.ToString();
				}
				else if (e.Name == "invPosY")
				{
					e.Value = posY.ToString();
				}
			}
			break;
		}
		if (!found)
		{
			XElement config = new XElement("config");
			XElement invPosX = new XElement("invPosX");
			invPosX.Value = posX.ToString();
			XElement invPosY = new XElement("invPosY");
			invPosY.Value = posY.ToString();
			config.Add(invPosX);
			config.Add(invPosY);
			LSL.LSLifeSave.Root.Add(config);
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		LSL.LsMenuPool.CloseAllMenus();
	}

	private static void InventoryPos_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		if (sender == InventoryPosX)
		{
			posX = sender.Value;
			sender.Text = "PosX: ~g~" + posX;
		}
		if (sender == InventoryPosY)
		{
			posY = sender.Value;
			sender.Text = "PosY: ~g~" + posY;
		}
	}

	private static void DrawHudText(string caption, int x, int y, float scale, Color color)
	{
		UIResText text = new UIResText(caption, new Point(x, y), scale, color,
			(GTA.Font)0, UIResText.Alignment.Centered);
		text.DropShadow = true;
		((UIText)text).Draw();
	}

	public static void InventoryGui()
	{
		// Coordonnées en espace NativeUI 1920 x 1080, comme les réglages existants.
		const int width = 365;
		const int height = 164;
		int left = posX - width;
		int top = System.Math.Max(0, posY - 25);
		// Fond et textes utilisent tous deux les primitives natives GTA/NativeUI.
		new UIResRectangle(new Point(left, top), new Size(width, height),
			Color.FromArgb(165, 0, 0, 0)).Draw();

		DrawHudText("ARGENT", left + 60, top + 8, 0.30f, Color.White);
		DrawHudText(LsFunctions.IntToMoney(LSL.player.Money) + " $", left + 290, top + 8, 0.30f, Color.LightGreen);
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
		DrawHudText("MODE VENTE", left + 80, top + 130, 0.30f, Color.WhiteSmoke);
		DrawHudText(LSL.isDealing ? "ACTIVÉ" : "DÉSACTIVÉ", left + 290, top + 130, 0.30f,
			LSL.isDealing ? Color.LightGreen : Color.LightCoral);

		// Masquer uniquement le compteur natif pendant l'affichage du panneau.
		// Il reste disponible normalement lorsque le panneau du mod n'est pas affiché.
		UI.HideHudComponentThisFrame((HudComponent)3);
	}
}
