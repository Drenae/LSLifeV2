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
		var customer = LSL.customerSellTo;
		int areaIndex = LSL.areaIndex;
		if (customer?.Drug == null || customer.amount <= 0 || !LsFunctions.IsLivePed(customer.Ped) ||
			areaIndex < 0 || areaIndex >= LSL.areas.Count || LSL.areas[areaIndex] == null) return false;
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		if (sale)
		{
			if (!LSL.PlayerInventory.TryGetValue(customer.Drug.Name, out int stock) || stock < customer.amount)
			{
				LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
				return false;
			}
			if (!LsFunctions.MakeStreetSale(customer, Game.Player.Character.IsInVehicle())) return false;
			foreach (Drug drug in LSL.areas[areaIndex].Drugs)
			{
				if (drug.Name == customer.Drug.Name) drug.Supplied = true;
			}
			LSL.lastTimeDealDone = Game.GameTime;
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
				ShotHandler.RemoveShot(ShotHandler.shots.FirstOrDefault((Shot s) => s.customer == customer));
				LSL.lastTimeDealDone = Game.GameTime;
				OldCustomerHandler.AddCustomer(LSL.customerSellTo.Ped, LSL.customerSellTo.Vehicle, LSL.HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(LSL.customerSellTo.Drug.Name, LSL.customerSellTo.amount));
				LSL.customerSellTo = null;
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
