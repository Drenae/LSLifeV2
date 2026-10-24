using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Navigation et rendu propres aux employes. Aucun etat de menu partage avec Zee.
internal static class CustomWorkerMenu
{
    private static CustomMenu workerMenu, workerSupplyMenu, workerEquipmentMenu, activeMenu;
    private static int supplyWeed, supplyCrack, supplyCocaine, supplyStep = 1;
    private static readonly int[] supplySteps = { 1, 5, 10, 25, 50, 100 };
    private static bool waitForRelease;
    private static int lastTickFrame = -1;
    private static int heldDirection;
    private static int nextRepeatAt;
    private const int EquipmentVisibleRows = 8;
    public static bool Visible => activeMenu != null && activeMenu.Visible;

    public static void Open()
    {
        if (Visible || LSL.LsMenuPool.IsAnyMenuOpen()) return;
        if (LSL.DealerHandler.currentDealer == null) return;
        if (workerMenu == null)
        {
            workerMenu = new CustomMenu("EMPLOYE", "");
            workerMenu.Add("Approvisionner", OpenWorkerSupply);
            workerMenu.Add("Recolter la recette", CollectWorker,
                () => LSL.DealerHandler.currentDealer != null &&
                    LSL.DealerHandler.currentDealer.Money >= 100);
            workerMenu.Add("Equipement", OpenWorkerEquipment);
            workerMenu.Add("Instructions", () => OpenNativeSubmenu());
        }
        activeMenu = workerMenu;
        workerMenu.Open();
        waitForRelease = true;
    }

    // Transition explicite : la touche ayant ouvert le menu ne peut pas le refermer.
    private static void OpenNativeSubmenu()
    {
        activeMenu.Close();
        activeMenu = null;
        LSL.WorkerMainMenu.Visible = true;
        LSL.WorkerMainMenu.CurrentSelection = 0;
    }

    private static void OpenWorkerEquipment()
    {
        if (LSL.DealerHandler.currentDealer == null) return;
        workerEquipmentMenu = new CustomMenu("EQUIPEMENT", "");
        workerEquipmentMenu.Add("Donner un gilet pare-balles", GiveWorkerArmour,
            () => LSL.playerArmours > 0 && LSL.DealerHandler.currentDealer != null &&
                  LSL.DealerHandler.currentDealer.Ped != null &&
                  LSL.DealerHandler.currentDealer.Ped.Armor < 200);
        // Liste reconstruite a chaque ouverture : le stock peut avoir change.
        for (int i = 0; i < LSL.playerWeps.Count; i++)
        {
            int weaponIndex = i;
            string weaponId = LSL.playerWeps[i];
            workerEquipmentMenu.Add(FrenchWeaponName(weaponId), () => GiveWorkerWeapon(weaponIndex),
                () => weaponIndex < LSL.playerWeps.Count);
        }
        workerMenu.Close();
        activeMenu = workerEquipmentMenu;
        workerEquipmentMenu.Open();
    }

    private static string FrenchWeaponName(string weapon)
    {
        switch (weapon.ToUpperInvariant())
        {
            case "FIREEXTINGUISHER": return "Extincteur";
            case "PETROLCAN": return "Bidon d'essence";
            case "STUNGUN": return "Pistolet paralysant";
            case "STICKYBOMB": return "Bombe collante";
            case "MINIGUN": return "Minigun";
            case "GOLFCLUB": return "Club de golf";
            case "HAMMER": return "Marteau";
            case "GUSENBERG": return "Mitraillette Gusenberg";
            case "PISTOL": return "Pistolet";
            case "COMBATPISTOL": return "Pistolet de combat";
            case "APPISTOL": return "Pistolet automatique";
            case "PISTOL50": return "Pistolet calibre .50";
            case "SNSPISTOL": return "Pistolet compact";
            case "HEAVYPISTOL": return "Pistolet lourd";
            case "VINTAGEPISTOL": return "Pistolet vintage";
            case "MICROSMG": return "Micro-mitraillette";
            case "SMG": return "Mitraillette";
            case "ASSAULTSMG": return "Mitraillette d'assaut";
            case "COMBATPDW": return "ADP de combat";
            case "ASSAULTRIFLE": return "Fusil d'assaut";
            case "CARBINERIFLE": return "Carabine";
            case "ADVANCEDRIFLE": return "Fusil avance";
            case "SPECIALCARBINE": return "Carabine speciale";
            case "BULLPUPRIFLE": return "Fusil bullpup";
            case "PUMPSHOTGUN": return "Fusil a pompe";
            case "SAWNOFFSHOTGUN": return "Fusil a canon scie";
            case "ASSAULTSHOTGUN": return "Fusil a pompe d'assaut";
            case "HEAVYSHOTGUN": return "Fusil a pompe lourd";
            case "SNIPERRIFLE": return "Fusil de precision";
            case "HEAVYSNIPER": return "Fusil de precision lourd";
            case "MG": return "Mitrailleuse";
            case "COMBATMG": return "Mitrailleuse de combat";
            default: return weapon;
        }
    }

