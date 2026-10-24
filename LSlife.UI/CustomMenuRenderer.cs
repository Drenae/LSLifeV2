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

    // Separateur horizontal avec marges symetriques. Retourne la hauteur occupee.
    public static float Separator(float x, float top, float width, float spacing = 0.015f)
    {
        Rect(x, top + spacing, width, 0.002f, 171, 133, 73, 235);
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
