using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Presentation uniquement : les ventes et refus restent geres par LsMenus.
internal static class CustomCustomerMenu
{
    private static CustomMenu menu;
    private static bool waitForRelease;
    private static int lastFrame = -1;

    public static bool Visible => menu != null && menu.Visible;

    public static void Open()
    {
        if (Visible || LSL.customerSellTo == null || LSL.LsMenuPool.IsAnyMenuOpen())
            return;

        if (menu == null)
        {
            menu = new CustomMenu("CLIENT", "TRANSACTION");
            menu.Add("Vendre", () => Execute(true), CanSell);
            menu.Add("Refuser la vente", () => Execute(false));
        }
        menu.Open();
        waitForRelease = true;
    }

    private static bool CanSell()
    {
        var customer = LSL.customerSellTo;
        return customer != null && customer.Drug != null &&
            LSL.PlayerInventory[customer.Drug.Name] >= customer.amount;
    }

    private static void Execute(bool sale)
    {
        if (LSL.customerSellTo == null) { Close(); return; }
        if (sale && !CanSell())
        {
            LsFunctions.DisplayHelpText("Stock insuffisant pour cette vente.");
            return;
        }
        Close();
        CustomerTransactionActions.HandleCustomerTransaction(sale);
    }

    public static void Close()
    {
        if (menu != null) menu.Close();
    }

    public static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (lastFrame == frame) return;
        lastFrame = frame;
        if (!Visible) return;
        if (LSL.customerSellTo == null) { Close(); return; }

        foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }
        if (waitForRelease)
        {
            if (!Pressed(201) && !Pressed(202)) waitForRelease = false;
            Draw();
            return;
        }
        if (JustPressed(172)) menu.Move(-1);
        else if (JustPressed(173)) menu.Move(1);
        else if (JustPressed(201))
        {
            menu.Activate();
            if (!Visible) return;
        }
        else if (JustPressed(202)) { Close(); return; }
        if (Visible) Draw();
    }

    private static bool Pressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);

    private static bool JustPressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);

    private static void Draw()
    {
        var c = LSL.customerSellTo;
        if (c == null || c.Drug == null) { Close(); return; }

        int available = LSL.PlayerInventory[c.Drug.Name];
        var layout = new CustomMenuLayout();
        layout.Header("CLIENT", "TRANSACTION", titleScale: 0.54f);
        layout.Separator(0.004f);
        layout.Block(0.030f, y =>
            CustomMenuRenderer.TextCenteredVertically("DEMANDE DU CLIENT",
                layout.Left + 0.019f, y, 0.030f, 0.27f, 226, 206, 167, 255));
        layout.TextPair(LsFunctions.DrugDisplayName(c.Drug.Name), LsFunctions.GramsToOz(c.amount));
        layout.TextPair("Stock disponible", LsFunctions.GramsToOz(available));
        layout.Separator(0.005f);
        layout.Row("Vendre", menu.SelectedIndex == 0,
            CanSell() ? "+" + LsFunctions.IntToMoney(CustomerSalePricing.SalePrice()) + " $" : "Stock insuffisant",
            enabled: CanSell(), emphasizeValue: CanSell());
        layout.Row("Refuser la vente", menu.SelectedIndex == 1);
        layout.Render();
    }
}