    private static void GiveWorkerArmour()
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

    private static void GiveWorkerWeapon(int index)
    {
        if (LSL.DealerHandler.currentDealer == null || index < 0 ||
            index >= LSL.playerWeps.Count) return;
        // Reutilise provisoirement la logique historique de transfert et de sauvegarde,
        // sans ouvrir de menu NativeUI. La migration du traitement viendra ensuite.
        LSL.wWeapons.Index = index;
        LsMenus.WWeapons_Activated(LSL.WorkerMainMenu, LSL.wWeapons);
        if (activeMenu == workerEquipmentMenu)
            OpenWorkerEquipmentRefresh();
    }

    private static void OpenWorkerEquipmentRefresh()
    {
        workerEquipmentMenu.Close();
        activeMenu = workerMenu;
        OpenWorkerEquipment();
        waitForRelease = true;
    }

    private static void OpenWorkerSupply()
    {
        if (LSL.DealerHandler.currentDealer == null) return;
        supplyWeed = supplyCrack = supplyCocaine = 0;
        supplyStep = 1;
        heldDirection = 0;
        if (workerSupplyMenu == null)
        {
            workerSupplyMenu = new CustomMenu("APPROVISIONNER", "");
            workerSupplyMenu.Add("Cannabis", () => ChangeSupply(0, 1));
            workerSupplyMenu.Add("Crack", () => ChangeSupply(1, 1));
            workerSupplyMenu.Add("Cocaïne", () => ChangeSupply(2, 1));
            workerSupplyMenu.Add("Multiplicateur", () => ChangeSupplyStep(1));
            workerSupplyMenu.Add("Confirmer le transfert", ConfirmWorkerSupply,
                () => supplyWeed + supplyCrack + supplyCocaine > 0);
        }
        workerMenu.Close();
        activeMenu = workerSupplyMenu;
        workerSupplyMenu.Open();
    }

    private static int SupplyAvailable(string key)
    {
        int value;
        return LSL.PlayerInventory.TryGetValue(key, out value) ? Math.Max(0, value) : 0;
    }

    private static void ChangeSupply(int index, int direction)
    {
        string key = index == 0 ? "Weed" : index == 1 ? "Crack" : "Cocaine";
        int value = index == 0 ? supplyWeed : index == 1 ? supplyCrack : supplyCocaine;
        value = Math.Max(0, Math.Min(SupplyAvailable(key), value + direction * supplyStep));
        if (index == 0) supplyWeed = value;
        else if (index == 1) supplyCrack = value;
        else supplyCocaine = value;
    }

    private static void ChangeSupplyStep(int direction)
    {
        int index = Array.IndexOf(supplySteps, supplyStep);
        supplyStep = supplySteps[(index + direction + supplySteps.Length) % supplySteps.Length];
    }

