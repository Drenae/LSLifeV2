using GTA;

namespace LSlife;

// Gameplay d'equipement d'employe independant des menus NativeUI.
internal static class WorkerArmourTransfer
{
    internal static void GiveWorkerArmour()
    {
        PlayerDealer dealer = LSL.DealerHandler.currentDealer;
        if (dealer == null || dealer.Ped == null || dealer.Ped.Armor >= 200 ||
            LSL.playerArmours <= 0) return;
        LsFunctions.HandOverGoods(LSL.player.Character, dealer.Ped);
        dealer.Ped.Armor = 200;
        dealer.dArmour = 200;
        LSL.playerArmours--;
        LsFunctions.PlayTheSound();
        LsFunctions.UpdateInventory();
        LSL.DealerHandler.needsSave = true;
    }

}
