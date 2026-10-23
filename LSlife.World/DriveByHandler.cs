using System.Collections.Generic;
using System.Linq;
using GTA;

namespace LSlife;

public class DriveByHandler
{
	public List<DriveBy> GetDriveBies = new List<DriveBy>();

	public void OnTick()
	{
		if (GetDriveBies.Count <= 0)
		{
			return;
		}
		foreach (DriveBy driveBy in GetDriveBies.ToList())
		{
			if (driveBy.Dead)
			{
				GetDriveBies.Remove(driveBy);
			}
			else
			{
				driveBy.OnTick();
			}
		}
	}

	public void AddDriveBy(VehicleClass _class, int _amount, Ped _target, bool _agres)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		GetDriveBies.Add(new DriveBy(_class, _amount, _target, _agres));
	}

	public int AmountOfDriveBy(bool _agg)
	{
		if (_agg)
		{
			return GetDriveBies.Where((DriveBy r) => r.Agressive).Count();
		}
		return GetDriveBies.Count;
	}

	public List<Ped> GetAllPeds()
	{
		Ped[] p = new Ped[0];
		if (GetDriveBies.Count > 0)
		{
			foreach (DriveBy getDriveBy in GetDriveBies)
			{
				foreach (Ped ped in getDriveBy.Peds.Where((Ped r) => (Entity)(object)r != (Entity)null && ((Entity)r).Exists()))
				{
					p.Append(ped);
				}
			}
		}
		if (p.Count() > 0)
		{
			return p.ToList();
		}
		return null;
	}

	public void CleanDriveBys()
	{
		foreach (DriveBy d in GetDriveBies.ToList())
		{
			d.CleanDriveBy();
			GetDriveBies.Remove(d);
		}
	}

	internal void SetAgressiveTarget(bool _agg, Ped _ped)
	{
		foreach (DriveBy d in GetDriveBies)
		{
			if (!d.Agressive)
			{
				d.Agressive = true;
				d.ChangeBlip(_agg: true);
				d.SetNewTarget(_ped);
			}
		}
	}
}
