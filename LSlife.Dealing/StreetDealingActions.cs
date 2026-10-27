using GTA;
using GTA.Math;

namespace LSlife;

// Logique des activites de vente, independante des callbacks NativeUI.
internal static class StreetDealingActions
{
	internal static void StartDealing(bool weed, bool crack, bool cocaine, bool deliveries)
	{
		LSL.sellingWeed = weed;
		LSL.sellingCrack = crack;
		LSL.sellingCocaine = cocaine;
		LSL.deliveriesEnabled = deliveries;
		if (!LSL.sellingWeed && !LSL.sellingCrack && !LSL.sellingCocaine)
		{
			LsFunctions.TextMsg("ZEE", "Travail", "Dis-moi d'abord ce que tu veux vendre.");
		}
		else if (!LSL.isDealing)
		{
			int total = LsFunctions.TotalDrugs(LSL.PlayerInventory) + LSL.HouseHandler.TotalStashDrugs();
			if (LSL.pStashVehicle != null)
			{
				total += LSL.pStashVehicle.TotalDrugs();
			}
			// L'interface appelante gere sa propre visibilite.
			if (LSL.playerHeat > 0f)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Tu attires trop l'attention dans ce quartier.");
			}
			else if (total > 0)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "OK, je vais faire passer le mot.");
				LSL.isDealing = true;
				LSL.lastStreet = World.GetStreetName(new Vector2(((Entity)Game.Player.Character).Position.X, ((Entity)Game.Player.Character).Position.Y));
				LSL.thisStreet = LSL.lastStreet;
				LSL.timeStartedDealing = Game.GameTime;
			}
			else
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Tu n'as rien à vendre.");
			}
		}
		else if (LSL.isDealing)
		{
			LSL.isDealing = false;
			LsFunctions.TextMsg("ZEE", "Travail", "Tu n'as rien à vendre.");
		}
	}

	internal static bool PlaceDrugOrder()
	{
		if (LSL.NewOrder == null || !LSL.NewOrder.OrderValid()) return false;
		LSL.NewOrder.Placed = true;
		LSL.callDealer(wepdealer: false);
		return true;
	}

}
