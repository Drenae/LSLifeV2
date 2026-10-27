namespace LSlife;

// Mise a jour des commandes Zee sans parametres de callbacks NativeUI.
internal static class ZeeDrugOrderActions
{
	internal static void AddWeed()
	{
		if (LSL.dealer1Weed <= 0 || LSL.bushWeed.Value <= 0) return;
		int total = LSL.bushWeed.Value * (LSL.weed1Price - LsFunctions.Discount(LSL.weed1Price));
		LSL.NewOrder.OrderWeed(LSL.bushWeed.Value, total);
		LSL.bushWeed.Maximum -= LSL.bushWeed.Value;
		LSL.dealer1Weed = LSL.bushWeed.Maximum;
		LSL.WeedBuy.Text = "Commander ~g~" + LSL.bushWeed.Value + " ~s~pour " + LsFunctions.IntToMoney(total) + " $";
		LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
		LsFunctions.DbugString("Weed Order changed");
		LsFunctions.UpdateOrderMenuDesc();
	}

	internal static void AddCrack()
	{
		if (LSL.dealer1Crack <= 0 || LSL.crack.Value <= 0) return;
		int total = LSL.crack.Value * (LSL.crack1Price - LsFunctions.Discount(LSL.crack1Price));
		LSL.NewOrder.OrderCrack(LSL.crack.Value, total);
		LSL.crack.Maximum -= LSL.crack.Value;
		LSL.dealer1Crack = LSL.crack.Maximum;
		LSL.CrackBuy.Text = "Commander ~g~" + LSL.crack.Value + " ~w~pour " + LsFunctions.IntToMoney(total) + " $";
		LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
		LsFunctions.DbugString("Crack Order changed");
		LsFunctions.UpdateOrderMenuDesc();
	}

	internal static void AddCocaine()
	{
		if (LSL.dealer1Cocain <= 0) return;
		int total = LSL.cocaine.Value * (LSL.cocain1Price - LsFunctions.Discount(LSL.cocain1Price));
		LSL.NewOrder.OrderCocaine(LSL.cocaine.Value, total);
		LSL.cocaine.Maximum -= LSL.cocaine.Value;
		LSL.dealer1Cocain = LSL.cocaine.Maximum;
		LSL.CocainBuy.Text = "Commander ~g~" + LSL.cocaine.Value + " ~w~pour " + LsFunctions.IntToMoney(total) + " $";
		LSL.PlaceOrder.Text = "Commander pour " + LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $";
		LsFunctions.DbugString("Cocaine Order changed");
		LsFunctions.UpdateOrderMenuDesc();
	}
}
