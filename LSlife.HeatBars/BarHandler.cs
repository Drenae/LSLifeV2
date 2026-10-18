using NativeUI;

namespace LSlife.HeatBars;

internal static class BarHandler
{
	private static TimerBarPool BarPool = new TimerBarPool();

	public static void Tick()
	{
		AreaHeatBar.Tick();
		PlayerHeatBar.Tick();
		if (BarPool.ToList().Count > 0)
		{
			BarPool.Draw();
		}
	}

	public static void AddBar(BarTimerBar _bar)
	{
		BarPool.Add((TimerBarBase)(object)_bar);
	}

	public static void RemoveBar(BarTimerBar _bar)
	{
		BarPool.Remove((TimerBarBase)(object)_bar);
	}

	static BarHandler()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected Obj, but got Unknown
	}
}
