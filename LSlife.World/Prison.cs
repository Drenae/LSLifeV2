using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

internal static class Prison
{
	public enum PrisonStates
	{
		None,
		PlayerEnterCar,
		CopEnterCar,
		ApproachBarrier,
		ApproachGate1,
		ApproachGate2,
		FinalApproach,
		FinalPos
	}

	public class Gate
	{
		private readonly bool DEBUG = true;

		public Prop Prop;

		private bool Locked = true;

		public Gate(Prop prop)
		{
			Prop = prop;
		}

		public void Unlock()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			Script.Wait(10);
			Model model = ((Entity)Prop).Model;
			Function.Call(unchecked((Hash)(-7272475972733243984L)), (model.Hash), (((Entity)Prop).Position.X), (((Entity)Prop).Position.Y), (((Entity)Prop).Position.Z), (false), (0f), (50f), (0f));
		}

		public void Lock()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			Script.Wait(10);
			Model model = ((Entity)Prop).Model;
			Function.Call(unchecked((Hash)(-7272475972733243984L)), (model.Hash), (((Entity)Prop).Position.X), (((Entity)Prop).Position.Y), (((Entity)Prop).Position.Z), (true), (0f), (50f), (0f));
		}
	}

	public static LsTimer SentanceTime = null;

	private static int BailAmount = -1;

	private static PrisonStates NewState = PrisonStates.None;

	public static bool Started = false;

	public static bool Ready = false;

	private static Ped cop = null;

	private static Vehicle copCar = null;

	private static Vector3 PrisonPos = new Vector3(1946.057f, 2617.254f, 45.63307f);

	private static Vector3 BarrierWaitPos = new Vector3(1899.201f, 2609.867f, 45.34259f);

	private static Vector3 Gate1WaitPos = new Vector3(1851.723f, 2608.119f, 45.23964f);

	private static Vector3 Gate2WaitPos = new Vector3(1824.335f, 2607.923f, 45.18952f);

	private static Vector3 Gate2ExitPos = new Vector3(1798.774f, 2606.744f, 45.16935f);

	private static Vector3 FinalPos = new Vector3(1690.612f, 2604.468f, 45.16944f);

	private static Gate PriGate1 = null;

	private static Gate PriGate2 = null;

	private static bool DEBUG = LSL.DEBUG;

	private static float Speed = 0f;

	public static PrisonStates CurrentState { get; set; } = PrisonStates.ApproachGate2;

	public static void PrisonTick()
	{
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected Obj, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected Obj, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		DriverSpeed(cop, copCar);
		CancelWantedLevels();
		if (NewState != CurrentState)
		{
			bool stateChanged = false;
			switch (NewState)
			{
			case PrisonStates.None:
			{
				if (!Ready)
				{
					break;
				}
				Game.Player.IgnoredByPolice = true;
				Game.Player.IgnoredByEveryone = true;
				Vector3 playerPos = ((Entity)Game.Player.Character).Position;
				OutputArgument outArgA = new OutputArgument();
				OutputArgument outArgB = new OutputArgument();
				if (Function.Call<bool>(unchecked((Hash)(-70052395799280720L)), (playerPos.X), (playerPos.Y), (playerPos.Z), (InputArgument)(object)outArgA, (InputArgument)(object)outArgB, (0), (1077936128), (0)))
				{
					float heading = outArgB.GetResult<float>();
					Vector3 pos = outArgA.GetResult<Vector3>();
					if ((Entity)(object)copCar == (Entity)null)
					{
						copCar = World.CreateVehicle(new Model((VehicleHash)2046537925), pos, heading);
						((Entity)copCar).AddBlip();
						((Entity)copCar).AttachedBlip.Sprite = (BlipSprite)672;
						if (DEBUG)
						{
							GTA.UI.Notification.Show("Transport généré : " + (object)copCar);
						}
					}
				}
				if ((Entity)(object)cop == (Entity)null)
				{
					cop = World.CreatePed(new Model((PedHash)1581098148), ((Entity)Game.Player.Character).Position + ((Entity)Game.Player.Character).ForwardVector * -1f * 1.5f);
				}
				if ((Entity)(object)copCar != (Entity)null && (Entity)(object)cop != (Entity)null)
				{
					Function.Call(unchecked((Hash)(-3189321952077349130L)), ("mp_arresting"));
					while (!Function.Call<bool>(unchecked((Hash)(-3444786327252301684L)), ("mp_arresting")))
					{
						Script.Wait(100);
					}
					Function.Call(unchecked((Hash)(-1565002832890405996L)), (Game.Player.Character), ("mp_arresting"), ("idle"), (8f), (1f), (-1), (49), (0.1f), (0), (0), (0));
					Game.Player.Character.Task.EnterVehicle(copCar, (VehicleSeat)2);
					if (Function.Call<bool>((Hash)5315141875575487047L))
					{
						Function.Call(unchecked((Hash)(-8272491852216909662L)), (Game.Player.Handle), (false), (256));
					}
					Function.Call((Hash)3479844549214047358L, (((Entity)cop).Handle), (((Entity)Game.Player.Character).Handle), (1), (1), (0), (1), (-1), (10), (true));
					CurrentState = PrisonStates.None;
					stateChanged = true;
				}
				break;
			}
			case PrisonStates.PlayerEnterCar:
				cop.Task.ClearAll();
				cop.AlwaysKeepTask = true;
				cop.BlockPermanentEvents = true;
				cop.Task.EnterVehicle(copCar, unchecked((VehicleSeat)(-1)));
				CurrentState = PrisonStates.PlayerEnterCar;
				stateChanged = true;
				break;
			case PrisonStates.CopEnterCar:
				Function.Call(unchecked((Hash)(-5650329060376590909L)), (((Entity)cop).Handle), (100f));
				cop.Task.DriveTo(copCar, PrisonPos, 5f, (GTA.VehicleDrivingFlags)(5), Speed);
				CurrentState = PrisonStates.CopEnterCar;
				stateChanged = true;
				break;
			case PrisonStates.ApproachBarrier:
				cop.Task.DriveTo(copCar, BarrierWaitPos, 1f, (GTA.VehicleDrivingFlags)(2883621), Speed);
				CurrentState = PrisonStates.ApproachBarrier;
				stateChanged = true;
				break;
			case PrisonStates.ApproachGate1:
				cop.Task.DriveTo(copCar, Gate1WaitPos, 1f, (GTA.VehicleDrivingFlags)(262144), Speed);
				CurrentState = PrisonStates.ApproachGate1;
				stateChanged = true;
				break;
			case PrisonStates.ApproachGate2:
				cop.Task.DriveTo(copCar, Gate2WaitPos, 1f, (GTA.VehicleDrivingFlags)(262144), Speed);
				CurrentState = PrisonStates.ApproachGate2;
				stateChanged = true;
				break;
			case PrisonStates.FinalApproach:
				cop.Task.DriveTo(copCar, Gate2ExitPos, 1f, (GTA.VehicleDrivingFlags)(262144), Speed);
				CurrentState = PrisonStates.FinalApproach;
				stateChanged = true;
				break;
			case PrisonStates.FinalPos:
				cop.Task.DriveTo(copCar, FinalPos, 1f, (GTA.VehicleDrivingFlags)(262144), Speed);
				CurrentState = PrisonStates.FinalPos;
				stateChanged = true;
				break;
			}
			if (stateChanged)
			{
				GTA.UI.Notification.Show("Passage à l'état " + NewState);
			}
			return;
		}
		Vector3 position;
		switch (CurrentState)
		{
		case PrisonStates.None:
			if (Game.Player.Character.IsInVehicle(copCar) && !cop.IsInVehicle(copCar))
			{
				NewState = PrisonStates.PlayerEnterCar;
			}
			break;
		case PrisonStates.PlayerEnterCar:
			if (cop.IsInVehicle(copCar))
			{
				Script.Wait(1000);
				NewState = PrisonStates.CopEnterCar;
			}
			break;
		case PrisonStates.CopEnterCar:
			position = ((Entity)copCar).Position;
			if (position.DistanceTo(PrisonPos) < 50f)
			{
				NewState = PrisonStates.ApproachBarrier;
			}
			break;
		case PrisonStates.ApproachBarrier:
			position = ((Entity)copCar).Position;
			if (position.DistanceTo(BarrierWaitPos) <= 2f)
			{
				NewState = PrisonStates.ApproachGate1;
			}
			break;
		case PrisonStates.ApproachGate1:
			position = ((Entity)copCar).Position;
			if (position.DistanceTo(Gate1WaitPos) <= 5f && PriGate1 == null && (Entity)(object)pGate1() != (Entity)null)
			{
				PriGate1 = new Gate(pGate1());
				((Entity)PriGate1.Prop).IsPositionFrozen = false;
				NewState = PrisonStates.ApproachGate2;
				Model model = ((Entity)pGate1()).Model;
				GTA.UI.Notification.Show("Portail trouvé : " + model.Hash);
			}
			break;
		case PrisonStates.ApproachGate2:
			PriGate1.Unlock();
			position = ((Entity)copCar).Position;
			if (position.DistanceTo(Gate2WaitPos) <= 3f && PriGate2 == null && (Entity)(object)pGate2() != (Entity)null)
			{
				PriGate2 = new Gate(pGate2());
				((Entity)PriGate2.Prop).IsPositionFrozen = false;
				NewState = PrisonStates.FinalApproach;
			}
			break;
		case PrisonStates.FinalApproach:
			PriGate2.Unlock();
			position = ((Entity)copCar).Position;
			if (position.DistanceTo(Gate2ExitPos) <= 3f)
			{
				NewState = PrisonStates.FinalPos;
			}
			break;
		case PrisonStates.FinalPos:
			position = ((Entity)copCar).Position;
			if (position.DistanceTo2D(FinalPos) < 10f)
			{
				PlayerExitPrison();
			}
			break;
		}
	}

	private static void DriverSpeed(Ped p, Vehicle v)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((Entity)(object)v != (Entity)null) || !((Entity)(object)p != (Entity)null) || ((Entity)v.Driver).Handle != ((Entity)p).Handle)
		{
			return;
		}
		float roadSpeed = LsFunctions.GetRoadSpeed(((Entity)v).Position + ((Entity)v).Velocity, canSpeed: false, _shouldSpeed: false);
		if (roadSpeed != Speed)
		{
			float increment = 0.01f;
			Speed = roadSpeed;
			if (v.Speed > Speed)
			{
				p.MaxDrivingSpeed = v.Speed - increment * Game.LastFrameTime;
				p.DrivingSpeed = v.Speed - increment * Game.LastFrameTime;
			}
			else
			{
				p.MaxDrivingSpeed = Speed;
				p.DrivingSpeed = Speed;
			}
		}
	}

	private static void CancelWantedLevels()
	{
		if (Game.MaxWantedLevel != 0)
		{
			Game.MaxWantedLevel = 0;
		}
	}

	private static void PlayerExitPrison()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Function.Call(unchecked((Hash)(-8272491852216909662L)), (Game.Player.Handle), (true), (256));
		Game.Player.CanControlCharacter = true;
		LSL.entitiesCleanUp.Add((Entity)(object)copCar);
		LSL.entitiesCleanUp.Add((Entity)(object)cop);
		cop = null;
		copCar = null;
		Game.MaxWantedLevel = 5;
		((Entity)Game.Player.Character).Position = Gate1WaitPos;
		Started = false;
		NewState = PrisonStates.None;
	}

	public static Prop pGate1()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		int doorHash = 741314661;
		Prop prop = null;
		Prop[] props = World.GetNearbyProps(new Vector3(1844.9984f, 2604.8125f, 44.6398f), 10f);
		if (props.Count() > 0)
		{
			Prop[] array = props;
			foreach (Prop p in array)
			{
				Model model = ((Entity)p).Model;
				if (model.Hash == doorHash)
				{
					prop = p;
					GTA.UI.Notification.Show("Portail 1 trouvé");
					break;
				}
			}
		}
		return prop;
	}

	public static Prop pGate2()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		int doorHash = 741314661;
		Prop prop = null;
		Prop[] props = World.GetNearbyProps(new Vector3(1818.5428f, 2604.8125f, 44.611f), 10f);
		if (props.Count() > 0)
		{
			Prop[] array = props;
			foreach (Prop p in array)
			{
				Model model = ((Entity)p).Model;
				if (model.Hash == doorHash)
				{
					prop = p;
					GTA.UI.Notification.Show("Portail 2 trouvé");
					break;
				}
			}
		}
		return prop;
	}

	private static float changeSpeed(float fromVal, float toVal, float duration)
	{
		return 0f;
	}

	private static float MaxSpeed()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		LSL.GetRoadFlags(((Entity)copCar).Position);
		return 0f;
	}

	static Prison()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
	}
}
