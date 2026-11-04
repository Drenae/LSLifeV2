using GTA;

namespace LSlife;

// Gameplay d'equipement d'employe independant des menus NativeUI.
internal static class WorkerArmourTransfer
{
    internal static void GiveWorkerArmour()
    {
        PlayerDealer dealer = LSL.DealerHandler.currentDealer;
        if (dealer == null || !LsFunctions.IsLivePed(dealer.Ped) || dealer.Ped.Armor >= 200 ||
            LSL.playerArmours <= 0) return;
        Ped ped = dealer.Ped;
        if (!LsFunctions.HandOverGoods(LSL.player.Character, ped) ||
            LSL.DealerHandler.currentDealer != dealer || dealer.Ped != ped) return;
        if (LSL.playerArmours <= 0 || ped.Armor >= 200) return;
        ped.Armor = 200;
        dealer.dArmour = 200;
        LSL.playerArmours--;
        LsFunctions.PlayTheSound();
        LsFunctions.UpdateInventory();
        LSL.DealerHandler.needsSave = true;
    }

}
