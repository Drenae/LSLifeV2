using System.Xml.Linq;
using GTA;
using GTA.Math;

namespace LSlife;

public class DrugOrder
{
	public bool Placed;

	private bool payed;

	public bool pickedUp;

	public decimal weed { get; private set; }

	public decimal crack { get; private set; }

	public decimal cocaine { get; private set; }

	private decimal bill;

	public int Bill => (int)decimal.Round(bill, 0, System.MidpointRounding.AwayFromZero);

	public void OrderWeed(decimal amount, decimal price)
	{
		weed += amount;
		bill += price;
	}

	public void OrderCrack(decimal amount, decimal price)
	{
		crack += amount;
		bill += price;
	}

	public void OrderCocaine(decimal amount, decimal price)
	{
		cocaine += amount;
		bill += price;
	}

	public bool PayBill()
	{
		if (CanPayBill())
		{
			payed = true;
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
			payed = true;
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
				LsFunctions.playerBulkWeedGrams += weed;
			}
			if (crack > 0)
			{
				LsFunctions.playerBulkCrackGrams += crack;
			}
			if (cocaine > 0)
			{
				LsFunctions.playerBulkCocaineGrams += cocaine;
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
				LSL.player.Character.SetIntoVehicle(stash.drugCar, unchecked((VehicleSeat)(-1)));
				if (weed > 0)
				{
					LSL.pStashVehicle.carBulkWeedGrams += weed;
				}
				if (crack > 0)
				{
					LSL.pStashVehicle.carBulkCrackGrams += crack;
				}
				if (cocaine > 0)
				{
					LSL.pStashVehicle.carBulkCocaineGrams += cocaine;
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
		LSL.LSLifeSave = XmlSaveFile.Load("scripts\\LSLife\\LSLifeSave.xml");
		foreach (XElement element in LSL.LSLifeSave.Descendants())
		{
			if (element.Name == "playerInventory")
			{
				foreach (XElement e in element.Elements())
				{
					if (e.Name == "weedOz")
					{
						LsFunctions.WriteStockGrams(e, LsFunctions.playerBulkWeedGrams);
					}
					if (e.Name == "crackOz")
					{
						LsFunctions.WriteStockGrams(e, LsFunctions.playerBulkCrackGrams);
					}
					if (e.Name == "cocaineOz")
					{
						LsFunctions.WriteStockGrams(e, LsFunctions.playerBulkCocaineGrams);
					}
				}
			}
			if (element.Name == "drugCarStash" && LSL.pStashVehicle != null)
			{
				foreach (XElement e2 in element.Elements())
				{
					if (e2.Name == "weedOz")
					{
						LsFunctions.WriteStockGrams(e2, LSL.pStashVehicle.carBulkWeedGrams);
					}
					if (e2.Name == "crackOz")
					{
						LsFunctions.WriteStockGrams(e2, LSL.pStashVehicle.carBulkCrackGrams);
					}
					if (e2.Name == "cocainOz")
					{
						LsFunctions.WriteStockGrams(e2, LSL.pStashVehicle.carBulkCocaineGrams);
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
					LsFunctions.WriteStockGrams(e3, LSL.dealerWeedGrams);
				}
				if (e3.Name == "crack")
				{
					LsFunctions.WriteStockGrams(e3, LSL.dealerCrackGrams);
				}
				if (e3.Name == "cocain")
				{
					LsFunctions.WriteStockGrams(e3, LSL.dealerCocaineGrams);
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
		XmlSaveFile.Save(LSL.LSLifeSave, "scripts\\LSLife\\LSLifeSave.xml");
	}

	public bool PlayerCanPickup()
	{
		if (LSL.TotalDrugsCarried() + OrderSizeInGrams() < LsFunctions.OuncesToGrams(256))
		{
			return true;
		}
		return false;
	}

	private decimal OrderSizeInGrams()
	{
		return weed + crack + cocaine;
	}

	internal bool OrderValid()
	{
		if (weed == 0 && crack == 0 && cocaine == 0)
		{
			LsFunctions.TextMsg("ZEE", "Commande", "Ta commande est vide.");
			return false;
		}
		return true;
	}

	internal void PutBackDrugs()
	{
		LSL.dealerWeedGrams += weed;
		LSL.dealerCrackGrams += crack;
		LSL.dealerCocaineGrams += cocaine;
		LsFunctions.SaveDealer();
	}

	internal void ResetOrder()
	{
		weed = 0;
		crack = 0;
		cocaine = 0;
		bill = 0;
		payed = false;
		pickedUp = false;
		Placed = false;
	}
}
