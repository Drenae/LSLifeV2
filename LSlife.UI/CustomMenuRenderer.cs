using GTA.Native;

namespace LSlife;

// Primitives graphiques partagees entre les menus personnalises.
internal static class CustomMenuRenderer
{
    // Centre le texte dans une zone verticale en utilisant la hauteur
    // reelle du caractere calculee par GTA pour la police et l'echelle.
    // Les dimensions des zones peuvent changer sans ajuster les offsets.
    public static void TextCenteredVertically(string value, float x, float zoneTop,
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
    public static void TextRightCenteredVertically(string value, float right, float zoneTop,
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

    public const float MenuX = 0.149f;
    public const float MenuWidth = 0.268f;
    public const float MenuTop = 0.035f;
    public const float RowHeight = 0.043f;
    public const float HeaderHeight = 0.090f;

    public static void Frame(float height)
    {
        float x = MenuX, width = MenuWidth, top = MenuTop;
        float left = x - width / 2f;
        Rect(x, top + height / 2f, width, height, 13, 16, 20, 238);
        Rect(x, top, width, 0.002f, 194, 156, 88, 255);
        Rect(x, top + height, width, 0.002f, 194, 156, 88, 255);
        Rect(left, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        Rect(left + width, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
    }

    public static void Header(string title, float titleScale, string rightText = null,
        string subtitle = null, float subtitleScale = 0.25f)
    {
        float x = MenuX, width = MenuWidth, top = MenuTop;
        float left = x - width / 2f;
        Rect(x, top + 0.046f, width - 0.004f, HeaderHeight, 22, 25, 29, 240);
        Rect(left + 0.008f, top + 0.046f, 0.003f, 0.062f, 194, 156, 88, 255);
        TextCenteredVertically(title, left + 0.023f, top, HeaderHeight,
            titleScale, 234, 207, 151, 255);
        if (!string.IsNullOrEmpty(rightText))
            TextRightCenteredVertically(rightText, left + width - 0.018f,
                top, HeaderHeight, 0.270f, 218, 176, 101, 255);
        if (!string.IsNullOrEmpty(subtitle))
            TextCenteredVertically(subtitle, left + 0.116f, top, HeaderHeight,
                subtitleScale, 170, 159, 138, 255);
        Separator(x, top + 0.091f - 0.0065f, width - 0.019f);
    }

    public static void Row(float y, bool selected)
    {
        float x = MenuX, width = MenuWidth;
        float left = x - width / 2f;
        Rect(x, y + 0.0205f, width - 0.012f, 0.040f,
            selected ? 65 : 25, selected ? 55 : 28,
            selected ? 40 : 32, selected ? 248 : 229);
        if (selected)
        {
            Rect(left + 0.008f, y + 0.0205f, 0.0035f, 0.038f, 218, 176, 101, 255);
            Rect(x, y + 0.001f, width - 0.012f, 0.001f, 172, 137, 78, 225);
            Rect(x, y + 0.041f, width - 0.012f, 0.001f, 172, 137, 78, 225);
        }
    }

    // Separateur horizontal avec marges symetriques. Retourne la hauteur occupee.
    public static float Separator(float x, float top, float width, float spacing = 0.015f)
    {
        Rect(x, top + spacing, width, 0.001f, 171, 133, 73, 235);
        return spacing * 2f + 0.001f;
    }

    public static void Rect(float x, float y, float w, float h, int r, int g, int b, int a)
    {
        Function.Call(Hash.DRAW_RECT, x, y, w, h, r, g, b, a);
    }

    public static void Text(string value, float x, float y, float scale, int r, int g, int b, int a)
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
