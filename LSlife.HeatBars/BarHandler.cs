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
		BarPool.Add(_bar);
	}

	public static void RemoveBar(BarTimerBar _bar)
	{
		BarPool.Remove(_bar);
	}
}
