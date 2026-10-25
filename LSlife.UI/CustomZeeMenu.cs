using System;
using System.Drawing;
using GTA;
using GTA.Native;

namespace LSlife;

// Prototype pilote : rendu GTA natif, sans controles graphiques NativeUI.
// Les sous-menus et leurs traitements existants restent sous NativeUI.
internal static class CustomZeeMenu
{
    private static CustomMenu menu;
    private static CustomMenu dealerMenu;
    private static CustomMenu orderMenu;
    private static CustomMenu cannabisMenu;
    private static CustomMenu crackMenu;
    private static CustomMenu cocaineMenu;
    private static CustomMenu activeMenu;
    private static bool dealerWeed, dealerCrack, dealerCocaine, dealerDeliveries;
    private static bool returningFromSubmenu;
    private static bool submenuSeen;
    private static CustomMenu submenuReturnMenu;

    public static bool Visible => activeMenu != null && activeMenu.Visible;

    public static void Toggle()
    {
        if (Visible) { activeMenu.Close(); activeMenu = null; return; }
        if (LSL.LsMenuPool.IsAnyMenuOpen()) return;
        if (menu == null) Create();
        menu.Subtitle = "REPUTATION  " + (int)LSL.dealer1Rep +
                        "    |    NIVEAU  " + (int)LsFunctions.CurrentRepLvl(LSL.dealer1Rep);
        activeMenu = menu;
        menu.Open();
    }

    private static void Create()
    {
        menu = new CustomMenu("ZEE", "");
        menu.Add("Dealer", OpenDealer);
        menu.Add("Commander de la drogue", OpenOrders);
        menu.Add("Acheter des armes", () => RunExisting(LSL.buyWeapons));
        menu.Add("Demander a etre depose", () => RunExisting(LSL.buyLift));
        menu.Add("Rembourser la dette", () => RunExisting(LSL.payDebt), () => LSL.dealer1Debt > 0);
        menu.Add("Options du HUD", OpenHudOptions);
    }

    private static void OpenHudOptions()
    {
        menu.Close();
        activeMenu = null;
        CustomHudOptionsMenu.Open();
    }

    private static void OpenDealer()
    {
        dealerWeed = LSL.sellWeed.Checked;
        dealerCrack = LSL.sellCrack.Checked;
        dealerCocaine = LSL.sellCocaine.Checked;
        dealerDeliveries = LSL.allowDeliveries.Checked;
        if (dealerMenu == null)
        {
            dealerMenu = new CustomMenu("DEALER", "ZEE / ACTIVITE");
            dealerMenu.Add("Vendre du cannabis", () => dealerWeed = !dealerWeed);
            dealerMenu.Add("Vendre du crack", () => dealerCrack = !dealerCrack);
            dealerMenu.Add("Vendre de la cocaine", () => dealerCocaine = !dealerCocaine);
            dealerMenu.Add("Livraisons", () => dealerDeliveries = !dealerDeliveries);
            dealerMenu.Add("Commencer a dealer", StartDealer);
        }
        menu.Close();
        activeMenu = dealerMenu;
        dealerMenu.Open();
    }

    private static void OpenOrders()
    {
        if (orderMenu == null)
        {
            orderMenu = new CustomMenu("COMMANDES", "ZEE / COMMANDES");
            orderMenu.Add("Cannabis", OpenCannabis);
            orderMenu.Add("Crack", OpenCrack);
            orderMenu.Add("Cocaine", OpenCocaine);
            orderMenu.Add("Valider la commande", PlaceExistingOrder);
        }
        menu.Close();
        activeMenu = orderMenu;
        orderMenu.Open();
    }

    private static void OpenCannabis()
    {
        if (cannabisMenu == null)
        {
            cannabisMenu = new CustomMenu("CANNABIS", "ZEE / COMMANDES");
            cannabisMenu.Add("Quantite", () => ChangeCannabis(1));
            cannabisMenu.Add("Multiplicateur", () => ChangeCannabisMultiplier(1));
            cannabisMenu.Add("Ajouter a la commande", AddCannabis,
                () => LSL.dealer1Weed > 0 && LSL.bushWeed.Value > 0);
        }
        orderMenu.Close();
        // Rester synchronise avec le stock reel du fournisseur.
        LSL.bushWeed.Maximum = LSL.dealer1Weed;
        if (LSL.bushWeed.Value > LSL.bushWeed.Maximum)
            LSL.bushWeed.Value = LSL.bushWeed.Maximum;
        activeMenu = cannabisMenu;
        cannabisMenu.Open();
    }


