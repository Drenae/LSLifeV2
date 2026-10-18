using GTA;
using GTA.Math;

namespace LSlife;

internal class CustomerPed : DefaultPed
{
	private enum CustomerStates
	{
		None,
		Waiting,
		WalkTo,
		Buying,
		WalktAway
	}

	private Vehicle Vehicle;

	public Drug Drug;

	public int amount;

	private bool walkToPlayer;

	private CustomerStates cState;

	public CustomerPed(PedType type, Ped ped, Vector3 target, bool working, int areaIndex, Vehicle _vehicle, Drug _drug, int _drugAmount, bool _walk)
		: base(type, ped, target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Vehicle = _vehicle;
		Drug = _drug;
		amount = _drugAmount;
		walkToPlayer = _walk;
	}

	public new void Tick()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position;
		switch (cState)
		{
		case CustomerStates.None:
			if (walkToPlayer)
			{
				MovePed();
				cState = CustomerStates.WalkTo;
			}
			else
			{
				Wait();
				cState = CustomerStates.Waiting;
			}
			break;
		case CustomerStates.Waiting:
			position = ((Entity)Game.Player.Character).Position;
			if (position.DistanceTo(((Entity)Ped).Position) < 10f)
			{
				MovePed();
				cState = CustomerStates.WalkTo;
			}
			break;
		case CustomerStates.WalkTo:
			if (((Entity)Game.Player.Character).Position != target)
			{
				target = ((Entity)Game.Player.Character).Position;
				MovePed();
			}
			position = ((Entity)Game.Player.Character).Position;
			position.DistanceTo(((Entity)Ped).Position);
			_ = 3f;
			break;
		}
	}

	private void BuyDrugs()
	{
	}
}
