using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class Pig
{
	public enum States
	{
		Destroy,
		NoState,
		driveToPlayer,
		Normal,
		ApproachPlayer,
		WatchPlayer,
		SearchPlayer,
		GetBag
	}

	private readonly bool DEBUG;

	public Ped Ped;

	public Vehicle Vehicle;

	public States CurrentState = States.NoState;

	public States NewState = States.NoState;

	public Vector3 destination = Vector3.Zero;

	internal bool leader;

	private int time;

	private int delay = 20000;

	private int timeSeen;

	private bool sayWait;

	public bool detected;

	public bool seenPlayer;

	public DroppedBag Bag;

	private Vector3 pPos = Vector3.Zero;

	public bool AskPlayerToComply { get; private set; }

	public bool SirenActive { get; private set; }

	public bool SirenSoundActive { get; private set; } = true;

	public Pig(Ped _ped, bool _leader)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Ped = _ped;
		leader = _leader;
		if ((Entity)(object)Ped.CurrentVehicle != (Entity)null)
		{
			Vehicle = Ped.CurrentVehicle;
		}
		destination = StopPosition();
		((Entity)Ped).IsPersistent = true;
		Ped.KeepTaskWhenMarkedAsNoLongerNeeded = true;
		Ped.BlockPermanentEvents = true;
		Function.Call(unchecked((Hash)(-4970914549772615683L)), (Ped), (true));
		Function.Call(unchecked((Hash)(-5650329060376590909L)), (Ped), (100));
	}

	private Vector3 StopPosition()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 front = World.GetNextPositionOnStreet(((Entity)Game.Player.Character).Position + ((Entity)Game.Player.Character).ForwardVector * 3f);
		Vector3 back = World.GetNextPositionOnStreet(((Entity)Game.Player.Character).Position + ((Entity)Game.Player.Character).ForwardVector * -3f * 5f);
		Vector3 final = Vector3.Zero;
		if (front.DistanceTo(((Entity)Ped).Position) < back.DistanceTo(((Entity)Ped).Position))
		{
			return front;
		}
		return back;
	}

	public void OnTick()
	{
		DoState();
		CheckState();
		if (Game.Player.Wanted.WantedLevel > 0 || !LSL.isDealing)
		{
			DeletePig();
		}
	}

	public void CheckState()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position;
		switch (CurrentState)
		{
		case States.GetBag:
			if (Bag == null || Bag.CheckIfPickedUp())
			{
				if (DEBUG)
				{
					GTA.UI.Notification.PostTicker("Sac disparu", false, false);
				}
				NewState = States.Destroy;
			}
			break;
		case States.NoState:
			NewState = States.driveToPlayer;
			break;
		case States.driveToPlayer:
		{
			Vector3 pos = StopPosition();
			if (destination != pos)
			{
				destination = pos;
				NewState = States.NoState;
				break;
			}
			position = ((Entity)Vehicle).Position;
			if (position.DistanceTo(destination) < 15f && Vehicle.IsStopped)
			{
				position = ((Entity)Vehicle).Position;
				if (position.DistanceTo(LSL.playerPos) > 50f)
				{
					if (LSL.playerHeat > 25f)
					{
						destination = StopPosition();
						NewState = States.NoState;
					}
					else
					{
						NewState = States.Destroy;
					}
				}
				else if ((Entity)(object)Game.Player.Character.CurrentVehicle != (Entity)null)
				{
					if (Game.Player.Character.CurrentVehicle.Speed < 1f)
					{
						NewState = States.ApproachPlayer;
					}
					else
					{
						if (Game.GameTime > time + 3000)
						{
							time = Game.GameTime;
							Function.Call((Hash)1986128932158055094L, (((Entity)Vehicle).Handle));
						}
						if (!seenPlayer && LsFunctions.CanSee((Entity)(object)Ped, (Entity)(object)Game.Player.Character))
						{
							seenPlayer = true;
							timeSeen = Game.GameTime;
						}
						else if (!LsFunctions.CanSee((Entity)(object)Ped, (Entity)(object)Game.Player.Character))
						{
							seenPlayer = false;
							if (Game.Player.Wanted.WantedLevel < 1)
							{
								Game.Player.Wanted.SetWantedLevel(1, false);
			Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
							}
							LSL.areas[LSL.areaIndex].Heat += 10f;
						}
						else if (Game.GameTime > timeSeen + delay + 10000)
						{
							if (Game.Player.Wanted.WantedLevel < 1)
							{
								Game.Player.Wanted.SetWantedLevel(1, false);
			Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
							}
							LSL.areas[LSL.areaIndex].Heat += 10f;
						}
					}
				}
				else
				{
					NewState = States.ApproachPlayer;
				}
			}
			if (!leader)
			{
				break;
			}
			position = ((Entity)Vehicle).Position;
			if (position.DistanceTo(destination) < 50f)
			{
				if (!SirenActive)
				{
					Function.Call(unchecked((Hash)(-823518585696636035L)), (((Entity)Vehicle).Handle), (true));
					SirenActive = true;
				}
				if (SirenSoundActive)
				{
					SirenSoundActive = false;
					Function.Call(unchecked((Hash)(-2880880930286931340L)), (((Entity)Vehicle).Handle), (true));
				}
			}
			break;
		}
		case States.Normal:
			position = ((Entity)Ped).Position;
			if (position.DistanceTo(((Entity)Game.Player.Character).Position) > 80f)
			{
				NewState = States.ApproachPlayer;
			}
			break;
		case States.WatchPlayer:
			if (Game.Player.Wanted.WantedLevel > 0)
			{
				NewState = States.Destroy;
				break;
			}
			if (Game.Player.Wanted.WantedLevel < 1)
			{
				if ((Entity)(object)Game.Player.Character.CurrentVehicle != (Entity)null && !Game.Player.Character.CurrentVehicle.IsStopped)
				{
					LsFunctions.DbugString("Indice de recherche augmenté : refus d'obtempérer en véhicule");
					Game.Player.Wanted.SetWantedLevel(1, false);
			Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
					LSL.areas[LSL.areaIndex].Heat += 200f;
					break;
				}
				if (Game.GameTime > time + delay || LSL.playerPos.DistanceTo(pPos) > 2f)
				{
					Game.Player.Wanted.SetWantedLevel(1, false);
			Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
					LSL.areas[LSL.areaIndex].Heat += 200f;
					break;
				}
			}
			if (LSL.PlayerComplied && !Game.Player.Character.IsInVehicle())
			{
				AskPlayerToComply = false;
				NewState = States.SearchPlayer;
			}
			break;
		case States.SearchPlayer:
			if (Game.GameTime <= time + 3000)
			{
				break;
			}
			if (LSL.TotalDrugsCarried() > 0)
			{
				if (leader)
				{
					GTA.UI.Screen.ShowSubtitle("Tu es en état d'arrestation.", 3000);
					Game.Player.Wanted.SetWantedLevel(2, false);
			Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
				}
				NewState = States.Destroy;
				break;
			}
			if (leader)
			{
				GTA.UI.Screen.ShowSubtitle("On t'a à l'œil. Tu ferais mieux de quitter ~y~" + LsFunctions.GetZoneLabel(LSL.playerPos) + "~n~.", 4000);
			}
			LSL.areas[LSL.areaIndex].HeatDecay();
			Game.Player.Character.Task.ClearAll();
			NewState = States.Destroy;
			break;
		case States.ApproachPlayer:
			if (Game.Player.Wanted.WantedLevel > 0)
			{
				NewState = States.Destroy;
				break;
			}
			if (Game.Player.Wanted.WantedLevel < 1 && LSL.playerPos.DistanceTo(pPos) > 8f)
			{
				LsFunctions.DbugString("Indice de recherche augmenté : refus d'obtempérer");
				Game.Player.Wanted.SetWantedLevel(2, false);
			Game.Player.Wanted.ApplyWantedLevelChangeNow(false);
				LSL.areas[LSL.areaIndex].Heat += 200f;
				break;
			}
			position = ((Entity)Ped).Position;
			if (position.DistanceTo(destination) < 1f)
			{
				pPos = LSL.playerPos;
				NewState = States.WatchPlayer;
			}
			if (!sayWait && !Ped.IsInVehicle() && Ped.IsWalking && LSL.playerPos.DistanceTo(((Entity)Ped).Position) < 10f)
			{
				sayWait = true;
				Function.Call(unchecked((Hash)(-8213159594590722974L)), (Ped), ("WAIT"), ("SPEECH_PARAMS_STANDARD"));
			}
			break;
		}
	}

	public void DoState()
	{
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		if (NewState == CurrentState)
		{
			return;
		}
		if (DEBUG)
		{
			GTA.UI.Notification.PostTicker("Nouvel état de la police : " + NewState, false, false);
		}
		switch (NewState)
		{
		case States.GetBag:
			if (Bag == null)
			{
				NewState = CurrentState;
				break;
			}
			if (DEBUG)
			{
				GTA.UI.Notification.PostTicker("Récupération du sac", false, false);
			}
			Ped.Task.FollowNavMeshTo(Bag.pos);
			CurrentState = NewState;
			break;
		case States.SearchPlayer:
			if (leader)
			{
				TaskSequence searchPlayer = new TaskSequence();
				searchPlayer.AddTask.FollowNavMeshTo(((Entity)Ped).Position + ((Entity)Ped).ForwardVector * 0.5f);
				searchPlayer.AddTask.TurnTo((Entity)(object)Game.Player.Character, 1000);
				searchPlayer.AddTask.PlayAnimation("missexile3", "ex03_dingy_search_case_base_michael");
				Ped.Task.PerformSequence(searchPlayer);
			}
			time = Game.GameTime;
			CurrentState = States.SearchPlayer;
			break;
		case States.Destroy:
			LsFunctions.RemovePedFromWorld(Ped, clearTask: true);
			Ped.KeepTaskWhenMarkedAsNoLongerNeeded = false;
			Ped.BlockPermanentEvents = false;
			Ped = null;
			if ((Entity)(object)Vehicle != (Entity)null)
			{
				((Entity)Vehicle).IsPersistent = false;
			}
			LSL.entitiesCleanUp.Add((Entity)(object)Vehicle);
			Vehicle = null;
			CurrentState = States.Destroy;
			break;
		case States.NoState:
			CurrentState = States.NoState;
			break;
		case States.driveToPlayer:
			destination = StopPosition();
			if ((Entity)(object)Ped == (Entity)(object)Vehicle.Driver)
			{
				Ped.Task.DriveTo(Vehicle, destination, 10f, (GTA.VehicleDrivingFlags)(786468), 12f);
			}
			CurrentState = States.driveToPlayer;
			break;
		case States.Normal:
			if ((Entity)(object)Vehicle != (Entity)null)
			{
				Ped.Task.CruiseWithVehicle(Vehicle, 9f, (GTA.VehicleDrivingFlags)(786603));
			}
			else
			{
				Ped.Task.Wander();
			}
			CurrentState = States.Normal;
			if (DEBUG)
			{
				GTA.UI.Notification.PostTicker("Retour à l'état normal", false, false);
			}
			break;
		case States.WatchPlayer:
			Ped.Task.ClearAll();
			Ped.Weapons.Give((WeaponHash)1593441988, 100, true, true);
			Ped.Task.AimGunAtEntity((Entity)(object)Game.Player.Character, 20000);
			time = Game.GameTime;
			CurrentState = States.WatchPlayer;
			if ((Entity)(object)Game.Player.Character.CurrentVehicle != (Entity)null)
			{
				if (leader)
				{
					GTA.UI.Screen.ShowSubtitle("Monsieur, veuillez sortir du véhicule.", 4000);
				}
				AskPlayerToComply = true;
			}
			else
			{
				if (leader)
				{
					GTA.UI.Screen.ShowSubtitle("À terre !", 4000);
				}
				AskPlayerToComply = true;
			}
			if (DEBUG)
			{
				GTA.UI.Notification.PostTicker("Passage en surveillance", false, false);
			}
			break;
		case States.ApproachPlayer:
			if ((Entity)(object)Game.Player.Character.CurrentVehicle != (Entity)null)
			{
				if (leader)
				{
					destination = LSL.playerPos + ((Entity)Game.Player.Character).RightVector * -1f * 2f;
				}
				else
				{
					destination = LSL.playerPos + ((Entity)Game.Player.Character).RightVector * -1f * 2f + ((Entity)Game.Player.Character).ForwardVector * -1f * 2f;
				}
			}
			else if (leader)
			{
				destination = LSL.playerPos + ((Entity)Game.Player.Character).ForwardVector * 2f;
			}
			else
			{
				destination = LSL.playerPos + ((Entity)Game.Player.Character).RightVector * -1f * 2f + ((Entity)Game.Player.Character).ForwardVector * 2f;
			}
			Ped.Task.FollowNavMeshTo(destination);
			pPos = LSL.playerPos;
			Ped.Task.LookAt((Entity)(object)Game.Player.Character);
			CurrentState = States.ApproachPlayer;
			break;
		}
	}

	public void DeletePig()
	{
		NewState = States.Destroy;
	}
}
