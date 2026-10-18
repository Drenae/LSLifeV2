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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected Obj, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected Obj, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected Obj, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected Obj, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected Obj, but got Unknown
		LoadInvConfig();
		Options = LSL.LsMenuPool.AddSubMenu(LSL.zeeMenu, "Options");
		InventoryPosX = new UIMenuSliderItem("PosX: ~g~" + posX);
		InventoryPosX.Maximum = 1920;
		InventoryPosX.Value = posX;
		InventoryPosY = new UIMenuSliderItem("PosY: ~g~" + posY);
		InventoryPosY.Maximum = 1080;
		InventoryPosY.Value = posY;
		Save = new UIMenuItem("Enregistrer les reglages");
		Options.AddItem((UIMenuItem)(object)InventoryPosX);
		Options.AddItem((UIMenuItem)(object)InventoryPosY);
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
			((UIMenuItem)sender).Text = "PosX: ~g~" + posX;
		}
		if (sender == InventoryPosY)
		{
			posY = sender.Value;
			((UIMenuItem)sender).Text = "PosY: ~g~" + posY;
		}
	}

	public static void InventoryGui()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected Obj, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected Obj, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected Obj, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected Obj, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected Obj, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected Obj, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected Obj, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected Obj, but got Unknown
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		Point UIpos = new Point(posX, posY);
		UIResText UIWeedtxt = new UIResText("-Cannabis-", UIpos, 0.4f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		UIWeedtxt.DropShadow = true;
		((UIText)UIWeedtxt).Shadow = true;
		UIWeedtxt.Outline = true;
		UIResText UIWeed = new UIResText("", new Point(UIpos.X, UIpos.Y + 20), 0.5f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		((UIText)UIWeed).Enabled = true;
		UIWeed.DropShadow = true;
		((UIText)UIWeed).Shadow = true;
		UIWeed.Outline = true;
		UIResText UIweed = new UIResText("", new Point(UIpos.X, UIpos.Y + 45), 0.5f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		((UIText)UIweed).Enabled = true;
		UIweed.DropShadow = true;
		((UIText)UIweed).Shadow = true;
		UIweed.Outline = true;
		UIResText UICracktxt = new UIResText("-Crack-", new Point(UIpos.X, UIpos.Y + 80), 0.4f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		UICracktxt.DropShadow = true;
		((UIText)UICracktxt).Shadow = true;
		UICracktxt.Outline = true;
		UIResText UICrack = new UIResText("", new Point(UIpos.X, UIpos.Y + 80 + 20), 0.5f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		((UIText)UICrack).Enabled = true;
		UICrack.DropShadow = true;
		((UIText)UICrack).Shadow = true;
		UICrack.Outline = true;
		UIResText UIcrack = new UIResText("", new Point(UIpos.X, UIpos.Y + 100 + 25), 0.5f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		((UIText)UIcrack).Enabled = true;
		UIcrack.DropShadow = true;
		((UIText)UIcrack).Shadow = true;
		UIcrack.Outline = true;
		UIResText UICocainetxt = new UIResText("-Cocaine-", new Point(UIpos.X, UIpos.Y + 160), 0.4f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		UICocainetxt.DropShadow = true;
		((UIText)UICocainetxt).Shadow = true;
		UICocainetxt.Outline = true;
		UIResText UICocaine = new UIResText("", new Point(UIpos.X, UIpos.Y + 160 + 20), 0.5f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2);
		((UIText)UICocaine).Enabled = true;
		UICocaine.DropShadow = true;
		((UIText)UICocaine).Shadow = true;
		UICocaine.Outline = true;
		UIResText val = new UIResText("", new Point(UIpos.X, UIpos.Y + 160 + 20 + 25), 0.5f, Color.WhiteSmoke, (GTA.Font)7, (UIResText.Alignment)2)
		{
			Enabled = true,
			DropShadow = true,
			Shadow = true,
			Outline = true
		};
		((UIText)UIWeed).Caption = "~r~" + LsFunctions.pWeedOunce + "~s~z";
		((UIText)UIweed).Caption = LsFunctions.GramsToOz(LSL.PlayerInventory["Weed"]) ?? "";
		((UIText)UICrack).Caption = "~r~" + LsFunctions.pCrackOunce + "~s~z";
		((UIText)UIcrack).Caption = LsFunctions.GramsToOz(LSL.PlayerInventory["Crack"]) ?? "";
		((UIText)UICocaine).Caption = "~r~" + LsFunctions.pCocaineOunce + "~s~z";
		((UIText)val).Caption = LsFunctions.GramsToOz(LSL.PlayerInventory["Cocaine"]) ?? "";
		((UIText)UIWeedtxt).Draw();
		((UIText)UIWeed).Draw();
		((UIText)UIweed).Draw();
		((UIText)UICracktxt).Draw();
		((UIText)UICrack).Draw();
		((UIText)UIcrack).Draw();
		((UIText)UICocainetxt).Draw();
		((UIText)UICocaine).Draw();
		((UIText)val).Draw();
		UI.ShowHudComponentThisFrame((HudComponent)3);
	}
}
