using GTA;
using GTA.Native;

namespace LSlife;

internal class HeadShot
{
	internal int Handle;

	internal string Texture;

	internal PedHash PedHash;

	internal int timeCreated;

	public HeadShot(int _handle, string _texure, PedHash _hash)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Handle = _handle;
		Texture = _texure;
		PedHash = _hash;
		timeCreated = Game.GameTime;
	}
}
