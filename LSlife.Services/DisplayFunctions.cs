using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using iFruitAddon2;
using LSlife.OldCustomers;
using LSlife.Shots;


namespace LSlife;

internal static partial class LsFunctions
{
	public static void SendTextMsg(string str)
	{
		GTA.Native.Function.Call(GTA.Native.Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet_Default", true);
		GTA.UI.Notification.PostTicker(str);
	}

	public static void TextMsg(string _name, string _title, string _msg)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!HeadShotHandler.headShots.ContainsKey(_name))
		{
			Ped ped = Ped.Create(RequestModel(PedModels[_name]), new Vector3(0f, 0f, 0f));
			while ((Entity)(object)ped == (Entity)null)
			{
				Script.Yield();
			}
			HeadShotHandler.MugShot(ped, _name);
			((Entity)ped).Delete();
		}
		GTA.Native.Function.Call(GTA.Native.Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet_Default", true);
		Function.Call((Hash)2316831480196236324L, ("CELL_EMAIL_BCON"));
		Function.Call((Hash)7789129354908300458L, (_msg));
		Function.Call((Hash)2075484565200204495L, (HeadShotHandler.headShots[_name].Texture), (HeadShotHandler.headShots[_name].Texture), (true), (1), (_title), (_name));
		Script.Yield();
	}

	public static void TextMsg(string _name, Ped _ped, string _title, string _msg)
	{
		if (!HeadShotHandler.headShots.ContainsKey(_name))
		{
			HeadShotHandler.MugShot(_ped, _name);
		}
		else if (HeadShotHandler.headShots.Where((KeyValuePair<string, HeadShot> r) =>
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			PedHash pedHash = r.Value.PedHash;
			Model model = ((Entity)_ped).Model;
			return (int)pedHash == model.Hash;
		}).Count() == 0)
		{
			HeadShotHandler.DeleteHeadShot(_name);
			HeadShotHandler.MugShot(_ped, _name);
		}
		GTA.Native.Function.Call(GTA.Native.Hash.PLAY_SOUND_FRONTEND, -1, "Text_Arrive_Tone", "Phone_SoundSet_Default", true);
		Function.Call((Hash)2316831480196236324L, ("CELL_EMAIL_BCON"));
		Function.Call((Hash)7789129354908300458L, (_msg));
		Function.Call((Hash)2075484565200204495L, (HeadShotHandler.headShots[_name].Texture), (HeadShotHandler.headShots[_name].Texture), (true), (1), (_title), (_name));
		Script.Yield();
	}

	public static void DisplayHelpText(string text)
	{
		InputArgument[] arguments = new InputArgument[1] { ("STRING") };
		Function.Call(unchecked((Hash)(-8860350453193909743L)), arguments);
		InputArgument[] argumentArray2 = new InputArgument[1] { (text) };
		Function.Call((Hash)7789129354908300458L, argumentArray2);
		InputArgument[] argumentArray3 = new InputArgument[4]
		{
			(0),
			(0),
			(1),
			(-1)
		};
		Function.Call((Hash)2562546386151446694L, argumentArray3);
	}

	public static void PlayTheSound()
	{
		GTA.Native.Function.Call(GTA.Native.Hash.PLAY_SOUND_FRONTEND, -1, "LOCAL_PLYR_CASH_COUNTER_INCREASE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", true);
	}

	public static void DbugString(string _string)
	{
		if (LSL.DEBUG)
		{
			GTA.UI.Notification.PostTicker(_string);
		}
	}

	public static string IntToMoney(int _money)
	{
		return "~g~" + _money.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", " ") + "~s~";
	}
}
