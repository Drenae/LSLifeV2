namespace LSlife;

// Gestion des commandes Zee sans lecture ni ecriture de controles NativeUI.
internal static class ZeeDrugOrderActions
{
    internal static decimal DiscountedPricePerGram(int price) =>
        (price - LsFunctions.Discount(price)) / LsFunctions.GramsPerOunce;

    internal static bool AddWeed(decimal amount)
    {
        if (LSL.NewOrder == null || LSL.dealerWeedGrams <= 0 || amount <= 0 || amount > LSL.dealerWeedGrams) return false;
        decimal total = amount * DiscountedPricePerGram(LSL.weed1Price);
        LSL.NewOrder.OrderWeed(amount, total);
        LSL.dealerWeedGrams -= amount;
        LsFunctions.DbugString("Weed Order changed");
        return true;
    }

    internal static bool AddCrack(decimal amount)
    {
        if (LSL.NewOrder == null || LSL.dealerCrackGrams <= 0 || amount <= 0 || amount > LSL.dealerCrackGrams) return false;
        decimal total = amount * DiscountedPricePerGram(LSL.crack1Price);
        LSL.NewOrder.OrderCrack(amount, total);
        LSL.dealerCrackGrams -= amount;
        LsFunctions.DbugString("Crack Order changed");
        return true;
    }

    internal static bool AddCocaine(decimal amount)
    {
        if (LSL.NewOrder == null || LSL.dealerCocaineGrams <= 0 || amount <= 0 || amount > LSL.dealerCocaineGrams) return false;
        decimal total = amount * DiscountedPricePerGram(LSL.cocain1Price);
        LSL.NewOrder.OrderCocaine(amount, total);
        LSL.dealerCocaineGrams -= amount;
        LsFunctions.DbugString("Cocaine Order changed");
        return true;
    }
}
