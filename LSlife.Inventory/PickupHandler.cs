using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;

namespace LSlife;

public class PickupHandler
{
	private List<DroppedBag> Bags = new List<DroppedBag>();

	public void OnSlowTick()
	{
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		if (Bags == null || Bags.Count <= 0)
		{
			return;
		}
		List<Ped> cops = (from r in World.GetAllPeds()
			where ((Entity)r).IsAlive && r.IsHuman && LsFunctions.IsPolice(r)
			select r).ToList();
		foreach (DroppedBag bag in Bags.ToList())
		{
			if (bag != null)
			{
				if (bag.Pig == null)
				{
					foreach (Ped cop in cops.Where((Ped r) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						Vector3 position2 = ((Entity)r).Position;
						return position2.DistanceTo(bag.pos) < 50f && !r.IsInCombat && !r.IsInMeleeCombat && !r.IsRagdoll;
					}))
					{
						Vector3 pPos = ((Entity)cop).Position;
						Vector3 pfPos = ((Entity)cop).ForwardVector * 3f + pPos;
						Vector3 bPos = bag.pos;
						float distToPos = bPos.DistanceTo(pPos);
						float distToFront = bPos.DistanceTo(pfPos);
						Pig pig = LsFunctions.GetPig(cop);
						if ((pig != null && pig.CurrentState == Pig.States.GetBag) || !(distToPos > distToFront))
						{
							continue;
						}
						RaycastResult raycast = World.Raycast(((Entity)cop).Position, bag.pos, (IntersectOptions)1);
						if (!raycast.DitHitAnything)
						{
							if (pig == null || pig.leader)
							{
								pig = new Pig(cop, _leader: false);
								pig.NewState = Pig.States.GetBag;
								pig.Bag = bag;
								bag.Pig = pig;
								LSL.pigs.Add(pig);
								break;
							}
							pig.NewState = Pig.States.GetBag;
						}
					}
				}
				else
				{
					Vector3 position = ((Entity)bag.Pig.Ped).Position;
					if (position.DistanceTo(bag.pos) < 1.5f)
					{
						bag.RemovePickup();
						RemoveBag(bag);
						continue;
					}
				}
				if (bag.CheckIfPickedUp())
				{
					bag.AddStuffToPlayer();
					bag.RemovePickup();
					RemoveBag(bag);
				}
				else if (bag.CanDespawn())
				{
					bag.RemovePickup();
					RemoveBag(bag);
					if (LSL.DEBUG)
					{
						LsFunctions.SendTextMsg("Un sac a disparu car il était resté trop longtemps au sol.");
					}
				}
			}
			else
			{
				LsFunctions.DbugString("Bag was null");
				RemoveBag(bag);
			}
		}
	}

	public void AddBag(DroppedBag _bag)
	{
		Bags.Add(_bag);
	}

	private void RemoveBag(DroppedBag _bag)
	{
		Bags.Remove(_bag);
	}

	public void RemoveBags()
	{
		if (Bags == null || Bags.Count <= 0)
		{
			return;
		}
		foreach (DroppedBag bag in Bags.ToList())
		{
			if (bag != null)
			{
				bag.RemovePickup();
				RemoveBag(bag);
			}
		}
	}
}
