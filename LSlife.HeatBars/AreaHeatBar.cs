using System.Drawing;
using NativeUI;

namespace LSlife.HeatBars;

internal static class AreaHeatBar
{
	private static BarTimerBar aBar = new BarTimerBar("Quartier");

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

	static AreaHeatBar()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected Obj, but got Unknown
	}
}
