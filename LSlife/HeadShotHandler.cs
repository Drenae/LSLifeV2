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
			else if (!Function.Call<bool>((Hash)8107924672780253799L, (InputArgument)(headShot.Handle)) || !Function.Call<bool>(unchecked((Hash)(-6869846991849838174L)), (InputArgument)(headShot.Handle)))
			{
				DeleteHeadShot(headShot.Handle);
			}
		}
	}

	public static void DeleteHeadShot(string _name)
	{
		Function.Call(unchecked((Hash)(-7588224396357745921L)), (InputArgument)(headShots[_name].Handle));
		headShots.Remove(_name);
	}

	private static void DeleteHeadShot(int _handle)
	{
		Function.Call(unchecked((Hash)(-7588224396357745921L)), (InputArgument)(_handle));
		headShots.Remove(headShots.First((KeyValuePair<string, HeadShot> r) => r.Value.Handle == _handle).Key);
	}

	public static bool HeadShotValid(HeadShot _h)
	{
		if (Function.Call<bool>((Hash)8107924672780253799L, (InputArgument)(_h.Handle)) && Function.Call<bool>(unchecked((Hash)(-6869846991849838174L)), (InputArgument)(_h.Handle)))
		{
			return true;
		}
		return false;
	}

	public static void MugShot(Ped _ped, string _name)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		int handle = Function.Call<int>((Hash)4927612575077781622L, (InputArgument)(_ped));
		while (!Function.Call<bool>((Hash)8107924672780253799L, (InputArgument)(handle)) || !Function.Call<bool>(unchecked((Hash)(-6869846991849838174L)), (InputArgument)(handle)))
		{
			Script.Yield();
		}
		string texture = Function.Call<string>(unchecked((Hash)(-2643985901783720597L)), (InputArgument)(handle));
		Dictionary<string, HeadShot> dictionary = headShots;
		Model model = ((Entity)_ped).Model;
		dictionary.Add(_name, new HeadShot(handle, texture, (PedHash)model.Hash));
	}
}
