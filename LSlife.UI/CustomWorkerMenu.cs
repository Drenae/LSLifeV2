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
    private static readonly CustomMenuShared.HorizontalRepeat horizontalRepeat = new CustomMenuShared.HorizontalRepeat();
    private static int equipmentWeaponIndex;
    public static bool Visible => activeMenu != null && activeMenu.Visible;

    public static void Open()
    {
        if (Visible || LSL.LsMenuPool.IsAnyMenuOpen()) return;
        if (LSL.DealerHandler.currentDealer == null) return;
        if (workerMenu == null)
        {
            workerMenu = new CustomMenu("EMPLOYE", "");
            workerMenu.Add("Approvisionner", OpenWorkerSupply);
            workerMenu.Add("Recolter la recette", WorkerRevenueCollection.CollectWorker,
                () => LSL.DealerHandler.currentDealer != null &&
                    LSL.DealerHandler.currentDealer.Money >= 100);
            workerMenu.Add("Equipement", OpenWorkerEquipment);
            workerMenu.Add("Instruction", () => ChangeWorkerRole(1));
        }
        activeMenu = workerMenu;
        workerMenu.Open();
        waitForRelease = true;
    }

    // Instructions utilisateur distinctes des états techniques (TalkToPlayer, etc.).
    private static readonly string[] instructionStates = { "None", "FollowingPlayer", "Work" };
    private static readonly string[] instructionLabels = { "Ne rien faire", "Me suivre", "Travailler" };
    private static readonly System.Collections.Generic.Dictionary<int, int> selectedInstructions =
        new System.Collections.Generic.Dictionary<int, int>();

    private static int CurrentInstructionIndex(PlayerDealer dealer)
    {
        if (dealer == null) return 0;
        int saved;
        if (selectedInstructions.TryGetValue(dealer.ID, out saved)) return saved;
        string state = dealer.NewState.ToString();
        if (state == "Work" || state == "Working") return 2;
        if (state == "Follow" || state == "FollowingPlayer") return 1;
        return 0;
    }

    private static string CurrentWorkerRole()
    {
        PlayerDealer dealer = LSL.DealerHandler.currentDealer;
        return dealer == null ? "" : instructionLabels[CurrentInstructionIndex(dealer)];
    }

    private static void ChangeWorkerRole(int direction)
    {
        PlayerDealer dealer = LSL.DealerHandler.currentDealer;
        if (dealer == null) return;
        int index = (CurrentInstructionIndex(dealer) + direction + instructionStates.Length) %
            instructionStates.Length;
        Enums.PdStates state;
        if (!Enum.TryParse(instructionStates[index], out state)) return;
        dealer.NewState = state;
        selectedInstructions[dealer.ID] = index;
        LSL.DealerHandler.needsSave = true;
    }

    private static void OpenWorkerEquipment()
    {
        if (LSL.DealerHandler.currentDealer == null) return;
        equipmentWeaponIndex = 0;
        workerEquipmentMenu = new CustomMenu("EQUIPEMENT", "");
        workerEquipmentMenu.Add("Donner un gilet pare-balles", WorkerArmourTransfer.GiveWorkerArmour,
            () => LSL.playerArmours > 0 && LSL.DealerHandler.currentDealer != null &&
                  LSL.DealerHandler.currentDealer.Ped != null &&
                  LSL.DealerHandler.currentDealer.Ped.Armor < 200);
        workerEquipmentMenu.Add("Arme", () => GiveWorkerWeapon(equipmentWeaponIndex),
            () => LSL.playerWeps.Count > 0);
        workerMenu.Close();
        activeMenu = workerEquipmentMenu;
        workerEquipmentMenu.Open();
    }

    private static void GiveWorkerWeapon(int index)
    {
        if (LSL.DealerHandler.currentDealer == null || index < 0 ||
            index >= LSL.playerWeps.Count) return;
        WorkerWeaponTransfer.GiveWeaponToWorker(index);
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
        horizontalRepeat.Reset();
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
        if (activeMenu == workerMenu && activeMenu.SelectedIndex == 3 && HandleWorkerRoleInput())
        {
        }
        else if (activeMenu == workerEquipmentMenu && activeMenu.SelectedIndex == 1 &&
            HandleEquipmentHorizontalInput())
        {
        }
        else if (activeMenu == workerSupplyMenu && activeMenu.SelectedIndex < 4 &&
            HandleSupplyHorizontalInput())
        {
            // Une impulsion initiale, puis repetition temporisee pendant le maintien.
        }
        else if (DisabledJustPressed(172)) { horizontalRepeat.Reset(); activeMenu.Move(-1); }
        else if (DisabledJustPressed(173)) { horizontalRepeat.Reset(); activeMenu.Move(1); }
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

    private static bool HandleWorkerRoleInput()
    {
        int direction = horizontalRepeat.Poll(250);
        if (direction == 0) return false;
        ChangeWorkerRole(direction);
        return true;
    }

    private static bool HandleEquipmentHorizontalInput()
    {
        int direction = horizontalRepeat.Poll(110);
        if (direction == 0) return false;
        int count = LSL.playerWeps.Count;
        if (count > 0) equipmentWeaponIndex = (equipmentWeaponIndex + direction + count) % count;
        return true;
    }

    private static bool HandleSupplyHorizontalInput()
    {
        int direction = horizontalRepeat.Poll(65);
        if (direction == 0) return false;
        if (activeMenu.SelectedIndex < 3) ChangeSupply(activeMenu.SelectedIndex, direction);
        else ChangeSupplyStep(direction);
        return true;
    }

    private static bool DisabledJustPressed(int control)
    {
        return Function.Call<bool>(unchecked((Hash)0x91AEF906BCA88877UL), 2, control);
    }

    // Premiere migration : seul le menu principal EMPLOYE utilise le flux vertical.
    // Les sous-menus restent sur leur rendu valide tant que le nouveau rendu
    // n'a pas ete teste dans GTA V.
    private static void DrawWorkerMainFlow()
    {
        PlayerDealer worker = LSL.DealerHandler.currentDealer;
        if (worker == null) return;

        var layout = new CustomMenuLayout();
        layout.Header(workerMenu.Title, "DEALER #" + worker.ID, titleScale: 0.67f);
        // Pas de coordonnees Y : le separateur reserve son espace.
        layout.Separator(0.004f);
        for (int i = 0; i < workerMenu.Entries.Count; i++)
        {
            string value = null;
            if (i == 3) value = "< " + CurrentWorkerRole() + " >";
            if (i == 1)
            {
                int money = worker.Money;
                value = LsFunctions.IntToMoney(money - (money / 100) * 20) + " $";
            }
            layout.Row(workerMenu.Entries[i].Label, i == workerMenu.SelectedIndex,
                value, enabled: workerMenu.Entries[i].Enabled);
        }
        layout.Separator(0.006f);
        layout.TextPair("Instruction", CurrentWorkerRole());
        layout.TextPair("Recette brute", LsFunctions.IntToMoney(worker.Money) + " $");
        layout.Block(0.029f, y =>
        {
            int hp = worker.Ped != null ? Math.Max(0, worker.Ped.Health) : 0;
            int armor = worker.Ped != null ? Math.Max(0, worker.Ped.Armor) : 0;
            CustomMenuRenderer.TextCenteredVertically("SANTE  " + hp + "     ARMURE  " + armor,
                layout.Left + 0.021f, y, 0.029f, 0.255f, 226, 206, 167, 255);
        });
        string[] names = { "Cannabis", "Crack", "Cocaïne" };
        string[] keys = { "Weed", "Crack", "Cocaine" };
        for (int i = 0; i < names.Length; i++)
        {
            string label = names[i];
            string key = keys[i];
            layout.Block(0.023f, y =>
            {
                int amount = worker.Drugs.ContainsKey(key) ? worker.Drugs[key] : 0;
                CustomMenuRenderer.TextCenteredVertically(label, layout.Left + 0.021f,
                    y, 0.023f, 0.270f, 226, 227, 229, 255);
                CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.GramsToOz(amount),
                    layout.Right - 0.019f, y, 0.023f, 0.295f, 111, 216, 139, 255);
            });
        }
        layout.Block(0.029f, y =>
        {
            int net = worker.Money - (worker.Money / 100) * 20;
            CustomMenuRenderer.TextCenteredVertically("A RECOLTER", layout.Left + 0.021f,
                y, 0.029f, 0.275f, 234, 207, 151, 255);
            CustomMenuRenderer.TextRightCenteredVertically(LsFunctions.IntToMoney(net) + " $",
                layout.Right - 0.019f, y, 0.029f, 0.300f, 218, 176, 101, 255);
        });
        layout.Render();
    }

    private static void Draw()
    {
        if (activeMenu == workerMenu)
        {
            DrawWorkerMainFlow();
            return;
        }
        bool supply = activeMenu == workerSupplyMenu;
        bool equipment = activeMenu == workerEquipmentMenu;
        PlayerDealer dealer = LSL.DealerHandler.currentDealer;
        if (dealer == null) return;

        var layout = new CustomMenuLayout();
        layout.Header(activeMenu.Title, "DEALER #" + dealer.ID);
        layout.Separator(0.004f);
        for (int i = 0; i < activeMenu.Entries.Count; i++)
        {
            int index = i;
            bool selected = i == activeMenu.SelectedIndex;
            string value = null;
            if (supply && i < 3)
            {
                int amount = i == 0 ? supplyWeed : i == 1 ? supplyCrack : supplyCocaine;
                value = LsFunctions.GramsToOz(amount);
            }
            else if (supply && i == 3)
                value = "< x" + supplyStep + " >";
            else if (equipment && i == 1)
            {
                string weapon = LSL.playerWeps.Count == 0 ? "Aucune" :
                    CustomMenuShared.FrenchWeaponName(LSL.playerWeps[Math.Min(equipmentWeaponIndex, LSL.playerWeps.Count - 1)]);
                value = "< " + weapon + " >";
            }
            layout.Row(activeMenu.Entries[i].Label, selected, value,
                enabled: activeMenu.Entries[i].Enabled);
            if (supply && i < 3)
            {
                layout.Block(0f, y =>
                {
                    int amount = index == 0 ? supplyWeed : index == 1 ? supplyCrack : supplyCocaine;
                    int available = SupplyAvailable(index == 0 ? "Weed" : index == 1 ? "Crack" : "Cocaine");
                    float fraction = available > 0 ? Math.Min(1f, (float)amount / available) : 0f;
                    float barLeft = layout.Left + 0.116f, barWidth = 0.085f;
                    float barY = y - 0.043f / 2f;
                    CustomMenuRenderer.Rect(barLeft + barWidth / 2f, barY, barWidth, 0.008f, 66, 68, 70, 255);
                    if (fraction > 0f)
                        CustomMenuRenderer.Rect(barLeft + barWidth * fraction / 2f, barY,
                            barWidth * fraction, 0.008f, 111, 216, 139, 255);
                });
            }
        }
        layout.Separator(0.006f);
        if (equipment)
        {
            layout.Block(0.026f, y =>
                CustomMenuRenderer.TextCenteredVertically("EQUIPEMENT DU DEALER",
                    layout.Left + 0.019f, y, 0.026f, 0.265f, 234, 207, 151, 255));
            int protection = dealer.Ped != null ? dealer.Ped.Armor : dealer.dArmour;
            layout.TextPair("Protection", protection.ToString());
            string equipped = "Aucune";
            if (dealer.Ped != null && dealer.Ped.Weapons != null)
                equipped = CustomMenuShared.FrenchWeaponName(dealer.Ped.Weapons.Current.Hash.ToString());
            layout.TextPair("Arme actuelle", equipped);
        }
        else
        {
            layout.Block(0.026f, y =>
                CustomMenuRenderer.TextCenteredVertically("STOCK JOUEUR",
                    layout.Left + 0.019f, y, 0.026f, 0.270f, 234, 207, 151, 255));
            layout.TextPair("Cannabis", LsFunctions.GramsToOz(SupplyAvailable("Weed")));
            layout.TextPair("Crack", LsFunctions.GramsToOz(SupplyAvailable("Crack")));
            layout.TextPair("Cocaïne", LsFunctions.GramsToOz(SupplyAvailable("Cocaine")));
        }
        layout.Render();
    }
}
