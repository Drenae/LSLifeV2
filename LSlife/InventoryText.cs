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

	private static string hudBackgroundPath;

	private static bool hudSpriteReady;

	private static string renderedHudPath;

	private static string lastHudState;

	private static int hudRenderNumber;

	private static readonly System.Collections.Generic.Queue<string> oldHudImages = new System.Collections.Generic.Queue<string>();

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

	private static string CleanHudText(string caption)
	{
		// Les codes de mise en forme GTA (~g~, ~r~, ~s~, etc.) ne sont pas interprétés par GDI+.
		return System.Text.RegularExpressions.Regex.Replace(caption ?? "", @"~[^~]*~", "");
	}

	private static void DrawHudText(Graphics graphics, string caption, int x, int y, float size, Color color)
	{
		using (System.Drawing.Font font = new System.Drawing.Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Pixel))
		using (Brush brush = new SolidBrush(color))
		using (Brush shadow = new SolidBrush(Color.FromArgb(190, 0, 0, 0)))
		using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
		{
			graphics.DrawString(CleanHudText(caption), font, shadow, new PointF(x + 1, y + 1), format);
			graphics.DrawString(CleanHudText(caption), font, brush, new PointF(x, y), format);
		}
	}

	private static void DrawHudBackground(int left, int top, int width, int height, string money,
		string[] ready, string[] raw, bool selling)
	{
		if (!hudSpriteReady)
		{
			new UIResRectangle(new Point(left, top), new Size(width, height), Color.FromArgb(115, 0, 0, 0)).Draw();
			return;
		}

		// Une seule texture Direct3D pour le fond ET les textes : pas de superposition.
		string state = money + "|" + string.Join("|", ready) + "|" + string.Join("|", raw) + "|" + selling;
		if (state != lastHudState || renderedHudPath == null)
		{
			try
			{
				using (Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
				using (Graphics graphics = Graphics.FromImage(bitmap))
				using (Image background = Image.FromFile(hudBackgroundPath))
				{
					graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
					graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(background, 0, 0, width, height);
					DrawHudText(graphics, "ARGENT", 60, 8, 19f, Color.White);
					DrawHudText(graphics, money, 290, 8, 19f, Color.LightGreen);
					string[] names = { "Cannabis", "Crack", "Cocaïne" };
					for (int i = 0; i < 3; i++)
					{
						int x = 75 + i * 109;
						DrawHudText(graphics, names[i], x, 45, 19f, Color.WhiteSmoke);
						DrawHudText(graphics, ready[i], x, 70, 19f, Color.LightGreen);
						DrawHudText(graphics, raw[i], x, 100, 19f, Color.LightCoral);
					}
					DrawHudText(graphics, "MODE VENTE", 80, 130, 19f, Color.WhiteSmoke);
					DrawHudText(graphics, selling ? "ACTIVÉ" : "DÉSACTIVÉ", 290, 130, 19f,
						selling ? Color.LightGreen : Color.LightCoral);
					// Nom distinct : DrawTexture peut conserver en cache les chemins déjà chargés.
					string next = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(hudBackgroundPath),
						"hud_render_" + (++hudRenderNumber) + ".png");
					bitmap.Save(next, System.Drawing.Imaging.ImageFormat.Png);
					if (renderedHudPath != null)
						oldHudImages.Enqueue(renderedHudPath);
					renderedHudPath = next;
					lastHudState = state;
					// Nettoyage des fichiers temporaires sur disque.
					while (oldHudImages.Count > 8)
					{
						string old = oldHudImages.Dequeue();
						try { System.IO.File.Delete(old); } catch (System.IO.IOException) { }
					}
				}
			}
			catch (System.Exception)
			{
					// En cas d'échec de création, conserver le dernier rendu valide.
			}
		}
		if (renderedHudPath != null)
			NativeUI.Sprite.DrawTexture(renderedHudPath, new Point(left, top), new Size(width, height));
	}

	private static void PrepareHudSprite()
	{
		// Sprite embarqué dans la DLL : aucune installation manuelle de PNG.
		try
		{
			string folder = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
				"LSLife", "sprites");
			System.IO.Directory.CreateDirectory(folder);
			hudBackgroundPath = System.IO.Path.Combine(folder, "hud_background.png");
			using (System.IO.Stream resource = typeof(InventoryText).Assembly.GetManifestResourceStream("LSlife.HudBackground"))
			{
				if (resource == null)
					return;
				using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
				{
					resource.CopyTo(memory);
					byte[] png = memory.ToArray();
					if (!System.IO.File.Exists(hudBackgroundPath) ||
						!System.Linq.Enumerable.SequenceEqual(System.IO.File.ReadAllBytes(hudBackgroundPath), png))
						System.IO.File.WriteAllBytes(hudBackgroundPath, png);
				}
			}
			hudSpriteReady = true;
		}
		catch (System.Exception)
		{
			// Ne pas empêcher le fonctionnement du mod si le PNG est inaccessible.
			hudSpriteReady = false;
		}
	}

	private static void DrawHudBackground(int left, int top, int width, int height)
	{
		if (hudSpriteReady)
		{
			NativeUI.Sprite.DrawTexture(hudBackgroundPath, new Point(left, top), new Size(width, height));
		}
		else
		{
			new UIResRectangle(new Point(left, top), new Size(width, height),
				Color.FromArgb(115, 0, 0, 0)).Draw();
		}
	}

	public static void InventoryGui()
	{
		// Coordonnées en espace NativeUI 1920 x 1080, comme les réglages existants.
		const int width = 365;
		const int height = 164;
		int left = posX - width;
		int top = System.Math.Max(0, posY - 25);
		string money = LsFunctions.IntToMoney(LSL.player.Money) + " $";
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
		DrawHudBackground(left, top, width, height, money, ready, raw, LSL.isDealing);

		// Masquer uniquement le compteur natif pendant l'affichage du panneau.
		// Il reste disponible normalement lorsque le panneau du mod n'est pas affiché.
		UI.HideHudComponentThisFrame((HudComponent)3);
	}
}
