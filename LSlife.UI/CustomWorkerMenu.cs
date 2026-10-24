using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Navigation et rendu propres aux employes. Aucun etat de menu partage avec Zee.
internal static class CustomWorkerMenu
{
    private static CustomMenu workerMenu, workerSupplyMenu, activeMenu;
    private static int supplyWeed, supplyCrack, supplyCocaine, supplyStep = 1;
    private static readonly int[] supplySteps = { 1, 5, 10, 25, 50, 100 };
    private static bool waitForRelease;
    private static int lastTickFrame = -1;
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
            workerMenu.Add("Equipement", () => OpenNativeSubmenu());
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

    private static void OpenWorkerSupply()
    {
        if (LSL.DealerHandler.currentDealer == null) return;
        supplyWeed = supplyCrack = supplyCocaine = 0;
        supplyStep = 1;
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
        if (activeMenu == workerSupplyMenu && DisabledJustPressed(174))
        {
            if (activeMenu.SelectedIndex < 3) ChangeSupply(activeMenu.SelectedIndex, -1);
            else if (activeMenu.SelectedIndex == 3) ChangeSupplyStep(-1);
        }
        else if (activeMenu == workerSupplyMenu && DisabledJustPressed(175))
        {
            if (activeMenu.SelectedIndex < 3) ChangeSupply(activeMenu.SelectedIndex, 1);
            else if (activeMenu.SelectedIndex == 3) ChangeSupplyStep(1);
        }
        else if (DisabledJustPressed(172)) activeMenu.Move(-1);
        else if (DisabledJustPressed(173)) activeMenu.Move(1);
        else if (DisabledJustPressed(201))
        {
            CustomMenu before = activeMenu;
            activeMenu.Activate();
            if (before != activeMenu) waitForRelease = true;
        }
        else if (DisabledJustPressed(202))
        {
            activeMenu.Close();
            if (activeMenu == workerSupplyMenu)
            {
                activeMenu = workerMenu;
                workerMenu.Open();
                waitForRelease = true;
            }
            else activeMenu = null;
        }
        if (Visible) Draw();
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
        float height = 0.102f + activeMenu.Entries.Count * 0.043f +
            (main ? 0.168f : 0.142f);
        Rect(x, top + height / 2f, width, height, 13, 16, 20, 238);
        Rect(x, top, width, 0.002f, 194, 156, 88, 255);
        Rect(x, top + height, width, 0.002f, 194, 156, 88, 255);
        Rect(left, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        Rect(left + width, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        Rect(x, top + 0.046f, width - 0.004f, 0.090f, 22, 25, 29, 240);
        Rect(left + 0.008f, top + 0.046f, 0.003f, 0.062f, 194, 156, 88, 255);
        TextCenteredVertically(activeMenu.Title, left + 0.023f, top, 0.090f,
            main ? 0.67f : 0.46f, 234, 207, 151, 255);
        if (LSL.DealerHandler.currentDealer != null)
            TextRightCenteredVertically("DEALER #" + LSL.DealerHandler.currentDealer.ID,
                left + width - 0.018f, top, 0.090f, 0.270f, 218, 176, 101, 255);
        Rect(x, top + 0.091f, width - 0.019f, 0.0015f, 171, 133, 73, 235);
        for (int i = 0; i < activeMenu.Entries.Count; i++)
        {
            float y = top + 0.098f + i * 0.043f;
            bool selected = i == activeMenu.SelectedIndex;
            Rect(x, y + 0.0205f, width - 0.012f, 0.040f,
                selected ? 65 : 25, selected ? 55 : 28,
                selected ? 40 : 32, selected ? 248 : 229);
            if (selected)
            {
                Rect(left + 0.008f, y + 0.0205f, 0.0035f, 0.038f, 218, 176, 101, 255);
                Rect(x, y + 0.001f, width - 0.012f, 0.001f, 172, 137, 78, 225);
                Rect(x, y + 0.041f, width - 0.012f, 0.001f, 172, 137, 78, 225);
            }
            TextCenteredVertically(activeMenu.Entries[i].Label, left + 0.019f,
                y + 0.0005f, 0.040f, 0.315f, selected ? 255 : 226,
                selected ? 247 : 227, selected ? 226 : 229, 255);
            if (main && i == 1 && LSL.DealerHandler.currentDealer != null)
            {
                int money = LSL.DealerHandler.currentDealer.Money;
                TextRightCenteredVertically(LsFunctions.IntToMoney(money - (money / 100) * 20) + " $",
                    left + width - 0.018f, y + 0.0005f, 0.040f, 0.305f, 111, 216, 139, 255);
            }
            if (!main && i < 3)
            {
                int selectedAmount = i == 0 ? supplyWeed : i == 1 ? supplyCrack : supplyCocaine;
                int available = SupplyAvailable(i == 0 ? "Weed" : i == 1 ? "Crack" : "Cocaine");
                float barLeft = left + 0.116f, barWidth = 0.085f;
                float fraction = available > 0 ? Math.Min(1f, (float)selectedAmount / available) : 0f;
                Rect(barLeft + barWidth / 2f, y + 0.033f, barWidth, 0.004f, 66, 68, 70, 255);
                if (fraction > 0f)
                    Rect(barLeft + barWidth * fraction / 2f, y + 0.033f,
                        barWidth * fraction, 0.004f, 111, 216, 139, 255);
            }
            if (!main && i < 3)
            {
                int amount = i == 0 ? supplyWeed : i == 1 ? supplyCrack : supplyCocaine;
                TextRightCenteredVertically(amount + " g", left + width - 0.018f,
                    y + 0.0005f, 0.040f, 0.305f, 111, 216, 139, 255);
            }
            if (!main && i == 3)
                TextRightCenteredVertically("< x" + supplyStep + " >", left + width - 0.018f,
                    y + 0.0005f, 0.040f, 0.305f, 218, 176, 101, 255);
        }
        float detailY = top + 0.102f + activeMenu.Entries.Count * 0.043f;
        float innerLeft = left + 0.009f, innerWidth = width - 0.018f;
        Rect(x, detailY + (main ? 0.082f : 0.048f), innerWidth,
            main ? 0.153f : 0.134f, 25, 29, 33, 248);
        Rect(x, detailY + 0.004f, innerWidth, 0.001f, 171, 133, 73, 235);
        if (main && LSL.DealerHandler.currentDealer != null)
        {
            PlayerDealer worker = LSL.DealerHandler.currentDealer;
            int hp = worker.Ped != null ? Math.Max(0, worker.Ped.Health) : 0;
            int armor = worker.Ped != null ? Math.Max(0, worker.Ped.Armor) : 0;
            TextCenteredVertically("SANTE  " + hp + "     ARMURE  " + armor,
                innerLeft + 0.012f, detailY + 0.011f, 0.029f, 0.275f, 226, 206, 167, 255);
            string[] names = { "Cannabis", "Crack", "Cocaïne" };
            string[] keys = { "Weed", "Crack", "Cocaine" };
            for (int i = 0; i < 3; i++)
            {
                float lineY = detailY + 0.039f + i * 0.023f;
                int amount = worker.Drugs.ContainsKey(keys[i]) ? worker.Drugs[keys[i]] : 0;
                TextCenteredVertically(names[i], innerLeft + 0.012f, lineY,
                    0.026f, 0.270f, 226, 227, 229, 255);
                TextRightCenteredVertically(amount + " g", left + width - 0.019f,
                    lineY, 0.026f, 0.295f, 111, 216, 139, 255);
            }
            int net = worker.Money - (worker.Money / 100) * 20;
            TextCenteredVertically("A RECOLTER", innerLeft + 0.012f, detailY + 0.113f,
                0.029f, 0.275f, 234, 207, 151, 255);
            TextRightCenteredVertically(LsFunctions.IntToMoney(net) + " $",
                left + width - 0.019f, detailY + 0.113f,
                0.029f, 0.300f, 218, 176, 101, 255);
        }
        else if (!main)
        {
            TextCenteredVertically("STOCK JOUEUR", innerLeft + 0.012f,
                detailY + 0.009f, 0.024f, 0.270f, 234, 207, 151, 255);
            string[] labels = { "Cannabis", "Crack", "Cocaïne" };
            string[] keys = { "Weed", "Crack", "Cocaine" };
            for (int i = 0; i < 3; i++)
            {
                float lineY = detailY + 0.037f + i * 0.026f;
                TextCenteredVertically(labels[i], innerLeft + 0.012f,
                    lineY, 0.026f, 0.270f, 226, 227, 229, 255);
                TextRightCenteredVertically(SupplyAvailable(keys[i]) + " g",
                    left + width - 0.019f, lineY, 0.026f,
                    0.295f, 111, 216, 139, 255);
            }
        }
    }

    // Centre le texte dans une zone verticale en utilisant la hauteur
    // reelle du caractere calculee par GTA pour la police et l'echelle.
    // Les dimensions des zones peuvent changer sans ajuster les offsets.
    private static void TextCenteredVertically(string value, float x, float zoneTop,
        float zoneHeight, float scale, int r, int g, int b, int a)
    {
        const int font = 0;
        float textHeight = Function.Call<float>(
            unchecked((Hash)0xDB88A37483346780UL), scale, font);
        // GTA ancre DRAW_TEXT au sommet de la ligne de police.
        // Utiliser une compensation negative pour remonter le rendu visuel.
        float textY = zoneTop + (zoneHeight - textHeight) / 2f
                    - textHeight * 0.15f;
        Text(value, x, textY, scale, r, g, b, a);
    }

    // Aligne les montants a droite tout en reutilisant le meme centrage vertical.
    private static void TextRightCenteredVertically(string value, float right, float zoneTop,
        float zoneHeight, float scale, int r, int g, int b, int a)
    {
        const int font = 0;
        float textHeight = Function.Call<float>(
            unchecked((Hash)0xDB88A37483346780UL), scale, font);
        float textY = zoneTop + (zoneHeight - textHeight) / 2f
                    - textHeight * 0.15f;
        Function.Call(Hash.SET_TEXT_FONT, font);
        Function.Call(Hash.SET_TEXT_SCALE, 0f, scale);
        Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
        Function.Call(Hash.SET_TEXT_OUTLINE);
        Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, true);
        Function.Call(Hash.SET_TEXT_WRAP, 0f, right);
        Function.Call(unchecked((Hash)0x25FBB336DF1804CBUL), "STRING");
        Function.Call(unchecked((Hash)0x6C188BE134E074AAUL), value);
        Function.Call(unchecked((Hash)0xCD015E5BB0D96A57UL), right, textY);
        Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, false);
    }

    private static void Rect(float x, float y, float w, float h, int r, int g, int b, int a)
    {
        Function.Call(Hash.DRAW_RECT, x, y, w, h, r, g, b, a);
    }

    private static void Text(string value, float x, float y, float scale, int r, int g, int b, int a)
    {
        Function.Call(Hash.SET_TEXT_FONT, 0);
        Function.Call(Hash.SET_TEXT_SCALE, 0f, scale);
        Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
        Function.Call(Hash.SET_TEXT_OUTLINE);
        Function.Call(unchecked((Hash)0x25FBB336DF1804CBUL), "STRING");
        Function.Call(unchecked((Hash)0x6C188BE134E074AAUL), value);
        Function.Call(unchecked((Hash)0xCD015E5BB0D96A57UL), x, y);
    }
}
