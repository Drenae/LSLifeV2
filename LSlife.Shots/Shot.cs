using GTA;
using GTA.Native;

namespace LSlife.Shots;

public class Shot
{
	internal bool accepted;

	internal bool rejected;

	internal int timeSpawned;

	internal int timeShotOffered;

	internal float dist;

	internal bool shotOffered;

	internal Customer customer;

	public Shot(Customer _c)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		accepted = false;
		timeSpawned = Game.GameTime;
		customer = _c;
		dist = LSL.playerPos.DistanceTo(((Entity)customer.Ped).Position);
	}

	internal void Tick()
	{
		DoShot();
	}

	private void DoShot()
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		if (!LSL.LsMenuPool.IsAnyMenuOpen())
		{
			if (!shotOffered && !accepted && !rejected && !ShotHandler.ShotOffers)
			{
				float div = customer.amount / 7;
				int price = customer.amount * customer.Drug.PricePerG;
				if (div >= 1f)
				{
					price -= (int)div * customer.Drug.PricePerG;
				}
				LsFunctions.TextMsg("Drop", customer.Ped, "Text message", "Can you do ~g~" + LsFunctions.GramsToOz(customer.amount) + "~w~ of ~g~" + customer.Drug.Name + " ~w~for $" + LsFunctions.IntToMoney(price) + " to ~y~" + World.GetStreetName(((Entity)customer.Ped).Position) + " ~w~in ~y~" + World.GetZoneName(((Entity)customer.Ped).Position) + "~w~?");
				timeShotOffered = Game.GameTime;
				shotOffered = true;
			}
			if (shotOffered && !accepted && !rejected)
			{
				if ((float)Game.GameTime > LSL.displayHelpTimer + 5000f && !LSL.LsMenuPool.IsAnyMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					LSL.displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("~INPUT_CONTEXT_SECONDARY~ to reject or ~INPUT_CONTEXT~ to accept");
				}
				if (Game.IsControlJustPressed(2, (Control)52) && !LSL.LsMenuPool.IsAnyMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					rejected = true;
					shotOffered = false;
					if ((Entity)(object)customer.Ped != (Entity)null)
					{
						LsFunctions.TextMsg("Drop", customer.Ped, "Text message", "ok ill let you know if i want anymore");
						Function.Call((Hash)7023634693725934496L);
						LsFunctions.RemovePedFromWorld(customer.Ped, clearTask: true);
					}
				}
				if (Game.IsControlJustPressed(2, (Control)51) && !LSL.LsMenuPool.IsAnyMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					accepted = true;
					shotOffered = false;
					((Entity)customer.Ped).CurrentBlip.IsShortRange = false;
					LsFunctions.TextMsg("Drop", customer.Ped, "Text message", "See you there");
					if (LSL.PlayerInventory[customer.Drug.Name] >= customer.amount)
					{
						UI.ShowSubtitle("Deliver " + LsFunctions.GramsToOz(customer.amount) + " of ~g~" + customer.Drug.Name + "~w~ to ~y~" + World.GetStreetName(((Entity)customer.Ped).Position) + ".", 10000);
					}
					else
					{
						UI.ShowSubtitle("You do not have enough ~g~" + customer.Drug.Name + "~w~ go get ~g~" + LsFunctions.GramsToOz(customer.amount) + "~w~ and deliver it to ~y~" + World.GetStreetName(((Entity)customer.Ped).Position) + ".", 10000);
					}
				}
				if (Game.GameTime > timeShotOffered + 25000)
				{
					LsFunctions.TextMsg("Drop", customer.Ped, "Text message", "i got someone dont worry");
					if (LSL.areas[LSL.areaIndex].Reputation > 0f)
					{
						float repLost = LSL.areas[LSL.areaIndex].Reputation * 0.1f;
						LSL.areas[LSL.areaIndex].Reputation -= repLost;
					}
					Function.Call((Hash)7023634693725934496L);
					rejected = true;
					shotOffered = false;
					if ((Entity)(object)customer.Ped != (Entity)null)
					{
						LsFunctions.RemovePedFromWorld(customer.Ped, clearTask: true);
					}
				}
			}
			if (accepted && customer != null)
			{
				if (customer.Ped.CurrentPedGroup != LSL.player.Character.CurrentPedGroup && dist <= 10f)
				{
					customer.Ped.Task.ClearAll();
					Function.Call(unchecked((Hash)(-6974808092771143243L)), (((Entity)customer.Ped).Handle), (LSL.playerGroup));
					((Entity)customer.Ped).CurrentBlip.ShowRoute = false;
				}
				if (!LSL.LsMenuPool.IsAnyMenuOpen() && dist <= 2f)
				{
					if (LSL.player.Character.IsInVehicle() && customer.Ped.IsInVehicle() && LSL.player.Character.CurrentVehicle.Speed < 1f)
					{
						if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.displayHelpTimer = Game.GameTime;
							LsFunctions.DisplayHelpText("~INPUT_SPRINT~ to sell to this person.");
						}
						if (Game.IsControlJustPressed(2, (Control)21) && !LSL.LsMenuPool.IsAnyMenuOpen())
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.customerSellTo = customer;
							LSL.sell.Text = "Sell " + LsFunctions.GramsToOz(customer.amount) + " of " + customer.Drug.Name + "?";
							((UIText)LSL.customerMainMenu.Subtitle).Caption = "You have " + LsFunctions.GramsToOz(LSL.PlayerInventory[customer.Drug.Name]) + " of " + customer.Drug.Name;
							LSL.customerMainMenu.Visible = !LSL.customerMainMenu.Visible;
							LSL.customerMainMenu.CurrentSelection = 0;
						}
					}
					if (!LSL.player.Character.IsInVehicle() && LSL.player.Character.IsStopped && !LSL.player.Character.IsInCombat)
					{
						if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.displayHelpTimer = Game.GameTime;
							LsFunctions.DisplayHelpText("Press ~INPUT_SPRINT~ to sell to this person.");
						}
						if (Game.IsControlJustPressed(2, (Control)21) && !LSL.LsMenuPool.IsAnyMenuOpen())
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.customerSellTo = customer;
							LSL.sell.Text = "Sell " + LsFunctions.GramsToOz(customer.amount) + " of " + customer.Drug.Name + "?";
							((UIText)LSL.customerMainMenu.Subtitle).Caption = "You have " + LsFunctions.GramsToOz(LSL.PlayerInventory[customer.Drug.Name]) + " of " + customer.Drug.Name;
							LSL.customerMainMenu.Visible = !LSL.customerMainMenu.Visible;
							LSL.customerMainMenu.CurrentSelection = 0;
						}
					}
				}
			}
		}
		if (rejected)
		{
			LsFunctions.RemovePedFromWorld(customer.Ped, clearTask: true);
			ShotHandler.RemoveShot(this);
		}
	}

	internal void SlowTick()
	{
	}
}
