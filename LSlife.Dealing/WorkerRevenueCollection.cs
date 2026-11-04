using GTA;

namespace LSlife;

// Encaissement de la recette des employes, independant de NativeUI.
internal static class WorkerRevenueCollection
{
    internal static void CollectWorker()
    {
        if (LSL.DealerHandler.currentDealer == null ||
            LSL.DealerHandler.currentDealer.Money < 100) return;
        PlayerDealer worker = LSL.DealerHandler.currentDealer;
        if (worker.Ped == null) return;
        Ped ped = worker.Ped;
        if (!LsFunctions.HandOverGoods(LSL.player.Character, ped) ||
            LSL.DealerHandler.currentDealer != worker || worker.Ped != ped) return;
        worker.GiveMoneyToPlayer();
        LSL.DealerHandler.needsSave = true;
    }
}
