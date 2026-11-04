using System;
using GTA;

namespace LSlife;

// Transfert de stock aux employes, sans types NativeUI ni navigation de menu.
internal static class WorkerSupplyTransfer
{
    internal static bool ConfirmWorkerSupply(int supplyWeed, int supplyCrack, int supplyCocaine)
    {
        PlayerDealer worker = LSL.DealerHandler.currentDealer;
        if (worker == null || worker.Ped == null) return false;
        if (!CanSupply(supplyWeed, supplyCrack, supplyCocaine)) return false;

        Ped ped = worker.Ped;
        if (!LsFunctions.HandOverGoods(LSL.player.Character, ped) ||
            LSL.DealerHandler.currentDealer != worker || worker.Ped != ped ||
            !CanSupply(supplyWeed, supplyCrack, supplyCocaine)) return false;
        if (supplyWeed > 0) { worker.Drugs["Weed"] += supplyWeed; LSL.PlayerInventory["Weed"] -= supplyWeed; }
        if (supplyCrack > 0) { worker.Drugs["Crack"] += supplyCrack; LSL.PlayerInventory["Crack"] -= supplyCrack; }
        if (supplyCocaine > 0) { worker.Drugs["Cocaine"] += supplyCocaine; LSL.PlayerInventory["Cocaine"] -= supplyCocaine; }
        LsFunctions.UpdateInventory();
        LSL.DealerHandler.needsSave = true;
        return true;
    }

    private static bool CanSupply(int weed, int crack, int cocaine) =>
        weed >= 0 && crack >= 0 && cocaine >= 0 && (weed > 0 || crack > 0 || cocaine > 0) &&
        weed <= SupplyAvailable("Weed") && crack <= SupplyAvailable("Crack") &&
        cocaine <= SupplyAvailable("Cocaine");

    internal static int SupplyAvailable(string key)
    {
        int value;
        return LSL.PlayerInventory.TryGetValue(key, out value) ? Math.Max(0, value) : 0;
    }
}
