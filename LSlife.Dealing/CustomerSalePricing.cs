namespace LSlife;

// Calcul du prix de vente client independant des menus NativeUI.
internal static class CustomerSalePricing
{
    internal static int SalePrice()
    {
        var c = LSL.customerSellTo;
        if (c == null || c.Drug == null) return 0;
        int price = c.amount * c.Drug.PricePerG;
        float divider = (float)c.amount / 7f;
        if (divider >= 1f)
            price -= (int)divider * c.Drug.PricePerG;
        return price;
    }

}
