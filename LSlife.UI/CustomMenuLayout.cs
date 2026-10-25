using System;
using System.Collections.Generic;

namespace LSlife;

// Layout vertical pour les menus GTA : seule l'origine et la largeur sont fixes.
// Les blocs consomment leur hauteur et deplacent automatiquement les suivants.
internal sealed class CustomMenuLayout
{
    private readonly float centerX;
    private readonly float top;
    private readonly float width;
    private readonly List<Action> draws = new List<Action>();
    private float cursor;
    private readonly float inset;

    public float Height => cursor - top;
    public float NextY => cursor;
    public float Left => centerX - width / 2f;
    public float Right => centerX + width / 2f;

    public CustomMenuLayout(float centerX = 0.149f, float top = 0.035f,
        float width = 0.268f, float inset = 0.0095f)
    {
        if (width <= 0f || top < 0f || inset < 0f || inset * 2f >= width)
            throw new ArgumentOutOfRangeException(nameof(width));
        this.centerX = centerX;
        this.top = top;
        this.width = width;
        this.inset = inset;
        cursor = top;
    }

    // Reserve une zone et laisse le composant la dessiner a sa position calculee.
    public void Block(float height, Action<float> draw)
    {
        if (height < 0f) throw new ArgumentOutOfRangeException(nameof(height));
        float y = cursor;
        if (draw != null) draws.Add(() => draw(y));
        cursor += height;
    }

    public void Space(float height)
    {
        if (height < 0f) throw new ArgumentOutOfRangeException(nameof(height));
        cursor += height;
    }

    // Un separateur reserve reellement ses marges dans le flux.
    public void Separator(float spacing = 0.006f, float thickness = 0.001f)
    {
        if (spacing < 0f || thickness <= 0f)
            throw new ArgumentOutOfRangeException(nameof(spacing));
        float centerY = cursor + spacing + thickness / 2f;
        draws.Add(() => CustomMenuRenderer.Rect(centerX, centerY,
            width - 2f * inset, thickness, 171, 133, 73, 235));
        cursor += spacing * 2f + thickness;
    }

    public void Header(string title, string right = null, float height = 0.090f,
        float titleScale = 0.46f)
    {
        if (height <= 0f) throw new ArgumentOutOfRangeException(nameof(height));
        float y = cursor;
        draws.Add(() =>
        {
            CustomMenuRenderer.Rect(centerX, y + height / 2f,
                width - 0.004f, height, 22, 25, 29, 240);
            CustomMenuRenderer.Rect(Left + 0.008f, y + height / 2f,
                0.003f, Math.Max(0f, height - 0.028f), 194, 156, 88, 255);
            CustomMenuRenderer.TextCenteredVertically(title, Left + 0.023f,
                y, height, titleScale, 234, 207, 151, 255);
            if (!string.IsNullOrEmpty(right))
                CustomMenuRenderer.TextRightCenteredVertically(right, Right - 0.018f,
                    y, height, 0.270f, 218, 176, 101, 255);
        });
        cursor += height;
    }

    public void Row(string label, bool selected, string value = null,
        float height = 0.043f, bool enabled = true, bool redValue = false)
    {
        if (height <= 0f) throw new ArgumentOutOfRangeException(nameof(height));
        float y = cursor;
        draws.Add(() =>
        {
            CustomMenuRenderer.Rect(centerX, y + height / 2f,
                width - 0.012f, height - 0.003f,
                selected ? 65 : 39, selected ? 55 : 43,
                selected ? 40 : 48, selected ? 248 : 245);
            if (selected)
            {
                CustomMenuRenderer.Rect(Left + 0.008f, y + height / 2f,
                    0.0035f, height - 0.005f, 218, 176, 101, 255);
                CustomMenuRenderer.Rect(centerX, y + 0.001f, width - 0.012f,
                    0.001f, 172, 137, 78, 225);
                CustomMenuRenderer.Rect(centerX, y + height - 0.002f, width - 0.012f,
                    0.001f, 172, 137, 78, 225);
            }
            int color = enabled ? 234 : 133;
            CustomMenuRenderer.TextCenteredVertically(label, Left + 0.019f,
                y, height, 0.315f, selected ? 255 : color,
                selected ? 247 : color, selected ? 226 : color, 255);
            if (!string.IsNullOrEmpty(value))
                CustomMenuRenderer.TextRightCenteredVertically(value, Right - 0.019f,
                    y, height, 0.27f, redValue ? 226 : 111, redValue ? 85 : 216, redValue ? 85 : 139, 255);
        });
        cursor += height;
    }

    public void TextPair(string label, string value, float height = 0.025f)
    {
        if (height <= 0f) throw new ArgumentOutOfRangeException(nameof(height));
        float y = cursor;
        draws.Add(() =>
        {
            CustomMenuRenderer.TextCenteredVertically(label, Left + 0.019f,
                y, height, 0.265f, 226, 227, 229, 255);
            CustomMenuRenderer.TextRightCenteredVertically(value, Right - 0.019f,
                y, height, 0.265f, 111, 216, 139, 255);
        });
        cursor += height;
    }

    // Ligne informative avec couleur de valeur configurable, sans impact sur les autres menus.
    public void ColoredTextPair(string label, string value, int r, int g, int b, float height = 0.025f)
    {
        if (height <= 0f) throw new ArgumentOutOfRangeException(nameof(height));
        float y = cursor;
        draws.Add(() =>
        {
            CustomMenuRenderer.TextCenteredVertically(label, Left + 0.019f,
                y, height, 0.265f, 226, 227, 229, 255);
            CustomMenuRenderer.TextRightCenteredVertically(value, Right - 0.019f,
                y, height, 0.265f, r, g, b, 255);
        });
        cursor += height;
    }

    // Fond dessine avant les elements, sans calcul manuel de hauteur.
    public void Render(float bottomPadding = 0.009f)
    {
        if (bottomPadding < 0f) throw new ArgumentOutOfRangeException(nameof(bottomPadding));
        float height = Height + bottomPadding;
        CustomMenuRenderer.Rect(centerX, top + height / 2f, width, height, 13, 16, 20, 238);
        CustomMenuRenderer.Rect(centerX, top, width, 0.002f, 194, 156, 88, 255);
        CustomMenuRenderer.Rect(centerX, top + height, width, 0.002f, 194, 156, 88, 255);
        CustomMenuRenderer.Rect(Left, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        CustomMenuRenderer.Rect(Right, top + height / 2f, 0.0015f, height, 151, 119, 69, 220);
        foreach (Action draw in draws) draw();
    }
}
