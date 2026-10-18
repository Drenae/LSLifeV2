using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace LSlife;

public class Customer
{
	private Ped ped;

	private Vector3 target;

	private Vehicle vehicle;

	public List<string> drugs = new List<string>();

	public Drug Drug;

	public int amount;

	public bool FarAway;

	private Random rnd = new Random();

	public Vector3 Target
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return target;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			target = value;
		}
	}

	public Ped Ped
	{
		get
		{
			return ped;
		}
		set
		{
			ped = value;
		}
	}

	public Vehicle Vehicle
	{
		get
		{
			return vehicle;
		}
		set
		{
			vehicle = value;
		}
	}

	public Customer(Ped ped, Vector3 target, bool farAway, Vehicle vehicle, Drug drug, int _playerLevel)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		FarAway = farAway;
		Ped = ped;
		Target = target;
		Vehicle = vehicle;
		Drug = drug;
		((Entity)ped).IsPersistent = true;
		ped.Task.ClearAll();
		ped.BlockPermanentEvents = true;
		if (FarAway)
		{
			amount = LsFunctions.GetLargeStreetAmount();
		}
		else
		{
			int modifier = (int)(LsFunctions.CurrentRepLvl(LSL.areas[LSL.areaIndex].Reputation) * 0.5);
			int max = 7 + modifier;
			if (max <= 1)
			{
				max = 7;
			}
			amount = LsFunctions.RanRange(1, max);
		}
		int extraMoney = rnd.Next(0, 500);
		Ped.Money = amount * Drug.PricePerG + extraMoney;
		if (!FarAway && (Entity)(object)vehicle == (Entity)null)
		{
			Ped.Task.GoTo(target);
		}
		string blipName = "Livraison";
		if (farAway)
		{
			blipName = blipName + " " + Drug.Name + " " + LsFunctions.GramsToOz(amount);
		}
		LsFunctions.AddBlip((Entity)(object)ped, (BlipColor)3, blipName, _friend: true, _edge: true);
		((Entity)ped).CurrentBlip.Scale = 0.5f;
	}
}
