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
	public enum PedVariationData
	{
		PED_VARIATION_FACE,
		PED_VARIATION_HEAD,
		PED_VARIATION_HAIR,
		PED_VARIATION_TORSO,
		PED_VARIATION_LEGS,
		PED_VARIATION_HANDS,
		PED_VARIATION_FEET,
		PED_VARIATION_EYES,
		PED_VARIATION_ACCESSORIES,
		PED_VARIATION_TASKS,
		PED_VARIATION_TEXTURES,
		PED_VARIATION_TORSO2
	}

	public enum AreaType
	{
		Poor,
		Normal,
		Rich
	}

	public enum jurisdictionType
	{
		LSPD,
		LSSD,
		BCSO,
		SAPR
	}

	public class Door
	{
		private readonly bool DEBUG = true;

		public Prop Prop;

		private bool Locked;

		public Door(Prop prop)
		{
			Prop = prop;
			((Entity)Prop).IsInvincible = true;
			UseLock();
		}

		public void UseLock()
		{
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			Model model;
			if (Locked)
			{
				Locked = false;
				Script.Wait(10);
				Function.Call((Hash)3423943577717663365L, ("BIG_SCORE_GOLD_CAGE_LOCK"), (0));
				GTA.Native.Function.Call(GTA.Native.Hash.PLAY_SOUND_FRONTEND, -1, "0x08053C9C", "BIG_SCORE_GOLD_CAGE_LOCK", true);
				model = ((Entity)Prop).Model;
				Function.Call(unchecked((Hash)(-7272475972733243984L)), (model.Hash), (((Entity)Prop).Position.X), (((Entity)Prop).Position.Y), (((Entity)Prop).Position.Z), (false), (0f), (50f), (0f));
				if (DEBUG)
				{
					model = ((Entity)Prop).Model;
					GTA.UI.Notification.PostTicker(model.Hash + " Porte déverrouillée " + ((object)((Entity)Prop).Position/*cast due to constrained. prefix*/).ToString());
				}
			}
			else
			{
				Locked = true;
				Script.Wait(10);
				GTA.Native.Function.Call(GTA.Native.Hash.PLAY_SOUND_FRONTEND, -1, "Bar_Lower_And_Lock", "DLC_IND_ROLLERCOASTER_SOUNDS", true);
				model = ((Entity)Prop).Model;
				Function.Call(unchecked((Hash)(-7272475972733243984L)), (model.Hash), (((Entity)Prop).Position.X), (((Entity)Prop).Position.Y), (((Entity)Prop).Position.Z), (true), (0f), (50f), (0f));
				if (DEBUG)
				{
					model = ((Entity)Prop).Model;
					GTA.UI.Notification.PostTicker(model.Hash + " Porte verrouillée " + ((object)((Entity)Prop).Position/*cast due to constrained. prefix*/).ToString());
				}
			}
		}
	}

	public enum Nodetype
	{
		AnyRoad,
		Road,
		Offroad,
		Water
	}

	public enum PathnodeFlags
	{
		Slow = 1,
		Two = 2,
		Intersection = 4,
		Eight = 8,
		SlowTraffic = 12,
		ThirtyTwo = 32,
		Freeway = 64,
		FourWayIntersection = 128,
		BigIntersectionLeft = 512
	}
}