    private static void OpenCrack()
    {
        if (crackMenu == null)
        {
            crackMenu = new CustomMenu("CRACK", "ZEE / COMMANDES");
            crackMenu.Add("Quantite", () => ChangeQuantity(LSL.crack, LSL.dealer1Crack, 1));
            crackMenu.Add("Multiplicateur", () => ChangeCannabisMultiplier(1));
            crackMenu.Add("Ajouter a la commande", () =>
                LsMenus.onCrackMenuSelect(LSL.crackMenu, LSL.CrackBuy, 1),
                () => LSL.dealer1Crack > 0 && LSL.crack.Value > 0);
        }
        OpenDrugMenu(crackMenu, LSL.crack, LSL.dealer1Crack);
    }

    private static void OpenCocaine()
    {
        if (cocaineMenu == null)
        {
            cocaineMenu = new CustomMenu("COCAINE", "ZEE / COMMANDES");
            cocaineMenu.Add("Quantite", () => ChangeQuantity(LSL.cocaine, LSL.dealer1Cocain, 1));
            cocaineMenu.Add("Multiplicateur", () => ChangeCannabisMultiplier(1));
            cocaineMenu.Add("Ajouter a la commande", () =>
                LsMenus.onCocainMenuSelect(LSL.cocainMenu, LSL.CocainBuy, 1),
                () => LSL.dealer1Cocain > 0 && LSL.cocaine.Value > 0);
        }
        OpenDrugMenu(cocaineMenu, LSL.cocaine, LSL.dealer1Cocain);
    }

    private static void OpenDrugMenu(CustomMenu target, NativeUI.UIMenuSliderItem slider, int stock)
    {
        orderMenu.Close();
        slider.Maximum = stock;
        if (slider.Value > slider.Maximum) slider.Value = slider.Maximum;
        activeMenu = target;
        target.Open();
    }

    private static void ChangeQuantity(NativeUI.UIMenuSliderItem slider, int stock, int direction)
    {
        int value = slider.Value + direction * slider.Multiplier;
        if (value < 0) value = 0;
        if (value > stock) value = stock;
        slider.Value = value;
    }

    private static bool IsDrugMenu(CustomMenu target)
    {
        return target == cannabisMenu || target == crackMenu || target == cocaineMenu;
    }

    private static NativeUI.UIMenuSliderItem DrugSlider(CustomMenu target)
    {
        return target == cannabisMenu ? LSL.bushWeed :
            target == crackMenu ? LSL.crack : LSL.cocaine;
    }

    private static int DrugStock(CustomMenu target)
    {
        return target == cannabisMenu ? LSL.dealer1Weed :
            target == crackMenu ? LSL.dealer1Crack : LSL.dealer1Cocain;
    }

    private static int DrugUnitPrice(CustomMenu target)
    {
        int price = target == cannabisMenu ? LSL.weed1Price :
            target == crackMenu ? LSL.crack1Price : LSL.cocain1Price;
        return price - LsFunctions.Discount(price);
    }

    private static void ChangeCannabis(int direction)
    {
        int value = LSL.bushWeed.Value + direction * LSL.bushWeed.Multiplier;
        if (value < 0) value = 0;
        if (value > LSL.dealer1Weed) value = LSL.dealer1Weed;
        LSL.bushWeed.Value = value;
    }

    private static void ChangeCannabisMultiplier(int direction)
    {
        // Le multiplicateur NativeUI est prive : utiliser les valeurs du controle
        // de quantite, sans acceder au menu NativeUI.
        int[] multipliers = { 1, 5, 10, 25, 50, 100 };
        int current = Array.IndexOf(multipliers, LSL.bushWeed.Multiplier);
        if (current < 0) current = 0;
        int next = (current + direction + multipliers.Length) % multipliers.Length;
        int multiplier = multipliers[next];
        LSL.bushWeed.Multiplier = multiplier;
        LSL.crack.Multiplier = multiplier;
        LSL.cocaine.Multiplier = multiplier;
    }

