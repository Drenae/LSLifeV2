using GTA;
using GTA.Native;

namespace LSlife;

// Recrutement des dealers : regles de jeu sans dependance aux menus NativeUI.
internal static class DealerRecruitmentActions
{
	internal static void CancelDealerRecruitment()
	{
		if (LSL.aDealer != null)
		{
			LSL.aDealer.NewState = Enums.DStates.Normal;
		}
	}

	internal static void SubmitDealerOffer(int amount)
	{
		if (LSL.aDealer == null)
		{
			return;
		}
		LSL.aDealer.NewState = Enums.DStates.Normal;
		if (LSL.aDealer.LastOffer < amount)
		{
			if (LSL.aDealer.DecidePlayerOffer(amount))
			{
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_YES"), ("SPEECH_PARAMS_FORCE"));
				LsFunctions.ChangeAdealerToPdealer();
				GTA.UI.Screen.ShowSubtitle("Ouais, je veux bien bosser pour toi.", 4000);
				Player player = Game.Player;
				player.Money -= amount;
				LsFunctions.PlayTheSound();
				
				if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
				{
					LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
				}
			}
			else if (LSL.rnd.Next(0, 3) == 0)
			{
				LSL.aDealer.NewState = Enums.DStates.AttackPlayer;
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_CURSE_HIGH"), ("SPEECH_PARAMS_FORCE"));
				if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
				{
					LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
				}
			}
			else
			{
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_NO"), ("SPEECH_PARAMS_FORCE"));
				GTA.UI.Screen.ShowSubtitle("Tu plaisantes ? Je vaux bien plus que ça !", 4000);
				LSL.aDealer.SetLastOfferAmount(amount);
			}
		}
		else if (LSL.rnd.Next(0, 3) == 0)
		{
			LSL.aDealer.NewState = Enums.DStates.AttackPlayer;
			Function.Call(unchecked((Hash)(-8213159594590722974L)), (LSL.aDealer.dPed), ("GENERIC_CURSE_HIGH"), ("SPEECH_PARAMS_FORCE"));
			GTA.UI.Screen.ShowSubtitle("Nan, je vais plutôt te buter et prendre ton fric.", 4000);
			if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 && LSL.rnd.Next(0, 5) == 0)
			{
				LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
			}
		}
		else
		{
			GTA.UI.Screen.ShowSubtitle("Propose plus et je réfléchirai.", 4000);
		}
	}

}
