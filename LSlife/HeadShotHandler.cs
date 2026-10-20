using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;

namespace LSlife;

internal static class HeadShotHandler
{
	public static Dictionary<string, HeadShot> headShots = new Dictionary<string, HeadShot>();

	public static void HandleHeadShots()
	{
		if (headShots.Count <= 0)
		{
			return;
		}
		foreach (HeadShot headShot in headShots.Values)
		{
			if (Game.GameTime > headShot.timeCreated + 4000)
			{
				DeleteHeadShot(headShot.Handle);
			}
			else if (!Function.Call<bool>((Hash)8107924672780253799L, InputArgument.op_Implicit(headShot.Handle)) || !Function.Call<bool>((Hash)(-6869846991849838174L), InputArgument.op_Implicit(headShot.Handle)))
			{
				DeleteHeadShot(headShot.Handle);
			}
		}
	}

	public static void DeleteHeadShot(string _name)
	{
		Function.Call((Hash)(-7588224396357745921L), InputArgument.op_Implicit(headShots[_name].Handle));
		headShots.Remove(_name);
	}

	private static void DeleteHeadShot(int _handle)
	{
		Function.Call((Hash)(-7588224396357745921L), InputArgument.op_Implicit(_handle));
		headShots.Remove(headShots.First((KeyValuePair<string, HeadShot> r) => r.Value.Handle == _handle).Key);
	}

	public static bool HeadShotValid(HeadShot _h)
	{
		if (Function.Call<bool>((Hash)8107924672780253799L, InputArgument.op_Implicit(_h.Handle)) && Function.Call<bool>((Hash)(-6869846991849838174L), InputArgument.op_Implicit(_h.Handle)))
		{
			return true;
		}
		return false;
	}

	public static void MugShot(Ped _ped, string _name)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		int handle = Function.Call<int>((Hash)4927612575077781622L, InputArgument.op_Implicit(_ped));
		while (!Function.Call<bool>((Hash)8107924672780253799L, InputArgument.op_Implicit(handle)) || !Function.Call<bool>((Hash)(-6869846991849838174L), InputArgument.op_Implicit(handle)))
		{
			Script.Yield();
		}
		string texture = Function.Call<string>((Hash)(-2643985901783720597L), InputArgument.op_Implicit(handle));
		Dictionary<string, HeadShot> dictionary = headShots;
		Model model = ((Entity)_ped).Model;
		dictionary.Add(_name, new HeadShot(handle, texture, (PedHash)model.Hash));
	}
}