    private static void ConfirmWorkerSupply()
    {
        PlayerDealer worker = LSL.DealerHandler.currentDealer;
        if (worker == null || worker.Ped == null) return;
        if (supplyWeed + supplyCrack + supplyCocaine <= 0 ||
            supplyWeed > SupplyAvailable("Weed") ||
            supplyCrack > SupplyAvailable("Crack") ||
            supplyCocaine > SupplyAvailable("Cocaine")) return;

        LsFunctions.HandOverGoods(LSL.player.Character, worker.Ped);
        if (supplyWeed > 0) { worker.Drugs["Weed"] += supplyWeed; LSL.PlayerInventory["Weed"] -= supplyWeed; }
        if (supplyCrack > 0) { worker.Drugs["Crack"] += supplyCrack; LSL.PlayerInventory["Crack"] -= supplyCrack; }
        if (supplyCocaine > 0) { worker.Drugs["Cocaine"] += supplyCocaine; LSL.PlayerInventory["Cocaine"] -= supplyCocaine; }
        LsFunctions.UpdateInventory();
        worker.SetDrugDesc();
        LSL.DealerHandler.needsSave = true;
        supplyWeed = supplyCrack = supplyCocaine = 0;
        workerSupplyMenu.Close();
        activeMenu = workerMenu;
        workerMenu.Open();
    }

    private static void CollectWorker()
    {
        if (LSL.DealerHandler.currentDealer == null ||
            LSL.DealerHandler.currentDealer.Money < 100) return;
        LsMenus.WCollect_Activated(LSL.WorkerMainMenu, LSL.wCollect);
    }


    public static void TickOncePerFrame()
    {
        int frame = Game.GameTime;
        if (lastTickFrame == frame) return;
        lastTickFrame = frame;
        Tick();
    }

