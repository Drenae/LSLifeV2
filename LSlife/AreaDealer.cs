using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class AreaDealer
{
	private bool DEBUG;

	public Ped dPed;

	public Vehicle Vehicle;

	public Enums.DStates CurrentState;

	public Enums.DStates NewState = Enums.DStates.Normal;

	public Enums.DealState dealState;

	public bool Detected;

	private Vector3 servePos = Vector3.Zero;

	public LsTimer cTimer = new LsTimer(5, timeOut: true, start: false);

	public Ped cPed;

	private int time;

	public string Area;

	public int LastCustomer { get; private set; }

	public int LastOffer { get; private set; }

	public AreaDealer(Ped p)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		dPed = p;
		((Entity)dPed).IsPersistent = true;
		dPed.Armor = 100;
		WeaponHash[] weaponHashes = { WeaponHash.Pistol, WeaponHash.MicroSMG, WeaponHash.SMG, WeaponHash.AssaultRifle };
		dPed.Weapons.Give(weaponHashes[LSL.rnd.Next(0, weaponHashes.Count())], 100, false, true);
		dPed.RelationshipGroup = LSL.rival_nutral;
		LsFunctions.SetRandomCombatStats(dPed);
		dPed.FiringPattern = unchecked((FiringPattern)(-957453492));
		dPed.CanSwitchWeapons = true;
		Function.Call(unchecked((Hash)(-4079649877180351064L)), (dPed), (100));
		if (dPed.IsInVehicle())
		{
			Vehicle = dPed.CurrentVehicle;
		}
		DoState();
		CheckState();
		if (DEBUG)
		{
			((Entity)dPed).AddBlip();
		}
		dPed.Money = LSL.rnd.Next(200, 2000);
		Area = World.GetZoneNameLabel(((Entity)dPed).Position);
	}

	public void CheckState()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		servePos = World.GetNextPositionOnSidewalk(((Entity)dPed).Position + ((Entity)dPed).ForwardVector);
		Vector3 position;
		switch (CurrentState)
		{
		case Enums.DStates.WalkToPath:
			position = ((Entity)dPed).Position;
			if (position.DistanceTo(servePos) < 1f)
			{
				NewState = Enums.DStates.Normal;
			}
			if (playerNear())
			{
				NewState = Enums.DStates.StopForPlayer;
			}
			break;
		case Enums.DStates.Normal:
			position = ((Entity)dPed).Position;
			if (position.DistanceTo(servePos) < 1f)
			{
				NewState = Enums.DStates.Dealing;
			}
			else
			{
				NewState = Enums.DStates.WalkToPath;
			}
			if (playerNear())
			{
				NewState = Enums.DStates.StopForPlayer;
			}
			if (ShouldAttackPlayer())
			{
				NewState = Enums.DStates.AttackPlayer;
			}
			break;
		case Enums.DStates.Dealing:
			if ((Entity)(object)cPed == (Entity)null)
			{
				position = ((Entity)dPed).Position;
				if (position.DistanceTo(servePos) > 2.5f)
				{
					NewState = Enums.DStates.WalkToPath;
				}
			}
			if (playerNear())
			{
				NewState = Enums.DStates.StopForPlayer;
			}
			if (ShouldAttackPlayer())
			{
				NewState = Enums.DStates.AttackPlayer;
			}
			break;
		case Enums.DStates.StopForPlayer:
			if (dPed.IsStopped)
			{
				NewState = Enums.DStates.LookAtPlayer;
			}
			if (!playerNear())
			{
				NewState = Enums.DStates.Normal;
			}
			if (ShouldAttackPlayer())
			{
				NewState = Enums.DStates.AttackPlayer;
			}
			if ((Entity)(object)cPed != (Entity)null)
			{
				LsFunctions.RemovePedFromWorld(cPed, clearTask: true);
				Blip currentBlip = ((Entity)cPed).CurrentBlip;
				if (currentBlip != null)
				{
					currentBlip.Remove();
				}
				cPed = null;
			}
			break;
		case Enums.DStates.LookAtPlayer:
			if (!dPed.IsHeadtracking((Entity)(object)Game.Player.Character))
			{
				dPed.Task.LookAt((Entity)(object)Game.Player.Character);
			}
			else
			{
				NewState = Enums.DStates.TalkToPlayer;
			}
			if (!playerNear())
			{
				NewState = Enums.DStates.Normal;
			}
			if (ShouldAttackPlayer())
			{
				NewState = Enums.DStates.AttackPlayer;
			}
			break;
		case Enums.DStates.TalkToPlayer:
			if (!playerNear())
			{
				NewState = Enums.DStates.Normal;
				Function.Call(unchecked((Hash)(-4137652152022615111L)), (dPed), ("GENERIC_BYE"), ("SPEECH_PARAMS_FORCE"));
			}
			if (ShouldAttackPlayer())
			{
				NewState = Enums.DStates.AttackPlayer;
			}
			break;
		case Enums.DStates.AttackPlayer:
			if (Area == LSL.areas[LSL.areaIndex].Name && LSL.driveByHandler.GetDriveBies.Count < 1 && LsFunctions.RanRange(0, (int)LSL.areas[LSL.areaIndex].GangPresance + 1) != 0)
			{
				LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
			}
			if (!ShouldAttackPlayer())
			{
				NewState = Enums.DStates.Normal;
				if ((!Detected && ((Entity)dPed).CurrentBlip != (Blip)null) || ((Entity)dPed).CurrentBlip.Exists())
				{
					((Entity)dPed).CurrentBlip.Remove();
				}
			}
			break;
		case Enums.DStates.NoState:
			break;
		}
	}

	public void DoState()
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		if ((Entity)(object)dPed != (Entity)null)
		{
			if (dPed.RelationshipGroup != LSL.rival_nutral && !dPed.IsInCombat)
			{
				if (dPed.RelationshipGroup != LSL.rival_enemy)
				{
					LsFunctions.DbugString("~g~ Rival dealer relasion ship changed to rival_enemy");
				}
				else
				{
					LsFunctions.DbugString("~r~ Rival dealer relasion ship changed to something. ~n~Changing it to rival_nutral");
					dPed.RelationshipGroup = LSL.rival_nutral;
				}
			}
			if (NewState != CurrentState)
			{
				if (DEBUG)
				{
					UI.Notify("adealer state = " + NewState);
				}
				switch (NewState)
				{
				case Enums.DStates.WalkToPath:
					dPed.Task.ClearAll();
					dPed.Task.GoTo(servePos);
					CurrentState = Enums.DStates.WalkToPath;
					break;
				case Enums.DStates.Normal:
					if ((Entity)(object)Vehicle != (Entity)null)
					{
						if (((Entity)dPed.CurrentVehicle.Driver).Handle != ((Entity)dPed).Handle)
						{
							dPed.CurrentVehicle.Driver.Task.DriveTo(dPed.CurrentVehicle, World.GetNextPositionOnSidewalk(((Entity)dPed.CurrentVehicle).Position), 3f, 5f);
						}
						else
						{
							dPed.Task.DriveTo(dPed.CurrentVehicle, World.GetNextPositionOnSidewalk(((Entity)dPed.CurrentVehicle).Position), 3f, 5f);
						}
					}
					else
					{
						dPed.Task.WanderAround();
					}
					CurrentState = Enums.DStates.Normal;
					break;
				case Enums.DStates.Dealing:
					cTimer.Reset();
					cTimer.Start();
					dPed.Task.GuardCurrentPosition();
					CurrentState = Enums.DStates.Dealing;
					break;
				case Enums.DStates.StopForPlayer:
					if (dPed.IsWalking || dPed.IsRunning)
					{
						dPed.Task.Wait(20000);
					}
					CurrentState = Enums.DStates.StopForPlayer;
					break;
				case Enums.DStates.LookAtPlayer:
					dPed.Task.TurnTo((Entity)(object)Game.Player.Character);
					CurrentState = Enums.DStates.LookAtPlayer;
					break;
				case Enums.DStates.TalkToPlayer:
					Function.Call(unchecked((Hash)(-8213159594590722974L)), (dPed), ("GENERIC_HOWS_IT_GOING"), ("SPEECH_PARAMS_FORCE"));
					CurrentState = Enums.DStates.TalkToPlayer;
					break;
				case Enums.DStates.AttackPlayer:
					dPed.RelationshipGroup = LSL.rival_enemy;
					dPed.Task.ClearAll();
					LsFunctions.AddBlip((Entity)(object)dPed, (BlipColor)1, "Rival Dealer", _friend: false, _edge: false);
					dPed.CanSwitchWeapons = true;
					dPed.Weapons.Select(dPed.Weapons.BestWeapon);
					dPed.Task.FightAgainst(Game.Player.Character);
					CurrentState = Enums.DStates.AttackPlayer;
					break;
				case Enums.DStates.NoState:
					CurrentState = Enums.DStates.NoState;
					break;
				}
			}
			else if (CurrentState == Enums.DStates.Dealing)
			{
				if (!cTimer.Finished())
				{
					cTimer.Tick();
				}
				else
				{
					Deal();
				}
			}
		}
		else
		{
			UI.Notify("~r~Area Dealer Ped NULL something wrong");
		}
	}

	private bool ShouldAttackPlayer()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (LsFunctions.CanSee((Entity)(object)dPed, (Entity)(object)Game.Player.Character))
		{
			Vector3 position = ((Entity)dPed).Position;
			if (position.DistanceTo(LSL.playerPos) < 200f)
			{
				if (CurrentState == Enums.DStates.AttackPlayer)
				{
					return true;
				}
				if ((Entity)(object)Game.Player.Character.GetMeleeTarget() == (Entity)(object)dPed || Game.Player.IsTargetting((Entity)(object)dPed))
				{
					return true;
				}
				position = ((Entity)dPed).Position;
				if (position.DistanceTo(LSL.playerPos) < 50f && (dPed.IsInCombatAgainst(Game.Player.Character) || Game.Player.IsAiming))
				{
					return true;
				}
			}
			return false;
		}
		if (CurrentState == Enums.DStates.AttackPlayer && ((Entity)LSL.player.Character).IsAlive)
		{
			return true;
		}
		return false;
	}

	public void PromtPlayerToTalk()
	{
		if (LSL.LsMenuPool.IsAnyMenuOpen())
		{
			return;
		}
		bool canHire = true;
		string reason = "";
		if (canHire && LSL.DealerHandler.dealers.Count >= (int)LsFunctions.CurrentRepLvl(LSL.dealer1Rep))
		{
			canHire = false;
			reason = "You need more rep before hiring more dealers.";
		}
		if (canHire)
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 4000f)
			{
				Function.Call((Hash)7023634693725934496L);
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Press ~INPUT_SPRINT~ to Talk to this person.");
			}
			if (Game.IsControlJustPressed(2, (Control)21))
			{
				Function.Call((Hash)7023634693725934496L);
				((UIText)LSL.hireDealerMainMenu.Subtitle).Caption = "Test";
				LSL.hireDealerMainMenu.Visible = !LSL.hireDealerMainMenu.Visible;
				LSL.hireDealerMainMenu.CurrentSelection = 0;
				LSL.hireDealerOfferAmount.Maximum = Game.Player.Money;
			}
		}
		else if ((float)Game.GameTime > LSL.displayHelpTimer + 4000f)
		{
			Function.Call((Hash)7023634693725934496L);
			LSL.displayHelpTimer = Game.GameTime;
			LsFunctions.DisplayHelpText(reason);
		}
	}

	private bool playerNear()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Entity)dPed).Position;
		if ((position.DistanceTo(LSL.playerPos) < 4f && LsFunctions.CanSee((Entity)(object)dPed, (Entity)(object)Game.Player.Character)) || LsFunctions.CanHearPlayer(dPed))
		{
			return true;
		}
		return false;
	}

	public bool DecidePlayerOffer(int amount)
	{
		int max = 100 - amount / 1000;
		if (max < 1)
		{
			max = 1;
		}
		if (LsFunctions.RanRange(0, max + (int)LSL.areas[LSL.areaIndex].GangPresance) < 20)
		{
			return true;
		}
		return false;
	}

	public void SetLastOfferAmount(int _amount)
	{
		LastOffer = _amount;
	}

	private void Deal()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Entity)Game.Player.Character).Position;
		if (position.DistanceTo(((Entity)dPed).Position) < 300f)
		{
			if ((Entity)(object)cPed != (Entity)null && !((Entity)cPed).Exists())
			{
				cPed = null;
				if (DEBUG)
				{
					UI.Notify("aDealer Customer doesnt exist");
				}
			}
			if ((Entity)(object)cPed == (Entity)null)
			{
				if (DEBUG)
				{
					UI.Notify("aDealer Getting Customer");
				}
				List<Ped> peds = LSL.lPeds.GetPeds;
				if (peds.ToList().Count > 0)
				{
					foreach (Ped ped in peds)
					{
						if (!LSL.CheckIfPedUsed(ped, _allowPolice: false, _allowVehicle: false))
						{
							cPed = ped;
							((Entity)cPed).FreezePosition = false;
							((Entity)cPed).IsPersistent = true;
							LastCustomer = ((Entity)cPed).Handle;
							if (DEBUG)
							{
								((Entity)cPed).AddBlip();
							}
							break;
						}
					}
					return;
				}
				if (DEBUG)
				{
					UI.Notify("aDealer found no peds");
				}
				cTimer.Reset();
				return;
			}
			switch (dealState)
			{
			case Enums.DealState.GetNear:
				servePos = ((Entity)dPed).Position;
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(servePos) > 100f)
				{
					LsFunctions.RemovePedFromWorld(cPed, clearTask: false);
					cPed = null;
					cTimer.Reset();
					break;
				}
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(servePos) > 10f)
				{
					LsFunctions.MovePed(cPed, servePos);
					break;
				}
				dealState = Enums.DealState.Approach;
				cPed.Task.LookAt((Entity)(object)dPed);
				dPed.Task.TurnTo((Entity)(object)cPed);
				break;
			case Enums.DealState.Approach:
				servePos = ((Entity)dPed).Position;
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(servePos) > 2f)
				{
					LsFunctions.MovePed(cPed, servePos);
					break;
				}
				cPed.Task.TurnTo((Entity)(object)dPed);
				dPed.Task.TurnTo((Entity)(object)cPed);
				dealState = Enums.DealState.InPosition;
				time = Game.GameTime;
				break;
			case Enums.DealState.InPosition:
				if (Game.GameTime > time + 1000)
				{
					if (DEBUG)
					{
						UI.Notify("Doing Deal");
					}
					dealState = Enums.DealState.DoingDeal;
					cPed.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8f, 3000, (AnimationFlags)16);
					dPed.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8f, 3000, (AnimationFlags)16);
					time = Game.GameTime;
				}
				break;
			case Enums.DealState.DoingDeal:
				if (Game.GameTime > time + 2000)
				{
					dealState = Enums.DealState.WalkingAway;
					Function.Call(unchecked((Hash)(-8213159594590722974L)), (cPed), ("GENERIC_THANKS"), ("SPEECH_PARAMS_STANDARD"));
					cPed.Task.WanderAround();
					dPed.Task.GuardCurrentPosition();
				}
				break;
			case Enums.DealState.WalkingAway:
				servePos = ((Entity)dPed).Position;
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(servePos) > 4f)
				{
					dealState = Enums.DealState.GetNear;
					if (DEBUG)
					{
						UI.Notify("Deal Finished");
					}
					LsFunctions.RemovePedFromWorld(cPed, clearTask: false);
					((Entity)cPed).CurrentBlip.Remove();
					cPed = null;
					cTimer.Reset();
					cTimer.Start();
				}
				break;
			}
		}
		else if ((Entity)(object)cPed != (Entity)null)
		{
			if (DEBUG)
			{
				UI.Notify("aDealer Removing Customer to Far Away");
			}
			LsFunctions.RemovePedFromWorld(cPed, clearTask: false);
			cPed = null;
			if (dealState != Enums.DealState.GetNear)
			{
				dealState = Enums.DealState.GetNear;
			}
		}
	}
}
