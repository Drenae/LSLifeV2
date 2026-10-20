using System.Xml.Linq;
using GTA;
using GTA.Math;

namespace LSlife;

public class DrugOrder
{
	public bool Placed;

	public bool pickedUp;

	public int weed { get; private set; }

	public int crack { get; private set; }

	public int cocaine { get; private set; }

	public int Bill { get; private set; }

	public void OrderWeed(int amount, int price)
	{
		weed += amount;
		Bill += price;
	}

	public void OrderCrack(int amount, int price)
	{
		crack += amount;
		Bill += price;
	}

	public void OrderCocaine(int amount, int price)
	{
		cocaine += amount;
		Bill += price;
	}

	public bool PayBill()
	{
		if (CanPayBill())
		{
			int cash = LSL.player.Money;
			int toPay = 0;
			toPay = ((cash >= Bill) ? Bill : cash);
			LSL.player.Money = LSL.player.Money - toPay;
			LSL.dealer1Money += toPay;
			if (toPay > 0)
			{
				LsFunctions.Dealer1AddRep(toPay / 100);
			}
			if (Bill - toPay > 0)
			{
				LSL.dealer1Debt += Bill - toPay;
			}
			LsFunctions.SaveDealer();
			LsFunctions.PlayTheSound();
			return true;
		}
		return false;
	}

	public bool CanPayBill()
	{
		int _bill = Bill;
		_bill -= LSL.player.Money;
		if (LSL.dealer1Debt + _bill <= LsFunctions.zeeDebtCap())
		{
			return true;
		}
		return false;
	}

	public void PickupDrugs()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerCanPickup())
		{
			if (weed > 0)
			{
				LsFunctions.pWeedGrams += weed;
			}
			if (crack > 0)
			{
				LsFunctions.pCrackGrams += crack;
			}
			if (cocaine > 0)
			{
				LsFunctions.pCocaineGrams += cocaine;
			}
			LsFunctions.PlayTheSound();
			pickedUp = true;
		}
		else if ((Entity)(object)LSL.pStashVehicle?.drugCar != (Entity)null)
		{
			StashVehicle stash = LSL.pStashVehicle;
			Vector3 position = ((Entity)stash.drugCar).Position;
			if (position.DistanceTo(((Entity)LSL.ZeesDealers.dealerCar).Position) < 20f)
			{
				Game.FadeScreenOut(1000);
				Script.Wait(2000);
				LSL.player.Character.SetIntoVehicle(stash.drugCar, (VehicleSeat)(-1));
				if (weed > 0)
				{
					LSL.pStashVehicle.drugCarWeedGrams += weed;
				}
				if (crack > 0)
				{
					LSL.pStashVehicle.drugCarCrackGrams += crack;
				}
				if (cocaine > 0)
				{
					LSL.pStashVehicle.drugCarCocainGrams += cocaine;
				}
				LsFunctions.PlayTheSound();
				pickedUp = true;
				Game.FadeScreenIn(500);
			}
			else
			{
				UI.ShowSubtitle("Ton véhicule doit être plus proche.");
			}
		}
		else
		{
			UI.ShowSubtitle("Il te faut un véhicule de stockage pour récupérer une commande aussi importante.");
		}
		if (pickedUp)
		{
			SaveOrder();
		}
	}

	private void SaveOrder()
	{
		LSL.LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (element.Name == "playerInventory")
			{
				foreach (XElement e in element.Elements())
				{
					if (e.Name == "weedGrams")
					{
						e.Value = LsFunctions.pWeedGrams.ToString();
					}
					if (e.Name == "crackGrams")
					{
						e.Value = LsFunctions.pCrackGrams.ToString();
					}
					if (e.Name == "cocaineGrams")
					{
						e.Value = LsFunctions.pCocaineGrams.ToString();
					}
				}
			}
			if (element.Name == "drugCarStash")
			{
				foreach (XElement e2 in element.Elements())
				{
					if (e2.Name == "weedGrams")
					{
						e2.Value = LSL.pStashVehicle.drugCarWeedGrams.ToString();
					}
					if (e2.Name == "crackGrams")
					{
						e2.Value = LSL.pStashVehicle.drugCarCrackGrams.ToString();
					}
					if (e2.Name == "cocaineGrams")
					{
						e2.Value = LSL.pStashVehicle.drugCarCocainGrams.ToString();
					}
				}
			}
			if (!(element.Name == "dealer1"))
			{
				continue;
			}
			foreach (XElement e3 in element.Elements())
			{
				if (e3.Name == "weed")
				{
					e3.Value = LSL.dealer1Weed.ToString();
				}
				if (e3.Name == "crack")
				{
					e3.Value = LSL.dealer1Crack.ToString();
				}
				if (e3.Name == "cocain")
				{
					e3.Value = LSL.dealer1Cocain.ToString();
				}
				if (e3.Name == "debt")
				{
					e3.Value = LSL.dealer1Debt.ToString();
				}
				if (e3.Name == "money")
				{
					e3.Value = LSL.dealer1Money.ToString();
				}
			}
		}
		LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
	}

	public bool PlayerCanPickup()
	{
		if ((long)LsFunctions.TotalDrugs(LSL.PlayerInventory) + OrderSizeInGrams() <= 7000)
		{
			return true;
		}
		return false;
	}

	private int OrderSizeInGrams()
	{
		return checked(weed + crack + cocaine);
	}

	internal bool OrderValid()
	{
		if (weed == 0 && crack == 0 && cocaine == 0)
		{
			LsFunctions.TextMsg("ZEE", "Commande", "Ta commande est vide");
			return false;
		}
		return true;
	}

	internal void PutBackDrugs()
	{
		LSL.dealer1Weed += weed;
		LSL.dealer1Crack += crack;
		LSL.dealer1Cocain += cocaine;
		LsFunctions.SaveDealer();
	}

	internal void ResetOrder()
	{
		weed = 0;
		crack = 0;
		cocaine = 0;
		Bill = 0;
		pickedUp = false;
		Placed = false;
	}
}
