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
	private bool IsNight()
	{
		int hour = GTA.Chrono.GameClock.Hour;
		if (hour <= 20)
		{
			return hour < 5;
		}
		return true;
	}

	private static void LoadAreaData()
	{
		foreach (XElement aEle in XDocument.Load("scripts\\LSLife\\LSLifeAreas.xml").Descendants())
		{
			if (!(aEle.Name == "area"))
			{
				continue;
			}
			string name = "";
			int cStr = -1;
			int gstr = -1;
			int heat = -1;
			float rep = -1f;
			foreach (XElement aEl in aEle.Elements())
			{
				if (aEl.Name == "name")
				{
					name = aEl.Value;
				}
				if (aEl.Name == "cStr")
				{
					int.TryParse(aEl.Value, out cStr);
				}
				if (aEl.Name == "gStr")
				{
					int.TryParse(aEl.Value, out gstr);
				}
				if (aEl.Name == "aHeat")
				{
					int.TryParse(aEl.Value, out heat);
				}
				if (aEl.Name == "aRep")
				{
					float.TryParse(aEl.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out rep);
				}
			}
			if (name != "" && cStr != -1 && gstr != -1)
			{
				AreaType type = AreaType.Normal;
				if (zoneData.ContainsKey(name))
				{
					type = zoneData[name].Item1;
				}
				else
				{
					GTA.UI.Notification.PostTicker("~r~LsLife - Impossible de déterminer le type du quartier : " + name, false, false);
				}
				areas.Add(new Area(name, LsFunctions.LsDrugs(), type, cStr, gstr, heat, rep));
			}
		}
	}

	private void SavedDetected()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Function.Call<bool>((Hash)7576188390707304864L);
		bool SaveHud = GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_HUD_COMPONENT_ACTIVE, 17);
		bool StatSave = Function.Call<bool>(unchecked((Hash)(-4920555325479993693L)));
		bool Paused = Function.Call<bool>(unchecked((Hash)(-5763681587332382773L)));
		bool sPaused = Function.Call<bool>(unchecked((Hash)(-1572002141673951856L)));
		bool Busy = Function.Call<bool>(unchecked((Hash)(-5573887678010054197L)));
		if (flag | SaveHud | StatSave | Paused | sPaused | Busy)
		{
			pSaved = true;
		}
		new GTA.UI.TextElement(pSaved.ToString(), new PointF(640, 360), 1f).Draw();
	}

	public void HandleAreaDealers()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (aDealer != null && aDealer.NewState == Enums.DStates.TalkToPlayer && aDealer.CurrentState == Enums.DStates.TalkToPlayer && player.Character.IsStopped)
		{
			Vector3 position = ((Entity)aDealer.dPed).Position;
			if (position.DistanceTo(playerPos) < 2f)
			{
				aDealer.PromtPlayerToTalk();
			}
		}
	}
}
