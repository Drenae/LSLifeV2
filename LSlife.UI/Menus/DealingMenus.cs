namespace LSlife;

internal static partial class LsMenus
{
    internal static void StartDealing(bool weed, bool crack, bool cocaine, bool deliveries)
        => StreetDealingActions.StartDealing(weed, crack, cocaine, deliveries);

    internal static bool PlaceDrugOrder() => StreetDealingActions.PlaceDrugOrder();

    internal static bool HandleCustomerTransaction(bool sale)
        => CustomerTransactionActions.HandleCustomerTransaction(sale);
}