    private static void AddCannabis()
    {
        // Meme traitement que le bouton d'achat NativeUI.
        LsMenus.onWeedMenuSelect(LSL.weedMenu, LSL.WeedBuy, 1);
    }

    private static void PlaceExistingOrder()
    {
        if (LsMenus.PlaceDrugOrder())
        {
            orderMenu.Close();
            activeMenu = null;
        }
    }

    private static void StartDealer()
    {
        LsMenus.StartDealing(dealerWeed, dealerCrack, dealerCocaine, dealerDeliveries);
        if (LSL.isDealing)
        {
            dealerMenu.Close();
            activeMenu = null;
        }
    }

    private static void OpenSubmenu(NativeUI.UIMenu target)
    {
        submenuReturnMenu = activeMenu;
        activeMenu.Close();
        activeMenu = null;
        returningFromSubmenu = true;
        submenuSeen = false;
        target.Visible = true;
        target.CurrentSelection = 0;
    }

    private static void RunExisting(NativeUI.UIMenuItem item)
    {
        // Reutilise les traitements metier du menu Zee original.
        LSL.zeeMenu.Visible = true;
        try { LsMenus.OnZeeMenuSelect(LSL.zeeMenu, item, 0); }
        finally { LSL.zeeMenu.Visible = false; }
        menu.Close();
    }

    public static void Tick()
    {
        if (returningFromSubmenu)
        {
            if (LSL.LsMenuPool.IsAnyMenuOpen()) submenuSeen = true;
            else if (submenuSeen)
            {
                returningFromSubmenu = false;
                submenuSeen = false;
                // Retour au menu pilote apres fermeture du sous-menu NativeUI.
                if (submenuReturnMenu != null) { activeMenu = submenuReturnMenu; submenuReturnMenu.Open(); }
            }
        }

        if (!Visible) return;

        // Bloquer d'abord les commandes GTA (dont l'ouverture du telephone).
        // Lire ensuite les commandes desactivees pour conserver la navigation.
        // B / O sert aussi aux attaques au corps-a-corps dans GTA V.
        // Neutraliser ces actions pendant qu'un menu personnalise est ouvert.
        foreach (int control in new[] {
            27, 172, 173, 174, 175, 201, 202,
            24, 140, 141, 142, 257, 263, 264
        })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }

        if (IsDrugMenu(activeMenu) && activeMenu.SelectedIndex < 2 &&
            DisabledJustPressed(174))
        {
            if (activeMenu.SelectedIndex == 0)
                ChangeQuantity(DrugSlider(activeMenu), DrugStock(activeMenu), -1);
            else ChangeCannabisMultiplier(-1);
        }
        else if (IsDrugMenu(activeMenu) && activeMenu.SelectedIndex < 2 &&
                 DisabledJustPressed(175))
        {
            if (activeMenu.SelectedIndex == 0)
                ChangeQuantity(DrugSlider(activeMenu), DrugStock(activeMenu), 1);
            else ChangeCannabisMultiplier(1);
        }
        else if (DisabledJustPressed(172)) activeMenu.Move(-1);
        else if (DisabledJustPressed(173)) activeMenu.Move(1);
        else if (DisabledJustPressed(201)) activeMenu.Activate();
        else if (DisabledJustPressed(202))
        {
            activeMenu.Close();
            if (IsDrugMenu(activeMenu)) { activeMenu = orderMenu; orderMenu.Open(); }
            else if (activeMenu == orderMenu || activeMenu == dealerMenu)
            { activeMenu = menu; menu.Open(); }
            else activeMenu = null;
        }

