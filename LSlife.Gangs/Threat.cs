using GTA;
using GTA.Math;

namespace LSlife.Gangs;

internal class Threat
{
	internal Area Area;

	private Vector3 Pos;

	private Ped TargetPed;

	public bool Responded = false;

	public int Level { get; private set; }

	public bool Responding { get; private set; }

	public int Allocated { get; private set; }

	public Threat(Area area, Vector3 pos, Ped targetPed, int level)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Area = area;
		Pos = pos;
		Level = level;
		TargetPed = targetPed;
	}

	public bool HasMembersForResponce(int _Members)
	{
		return _Members >= Level;
	}

	public void SetResponding(bool _resp, int _alloc)
	{
		Responding = _resp;
		Allocated = _alloc;
	}
}
