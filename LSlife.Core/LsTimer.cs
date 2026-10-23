using GTA;

namespace LSlife;

public class LsTimer
{
	private int Time;

	private int TotalTime;

	private bool TimeOut;

	public bool StartTimer;

	private int lastTime;

	public LsTimer(int time, bool timeOut, bool start)
	{
		TotalTime = time;
		TimeOut = timeOut;
		lastTime = Game.GameTime;
		StartTimer = start;
	}

	public void Tick()
	{
		if (StartTimer && Game.GameTime > lastTime + 1000)
		{
			lastTime = Game.GameTime;
			if (!Finished())
			{
				Time++;
			}
			else if (TimeOut)
			{
				Time = 0;
			}
		}
	}

	public bool Finished()
	{
		if (Time >= TotalTime)
		{
			return true;
		}
		return false;
	}

	public int Remaining()
	{
		if (Time < TotalTime)
		{
			return Time - TotalTime;
		}
		return 0;
	}

	public int Elapsed()
	{
		return Time;
	}

	public void Start()
	{
		StartTimer = true;
		lastTime = Game.GameTime;
	}

	public void Stop()
	{
		StartTimer = false;
	}

	public void Reset()
	{
		Time = 0;
		lastTime = Game.GameTime;
	}
}