        if (Visible) Draw();
    }

    private static bool DisabledJustPressed(int control)
    {
        // IS_DISABLED_CONTROL_JUST_PRESSED : compatible avec les controles
        // desactives et avec la version SHVDN2 du projet.
        return Function.Call<bool>(unchecked((Hash)0x91AEF906BCA88877UL), 2, control);
    }

    // Theme PaperNest : anthracite profond, or champagne et typographie sobre.
    private static void Draw()
    {
        CustomMenu displayed = activeMenu;
        bool showOrderDetails = displayed == orderMenu || IsDrugMenu(displayed);
        var layout = new CustomMenuLayout();
        string subtitle = IsDrugMenu(displayed) || displayed == orderMenu ? "" :
            displayed == dealerMenu ? "ZEE / ACTIVITE" : "CONTACT  /  SERVICES";
        layout.Header(displayed.Title, subtitle, titleScale: 0.67f);
        layout.Separator(0.004f);
        layout.Block(0.045f, y =>
        {
            CustomMenuRenderer.Rect(0.149f, y + 0.0225f, 0.255f, 0.045f, 28, 30, 33, 232);
            CustomMenuRenderer.TextCenteredVertically(displayed == dealerMenu ? "CHOISIR LES OPTIONS" :
                displayed == orderMenu ? "SELECTION DES PRODUITS" :
                IsDrugMenu(displayed) ? "STOCK " + DrugStock(displayed) + " OZ  |  " +
                    LsFunctions.IntToMoney(DrugUnitPrice(displayed)) + " $/OZ" :
                displayed.Subtitle, layout.Left + 0.016f, y, 0.045f,
                0.275f, 226, 206, 167, 255);
        });
        layout.Separator(0.004f);
        for (int index = 0; index < displayed.Entries.Count; index++)
        {
            int entryIndex = index;
            layout.Block(0.043f, rowTop =>
            {
                int i = entryIndex;
                float y = rowTop;
                const float x = 0.149f, width = 0.268f;
                float left = layout.Left;
            bool selected = i == displayed.SelectedIndex;
            bool debtEntry = displayed == menu && displayed.Entries[i].Label == "Rembourser la dette";
            bool debtFree = debtEntry && LSL.dealer1Debt <= 0;
            CustomMenuRenderer.Rect(x, y + 0.0205f, width - 0.012f, 0.040f,
                selected ? 65 : 39, selected ? 55 : 43,
                selected ? 40 : 48, selected ? 248 : 245);
            if (selected)
            {
                CustomMenuRenderer.Rect(left + 0.008f, y + 0.0205f, 0.0035f, 0.038f, 218, 176, 101, 255);
                CustomMenuRenderer.Rect(x, y + 0.001f, width - 0.012f, 0.001f, 172, 137, 78, 225);
                CustomMenuRenderer.Rect(x, y + 0.041f, width - 0.012f, 0.001f, 172, 137, 78, 225);
            }
            CustomMenuRenderer.TextCenteredVertically(debtFree ? "Aucune dette en cours" :
                IsDrugMenu(displayed) && i == 0 ? "Quantite" :
                displayed.Entries[i].Label,
                left + 0.019f, y + 0.0005f, 0.040f,
                0.315f, debtFree ? 133 : (selected ? 255 : 226),
                debtFree ? 135 : (selected ? 247 : 227),
                debtFree ? 138 : (selected ? 226 : 229), 255);
            if (debtEntry && !debtFree)
            {
                // Valeur lue en direct : toute nouvelle dette ou tout remboursement
                // se repercute automatiquement sur la ligne.
                string amount = LsFunctions.IntToMoney(LSL.dealer1Debt) + " $";
                CustomMenuRenderer.TextRightCenteredVertically(amount, left + width - 0.018f,
                    y + 0.0005f, 0.040f, 0.305f, 218, 176, 101, 255);
            }
            if (IsDrugMenu(displayed) && i == 0)
            {
                // Vrai curseur graphique, proportionnel au stock disponible.
                float barLeft = left + 0.119f;
                float barWidth = 0.072f;
                float ratio = DrugStock(displayed) > 0 ?
                    Math.Max(0f, Math.Min(1f, (float)DrugSlider(displayed).Value / DrugStock(displayed))) : 0f;
                CustomMenuRenderer.Rect(barLeft + barWidth / 2f, y + 0.0205f, barWidth, 0.006f,
                    67, 65, 61, 255);
                if (ratio > 0f)
                    CustomMenuRenderer.Rect(barLeft + barWidth * ratio / 2f, y + 0.0205f,
                        barWidth * ratio, 0.006f, 218, 176, 101, 255);
                CustomMenuRenderer.Rect(barLeft + barWidth * ratio, y + 0.0205f,
                    0.004f, 0.013f, 234, 207, 151, 255);
                CustomMenuRenderer.TextRightCenteredVertically(DrugSlider(displayed).Value + " OZ",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f,
                    218, 176, 101, 255);
            }
            if (IsDrugMenu(displayed) && i == 1)
            {
                CustomMenuRenderer.TextRightCenteredVertically("< x" + DrugSlider(displayed).Multiplier + " >",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f,
                    218, 176, 101, 255);
            }
            if (displayed == orderMenu && i == 3)
            {
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f,
                    218, 176, 101, 255);
            }
            if (IsDrugMenu(displayed) && i == 2)
            {
                int price = DrugSlider(displayed).Value * DrugUnitPrice(displayed);
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.IntToMoney(price) + " $",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f,
                    218, 176, 101, 255);
            }
            if (displayed == dealerMenu && i < 4)
            {
                bool check = i == 0 ? dealerWeed : i == 1 ? dealerCrack :
                    i == 2 ? dealerCocaine : dealerDeliveries;
                CustomMenuRenderer.TextRightCenteredVertically(check ? "ON" : "OFF",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f,
                    check ? 218 : 145, check ? 176 : 145, check ? 101 : 145, 255);
            }
            if ((displayed == menu && i < 2) || (displayed == orderMenu && i < 3))
                CustomMenuRenderer.TextCenteredVertically(">", left + width - 0.027f, y + 0.0005f, 0.040f, 0.33f,
                    204, 168, 102, 255);
            });
        }
        if (showOrderDetails)
        {
            layout.Separator(0.006f);
            layout.Block(displayed == orderMenu ? 0.110f : 0.082f, detailTop =>
            {
                float y = detailTop;
                float left = layout.Left;
                const float width = 0.268f;
            if (displayed == orderMenu)
            {
                CustomMenuRenderer.TextCenteredVertically("CANNABIS  " + LSL.NewOrder.weed + " OZ",
                    left + 0.016f, y + 0.012f, 0.022f,
                    0.255f, 202, 195, 180, 255);
                CustomMenuRenderer.TextCenteredVertically("CRACK  " + LSL.NewOrder.crack + " OZ",
                    left + 0.016f, y + 0.035f, 0.022f,
                    0.255f, 202, 195, 180, 255);
                CustomMenuRenderer.TextCenteredVertically("COCAINE  " + LSL.NewOrder.cocaine + " OZ",
                    left + 0.016f, y + 0.058f, 0.022f,
                    0.255f, 202, 195, 180, 255);
            }
            else
            {
                int ordered = displayed == cannabisMenu ? LSL.NewOrder.weed :
                    displayed == crackMenu ? LSL.NewOrder.crack : LSL.NewOrder.cocaine;
                CustomMenuRenderer.TextCenteredVertically("DEJA AJOUTE  " + ordered + " OZ",
                    left + 0.016f, y + 0.012f, 0.022f,
                    0.255f, 202, 195, 180, 255);
                CustomMenuRenderer.TextCenteredVertically("PRIX SELECTION  " +
                    LsFunctions.IntToMoney(DrugSlider(displayed).Value *
                        DrugUnitPrice(displayed)) + " $",
                    left + 0.016f, y + 0.035f, 0.022f,
                    0.255f, 202, 195, 180, 255);
            }
            float totalY = displayed == orderMenu ? y + 0.086f : y + 0.058f;
            CustomMenuRenderer.TextCenteredVertically("TOTAL COMMANDE", left + 0.016f,
                totalY, 0.022f, 0.265f, 234, 207, 151, 255);
            CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.IntToMoney(LSL.NewOrder.Bill) + " $",
                left + width - 0.018f, totalY, 0.022f,
                0.265f, 234, 207, 151, 255);
            });
        }
        layout.Render();
    }
}
