using System;

namespace LSlife;

// Calcul des quantites de stock et du pas de transfert, independant de NativeUI.
internal static class WorkerSupplyQuantities
{
    internal static int ChangeAmount(int current, string drug, int direction, int step)
    {
        return Math.Max(0, Math.Min(WorkerSupplyTransfer.SupplyAvailable(drug), current + direction * step));
    }

    internal static int ChangeStep(int currentStep, int direction)
    {
        int[] steps = { 1, 5, 10, 25, 50, 100 };
        int index = Array.IndexOf(steps, currentStep);
        return steps[(index + direction + steps.Length) % steps.Length];
    }
}
