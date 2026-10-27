using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using LSlife.OldCustomers;
using LSlife.Shots;

namespace LSlife;

// Transactions client sans dependance aux composants NativeUI.
internal static class CustomerTransactionActions
{
	internal static bool HandleCustomerTransaction(bool sale)
	{
		if (LSL.customerSellTo == null) return false;
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		if (sale)
		{
			if (LSL.PlayerInventory[LSL.customerSellTo.Drug.Name] < LSL.customerSellTo.amount)
			{
				LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
				return false;
			}
			foreach (Drug d in LSL.areas[LSL.areaIndex].Drugs)
			{
				if (d.Name == LSL.customerSellTo.Drug.Name)
				{
					d.Supplied = true;
				}
			}
			if (Game.Player.Character.IsInVehicle())
			{
				LsFunctions.MakeStreetSale(LSL.customerSellTo, inCar: true);
				LSL.lastTimeDealDone = Game.GameTime;
			}
			else
			{
				LsFunctions.MakeStreetSale(LSL.customerSellTo, inCar: false);
				LSL.lastTimeDealDone = Game.GameTime;
			}
		}
		else
		{
			foreach (Drug d2 in LSL.areas[LSL.areaIndex].Drugs)
			{
				if (d2.Name == LSL.customerSellTo.Drug.Name)
				{
					d2.Supplied = false;
				}
			}
			if (LSL.customerSellTo.FarAway)
			{
				ShotHandler.RemoveShot(ShotHandler.shots.First((Shot s) => s.customer == LSL.customerSellTo));
				LSL.lastTimeDealDone = Game.GameTime;
				OldCustomerHandler.AddCustomer(LSL.customerSellTo.Ped, LSL.customerSellTo.Vehicle, LSL.HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(LSL.customerSellTo.Drug.Name, LSL.customerSellTo.amount));
				return true;
			}
			if ((Entity)(object)LSL.customerSellTo.Vehicle != (Entity)null && ((Entity)LSL.customerSellTo.Vehicle).Exists())
			{
				LSL.customerSellTo.Ped.Task.PerformSequence(LSL.PerformTaskSeq(1, LSL.customerSellTo.Vehicle, Vector3.Zero));
			}
			LSL.customers.Remove(LSL.customerSellTo);
			LSL.lastTimeDealDone = Game.GameTime;
			OldCustomerHandler.AddCustomer(LSL.customerSellTo.Ped, LSL.customerSellTo.Vehicle, LSL.HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(LSL.customerSellTo.Drug.Name, LSL.customerSellTo.amount));
			LSL.customerSellTo = null;
		}
		return true;
	}

}
