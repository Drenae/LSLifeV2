using System;

namespace LSlife;

// Calculs purs des quantites et multiplicateurs des commandes Zee.
internal static class ZeeOrderQuantities
{
    internal static decimal Adjust(decimal current, int multiplier, decimal stock, int direction)
    {
        decimal value = current + direction * multiplier;
        if (value < 0) value = 0;
        if (value > stock) value = stock;
        return value;
    }

    internal static int NextMultiplier(int current, int direction)
    {
        int[] multipliers = { 1, 5, 10, 25, 50, 100 };
        int index = Array.IndexOf(multipliers, current);
        if (index < 0) index = 0;
        return multipliers[(index + direction + multipliers.Length) % multipliers.Length];
    }
}
