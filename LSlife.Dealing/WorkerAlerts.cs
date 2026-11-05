using System;
using System.Collections.Generic;
using GTA;

namespace LSlife;

// Alertes par employe, uniquement sur changement reel de stock ou de recette.
// La premiere observation initialise les references sans declencher de SMS.
internal static class WorkerAlerts
{
    private sealed class Snapshot
    {
        internal readonly Dictionary<string, int> Stock = new Dictionary<string, int>();
        internal int Money;
        internal bool RevenueNotified;
        internal readonly HashSet<string> StockNotified = new HashSet<string>();
        internal bool AllEmptyNotified;
        internal readonly HashSet<string> EmptyNotified = new HashSet<string>();
        internal bool StockChangedPending;
    }

    private static readonly Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot>();
    private static readonly string[] keys = { "Weed", "Crack", "Cocaine" };
    private static readonly string[] labels = { "cannabis", "crack", "cocaine" };
    private static bool configured;
    private static bool enabled;
    private static int stockThreshold;
    private static int revenueThreshold;
    private static int cooldownMs;
    private static int lastMessageTime = -1;

    private static void Configure()
    {
        if (configured) return;
        configured = true;
        enabled = LSL.iniSettings.GetValue<bool>("WORKER_ALERTS", "ENABLED", true);
        stockThreshold = Math.Max(0, LSL.iniSettings.GetValue<int>("WORKER_ALERTS", "STOCK_THRESHOLD", 0));
        revenueThreshold = Math.Max(1, LSL.iniSettings.GetValue<int>("WORKER_ALERTS", "REVENUE_THRESHOLD", 10000));
        cooldownMs = (int)Math.Min(int.MaxValue, (long)Math.Max(0,
            LSL.iniSettings.GetValue<int>("WORKER_ALERTS", "COOLDOWN_SECONDS", 30)) * 1000);
    }

    internal static void Observe(PlayerDealer worker)
    {
        if (worker == null || worker.CurrentState == Enums.PdStates.Dead) return;
        Configure();
        if (!enabled) return;
        Snapshot s;
        if (!snapshots.TryGetValue(worker.ID, out s))
        {
            s = new Snapshot { Money = worker.Money };
            foreach (string key in keys) s.Stock[key] = Read(worker, key);
            s.RevenueNotified = NetRevenue(worker.Money) >= revenueThreshold;
            s.AllEmptyNotified = AllEmpty(worker);
            foreach (string key in keys)
            {
                if (s.Stock[key] <= stockThreshold) s.StockNotified.Add(key);
                if (s.Stock[key] == 0) s.EmptyNotified.Add(key);
            }
            snapshots[worker.ID] = s;
            return;
        }

        bool changed = false;
        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            int now = Read(worker, key), before = s.Stock[key];
            if (now != before) changed = true;
            if (now > stockThreshold) s.StockNotified.Remove(key);
            if (now > 0) s.EmptyNotified.Remove(key);
            s.Stock[key] = now;
        }

        bool empty = AllEmpty(worker);
        if (!empty) s.AllEmptyNotified = false;
        if (NetRevenue(worker.Money) < revenueThreshold) s.RevenueNotified = false;

        // Les notifications non emises (cooldown) restent eligibles au prochain passage,
        // mais seulement apres une transition de stock initiale.
        if (changed) s.StockChangedPending = true;
        if (s.StockChangedPending)
        {
            if (empty && !s.AllEmptyNotified)
            {
                if (Send(worker, "Je n'ai plus aucune drogue a vendre !"))
                {
                    s.AllEmptyNotified = true;
                    s.StockChangedPending = false;
                    foreach (string key in keys)
                    {
                        s.StockNotified.Add(key);
                        s.EmptyNotified.Add(key);
                    }
                }
            }
            else if (!empty)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    string key = keys[i];
                    bool exhausted = s.Stock[key] == 0 && !s.EmptyNotified.Contains(key);
                    bool low = s.Stock[key] <= stockThreshold && !s.StockNotified.Contains(key);
                    if (!exhausted && !low) continue;
                    string message = exhausted
                        ? "Mon stock de " + labels[i] + " est épuisé."
                        : "Il me reste " + LsFunctions.FormatWeight(s.Stock[key]) + " de " + labels[i] + ".";
                    if (Send(worker, message))
                    {
                        s.StockNotified.Add(key);
                        if (exhausted) s.EmptyNotified.Add(key);
                    }
                    break;
                }
            }
            if (!empty && !HasUnnotifiedLowStock(worker, s)) s.StockChangedPending = false;
        }

        if (NetRevenue(worker.Money) >= revenueThreshold && !s.RevenueNotified && (worker.Money != s.Money || NetRevenue(s.Money) >= revenueThreshold))
        {
            if (Send(worker, "J'ai " + LsFunctions.IntToMoney(NetRevenue(worker.Money)) + " $ de recette a recolter."))
                s.RevenueNotified = true;
        }
        s.Money = worker.Money;
    }

    private static bool HasUnnotifiedLowStock(PlayerDealer worker, Snapshot s)
    {
        foreach (string key in keys)
            if ((Read(worker, key) == 0 && !s.EmptyNotified.Contains(key)) ||
                (Read(worker, key) <= stockThreshold && !s.StockNotified.Contains(key))) return true;
        return false;
    }

    internal static void Forget(int id) { snapshots.Remove(id); }

    private static int NetRevenue(int gross) { return gross - (gross / 100) * 20; }

    private static int Read(PlayerDealer worker, string key)
    {
        int value;
        return worker.Drugs != null && worker.Drugs.TryGetValue(key, out value) ? Math.Max(0, value) : 0;
    }

    private static bool AllEmpty(PlayerDealer worker)
    {
        foreach (string key in keys) if (Read(worker, key) > 0) return false;
        return true;
    }

    private static bool Send(PlayerDealer worker, string message)
    {
        int now = Game.GameTime;
        if (lastMessageTime >= 0 && now >= lastMessageTime && now - lastMessageTime < cooldownMs) return false;
        lastMessageTime = now;
        // Utilise le systeme de messages du mod : son SMS + notification GTA.
        LsFunctions.SendTextMsg("Employe #" + worker.ID + " : " + message);
        return true;
    }
}
