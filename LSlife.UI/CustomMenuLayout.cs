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
            width - 2f * inset, thickness, 130, 130, 130, 115));
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
                width - 0.002f, height, 10, 10, 12, 105);
            bool hasSubtitle = !string.IsNullOrEmpty(right);
            float titleHeight = hasSubtitle ? height * 0.56f : height;
            CustomMenuRenderer.TextCenteredVertically(title, Left + 0.019f,
                y, titleHeight, Math.Min(titleScale, 0.49f), 226, 180, 94, 255);
            if (hasSubtitle)
                CustomMenuRenderer.TextCenteredVertically(right, Left + 0.019f,
                    y + titleHeight, height - titleHeight, 0.245f, 207, 207, 209, 255);
        });
        cursor += height;
    }

    public void Row(string label, bool selected, string value = null,
        float height = 0.043f, bool enabled = true, bool redValue = false, bool dimValue = false, bool emphasizeValue = false, string availableValue = null)
    {
        if (height <= 0f) throw new ArgumentOutOfRangeException(nameof(height));
        float y = cursor;
        draws.Add(() =>
        {
            CustomMenuRenderer.RowBackground(centerX, y, width, height, selected);
            int color = enabled ? 234 : 133;
            CustomMenuRenderer.TextCenteredVertically(label, Left + 0.019f,
                y, height, 0.315f, selected && enabled ? 255 : color,
                selected && enabled ? 255 : color, selected && enabled ? 255 : color, 255);
            if (!string.IsNullOrEmpty(value))
            {
                bool money = value.Contains("$");
                int r = dimValue ? 120 : redValue ? 226 : money ? 153 : 230;
                int g = dimValue ? 120 : redValue ? 85 : money ? 211 : 230;
                int b = dimValue ? 120 : redValue ? 85 : money ? 122 : 232;
                CustomMenuRenderer.TextRightCenteredVertically(value,
                    string.IsNullOrEmpty(availableValue) ? Right - 0.019f : Right - 0.094f,
                    y, height, emphasizeValue ? 0.28f : 0.27f, r, g, b, 255);
            }
            if (!string.IsNullOrEmpty(availableValue))
                CustomMenuRenderer.TextRightCenteredVertically(availableValue, Right - 0.019f,
                    y, height, 0.245f, dimValue ? 105 : 174, dimValue ? 105 : 176, dimValue ? 105 : 180, 255);
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
                y, height, 0.265f, value != null && value.Contains("$") ? 153 : 230,
                value != null && value.Contains("$") ? 211 : 230,
                value != null && value.Contains("$") ? 122 : 232, 255);
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
        CustomMenuRenderer.Rect(centerX, top + height / 2f, width, height, 9, 10, 12, 205);
        foreach (Action draw in draws) draw();
    }
}
