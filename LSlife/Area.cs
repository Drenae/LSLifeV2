using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using LSlife.Gangs;

namespace LSlife;

public class Area
{
	public string Name;

	public List<Drug> Drugs = new List<Drug>();

	public LSL.AreaType AreaType = LSL.AreaType.Normal;

	public LSL.jurisdictionType PoliceType;

	public int CopPresance;

	public float GangPresance;

	public float Reputation;

	public float Heat;

	public Gang Gang;

	public bool DealerKilled;

	public int DemandBuffTime;

	public int DemandBuffLength;

	public Area(string _name, List<Drug> _drugs, LSL.AreaType _type)
	{
		Innit(_name, _drugs, _type);
	}

	public Area(string _name, List<Drug> _drugs, LSL.AreaType _type, int _cStr, int _gStr, int _heat, float _rep)
	{
		Innit(_name, _drugs, _type);
		CopPresance = _cStr;
		GangPresance = _gStr;
		Heat = _heat;
		Reputation = _rep;
	}

	private void Innit(string _name, List<Drug> _drugs, LSL.AreaType _type)
	{
		Name = _name;
		Drugs = _drugs;
		AreaType = _type;
		PoliceType = LSL.zoneData[Name].Item2;
		switch (AreaType)
		{
		case LSL.AreaType.Poor:
			CopPresance = 1;
			GangPresance = 6f;
			break;
		case LSL.AreaType.Normal:
			CopPresance = 1;
			GangPresance = 5f;
			break;
		case LSL.AreaType.Rich:
			CopPresance = 1;
			GangPresance = 5f;
			break;
		}
		foreach (Drug drug in Drugs)
		{
			switch (AreaType)
			{
			case LSL.AreaType.Poor:
				if (drug.AreaType == LSL.AreaType.Poor)
				{
					drug.Demand = 1f + AddRandomAmount();
				}
				if (drug.AreaType == LSL.AreaType.Normal)
				{
					drug.Demand = 0.8f + AddRandomAmount();
				}
				if (drug.AreaType == LSL.AreaType.Rich)
				{
					drug.Demand = 0.5f + AddRandomAmount();
				}
				drug.Delay = 10000f;
				break;
			case LSL.AreaType.Normal:
				if (drug.AreaType == LSL.AreaType.Poor)
				{
					drug.Demand = 0.7f + AddRandomAmount();
				}
				if (drug.AreaType == LSL.AreaType.Normal)
				{
					drug.Demand = 1f + AddRandomAmount();
				}
				if (drug.AreaType == LSL.AreaType.Rich)
				{
					drug.Demand = 0.7f + AddRandomAmount();
				}
				drug.Delay = 10000f;
				break;
			case LSL.AreaType.Rich:
				if (drug.AreaType == LSL.AreaType.Poor)
				{
					drug.Demand = 0.5f + AddRandomAmount();
				}
				if (drug.AreaType == LSL.AreaType.Normal)
				{
					drug.Demand = 0.8f + AddRandomAmount();
				}
				if (drug.AreaType == LSL.AreaType.Rich)
				{
					drug.Demand = 1f + AddRandomAmount();
				}
				drug.Delay = 10000f;
				break;
			}
		}
		List<Drug> SortedList = Drugs.OrderByDescending((Drug d) => d.Demand).ToList();
		Drugs = SortedList;
		string gName = GangUtilities.LsLifeGangs[LSL.rnd.Next(0, GangUtilities.LsLifeGangs.Count)];
		Gang = new Gang(gName, (int)GangPresance * 10000, 50, 20, Name);
	}

	public bool NeedCustomer()
	{
		bool getCustomer = false;
		foreach (Drug drug in Drugs)
		{
			if ((float)Game.GameTime > (float)drug.TimeOfLastCustomer + drug.Delay / (drug.Demand / (float)Dealers()))
			{
				drug.SelectCustomer = true;
				getCustomer = true;
			}
		}
		if (getCustomer)
		{
			return true;
		}
		return false;
	}

	internal int Dealers()
	{
		int dealers = 0;
		dealers += LSL.DealerHandler.dealers.Where((PlayerDealer d) => d.Working && d.area == Name).Count();
		if (Name == LSL.areas[LSL.areaIndex].Name)
		{
			if (LSL.isDealing)
			{
				dealers++;
			}
			if (LSL.aDealer != null)
			{
				dealers++;
			}
		}
		else if (GangPresance > 0f)
		{
			dealers++;
		}
		return dealers;
	}

	private float AddRandomAmount()
	{
		return Function.Call<float>((Hash)3547962977077129165L, InputArgument.op_Implicit(0f), InputArgument.op_Implicit(0.25f));
	}

	public void GetCustomer(List<Ped> peds)
	{
		if (Drugs.Where((Drug d) => d.SelectCustomer).Count() <= 0 || peds.Count <= 0)
		{
			return;
		}
		foreach (Drug drug in Drugs.Where((Drug d) => d.SelectCustomer))
		{
			Random rInt = new Random();
			int rIntMax = (int)(10f * drug.Demand);
			if (drug.Demand > 1f)
			{
				rIntMax = 10;
			}
			if (rInt.Next(0, 11 - rIntMax) == 0)
			{
				int pCount = peds.Count;
				int pIndex = 0;
				pIndex = rInt.Next(0, pCount);
				if (pCount == 1)
				{
					pIndex = 0;
				}
				drug.Customer = peds[pIndex];
				peds.Remove(drug.Customer);
				drug.TimeOfLastCustomer = Game.GameTime;
				if (peds.Count == 0)
				{
					break;
				}
			}
		}
	}

	public void HeatDecay()
	{
		if (Heat - 1f < 0f)
		{
			Heat = 0f;
		}
		else
		{
			Heat--;
		}
		if (CopPresance > 0)
		{
			CopPresance--;
		}
	}

	public void KillDealer()
	{
		DealerKilled = true;
		GangPresance--;
		DemandBuffTime = Game.GameTime;
		DemandBuffLength = 60000 * (10 - (int)GangPresance);
	}

	public bool CanSpawnDealer()
	{
		if (DealerKilled)
		{
			if (Game.GameTime > DemandBuff())
			{
				DealerKilled = false;
				return true;
			}
			return false;
		}
		return true;
	}

	public int DemandBuff()
	{
		return DemandBuffTime + DemandBuffLength;
	}

	internal bool DemandMet()
	{
		foreach (Drug drug in Drugs)
		{
			if (!drug.Supplied)
			{
				return false;
			}
		}
		return true;
	}
}
