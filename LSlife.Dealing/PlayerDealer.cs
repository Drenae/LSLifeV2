using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LSlife.Shots;

namespace LSlife;

public class PlayerDealer
{
	public List<XElement> dWeapons = new List<XElement>();

	public Enums.DealState dealState;

	public int ID;

	public Dictionary<string, int> Drugs = new Dictionary<string, int>();

	public int Money;

	public Enums.PdStates CurrentState;

	public Enums.PdStates NewState = Enums.PdStates.Work;

	private Enums.HealPositionTypes HealPosType;

	public PedHash pHash;

	public Vector3 Pos;

	private Blip blip;

	private int time;

	private Vector3 Target;

	public Dictionary<int, Tuple<int, int>> pData;

	public bool Working;

	private int health;

	public int dArmour { get; set; }

	public string area { get; private set; }

	public Ped Ped { get; private set; }

	public Ped cPed { get; private set; }

	public Ped lPed { get; private set; }

	private bool isToFar
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return LSL.playerPos.DistanceTo(Pos) > 100f;
		}
	}

	public PlayerDealer(Ped ped)
	{
		if ((Entity)(object)ped != (Entity)null)
		{
			Ped = ped;
			((Entity)Ped).IsPersistent = true;
			LsFunctions.SetRandomCombatStats(Ped);
			SetAbilities();
			Ped.RelationshipGroup = LSL.playerRelationship;
			LsFunctions.SetImmuneToPlayer(Ped);
			dArmour = Ped.Armor;
			GetFreshPedsWeapons();
			Ped.Weapons.Select(Ped.Weapons.BestWeapon);
			Ped.CanSwitchWeapons = true;
			area = LSL.areas[LSL.areaIndex].Name;
			ID = LSL.DealerHandler.GetNewID();
			SetStartDrugs();
			pData = LsFunctions.GetPedVariationData(Ped);
			SaveFreshDealer();
			NewState = Enums.PdStates.FollowPlayer;
			LsFunctions.AddBlip((Entity)(object)ped, (BlipColor)3, "Dealer", _friend: true, _edge: true);
			SetBlip();
		}
	}

	public PlayerDealer(int id, PedHash hash, Enums.PdStates _newState, Vector3 pos, int weed, int crack, int cocaine, int money, int _armour, List<XElement> _weapons, Dictionary<int, Tuple<int, int>> _pData)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		area = World.GetZoneNameLabel(pos);
		ID = id;
		pHash = hash;
		Pos = pos;
		Drugs.Add("Weed", weed);
		Drugs.Add("Crack", crack);
		Drugs.Add("Cocaine", cocaine);
		Money = money;
		dArmour = _armour;
		if (_weapons.Count > 0)
		{
			for (int i = 0; i < _weapons.Count; i++)
			{
				dWeapons.Add(_weapons[i]);
			}
		}
		NewState = _newState;
		CreateLocationBlip();
		pData = _pData;
	}

	private void Innit()
	{
	}

	internal List<object> Orders()
	{
		List<object> _orders = new List<object>();
		_orders.Add("None");
		if (Working)
		{
			_orders.Add(Enums.PdStates.FollowPlayer.ToString());
		}
		else if (LSL.DealerHandler.dealers.Where((PlayerDealer d) => d != this && d.area == area && d.Working).Count() == 0)
		{
			_orders.Add(Enums.PdStates.Work.ToString());
		}
		return _orders;
	}

	private void GetFreshPedsWeapons()
	{
		if ((Entity)(object)Ped != (Entity)null)
		{
			dWeapons = LsFunctions.PedWeapons(Ped);
		}
	}

	private void SaveFreshDealer()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		XDocument dealerDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
		Money = 0;
		Pos = ((Entity)Ped).Position;
		Model model = ((Entity)Ped).Model;
		pHash = (PedHash)model.Hash;
		XElement parentElement = new XElement("dealer");
		XElement id = new XElement("id");
		id.Value = ID.ToString();
		XElement pedHash = new XElement("hash");
		pedHash.Value = ((object)pHash/*cast due to constrained. prefix*/).ToString();
		XElement posX = new XElement("posX");
		posX.Value = ((Entity)Ped).Position.X.ToString();
		XElement posY = new XElement("posY");
		posY.Value = ((Entity)Ped).Position.Y.ToString();
		XElement posZ = new XElement("posZ");
		posZ.Value = ((Entity)Ped).Position.Z.ToString();
		XElement weed = new XElement("weed");
		weed.Value = Drugs["Weed"].ToString();
		XElement crack = new XElement("crack");
		crack.Value = Drugs["Crack"].ToString();
		XElement coke = new XElement("cocaine");
		coke.Value = Drugs["Cocaine"].ToString();
		XElement money = new XElement("money");
		money.Value = Money.ToString();
		XElement arm = new XElement("armour");
		arm.Value = dArmour.ToString();
		XElement pedData = new XElement("pedData");
		bool savePedData = false;
		Dictionary<int, Tuple<int, int>> dictionary = pData;
		if (dictionary != null && dictionary.Count > 0)
		{
			savePedData = true;
			XElement headD = new XElement("headD", pData[0].Item1.ToString());
			XElement headT = new XElement("headT", pData[0].Item2.ToString());
			pedData.Add(headD);
			pedData.Add(headT);
			XElement beardD = new XElement("beardD", pData[1].Item1.ToString());
			XElement beardT = new XElement("beardT", pData[1].Item2.ToString());
			pedData.Add(beardD);
			pedData.Add(beardT);
			XElement hairD = new XElement("hairD", pData[2].Item1.ToString());
			XElement hairT = new XElement("hairT", pData[2].Item2.ToString());
			pedData.Add(hairD);
			pedData.Add(hairT);
			XElement torsoD = new XElement("torsoD", pData[3].Item1.ToString());
			XElement torsoT = new XElement("torsoT", pData[3].Item2.ToString());
			pedData.Add(torsoD);
			pedData.Add(torsoT);
			XElement legsD = new XElement("legsD", pData[4].Item1.ToString());
			XElement legsT = new XElement("legsT", pData[4].Item2.ToString());
			pedData.Add(legsD);
			pedData.Add(legsT);
			XElement handsD = new XElement("handsD", pData[5].Item1.ToString());
			XElement handsT = new XElement("handsT", pData[5].Item2.ToString());
			pedData.Add(handsD);
			pedData.Add(handsT);
			XElement footD = new XElement("footD", pData[6].Item1.ToString());
			XElement footT = new XElement("footT", pData[6].Item2.ToString());
			pedData.Add(footD);
			pedData.Add(footT);
			XElement acces1D = new XElement("acces1D", pData[8].Item1.ToString());
			XElement acces1T = new XElement("acces1T", pData[8].Item2.ToString());
			pedData.Add(acces1D);
			pedData.Add(acces1T);
			XElement acces2D = new XElement("acces2D", pData[9].Item1.ToString());
			XElement acces2T = new XElement("acces2T", pData[9].Item2.ToString());
			pedData.Add(acces2D);
			pedData.Add(acces2T);
			XElement decalD = new XElement("decalD", pData[10].Item1.ToString());
			XElement decalT = new XElement("decalT", pData[10].Item2.ToString());
			pedData.Add(decalD);
			pedData.Add(decalT);
			XElement torsoAuxD = new XElement("torsoAuxD", pData[11].Item1.ToString());
			XElement torsoAuxT = new XElement("torsoAuxT", pData[11].Item2.ToString());
			pedData.Add(torsoAuxD);
			pedData.Add(torsoAuxT);
		}
		parentElement.Add(id);
		parentElement.Add(pedHash);
		LsFunctions.DbugString(posX?.ToString() + ", " + posY?.ToString() + ", " + posZ);
		parentElement.Add(posX);
		parentElement.Add(posY);
		parentElement.Add(posZ);
		parentElement.Add(weed);
		parentElement.Add(crack);
		parentElement.Add(coke);
		parentElement.Add(money);
		parentElement.Add(arm);
		List<XElement> list = dWeapons;
		if (list != null && list.Count > 0)
		{
			foreach (XElement ele in dWeapons)
			{
				if (!(ele.Name == "weapon"))
				{
					continue;
				}
				foreach (XElement e in ele.Elements())
				{
					if (e.Name == "HASH" && e.Value != "Unarmed")
					{
						parentElement.Add(e.Parent);
					}
				}
			}
		}
		if (savePedData)
		{
			parentElement.Add(pedData);
		}
		dealerDoc.Root.Add(parentElement);
		dealerDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
	}

	public void OnTick()
	{
		if ((Entity)(object)Ped != (Entity)null)
		{
			Enums.PdStates currentState = CurrentState;
			if (currentState == Enums.PdStates.TalkToPlayer || (uint)(currentState - 8) <= 1u || currentState == Enums.PdStates.Defend)
			{
				PromtPlayerToTalk();
			}
			else
			{
				PromtToFollow();
			}
		}
	}

	public void OnSlowTick()
	{
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_094d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_095d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
		//IL_0967: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Invalid comparison between Unknown and I4
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c55: Invalid comparison between Unknown and I4
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Expected Obj, but got Unknown
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		if ((Entity)(object)Ped != (Entity)null)
		{
			if (Ped.RelationshipGroup != LSL.playerRelationship)
			{
				UI.Notify("~r~Employé retiré du groupe de compagnons : réintégration");
				Ped.RelationshipGroup = LSL.playerRelationship;
			}
			if (!isFollowing() && LSL.driveByHandler.AmountOfDriveBy(_agg: true) > 0 && !playerNear())
			{
				NewState = Enums.PdStates.Defend;
			}
			if (Pos != ((Entity)Ped).Position)
			{
				Pos = ((Entity)Ped).Position;
			}
			if (area != World.GetZoneNameLabel(((Entity)Ped).Position))
			{
				area = World.GetZoneNameLabel(((Entity)Ped).Position);
			}
			dArmour = Ped.Armor;
			if (LSL.DealerHandler.currentDealer == this)
			{
				bool dFarAway = Pos.DistanceTo(LSL.playerPos) > 3f;
				bool dNotTalking = CurrentState != Enums.PdStates.TalkToPlayer && CurrentState != Enums.PdStates.FollowPlayer && CurrentState != Enums.PdStates.FollowingPlayer;
				bool flag = !LSL.WorkerMainMenu.Visible && !CustomWorkerMenu.Visible;
				bool vMoving = Ped.IsInVehicle() && Ped.CurrentVehicle.Speed > 0f;
				if (flag | dNotTalking | dFarAway | vMoving)
				{
					LSL.DealerHandler.currentDealer = null;
				}
				if (LSL.DealerHandler.currentDealer == null && LSL.WorkerMainMenu.Visible)
				{
					LSL.WorkerMainMenu.Visible = false;
				}
			}
			if (!((Entity)Ped).IsAlive && NewState != Enums.PdStates.Dead)
			{
				NewState = Enums.PdStates.Dead;
			}
			Vector3 val;
			if (NewState != CurrentState)
			{
				if ((Entity)(object)cPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(cPed, clearTask: true);
				}
				if (LSL.DEBUG)
				{
					UI.Notify("PDealer state = " + NewState);
				}
				switch (NewState)
				{
				case Enums.PdStates.Dead:
				{
					CurrentState = Enums.PdStates.Dead;
					string onDeathSay = "One of your dealers dropped a ~g~bag.";
					LSL.pickupHandler.AddBag(new DroppedBag(onDeathSay, Drugs["Weed"], Drugs["Crack"], Drugs["Cocaine"], Money, dWeapons.ToList(), Pos));
					RemoveBlip();
					((Entity)Ped).IsPersistent = false;
					((Entity)Ped).MarkAsNoLongerNeeded();
					break;
				}
				case Enums.PdStates.Defend:
					CurrentState = Enums.PdStates.Defend;
					break;
				case Enums.PdStates.WalkToPath:
					Target = World.GetNextPositionOnSidewalk(((Entity)Ped).Position);
					Ped.Task.GoTo(Target);
					CurrentState = Enums.PdStates.WalkToPath;
					time = Game.GameTime;
					break;
				case Enums.PdStates.StopForPlayer:
					Ped.Task.StandStill(50000);
					CurrentState = Enums.PdStates.StopForPlayer;
					break;
				case Enums.PdStates.LookAtPlayer:
					Ped.Task.TurnTo((Entity)(object)Game.Player.Character);
					CurrentState = Enums.PdStates.LookAtPlayer;
					break;
				case Enums.PdStates.TalkToPlayer:
					CurrentState = Enums.PdStates.TalkToPlayer;
					Function.Call(unchecked((Hash)(-8213159594590722974L)), (Ped), ("GENERIC_HOWS_IT_GOING"), ("SPEECH_PARAMS_FORCE"));
					break;
				case Enums.PdStates.Work:
					Working = true;
					Target = World.GetNextPositionOnSidewalk(((Entity)Ped).Position);
					if ((Entity)(object)Ped != (Entity)null)
					{
						SetBlip();
					}
					if (Ped.CurrentPedGroup == Game.Player.Character.CurrentPedGroup)
					{
						Ped.LeaveGroup();
					}
					if ((Entity)(object)Ped.CurrentVehicle != (Entity)null && (Entity)(object)Ped.CurrentVehicle == (Entity)(object)Game.Player.Character.CurrentVehicle)
					{
						TaskSequence sequence = new TaskSequence();
						sequence.AddTask.LeaveVehicle();
						sequence.AddTask.GuardCurrentPosition();
						Ped.Task.PerformSequence(sequence);
						sequence.Dispose();
					}
					else
					{
						Ped.Task.GuardCurrentPosition();
					}
					CurrentState = Enums.PdStates.Work;
					break;
				case Enums.PdStates.Dealing:
					CurrentState = Enums.PdStates.Dealing;
					break;
				case Enums.PdStates.FollowPlayer:
					Working = false;
					if ((Entity)(object)Ped != (Entity)null)
					{
						SetBlip();
					}
					if (Ped.CurrentPedGroup == LSL.player.Character.CurrentPedGroup)
					{
						Ped.LeaveGroup();
					}
					Ped.Task.ClearAll();
					CurrentState = Enums.PdStates.FollowPlayer;
					break;
				case Enums.PdStates.FollowingPlayer:
					CurrentState = Enums.PdStates.FollowingPlayer;
					break;
				case Enums.PdStates.LeadTeam:
					if (LSL.DealerHandler.CreateTeam(this))
					{
						CurrentState = Enums.PdStates.LeadTeam;
						Ped.LeaveGroup();
						Ped.AlwaysKeepTask = true;
					}
					else
					{
						NewState = Enums.PdStates.FollowPlayer;
					}
					break;
				case Enums.PdStates.Rest:
					Target = LSL.DealerHandler.GetCloseHealPosition(((Entity)Ped).Position);
					LsFunctions.DbugString("pDealer distance to healPos ~g~" + Target.DistanceTo(((Entity)Ped).Position) + " ~s~healPos type ~g~" + LSL.DealerHandler.HealPositionType);
					if (LSL.DealerHandler.HealPositionType != Enums.HealPositionTypes.None && Target.DistanceTo(((Entity)Ped).Position) < 30f)
					{
						if (!Ped.IsInVehicle() || (Ped.IsInVehicle() && Ped.CurrentVehicle.Speed < 1f))
						{
							if (LSL.DealerHandler.HealPositionType == Enums.HealPositionTypes.Hospital && Game.Player.Money >= (((Entity)Ped).MaxHealth - ((Entity)Ped).Health) * 50)
							{
								Ped.LeaveGroup();
								Ped.BlockPermanentEvents = true;
								HealPosType = LSL.DealerHandler.HealPositionType;
								Ped.Task.RunTo(Target);
								CurrentState = Enums.PdStates.Rest;
							}
							else if (LSL.DealerHandler.HealPositionType == Enums.HealPositionTypes.House)
							{
								Ped.LeaveGroup();
								Ped.BlockPermanentEvents = true;
								HealPosType = LSL.DealerHandler.HealPositionType;
								Ped.Task.RunTo(Target);
								CurrentState = Enums.PdStates.Rest;
							}
						}
					}
					else if (((Entity)LSL.player.Character).IsDead || Ped.CurrentPedGroup != LSL.player.Character.CurrentPedGroup || LSL.playerPos.DistanceTo(((Entity)Ped).Position) > 100f)
					{
						Ped.LeaveGroup();
						NewState = Enums.PdStates.FollowPlayer;
					}
					break;
				case Enums.PdStates.Resting:
				{
					XDocument dealerDoc1 = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
					LSL.DealerHandler.SaveDealer(this, dealerDoc1);
					dealerDoc1.Save("scripts\\LSLife\\LSLife_Dealers.xml");
					CurrentState = Enums.PdStates.Resting;
					break;
				}
				}
			}
			else
			{
				switch (CurrentState)
				{
				case Enums.PdStates.WalkToPath:
					val = ((Entity)Ped).Position;
					if (val.DistanceTo(Target) <= 2f)
					{
						NewState = Enums.PdStates.Work;
					}
					if (playerNear())
					{
						NewState = Enums.PdStates.StopForPlayer;
					}
					break;
				case Enums.PdStates.Defend:
					if (LSL.driveByHandler.AmountOfDriveBy(_agg: true) == 0 || playerNear())
					{
						NewState = Enums.PdStates.Work;
					}
					break;
				case Enums.PdStates.Work:
					if (playerNear())
					{
						NewState = Enums.PdStates.StopForPlayer;
						break;
					}
					val = ((Entity)Ped).Position;
					if (val.DistanceTo(Target) > 1f)
					{
						NewState = Enums.PdStates.WalkToPath;
					}
					else
					{
						NewState = Enums.PdStates.Dealing;
					}
					break;
				case Enums.PdStates.StopForPlayer:
					if (Ped.IsStopped)
					{
						NewState = Enums.PdStates.LookAtPlayer;
					}
					if (!playerNear())
					{
						NewState = Enums.PdStates.Work;
					}
					break;
				case Enums.PdStates.LookAtPlayer:
					if (!Ped.IsHeadtracking((Entity)(object)LSL.player.Character))
					{
						Ped.Task.LookAt((Entity)(object)LSL.player.Character);
					}
					else
					{
						NewState = Enums.PdStates.TalkToPlayer;
					}
					if (!playerNear())
					{
						NewState = Enums.PdStates.Work;
					}
					break;
				case Enums.PdStates.TalkToPlayer:
					if (!playerNear())
					{
						if (LSL.DealerHandler.currentDealer == this && (LSL.WorkerMainMenu.Visible || CustomWorkerMenu.Visible))
						{
							LSL.WorkerMainMenu.Visible = false;
						}
						NewState = Enums.PdStates.Work;
						Function.Call(unchecked((Hash)(-4137652152022615111L)), (Ped), ("GENERIC_BYE"), ("SPEECH_PARAMS_FORCE"));
					}
					break;
				case Enums.PdStates.Dealing:
					Target = World.GetNextPositionOnSidewalk(Pos + ((Entity)Ped).Velocity);
					if (playerNear())
					{
						NewState = Enums.PdStates.StopForPlayer;
					}
					else if (Pos.DistanceTo(Target) > 2.3f && Function.Call<bool>((Hash)1322919935073282825L, (Pos.X), (Pos.Y), (Pos.Z)))
					{
						NewState = Enums.PdStates.WalkToPath;
					}
					else if (LsFunctions.TotalDrugs(Drugs) > 0)
					{
						Deal();
					}
					break;
				case Enums.PdStates.FollowPlayer:
					if (isToFar)
					{
						((Entity)Ped).Position = LSL.playerPos.Around(2f);
					}
					if (LSL.player.Character.CurrentPedGroup != Ped.CurrentPedGroup)
					{
						val = ((Entity)LSL.player.Character).Position;
						if (val.DistanceTo(Pos) < 100f)
						{
							Ped.BlockPermanentEvents = false;
							LSL.player.Character.CurrentPedGroup.Add(Ped, false);
							Function.Call((Hash)3327951495314425415L, (((Entity)Ped).Handle), (LSL.player.Character.CurrentPedGroup.Handle), (false));
							NewState = Enums.PdStates.FollowingPlayer;
							break;
						}
					}
					if (LSL.player.Character.CurrentPedGroup == Ped.CurrentPedGroup)
					{
						Ped.BlockPermanentEvents = false;
						Function.Call((Hash)3327951495314425415L, (((Entity)Ped).Handle), (LSL.player.Character.CurrentPedGroup.Handle), (false));
						NewState = Enums.PdStates.FollowingPlayer;
					}
					break;
				case Enums.PdStates.FollowingPlayer:
				{
					if (((Entity)LSL.player.Character).IsDead || Ped.CurrentPedGroup != LSL.player.Character.CurrentPedGroup || isToFar)
					{
						if (isToFar)
						{
							((Entity)Ped).Position = LSL.playerPos.Around(2f);
						}
						Ped.LeaveGroup();
						NewState = Enums.PdStates.FollowPlayer;
					}
					else if (((Entity)Ped).Health < ((Entity)Ped).MaxHealth && !Ped.IsInCombat)
					{
						val = LSL.DealerHandler.GetCloseHealPosition(Pos);
						if (val.DistanceTo(Pos) < 30f)
						{
							NewState = Enums.PdStates.Rest;
						}
					}
					bool pedUnarmed = (int)Ped.Weapons.Current.Hash == -1569615261;
					bool playerUnarmed = (int)LSL.player.Character.Weapons.Current.Hash == -1569615261;
					if (!pedUnarmed & playerUnarmed)
					{
						Ped.Weapons.Select(unchecked((WeaponHash)(-1569615261)), true);
						Ped.CanSwitchWeapons = false;
					}
					else if (pedUnarmed && !playerUnarmed)
					{
						Ped.Weapons.Select(Ped.Weapons.BestWeapon);
						Ped.CanSwitchWeapons = true;
					}
					break;
				}
				case Enums.PdStates.Rest:
					if (Pos.DistanceTo(Target) < 2f)
					{
						health = ((Entity)Ped).Health;
						NewState = Enums.PdStates.Resting;
					}
					break;
				case Enums.PdStates.Resting:
					if (((Entity)Ped).Health < ((Entity)Ped).MaxHealth)
					{
						if (HealPosType == Enums.HealPositionTypes.Hospital)
						{
							((Entity)Ped).Health = ((Entity)Ped).MaxHealth;
							Game.Player.Money = Game.Player.Money - (((Entity)Ped).MaxHealth - ((Entity)Ped).Health) * 50;
							LsFunctions.PlayTheSound();
						}
						else
						{
							Ped ped = Ped;
							int num = ((Entity)ped).Health;
							((Entity)ped).Health = num + 1;
						}
					}
					else
					{
						NewState = Enums.PdStates.FollowPlayer;
					}
					break;
				}
			}
			val = ((Entity)Ped).Position;
			if (val.DistanceTo(LSL.playerPos) > 200f && area != LSL.playerArea && NewState != Enums.PdStates.FollowPlayer && NewState != Enums.PdStates.FollowingPlayer)
			{
				DespawnPed();
			}
		}
		else if (Pos.DistanceTo(LSL.playerPos) <= 200f && CurrentState != Enums.PdStates.Dead)
		{
			SpawnPed();
		}
	}

	public void GiveArmourToDealer()
	{
		Ped.Armor = 200;
		dArmour = 200;
	}

	private bool CheckIfDead()
	{
		if (!((Entity)Ped).IsAlive)
		{
			RemoveBlip();
			((Entity)Ped).IsPersistent = false;
			Ped = null;
			return true;
		}
		return false;
	}

	private void Deal()
	{
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		if ((Entity)(object)Ped != (Entity)null)
		{
			if ((Entity)(object)cPed != (Entity)null && !((Entity)cPed).Exists())
			{
				lPed = cPed;
				cPed = null;
				if (LSL.DEBUG)
				{
					UI.Notify("Le client du dealer n'existe pas");
				}
			}
			if ((Entity)(object)cPed != (Entity)null && ((Entity)cPed).Exists() && ((Entity)cPed).IsDead)
			{
				lPed = cPed;
				cPed = null;
				if (LSL.DEBUG)
				{
					UI.Notify("Le client du dealer n'existe pas");
				}
			}
			if ((Entity)(object)cPed == (Entity)null)
			{
				if (LSL.rnd.Next(0, 5) != 0)
				{
					return;
				}
				if (LSL.DEBUG)
				{
					UI.Notify("Recherche d'un client pour le dealer");
				}
				List<Ped> peds = LSL.lPeds.GetPeds;
				if (peds.ToList().Count > 0)
				{
					foreach (Ped ped in peds)
					{
						if (!LSL.CheckIfPedUsed(ped, _allowPolice: false, _allowVehicle: false) && (Entity)(object)ped != (Entity)(object)lPed)
						{
							cPed = ped;
							((Entity)cPed).FreezePosition = false;
							((Entity)cPed).IsPersistent = true;
							if (LSL.DEBUG)
							{
								((Entity)cPed).AddBlip();
							}
							break;
						}
					}
					return;
				}
				if (LSL.DEBUG)
				{
					UI.Notify("Aucun personnage trouvé pour le dealer");
				}
				return;
			}
			Vector3 position;
			switch (dealState)
			{
			case Enums.DealState.GetNear:
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(((Entity)Ped).Position) > 100f)
				{
					LsFunctions.RemovePedFromWorld(cPed, clearTask: false);
					cPed = null;
					break;
				}
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(((Entity)Ped).Position) > 10f)
				{
					LsFunctions.MovePed(cPed, ((Entity)Ped).Position);
					break;
				}
				dealState = Enums.DealState.Approach;
				cPed.Task.LookAt((Entity)(object)Ped);
				Ped.Task.TurnTo((Entity)(object)cPed);
				break;
			case Enums.DealState.Approach:
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(((Entity)Ped).Position) > 2f)
				{
					LsFunctions.MovePed(cPed, ((Entity)Ped).Position);
					break;
				}
				cPed.Task.TurnTo((Entity)(object)Ped);
				Ped.Task.TurnTo((Entity)(object)cPed);
				dealState = Enums.DealState.InPosition;
				time = Game.GameTime;
				break;
			case Enums.DealState.InPosition:
				if (Game.GameTime > time + 1000)
				{
					if (LSL.DEBUG)
					{
						UI.Notify("Transaction en cours");
					}
					dealState = Enums.DealState.DoingDeal;
					cPed.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_franklin", 8f, 3000, (AnimationFlags)16);
					Ped.Task.PlayAnimation("switch@franklin@002110_04_magd_3_weed_exchange", "002110_04_magd_3_weed_exchange_shopkeeper", 8f, 3000, (AnimationFlags)16);
					time = Game.GameTime;
				}
				break;
			case Enums.DealState.DoingDeal:
				if (Game.GameTime > time + 2000)
				{
					dealState = Enums.DealState.WalkingAway;
					Function.Call(unchecked((Hash)(-8213159594590722974L)), (cPed), ("GENERIC_THANKS"), ("SPEECH_PARAMS_STANDARD"));
					cPed.Task.WanderAround();
					Ped.Task.GuardCurrentPosition();
				}
				break;
			case Enums.DealState.WalkingAway:
				position = ((Entity)cPed).Position;
				if (position.DistanceTo(((Entity)Ped).Position) > 4f)
				{
					dealState = Enums.DealState.GetNear;
					if (LSL.DEBUG)
					{
						UI.Notify("Transaction terminée");
					}
					LsFunctions.RemovePedFromWorld(cPed, clearTask: false);
					((Entity)cPed).CurrentBlip.Remove();
					lPed = cPed;
					cPed = null;
				}
				break;
			}
		}
		else if ((Entity)(object)cPed != (Entity)null)
		{
			if (LSL.DEBUG)
			{
				UI.Notify("Client supprimé : trop éloigné du dealer");
			}
			LsFunctions.RemovePedFromWorld(cPed, clearTask: false);
			cPed = null;
			if (dealState != Enums.DealState.GetNear)
			{
				dealState = Enums.DealState.GetNear;
			}
		}
	}

	private void SetAbilities()
	{
		Function.Call(unchecked((Hash)(-4079649877180351064L)), (Ped), (100));
		Function.Call((Hash)5592521861259579479L, (Ped), (2));
		Ped.Accuracy = 100;
		Ped.ShootRate = 100;
		Ped.FiringPattern = unchecked((FiringPattern)(-957453492));
		Ped.CanWrithe = false;
	}

	public void SpawnPed()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		LsFunctions.DbugString("Spawning Ped at" + Pos.X + ", " + Pos.Y + ", " + Pos.Z);
		Ped = World.CreatePed(new Model(pHash), Pos);
		if ((Entity)(object)Ped != (Entity)null)
		{
			LsFunctions.DbugString("Ped spawned at " + ((Entity)Ped).Position.X + ", " + ((Entity)Ped).Position.Y + ", " + ((Entity)Ped).Position.Z);
			Function.Call((Hash)3327951495314425415L, (((Entity)Ped).Handle), (Game.Player.Character.CurrentPedGroup.Handle), (false));
			LsFunctions.SetImmuneToPlayer(Ped);
			LsFunctions.AddBlip((Entity)(object)Ped, (BlipColor)3, "Dealer", _friend: true, _edge: true);
			SetBlip();
			Ped.Armor = dArmour;
			GiveSavedWeapons();
			RemoveLocationBlip();
			((Entity)Ped).Health = 200;
			((Entity)Ped).IsPersistent = true;
			SetAbilities();
			Ped.RelationshipGroup = LSL.playerRelationship;
			Ped.Weapons.Select(Ped.Weapons.BestWeapon);
			Ped.CanSwitchWeapons = true;
			if (CurrentState == Enums.PdStates.FollowPlayer && Game.Player.Character.CurrentPedGroup != Ped.CurrentPedGroup && LSL.playerPos.DistanceTo(((Entity)Ped).Position) < 100f)
			{
				Game.Player.Character.CurrentPedGroup.Add(Ped, false);
				Function.Call((Hash)3327951495314425415L, (((Entity)Ped).Handle), (Game.Player.Character.CurrentPedGroup.Handle), (false));
			}
			if (pData.Count > 0)
			{
				LsFunctions.SetPedVariation(Ped, pData);
			}
		}
	}

	private bool isFollowing()
	{
		if (NewState == Enums.PdStates.FollowPlayer || NewState == Enums.PdStates.FollowingPlayer || CurrentState == Enums.PdStates.FollowPlayer || CurrentState == Enums.PdStates.FollowingPlayer)
		{
			return true;
		}
		return false;
	}

	private void GiveSavedWeapons()
	{
		LsFunctions.GivePedWeapons(Ped, dWeapons);
		Ped.Weapons.Select(Ped.Weapons.BestWeapon);
	}

	public void DespawnPed()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((Entity)(object)Ped != (Entity)null)
		{
			LsFunctions.NotImmuneToPlayer(Ped);
			if (LSL.DEBUG)
			{
				UI.Notify("Dealer retiré du monde");
			}
			Pos = ((Entity)Ped).Position;
			RemoveBlip();
			((Entity)Ped).Delete();
			Ped = null;
			CreateLocationBlip();
		}
	}

	public void SimulateSales(int _hours)
	{
		int nDealers = 0;
		Area pArea = LSL.areas.Find((Area a) => a.Name == this.area);
		if (pArea != null)
		{
			nDealers = pArea.Dealers();
		}
		int startMoney = Money;
		if (pArea == null)
		{
			if (LSL.zoneData.ContainsKey(this.area))
			{
				LSL.areas.Add(new Area(this.area, LsFunctions.LsDrugs(), LSL.zoneData[this.area].Item1));
			}
			else
			{
				UI.Notify("~r~LsLife - Impossible de déterminer le type du quartier : " + this.area);
			}
			if (LSL.DEBUG)
			{
				UI.Notify("~r~Quartier ajouté au dealer : " + this.area);
			}
		}
		if (pArea == null || pArea.Drugs.Count <= 0)
		{
			return;
		}
		Random rInt = new Random();
		for (int i = 0; i < _hours * 3; i++)
		{
			foreach (Drug drug in pArea.Drugs)
			{
				int rIntMax = (int)(10f * drug.Demand);
				if (nDealers > 0)
				{
					rIntMax = (int)(10f * (drug.Demand / (float)nDealers));
				}
				if (drug.Demand > 1f)
				{
					rIntMax = 10;
				}
				if (rInt.Next(0, 11 - rIntMax) > 3)
				{
					continue;
				}
				if (Drugs[drug.Name] > 0)
				{
					Area area = LSL.areas.Find((Area a) => a.Name == this.area);
					int modifier = (int)(LsFunctions.CurrentRepLvl(area.Reputation) * 0.5);
					int max = 7 + modifier;
					if (max <= 1)
					{
						max = 7;
					}
					int amountWanted = LSL.rnd.Next(1, max);
					if (Drugs[drug.Name] < amountWanted)
					{
						amountWanted = Drugs[drug.Name];
					}
					int moneyEarned = amountWanted * drug.PricePerG;
					Money += moneyEarned;
					float repEarned = (float)moneyEarned * 0.01f;
					LsFunctions.DbugString("dealer earned " + repEarned + " rep");
					area.Reputation += repEarned;
					Drugs[drug.Name] -= amountWanted;
					drug.Supplied = true;
					LsFunctions.DbugString("~r~Sold " + drug.Name + " * " + amountWanted);
				}
				else
				{
					drug.Supplied = false;
				}
			}
		}
		if (LSL.DEBUG)
		{
			if (startMoney != Money)
			{
				UI.Notify("Gains : " + LsFunctions.IntToMoney(Money - startMoney) + " $");
			}
			else
			{
				UI.Notify("Aucun gain");
			}
		}
	}

	public void GiveMoneyToPlayer()
	{
		Money -= Money / 100 * 20;
		Player player = Game.Player;
		player.Money += Money;
		Money = 0;
		LsFunctions.PlayTheSound();
	}

	private void PromtPlayerToTalk()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (CustomWorkerMenu.Visible || LSL.LsMenuPool.IsAnyMenuOpen() || LsFunctions.zeeClose() || ShotHandler.ShotOffers || LSL.PottentialWorker != null || !LSL.player.Character.IsStopped || !(LSL.playerPos.DistanceTo(((Entity)Ped).Position) < 2.5f) || !LsFunctions.IsCustomerNear())
		{
			return;
		}
		if ((float)Game.GameTime > LSL.displayHelpTimer + 5000f)
		{
			Function.Call((Hash)7023634693725934496L);
			LSL.displayHelpTimer = Game.GameTime;
			LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour parler à cette personne.");
		}
		if (Game.IsControlJustReleased(2, (Control)51))
		{
			Function.Call((Hash)7023634693725934496L);
			LSL.WorkerMainMenu.CurrentSelection = 3;
			LSL.DealerHandler.currentDealer = this;
			SetDrugDesc();
			SetDealerOrders();
			SetArmourButton();
			CustomWorkerMenu.Open();
		}
	}

	private void PromtToFollow()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!LSL.LsMenuPool.IsAnyMenuOpen() && LSL.player.Character.IsStopped && LSL.playerPos.DistanceTo(((Entity)Ped).Position) < 10f && LSL.player.Character.IsInVehicle() && Ped.CurrentPedGroup != LSL.player.Character.CurrentPedGroup && LsFunctions.IsCustomerNear())
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 5000f)
			{
				Function.Call((Hash)7023634693725934496L);
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_VEH_HORN~ pour que ton dealer te suive.");
			}
			if (Game.IsControlJustPressed(2, (Control)86))
			{
				Function.Call((Hash)7023634693725934496L);
				LSL.DealerHandler.currentDealer = this;
				NewState = Enums.PdStates.FollowPlayer;
			}
		}
	}

	private void SetDealerOrders()
	{
		LSL.wOrder.Items = Orders();
		LSL.wOrder.Index = 0;
	}

	private void SetArmourButton()
	{
		if (LSL.playerArmours == 0)
		{
			LSL.wArmour.Text = "Plus de gilet disponible";
		}
		else
		{
			LSL.wArmour.Text = "Donner un gilet";
		}
	}

	public void SetDrugDesc()
	{
		LsFunctions.SetWorkerMenuHealth(Ped);
		LSL.wWeed.Maximum = LSL.PlayerInventory["Weed"];
		LSL.wCrack.Maximum = LSL.PlayerInventory["Crack"];
		LSL.wCocaine.Maximum = LSL.PlayerInventory["Cocaine"];
		LSL.wWeed.Description = "Stock : " + LsFunctions.GramsToOz(Drugs["Weed"]) + " de cannabis.";
		LSL.wCrack.Description = "Stock : " + LsFunctions.GramsToOz(Drugs["Crack"]) + " de crack.";
		LSL.wCocaine.Description = "Stock : " + LsFunctions.GramsToOz(Drugs["Cocaine"]) + " de cocaïne.";
		int cMoney = Money - Money / 100 * 20;
		if (Money > 100)
		{
			LSL.wCollect.Description = "Le dealer a " + LsFunctions.IntToMoney(Money) + " $" + " ; il garde 20 %. À récupérer : " + LsFunctions.IntToMoney(cMoney) + " $" + ".";
		}
		else
		{
			LSL.wCollect.Description = "Le dealer n'a encore rien gagné";
		}
	}

	private bool playerNear()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Entity)Ped).Position;
		if (position.DistanceTo(LSL.playerPos) <= 5f)
		{
			return true;
		}
		return false;
	}

	private void CreateLocationBlip()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (blip == (Blip)null)
		{
			blip = World.CreateBlip(Pos);
			blip.IsShortRange = true;
			blip.Name = "Dealer Location";
		}
	}

	public void RemoveLocationBlip()
	{
		if (blip != (Blip)null)
		{
			blip.Remove();
			blip = null;
		}
	}

	private void SetBlip()
	{
		if (NewState == Enums.PdStates.FollowPlayer || CurrentState == Enums.PdStates.FollowPlayer)
		{
			((Entity)Ped).CurrentBlip.Color = (BlipColor)54;
		}
		else
		{
			((Entity)Ped).CurrentBlip.Color = (BlipColor)50;
		}
		((Entity)Ped).CurrentBlip.Name = "Dealer";
		((Entity)Ped).CurrentBlip.Scale = 0.5f;
		((Entity)Ped).CurrentBlip.IsShortRange = true;
	}

	private void RemoveBlip()
	{
		((Entity)Ped).CurrentBlip.Remove();
	}

	private void SetStartDrugs()
	{
		Drugs.Add("Weed", LSL.rnd.Next(0, 28));
		Drugs.Add("Crack", LSL.rnd.Next(0, 28));
		Drugs.Add("Cocaine", LSL.rnd.Next(0, 28));
	}
}
