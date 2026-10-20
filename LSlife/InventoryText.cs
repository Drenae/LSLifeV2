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
		LoadInvConfig();
		Options = LSL.LsMenuPool.AddSubMenu(LSL.zeeMenu, "Options");
		InventoryPosX = new UIMenuSliderItem("PosX: ~g~" + posX);
		InventoryPosX.Maximum = 1920;
		InventoryPosX.Value = posX;
		InventoryPosY = new UIMenuSliderItem("PosY: ~g~" + posY);
		InventoryPosY.Maximum = 1080;
		InventoryPosY.Value = posY;
		Save = new UIMenuItem("Save config");
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

	public static void InventoryGui()
	{
		Point UIpos = new Point(posX, posY);
		UIResText UIWeedtxt = new UIResText("-Weed-", UIpos, 0.4f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		UIWeedtxt.DropShadow = true;
		((UIText)UIWeedtxt).Shadow = true;
		UIWeedtxt.Outline = true;
		UIResText UIWeed = new UIResText("", new Point(UIpos.X, UIpos.Y + 20), 0.5f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		((UIText)UIWeed).Enabled = true;
		UIWeed.DropShadow = true;
		((UIText)UIWeed).Shadow = true;
		UIWeed.Outline = true;
		UIResText UIweed = new UIResText("", new Point(UIpos.X, UIpos.Y + 45), 0.5f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		((UIText)UIweed).Enabled = true;
		UIweed.DropShadow = true;
		((UIText)UIweed).Shadow = true;
		UIweed.Outline = true;
		UIResText UICracktxt = new UIResText("-Crack-", new Point(UIpos.X, UIpos.Y + 80), 0.4f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		UICracktxt.DropShadow = true;
		((UIText)UICracktxt).Shadow = true;
		UICracktxt.Outline = true;
		UIResText UICrack = new UIResText("", new Point(UIpos.X, UIpos.Y + 80 + 20), 0.5f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		((UIText)UICrack).Enabled = true;
		UICrack.DropShadow = true;
		((UIText)UICrack).Shadow = true;
		UICrack.Outline = true;
		UIResText UIcrack = new UIResText("", new Point(UIpos.X, UIpos.Y + 100 + 25), 0.5f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		((UIText)UIcrack).Enabled = true;
		UIcrack.DropShadow = true;
		((UIText)UIcrack).Shadow = true;
		UIcrack.Outline = true;
		UIResText UICocainetxt = new UIResText("-Coke-", new Point(UIpos.X, UIpos.Y + 160), 0.4f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		UICocainetxt.DropShadow = true;
		((UIText)UICocainetxt).Shadow = true;
		UICocainetxt.Outline = true;
		UIResText UICocaine = new UIResText("", new Point(UIpos.X, UIpos.Y + 160 + 20), 0.5f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		((UIText)UICocaine).Enabled = true;
		UICocaine.DropShadow = true;
		((UIText)UICocaine).Shadow = true;
		UICocaine.Outline = true;
		UIResText uIResText = new UIResText("", new Point(UIpos.X, UIpos.Y + 160 + 20 + 25), 0.5f, Color.WhiteSmoke, (Font)7, UIResText.Alignment.Right);
		((UIText)uIResText).Enabled = true;
		uIResText.DropShadow = true;
		((UIText)uIResText).Shadow = true;
		uIResText.Outline = true;
		((UIText)UIWeed).Caption = "~r~" + LsFunctions.pWeedOunce + "~s~z";
		((UIText)UIweed).Caption = LsFunctions.GramsToOz(LSL.PlayerInventory["Weed"]) ?? "";
		((UIText)UICrack).Caption = "~r~" + LsFunctions.pCrackOunce + "~s~z";
		((UIText)UIcrack).Caption = LsFunctions.GramsToOz(LSL.PlayerInventory["Crack"]) ?? "";
		((UIText)UICocaine).Caption = "~r~" + LsFunctions.pCocaineOunce + "~s~z";
		((UIText)uIResText).Caption = LsFunctions.GramsToOz(LSL.PlayerInventory["Cocaine"]) ?? "";
		((UIText)UIWeedtxt).Draw();
		((UIText)UIWeed).Draw();
		((UIText)UIweed).Draw();
		((UIText)UICracktxt).Draw();
		((UIText)UICrack).Draw();
		((UIText)UIcrack).Draw();
		((UIText)UICocainetxt).Draw();
		((UIText)UICocaine).Draw();
		((UIText)uIResText).Draw();
		UI.ShowHudComponentThisFrame((HudComponent)3);
	}
}
