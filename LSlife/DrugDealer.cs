using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class DrugDealer
{
	private enum DrugDealSates
	{
		SpawnStuff,
		ConfigStuff,
		WaitForPlayer,
		GetIntoPosition,
		GetReadyToDeal,
		Destroy,
		ReadyToDeal,
		Payed,
		PickedUp,
		Dismiss
	}

	public Vehicle dealerCar;

	public Ped lieut;

	public Ped goon;

	private Blip blip;

	private DrugDealSates DealState;

	public bool EnterState;

	private bool Ready;

	private DrugOrder PlayerOrder;

	private DealerSpawnPostion dealerSpawn;

	private Vector3 backPos = new Vector3(0f, 0f, 0f);

	private int time;

	private PedGroup peds = new PedGroup();

	public bool Kill { get; private set; }

	public DrugDealer(DrugOrder _order)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected Obj, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		dealerSpawn = GenerateDealerSpawn();
		PlayerOrder = _order;
		if (dealerSpawn != null)
		{
			if (_order != null)
			{
				if (blip == (Blip)null)
				{
					blip = World.CreateBlip(dealerSpawn.Position);
					if (blip != (Blip)null)
					{
						blip.Sprite = (BlipSprite)478;
						blip.Name = "Drug Pickup";
						blip.ShowRoute = true;
						LsFunctions.DbugString("dealer car spawned");
						string areaName = World.GetZoneName(dealerSpawn.Position);
						LsFunctions.TextMsg("ZEE", "Order", "Ok go meet my people in ~y~" + areaName + " ~s~, they will sort you out.");
					}
					else
					{
						LsFunctions.SendTextMsg("LsLife: ~r~Failed to create blip");
						Destroy();
					}
				}
				string text = dealerSpawn.Heading.ToString();
				Vector3 position = dealerSpawn.Position;
				LsFunctions.DbugString("dealer spawn found,~n~heading " + text + ",~n~pos " + ((object)position/*cast due to constrained. prefix*/).ToString());
			}
			else
			{
				LsFunctions.SendTextMsg("LsLife: Error Order is null");
				Destroy();
			}
		}
		else
		{
			LsFunctions.TextMsg("ZEE", "Order", "We havent got anyone around there.");
			DealState = DrugDealSates.Destroy;
		}
	}

	public void OnTick()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		switch (DealState)
		{
		case DrugDealSates.ReadyToDeal:
			if (Ready && Game.IsControlJustReleased(2, (Control)51) && PlayerOrder.PayBill())
			{
				if (LSL.player.Money > 0)
				{
					LsFunctions.HandOverGoods(LSL.player.Character, lieut);
				}
				DealState = DrugDealSates.Payed;
				EnterState = true;
				Ready = false;
				Function.Call((Hash)(-8213159594590722974L), InputArgument.op_Implicit(lieut), InputArgument.op_Implicit("GENERIC_THANKS"), InputArgument.op_Implicit("SPEECH_PARAMS_STANDARD"), InputArgument.op_Implicit(0));
			}
			break;
		case DrugDealSates.Payed:
			World.DrawMarker((MarkerType)0, backPos, Vector3.Zero, Vector3.Zero, new Vector3(1f, 1f, 0.5f), Color.Green);
			if (Ready && Game.IsControlJustReleased(2, (Control)51))
			{
				PlayerOrder.PickupDrugs();
				if (PlayerOrder.pickedUp)
				{
					LSL.NewOrder.ResetOrder();
					DealState = DrugDealSates.PickedUp;
					EnterState = true;
					Ready = false;
				}
			}
			break;
		}
	}

	private static void UpdateZeeDealerMenu()
	{
	}

	public void OnSlowTick()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position5;
		switch (DealState)
		{
		case DrugDealSates.SpawnStuff:
			if (!(LSL.playerPos.DistanceTo(dealerSpawn.Position) < 200f))
			{
				break;
			}
			if ((Entity)(object)dealerCar == (Entity)null)
			{
				dealerCar = World.CreateVehicle(LsFunctions.RequestModel((VehicleHash)1177543287), dealerSpawn.Position, dealerSpawn.Heading);
				while ((Entity)(object)dealerCar == (Entity)null || !((Entity)dealerCar).Exists())
				{
					Script.Yield();
				}
				((Entity)dealerCar).IsPersistent = true;
				dealerCar.LockStatus = (VehicleLockStatus)3;
				LsFunctions.DbugString("Dealer Car Spawned");
				break;
			}
			if ((Entity)(object)goon == (Entity)null && (Entity)(object)dealerCar != (Entity)null)
			{
				SpawnPed((VehicleSeat)(-1), _leader: false);
				if ((Entity)(object)dealerCar.Driver != (Entity)null)
				{
					goon = dealerCar.Driver;
				}
			}
			if ((Entity)(object)lieut == (Entity)null && (Entity)(object)dealerCar != (Entity)null)
			{
				SpawnPed((VehicleSeat)0, _leader: true);
				if ((Entity)(object)dealerCar.GetPedOnSeat((VehicleSeat)0) != (Entity)null)
				{
					lieut = dealerCar.GetPedOnSeat((VehicleSeat)0);
				}
			}
			if ((Entity)(object)lieut != (Entity)null && ((Entity)lieut).Exists() && (Entity)(object)goon != (Entity)null && ((Entity)goon).Exists())
			{
				EnterState = true;
				DealState = DrugDealSates.ConfigStuff;
			}
			break;
		case DrugDealSates.ConfigStuff:
			if (EnterState)
			{
				EnterState = false;
				MakeGroup();
			}
			else
			{
				EnterState = true;
				DealState = DrugDealSates.WaitForPlayer;
			}
			break;
		case DrugDealSates.WaitForPlayer:
			if (EnterState)
			{
				EnterState = false;
			}
			else if (LSL.playerPos.DistanceTo(((Entity)lieut).Position) < 20f && LsFunctions.CanSee((Entity)(object)LSL.player.Character, (Entity)(object)lieut))
			{
				EnterState = true;
				DealState = DrugDealSates.GetIntoPosition;
			}
			break;
		case DrugDealSates.GetIntoPosition:
		{
			Model model;
			if (EnterState)
			{
				EnterState = false;
				Vector3 position = ((Entity)dealerCar).Position;
				Vector3 val = ((Entity)dealerCar).ForwardVector * -1f;
				model = ((Entity)dealerCar).Model;
				backPos = Vector3.Add(position, Vector3.Multiply(val, model.GetDimensions().Y / 2f + 0.5f));
				dealerCar.OpenDoor((VehicleDoor)5, false, false);
				Tasks task = lieut.Task;
				Vector3 position2 = ((Entity)dealerCar).Position;
				Vector3 val2 = ((Entity)dealerCar).ForwardVector * -1f;
				model = ((Entity)dealerCar).Model;
				task.LookAt(Vector3.Add(position2, Vector3.Multiply(val2, model.GetDimensions().Y / 2f + 0.5f)));
				Tasks task2 = lieut.Task;
				Vector3 position3 = ((Entity)dealerCar).Position;
				Vector3 val3 = ((Entity)dealerCar).ForwardVector * -1f;
				model = ((Entity)dealerCar).Model;
				task2.GoTo(Vector3.Add(position3, Vector3.Multiply(val3, model.GetDimensions().Y / 2f + 0.5f)) + ((Entity)dealerCar).RightVector * 1f);
				Tasks task3 = goon.Task;
				Vector3 position4 = ((Entity)dealerCar).Position;
				Vector3 val4 = ((Entity)dealerCar).ForwardVector * -1f;
				model = ((Entity)dealerCar).Model;
				task3.GoTo(Vector3.Add(position4, Vector3.Multiply(val4, model.GetDimensions().Y / 2f + 0.5f)) + ((Entity)dealerCar).RightVector * -2f);
			}
			else
			{
				position5 = ((Entity)lieut).Position;
				Vector3 position6 = ((Entity)dealerCar).Position;
				Vector3 val5 = ((Entity)dealerCar).ForwardVector * -1f;
				model = ((Entity)dealerCar).Model;
				if (position5.DistanceTo(Vector3.Add(position6, Vector3.Multiply(val5, model.GetDimensions().Y / 2f + 0.5f)) + ((Entity)dealerCar).RightVector * 1f) < 1f)
				{
					EnterState = true;
					DealState = DrugDealSates.GetReadyToDeal;
				}
			}
			break;
		}
		case DrugDealSates.GetReadyToDeal:
			if (EnterState)
			{
				EnterState = false;
			}
			else if (dealerCar.IsDoorOpen((VehicleDoor)5))
			{
				lieut.Task.LookAt((Entity)(object)LSL.player.Character);
				goon.Task.LookAt((Entity)(object)LSL.player.Character);
				lieut.Task.TurnTo((Entity)(object)LSL.player.Character);
				goon.Task.TurnTo((Entity)(object)LSL.player.Character);
				goon.Weapons.Select(goon.Weapons.BestWeapon);
				EnterState = true;
				DealState = DrugDealSates.ReadyToDeal;
			}
			break;
		case DrugDealSates.ReadyToDeal:
			if (EnterState)
			{
				EnterState = false;
			}
			if (LSL.player.Character.IsStopped)
			{
				position5 = ((Entity)LSL.player.Character).Position;
				if (position5.DistanceTo(((Entity)lieut).Position) < 3f)
				{
					PayBillPromt();
					Ready = true;
					break;
				}
			}
			Ready = false;
			break;
		case DrugDealSates.Payed:
			if (EnterState)
			{
				EnterState = false;
			}
			if (LSL.player.Character.IsStopped && LSL.playerPos.DistanceTo(backPos) < 1.5f)
			{
				PickupPromt();
				Ready = true;
			}
			else
			{
				Ready = false;
			}
			break;
		case DrugDealSates.PickedUp:
			if (EnterState)
			{
				Function.Call((Hash)(-8213159594590722974L), InputArgument.op_Implicit(lieut), InputArgument.op_Implicit("GENERIC_BYE"), InputArgument.op_Implicit("SPEECH_PARAMS_STANDARD"), InputArgument.op_Implicit(0));
				EnterState = false;
			}
			else
			{
				DealState = DrugDealSates.Dismiss;
				EnterState = true;
			}
			break;
		case DrugDealSates.Dismiss:
		{
			bool doesExists = (Entity)(object)lieut != (Entity)null && (Entity)(object)goon != (Entity)null && (Entity)(object)dealerCar != (Entity)null;
			if (EnteredState())
			{
				time = Game.GameTime;
				if (doesExists)
				{
					lieut.Task.EnterVehicle(dealerCar, (VehicleSeat)0);
					goon.Task.EnterVehicle(dealerCar, (VehicleSeat)(-1));
				}
			}
			else if (doesExists)
			{
				if ((goon.IsInVehicle() && lieut.IsInVehicle()) || Game.GameTime > time + 10000)
				{
					dealerCar.CloseDoor((VehicleDoor)5, false);
					SetDealState(DrugDealSates.Destroy);
					if (PlayerOrder != null)
					{
						PlayerOrder = null;
					}
				}
			}
			else
			{
				SetDealState(DrugDealSates.Destroy);
				if (PlayerOrder != null)
				{
					PlayerOrder = null;
				}
			}
			break;
		}
		case DrugDealSates.Destroy:
			if (EnteredState())
			{
				if ((Entity)(object)lieut != (Entity)null && ((Entity)lieut).Exists())
				{
					LsFunctions.RemovePedFromWorld(lieut, clearTask: true);
				}
				if ((Entity)(object)goon != (Entity)null && ((Entity)lieut).Exists())
				{
					LsFunctions.RemovePedFromWorld(goon, clearTask: true);
				}
				if ((Entity)(object)dealerCar != (Entity)null && ((Entity)dealerCar).Exists())
				{
					RemoveVehicle();
				}
				if (blip != (Blip)null && blip.Exists())
				{
					RemoveBlip();
				}
				peds.Dispose();
			}
			else
			{
				Kill = true;
			}
			break;
		}
	}

	private void SetDealState(DrugDealSates _state)
	{
		EnterState = true;
		DealState = _state;
	}

	private bool EnteredState()
	{
		if (EnterState)
		{
			EnterState = false;
			return true;
		}
		return false;
	}

	private void PickupPromt()
	{
		if (!LSL.LsMenuPool.IsAnyMenuOpen() && (float)Game.GameTime > LSL.displayHelpTimer + 6000f)
		{
			LSL.displayHelpTimer = Game.GameTime;
			LsFunctions.DisplayHelpText("Press ~INPUT_CONTEXT~ to pickup the Drugs");
		}
	}

	public void Dismiss()
	{
		EnterState = true;
		DealState = DrugDealSates.Dismiss;
	}

	private void PayBillPromt()
	{
		if (!LSL.LsMenuPool.IsAnyMenuOpen() && (float)Game.GameTime > LSL.displayHelpTimer + 6000f)
		{
			LSL.displayHelpTimer = Game.GameTime;
			if (PlayerOrder.CanPayBill())
			{
				LsFunctions.DisplayHelpText("Press ~INPUT_CONTEXT~ to do the deal");
			}
			else
			{
				LsFunctions.DisplayHelpText("You do not have enough credit or cash to cover the price.");
			}
		}
	}

	private void RemoveBlip()
	{
		blip.Alpha = 0;
		blip.Remove();
	}

	private void RemoveVehicle()
	{
		((Entity)dealerCar).MarkAsNoLongerNeeded();
	}

	private void SpawnPed(VehicleSeat _seat, bool _leader)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		PedHash pedHash = (PedHash)(-412008429);
		if (!_leader)
		{
			pedHash = (PedHash)(-984709238);
		}
		Ped p = dealerCar.CreatePedOnSeat(_seat, LsFunctions.RequestModel(pedHash));
		if ((Entity)(object)p != (Entity)null && ((Entity)p).Exists())
		{
			((Entity)p).IsPersistent = true;
			p.RelationshipGroup = LSL.playerRelationship;
			LsFunctions.SetRandomCombatStats(p);
			p.FiringPattern = (FiringPattern)(-753768974);
			p.AlwaysKeepTask = true;
			p.CanSwitchWeapons = true;
			p.BlockPermanentEvents = true;
			Function.Call((Hash)(-4079649877180351064L), InputArgument.op_Implicit(p), InputArgument.op_Implicit(100));
			switch (LsFunctions.RanRange(0, (int)(1f + LSL.areas[LSL.areaIndex].GangPresance * 0.5f)))
			{
			case 5:
				p.Weapons.Give((WeaponHash)(-1074790547), 500, false, true);
				p.Weapons.Give((WeaponHash)324215364, 500, true, true);
				break;
			case 4:
				p.Weapons.Give((WeaponHash)1649403952, 500, false, true);
				p.Weapons.Give((WeaponHash)(-1121678507), 500, true, true);
				break;
			case 3:
				p.Weapons.Give((WeaponHash)1649403952, 500, false, true);
				p.Weapons.Give((WeaponHash)(-619010992), 500, true, true);
				break;
			case 2:
				p.Weapons.Give((WeaponHash)487013001, 500, false, true);
				p.Weapons.Give((WeaponHash)(-619010992), 500, true, true);
				break;
			case 1:
				p.Weapons.Give((WeaponHash)2017895192, 500, false, true);
				p.Weapons.Give((WeaponHash)453432689, 500, true, true);
				break;
			case 0:
				p.Weapons.Give((WeaponHash)453432689, 500, true, true);
				break;
			}
		}
	}

	private DealerSpawnPostion GenerateDealerSpawn()
	{
		List<DealerSpawnPostion> spawnPostionsTmp = LsFunctions.spawnPostions.Where((DealerSpawnPostion s) =>
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			return s.Position.DistanceTo(LSL.playerPos) < 1800f && (double)s.Position.DistanceTo(LSL.playerPos) > 400.0 - LsFunctions.CurrentRepLvl(LSL.dealer1Rep);
		}).ToList();
		if (spawnPostionsTmp.Count > 0)
		{
			int rndInt = LSL.rnd.Next(0, spawnPostionsTmp.Count);
			return spawnPostionsTmp[rndInt];
		}
		return null;
	}

	private void MakeGroup()
	{
		if ((Entity)(object)lieut != (Entity)null && ((Entity)lieut).Exists())
		{
			peds.Add(lieut, true);
			lieut.NeverLeavesGroup = true;
		}
		if ((Entity)(object)goon != (Entity)null && ((Entity)goon).Exists())
		{
			peds.Add(goon, false);
			goon.NeverLeavesGroup = true;
		}
	}

	public void Destroy()
	{
		EnterState = true;
		DealState = DrugDealSates.Destroy;
	}
}
