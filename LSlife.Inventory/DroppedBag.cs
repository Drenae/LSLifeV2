using System.Collections.Generic;
using System.Xml.Linq;
using GTA;
using GTA.Math;

namespace LSlife;

public class DroppedBag
{
	private int weed;

	private int crack;

	private int cocaine;

	private int weedZ;

	private int crackZ;

	private int cocaineZ;

	private int money;

	private int timeSpawned;

	private int timeToLive = 600000;

	private List<XElement> weapons;

	public Vector3 pos;

	private Blip blip;

	private Pickup pickup;

	private bool players;

	public Pig Pig;

	public DroppedBag(string _subtitleString, int _weed, int _crack, int _cocaine, int _weedZ, int _crackZ, int _cocaineZ, int _money, Vector3 _pos)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		weed = _weed;
		crack = _crack;
		cocaine = _cocaine;
		weedZ = _weedZ;
		crackZ = _crackZ;
		cocaineZ = _cocaineZ;
		money = _money;
		pos = _pos;
		Innit(_subtitleString);
	}

	public DroppedBag(string _subtitleString, int _weed, int _crack, int _cocaine, int _money, List<XElement> _weapons, Vector3 _pos)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		weed = _weed;
		crack = _crack;
		cocaine = _cocaine;
		money = _money;
		weapons = _weapons;
		pos = _pos;
		Innit(_subtitleString);
	}

	private void Innit(string _subtitle)
	{
		CreateBlip();
		CreatePickup();
		GTA.UI.Screen.ShowSubtitle(_subtitle, 5000);
		timeSpawned = Game.GameTime;
	}

	public bool CheckIfPickedUp()
	{
		if (pickup != (Pickup)null && !pickup.ObjectExists())
		{
			return true;
		}
		return false;
	}

	public void AddStuffToPlayer()
	{
		LSL.PlayerInventory["Weed"] += weed;
		LSL.PlayerInventory["Crack"] += crack;
		LSL.PlayerInventory["Cocaine"] += cocaine;
		LsFunctions.pWeedOunce += weedZ;
		LsFunctions.pCrackOunce += crackZ;
		LsFunctions.pCocaineOunce += cocaineZ;
		Player player = Game.Player;
		player.Money += money;
		if (weapons != null)
		{
			LsFunctions.GivePedWeapons(Game.Player.Character, weapons);
		}
		GTA.UI.Screen.ShowSubtitle("Tu as ramassé " + LsFunctions.GramsToOz(weed) +  " de cannabis, " + LsFunctions.GramsToOz(crack) + " de crack, " + LsFunctions.GramsToOz(cocaine) + " de cocaïne et " + LsFunctions.IntToMoney(money) + " $", 5000);
		LsFunctions.UpdateInventory();
	}

	private void CreateBlip()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		blip = World.CreateBlip(pos);
		blip.Color = (BlipColor)2;
		blip.Name = "Bag";
		blip.IsShortRange = true;
	}

	private void RemoveBlip()
	{
		blip.Delete();
	}

	public bool CanDespawn()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Game.GameTime > timeSpawned + timeToLive)
		{
			return true;
		}
		if (Game.GameTime > timeSpawned + 10000 && LSL.playerPos.DistanceTo(pickup.Position) < 20f)
		{
			LsFunctions.DbugString("bag timer reset");
			timeSpawned = Game.GameTime;
		}
		return false;
	}

	public void RemovePickup()
	{
		pickup.Delete();
		RemoveBlip();
		if (LSL.DEBUG)
		{
			LsFunctions.DbugString("Pickup Deleted");
		}
	}

	private void CreatePickup()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		pickup = World.CreatePickup((PickupType)738282662, pos, new Model("prop_big_bag_01"), 0);
		while (pickup == (Pickup)null)
		{
			Script.Wait(0);
		}
	}
}
