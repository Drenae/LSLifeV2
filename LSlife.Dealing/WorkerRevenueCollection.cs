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
        LsFunctions.HandOverGoods(LSL.player.Character, worker.Ped);
        worker.GiveMoneyToPlayer();
        worker.SetDrugDesc();
        LSL.DealerHandler.needsSave = true;
    }
}
