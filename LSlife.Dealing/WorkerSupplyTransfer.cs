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
        if (supplyWeed + supplyCrack + supplyCocaine <= 0 ||
            supplyWeed > SupplyAvailable("Weed") ||
            supplyCrack > SupplyAvailable("Crack") ||
            supplyCocaine > SupplyAvailable("Cocaine")) return false;

        LsFunctions.HandOverGoods(LSL.player.Character, worker.Ped);
        if (supplyWeed > 0) { worker.Drugs["Weed"] += supplyWeed; LSL.PlayerInventory["Weed"] -= supplyWeed; }
        if (supplyCrack > 0) { worker.Drugs["Crack"] += supplyCrack; LSL.PlayerInventory["Crack"] -= supplyCrack; }
        if (supplyCocaine > 0) { worker.Drugs["Cocaine"] += supplyCocaine; LSL.PlayerInventory["Cocaine"] -= supplyCocaine; }
        LsFunctions.UpdateInventory();
        worker.SetDrugDesc();
        LSL.DealerHandler.needsSave = true;
        return true;
    }

    internal static int SupplyAvailable(string key)
    {
        int value;
        return LSL.PlayerInventory.TryGetValue(key, out value) ? Math.Max(0, value) : 0;
    }
}
