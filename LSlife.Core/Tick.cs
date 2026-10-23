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
using NativeUI;


namespace LSlife;

public partial class LSL
{
	private void onTick(object sender, EventArgs e)
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
		if (Game.IsLoading)
		{
			return;
		}
		if (firstRun && (Entity)(object)player.Character != (Entity)null)
		{
			if (File.Exists("scripts\\LSLife\\LSLifeAreas.xml"))
			{
				LoadAreaData();
			}
			UI.Notify("~g~LsLife: " + vNumber);
			UI.Notify("~g~LsLife: " + areas.Count + " zones chargées");
			lPeds = new LsLPeds();
			World.SetRelationshipBetweenGroups((Relationship)3, playerRelationship, CustomerRel);
			World.SetRelationshipBetweenGroups((Relationship)3, playerRelationship, rival_nutral);
			World.SetRelationshipBetweenGroups((Relationship)5, playerRelationship, rival_enemy);
			World.SetRelationshipBetweenGroups((Relationship)3, rival_nutral, CustomerRel);
			World.SetRelationshipBetweenGroups((Relationship)3, rival_nutral, playerRelationship);
			World.SetRelationshipBetweenGroups((Relationship)1, rival_nutral, rival_enemy);
			World.SetRelationshipBetweenGroups((Relationship)1, rival_enemy, CustomerRel);
			World.SetRelationshipBetweenGroups((Relationship)5, rival_enemy, playerRelationship);
			World.SetRelationshipBetweenGroups((Relationship)1, rival_enemy, rival_nutral);
			World.SetRelationshipBetweenGroups((Relationship)0, CustomerRel, playerRelationship);
			World.SetRelationshipBetweenGroups((Relationship)0, CustomerRel, rival_nutral);
			World.SetRelationshipBetweenGroups((Relationship)0, CustomerRel, rival_enemy);
			playerHeat = -100f;
			Function.Call((Hash)2750315038012726912L, (player.Character), (9), (0), (0), (0));
			playerHasBag = false;
			canSprint = true;
			playerArea = World.GetZoneNameLabel(playerPos);
			if (areas.Where((Area a) => a.Name != playerArea).Count() == 0)
			{
				if (zoneData.ContainsKey(playerArea))
				{
					areas.Add(new Area(playerArea, LsFunctions.LsDrugs(), zoneData[playerArea].Item1));
				}
				else
				{
					UI.Notify("~r~LsLife - Impossible de déterminer le type du quartier : " + playerArea);
				}
			}
			Area cArea = areas.Find((Area a) => a.Name == playerArea);
			if (cArea != null)
			{
				areaIndex = areas.IndexOf(cArea);
			}
			firstRun = false;
		}
		if (DEBUG)
		{
			string debugString = "0";
			if (Game.GameTime < areas[areaIndex].DemandBuff())
			{
				debugString = (areas[areaIndex].DemandBuff() - Game.GameTime).ToString();
			}
			myUIText = new UIText(debugString, new Point(10, 10), 0.4f, Color.WhiteSmoke, (GTA.Font)0, false);
			myUIText.Draw();
		}
		if (Prison.Started && Prison.CurrentState > Prison.PrisonStates.ApproachBarrier)
		{
			Function.Call(unchecked((Hash)(-7077668788463384353L)), ("restrictedareas"));
			Function.Call(unchecked((Hash)(-7077668788463384353L)), ("re_prison"));
		}
		if (LsMenuPool != null)
		{
			LsMenuPool.ProcessMenus();
		}
		iFruit.Update();
		if (Game.IsControlPressed(2, (Control)76) && Game.IsControlJustPressed(2, (Control)26))
		{
			DropDrugs.Drop();
		}
		Vector3 position;
		if ((Entity)(object)player.Character != (Entity)null && ((Entity)player.Character).IsAlive && player.IsPlaying)
		{
			playerPos = ((Entity)player.Character).Position;
			if (DEBUG)
			{
				PedDebugger.OnTick();
			}
			if (DealerHandler != null)
			{
				DealerHandler.OnTick();
			}
			if (HouseHandler != null)
			{
				HouseHandler.Ontick();
			}
			if (ZeesDealers != null)
			{
				ZeesDealers.OnTick();
			}
			HandlePotentialWorker();
			if (showInventory)
			{
				Hud.Draw();
				if (AreaDebug && !LsMenuPool.IsAnyMenuOpen())
				{
					areaInfoUI();
					ZeeInfoUI();
					PlayerUI();
					GangUI();
				}
				player.GetTargetedEntity();

			}
			else
			{
				Hud.Hide();
			}
			if (pigs.Count > 0 && pigs[0].AskPlayerToComply)
			{
				if ((float)Game.GameTime > displayHelpTimer + 3000f && !LsMenuPool.IsAnyMenuOpen())
				{
					Function.Call((Hash)7023634693725934496L);
					displayHelpTimer = Game.GameTime;
					LsFunctions.DisplayHelpText("~INPUT_CONTEXT~ pour obtempérer.");
				}
				if (Game.IsControlJustPressed(2, (Control)51))
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
			HandleAreaDealers();
			if (!LsMenuPool.IsAnyMenuOpen() && (Entity)(object)weaponDealer1 != (Entity)null && readyToDeal && Game.IsControlJustPressed(2, (Control)51) && LsMenuPool != null && !LsMenuPool.IsAnyMenuOpen())
			{
				wepMenu.Visible = !wepMenu.Visible;
			}
			if (pStashVehicle != null)
			{
				pStashVehicle.OnTick();
			}
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
					if (playerHeat > -100f && player.WantedLevel == 0)
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
											Ped cPed2 = World.CreatePed(LsFunctions.RequestModel(hash2), HouseHandler.CurrentHouse.exitPos);
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
											Ped cPed3 = World.CreatePed(LsFunctions.RequestModel(hash3), HouseHandler.CurrentHouse.exitPos);
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
											Ped cPed = World.CreatePed(LsFunctions.RequestModel(hash), HouseHandler.CurrentHouse.exitPos);
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
								if (!(c.Ped.CurrentPedGroup != player.Character.CurrentPedGroup))
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
					if (nCustomer != null && !LsMenuPool.IsAnyMenuOpen())
					{
						if (player.Character.IsInVehicle() && nCustomer.Ped.IsInVehicle() && player.Character.CurrentVehicle.Speed < 1f)
						{
							if ((float)Game.GameTime > displayHelpTimer + 3000f && !ShotHandler.ShotOffers && PottentialWorker == null)
							{
								Function.Call((Hash)7023634693725934496L);
								displayHelpTimer = Game.GameTime;
								LsFunctions.DisplayHelpText("~INPUT_SPRINT~ pour vendre depuis le véhicule.");
							}
							if (Game.IsControlJustPressed(2, (Control)21))
							{
								Function.Call((Hash)7023634693725934496L);
								customerSellTo = nCustomer;
								sell.Text = "Vendre " + LsFunctions.GramsToOz(customerSellTo.amount) + " de " + LsFunctions.DrugDisplayName(customerSellTo.Drug.Name) + "?";
								((UIText)customerMainMenu.Subtitle).Caption = "Tu possèdes " + LsFunctions.GramsToOz(PlayerInventory[customerSellTo.Drug.Name]) + " de " + LsFunctions.DrugDisplayName(customerSellTo.Drug.Name);
								sell.Enabled = PlayerInventory[customerSellTo.Drug.Name] >= customerSellTo.amount;
								if (!sell.Enabled)
								{
									LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
								}
								customerMainMenu.Visible = !customerMainMenu.Visible;
								customerMainMenu.CurrentSelection = 0;
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
							if (Game.IsControlJustPressed(2, (Control)21) && !LsMenuPool.IsAnyMenuOpen())
							{
								Function.Call((Hash)7023634693725934496L);
								customerSellTo = nCustomer;
								sell.Text = "Vendre " + LsFunctions.GramsToOz(customerSellTo.amount) + " de " + LsFunctions.DrugDisplayName(customerSellTo.Drug.Name) + "?";
								((UIText)customerMainMenu.Subtitle).Caption = "Tu possèdes " + LsFunctions.GramsToOz(PlayerInventory[customerSellTo.Drug.Name]) + " de " + LsFunctions.DrugDisplayName(customerSellTo.Drug.Name);
								sell.Enabled = PlayerInventory[customerSellTo.Drug.Name] >= customerSellTo.amount;
								if (!sell.Enabled)
								{
									LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
								}
								customerMainMenu.Visible = !customerMainMenu.Visible;
								customerMainMenu.CurrentSelection = 0;
							}
						}
					}
				}
			}
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
						if (Game.IsControlJustPressed(2, (Control)75))
						{
							Function.Call((Hash)7023634693725934496L);
							player.Character.Task.EnterVehicle(wepDealer.vehicle, (VehicleSeat)0);
						}
					}
				}
				else if (player.Character.IsInVehicle() && ((Entity)player.Character.CurrentVehicle).Handle == ((Entity)wepDealer.vehicle).Handle)
				{
					if (Game.IsControlJustPressed(2, (Control)52))
					{
						player.Character.Task.LeaveVehicle(player.Character.CurrentVehicle, true);
						Script.Wait(5500);
						wepDealer.ped.CurrentVehicle.SoundHorn(15);
						Script.Wait(15);
						wepDealer.ped.CurrentVehicle.SoundHorn(12);
						((Entity)wepDealer.ped).IsPersistent = false;
						((Entity)wepDealer.vehicle).IsPersistent = false;
						((Entity)wepDealer.vehicle).CurrentBlip.Remove();
						Function.Call((Hash)5188501456062479616L, (((Entity)wepDealer.ped).Handle), (((Entity)wepDealer.vehicle).Handle), (20f), (786603));
						wepDealer = null;
					}
					if (Game.IsControlJustPressed(2, (Control)51))
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
			ShotHandler.Tick();
		}
		if (Game.GameTime > oneSecDelay + 10001)
		{
			oneSecDelay = Game.GameTime;
			if (player.IsPlaying)
			{
				if (!dealer1Angry && (double)dealer1Debt > (double)LsFunctions.zeeDebtCap() + (double)LsFunctions.zeeDebtCap() * 0.5)
				{
					if (DEBUG)
					{
						UI.Notify("dealer1 angry");
					}
					dealer1Angry = true;
				}
				if (driveByHandler.AmountOfDriveBy(_agg: true) == 0)
				{
					if (isDealing && areas[areaIndex].GangPresance > 4f && timeDealing > 240000 && rnd.Next(0, 60) == 0)
					{
						if (DEBUG)
						{
							UI.Notify("Attaque en voiture dans un quartier défavorisé");
						}
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
					}
					if (dealer1Angry && driveByHandler.AmountOfDriveBy(_agg: true) == 0 && (Entity)(object)drugDealer1 == (Entity)null && rnd.Next(0, 10) == 0)
					{
						if (DEBUG)
						{
							UI.Notify("dealer1 drive-by");
						}
						LsFunctions.SpawnDriveBy(player.Character, _agressive: true);
						LsFunctions.TextMsg("ZEE", "Dette", "Rembourse ta dette et je les rappellerai.");
					}
				}
				if (entitiesCleanUp.Count > 0)
				{
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
							if (ent.CurrentBlip != (Blip)null || ent.CurrentBlip.Exists())
							{
								ent.CurrentBlip.Remove();
							}
							if (ent == (Entity)(object)attacker)
							{
								entitiesCleanUp.Remove(ent);
								if (DEBUG)
								{
									UI.Notify("Assaillant supprimé après l'arrêt de l'attaque");
								}
								continue;
							}
							position = ent.Position;
							if (position.DistanceTo(playerPos) > 200f && !ent.IsOnScreen)
							{
								entitiesCleanUp.Remove(ent);
								if (DEBUG)
								{
									UI.Notify("Delete:" + (object)ent);
								}
								ent.Delete();
							}
						}
					}
				}
			}
		}
		Model model;
		if (Game.GameTime > areaCheckTimer + 999)
		{
			newWepAmount = 0;
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
					UI.Notify("Changement d'arme du joueur détecté");
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
						UI.Notify("Arme ajoutée : " + ((object)weapon2/*cast due to constrained. prefix*/).ToString() + " (liste playerWeapons).");
					}
					XElement weaponAmmoElement = new XElement("AMMO", ammunitionInWeapon);
					parentElement.Add(weaponNameElement);
					parentElement.Add(weaponAmmoElement);
					foreach (WeaponComponent component in Enum.GetValues(typeof(WeaponComponent)))
					{
						if (Function.Call<bool>(unchecked((Hash)(-4209984786179366080L)), (player.Character), ((int)weapon2), ((int)component)))
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
			string newArea = World.GetZoneNameLabel(playerPos);
			if (aDealer != null && playerArea != aDealer.Area && playerPos.DistanceTo(((Entity)aDealer.dPed).Position) > 300f)
			{
				if ((Entity)(object)aDealer.cPed != (Entity)null)
				{
					LsFunctions.RemovePedFromWorld(aDealer.cPed, clearTask: false);
				}
				LsFunctions.RemovePedFromWorld(aDealer.dPed, clearTask: false);
				aDealer = null;
			}
			if (playerArea != newArea)
			{
				playerArea = newArea;
				if (areas.FindAll((Area a) => a.Name == playerArea).Count == 0)
				{
					if (zoneData.ContainsKey(playerArea))
					{
						areas.Add(new Area(playerArea, LsFunctions.LsDrugs(), zoneData[playerArea].Item1));
					}
					else
					{
						UI.Notify("~r~LsLife - Impossible de déterminer le type du quartier : " + playerArea);
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
					string area1 = ((HouseHandler.CurrentHouse == null || !HouseHandler.CurrentHouse.inside) ? playerArea : World.GetZoneNameLabel(HouseHandler.CurrentHouse.entrancePos));
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
			else if (aDealer == null && areas[areaIndex].CanSpawnDealer())
			{
				LsFunctions.DbugString("demandBuff 0");
				if (areas[areaIndex].GangPresance > 0f && areas[areaIndex].Name != World.GetZoneNameLabel(HouseHandler.Houses[0].entrancePos))
				{
					List<Ped> aPeds = lPeds.GetPeds.FindAll((Ped r) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_002c: Unknown result type (might be due to invalid IL or missing references)
						//IL_002f: Unknown result type (might be due to invalid IL or missing references)
						if (World.GetZoneNameLabel(((Entity)r).Position) == areas[areaIndex].Name)
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
								if (DEBUG)
								{
									model = ((Entity)p3).Model;
									UI.Notify("Dealer de quartier généré avec ~g~" + ((object)model/*cast due to constrained. prefix*/).ToString());
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
		if (Game.GameTime <= SlowTickTime + 375)
		{
			return;
		}
		SlowTickTime = Game.GameTime;
		player = Game.Player;
		playerGroup = player.Character.CurrentPedGroup;
		if (Function.Call<bool>((Hash)6160435850588389544L, (lsDebug)))
		{
			DEBUG = !DEBUG;
		}
		else if (Function.Call<bool>((Hash)6160435850588389544L, (areaDebug)))
		{
			AreaDebug = !AreaDebug;
		}
		else if (Function.Call<bool>((Hash)6160435850588389544L, (spawnP)))
		{
			LsFunctions.SpawnDriveBy(player.Character, _agressive: false);
		}
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
				if (aDealer.dPed.GetKiller() is Ped)
				{
					Entity killer = aDealer.dPed.GetKiller();
					dKiller = (Ped)(object)((killer is Ped) ? killer : null);
				}
				if ((Entity)(object)dKiller != (Entity)null && dKiller.CurrentPedGroup == player.Character.CurrentPedGroup)
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
						UI.Notify("Tu as été vu en train de tuer un dealer.");
					}
				}
				Ped dKiller2 = null;
				if (aDealer.dPed.GetKiller() is Ped)
				{
					Entity killer2 = aDealer.dPed.GetKiller();
					dKiller2 = (Ped)(object)((killer2 is Ped) ? killer2 : null);
				}
				if ((Entity)(object)dKiller2 != (Entity)null && dKiller2.CurrentPedGroup == player.Character.CurrentPedGroup)
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
		Function.Call((Hash)8084282276450675138L, (NodeID), (InputArgument)(object)outArgA);
		Vector3 parkPos = outArgA.GetResult<Vector3>();
		wepDealer.destination = parkPos;
		goto IL_25d6;
		IL_29de:
		if (dealer1Angry && dealer1Debt < 170000)
		{
			dealer1Angry = false;
		}
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
							Enum.TryParse<WeaponComponent>(c2.Value, out WeaponComponent wepcomp);
							player.Character.Weapons.Current.SetComponent(wepcomp, true);
						}
					}
					if (DEBUG)
					{
						UI.Notify("Munitions données pour " + ((object)weaponHash/*cast due to constrained. prefix*/).ToString() + " : " + ammo);
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
				return a.Name == World.GetZoneNameLabel(HouseHandler.CurrentHouse.entrancePos);
			}));
			areaToAddHeat.Heat += (float)player.WantedLevel * 0.05f;
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
			Entity killer3 = player.Character.GetKiller();
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
					if (((Entity)attacker).CurrentBlip != null && ((Entity)attacker).CurrentBlip.Exists())
					{
						((Entity)attacker).CurrentBlip.Scale = 1.3f;
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
					attacker.DropsWeaponsOnDeath = false;
					if (DEBUG)
					{
						UI.Notify("L'assaillant est équipé de " + attacker.Weapons.Current.Name + " et d'un gilet pare-balles");
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
					p5.Task.FightAgainst(player.Character);
					if (!((Entity)p5).IsPersistent)
					{
						((Entity)p5).IsPersistent = true;
					}
					p5.BlockPermanentEvents = true;
					if (((Entity)p5).CurrentBlip.Exists())
					{
						((Entity)p5).CurrentBlip.Name = "Robber";
						((Entity)p5).CurrentBlip.IsFriendly = false;
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
				UI.Notify("READY");
				Prison.Ready = true;
			}
			Prison.PrisonTick();
		}
		if ((Entity)(object)attacker != (Entity)null)
		{
			if (((Entity)attacker).CurrentBlip == (Blip)null || !((Entity)attacker).CurrentBlip.Exists())
			{
				LsFunctions.AddBlip((Entity)(object)attacker, (BlipColor)1, "Voleur", _friend: false, _edge: false);
					if (((Entity)attacker).CurrentBlip != null && ((Entity)attacker).CurrentBlip.Exists())
					{
						((Entity)attacker).CurrentBlip.Scale = 1.3f;
					}
			}
			if (((Entity)attacker).IsDead)
			{
				LsFunctions.RemovePedFromWorld(attacker, clearTask: true);
				if (!attackerPickupSpawned)
				{
					attackerAttacking = false;
					attackerPickupSpawned = true;
					attackerPickup = World.CreatePickup((PickupType)341217064, ((Entity)attacker).Position, Vector3.Zero, new Model("prop_big_bag_01"), 100);
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
						attacker.Task.FightAgainst(player.Character);
					}
					else
					{
						attacker.Task.ClearAll();
						attacker.Task.WanderAround();
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
		if (player.WantedLevel > 0 && !playerWanted)
		{
			playerWanted = true;
		}
		if (playerWanted && player.WantedLevel == 0)
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
		if (player.WantedLevel > 0 && isDealing)
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
		float distanceToPlayer = World.GetDistance(((Entity)weaponDealer1).Position, playerPos);
		if (distanceToPlayer > 15f && weaponDealer1.CurrentPedGroup != player.Character.CurrentPedGroup)
		{
			if (Game.GameTime > dealerTimer + 2000 && playerPos != pedTarget)
			{
				dealerTimer = Game.GameTime;
				pedTarget = playerPos;
				weaponDealer1.Task.GoTo((Entity)(object)player.Character);
			}
		}
		else if (weaponDealer1.CurrentPedGroup != player.Character.CurrentPedGroup)
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
				if (wepMenu.Visible)
				{
					wepMenu.Visible = !wepMenu.Visible;
				}
			}
			else if (!readyToDeal && !LsMenuPool.IsAnyMenuOpen())
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
			((Entity)weaponDealer1).CurrentBlip.Remove();
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
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 10f, 786603);
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
				Function.Call((Hash)8084282276450675138L, (NodeID2), (InputArgument)(object)outArgA2);
				Vector3 parkPos2 = outArgA2.GetResult<Vector3>();
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, parkPos2, 2f, 6f, 786603);
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
			if (Game.IsWaypointActive && wepDealer.destination != World.GetNextPositionOnStreet(World.GetWaypointPosition()))
			{
				wepDealer.destination = World.GetNextPositionOnStreet(World.GetWaypointPosition());
				((Entity)wepDealer.vehicle).IsInvincible = true;
				wepDealer.ped.Task.ClearAll();
				wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1f, 10f, 786603);
				UI.ShowSubtitle("OK, on se rend à ~y~" + World.GetStreetName(wepDealer.destination));
				wepDealer.state = Dealer.States.Leaving;
			}
		}
		if (wepDealer.state == Dealer.States.Leaving)
		{
			Script.Wait(5000);
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 1f, 40f, 786603);
			wepDealer.state = Dealer.States.Driving;
		}
		if (wepDealer.state == Dealer.States.Faster)
		{
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 50f, 6);
			wepDealer.state = Dealer.States.Fast;
			UI.ShowSubtitle("OK, j'accélère.", 2000);
			wepDealer.canSpeed = true;
		}
		if (wepDealer.state == Dealer.States.Slower)
		{
			wepDealer.ped.Task.DriveTo(wepDealer.vehicle, wepDealer.destination, 4f, 20f, 786603);
			wepDealer.state = Dealer.States.Driving;
			UI.ShowSubtitle("Ok, Slowing it down.", 2000);
			wepDealer.canSpeed = false;
		}
		goto IL_29de;
	}
}
