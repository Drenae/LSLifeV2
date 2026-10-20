using System.Drawing;
using NativeUI;

namespace LSlife.HeatBars;

internal static class AreaHeatBar
{
	private static BarTimerBar aBar = new BarTimerBar("Area");

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
			BarHandler.AddBar(aBar);
			Displayed = true;
			aBar.BackgroundColor = Color.Black;
		}
	}

	public static void Tick()
	{
		if (Displayed)
		{
			aBar.Percentage = LSL.areas[LSL.areaIndex].Heat * 0.001f;
		}
	}
}
