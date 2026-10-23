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

// Méthodes déplacées sans modification de leur logique.
public partial class LSL
{
	private void areaInfoUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected Obj, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected Obj, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected Obj, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected Obj, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected Obj, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected Obj, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected Obj, but got Unknown
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected Obj, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Expected Obj, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Expected Obj, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Expected Obj, but got Unknown
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Expected Obj, but got Unknown
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Expected Obj, but got Unknown
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Expected Obj, but got Unknown
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Expected Obj, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Expected Obj, but got Unknown
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Expected Obj, but got Unknown
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		Point point = new Point(10, 10);
		Size size = new Size(80, 184);
		UIContainer val = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--QUARTIER--", new Point(2, 2), 0.2f);
		UIText name = new UIText("nom : " + areas[areaIndex].Name, new Point(2, 12), 0.2f);
		UIText type = new UIText("type : " + areas[areaIndex].AreaType, new Point(2, 22), 0.2f);
		UIText cop = new UIText("cStr:" + areas[areaIndex].CopPresance, new Point(2, 32), 0.2f);
		UIText gang = new UIText("gStr:" + areas[areaIndex].GangPresance, new Point(2, 42), 0.2f);
		UIText heat = new UIText("pression : " + areas[areaIndex].Heat, new Point(2, 52), 0.2f);
		UIText dealers = new UIText("nDealer:" + areas[areaIndex].Dealers(), new Point(2, 62), 0.2f);
		UIText demandMet = new UIText("dMet:" + areas[areaIndex].DemandMet(), new Point(2, 72), 0.2f);
		UIText aRep = new UIText("aRep:" + areas[areaIndex].Reputation + " Lvl :" + (int)LsFunctions.CurrentRepLvl(areas[areaIndex].Reputation), new Point(2, 82), 0.2f);
		UIText drug0H = new UIText("--" + LsFunctions.DrugDisplayName(areas[areaIndex].Drugs[0].Name) + "--", new Point(2, 92), 0.2f);
		UIText drug0D = new UIText("Dem:" + areas[areaIndex].Drugs[0].Demand, new Point(2, 102), 0.2f);
		UIText drug0S = new UIText("Sup:" + areas[areaIndex].Drugs[0].Supplied, new Point(2, 112), 0.2f);
		UIText drug1H = new UIText("--" + LsFunctions.DrugDisplayName(areas[areaIndex].Drugs[1].Name) + "--", new Point(2, 122), 0.2f);
		UIText drug1D = new UIText("Dem:" + areas[areaIndex].Drugs[1].Demand, new Point(2, 132), 0.2f);
		UIText drug1S = new UIText("Sup:" + areas[areaIndex].Drugs[1].Supplied, new Point(2, 142), 0.2f);
		UIText drug2H = new UIText("--" + LsFunctions.DrugDisplayName(areas[areaIndex].Drugs[2].Name) + "--", new Point(2, 152), 0.2f);
		UIText drug2D = new UIText("Dem:" + areas[areaIndex].Drugs[2].Demand, new Point(2, 162), 0.2f);
		UIText drug2S = new UIText("Sup:" + areas[areaIndex].Drugs[2].Supplied, new Point(2, 172), 0.2f);
		val.Items.Add((UIElement)(object)aHeader);
		val.Items.Add((UIElement)(object)name);
		val.Items.Add((UIElement)(object)type);
		val.Items.Add((UIElement)(object)cop);
		val.Items.Add((UIElement)(object)gang);
		val.Items.Add((UIElement)(object)heat);
		val.Items.Add((UIElement)(object)dealers);
		val.Items.Add((UIElement)(object)demandMet);
		val.Items.Add((UIElement)(object)aRep);
		val.Items.Add((UIElement)(object)drug0H);
		val.Items.Add((UIElement)(object)drug0D);
		val.Items.Add((UIElement)(object)drug0S);
		val.Items.Add((UIElement)(object)drug1H);
		val.Items.Add((UIElement)(object)drug1D);
		val.Items.Add((UIElement)(object)drug1S);
		val.Items.Add((UIElement)(object)drug2H);
		val.Items.Add((UIElement)(object)drug2D);
		val.Items.Add((UIElement)(object)drug2S);
		((UIRectangle)val).Draw();
	}

	private void ZeeInfoUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected Obj, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected Obj, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected Obj, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected Obj, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected Obj, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected Obj, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		Point point = new Point(10, 194);
		Size size = new Size(80, 104);
		UIContainer val = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--Zee--", new Point(2, 2), 0.2f);
		UIText money = new UIText("argent : " + dealer1Money, new Point(2, 12), 0.2f);
		UIText reload = new UIText("reload:" + dealer1ReloadOrder, new Point(2, 22), 0.2f);
		UIText reloaded = new UIText("reloaded:" + dealer1Reloaded, new Point(2, 32), 0.2f);
		UIText weed = new UIText("cannabis : " + dealer1Weed, new Point(2, 42), 0.2f);
		UIText crack = new UIText("crack:" + dealer1Crack, new Point(2, 52), 0.2f);
		UIText coke = new UIText("cocaïne : " + dealer1Cocain, new Point(2, 62), 0.2f);
		val.Items.Add((UIElement)(object)aHeader);
		val.Items.Add((UIElement)(object)money);
		val.Items.Add((UIElement)(object)reload);
		val.Items.Add((UIElement)(object)reloaded);
		val.Items.Add((UIElement)(object)weed);
		val.Items.Add((UIElement)(object)crack);
		val.Items.Add((UIElement)(object)coke);
		((UIRectangle)val).Draw();
	}

	private void PlayerUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected Obj, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected Obj, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected Obj, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected Obj, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected Obj, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected Obj, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected Obj, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected Obj, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected Obj, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected Obj, but got Unknown
		Point point = new Point(10, 294);
		Size size = new Size(80, 100);
		UIContainer cont = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--JOUEUR--", new Point(2, 2), 0.2f);
		UIText heat = new UIText("pression : " + playerHeat, new Point(2, 12), 0.2f);
		UIText pDealing = new UIText("vente : " + isDealing, new Point(2, 22), 0.2f);
		UIText tPeds = new UIText("tPeds:" + lPeds.GetPeds.Count, new Point(2, 32), 0.2f);
		UIText oPeds = new UIText("oPeds:" + OldCustomerHandler.AmountOf(), new Point(2, 42), 0.2f);
		cont.Items.Add((UIElement)(object)aHeader);
		cont.Items.Add((UIElement)(object)heat);
		cont.Items.Add((UIElement)(object)pDealing);
		cont.Items.Add((UIElement)(object)tPeds);
		cont.Items.Add((UIElement)(object)oPeds);
		if (wepDealer != null)
		{
			UIText dSpeed = new UIText("dSpeed:" + wepDealer.maxSpeed + "|" + wepDealer.ped.CurrentVehicle.Speed, new Point(2, 52), 0.2f);
			cont.Items.Add((UIElement)(object)dSpeed);
			UIText cSpeed = new UIText("cSpeed:" + wepDealer.canSpeed, new Point(2, 62), 0.2f);
			cont.Items.Add((UIElement)(object)cSpeed);
			UIText sSpeed = new UIText("sSpeed:" + wepDealer.shouldSpeed, new Point(2, 72), 0.2f);
			cont.Items.Add((UIElement)(object)sSpeed);
		}
		int drvAmount = driveByHandler.GetDriveBies.Count;
		if (drvAmount > 0)
		{
			UIText drvAmou = new UIText("drvAmount:" + drvAmount, new Point(2, 82), 0.2f);
			cont.Items.Add((UIElement)(object)drvAmou);
		}
		((UIRectangle)cont).Draw();
	}

	private void GangUI()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected Obj, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected Obj, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected Obj, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected Obj, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected Obj, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected Obj, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected Obj, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected Obj, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		Gang gang = areas[areaIndex].Gang;
		Point point = new Point(10, 394);
		Size size = new Size(80, 100);
		UIContainer val = new UIContainer(point, size, Color.FromArgb(100, Color.Black));
		UIText aHeader = new UIText("--Gang--", new Point(2, 2), 0.2f);
		UIText gName = new UIText("nom : " + gang.Name, new Point(2, 12), 0.2f);
		UIText gMoney = new UIText("argent : " + gang.Money, new Point(2, 22), 0.2f);
		UIText gMax = new UIText("max:" + gang.MaxMembers, new Point(2, 32), 0.2f);
		UIText gTotal = new UIText("total : " + gang.TotalMembers, new Point(2, 42), 0.2f);
		UIText gThreat = new UIText("threats:" + gang.ThreatHandler.ThreatCount, new Point(2, 52), 0.2f);
		UIText gFree = new UIText("free:" + GangUtilities.AllocatedMemberCount(gang), new Point(2, 62), 0.2f);
		UIText night = new UIText("nuit : " + IsNight(), new Point(2, 72), 0.2f);
		UIText shots = new UIText("shots: " + ShotHandler.shots.Count, new Point(2, 82), 0.2f);
		val.Items.Add((UIElement)(object)aHeader);
		val.Items.Add((UIElement)(object)gName);
		val.Items.Add((UIElement)(object)gMoney);
		val.Items.Add((UIElement)(object)gMax);
		val.Items.Add((UIElement)(object)gTotal);
		val.Items.Add((UIElement)(object)gFree);
		val.Items.Add((UIElement)(object)gThreat);
		val.Items.Add((UIElement)(object)night);
		val.Items.Add((UIElement)(object)shots);
		((UIRectangle)val).Draw();
	}
}
