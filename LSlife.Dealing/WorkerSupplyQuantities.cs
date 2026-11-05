using System;

namespace LSlife;

// Calcul des quantites de stock et du pas de transfert, independant de NativeUI.
internal static class WorkerSupplyQuantities
{
    internal static int SuggestedAmount(PlayerDealer worker, string drug, int target)
    {
        int current;
        if (worker?.Drugs == null || !worker.Drugs.TryGetValue(drug, out current)) return 0;
        return (int)Math.Min(WorkerSupplyTransfer.SupplyAvailable(drug),
            Math.Max(0L, (long)target - Math.Max(0, current)));
    }

    internal static int ChangeAmount(int current, string drug, int direction, int step)
    {
        return (int)Math.Max(0L, Math.Min(WorkerSupplyTransfer.SupplyAvailable(drug), (long)current + (long)direction * step));
    }

    internal static int ChangeStep(int currentStep, int direction)
    {
        int[] steps = { 1, 5, 10, 25, 50, 100 };
        int index = Array.IndexOf(steps, currentStep);
        return steps[(index + direction + steps.Length) % steps.Length];
    }
}
