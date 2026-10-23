using NativeUI;

namespace LSlife.HeatBars;

// Gestion du pool de barres ; leur apparence et leur rendu sont dans Hud.cs.
internal static class BarHandler
{
	internal static readonly TimerBarPool Pool = new TimerBarPool();

	public static void AddBar(BarTimerBar bar)
	{
		Pool.Add(bar);
	}

	public static void RemoveBar(BarTimerBar bar)
	{
		Pool.Remove(bar);
	}
}
