using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

internal static class DropDrugs
{
	public static void Drop()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (LSL.TotalDrugsCarried() <= 0)
		{
			return;
		}
		switch (LsFunctions.IsNearStash())
		{
		case 0:
		{
			Vector3 forwardPos = LSL.playerPos + ((Entity)LSL.player.Character).ForwardVector * 2f;
			Function.Call<Vector3>((Hash)7932734660826570736L, (InputArgument)(forwardPos.X), (InputArgument)(forwardPos.Y), (InputArgument)(forwardPos.Z), (InputArgument)(0), (InputArgument)(0));
			RaycastResult ray = World.Raycast(LSL.playerPos, forwardPos, (IntersectOptions)1);
			if (ray.DitHitAnything)
			{
				forwardPos = ray.HitCoords + ray.SurfaceNormal;
			}
			LSL.pickupHandler.AddBag(new DroppedBag("You have dropped " + LsFunctions.GramsToOz(LSL.TotalDrugsCarried()) + " of drugs!", LSL.PlayerInventory["Weed"], LSL.PlayerInventory["Crack"], LSL.PlayerInventory["Cocaine"], LsFunctions.pWeedOunce, LsFunctions.pCrackOunce, LsFunctions.pCocaineOunce, 0, forwardPos));
			LsFunctions.PlayTheSound();
			RemovePlayerDrugs();
			LsFunctions.UpdateInventory();
			break;
		}
		case 1:
			LSL.pStashVehicle.AddDrugs();
			LsFunctions.SendTextMsg("Drogue transférée dans le véhicule");
			break;
		case 2:
			LSL.HouseHandler.CurrentHouse.AddDrugs();
			LsFunctions.SendTextMsg("Drogue transférée dans la cachette");
			break;
		}
	}

	private static void RemovePlayerDrugs()
	{
		foreach (string drug in LSL.PlayerInventory.Keys.ToList())
		{
			LSL.PlayerInventory[drug] = 0;
		}
		LsFunctions.pCocaineOunce = 0;
		LsFunctions.pWeedOunce = 0;
		LsFunctions.pCrackOunce = 0;
	}
}
