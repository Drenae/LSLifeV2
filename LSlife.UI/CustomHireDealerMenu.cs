using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Recrutement des dealers de quartier : interface personnalisee,
// avec decisions, reputation et paiement geres par le code historique.
internal static class CustomHireDealerMenu
{
    private static CustomMenu menu;
    private static bool waitForRelease;
    private static int lastFrame = -1;
    private static int proposedAmount;
    private static readonly CustomMenuShared.HorizontalRepeat repeat = new CustomMenuShared.HorizontalRepeat();

    public static bool Visible => menu != null && menu.Visible;

    public static void Open()
    {
        if (Visible || LSL.aDealer == null || LSL.LsMenuPool == null ||
            LSL.LsMenuPool.IsAnyMenuOpen()) return;

        if (menu == null)
        {
            menu = new CustomMenu("RECRUTEMENT", "DEALER");
            menu.Add("Montant propose", () => { });
            menu.Add("Faire une offre", Offer);
            menu.Add("Annuler", Cancel);
        }

        proposedAmount = Math.Max(0, Math.Min(proposedAmount, Game.Player.Money));
        menu.Open();
        waitForRelease = true;
        repeat.Reset();
    }

    public static void Close()
    {
        if (menu != null) menu.Close();
        repeat.Reset();
    }

    private static void Cancel()
    {
        Close();
        DealerRecruitmentActions.CancelDealerRecruitment();
    }

    private static void Offer()
    {
        if (LSL.aDealer == null) { Close(); return; }
        int offer = proposedAmount;
        if (offer <= 0 || offer > Game.Player.Money)
        {
            UI.ShowSubtitle("Choisis un montant que tu peux payer.", 3000);
            return;
        }
        if (offer <= LSL.aDealer.LastOffer)
        {
            UI.ShowSubtitle("Ton offre doit etre superieure a la precedente.", 3000);
            return;
        }

        Close();
        // Conserve l'acceptation aleatoire, les reactions du PNJ,
        // le paiement et la transformation en dealer employe.
        DealerRecruitmentActions.SubmitDealerOffer(offer);
    }

    private static bool Pressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, control);

    private static bool JustPressed(int control) =>
        Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, control);

    private static void Adjust(int direction)
    {
        int max = Math.Max(0, Game.Player.Money);
        int current = proposedAmount;
        int next = Math.Max(0, Math.Min(max, current + direction * 1000));
        proposedAmount = next;
    }

    public static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (lastFrame == frame) return;
        lastFrame = frame;
        if (!Visible) return;
        if (LSL.aDealer == null || LSL.aDealer.dPed == null ||
            !((Entity)LSL.aDealer.dPed).Exists()) { Close(); return; }

        foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }

        if (waitForRelease)
        {
            if (!Pressed(201) && !Pressed(202) && !Pressed(21)) waitForRelease = false;
            Draw();
            return;
        }

        if (JustPressed(172)) { menu.Move(-1); repeat.Reset(); }
        else if (JustPressed(173)) { menu.Move(1); repeat.Reset(); }
        else if (JustPressed(202)) { Cancel(); return; }
        else if (JustPressed(201))
        {
            menu.Activate();
            if (!Visible) return;
        }

        if (menu.SelectedIndex == 0)
        {
            int direction = repeat.Poll(110, 320);
            if (direction != 0) Adjust(direction);
        }
        else repeat.Reset();

        if (Visible) Draw();
    }

    private static void Draw()
    {
        if (LSL.aDealer == null) return;
        int offer = proposedAmount;
        int money = Game.Player.Money;
        int employed = LSL.DealerHandler.dealers.Count;
        int capacity = (int)LsFunctions.CurrentRepLvl(LSL.dealer1Rep);
        var layout = new CustomMenuLayout();
        layout.Header("RECRUTEMENT", "DEALER", titleScale: 0.46f);
        layout.Separator(0.005f);
        layout.Block(0.029f, y =>
            CustomMenuRenderer.TextCenteredVertically("INFORMATIONS",
                layout.Left + 0.019f, y, 0.029f, 0.27f, 226, 206, 167, 255));
        layout.TextPair("Argent disponible", LsFunctions.IntToMoney(money) + " $");
        layout.TextPair("Dealers recrutes", employed + " / " + capacity);
        layout.TextPair("Derniere offre", LsFunctions.IntToMoney(LSL.aDealer.LastOffer) + " $");
        layout.Separator(0.006f);
        layout.Row("Montant propose", menu.SelectedIndex == 0,
            LsFunctions.IntToMoney(offer) + " $");
        layout.Block(0.027f, y =>
            CustomMenuRenderer.TextCenteredVertically("Gauche / droite : ajuster par 1 000 $",
                layout.Left + 0.019f, y, 0.027f, 0.235f, 175, 177, 182, 255));
        layout.Row("Faire une offre", menu.SelectedIndex == 1,
            LsFunctions.IntToMoney(offer) + " $", enabled: offer > 0 && offer <= money);
        layout.Separator(0.005f);
        layout.Row("Annuler", menu.SelectedIndex == 2);
        layout.Render();
    }
}
