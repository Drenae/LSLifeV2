using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class PedDebugger
{
	private List<Ped> debugPeds = new List<Ped>();

	private int dTime;

	private int dInterval = 2000;

	public void OnTick()
	{
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		if (Game.GameTime > dTime + dInterval)
		{
			foreach (Ped p in World.GetAllPeds().ToList())
			{
				if (!debugPeds.Contains(p))
				{
					debugPeds.Add(p);
				}
			}
		}
		foreach (Ped p2 in debugPeds.ToList())
		{
			if ((Entity)(object)p2 != (Entity)null && ((Entity)p2).Exists())
			{
				if (((Entity)p2).IsOnScreen && ((Entity)p2).IsVisible)
				{
					bool allowed = false;
					if (!LSL.CheckIfPedUsed(p2, _allowPolice: true, _allowVehicle: true))
					{
						allowed = true;
					}
					string text = SetPedDebugtext((Entity)(object)p2, allowed);
					Vector2 tPos = LsFunctions.World3DToScreen2d(new Vector3(p2.GetBoneCoord((Bone)37193).X, p2.GetBoneCoord((Bone)37193).Y, p2.GetBoneCoord((Bone)37193).Z + 0.5f));
					tPos.X = 1280f * tPos.X;
					tPos.Y = 720f * tPos.Y;
					new UIText(text, new Point((int)tPos.X, (int)tPos.Y), 0.2f, Color.WhiteSmoke, (GTA.Font)0, true, true, true).Draw();
				}
			}
			else
			{
				debugPeds.Remove(p2);
			}
		}
		if (LSL.pStashVehicle != null && (Entity)(object)LSL.pStashVehicle.drugCar != (Entity)null && ((Entity)LSL.pStashVehicle.drugCar).Exists())
		{
			Vector2 tPos2 = LsFunctions.World3DToScreen2d(((Entity)LSL.pStashVehicle.drugCar).Position);
			tPos2.X = 1280f * tPos2.X;
			tPos2.Y = 720f * tPos2.Y;
			new UIText(SetPedDebugtext((Entity)(object)LSL.pStashVehicle.drugCar, allowed: true), new Point((int)tPos2.X, (int)tPos2.Y), 0.2f, Color.WhiteSmoke, (GTA.Font)0, true, true, true).Draw();
		}
	}

	private static string SetPedDebugtext(Entity p, bool allowed)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (LSL.pStashVehicle != null && p == (Entity)(object)LSL.pStashVehicle.drugCar)
		{
			Entity val = p;
			Vehicle v = (Vehicle)(object)((val is Vehicle) ? val : null);
			return p.Health + " | " + LSL.pStashVehicle.drugCarDestroyed + " | " + ((Entity)LSL.pStashVehicle.drugCar).IsDead + " | " + v.BodyHealth + " | " + v.EngineHealth + " | " + v.IsDriveable;
		}
		if (p is Ped)
		{
			Entity val2 = p;
			if (((val2 is Ped) ? val2 : null) == (Entity)(object)LSL.player.Character)
			{
				string str = "";
				{
					foreach (LSL.PathnodeFlags roadFlag in LSL.GetRoadFlags(LSL.playerPos))
					{
						str = str + " " + roadFlag;
					}
					return str;
				}
			}
		}
		if (LSL.DealerHandler.dealers.Count > 0 && LSL.DealerHandler.dealers.Find((PlayerDealer playerDealer) => (Entity)(object)playerDealer.Ped == p) != null)
		{
			return LSL.DealerHandler.dealers.Find((PlayerDealer playerDealer) => (Entity)(object)playerDealer.Ped == p).NewState.ToString() + " > " + LSL.DealerHandler.dealers.Find((PlayerDealer playerDealer) => (Entity)(object)playerDealer.Ped == p).CurrentState;
		}
		if (LSL.aDealer != null)
		{
			if ((Entity)(object)LSL.aDealer.dPed != (Entity)null && p == (Entity)(object)LSL.aDealer.dPed)
			{
				foreach (Enums.eTaskTypeIndex task in Enum.GetValues(typeof(Enums.eTaskTypeIndex)))
				{
					if (Function.Call<bool>(unchecked((Hash)(-5731389963444272811L)), (p), ((int)task)))
					{
						return task.ToString();
					}
				}
			}
			else if ((Entity)(object)LSL.aDealer.cPed != (Entity)null && p == (Entity)(object)LSL.aDealer.cPed)
			{
				return LSL.aDealer.dealState.ToString();
			}
		}
		if (LSL.driveByHandler != null && LSL.driveByHandler.GetDriveBies.Count > 0)
		{
			foreach (DriveBy d in LSL.driveByHandler.GetDriveBies)
			{
				if (d.Peds.Count > 0 && (Entity)(object)d.Peds.Find((Ped r) => (Entity)(object)r == p) != (Entity)null)
				{
					return d.DriveBySate.ToString();
				}
			}
		}
		if (p == (Entity)(object)LSL.drugDealer1)
		{
			return LSL.dealer1Rep.ToString();
		}
		if (LSL.pigs.Count > 0)
		{
			foreach (Pig pig in LSL.pigs)
			{
				if ((Entity)(object)pig.Ped == p)
				{
					return pig.CurrentState.ToString();
				}
			}
		}
		return allowed.ToString();
	}
}
