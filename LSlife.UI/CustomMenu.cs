using System;
using System.Collections.Generic;

namespace LSlife;

// Noyau independant de NativeUI : donnees et navigation des futurs menus.
// Aucune modification des menus existants tant que le rendu et les commandes
// du prototype Zee ne sont pas valides en jeu.
internal sealed class CustomMenu
{
    private readonly List<CustomMenuEntry> entries = new List<CustomMenuEntry>();

    public string Title { get; }
    public string Subtitle { get; set; }
    public bool Visible { get; private set; }
    public int SelectedIndex { get; private set; }
    public IReadOnlyList<CustomMenuEntry> Entries => entries;

    public CustomMenu(string title, string subtitle)
    {
        Title = title;
        Subtitle = subtitle;
    }

    public void Add(string label, Action action, Func<bool> enabled = null)
    {
        entries.Add(new CustomMenuEntry(label, action, enabled));
    }

    public void Open()
    {
        Visible = true;
        SelectedIndex = 0;
        Move(0);
    }

    public void Close()
    {
        Visible = false;
    }

    public void Move(int direction)
    {
        if (entries.Count == 0) return;
        int next = SelectedIndex;
        for (int i = 0; i < entries.Count; i++)
        {
            next = (next + direction + entries.Count) % entries.Count;
            if (entries[next].Enabled)
            {
                SelectedIndex = next;
                return;
            }
            if (direction == 0) direction = 1;
        }
    }

    public void Activate()
    {
        if (!Visible || entries.Count == 0) return;
        CustomMenuEntry entry = entries[SelectedIndex];
        if (entry.Enabled) entry.Action?.Invoke();
    }
}

internal sealed class CustomMenuEntry
{
    public string Label { get; }
    public Action Action { get; }
    private readonly Func<bool> enabled;
    public bool Enabled => enabled == null || enabled();

    public CustomMenuEntry(string label, Action action, Func<bool> enabled)
    {
        Label = label;
        Action = action;
        this.enabled = enabled;
    }
}
