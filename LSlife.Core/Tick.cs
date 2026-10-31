using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Control = GTA.Control;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.Gangs;
using LSlife.HeatBars;
using LSlife.OldCustomers;
using LSlife.Shots;


namespace LSlife;

public partial class LSL
{
	// Courte stabilisation du personnage apres chargement (le conflit Menyoo etait externe).
	private int phoneStartupPedHandle = -1;
	private int phoneStartupStableSince = -1;
	private const int PhoneStartupStableMs = 750;

	private string lastTickStage = "not-started";

	private void onTick(object sender, EventArgs e)
	{
		try
		{
			lastTickStage = "tick start";
			OnTickCore(sender, e);
		}
		catch (Exception ex)
		{
			LSLifeLog.Error("Tick", "Echec pendant : " + lastTickStage, ex);
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
			string details = "[LSLife] Derniere etape onTick : " + lastTickStage + Environment.NewLine + "Exception : " + ex + Environment.NewLine + trace;
			try
			{
				Directory.CreateDirectory(@"scripts\LSLife");
				File.AppendAllText(@"scripts\LSLife\LSLifeRuntimeErrors.log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + details + Environment.NewLine);
			}
			catch (Exception) { }
			throw;
		}
	}

	private void OnTickCore(object sender, EventArgs e)
	{
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected Obj, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5b: Expected I4, but got Unknown
		//IL_1cd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b26: Expected I4, but got Unknown
		//IL_1b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4a: Expected I4, but got Unknown
		//IL_1f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e73: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Expected Obj, but got Unknown
		//IL_2508: Unknown result type (might be due to invalid IL or missing references)
		//IL_250d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2516: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_24ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1983: Unknown result type (might be due to invalid IL or missing references)
		//IL_1988: Unknown result type (might be due to invalid IL or missing references)
		//IL_198c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_2534: Unknown result type (might be due to invalid IL or missing references)
		//IL_2543: Unknown result type (might be due to invalid IL or missing references)
		//IL_2548: Unknown result type (might be due to invalid IL or missing references)
		//IL_254d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2558: Unknown result type (might be due to invalid IL or missing references)
		//IL_2564: Unknown result type (might be due to invalid IL or missing references)
		//IL_2570: Unknown result type (might be due to invalid IL or missing references)
		//IL_25a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_25aa: Expected Obj, but got Unknown
		//IL_25c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_25c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_25cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_25d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_25f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_25fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2607: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2f: Expected I4, but got Unknown
		//IL_1c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c36: Expected I4, but got Unknown
		//IL_158a: Unknown result type (might be due to invalid IL or missing references)
		//IL_158f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1593: Unknown result type (might be due to invalid IL or missing references)
		//IL_2636: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c47: Unknown result type (might be due to invalid IL or missing references)
		//IL_2674: Unknown result type (might be due to invalid IL or missing references)
		//IL_2679: Unknown result type (might be due to invalid IL or missing references)
		//IL_2682: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c24: Unknown result type (might be due to invalid IL or missing references)
		//IL_26a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_26b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_2701: Unknown result type (might be due to invalid IL or missing references)
		//IL_2708: Expected Obj, but got Unknown
		//IL_2721: Unknown result type (might be due to invalid IL or missing references)
		//IL_2726: Unknown result type (might be due to invalid IL or missing references)
		//IL_2741: Unknown result type (might be due to invalid IL or missing references)
		//IL_275c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2761: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_23de: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_293b: Unknown result type (might be due to invalid IL or missing references)
		//IL_29a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2806: Unknown result type (might be due to invalid IL or missing references)
		//IL_280b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2810: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ae9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2af0: Invalid comparison between Unknown and I4
		//IL_2824: Unknown result type (might be due to invalid IL or missing references)
		//IL_2829: Unknown result type (might be due to invalid IL or missing references)
		//IL_282e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2875: Unknown result type (might be due to invalid IL or missing references)
		//IL_2898: Unknown result type (might be due to invalid IL or missing references)
		//IL_3822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1064: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Unknown result type (might be due to invalid IL or missing references)
		//IL_106d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1100: Unknown result type (might be due to invalid IL or missing references)
		//IL_1105: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b86: Unknown result type (might be due to invalid IL or missing references)
		//IL_119f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_117b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1181: Invalid comparison between Unknown and I4
		//IL_1113: Unknown result type (might be due to invalid IL or missing references)
		//IL_1118: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c31: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_3abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_30f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_31be: Unknown result type (might be due to invalid IL or missing references)
		//IL_31c5: Invalid comparison between Unknown and I4
		//IL_31d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fef: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_400d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4012: Unknown result type (might be due to invalid IL or missing references)
		// Un Ped existant et IsPlaying ne suffisent pas : le protagoniste peut
		// encore changer plusieurs fois pendant les premieres secondes.
		lastTickStage = "if (iFruit == null)";
		if (iFruit == null)
		{
			Ped currentPed = Game.Player != null ? Game.Player.Character : null;
			int currentHandle = currentPed != null && currentPed.Exists() &&
				Game.Player.IsPlaying ? currentPed.Handle : -1;
			if (currentHandle != phoneStartupPedHandle || currentHandle == -1)
			{
				phoneStartupPedHandle = currentHandle;
				phoneStartupStableSince = currentHandle == -1 ? -1 : Game.GameTime;
			}
			else if (phoneStartupStableSince >= 0 &&
				Game.GameTime - phoneStartupStableSince >= PhoneStartupStableMs)
			{
				// Publier les references uniquement apres l'enregistrement complet.
				var phone = new CustomiFruit();
				var zee = new iFruitContact("Zee");
				zee.Answered += LsFunctions.ContactAnswered;
				zee.DialTimeout = 1000;
				zee.Active = true;
				zee.Icon = ContactIcon.Blank;
				phone.Contacts.Add(zee);
				contactZee = zee;
				iFruit = phone;
			}
		}
		lastTickStage = "if (firstRun &&";
		if (firstRun && (Entity)(object)player.Character != (Entity)null)
		{
			if (File.Exists("scripts\\LSLife\\LSLifeAreas.xml"))
			{
				LoadAreaData();
			}
			GTA.UI.Notification.PostTicker("~g~LsLife: " + vNumber, false, false);
			GTA.UI.Notification.PostTicker("~g~LsLife: " + areas.Count + " zones chargées", false, false);
			lPeds = new LsLPeds();
			((GTA.RelationshipGroup)playerRelationship).SetRelationshipBetweenGroups(CustomerRel, (Relationship)3);
			((GTA.RelationshipGroup)playerRelationship).SetRelationshipBetweenGroups(rival_nutral, (Relationship)3);
			((GTA.RelationshipGroup)playerRelationship).SetRelationshipBetweenGroups(rival_enemy, (Relationship)5);
			((GTA.RelationshipGroup)rival_nutral).SetRelationshipBetweenGroups(CustomerRel, (Relationship)3);
			((GTA.RelationshipGroup)rival_nutral).SetRelationshipBetweenGroups(playerRelationship, (Relationship)3);
			((GTA.RelationshipGroup)rival_nutral).SetRelationshipBetweenGroups(rival_enemy, (Relationship)1);
			((GTA.RelationshipGroup)rival_enemy).SetRelationshipBetweenGroups(CustomerRel, (Relationship)1);
			((GTA.RelationshipGroup)rival_enemy).SetRelationshipBetweenGroups(playerRelationship, (Relationship)5);
			((GTA.RelationshipGroup)rival_enemy).SetRelationshipBetweenGroups(rival_nutral, (Relationship)1);
			((GTA.RelationshipGroup)CustomerRel).SetRelationshipBetweenGroups(playerRelationship, (Relationship)0);
			((GTA.RelationshipGroup)CustomerRel).SetRelationshipBetweenGroups(rival_nutral, (Relationship)0);
			((GTA.RelationshipGroup)CustomerRel).SetRelationshipBetweenGroups(rival_enemy, (Relationship)0);
			playerHeat = -100f;
			Function.Call((Hash)2750315038012726912L, (player.Character), (9), (0), (0), (0));
			playerHasBag = false;
			canSprint = true;
			playerArea = LsFunctions.GetZoneLabel(playerPos);
			if (areas.Where((Area a) => a.Name != playerArea).Count() == 0)
			{
				if (zoneData.ContainsKey(playerArea))
				{
					areas.Add(new Area(playerArea, LsFunctions.LsDrugs(), zoneData[playerArea].Item1));
				}
				else
				{
					GTA.UI.Notification.PostTicker("~r~LsLife - Impossible de déterminer le type du quartier : " + playerArea, false, false);
				}
			}
			Area cArea = areas.Find((Area a) => a.Name == playerArea);
			if (cArea != null)
			{
				areaIndex = areas.IndexOf(cArea);
			}
			firstRun = false;
		}
		lastTickStage = "if (DEBUG)";
		if (DEBUG)
		{
			string debugString = "0";
			if (Game.GameTime < areas[areaIndex].DemandBuff())
			{
				debugString = (areas[areaIndex].DemandBuff() - Game.GameTime).ToString();
			}
			myUIText = new GTA.UI.TextElement(debugString, new PointF(10, 10), 0.4f, Color.WhiteSmoke, GTA.UI.Font.ChaletLondon);
			myUIText.Draw();
		}
		lastTickStage = "if (Prison.Started";
		if (Prison.Started && Prison.CurrentState > Prison.PrisonStates.ApproachBarrier)
		{
			Function.Call(unchecked((Hash)(-7077668788463384353L)), ("restrictedareas"));
			Function.Call(unchecked((Hash)(-7077668788463384353L)), ("re_prison"));
		}
		// iFruit doit traiter le telephone avant que les menus personnalises
		// ne lisent ou ne neutralisent les controles GTA de cette frame.
		lastTickStage = "if (iFruit != null) iFruit.Update();";
		if (iFruit != null) iFruit.Update();
		// Les menus CustomMenu ne dependent plus de l'existence du pool historique.
		lastTickStage = "CustomZeeMenu.Tick();";
		CustomZeeMenu.Tick();
		CustomStashMenu.TickOncePerFrame();
		CustomCustomerMenu.TickOncePerFrame();
		CustomWeaponDealerMenu.TickOncePerFrame();
		CustomHireDealerMenu.TickOncePerFrame();
		CustomWorkerMenu.TickOncePerFrame();
		CustomHudOptionsMenu.TickOncePerFrame();
		if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_CONTROL_PRESSED, 2, 76) && GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 26))
		{
			DropDrugs.Drop();
		}
		Vector3 position;
		lastTickStage = "if ((Entity)(object)player.Character";
		if ((Entity)(object)player.Character != (Entity)null && ((Entity)player.Character).IsAlive && player.IsPlaying)
		{
			playerPos = ((Entity)player.Character).Position;
			if (DEBUG)
			{
				PedDebugger.OnTick();
			}
		lastTickStage = "if (DealerHandler != null)";
			if (DealerHandler != null)
			{
				DealerHandler.OnTick();
			}
		lastTickStage = "if (HouseHandler != null)";
			if (HouseHandler != null)
			{
				HouseHandler.Ontick();
			}
		lastTickStage = "if (ZeesDealers != null)";
			if (ZeesDealers != null)
			{
				ZeesDealers.OnTick();
			}
		lastTickStage = "HandlePotentialWorker();";
			HandlePotentialWorker();
		lastTickStage = "Hud.DebugEnabled = AreaDebug;";
			Hud.DebugEnabled = AreaDebug;
			if (showInventory) Hud.Draw();
			else Hud.Hide();
			if (AreaDebug && !CustomMenuShared.IsAnyGameplayMenuOpen()
				&& !CustomZeeMenu.Visible && !CustomHudOptionsMenu.Visible
				&& !CustomStashMenu.Visible && !CustomCustomerMenu.Visible
				&& !CustomWeaponDealerMenu.Visible && !CustomHireDealerMenu.Visible
				&& !CustomWorkerMenu.Visible)
			{
				lastTickStage = "debug overlay";
				areaInfoUI();
				ZeeInfoUI();
				PlayerUI();
				GangUI();
			}

			if (pigs.Count > 0 && pigs[0].AskPlayerToComply)
			{
				if ((float)Game.GameTime > displayHelpTimer + 3000f && !CustomMenuShared.IsAnyGameplayMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("~INPUT_CONTEXT~ pour obtempérer.");
				}
				if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 51))
				{
					PlayerComplied = true;
					TaskSequence giveUP = new TaskSequence();
					if ((Entity)(object)player.Character.CurrentVehicle != (Entity)null)
					{
						giveUP.AddTask.LeaveVehicle();
					}
					giveUP.AddTask.TurnTo(((Entity)pigs[0].Ped).ForwardVector + playerPos, 1000);
					giveUP.AddTask.HandsUp(20000);
					player.Character.Task.PerformSequence(giveUP);
				}
			}
		lastTickStage = "HandleAreaDealers();";
			HandleAreaDealers();
			if (!CustomMenuShared.IsAnyGameplayMenuOpen() && (Entity)(object)weaponDealer1 != (Entity)null && readyToDeal && GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 51))
			{
				CustomWeaponDealerMenu.Open();
			}
		lastTickStage = "if (pStashVehicle != null)";
			if (pStashVehicle != null)
			{
				pStashVehicle.OnTick();
			}
		lastTickStage = "if ((float)Game.GameTime > ratTimer";
			if ((float)Game.GameTime > ratTimer + 1000f)
			{
				if (isDealing)
				{
					timeDealing = Game.GameTime - timeStartedDealing;
				}
				else
				{
					timeDealing = 0;
				}
				thisStreet = World.GetStreetName(new Vector2(playerPos.X, playerPos.Y));
				ratTimer = Game.GameTime;
				if (thisStreet != lastStreet)
				{
					lastStreet = thisStreet;
					if (playerHeat - 10f >= -100f)
					{
						playerHeat -= 10f;
					}
					else
					{
						playerHeat = -100f;
					}
					if (isDealing)
					{
						timeStartedDealing = Game.GameTime;
					}
				}
				else if ((float)Game.GameTime > lastTimeDealDone + 10000f)
				{
					if (playerHeat > -100f && player.Wanted.WantedLevel == 0)
					{
						if (isDealing)
						{
							if (playerHeat < 100f)
							{
								playerHeat += Math.Min(1f, timeDealing / 360000f);
							}
							if (playerHeat > 100f)
							{
								playerHeat = 100f;
							}
						}
						float repMod = (float)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation) * 0.1f;
						if (repMod < 0f)
						{
							repMod = 0f;
						}
						LsFunctions.DbugString(repMod.ToString() ?? "");
						playerHeat -= 1f + repMod;
						if (playerHeat < -100f)
						{
							playerHeat = -100f;
						}
					}
				}
				else if (playerHeat < 100f)
				{
					float heatThisTick = (float)((double)(timeDealing / 180000f) + ((double)areas[areaIndex].Heat * 0.1 + (double)areas[areaIndex].CopPresance) * 0.5 + (double)PedsCanSeeDeal * 0.2 - Math.Min(10.0, LsFunctions.CurrentRepLvl((int)areas[areaIndex].Reputation)));
					heatThisTick = Math.Max(heatThisTick, 0.5f);
					heatThisTick = Math.Min(6f, heatThisTick);
					heatThisTick *= 0.25f;
					float newRatChance = playerHeat + heatThisTick;
					if (newRatChance > 100f)
					{
						playerHeat = 100f;
					}
					else if (newRatChance < -100f)
					{
						playerHeat = -100f;
					}
					else
					{
						playerHeat = newRatChance;
					}
					float repMod2 = (float)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation) * 0.1f;
					float mod = 1f;
					if (IsNight())
					{
						mod = 0.5f;
					}
					mod = (float)PedsCanSeeDeal * mod;
					if (mod < repMod2)
					{
						repMod2 = mod;
					}
					mod -= repMod2;
					if (mod < 0f)
					{
						mod = 0f;
					}
					areas[areaIndex].Heat += mod * 0.25f;
					if (areas[areaIndex].Heat > 1000f)
					{
						areas[areaIndex].Heat = 1000f;
					}
				}
			}
			HandleOldCutomers();
			if (isDealing)
			{
				if (removedStuff)
				{
					removedStuff = false;
				}
				if (customers.Count < 5 && ShotHandler.shots.Count < 1 && (float)Game.GameTime > lastTimePedSelected + 1000f)
				{
					lastTimePedSelected = Game.GameTime;
					if (rnd.Next(0, 100) > 60 && areas[areaIndex].NeedCustomer())
					{
						if (HouseHandler.CurrentHouse != null && HouseHandler.CurrentHouse.inside)
						{
							foreach (Drug drug in areas[areaIndex].Drugs)
							{
								if (customers.Count >= 3)
								{
									break;
								}
								switch (drug.Name)
								{
								case "Weed":
									if (!sellingWeed)
									{
										drug.Supplied = false;
										drug.SelectCustomer = false;
									}
									else
									{
										if (!drug.SelectCustomer)
										{
											break;
										}
										Random random2 = new Random();
										int rIntMax2 = (int)(10f * drug.Demand);
										if (drug.Demand > 1f)
										{
											rIntMax2 = 10;
										}
										if (random2.Next(0, 15 - rIntMax2) == 0)
										{
											PedHash hash2 = unchecked((PedHash)(-1954728090));
											if (LsFunctions.cHashes.Count > 0)
											{
												hash2 = LsFunctions.cHashes[rnd.Next(0, LsFunctions.cHashes.Count)];
											}
											Ped cPed2 = Ped.Create(LsFunctions.RequestModel(hash2), HouseHandler.CurrentHouse.exitPos);
											if ((Entity)(object)cPed2 != (Entity)null)
											{
												customers.Add(new Customer(cPed2, playerPos, farAway: false, null, drug, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
											}
										}
									}
									break;
								case "Crack":
									if (!sellingCrack)
									{
										drug.Supplied = false;
										drug.SelectCustomer = false;
									}
									else
									{
										if (!drug.SelectCustomer)
										{
											break;
										}
										Random random3 = new Random();
										int rIntMax3 = (int)(10f * drug.Demand);
										if (drug.Demand > 1f)
										{
											rIntMax3 = 10;
										}
										if (random3.Next(0, 15 - rIntMax3) == 0)
										{
											PedHash hash3 = unchecked((PedHash)(-1954728090));
											if (LsFunctions.cHashes.Count > 0)
											{
												hash3 = LsFunctions.cHashes[rnd.Next(0, LsFunctions.cHashes.Count)];
											}
											Ped cPed3 = Ped.Create(LsFunctions.RequestModel(hash3), HouseHandler.CurrentHouse.exitPos);
											if ((Entity)(object)cPed3 != (Entity)null)
											{
												customers.Add(new Customer(cPed3, playerPos, farAway: false, null, drug, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
											}
										}
									}
									break;
								case "Cocaine":
									if (!sellingCocaine)
									{
										drug.Supplied = false;
										drug.SelectCustomer = false;
									}
									else
									{
										if (!drug.SelectCustomer)
										{
											break;
										}
										Random random = new Random();
										int rIntMax = (int)(10f * drug.Demand);
										if (drug.Demand > 1f)
										{
											rIntMax = 10;
										}
										if (random.Next(0, 15 - rIntMax) == 0)
										{
											PedHash hash = unchecked((PedHash)(-1954728090));
											if (LsFunctions.cHashes.Count > 0)
											{
												hash = LsFunctions.cHashes[rnd.Next(0, LsFunctions.cHashes.Count)];
											}
											Ped cPed = Ped.Create(LsFunctions.RequestModel(hash), HouseHandler.CurrentHouse.exitPos);
											if ((Entity)(object)cPed != (Entity)null)
											{
												customers.Add(new Customer(cPed, playerPos, farAway: false, null, drug, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
											}
										}
									}
									break;
								}
							}
						}
						else
						{
							List<Ped> peds = new List<Ped>();
							foreach (Ped p in lPeds.GetPeds)
							{
								if (!CheckIfPedUsed(p, _allowPolice: true, _allowVehicle: true))
								{
									peds.Add(p);
								}
							}
							if (peds.Count > 0)
							{
								areas[areaIndex].GetCustomer(peds);
								foreach (Drug drug2 in areas[areaIndex].Drugs)
								{
									bool chooseCustomer = true;
									switch (drug2.Name)
									{
									case "Weed":
										if (!sellingWeed)
										{
											drug2.Supplied = false;
											chooseCustomer = false;
											drug2.SelectCustomer = false;
											drug2.Customer = null;
										}
										break;
									case "Crack":
										if (!sellingCrack)
										{
											drug2.Supplied = false;
											chooseCustomer = false;
											drug2.SelectCustomer = false;
											drug2.Customer = null;
										}
										break;
									case "Cocaine":
										if (!sellingCocaine)
										{
											drug2.Supplied = false;
											chooseCustomer = false;
											drug2.SelectCustomer = false;
											drug2.Customer = null;
										}
										break;
									}
									if (!chooseCustomer || !drug2.SelectCustomer)
									{
										continue;
									}
									Ped p2 = drug2.Customer;
									drug2.Customer = null;
									drug2.SelectCustomer = false;
									if ((Entity)(object)p2 != (Entity)null)
									{
										if (p2.IsInVehicle() && (Entity)(object)p2.CurrentVehicle.Driver == (Entity)(object)p2)
										{
											Vehicle vehicle = p2.CurrentVehicle;
											customers.Add(new Customer(p2, playerPos, farAway: false, vehicle, drug2, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
										}
										else
										{
											customers.Add(new Customer(p2, playerPos, farAway: false, null, drug2, (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation)));
										}
									}
								}
							}
						}
					}
				}
				if (customers.Count > 0)
				{
					if (Game.GameTime > lastCustomerCheckTimer + 370)
					{
						foreach (Customer c in customers.ToList())
						{
							if (c == null)
							{
								continue;
							}
							if (((Entity)c.Ped).Exists() && ((Entity)c.Ped).IsAlive && !c.Ped.IsPlayer)
							{
								position = ((Entity)c.Ped).Position;
								if (position.DistanceTo(playerPos) > 80f)
								{
									LsFunctions.DbugString("Client supprimé : hors de portée");
									customers.Remove(c);
									LsFunctions.RemovePedFromWorld(c.Ped, clearTask: true);
									break;
								}
								if (!(c.Ped.PedGroup != player.Character.PedGroup))
								{
									continue;
								}
								position = ((Entity)c.Ped).Position;
								if (position.DistanceTo(playerPos) < 3f)
								{
									c.Ped.Task.ClearAll();
									Function.Call(unchecked((Hash)(-6974808092771143243L)), (((Entity)c.Ped).Handle), (playerGroup));
									Function.Call(unchecked((Hash)(-8213159594590722974L)), (c.Ped), ("GENERIC_HI"), ("SPEECH_PARAMS_STANDARD"));
									continue;
								}
								if (c.Ped.IsInVehicle())
								{
									if (c.Target != World.GetNextPositionOnStreet(playerPos))
									{
										c.Target = World.GetNextPositionOnStreet(playerPos);
										c.Ped.Task.PerformSequence(PerformTaskSeq(2, c.Ped.CurrentVehicle, c.Target));
									}
									lastCustomerCheckTimer = Game.GameTime;
									continue;
								}
								if ((Entity)(object)c.Ped.LastVehicle != (Entity)null && (int)c.Ped.LastVehicle.LockStatus != 3)
								{
									c.Ped.LastVehicle.LockStatus = (VehicleLockStatus)3;
								}
								Vector3 playerForward = ((Entity)player.Character).ForwardVector;
								Vector3 playerRight = ((Entity)player.Character).RightVector;
								if (player.Character.IsInVehicle())
								{
									MoveCustomer(c, playerPos + playerRight * 2f);
								}
								else
								{
									MoveCustomer(c, playerPos + playerForward * 2f);
								}
								lastCustomerCheckTimer = Game.GameTime;
							}
							else
							{
								customers.Remove(c);
								LsFunctions.RemovePedFromWorld(c.Ped, clearTask: true);
							}
						}
					}
					Customer nCustomer = null;
					nCustomer = ((!player.Character.IsInVehicle()) ? FindClosestCustomer() : FindClosestCustomerInVehicle());
					if (nCustomer != null && !CustomMenuShared.IsAnyGameplayMenuOpen())
					{
						if (player.Character.IsInVehicle() && nCustomer.Ped.IsInVehicle() && player.Character.CurrentVehicle.Speed < 1f)
						{
							if ((float)Game.GameTime > displayHelpTimer + 3000f && !ShotHandler.ShotOffers && PottentialWorker == null)
							{
								Function.Call((Hash)7023634693725934496L);
								displayHelpTimer = Game.GameTime;
								LsFunctions.DisplayHelpText("~INPUT_SPRINT~ pour vendre depuis le véhicule.");
							}
							if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 21))
							{
								Function.Call((Hash)7023634693725934496L);
								customerSellTo = nCustomer;
								bool hasStock = PlayerInventory[customerSellTo.Drug.Name] >= customerSellTo.amount;
								if (!hasStock)
								{
									LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
								}
								CustomCustomerMenu.Open();
							}
						}
						if (!player.Character.IsInVehicle() && player.Character.IsStopped && !player.Character.IsInCombat)
						{
							if ((float)Game.GameTime > displayHelpTimer + 3000f && !ShotHandler.ShotOffers && PottentialWorker == null)
							{
								Function.Call((Hash)7023634693725934496L);
								displayHelpTimer = Game.GameTime;
								LsFunctions.DisplayHelpText("Appuie sur ~INPUT_SPRINT~ pour vendre à cette personne.");
							}
							if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 21) && !CustomMenuShared.IsAnyGameplayMenuOpen())
							{
								Function.Call((Hash)7023634693725934496L);
								customerSellTo = nCustomer;
								bool hasStock = PlayerInventory[customerSellTo.Drug.Name] >= customerSellTo.amount;
								if (!hasStock)
								{
									LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
								}
								CustomCustomerMenu.Open();
							}
						}
					}
				}
			}
		lastTickStage = "vehicle-dealer processing";
			if (wepDealer != null)
			{
				wepDealer.SetSpeed();
				if (!player.Character.IsInVehicle())
				{
					position = ((Entity)wepDealer.vehicle).Position;
					if (position.DistanceTo(playerPos) < 3f)
					{
						if ((float)Game.GameTime > displayHelpTimer + 3000f)
						{
							Function.Call((Hash)7023634693725934496L);
							displayHelpTimer = Game.GameTime;
							LsFunctions.DisplayHelpText("Appuie sur ~INPUT_VEH_EXIT~ pour entrer.");
						}
						if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 75))
						{
							Function.Call((Hash)7023634693725934496L);
							player.Character.Task.EnterVehicle(wepDealer.vehicle, (VehicleSeat)0);
						}
					}
				}
				else if (player.Character.IsInVehicle() && ((Entity)player.Character.CurrentVehicle).Handle == ((Entity)wepDealer.vehicle).Handle)
				{
					if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 52))
					{
						player.Character.Task.LeaveVehicle(player.Character.CurrentVehicle, true);
						Script.Wait(5500);
						wepDealer.ped.CurrentVehicle.SoundHorn(15);
						Script.Wait(15);
						wepDealer.ped.CurrentVehicle.SoundHorn(12);
						((Entity)wepDealer.ped).IsPersistent = false;
						((Entity)wepDealer.vehicle).IsPersistent = false;
						((Entity)wepDealer.vehicle).AttachedBlip.Delete();
						Function.Call((Hash)5188501456062479616L, (((Entity)wepDealer.ped).Handle), (((Entity)wepDealer.vehicle).Handle), (20f), (786603));
						wepDealer = null;
					}
					if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, 51))
					{
						if (wepDealer.state == Dealer.States.Driving)
						{
							wepDealer.state = Dealer.States.Faster;
						}
						if (wepDealer.state == Dealer.States.Fast)
						{
							wepDealer.state = Dealer.States.Slower;
						}
					}
				}
			}
		lastTickStage = "ShotHandler.Tick";
		lastTickStage = "shot handler";
			ShotHandler.Tick();
		}
		lastTickStage = "10-second maintenance";
		if (Game.GameTime > oneSecDelay + 10001)
		{
			oneSecDelay = Game.GameTime;
			if (player.IsPlaying)
			{
				if (!dealer1Angry && (double)dealer1Debt > (double)LsFunctions.zeeDebtCap() + (double)LsFunctions.zeeDebtCap() * 0.5)
				{
					if (DEBUG)
					{
						GTA.UI.Notification.PostTicker("dealer1 angry", false, false);
					}
					dealer1Angry = true;
				}
				if (driveByHandler.AmountOfDriveBy(_agg: true) == 0)
				{
					if (isDealing && areas[areaIndex].GangPresance > 4f && timeDealing > 240000 && rnd.Next(0, 60) == 0)
					{
						if (DEBUG)
						{
							GTA.UI.Notification.PostTicker("Attaque en voiture dans un quartier défavorisé", false, false);
						}
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
					if (dealer1Angry && driveByHandler.AmountOfDriveBy(_agg: true) == 0 && (Entity)(object)drugDealer1 == (Entity)null && rnd.Next(0, 10) == 0)
					{
						if (DEBUG)
						{
							GTA.UI.Notification.PostTicker("dealer1 drive-by", false, false);
						}
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
						LsFunctions.TextMsg("ZEE", "Dette", "Rembourse ta dette et je les rappellerai.");
					}
				}
				if (entitiesCleanUp.Count > 0)
				{
			lastTickStage = "entity cleanup loop";
				foreach (Entity ent in entitiesCleanUp.ToList())
					{
						if (ent == (Entity)null || !ent.Exists())
						{
							entitiesCleanUp.Remove(ent);
						}
						else
						{
							if (!ent.Exists())
							{
								continue;
							}
							if (ent.AttachedBlip != null && ent.AttachedBlip.Exists())
							{
								ent.AttachedBlip.Delete();
							}
							if (ent == (Entity)(object)attacker)
							{
								entitiesCleanUp.Remove(ent);
								if (DEBUG)
								{
									GTA.UI.Notification.PostTicker("Assaillant supprimé après l'arrêt de l'attaque", false, false);
								}
								continue;
							}
							position = ent.Position;
							if (position.DistanceTo(playerPos) > 200f && !ent.IsOnScreen)
							{
								entitiesCleanUp.Remove(ent);
								if (DEBUG)
								{
									GTA.UI.Notification.PostTicker("Delete:" + (object)ent, false, false);
								}
								ent.Delete();
							}
						}
					}
				}
			}
		}
		lastTickStage = "area-check preparation";
		Model model;
		lastTickStage = "area-check";
		if (Game.GameTime > areaCheckTimer + 999)
		{
			newWepAmount = 0;
		lastTickStage = "weapon scan";
			foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
			{
				if (Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (player.Character), ((int)weapon)))
				{
					newWepAmount++;
					if (newWepAmount > playerWeapons.Count)
					{
						break;
					}
				}
			}
			if (newWepAmount != playerWeps.Count)
			{
				if (DEBUG)
				{
					GTA.UI.Notification.PostTicker("Changement d'arme du joueur détecté", false, false);
				}
				playerWeps.Clear();
				playerWeapons.Clear();
				int i = 0;
				foreach (WeaponHash weapon2 in Enum.GetValues(typeof(WeaponHash)))
				{
					bool flag = Function.Call<bool>(unchecked((Hash)(-8220001501650343748L)), (player.Character), ((int)weapon2));
					int ammunitionInWeapon = Function.Call<int>((Hash)97480644549409105L, (player.Character), ((int)weapon2));
					if (!flag)
					{
						continue;
					}
					XElement parentElement = new XElement("weapon");
					parentElement.SetAttributeValue("id", i);
					XElement weaponNameElement = new XElement("HASH", (object)weapon2);
					if (DEBUG)
					{
						GTA.UI.Notification.PostTicker("Arme ajoutée : " + ((object)weapon2/*cast due to constrained. prefix*/).ToString() + " (liste playerWeapons).", false, false);
					}
					XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
					parentElement.Add(weaponNameElement);
					parentElement.Add(weaponAmmoElement);
					foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
					{
						if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (player.Character), ((int)weapon2), ((int)component.ComponentHash)))
						{
							XElement weaponComponentElement = new XElement("COMPONENT", (object)component);
							parentElement.Add(weaponComponentElement);
						}
					}
					playerWeapons.Add(parentElement);
					playerWeps.Add(((object)weapon2/*cast due to constrained. prefix*/).ToString());
					i++;
				}
			}
			areaCheckTimer = Game.GameTime;
		lastTickStage = "zone detection";
			string newArea = LsFunctions.GetZoneLabel(playerPos);
		lastTickStage = "existing area dealer validation";
			if (aDealer != null && playerArea != aDealer.Area && (aDealer.dPed == null || !aDealer.dPed.Exists() || playerPos.DistanceTo(((Entity)aDealer.dPed).Position) > 300f))
			{
				if ((Entity)(object)aDealer.cPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: false);
				}
				LsFunctions.RemovePedFromWorld(aDealer.dPed, clearTask: false);
				aDealer = null;
				LSLifeLog.Info("Dealer", "Ancien dealer retire apres changement de quartier");
			}
		lastTickStage = "area changed";
		lastTickStage = "zone transition";
			if (playerArea != newArea)
			{
				LSLifeLog.Info("Quartier", "Transition : " + playerArea + " -> " + newArea);
				playerArea = newArea;
				if (areas.FindAll((Area a) => a.Name == playerArea).Count == 0)
				{
					if (zoneData.ContainsKey(playerArea))
					{
						areas.Add(new Area(playerArea, LsFunctions.LsDrugs(), zoneData[playerArea].Item1));
					}
					else
					{
						GTA.UI.Notification.PostTicker("~r~LsLife - Impossible de déterminer le type du quartier : " + playerArea, false, false);
					}
					int aIndex = 0;
					foreach (Area area2 in areas)
					{
						if (area2.Name == playerArea)
						{
							areaIndex = aIndex;
							break;
						}
						aIndex++;
					}
				}
				else
				{
					string area1 = ((HouseHandler.CurrentHouse == null || !HouseHandler.CurrentHouse.inside) ? playerArea : LsFunctions.GetZoneLabel(HouseHandler.CurrentHouse.entrancePos));
					int aIndex2 = 0;
					foreach (Area area3 in areas)
					{
						if (area3.Name == area1)
						{
							areaIndex = aIndex2;
							break;
						}
						aIndex2++;
					}
				}
			}
			if (aDealer == null && areas.Count > areaIndex && areas[areaIndex].CanSpawnDealer())
			{
				lastTickStage = "area dealer spawn";
				LsFunctions.DbugString("demandBuff 0");
				if (areas[areaIndex].GangPresance > 0f && (HouseHandler == null || HouseHandler.Houses == null || HouseHandler.Houses.Count == 0 || areas[areaIndex].Name != LsFunctions.GetZoneLabel(HouseHandler.Houses[0].entrancePos)))
				{
		lastTickStage = "area dealer ped selection";
					List<Ped> aPeds = (lPeds != null && lPeds.GetPeds != null ? lPeds.GetPeds : new List<Ped>()).FindAll((Ped r) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						//IL_002f: Unknown result type (might be due to invalid IL or missing references)
						if (LsFunctions.GetZoneLabel(((Entity)r).Position) == areas[areaIndex].Name)
						{
							Vector3 position2 = ((Entity)r).Position;
							return position2.DistanceTo(playerPos) > 20f;
						}
						return false;
					});
					if (aPeds.Count > 0)
					{
						foreach (Ped p3 in aPeds)
						{
							if (!CheckIfPedUsed(p3, _allowPolice: false, _allowVehicle: false))
							{
								aDealer = new AreaDealer(p3);
								LSLifeLog.Info("Dealer", "Dealer de quartier cree : " + playerArea + " / ped=" + p3.Handle);
								aDealer.UpdateRecruitmentDebugMarker(DEBUG);
								if (DEBUG)
								{
									model = ((Entity)p3).Model;
									GTA.UI.Notification.PostTicker("Dealer de quartier généré avec ~g~" + ((object)model/*cast due to constrained. prefix*/).ToString(), false, false);
								}
								break;
							}
						}
					}
				}
			}
			else
			{
				LsFunctions.DbugString("Cant spawn dealer dead :" + areas[areaIndex].DealerKilled);
			}
		}
		lastTickStage = "if (aDealer != null)";
		if (aDealer != null) aDealer.UpdateRecruitmentDebugMarker(DEBUG);
		if (Game.GameTime <= SlowTickTime + 375)
		{
			return;
		}
		SlowTickTime = Game.GameTime;
		player = Game.Player;
		playerGroup = player.Character.PedGroup;
		if (Function.Call<bool>((Hash)6160435850588389544L, (lsDebug)))
		{
			DEBUG = !DEBUG;
		}
		else if (Function.Call<bool>((Hash)6160435850588389544L, (areaDebug)))
		{
			AreaDebug = !AreaDebug;
			GTA.UI.Screen.ShowSubtitle(AreaDebug ? "Diagnostic quartier active" : "Diagnostic quartier desactive", 1800);
		}
		else if (Function.Call<bool>((Hash)6160435850588389544L, (spawnP)))
		{
			LsFunctions.SpawnDriveBy(player.Character, _agressive: false);
		}
		lastTickStage = "if (lPeds != null)";
		if (lPeds != null)
		{
			lPeds.Ontick();
		}
		if (DealerHandler != null)
		{
			DealerHandler?.OnSlowTick();
		}
		if (pickupHandler != null)
		{
			pickupHandler?.OnSlowTick();
		}
		if (driveByHandler != null)
		{
			driveByHandler.OnTick();
		}
		if (ZeesDealers != null)
		{
			ZeesDealers.OnSlowTick();
			if (ZeesDealers.Kill)
			{
				ZeesDealers = null;
			}
		}
		if (aDealer != null)
		{
			if ((Entity)(object)aDealer.cPed != (Entity)null && ((Entity)aDealer.cPed).IsDead)
			{
				Ped dKiller = null;
				if (GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, aDealer.cPed.Handle)) is Ped)
				{
					Entity killer = GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, aDealer.cPed.Handle));
					dKiller = (Ped)(object)((killer is Ped) ? killer : null);
				}
				if ((Entity)(object)dKiller != (Entity)null && dKiller.PedGroup == player.Character.PedGroup)
				{
					aDealer.NewState = Enums.DStates.AttackPlayer;
					if (driveByHandler.AmountOfDriveBy(_agg: true) == 0 && rnd.Next(0, 3) == 0)
					{
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
				}
				LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: false);
				aDealer.cPed = null;
			}
			if (aDealer != null && ((Entity)aDealer.dPed).IsDead)
			{
				areas[areaIndex].KillDealer();
				if (aDealer.dPed.WasKilledByTakedown && LsFunctions.CanSee((Entity)(object)aDealer.dPed, (Entity)(object)player.Character) && rnd.Next(0, 3) == 0)
				{
					if (driveByHandler.AmountOfDriveBy(_agg: true) == 0 && rnd.Next(0, 3) == 0)
					{
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
					if (DEBUG)
					{
						GTA.UI.Notification.PostTicker("Tu as été vu en train de tuer un dealer.", false, false);
					}
				}
				Ped dKiller2 = null;
				if (GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, aDealer.cPed.Handle)) is Ped)
				{
					Entity killer2 = GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, aDealer.cPed.Handle));
					dKiller2 = (Ped)(object)((killer2 is Ped) ? killer2 : null);
				}
				if ((Entity)(object)dKiller2 != (Entity)null && dKiller2.PedGroup == player.Character.PedGroup)
				{
					int drugCap = 28 * ((int)areas[areaIndex].GangPresance + 1);
					if (drugCap < 28)
					{
						drugCap = 28;
					}
					int moneyCap = 1000 * ((int)areas[areaIndex].GangPresance + 1);
					if (moneyCap < 1000)
					{
						moneyCap = 1000;
					}
					pickupHandler.AddBag(new DroppedBag("A ~r~rival dealer~s~ dropped a ~g~bag.", LsFunctions.RanRange(1, drugCap), LsFunctions.RanRange(1, drugCap), LsFunctions.RanRange(1, drugCap), LsFunctions.RanRange(100, moneyCap), LsFunctions.PedWeapons(aDealer.dPed), ((Entity)aDealer.dPed).Position));
				}
				if ((Entity)(object)aDealer.cPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: true);
					aDealer.cPed = null;
				}
				if ((Entity)(object)aDealer.dPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.dPed, clearTask: true);
					aDealer.dPed = null;
				}
				aDealer = null;
			}
			if (aDealer != null && (Entity)(object)aDealer.dPed != (Entity)null && ((Entity)aDealer.dPed).Exists() && ((Entity)aDealer.dPed).IsAlive)
			{
				aDealer.DoState();
				aDealer.CheckState();
			}
		}
		if (wepDealer != null && ((Entity)wepDealer.ped).IsAlive)
		{
			if ((Entity)(object)player.Character.CurrentVehicle != (Entity)(object)wepDealer.vehicle)
			{
				position = ((Entity)wepDealer.ped).Position;
				if (position.DistanceTo(playerPos) < 10f)
				{
					goto IL_252a;
				}
			}
			position = ((Entity)wepDealer.ped).Position;
			if (position.DistanceTo(wepDealer.destination) < 10f)
			{
				goto IL_252a;
			}
			goto IL_25d6;
		}
		goto IL_29de;
		IL_252a:
		Vector3 pos = ((Entity)wepDealer.vehicle).Position + ((Entity)wepDealer.vehicle).Velocity;
		int NodeID = Function.Call<int>((Hash)2510518586829603349L, (pos.X), (pos.Y), (pos.Z), (1), (1), (1f), (0f));
		OutputArgument outArgA = new OutputArgument();
		Function.Call((Hash)8084282276450675138L, (NodeID), outArgA);
		Vector3 parkPos = outArgA.GetResult<Vector3>();
		wepDealer.destination = parkPos;
		goto IL_25d6;
		IL_29de:
		if (dealer1Angry && dealer1Debt < 170000)
		{
			dealer1Angry = false;
		}
		lastTickStage = "LsFunctions.DateTimeUpdate();";
		LsFunctions.DateTimeUpdate();
		if (attackerPickupSpawned && !attackerPickup.ObjectExists())
		{
			attackerPickup.Delete();
			attackerPickupSpawned = false;
			WeaponHash weaponHash = unchecked((WeaponHash)(-1569615261));
			int ammo = 0;
			WeaponHash currentWep = player.Character.Weapons.Current.Hash;
			foreach (XElement element in attackerWeapons)
			{
				if (!(element.Name == "weapon"))
				{
					continue;
				}
				foreach (XElement wep in element.Elements())
				{
					if (wep.Name == "HASH")
					{
						Enum.TryParse<WeaponHash>(wep.Value, out weaponHash);
					}
					if (wep.Name == "AMMO")
					{
						int.TryParse(wep.Value, out ammo);
					}
					if ((int)weaponHash == -1569615261)
					{
						continue;
					}
					if (ammo < 100)
					{
						ammo = 100;
					}
					player.Character.Weapons.Give(weaponHash, ammo, true, true);
					player.Character.Weapons.Select(weaponHash, true);
					foreach (XElement c2 in element.Elements())
					{
						if (c2.Name == "COMPONENT")
						{
							Enum.TryParse<WeaponComponentHash>(c2.Value, out WeaponComponentHash wepcomp);
							player.Character.Weapons.Current.Components[wepcomp].Active = true;
						}
					}
					if (DEBUG)
					{
						GTA.UI.Notification.PostTicker("Munitions données pour " + ((object)weaponHash/*cast due to constrained. prefix*/).ToString() + " : " + ammo, false, false);
					}
				}
			}
			attackerWeapons.Clear();
			player.Character.Weapons.Select(currentWep);
			PlayerInventory["Weed"] += attackerWeed;
			PlayerInventory["Crack"] += attackerCrack;
			PlayerInventory["Cocaine"] += attackerCocaine;
			Player val = player;
			val.Money += attackerMoney;
			attackerWeed = 0;
			attackerCrack = 0;
			attackerCocaine = 0;
			LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
			foreach (XElement element2 in LSLifeSave.Descendants())
			{
				if (!(element2.Name == "playerInventory"))
				{
					continue;
				}
				foreach (XElement drug3 in element2.Elements())
				{
					if (drug3.Name == "weed")
					{
						drug3.Value = PlayerInventory["Weed"].ToString();
					}
					if (drug3.Name == "crack")
					{
						drug3.Value = PlayerInventory["Crack"].ToString();
					}
					if (drug3.Name == "cocain")
					{
						drug3.Value = PlayerInventory["Cocaine"].ToString();
					}
				}
			}
			LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
		}
		if (areas.Count > 0 && playerWanted && areas[areaIndex].Heat < 5000f)
		{
			Area areaToAddHeat = ((HouseHandler.CurrentHouse == null || !HouseHandler.CurrentHouse.inside) ? areas[areaIndex] : areas.Find((Area a) =>
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				return a.Name == LsFunctions.GetZoneLabel(HouseHandler.CurrentHouse.entrancePos);
			}));
			areaToAddHeat.Heat += (float)player.Wanted.WantedLevel * 0.05f;
		}
		if (player.IsPlaying && !notFined)
		{
			notFined = true;
		}
		if ((Entity)(object)player.Character != (Entity)null && !((Entity)player.Character).IsAlive && !playerDead)
		{
			driveByHandler.CleanDriveBys();
			playerDead = true;
			if (isDealing)
			{
				isDealing = false;
			}
			Entity killer3 = GTA.Entity.FromHandle(GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_PED_SOURCE_OF_DEATH, player.Character.Handle));
			if (killer3 != (Entity)null)
			{
				Ped p4 = (Ped)(object)((killer3 is Ped) ? killer3 : null);
				if (p4 != null && (Entity)(object)p4 != (Entity)null && p4.IsHuman && !p4.IsPlayer && !LsFunctions.IsPolice(p4) && (Entity)(object)attacker == (Entity)null)
				{
					LsFunctions.TextMsg("ZEE", "Information", "J'ai retrouvé cette personne. Je t'ai envoyé sa position.");
					attacker = p4;
					((Entity)attacker).IsPersistent = true;
					attackerMoney = player.Money - (int)((double)player.Money * 0.1);
					Player val2 = player;
					val2.Money -= player.Money;
					if (angryPeds.Contains(attacker))
					{
						angryPeds.Remove(attacker);
					}
					if (driveByHandler.GetDriveBies.Count > 0)
					{
						foreach (DriveBy driveBy in driveByHandler.GetDriveBies)
						{
							if (driveBy.Peds.Count > 0 && driveBy.Peds.Contains(attacker))
							{
								driveBy.Peds.Remove(attacker);
								break;
							}
						}
					}
					if ((Entity)(object)aDealer?.dPed == (Entity)(object)attacker)
					{
						aDealer = null;
					}
					attacker.BlockPermanentEvents = true;
					((Entity)attacker).IsPersistent = true;
					LsFunctions.AddBlip((Entity)(object)attacker, (BlipColor)1, "Voleur", _friend: false, _edge: false);
					if (((Entity)attacker).AttachedBlip != null && ((Entity)attacker).AttachedBlip.Exists())
					{
						((Entity)attacker).AttachedBlip.Scale = 1.3f;
					}
					WeaponHash weaponHash2 = unchecked((WeaponHash)(-1569615261));
					int ammo2 = 0;
					player.Character.Weapons.RemoveAll();
					attacker.Armor = 100;
					foreach (XElement element3 in playerWeapons)
					{
						if (!(element3.Name == "weapon"))
						{
							continue;
						}
						attackerWeapons.Add(element3);
						foreach (XElement wep2 in element3.Elements())
						{
							if (wep2.Name == "HASH")
							{
								Enum.TryParse<WeaponHash>(wep2.Value, out weaponHash2);
							}
							if (wep2.Name == "AMMO")
							{
								int.TryParse(wep2.Value, out ammo2);
							}
							if ((int)weaponHash2 != -1569615261)
							{
								attacker.Weapons.Give(weaponHash2, ammo2, true, true);
							}
						}
					}
					playerWeapons.Clear();
					playerWeps.Clear();
					attacker.Weapons.Select(attacker.Weapons.BestWeapon);
					GTA.Native.Function.Call(GTA.Native.Hash.SET_PED_DROPS_WEAPONS_WHEN_DEAD, attacker.Handle, false);
					if (DEBUG)
					{
						GTA.UI.Notification.PostTicker("L'assaillant est équipé de " + attacker.Weapons.Current.Hash.ToString() + " et d'un gilet pare-balles", false, false);
					}
					attackerWeed = PlayerInventory["Weed"];
					attackerCrack = PlayerInventory["Crack"];
					attackerCocaine = PlayerInventory["Cocaine"];
					foreach (string drug4 in PlayerInventory.Keys.ToList())
					{
						PlayerInventory[drug4] = 0;
					}
				}
			}
		}
		if ((Entity)(object)player.Character != (Entity)null && !player.IsPlaying)
		{
			if (isDealing)
			{
				isDealing = false;
			}
			if (notFined && playerWanted)
			{
				notFined = false;
				playerWanted = false;
				int AmountOfDrugs = TotalDrugsCarried();
				if (pStashVehicle != null && pStashVehicle.drugCarWanted && pStashVehicle.TotalDrugs() > 0)
				{
					AmountOfDrugs += pStashVehicle.TotalDrugs();
					pStashVehicle.ImpoundCar();
				}
				if ((Entity)(object)attacker == (Entity)null && AmountOfDrugs > 0)
				{
					LsFunctions.SendTextMsg("Ils ont pris " + LsFunctions.IntToMoney(player.Money) + " $, ainsi que toute ta drogue et tes armes.");
					player.Character.Weapons.RemoveAll();
					Player val3 = player;
					val3.Money -= player.Money;
					if (PlayerInventory.Keys.ToList().Count > 0)
					{
						foreach (string drug5 in PlayerInventory.Keys.ToList())
						{
							PlayerInventory[drug5] = 0;
						}
						LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
						foreach (XElement element4 in LSLifeSave.Descendants())
						{
							if (!(element4.Name == "playerInventory"))
							{
								continue;
							}
							foreach (XElement pe in element4.Elements())
							{
								if (pe.Name == "weed")
								{
									pe.Value = "0";
								}
								if (pe.Name == "crack")
								{
									pe.Value = "0";
								}
								if (pe.Name == "cocain")
								{
									pe.Value = "0";
								}
							}
						}
						LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
					}
				}
			}
			else if (playerDead)
			{
				playerDead = false;
				if ((Entity)(object)attacker == (Entity)null && TotalDrugsCarried() > 0)
				{
					player.Character.Weapons.RemoveAll();
					LsFunctions.SendTextMsg("La police a confisqué " + LsFunctions.IntToMoney(player.Money) + " $, ainsi que toute ta drogue et tes armes.");
					Player val4 = player;
					val4.Money -= player.Money;
					if (PlayerInventory.Keys.ToList().Count > 0)
					{
						foreach (string drug6 in PlayerInventory.Keys.ToList())
						{
							PlayerInventory[drug6] = 0;
						}
						LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
						foreach (XElement element5 in LSLifeSave.Descendants())
						{
							if (!(element5.Name == "playerInventory"))
							{
								continue;
							}
							foreach (XElement pe2 in element5.Elements())
							{
								if (pe2.Name == "weed")
								{
									pe2.Value = "0";
								}
								if (pe2.Name == "crack")
								{
									pe2.Value = "0";
								}
								if (pe2.Name == "cocain")
								{
									pe2.Value = "0";
								}
							}
						}
						LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
					}
				}
			}
		}
		if (angryPeds.Count > 0)
		{
			int pIndex = 0;
			foreach (Ped p5 in angryPeds)
			{
				pIndex++;
				if ((Entity)(object)p5 == (Entity)null)
				{
					angryPeds.RemoveAt(pIndex);
					break;
				}
				if (!((Entity)(object)p5 != (Entity)null))
				{
					continue;
				}
				if (((Entity)p5).IsDead)
				{
					LsFunctions.RemovePedFromWorld(p5, clearTask: false);
					angryPeds.Remove(p5);
					if (areas[areaIndex].GangPresance > 0f)
					{
						areas[areaIndex].GangPresance--;
					}
					break;
				}
				if (playerPos.DistanceTo(((Entity)p5).Position) > 100f)
				{
					LsFunctions.RemovePedFromWorld(p5, clearTask: false);
					angryPeds.Remove(p5);
					break;
				}
				if (!p5.IsInCombatAgainst(player.Character) || !p5.IsInMeleeCombat)
				{
					p5.Task.Combat(player.Character);
					if (!((Entity)p5).IsPersistent)
					{
						((Entity)p5).IsPersistent = true;
					}
					p5.BlockPermanentEvents = true;
					if (((Entity)p5).AttachedBlip.Exists())
					{
						((Entity)p5).AttachedBlip.Name = "Robber";
						((Entity)p5).AttachedBlip.IsFriendly = false;
					}
				}
			}
		}
		if (!((Entity)(object)player.Character != (Entity)null) || !((Entity)player.Character).IsAlive || !player.IsPlaying)
		{
			return;
		}
		if (pigs.Count == 0)
		{
			if (PlayerComplied)
			{
				PlayerComplied = false;
			}
			if (playerHeat > 65f && rnd.Next(0, 200) == 0)
			{
				SpawnPigs();
			}
			if (playerHeat > 95f && rnd.Next(0, 30) == 0)
			{
				SpawnPigs();
			}
		}
		else
		{
			HandlePigs();
		}
		if (pStashVehicle == null)
		{
			pStashVehicle = new StashVehicle();
		}
		else
		{
			pStashVehicle.OnDelayedTick();
		}
		if (Prison.Started)
		{
			if (Function.Call<bool>((Hash)5315141875575487047L) && !Prison.Ready)
			{
				Function.Call(unchecked((Hash)(-8272491852216909662L)), (player.Handle), (false), (256));
				GTA.UI.Notification.PostTicker("READY", false, false);
				Prison.Ready = true;
			}
			Prison.PrisonTick();
		}
		if ((Entity)(object)attacker != (Entity)null)
		{
			if (((Entity)attacker).AttachedBlip == (Blip)null || !((Entity)attacker).AttachedBlip.Exists())
			{
				LsFunctions.AddBlip((Entity)(object)attacker, (BlipColor)1, "Voleur", _friend: false, _edge: false);
					if (((Entity)attacker).AttachedBlip != null && ((Entity)attacker).AttachedBlip.Exists())
					{
						((Entity)attacker).AttachedBlip.Scale = 1.3f;
					}
			}
			if (((Entity)attacker).IsDead)
			{
				LsFunctions.RemovePedFromWorld(attacker, clearTask: true);
				if (!attackerPickupSpawned)
				{
					attackerAttacking = false;
					attackerPickupSpawned = true;
					attackerPickup = Pickup.Create((PickupType)341217064, ((Entity)attacker).Position, Vector3.Zero, amount: 100, customModel: new Model("prop_big_bag_01"));
				}
				attacker = null;
			}
			else
			{
				if (!((Entity)attacker).IsPersistent)
				{
					((Entity)attacker).IsPersistent = true;
				}
				position = ((Entity)attacker).Position;
				float distance = position.DistanceTo(playerPos);
				if (!attackerAttacking && Game.GameTime > attackCheckTime + 1000)
				{
					attackCheckTime = Game.GameTime;
					if (distance < 60f && LsFunctions.CanSee((Entity)(object)attacker, (Entity)(object)player.Character))
					{
						int EnemyRelationShipGroup = Function.Call<int>(unchecked((Hash)(-3292914402564945716L)), ("HATES_PLAYER"));
						Function.Call(unchecked((Hash)(-4032282232082276974L)), (attacker), (EnemyRelationShipGroup));
						attackerAttacking = true;
						attacker.Task.ClearAll();
						attacker.Task.Combat(player.Character);
					}
					else
					{
						attacker.Task.ClearAll();
						attacker.Task.Wander();
						attackerAttacking = false;
					}
				}
			}
		}
		bool sprintChange = false;
		if (!playerHasBag)
		{
			if (!canSprint)
			{
				canSprint = true;
				sprintChange = true;
			}
			if (TotalDrugsCarried() / 28 + (playerWeps.Count - 1) + player.Money / 50000 > 35)
			{
				LsFunctions.PedBag(player.Character, give: true);
				playerHasBag = true;
			}
		}
		if (playerHasBag)
		{
			if (!canSprint)
			{
				if ((Entity)(object)player.Character.CurrentVehicle != (Entity)null)
				{
					model = ((Entity)player.Character.CurrentVehicle).Model;
					if (model.IsBicycle)
					{
						canSprint = true;
						sprintChange = true;
					}
				}
			}
			else if ((Entity)(object)player.Character.CurrentVehicle == (Entity)null)
			{
				canSprint = false;
				sprintChange = true;
			}
			if (TotalDrugsCarried() / 28 + (playerWeps.Count - 1) + player.Money / 50000 <= 35)
			{
				LsFunctions.PedBag(player.Character, give: false);
				playerHasBag = false;
			}
		}
		if (sprintChange)
		{
			if (canSprint)
			{
				Function.Call(unchecked((Hash)(-6909787959634350604L)), (player), (true));
			}
			else
			{
				Function.Call(unchecked((Hash)(-6909787959634350604L)), (player), (false));
			}
		}
		if (player.Wanted.WantedLevel > 0 && !playerWanted)
		{
			playerWanted = true;
		}
		if (playerWanted && player.Wanted.WantedLevel == 0)
		{
			playerWanted = false;
		}
		if ((Entity)(object)drugDealer1 != (Entity)null && ((Entity)drugDealer1).IsDead)
		{
			LsFunctions.RemovePedFromWorld(drugDealer1, clearTask: true);
			drugDealer1 = null;
		}
		if ((Entity)(object)weaponDealer1 != (Entity)null && ((Entity)weaponDealer1).IsDead)
		{
			LsFunctions.RemovePedFromWorld(weaponDealer1, clearTask: true);
			weaponDealer1 = null;
		}
		if (player.Wanted.WantedLevel > 0 && isDealing)
		{
			isDealing = false;
		}
		if (!isDealing && !removedStuff)
		{
			removedStuff = true;
			if (ShotHandler.shots.Count > 0)
			{
				foreach (Shot s in ShotHandler.shots)
				{
					if (s.customer != null)
					{
						LsFunctions.RemovePedFromWorld(s.customer.Ped, clearTask: true);
					}
				}
				ShotHandler.shots.Clear();
			}
			if (customers.Count > 0)
			{
				foreach (Customer c3 in customers)
				{
					if (c3 == null)
					{
						continue;
					}
					foreach (Drug d in areas[areaIndex].Drugs)
					{
						if (d.Name == c3.Drug.Name)
						{
							d.Supplied = false;
						}
					}
					if (((Entity)c3.Ped).Exists())
					{
						OldCustomerHandler.AddCustomer(c3.Ped, c3.Vehicle, HouseHandler.Inside(), _canBeRival: true, new KeyValuePair<string, int>(c3.Drug.Name, c3.amount));
					}
				}
				customers.Clear();
			}
			if (angryPeds.Count > 0)
			{
				foreach (Ped p6 in angryPeds)
				{
					if ((Entity)(object)p6 != (Entity)null && ((Entity)p6).Exists())
					{
						LsFunctions.RemovePedFromWorld(p6, clearTask: true);
					}
				}
				angryPeds.Clear();
			}
		}
		if (isDealing)
		{
			ShotHandler.OnSlowTick();
		}
		if (!((Entity)(object)weaponDealer1 != (Entity)null) || !((Entity)weaponDealer1).IsAlive)
		{
			return;
		}
		float distanceToPlayer = GTA.Math.Vector3.Distance(((Entity)weaponDealer1).Position, playerPos);
		if (distanceToPlayer > 15f && weaponDealer1.PedGroup != player.Character.PedGroup)
		{
			if (Game.GameTime > dealerTimer + 2000 && playerPos != pedTarget)
			{
				dealerTimer = Game.GameTime;
				pedTarget = playerPos;
				weaponDealer1.Task.FollowNavMeshTo(player.Character.Position);
			}
		}
		else if (weaponDealer1.PedGroup != player.Character.PedGroup)
		{
			weaponDealer1.Task.ClearAll();
			Function.Call(unchecked((Hash)(-6974808092771143243L)), (((Entity)weaponDealer1).Handle), (playerGroup));
		}
		if (!ShotHandler.ShotOffers && weaponDealer1.IsStopped && distanceToPlayer <= 3f && player.Character.IsStopped && LsFunctions.IsCustomerNear() && PottentialWorker == null)
		{
			if (lPeds.GetPeds.FindAll((Ped r) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position2 = ((Entity)r).Position;
				return position2.DistanceTo(playerPos) < 10f;
			}).Count() - PedsInPlayersGroup() > 3)
			{
				if ((float)Game.GameTime > displayHelpTimer + 5000f)
				{
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Trouve un endroit plus tranquille.");
				}
				readyToDeal = false;
				if (CustomWeaponDealerMenu.Visible)
				{
					CustomWeaponDealerMenu.Close();
				}
			}
			else if (!readyToDeal && !CustomMenuShared.IsAnyGameplayMenuOpen())
			{
				if ((float)Game.GameTime > displayHelpTimer + 5000f)
				{
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour interagir");
				}
				readyToDeal = true;
			}
		}
		else
		{
			readyToDeal = false;
		}
		if ((Entity)(object)weaponDealer1 != (Entity)null && !((Entity)weaponDealer1).IsAlive)
		{
			((Entity)weaponDealer1).IsPersistent = false;
			((Entity)weaponDealer1).AttachedBlip.Delete();
			((Entity)weaponDealer1).MarkAsNoLongerNeeded();
			LsFunctions.SendTextMsg("Le dealer est mort !");
			weaponDealer1 = null;
			readyToDeal = false;
		}
		return;
		IL_25d6:
		if (wepDealer.state == Dealer.States.Driving || wepDealer.state == Dealer.States.Fast)
		{
			position = ((Entity)wepDealer.vehicle).Position;
			if (position.DistanceTo(wepDealer.destination) < 80f)
			{
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, (GTA.VehicleDrivingFlags)(786603), 10f);
				wepDealer.state = Dealer.States.Ariving;
			}
		}
		if (wepDealer.state == Dealer.States.Ariving)
		{
			position = ((Entity)wepDealer.vehicle).Position;
			if (position.DistanceTo(wepDealer.destination) < 20f)
			{
				wepDealer.doneTutorial = false;
				Vector3 pos2 = wepDealer.destination;
				int NodeID2 = Function.Call<int>((Hash)2510518586829603349L, (pos2.X), (pos2.Y), (pos2.Z), (1), (1), (1f), (0f));
				OutputArgument outArgA2 = new OutputArgument();
				Function.Call((Hash)8084282276450675138L, (NodeID2), outArgA2);
				Vector3 parkPos2 = outArgA2.GetResult<Vector3>();
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, parkPos2, 2f, (GTA.VehicleDrivingFlags)(786603), 6f);
				wepDealer.destination = Vector3.Zero;
				wepDealer.state = Dealer.States.Arrived;
			}
		}
		if (wepDealer.state == Dealer.States.Arrived && player.Character.IsInVehicle() && ((Entity)player.Character.CurrentVehicle).Handle == ((Entity)wepDealer.vehicle).Handle)
		{
			if (!wepDealer.doneTutorial)
			{
				Function.Call((Hash)7023634693725934496L);
				wepDealer.doneTutorial = true;
				displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Appuie sur ~INPUT_CONTEXT~ pour interagir ou sur ~INPUT_CONTEXT_SECONDARY~ pour renvoyer.");
			}
			if (Game.IsWaypointActive && wepDealer.destination != World.GetNextPositionOnStreet(GTA.Native.Function.Call<GTA.Math.Vector3>(GTA.Native.Hash.GET_BLIP_INFO_ID_COORD, GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_FIRST_BLIP_INFO_ID, 8))))
			{
				wepDealer.destination = World.GetNextPositionOnStreet(GTA.Native.Function.Call<GTA.Math.Vector3>(GTA.Native.Hash.GET_BLIP_INFO_ID_COORD, GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_FIRST_BLIP_INFO_ID, 8)));
				((Entity)wepDealer.vehicle).IsInvincible = true;
				wepDealer.ped.Task.ClearAll();
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1f, (GTA.VehicleDrivingFlags)(786603), 10f);
				GTA.UI.Screen.ShowSubtitle("OK, on se rend à ~y~" + World.GetStreetName(wepDealer.destination));
				wepDealer.state = Dealer.States.Leaving;
			}
		}
		if (wepDealer.state == Dealer.States.Leaving)
		{
			Script.Wait(5000);
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1f, (GTA.VehicleDrivingFlags)(786603), 40f);
			wepDealer.state = Dealer.States.Driving;
		}
		if (wepDealer.state == Dealer.States.Faster)
		{
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, (GTA.VehicleDrivingFlags)(6), 50f);
			wepDealer.state = Dealer.States.Fast;
			GTA.UI.Screen.ShowSubtitle("OK, j'accélère.", 2000);
			wepDealer.canSpeed = true;
		}
		if (wepDealer.state == Dealer.States.Slower)
		{
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, (GTA.VehicleDrivingFlags)(786603), 20f);
			wepDealer.state = Dealer.States.Driving;
			GTA.UI.Screen.ShowSubtitle("Ok, Slowing it down.", 2000);
			wepDealer.canSpeed = false;
		}
		goto IL_29de;
	}
}
