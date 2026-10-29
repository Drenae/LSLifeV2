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
		if (!CustomMenuShared.IsAnyGameplayMenuOpen())
		{
			if (!shotOffered && !accepted && !rejected && !ShotHandler.ShotOffers)
			{
				float div = customer.amount / 7;
				int price = customer.amount * customer.Drug.PricePerG;
				if (div >= 1f)
				{
					price -= (int)div * customer.Drug.PricePerG;
				}
				LsFunctions.TextMsg("Livraison", customer.Ped, "Message", "Tu peux me livrer ~g~" + LsFunctions.GramsToOz(customer.amount) + "~w~ de ~g~" + LsFunctions.DrugDisplayName(customer.Drug.Name) + " ~w~pour $" + LsFunctions.IntToMoney(price) + " à ~y~" + World.GetStreetName(((Entity)customer.Ped).Position) + " ~w~dans ~y~" + World.GetZoneName(((Entity)customer.Ped).Position) + "~w~?");
				timeShotOffered = Game.GameTime;
				shotOffered = true;
			}
			if (shotOffered && !accepted && !rejected)
			{
				if ((float)Game.GameTime > LSL.displayHelpTimer + 5000f && !CustomMenuShared.IsAnyGameplayMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					LSL.displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("~INPUT_CONTEXT_SECONDARY~ pour refuser ou ~INPUT_CONTEXT~ pour accepter");
				}
				if (Game.IsControlJustPressed(2, (Control)52) && !CustomMenuShared.IsAnyGameplayMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					rejected = true;
					shotOffered = false;
					if ((Entity)(object)customer.Ped != (Entity)null)
					{
						LsFunctions.TextMsg("Livraison", customer.Ped, "Message", "OK, je te dirai si j'en veux encore.");
						Function.Call((Hash)7023634693725934496L);
						LsFunctions.RemovePedFromWorld(customer.Ped, clearTask: true);
					}
				}
				if (Game.IsControlJustPressed(2, (Control)51) && !CustomMenuShared.IsAnyGameplayMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					accepted = true;
					shotOffered = false;
					((Entity)customer.Ped).CurrentBlip.IsShortRange = false;
					LsFunctions.TextMsg("Livraison", customer.Ped, "Message", "On se retrouve là-bas.");
					if (LSL.PlayerInventory[customer.Drug.Name] >= customer.amount)
					{
						UI.ShowSubtitle("Livre " + LsFunctions.GramsToOz(customer.amount) + " de ~g~" + LsFunctions.DrugDisplayName(customer.Drug.Name) + "~w~ à ~y~" + World.GetStreetName(((Entity)customer.Ped).Position) + ".", 10000);
					}
					else
					{
						UI.ShowSubtitle("Tu n’as pas assez de ~g~" + LsFunctions.DrugDisplayName(customer.Drug.Name) + "~w~. Récupère ~g~" + LsFunctions.GramsToOz(customer.amount) + "~w~ et livre-la à ~y~" + World.GetStreetName(((Entity)customer.Ped).Position) + ".", 10000);
					}
				}
				if (Game.GameTime > timeShotOffered + 25000)
				{
					LsFunctions.TextMsg("Livraison", customer.Ped, "Message", "J'ai trouvé quelqu'un, ne t'inquiète pas.");
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
				if (!CustomMenuShared.IsAnyGameplayMenuOpen() && dist <= 2f)
				{
					if (LSL.player.Character.IsInVehicle() && customer.Ped.IsInVehicle() && LSL.player.Character.CurrentVehicle.Speed < 1f)
					{
						if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.displayHelpTimer = Game.GameTime;
							LsFunctions.DisplayHelpText("~INPUT_SPRINT~ pour vendre à cette personne.");
						}
						if (Game.IsControlJustPressed(2, (Control)21) && !CustomMenuShared.IsAnyGameplayMenuOpen())
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.customerSellTo = customer;
							
							
							CustomCustomerMenu.Open();
							
						}
					}
					if (!LSL.player.Character.IsInVehicle() && LSL.player.Character.IsStopped && !LSL.player.Character.IsInCombat)
					{
						if ((float)Game.GameTime > LSL.displayHelpTimer + 3000f)
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.displayHelpTimer = Game.GameTime;
							LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour vendre à cette personne.");
						}
						if (Game.IsControlJustPressed(2, (Control)21) && !CustomMenuShared.IsAnyGameplayMenuOpen())
						{
							Function.Call((Hash)7023634693725934496L);
							LSL.customerSellTo = customer;
							
							
							CustomCustomerMenu.Open();
							
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
