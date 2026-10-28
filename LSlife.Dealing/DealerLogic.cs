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
	private void HandlePotentialWorker()
	{
		if (PottentialWorker != null)
		{
			PottentialWorker.OnTick();
			if (PottentialWorker.Destroy)
			{
				PottentialWorker = null;
			}
		}
	}

	public static void callDealer(bool wepdealer)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (wepdealer)
		{
			Vector3 dealer1pos = World.GetNextPositionOnSidewalk(GenerateSpawnPos(playerPos.Around(60f), Nodetype.AnyRoad, sidewalk: true));
			weaponDealer1 = World.CreatePed(new Model(unchecked((PedHash)(-236444766))), GenerateSpawnPos(dealer1pos, Nodetype.AnyRoad, sidewalk: true));
			while ((Entity)(object)weaponDealer1 == (Entity)null)
			{
				Script.Wait(0);
			}
			pedTarget = playerPos;
			if ((Entity)(object)weaponDealer1 != (Entity)null)
			{
				weaponDealer1.RelationshipGroup = playerRelationship;
				LsFunctions.AddBlip((Entity)(object)weaponDealer1, (BlipColor)3, "Zee's Dealer", _friend: true, _edge: true);
				weaponDealer1.CanSwitchWeapons = true;
				weaponDealer1.Weapons.Give((WeaponHash)453432689, 9999, false, true);
				weaponDealer1.Armor = 100;
				Function.Call(unchecked((Hash)(-4079649877180351064L)), (weaponDealer1), (100));
				readyToDeal = false;
				Script.Wait(100);
				weaponDealer1.Task.GoTo(pedTarget);
				dealerTimer = Game.GameTime;
				LSLifeSave = XDocument.Load("scripts\\LSLife\\LSLifeSave.xml");
				foreach (XElement element in LSLifeSave.Descendants())
				{
					if (!(element.Name == "dealer1"))
					{
						continue;
					}
					foreach (XElement e in element.Elements())
					{
						if (e.Name == "debt")
						{
							int.TryParse(e.Value, out dealer1Debt);
						}
						if (e.Name == "weed")
						{
							int.TryParse(e.Value, out dealer1Weed);
						}
						if (e.Name == "crack")
						{
							int.TryParse(e.Value, out dealer1Crack);
						}
						if (e.Name == "cocain")
						{
							int.TryParse(e.Value, out dealer1Cocain);
						}
						if (e.Name == "money")
						{
							int.TryParse(e.Value, out dealer1Money);
						}
						if (e.Name == "reloaded")
						{
							bool.TryParse(e.Value, out dealer1Reloaded);
						}
					}
				}
				
				
				
			}
			else
			{
				LsFunctions.SendTextMsg("~r~Impossible de faire apparaître le vendeur d'armes.");
			}
		}
		else if (ZeesDealers == null && NewOrder != null)
		{
			ZeesDealers = new DrugDealer(NewOrder);
			if (ZeesDealers != null)
			{

			}
		}
		else
		{
			LsFunctions.TextMsg("ZEE", "Commande", "Je leur dirai que tu ne viens pas.");
			ZeesDealers.Dismiss();
			if (NewOrder != null)
			{
				NewOrder.PutBackDrugs();
				NewOrder.ResetOrder();
			}
			CustomZeeMenu.Close();
		}
	}
}
