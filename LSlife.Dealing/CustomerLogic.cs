using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Control = GTA.Control;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.Gangs;
using LSlife.HeatBars;
using LSlife.OldCustomers;
using LSlife.Shots;

namespace LSlife;

public partial class LSL
{
	private static void HandleOldCutomers()
	{
		OldCustomerHandler.OnTick();
	}

	private Customer FindClosestCustomer()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float nDistance = 100f;
		Customer cCustomer = null;
		foreach (Customer c in customers.Where((Customer r) => r.Ped.PedGroup == playerGroup))
		{
			Vector3 position = ((Entity)c.Ped).Position;
			float cDistance = position.DistanceTo(playerPos + ((Entity)player.Character).ForwardVector);
			if (cDistance < 3f && cDistance < nDistance)
			{
				nDistance = cDistance;
				cCustomer = c;
			}
		}
		return cCustomer;
	}

	private Customer FindClosestCustomerInVehicle()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float nDistance = 100f;
		Customer cCustomer = null;
		foreach (Customer c in customers.Where((Customer r) => r.Ped.PedGroup == playerGroup && (Entity)(object)r.Ped.CurrentVehicle == (Entity)(object)player.Character.CurrentVehicle))
		{
			Vector3 position = ((Entity)c.Ped).Position;
			float cDistance = position.DistanceTo(playerPos);
			if (cDistance < nDistance)
			{
				nDistance = cDistance;
				cCustomer = c;
			}
		}
		return cCustomer;
	}

	public static bool CheckIfPedUsed(Ped _ped, bool _allowPolice, bool _allowVehicle)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		Model model = ((Entity)_ped).Model;
		if (model.Hash != 1885233650)
		{
			model = ((Entity)_ped).Model;
			if (model.Hash != -1667301416)
			{
				if (!((Entity)_ped).Exists())
				{
					return true;
				}
				if (_ped.IsPlayer)
				{
					return true;
				}
				if (!_allowVehicle && _ped.IsInVehicle())
				{
					return true;
				}
				if (GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_ENTITY_ALPHA, _ped.Handle) == 0)
				{
					return true;
				}
				if (!_ped.IsHuman)
				{
					return true;
				}
				if (((Entity)_ped).IsDead)
				{
					return true;
				}
				if (!((Entity)_ped).IsAlive)
				{
					return true;
				}
				if (LsFunctions.IsInInterior((Entity)(object)_ped))
				{
					return true;
				}
				if (LsFunctions.IsUnderGround((Entity)(object)_ped))
				{
					return true;
				}
				if (((Entity)_ped).IsInAir)
				{
					return true;
				}
				if (!_ped.IsInVehicle() && ((Entity)_ped).Position.Z > (World.GetGroundHeight(((Entity)_ped).Position + ((Entity)_ped).ForwardVector * 2f, out float customerGroundHeight) ? customerGroundHeight : 0f) + 2f)
				{
					return true;
				}
				if (player.Character.IsInVehicle() && _ped.IsInVehicle() && (Entity)(object)_ped.CurrentVehicle == (Entity)(object)player.Character.CurrentVehicle)
				{
					return true;
				}
				if (Game.Player.Character.PedGroup == _ped.PedGroup)
				{
					return true;
				}
				if (_ped.RelationshipGroup == player.Character.RelationshipGroup)
				{
					return true;
				}
				if ((Entity)(object)drugDealer1 != (Entity)null && (Entity)(object)drugDealer1 == (Entity)(object)_ped)
				{
					return true;
				}
				if ((Entity)(object)weaponDealer1 != (Entity)null && (Entity)(object)weaponDealer1 == (Entity)(object)_ped)
				{
					return true;
				}
				if (wepDealer != null && (Entity)(object)wepDealer.ped == (Entity)(object)_ped)
				{
					return true;
				}
				if (customers.Count > 0 && customers.FindAll((Customer r) => (Entity)(object)r.Ped == (Entity)(object)_ped).Count != 0)
				{
					return true;
				}
				if (OldCustomerHandler.IsPedOldCustomer(_ped))
				{
					return true;
				}
				if (ShotHandler.shots.Count > 0 && ShotHandler.shots.Where((Shot s) => (Entity)(object)s.customer.Ped == (Entity)(object)_ped).Count() != 0)
				{
					return true;
				}
				if (angryPeds.Count > 0 && angryPeds.FindAll((Ped r) => (Entity)(object)r == (Entity)(object)_ped).Count != 0)
				{
					return true;
				}
				if (pigs.Count > 0 && pigs.FindAll((Pig r) => (Entity)(object)r.Ped == (Entity)(object)_ped).Count != 0)
				{
					return true;
				}
				if (aDealer != null)
				{
					if ((Entity)(object)aDealer.dPed != (Entity)null && (Entity)(object)aDealer.dPed == (Entity)(object)_ped)
					{
						return true;
					}
					if ((Entity)(object)aDealer.cPed != (Entity)null && (Entity)(object)aDealer.cPed == (Entity)(object)_ped)
					{
						return true;
					}
					if (aDealer.LastCustomer == ((Entity)_ped).Handle)
					{
						return true;
					}
				}
				if (ZeesDealers != null && ((Entity)(object)ZeesDealers.lieut == (Entity)(object)_ped || (Entity)(object)ZeesDealers.goon == (Entity)(object)_ped))
				{
					return true;
				}
				if ((Entity)(object)attacker != (Entity)null && (Entity)(object)_ped == (Entity)(object)attacker)
				{
					return true;
				}
				if (DealerHandler.dealers.Count > 0)
				{
					foreach (PlayerDealer d in DealerHandler.dealers)
					{
						if (d != null)
						{
							if ((Entity)(object)d.Ped != (Entity)null && (Entity)(object)d.Ped == (Entity)(object)_ped)
							{
								return true;
							}
							if ((Entity)(object)d.cPed != (Entity)null && (Entity)(object)d.cPed == (Entity)(object)_ped)
							{
								return true;
							}
							if ((Entity)(object)d.lPed == (Entity)(object)_ped)
							{
								return true;
							}
						}
					}
				}
				if (driveByHandler.GetDriveBies.Count > 0)
				{
					foreach (DriveBy getDriveBy in driveByHandler.GetDriveBies)
					{
						if (getDriveBy.Peds.Contains(_ped))
						{
							return true;
						}
					}
				}
				if (!Function.Call<bool>((Hash)5176593366545292405L, (_ped)))
				{
					return true;
				}
				int type = Function.Call<int>(unchecked((Hash)(-70476366192974276L)), (((Entity)_ped).Handle));
				if (type == 24 || type == 26)
				{
					return true;
				}
				if (!_allowPolice && LsFunctions.IsPolice(_ped))
				{
					return true;
				}
				return false;
			}
		}
		return true;
	}

	public static void MoveCustomer(Customer c, Vector3 target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (c.Target != target)
		{
			c.Target = target;
		}
		if (c.Target == target)
		{
			Vector3 position = ((Entity)c.Ped).Position;
			if (position.DistanceTo(c.Target) > 20f || Function.Call<bool>((Hash)1322919935073282825L, (((Entity)c.Ped).Position.X), (((Entity)c.Ped).Position.Y), (((Entity)c.Ped).Position.Z)))
			{
				c.Ped.Task.RunTo(c.Target);
			}
			else
			{
				c.Ped.Task.FollowNavMeshTo(c.Target);
			}
		}
	}
}
