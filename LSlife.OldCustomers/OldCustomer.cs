using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife.OldCustomers;

internal class OldCustomer
{
	private enum OldCustomerStates
	{
		None,
		WalkAway,
		Attack,
		Destroy,
		Dead
	}

	internal Ped Ped;

	internal Vehicle Vehicle;

	internal bool Inside;

	internal bool CanBeRival;

	internal KeyValuePair<string, int> Drug;

	internal bool IsRival;

	private OldCustomerStates currentState;

	private bool enteredState;

	public OldCustomer(Ped _ped, Vehicle _veh, bool _inside, bool _canBeRival, KeyValuePair<string, int> _drug)
	{
		Ped = _ped;
		Vehicle = _veh;
		Inside = _inside;
		CanBeRival = _canBeRival;
		Drug = _drug;
		Ped.CanBeTargetted = true;
		Ped.CanBeShotInVehicle = true;
	}

	internal void Destroy()
	{
		LsFunctions.DbugString("Old customer Destroyed");
		LsFunctions.RemovePedFromWorld(Ped, clearTask: false);
		if (Inside && (Entity)(object)Ped != (Entity)null)
		{
			((Entity)Ped).Delete();
		}
	}

	internal void Remove()
	{
		OldCustomerHandler.RemoveCustomer(this);
	}

	public void OnTick()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		CheckPed();
		if (currentState != OldCustomerStates.Destroy && FarAway())
		{
			SetState(OldCustomerStates.Destroy);
		}
		switch (currentState)
		{
		case OldCustomerStates.None:
			SetState(OldCustomerStates.WalkAway);
			break;
		case OldCustomerStates.WalkAway:
			if (EnteredState())
			{
				WalkAway();
			}
			else
			{
				if (!WalkedAway())
				{
					break;
				}
				if (CanBeRival)
				{
					Model model;
					if (!IsRival && ShouldAttack())
					{
						IsRival = true;
						SetState(OldCustomerStates.Attack);
						model = ((Entity)Ped).Model;
						LsFunctions.DbugString(((object)(PedHash)model.Hash/*cast due to constrained. prefix*/).ToString() + "is rival");
						break;
					}
					CanBeRival = false;
					model = ((Entity)Ped).Model;
					LsFunctions.DbugString(((object)(PedHash)model.Hash/*cast due to constrained. prefix*/).ToString() + "not rival");
				}
				SetState(OldCustomerStates.Destroy);
			}
			break;
		case OldCustomerStates.Attack:
			if (EnteredState())
			{
				AttackTarget(LSL.player.Character);
			}
			break;
		case OldCustomerStates.Dead:
			if (EnteredState() && IsRival)
			{
				LSL.areas[LSL.areaIndex].GangPresance--;
				if (LSL.areas[LSL.areaIndex].GangPresance < 0f)
				{
					LSL.areas[LSL.areaIndex].GangPresance = 0f;
				}
				if (GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, Ped.Handle)) is Ped)
				{
					Entity killer = GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, Ped.Handle));
					Ped killer2 = (Ped)(object)((killer is Ped) ? killer : null);
					if ((Entity)(object)killer2 != (Entity)null && killer2.PedGroup == LSL.player.Character.PedGroup && ShouldAttack())
					{
						LsFunctions.SpawnDriveBy(LSL.player.Character, _agressive: true);
					}
				}
			}
			SetState(OldCustomerStates.Destroy);
			break;
		case OldCustomerStates.Destroy:
			if (EnteredState())
			{
				Destroy();
			}
			break;
		}
	}

	private bool FarAway()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((Entity)(object)Ped != (Entity)null)
		{
			Vector3 position = ((Entity)Ped).Position;
			return position.DistanceTo(LSL.playerPos) > 20f;
		}
		return false;
	}

	private static bool ShouldAttack()
	{
		if (LSL.areas[LSL.areaIndex].GangPresance > 0f)
		{
			return LSL.rnd.Next(0, 31 - (int)LSL.areas[LSL.areaIndex].GangPresance) == 0;
		}
		return false;
	}

	private void AttackTarget(Ped _ped)
	{
		LsFunctions.SetRandomCombatStats(Ped);
		LsFunctions.SetRivalWeapons(Ped);
		Ped.Weapons.Select(Ped.Weapons.BestWeapon);
		Ped.CanSwitchWeapons = true;
		Ped.IsEnemy = true;
		Ped.RelationshipGroup = LSL.rival_enemy;
		Ped.Task.Combat(_ped);
	}

	private bool EnteredState()
	{
		if (enteredState)
		{
			enteredState = false;
			return true;
		}
		return false;
	}

	private void CheckPed()
	{
		if (currentState != OldCustomerStates.Destroy && currentState != OldCustomerStates.Dead && ((Entity)Ped).IsDead)
		{
			SetState(OldCustomerStates.Dead);
		}
	}

	private bool WalkedAway()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position;
		if (Inside && LSL.HouseHandler.CurrentHouse != null)
		{
			position = ((Entity)Ped).Position;
			if (position.DistanceTo(LSL.HouseHandler.CurrentHouse.exitPos) < 1f)
			{
				return true;
			}
		}
		else
		{
			position = ((Entity)Ped).Position;
			if (position.DistanceTo(LSL.playerPos) > 6f)
			{
				return true;
			}
		}
		return false;
	}

	private void WalkAway()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (Inside && LSL.HouseHandler.CurrentHouse != null)
		{
			Ped.Task.FollowNavMeshTo(LSL.HouseHandler.CurrentHouse.exitPos);
			return;
		}
		if ((Entity)(object)Vehicle != (Entity)null && ((Entity)Vehicle).Exists())
		{
			LSL.PerformTaskSeq(1, Vehicle, ((Entity)Vehicle).Position);
		}
		Ped.Task.Wander();
	}

	private void SetState(OldCustomerStates _state)
	{
		currentState = _state;
		enteredState = true;
	}
}
