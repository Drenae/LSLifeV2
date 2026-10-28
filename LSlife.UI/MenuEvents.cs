using System;

namespace LSlife;

public partial class LSL
{
    internal static void NotifyZeeRestock()
    {
        if (!dealer1ReloadOrder) return;
        Enum.TryParse<Enums.DayGTA>(dealer1ReloadDay.ToString(), out var day);
        LsFunctions.TextMsg("ZEE", "Réassort",
            "La commande devrait arriver " + day + " à " + dealer1ReloadHour + ":00");
    }
}
