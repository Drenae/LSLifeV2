using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class Dealer
{
	public enum States
	{
		Driving,
		Ariving,
		Leaving,
		Arrived,
		Faster,
		Fast,
		Slower
	}

	public float maxSpeed;

	public Vector3 destination;

	public bool doneTutorial;

	public States state;

	public Ped ped;

	public Vector3 pos;

	private Model vehModel;

	public Vehicle vehicle;

	internal bool canSpeed;

	internal bool shouldSpeed;

	public Dealer(Model pedModel, Vector3 pos, Model vehModel, PedGroup group)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		this.pos = pos;
		this.vehModel = vehModel;
		vehicle = World.CreateVehicle(this.vehModel, LSL.GenerateSpawnPos(pos.Around(80f), LSL.Nodetype.Road, sidewalk: false));
		while ((Entity)(object)vehicle == (Entity)null)
		{
			Script.Wait(0);
		}
		((Entity)vehicle).IsPersistent = true;
		LsFunctions.AddBlip((Entity)(object)vehicle, (BlipColor)3, "Zee's Driver", _friend: true, _edge: false);
		ped = World.CreatePed(pedModel, ((Entity)vehicle).Position);
		while ((Entity)(object)ped == (Entity)null)
		{
			Script.Wait(0);
		}
		((Entity)ped).IsPersistent = true;
		ped.SetIntoVehicle(vehicle, (VehicleSeat)(-1));
		Function.Call(unchecked((Hash)(-5650329060376590909L)), (InputArgument)(((Entity)ped).Handle), (InputArgument)(100f));
		Function.Call(unchecked((Hash)(-5348838930497642877L)), (InputArgument)(((Entity)vehicle).Handle), (InputArgument)(((Entity)ped).Handle));
		destination = World.GetNextPositionOnStreet(pos, true);
		ped.Task.DriveTo(vehicle, destination, 5f, 25f, 5);
		state = States.Driving;
		ped.StaysInVehicleWhenJacked = true;
		ped.BlockPermanentEvents = true;
		maxSpeed = Function.Call<float>(unchecked((Hash)(-858003554332316349L)), (InputArgument)(vehicle)) * 0.8f;
	}

	internal void SetSpeed()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (state == States.Driving || state == States.Fast)
		{
			Vector3 vPos = ((Entity)vehicle).Position + ((Entity)vehicle).Velocity * 2f;
			shouldSpeed = LsFunctions.ShouldSpeed(vPos);
			float speed = LsFunctions.GetRoadSpeed(vPos, canSpeed, shouldSpeed);
			if (maxSpeed > speed)
			{
				maxSpeed -= 10f * Game.LastFrameTime;
			}
			else
			{
				maxSpeed = speed;
			}
			ped.DrivingSpeed = maxSpeed;
			ped.MaxDrivingSpeed = maxSpeed;
		}
	}
}
