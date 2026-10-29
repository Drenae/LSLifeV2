using GTA;
using GTA.Native;

namespace LSlife;

// Actions Zee independantes des menus NativeUI.
internal static class ZeeContactActions
{
	internal static void CallWeaponDealer()
	{
		{
			if (LSL.playerHeat > 0f)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Non, tu attires trop l'attention.");
			}
			else if ((Entity)(object)LSL.weaponDealer1 == (Entity)null)
			{
				LSL.callDealer(wepdealer: true);
				LsFunctions.TextMsg("ZEE", "Travail", "J'envoie quelqu'un te rejoindre.");
			}
			else
			{
				LsFunctions.RemovePedFromWorld(LSL.weaponDealer1, clearTask: true);
				LSL.weaponDealer1 = null;
				LsFunctions.TextMsg("ZEE", "Travail", "OK, je leur dis de rentrer.");
			}
		}

	}

	internal static void CallLift()
	{
		{
			if (LSL.wepDealer == null)
			{
				LSL.wepDealer = new Dealer(new Model(unchecked((PedHash)(-412008429))), ((Entity)Game.Player.Character).Position + ((Entity)Game.Player.Character).ForwardVector * 2f, new Model("INGOT"), LSL.playerGroup);
				if ((Entity)(object)LSL.wepDealer.ped != (Entity)null)
				{
					LsFunctions.TextMsg("ZEE", LSL.wepDealer.ped, "Travail", "J'envoie quelqu'un te rejoindre.");
				}
			}
			else
			{
				LsFunctions.RemovePedFromWorld(LSL.wepDealer.ped, clearTask: true);
				Vehicle vehicle = LSL.wepDealer.vehicle;
				if (vehicle != null)
				{
					((Entity)vehicle).MarkAsNoLongerNeeded();
				}
				Vehicle vehicle2 = LSL.wepDealer.vehicle;
				if (vehicle2 != null)
				{
					Blip currentBlip = ((Entity)vehicle2).CurrentBlip;
					if (currentBlip != null)
					{
						currentBlip.Remove();
					}
				}
				LSL.wepDealer = null;
				LsFunctions.TextMsg("ZEE", "Travail", "OK, je leur dis que tu ne peux pas venir.");
			}
		}

	}

	internal static void PayZeeDebt()
	{
		if (LSL.player.Money > 0)
		{
			int toPay = 0;
			toPay = ((LSL.dealer1Debt >= LSL.player.Money) ? LSL.player.Money : LSL.dealer1Debt);
			LSL.dealer1Debt -= toPay;
			Player player = LSL.player;
			player.Money -= toPay;
			LSL.dealer1Money += toPay;
			LsFunctions.Dealer1AddRep(LsFunctions.MoneyToRep(toPay, _debt: true));
			LsFunctions.PlayTheSound();
			LsFunctions.SaveDealer();
			Script.Wait(500);
			if (LSL.dealer1Debt > 0)
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Merci d'avoir remboursé une partie de ta dette.");
			}
			else
			{
				LsFunctions.TextMsg("ZEE", "Travail", "Merci, ta dette est entièrement remboursée.");
			}
		}
		else
		{
			LsFunctions.TextMsg("ZEE", "Travail", "Tu n'as pas d'argent.");
		}
	}

}
