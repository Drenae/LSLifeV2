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
	private void onKeyDown(object sender, KeyEventArgs e)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (e.KeyCode == SetDrugCar && ((Entity)player.Character).IsAlive)
		{
			if (player.Character.IsInVehicle())
			{
				if (pStashVehicle != null && (Entity)(object)pStashVehicle.drugCar != (Entity)null)
				{
					LsFunctions.SendTextMsg(pStashVehicle.drugCar.DisplayName + " n'est plus un véhicule de stockage.");
					pStashVehicle.RemoveOldCar();
					pStashVehicle = null;
				}
				else
				{
					Model model = ((Entity)player.Character.CurrentVehicle).Model;
					if (!model.IsCar)
					{
						model = ((Entity)player.Character.CurrentVehicle).Model;
						if (!model.IsBike)
						{
							model = ((Entity)player.Character.CurrentVehicle).Model;
							if (!model.IsBoat)
							{
								model = ((Entity)player.Character.CurrentVehicle).Model;
								if (!model.IsHelicopter)
								{
									model = ((Entity)player.Character.CurrentVehicle).Model;
									if (!model.IsPlane)
									{
										LsFunctions.SendTextMsg("Tu ne peux pas utiliser ce véhicule comme stockage.");
										goto IL_0180;
									}
								}
							}
						}
					}
					pStashVehicle = new StashVehicle();
					pStashVehicle.VehicleSave(player.Character.CurrentVehicle);
					if ((Entity)(object)pStashVehicle.drugCar != (Entity)null)
					{
						LsFunctions.SendTextMsg(pStashVehicle.drugCar.DisplayName + " est maintenant ton véhicule de stockage.");
					}
				}
			}
			else
			{
				DropDrugs.Drop();
			}
		}
		goto IL_0180;
		IL_0180:
		if (e.KeyCode == StopDealing && ((Entity)player.Character).IsAlive)
		{
			if (isDealing)
			{
				isDealing = false;
				LsFunctions.SendTextMsg("Vente de rue annulée.");
			}
			else if (!isDealing)
			{
				LsFunctions.SendTextMsg("Aucune vente de rue en cours.");
			}
		}
		if (e.KeyCode == Keys.J && !started)
		{
			if (DEBUG)
			{
				if (player.Character.IsInVehicle())
				{
					LsFunctions.PlayTheSound();
					Vector3 vPos = ((Entity)player.Character.CurrentVehicle).Position;
					float vHead = ((Entity)player.Character.CurrentVehicle).Heading;
					XDocument xDocument = XDocument.Load("scripts\\LSLife\\LSLife_Spawns.xml");
					XElement ePos = new XElement("pos");
					ePos.Add(new XElement("x", vPos.X));
					ePos.Add(new XElement("y", vPos.Y));
					ePos.Add(new XElement("z", vPos.Z));
					ePos.Add(new XElement("h", vHead));
					xDocument.Root.Add(ePos);
					xDocument.Save("scripts\\LSLife\\LSLife_Spawns.xml");
					UI.Notify("Position ajoutée au fichier XML");
				}
				else
				{
					UI.Notify("Tu dois être dans un véhicule pour ajouter une position.");
				}
			}
			if (DEBUG)
			{
				player.Character.IsInVehicle();
			}
		}
		if (e.KeyCode != MenuKey || !(player != (Player)null) || !player.IsAlive)
		{
			return;
		}
		if (DEBUG && (Entity)(object)player.Character != (Entity)null)
		{
			string toPaste = playerPos.X + "f, " + playerPos.Y + "f, " + playerPos.Z + "f";
			Thread thread = new Thread(() =>
			{
				Clipboard.SetText(toPaste);
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			thread.Abort();
		}
		showInventory = !showInventory;
		Game.PlaySound("Text_Arrive_Tone", "Phone_SoundSet_Default");
	}

	private void OnAbort(object sender, EventArgs e)
	{
		if (1 == 0)
		{
			return;
		}
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
		StashHouseHandler houseHandler = HouseHandler;
		if (houseHandler != null && houseHandler.Houses.Count > 0)
		{
			foreach (StashHouse house in HouseHandler.Houses)
			{
				house?.RemoveBlip();
			}
		}
		if ((Entity)(object)drugDealer1 != (Entity)null)
		{
			Blip currentBlip = ((Entity)drugDealer1).CurrentBlip;
			if (currentBlip != null)
			{
				currentBlip.Remove();
			}
			((Entity)drugDealer1).Delete();
		}
		if ((Entity)(object)weaponDealer1 != (Entity)null)
		{
			Blip currentBlip2 = ((Entity)weaponDealer1).CurrentBlip;
			if (currentBlip2 != null)
			{
				currentBlip2.Remove();
			}
			((Entity)weaponDealer1).Delete();
		}
		if (customers.Count >= 1)
		{
			foreach (Customer c in customers.ToList())
			{
				if (c != null)
				{
					Ped ped = c.Ped;
					Blip currentBlip3 = ((Entity)ped).CurrentBlip;
					if (currentBlip3 != null)
					{
						currentBlip3.Remove();
					}
					((Entity)ped).Delete();
				}
			}
		}
		if (pStashVehicle != null && (Entity)(object)pStashVehicle.drugCar != (Entity)null)
		{
			if ((Entity)(object)pStashVehicle.drugCar != (Entity)null && ((Entity)pStashVehicle.drugCar).Exists())
			{
				((Entity)pStashVehicle.drugCar).CurrentBlip.Remove();
				((Entity)pStashVehicle.drugCar).Delete();
			}
			Blip drugCarBlip = pStashVehicle.drugCarBlip;
			if (drugCarBlip != null)
			{
				drugCarBlip.Remove();
			}
		}
		if (wepDealer != null)
		{
			if ((Entity)(object)wepDealer.vehicle != (Entity)null)
			{
				((Entity)wepDealer.vehicle).CurrentBlip.Remove();
				((Entity)wepDealer.vehicle).Delete();
			}
			if ((Entity)(object)wepDealer.ped != (Entity)null)
			{
				((Entity)wepDealer.ped).Delete();
			}
		}
		if (aDealer != null)
		{
			if ((Entity)(object)aDealer.dPed != (Entity)null)
			{
				Blip currentBlip4 = ((Entity)aDealer.dPed).CurrentBlip;
				if (currentBlip4 != null)
				{
					currentBlip4.Remove();
				}
				((Entity)aDealer.dPed).Delete();
			}
			if ((Entity)(object)aDealer.cPed != (Entity)null)
			{
				Blip currentBlip5 = ((Entity)aDealer.cPed).CurrentBlip;
				if (currentBlip5 != null)
				{
					currentBlip5.Remove();
				}
				((Entity)aDealer.cPed).Delete();
			}
		}
		if (DealerHandler.dealers.Count > 0)
		{
			foreach (PlayerDealer pd in DealerHandler.dealers)
			{
				if ((Entity)(object)pd.Ped != (Entity)null)
				{
					LsFunctions.NotImmuneToPlayer(pd.Ped);
					Ped cPed = pd.cPed;
					if (cPed != null)
					{
						Blip currentBlip6 = ((Entity)cPed).CurrentBlip;
						if (currentBlip6 != null)
						{
							currentBlip6.Remove();
						}
					}
					Ped cPed2 = pd.cPed;
					if (cPed2 != null)
					{
						((Entity)cPed2).Delete();
					}
					Ped ped2 = pd.Ped;
					if (ped2 != null)
					{
						Blip currentBlip7 = ((Entity)ped2).CurrentBlip;
						if (currentBlip7 != null)
						{
							currentBlip7.Remove();
						}
					}
					((Entity)pd.Ped).Delete();
				}
				else
				{
					pd.RemoveLocationBlip();
				}
			}
		}
		World.RemoveRelationshipGroup(rival_nutral);
		World.RemoveRelationshipGroup(rival_enemy);
		World.RemoveRelationshipGroup(CustomerRel);
		driveByHandler.CleanDriveBys();
		if (pigs.Count > 0)
		{
			foreach (Pig pig in pigs)
			{
				Ped ped3 = pig.Ped;
				if (ped3 != null)
				{
					Blip currentBlip8 = ((Entity)ped3).CurrentBlip;
					if (currentBlip8 != null)
					{
						currentBlip8.Remove();
					}
				}
				Ped ped4 = pig.Ped;
				if (ped4 != null)
				{
					((Entity)ped4).Delete();
				}
				Vehicle vehicle = pig.Vehicle;
				if (vehicle != null)
				{
					Blip currentBlip9 = ((Entity)vehicle).CurrentBlip;
					if (currentBlip9 != null)
					{
						currentBlip9.Remove();
					}
				}
				((Entity)pig.Vehicle).Delete();
			}
		}
		pickupHandler.RemoveBags();
		OldCustomerHandler.ClearOldCustomers();
	}

	static LSL()
	{
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
	}
}
