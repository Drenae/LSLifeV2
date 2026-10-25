using System;
using GTA.Native;

namespace LSlife;

// Rendu HUD indépendant de NativeUI. Coordonnées en pixels virtuels 1920 x 1080.
// DRAW_RECT attend un centre, contrairement aux rectangles de nos menus.
internal static class HudRender
{
    private const float Width = 1920f;
    private const float Height = 1080f;

    public static void Rect(int left, int top, int width, int height,
        int r, int g, int b, int a)
    {
        if (width <= 0 || height <= 0) return;
        CustomMenuRenderer.Rect((left + width / 2f) / Width,
            (top + height / 2f) / Height, width / Width, height / Height, r, g, b, a);
    }

    public static void Text(string value, int x, int y, float scale,
        int r, int g, int b, int a = 255, bool centered = false)
    {
        Function.Call(Hash.SET_TEXT_FONT, 0);
        Function.Call(Hash.SET_TEXT_SCALE, 0f, scale);
        Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
        Function.Call(Hash.SET_TEXT_OUTLINE);
        Function.Call(Hash.SET_TEXT_CENTRE, centered);
        Function.Call(unchecked((Hash)0x25FBB336DF1804CBUL), "STRING");
        Function.Call(unchecked((Hash)0x6C188BE134E074AAUL), value ?? "");
        Function.Call(unchecked((Hash)0xCD015E5BB0D96A57UL), x / Width, y / Height);
        Function.Call(Hash.SET_TEXT_CENTRE, false);
    }

    public static void Progress(int left, int top, int width, int height, float value)
    {
        float ratio = Math.Max(0f, Math.Min(1f, value));
        Rect(left, top, width, height, 53, 56, 60, 225);
        int filled = (int)(width * ratio);
        if (ratio >= 0.70f) Rect(left, top, filled, height, 213, 83, 78, 235);
        else if (ratio >= 0.35f) Rect(left, top, filled, height, 225, 160, 70, 235);
        else Rect(left, top, filled, height, 123, 187, 117, 235);
    }

    public static void DebugPanel(string title, int x, int y, int width, string[] lines)
    {
        int height = 28 + lines.Length * 18;
        Rect(x, y, width, height, 11, 13, 16, 190);
        Rect(x, y, 3, height, 187, 154, 82, 225);
        Text(title, x + 10, y + 4, 0.28f, 187, 154, 82);
        for (int i = 0; i < lines.Length; i++)
            Text(lines[i], x + 10, y + 25 + i * 18, 0.24f, 235, 235, 235);
    }
}
