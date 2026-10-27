namespace LSlife;

// Gestion des commandes Zee sans lecture ni ecriture de controles NativeUI.
internal static class ZeeDrugOrderActions
{
    internal static bool AddWeed(int amount)
    {
        if (LSL.NewOrder == null || LSL.dealer1Weed <= 0 || amount <= 0 || amount > LSL.dealer1Weed) return false;
        int total = amount * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price));
        LSL.NewOrder.OrderWeed(amount, total);
        LSL.dealer1Weed -= amount;
        LsFunctions.DbugString("Weed Order changed");
        return true;
    }

    internal static bool AddCrack(int amount)
    {
        if (LSL.NewOrder == null || LSL.dealer1Crack <= 0 || amount <= 0 || amount > LSL.dealer1Crack) return false;
        int total = amount * (LSL.crack1Price - LsFunctions.Discount(LSL.crack1Price));
        LSL.NewOrder.OrderCrack(amount, total);
        LSL.dealer1Crack -= amount;
        LsFunctions.DbugString("Crack Order changed");
        return true;
    }

    internal static bool AddCocaine(int amount)
    {
        if (LSL.NewOrder == null || LSL.dealer1Cocain <= 0 || amount <= 0 || amount > LSL.dealer1Cocain) return false;
        int total = amount * (LSL.cocain1Price - LsFunctions.Discount(LSL.cocain1Price));
        LSL.NewOrder.OrderCocaine(amount, total);
        LSL.dealer1Cocain -= amount;
        LsFunctions.DbugString("Cocaine Order changed");
        return true;
    }
}
