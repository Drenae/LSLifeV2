using System.Drawing;
using NativeUI;

namespace LSlife.HeatBars;

internal static class PlayerHeatBar
{
	private static BarTimerBar aBar = new BarTimerBar("Player");

	public static bool Displayed = false;

	public static void Toggle()
	{
		if (Displayed)
		{
			BarHandler.RemoveBar(aBar);
			Displayed = false;
		}
		else
		{
			aBar.BackgroundColor = Color.Black;
			BarHandler.AddBar(aBar);
			Displayed = true;
		}
	}

	public static void Tick()
	{
		if (Displayed)
		{
			if (LSL.playerHeat > 0f)
			{
				aBar.Percentage = LSL.playerHeat * 0.01f;
			}
			else
			{
				aBar.Percentage = 0f;
			}
		}
	}

	static PlayerHeatBar()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected Obj, but got Unknown
	}
}
