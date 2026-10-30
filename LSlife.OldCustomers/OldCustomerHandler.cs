using System.Collections.Generic;
using System.Linq;
using GTA;

namespace LSlife.OldCustomers;

internal static class OldCustomerHandler
{
	private static List<OldCustomer> OldCustomers = new List<OldCustomer>();

	public static void OnTick()
	{
		foreach (OldCustomer o in OldCustomers.ToList())
		{
			if (o == null || !((Entity)o.Ped).Exists())
			{
				OldCustomers.Remove(o);
			}
			else
			{
				o.OnTick();
			}
		}
	}

	internal static void RemoveIfPedOldCustomer(Ped ped)
	{
		if (OldCustomers.Count > 0)
		{
			List<OldCustomer> oc = OldCustomers.Where((OldCustomer c) => (Entity)(object)c.Ped == (Entity)(object)ped).ToList();
			if (oc.Count > 0)
			{
				LsFunctions.DbugString("Old customer removed as they are a worker");
				oc.First().Remove();
			}
		}
	}

	public static void AddCustomer(Ped _ped, Vehicle _veh, bool _inside, bool _canBeRival, KeyValuePair<string, int> _drug)
	{
		if (!_ped.IsPlayer)
		{
			if (_ped.PedGroup == LSL.player.Character.PedGroup)
			{
				_ped.LeaveGroup();
			}
			if (((Entity)_ped).AttachedBlip != (Blip)null)
			{
				((Entity)_ped).AttachedBlip.Delete();
			}
			OldCustomer oC = new OldCustomer(_ped, _veh, _inside, _canBeRival, _drug);
			OldCustomers.Add(oC);
		}
	}

	internal static int AmountOf()
	{
		return OldCustomers.Count;
	}

	public static void RemoveCustomer(OldCustomer o)
	{
		OldCustomers.Remove(o);
	}

	internal static void ClearOldCustomers()
	{
		foreach (OldCustomer item in OldCustomers.ToList())
		{
			item.Destroy();
			item.Remove();
		}
	}

	public static bool IsPedOldCustomer(Ped _ped)
	{
		if (OldCustomers.Count > 0 && OldCustomers.Where((OldCustomer c) => (Entity)(object)c.Ped == (Entity)(object)_ped).Count() > 0)
		{
			return true;
		}
		return false;
	}
}
