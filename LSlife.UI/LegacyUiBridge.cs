namespace LSlife;

/// <summary>Compatibility facade for legacy LS Life notification calls on SHVDN3.</summary>
internal static class UI
{
    public static void Notify(string message)
    {
        GTA.UI.Notification.Show(message);
    }

    public static void ShowSubtitle(string message, int duration = 2500)
    {
        GTA.UI.Screen.ShowSubtitle(message, duration);
    }

    public static void ShowHelpMessage(string message)
    {
        GTA.UI.Screen.ShowHelpTextThisFrame(message);
    }
}
