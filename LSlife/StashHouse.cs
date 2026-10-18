using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class StashHouse
{
	public int id;

	public Vector3 stashPos;

	private Blip blip;

	private bool purchased;

	private int price;

	private Color color = Color.Yellow;

	public bool inside;

	public int Weed;

	public int Crack;

	public int Cocain;

	public int WeedOz;

	public int CrackOz;

	public int CocainOz;

	public int Money;

	public int Armor;

	public List<XElement> Weapons;

	public List<dynamic> Weps;

	public Vector3 houseHealthPickPos;

	public bool pickupSpawned;

	public static Pickup healthPickup;

	public Vector3 entrancePos
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public Vector3 exitPos
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public StashHouse(int _id, Vector3 _entrancePos, Vector3 _exitPos, Vector3 _stashPos, Vector3 _health, bool _purchased, int _price, bool _inside, int _Weed, int _Crack, int _Cocain, int _Money, int _Armor, List<XElement> _weapons, List<dynamic> _weps)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		id = _id;
		entrancePos = _entrancePos;
		_exitPos.Z -= 0.5f;
		exitPos = _exitPos;
		stashPos = _stashPos;
		purchased = _purchased;
		price = _price;
		blip = World.CreateBlip(entrancePos);
		blip.Sprite = (BlipSprite)40;
		blip.Name = "Planque";
		SetBlip();
		inside = _inside;
		Weapons = _weapons;
		Weps = _weps;
		Weed = _Weed;
		Crack = _Crack;
		Cocain = _Cocain;
		Money = _Money;
		Armor = _Armor;
		houseHealthPickPos = _health;
		if (purchased)
		{
			color = Color.Green;
		}
	}

	internal void AddDrugs()
	{
		Weed += LSL.PlayerInventory["Weed"];
		LSL.PlayerInventory["Weed"] = 0;
		Crack += LSL.PlayerInventory["Crack"];
		LSL.PlayerInventory["Crack"] = 0;
		Cocain += LSL.PlayerInventory["Cocaine"];
		LSL.PlayerInventory["Cocaine"] = 0;
		Weed += LsFunctions.pWeedOunce * 28;
		LsFunctions.pWeedOunce = 0;
		Crack += LsFunctions.pCrackOunce * 28;
		LsFunctions.pCrackOunce = 0;
		Cocain += LsFunctions.pCocaineOunce * 28;
		LsFunctions.pCocaineOunce = 0;
		LsFunctions.UpdateInventory();
		LSL.HouseHandler.UpdateMenu(this);
		LSL.HouseHandler.SaveHouse(this);
	}

	public void OnTick()
	{
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		if (!inside)
		{
			if (pickupSpawned && healthPickup != (Pickup)null && healthPickup.ObjectExists())
			{
				LsFunctions.DbugString("Soin supprimé");
				healthPickup.Delete();
				pickupSpawned = false;
			}
			if (LSL.DrawMarkers && LSL.playerPos.DistanceTo(entrancePos) < 30f)
			{
				World.DrawMarker((MarkerType)1, entrancePos, Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 1f), color);
			}
			if (!LSL.player.Character.IsStopped || !(LSL.playerPos.DistanceTo(entrancePos) < 2f))
			{
				return;
			}
			if (purchased)
			{
				if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
				{
					Function.Call((Hash)7023634693725934496L);
					LSL.displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour entrer dans la planque.");
				}
				if (!Game.IsControlJustReleased(2, (Control)51))
				{
					return;
				}
				Function.Call((Hash)7023634693725934496L);
				((Entity)LSL.player.Character).FreezePosition = true;
				foreach (PlayerDealer playerDealer in LSL.DealerHandler.dealers)
				{
					if (playerDealer.NewState == Enums.PdStates.FollowingPlayer)
					{
						playerDealer.NewState = Enums.PdStates.FollowPlayer;
						playerDealer.Ped.LeaveGroup();
					}
				}
				Game.FadeScreenOut(300);
				Script.Wait(300);
				((Entity)LSL.player.Character).Position = exitPos;
				Script.Wait(400);
				((Entity)LSL.player.Character).FreezePosition = false;
				Game.FadeScreenIn(300);
				inside = true;
				LSL.HouseHandler.CurrentHouse = this;
				foreach (Ped getPed in LSL.lPeds.GetPeds)
				{
					Model model = ((Entity)getPed).Model;
					PedHash hash = (PedHash)model.Hash;
					if (!new List<PedHash>
					{
						(PedHash)1581098148,
						(PedHash)368603149,
						unchecked((PedHash)(-1920001264)),
						unchecked((PedHash)(-1614285257)),
						unchecked((PedHash)(-277793362)),
						(PedHash)1096929346,
						unchecked((PedHash)(-1320879687))
					}.Contains(hash) && !LsFunctions.cHashes.Contains(hash))
					{
						LsFunctions.cHashes.Add(hash);
					}
				}
				SetInside(v: true);
				return;
			}
			if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
			{
				Function.Call((Hash)7023634693725934496L);
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour acheter cette propriété : $" + LsFunctions.IntToMoney(price));
			}
			if (Game.IsControlJustReleased(2, (Control)51))
			{
				if (LSL.player.Money >= price)
				{
					Player player = LSL.player;
					player.Money -= price;
					purchased = true;
					SetBlip();
					SetPurchased(v: true);
				}
				else
				{
					UI.ShowSubtitle("Tu n'as pas assez d'argent pour acheter cette propriété.");
				}
			}
			return;
		}
		if (LSL.HouseHandler.CurrentHouse == null)
		{
			LSL.HouseHandler.CurrentHouse = this;
		}
		if (LSL.playerPos.DistanceTo(exitPos) > LSL.playerPos.DistanceTo(entrancePos))
		{
			SetInside(v: false);
		}
		if (LSL.DrawMarkers)
		{
			World.DrawMarker((MarkerType)0, new Vector3(stashPos.X, stashPos.Y, stashPos.Z - 0.5f), Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
		}
		if (pickupSpawned)
		{
			if (healthPickup != (Pickup)null && !healthPickup.ObjectExists())
			{
				LsFunctions.DbugString("Soin supprimé");
				healthPickup.Delete();
				healthPickup = null;
				pickupSpawned = false;
			}
		}
		else
		{
			pickupSpawned = true;
			healthPickup = World.CreatePickup(unchecked((PickupType)(-1888453608)), houseHealthPickPos, Vector3.Zero, new Model("prop_ld_health_pack"), 100);
			LsFunctions.DbugString("Soin créé");
		}
		if (!LSL.LsMenuPool.IsAnyMenuOpen() && LSL.playerPos.DistanceTo(stashPos) <= 1f)
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
			{
				Function.Call((Hash)7023634693725934496L);
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour ouvrir la réserve.");
			}
			if (Game.IsControlJustReleased(2, (Control)51))
			{
				Function.Call((Hash)7023634693725934496L);
				LsFunctions.UpdateInventory();
				LSL.HouseHandler.UpdateMenu(this);
				LSL.HouseHandler.CurrentHouse = this;
				LSL.stashMainMenu.Visible = !LSL.stashMainMenu.Visible;
			}
		}
		if (LSL.playerPos.DistanceTo(exitPos) < 2f)
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 10000f)
			{
				Function.Call((Hash)7023634693725934496L);
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour sortir de la planque.");
			}
			if (Game.IsControlJustReleased(2, (Control)51))
			{
				Function.Call((Hash)7023634693725934496L);
				((Entity)LSL.player.Character).FreezePosition = true;
				Game.FadeScreenOut(300);
				Script.Wait(300);
				((Entity)LSL.player.Character).Position = entrancePos;
				Script.Wait(400);
				((Entity)LSL.player.Character).FreezePosition = false;
				Game.FadeScreenIn(300);
				inside = false;
				SetInside(v: false);
			}
		}
		if (LSL.stashMainMenu.Visible && LSL.playerPos.DistanceTo(stashPos) > 1f)
		{
			LSL.stashMainMenu.Visible = false;
		}
	}

	private void SetInside(bool v)
	{
		inside = v;
		XDocument save = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
		foreach (XElement e in save.Descendants())
		{
			if (!(e.Name == "house") || !(e.FirstAttribute.Value == id.ToString()))
			{
				continue;
			}
			foreach (XElement el in e.Elements())
			{
				if (el.Name == "inside")
				{
					el.Value = v.ToString();
					save.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
					break;
				}
			}
		}
	}

	private void SetPurchased(bool v)
	{
		XDocument save = XDocument.Load("scripts\\LSLife\\LSLife_StashHouses.xml");
		foreach (XElement e in save.Descendants())
		{
			if (!(e.Name == "house") || !(e.FirstAttribute.Value == id.ToString()))
			{
				continue;
			}
			foreach (XElement el in e.Elements())
			{
				if (el.Name == "purchased")
				{
					el.Value = v.ToString();
					save.Save("scripts\\LSLife\\LSLife_StashHouses.xml");
					break;
				}
			}
		}
	}

	public void RemoveBlip()
	{
		blip.Remove();
	}

	public void SetBlip()
	{
		if (purchased)
		{
			color = Color.Green;
			blip.Color = (BlipColor)2;
		}
		else
		{
			blip.Color = (BlipColor)4;
		}
	}
}
