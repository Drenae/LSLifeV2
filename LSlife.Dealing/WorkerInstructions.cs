using System;
using System.Collections.Generic;

namespace LSlife;

// Instructions des employes sans dependance aux menus NativeUI.
internal static class WorkerInstructions
{
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

    internal static string CurrentWorkerRole()
    {
        PlayerDealer dealer = LSL.DealerHandler.currentDealer;
        return dealer == null ? "" : instructionLabels[CurrentInstructionIndex(dealer)];
    }

    internal static void ChangeWorkerRole(int direction)
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

}