    private static void Tick()
    {
        if (!Visible) return;
        if (LSL.DealerHandler.currentDealer == null)
        {
            activeMenu.Close();
            activeMenu = null;
            return;
        }
        foreach (int control in new[] { 27, 172, 173, 174, 175, 201, 202, 24, 140, 141, 142, 257, 263, 264 })
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, control, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 2, control, true);
        }
        // Attendre le relachement de validation/retour entre deux ecrans.
        // Evite la fermeture immediate du sous-menu au meme appui.
        if (waitForRelease)
        {
            if (!Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, 201) &&
                !Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, 202))
                waitForRelease = false;
            if (Visible) Draw();
            return;
        }
        if (activeMenu == workerSupplyMenu && activeMenu.SelectedIndex < 4 &&
            HandleSupplyHorizontalInput())
        {
            // Une impulsion initiale, puis repetition temporisee pendant le maintien.
        }
        else if (DisabledJustPressed(172)) { heldDirection = 0; activeMenu.Move(-1); }
        else if (DisabledJustPressed(173)) { heldDirection = 0; activeMenu.Move(1); }
        else if (DisabledJustPressed(201))
        {
            CustomMenu before = activeMenu;
            activeMenu.Activate();
            if (before != activeMenu) waitForRelease = true;
        }
        else if (DisabledJustPressed(202))
        {
            activeMenu.Close();
            if (activeMenu == workerSupplyMenu || activeMenu == workerEquipmentMenu)
            {
                activeMenu = workerMenu;
                workerMenu.Open();
                waitForRelease = true;
            }
            else activeMenu = null;
        }
        if (Visible) Draw();
    }

    private static bool HandleSupplyHorizontalInput()
    {
        bool left = Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, 174);
        bool right = Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, 175);
        int direction = left == right ? 0 : (right ? 1 : -1);
        if (direction == 0)
        {
            heldDirection = 0;
            return false;
        }
        int now = Game.GameTime;
        if (heldDirection != direction)
        {
            heldDirection = direction;
            nextRepeatAt = now + 350;
        }
        else
        {
            if (now < nextRepeatAt) return false;
            nextRepeatAt = now + 65;
        }
        if (activeMenu.SelectedIndex < 3)
            ChangeSupply(activeMenu.SelectedIndex, direction);
        else ChangeSupplyStep(direction);
        return true;
    }

    private static bool DisabledJustPressed(int control)
    {
        return Function.Call<bool>(unchecked((Hash)0x91AEF906BCA88877UL), 2, control);
    }

    private static void Draw()
    {
        const float x = 0.149f, width = 0.268f, top = 0.035f;
        const float left = x - width / 2f;
        bool main = activeMenu == workerMenu;
        bool supply = activeMenu == workerSupplyMenu;
        bool equipment = activeMenu == workerEquipmentMenu;
        int visibleRows = equipment
            ? Math.Min(EquipmentVisibleRows, activeMenu.Entries.Count)
            : activeMenu.Entries.Count;
        float height = 0.102f + visibleRows * 0.043f +
            (main ? 0.168f : supply ? 0.126f : 0.090f);
        CustomMenuRenderer.Rect(x, top + height / 2f, width, height, 13, 16, 20, 238);
        CustomMenuRenderer.Rect(x, top, width, 0.002f, 194, 156, 88, 255);
        CustomMenuRenderer.Rect(x, top + height, width, 0.002f, 194, 156, 88, 255);
        CustomMenuRenderer.Rect(left, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        CustomMenuRenderer.Rect(left + width, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        CustomMenuRenderer.Rect(x, top + 0.046f, width - 0.004f, 0.090f, 22, 25, 29, 240);
        CustomMenuRenderer.Rect(left + 0.008f, top + 0.046f, 0.003f, 0.062f, 194, 156, 88, 255);
        CustomMenuRenderer.TextCenteredVertically(activeMenu.Title, left + 0.023f, top, 0.090f,
            main ? 0.67f : 0.46f, 234, 207, 151, 255);
        if (LSL.DealerHandler.currentDealer != null)
            CustomMenuRenderer.TextRightCenteredVertically("DEALER #" + LSL.DealerHandler.currentDealer.ID,
                left + width - 0.018f, top, 0.090f, 0.270f, 218, 176, 101, 255);
        CustomMenuRenderer.Rect(x, top + 0.091f, width - 0.019f, 0.0015f, 171, 133, 73, 235);
        int firstVisible = equipment
            ? Math.Max(0, Math.Min(activeMenu.SelectedIndex - EquipmentVisibleRows + 1,
                activeMenu.Entries.Count - visibleRows))
            : 0;
        for (int i = firstVisible; i < firstVisible + visibleRows; i++)
        {
            float y = top + 0.098f + (i - firstVisible) * 0.043f;
            bool selected = i == activeMenu.SelectedIndex;
            CustomMenuRenderer.Rect(x, y + 0.0205f, width - 0.012f, 0.040f,
                selected ? 65 : 25, selected ? 55 : 28,
                selected ? 40 : 32, selected ? 248 : 229);
            if (selected)
            {
                CustomMenuRenderer.Rect(left + 0.008f, y + 0.0205f, 0.0035f, 0.038f, 218, 176, 101, 255);
                CustomMenuRenderer.Rect(x, y + 0.001f, width - 0.012f, 0.001f, 172, 137, 78, 225);
                CustomMenuRenderer.Rect(x, y + 0.041f, width - 0.012f, 0.001f, 172, 137, 78, 225);
            }
            CustomMenuRenderer.TextCenteredVertically(activeMenu.Entries[i].Label, left + 0.019f,
                y + 0.0005f, 0.040f, 0.315f, selected ? 255 : 226,
                selected ? 247 : 227, selected ? 226 : 229, 255);
            if (main && i == 1 && LSL.DealerHandler.currentDealer != null)
            {
                int money = LSL.DealerHandler.currentDealer.Money;
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.IntToMoney(money - (money / 100) * 20) + " $",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f, 111, 216, 139, 255);
            }
            if (supply && i < 3)
            {
                int selectedAmount = i == 0 ? supplyWeed : i == 1 ? supplyCrack : supplyCocaine;
                int available = SupplyAvailable(i == 0 ? "Weed" : i == 1 ? "Crack" : "Cocaine");
                float barLeft = left + 0.116f, barWidth = 0.085f;
                float fraction = available > 0 ? Math.Min(1f, (float)selectedAmount / available) : 0f;
                CustomMenuRenderer.Rect(barLeft + barWidth / 2f, y + 0.0205f, barWidth, 0.008f, 66, 68, 70, 255);
                if (fraction > 0f)
                    CustomMenuRenderer.Rect(barLeft + barWidth * fraction / 2f, y + 0.0205f,
                        barWidth * fraction, 0.008f, 111, 216, 139, 255);
            }
            if (!main && i < 3)
            {
                int amount = i == 0 ? supplyWeed : i == 1 ? supplyCrack : supplyCocaine;
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.GramsToOz(amount), left + width - 0.018f,
                    y + 0.0005f, 0.040f, 0.305f, 111, 216, 139, 255);
            }
            if (supply && i == 3)
                CustomMenuRenderer.TextRightCenteredVertically("< x" + supplyStep + " >", left + width - 0.018f,
                    y + 0.0005f, 0.040f, 0.305f, 218, 176, 101, 255);
        }
        // Espace entre la derniere action et le panneau de stock.
        if (equipment)
        {
            float footerTop = top + 0.098f + visibleRows * 0.043f;
            CustomMenuRenderer.Separator(x, footerTop, width - 0.019f, 0.006f);
            CustomMenuRenderer.TextCenteredVertically(
                "Gilets : " + LSL.playerArmours + "    Armes : " +
                Math.Max(0, activeMenu.Entries.Count - 1),
                left + 0.019f, footerTop + 0.015f,
                0.030f, 0.270f, 234, 207, 151, 255);
            CustomMenuRenderer.TextRightCenteredVertically(
                (activeMenu.SelectedIndex + 1) + "/" + activeMenu.Entries.Count,
                left + width - 0.019f, footerTop + 0.015f,
                0.030f, 0.270f, 111, 216, 139, 255);
            return;
        }
        float detailY = top + 0.102f + activeMenu.Entries.Count * 0.043f;
        float innerLeft = left + 0.009f, innerWidth = width - 0.018f;
        CustomMenuRenderer.Rect(x, detailY + (main ? 0.082f : 0.056f), innerWidth,
            main ? 0.153f : 0.112f, 25, 29, 33, 248);
        // Separateur autonome, juste sous la derniere action.
        CustomMenuRenderer.Separator(x, detailY - (supply ? 0.009f : 0.012f), innerWidth, 0.006f);
        if (main && LSL.DealerHandler.currentDealer != null)
        {
            PlayerDealer worker = LSL.DealerHandler.currentDealer;
            int hp = worker.Ped != null ? Math.Max(0, worker.Ped.Health) : 0;
            int armor = worker.Ped != null ? Math.Max(0, worker.Ped.Armor) : 0;
            CustomMenuRenderer.TextCenteredVertically("SANTE  " + hp + "     ARMURE  " + armor,
                innerLeft + 0.012f, detailY + 0.011f, 0.029f, 0.275f, 226, 206, 167, 255);
            string[] names = { "Cannabis", "Crack", "Cocaïne" };
            string[] keys = { "Weed", "Crack", "Cocaine" };
            for (int i = 0; i < 3; i++)
            {
                float lineY = detailY + 0.039f + i * 0.023f;
                int amount = worker.Drugs.ContainsKey(keys[i]) ? worker.Drugs[keys[i]] : 0;
                CustomMenuRenderer.TextCenteredVertically(names[i], innerLeft + 0.012f, lineY,
                    0.026f, 0.270f, 226, 227, 229, 255);
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.GramsToOz(amount), left + width - 0.019f,
                    lineY, 0.026f, 0.295f, 111, 216, 139, 255);
            }
            int net = worker.Money - (worker.Money / 100) * 20;
            CustomMenuRenderer.TextCenteredVertically("A RECOLTER", innerLeft + 0.012f, detailY + 0.113f,
                0.029f, 0.275f, 234, 207, 151, 255);
            CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.IntToMoney(net) + " $",
                left + width - 0.019f, detailY + 0.113f,
                0.029f, 0.300f, 218, 176, 101, 255);
        }
        else if (!main)
        {
            CustomMenuRenderer.TextCenteredVertically("STOCK JOUEUR", innerLeft + 0.012f,
                detailY + 0.004f, 0.024f, 0.270f, 234, 207, 151, 255);
            string[] labels = { "Cannabis", "Crack", "Cocaïne" };
            string[] keys = { "Weed", "Crack", "Cocaine" };
            for (int i = 0; i < 3; i++)
            {
                float lineY = detailY + 0.028f + i * 0.023f;
                CustomMenuRenderer.TextCenteredVertically(labels[i], innerLeft + 0.012f,
                    lineY, 0.026f, 0.270f, 226, 227, 229, 255);
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.GramsToOz(SupplyAvailable(keys[i])),
                    left + width - 0.019f, lineY, 0.026f,
                    0.295f, 111, 216, 139, 255);
            }
        }
    }


}
