using System;
using GTA;
using GTA.Math;
using LSlife.Inventory;

namespace LSlife;

internal class DefaultPed
{
	public enum PedType
	{
		Default,
		NewCustomer,
		OldCustomer
	}

	public PedType type;

	public Ped Ped;

	public Vector3 target;

	public bool working;

	private int areaIndex;

	private InventoryObject pInventory = new InventoryObject();

	public DefaultPed(PedType type, Ped ped, Vector3 target)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		this.type = type;
		Ped = ped ?? throw new ArgumentNullException("ped");
		this.target = target;
	}

	public void MovePed()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Ped.Task.FollowNavMeshTo(target);
		pInventory.AddItem(new Cocaine("Coke", _stack: true, ""), 420);
	}

	public void Wait()
	{
		Ped.Task.PlayAnimation("friends@", "pickupwait", 8f, -1, (AnimationFlags)1);
	}

	public void Tick()
	{
	}
}
