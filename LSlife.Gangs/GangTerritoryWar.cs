using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

internal static class GangTerritoryWar
{
    internal const int RevenuePerZone = 250;
    private const int RevenueInterval = 600000;
    private const int CaptureReward = 2500;
    private const int RetreatGrace = 12000;
    private const int SpawnGrace = 15000;
    private static readonly int[] WaveSizes = { 3, 4, 5 };
    private static readonly Dictionary<string, string> DefenderModels = new Dictionary<string, string>
    {
        { "Ballas", "g_m_y_ballaeast_01" },
        { "Families", "g_m_y_famfor_01" },
        { "Vagos", "g_m_y_mexgoon_01" },
        { "Marabunta Grande", "g_m_y_salvagoon_01" }
    };

    private static readonly List<Ped> defenders = new List<Ped>();
    private static string contestedZone;
    private static string owner;
    private static int wave;
    private static int playerHandle;
    private static int outsideSince;
    private static int waveAt;
    private static int statusAt;
    private static int nextSpawnAttempt;
    private static int revenueAt;
    private static int cooldownUntil;
    private static int handledDealer = -1;

    internal static bool Active => contestedZone != null;

    internal static bool Owns(string zone) =>
        PlayerGangOrganization.Current?.Territories.Contains(zone) == true;

    internal static void Tick(Player player, Ped dealer, string dealerZone)
    {
        int now = Game.GameTime;
        Ped character = player?.Character;
        if (character == null || !character.Exists() || !((Entity)character).IsAlive)
        {
            if (Active) Finish("Défaite : le joueur est mort.");
            return;
        }

        if (Active)
        {
            if (character.Handle != playerHandle || PlayerGangOrganization.Current == null)
                Finish("Conquête abandonnée.");
            else
                Advance(character, now);
            return;
        }

        if (!player.IsPlaying) return;
        if (revenueAt == 0) revenueAt = now + RevenueInterval;
        if (now >= revenueAt)
        {
            revenueAt = now + RevenueInterval;
            PlayerGangOrganization.CollectTerritoryRevenue(RevenuePerZone);
        }

        if (dealer == null || !dealer.Exists())
        {
            handledDealer = -1;
            return;
        }
        if (now < cooldownUntil || dealer.Handle == handledDealer) return;
        string gang = GangDiscovery.ReferenceOwner(dealerZone);
        if (gang == null || Owns(dealerZone)) return;
        Vector3 position = ((Entity)character).Position;
        if (!InZone(position, dealerZone) || !InZone(((Entity)dealer).Position, dealerZone)) return;

        bool damaged = Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ENTITY, dealer, character, true);
        bool killed = ((Entity)dealer).IsDead && dealer.GetKiller() is Ped killer &&
            killer.Handle == character.Handle;
        if (!damaged && !killed) return;
        handledDealer = dealer.Handle;

        if (PlayerGangOrganization.Current == null)
        {
            GTA.UI.ShowSubtitle("Fonde un gang pour lancer une conquête.", 4000);
            return;
        }

        var agreement = GangRelations.Get(gang).Agreement;
        if (!GangRelations.RecordTerritorialAttack(gang)) return;
        contestedZone = dealerZone;
        owner = gang;
        playerHandle = character.Handle;
        wave = 0;
        waveAt = now + 3000;
        statusAt = waveAt;
        nextSpawnAttempt = waveAt;
        GTA.UI.ShowSubtitle(agreement == GangAgreement.None
            ? "Guerre territoriale : " + contestedZone
            : "Trahison : accord rompu avec " + owner + " — " + contestedZone, 3000);
    }

    private static bool InZone(Vector3 position, string zone) =>
        string.Equals(World.GetZoneNameLabel(position), zone, StringComparison.OrdinalIgnoreCase);

    private static void Advance(Ped character, int now)
    {
        Vector3 position = ((Entity)character).Position;
        if (InZone(position, contestedZone)) outsideSince = 0;
        else
        {
            if (outsideSince == 0) outsideSince = now;
            if (now - outsideSince >= RetreatGrace)
            {
                Finish("Défaite : territoire abandonné.");
                return;
            }
        }

        if (defenders.Any(ped => !ped.Exists()))
        {
            Finish("Conquête interrompue : renfort introuvable.");
            return;
        }

        int remaining = defenders.Count(ped => ((Entity)ped).IsAlive);
        if (defenders.Count != 0 && remaining == 0)
        {
            ClearDefenders();
            wave++;
            if (wave == WaveSizes.Length)
            {
                bool saved = PlayerGangOrganization.Capture(contestedZone, CaptureReward);
                Finish(saved ? "Territoire conquis : " + contestedZone : "Conquête non enregistrée.");
                return;
            }
            waveAt = now + 3500;
            nextSpawnAttempt = waveAt;
        }

        if (defenders.Count == 0 && now >= nextSpawnAttempt && outsideSince == 0)
        {
            nextSpawnAttempt = now + 1000;
            if (!SpawnWave(character, position) && now - waveAt >= SpawnGrace)
            {
                Finish("Conquête interrompue : accès des renforts impossible.");
                return;
            }
            remaining = defenders.Count;
        }

        if (now < statusAt || CustomMenuShared.IsAnyGameplayMenuOpen()) return;
        statusAt = now + 1000;
        string status = outsideSince != 0 ? "Retourne dans le territoire" :
            remaining == 0 ? "Renforts en approche" : remaining + " ennemis restants";
        GTA.UI.ShowSubtitle(contestedZone + " | Vague " + (wave + 1) + "/" +
            WaveSizes.Length + " | " + status, 1100);
    }

    private static bool SpawnWave(Ped character, Vector3 position)
    {
        int count = WaveSizes[wave];
        var positions = new List<Vector3>();
        for (int i = 0; i < 16 && positions.Count < count; i++)
        {
            double angle = i * Math.PI / 8;
            float distance = i % 2 == 0 ? 52f : 76f;
            var offset = new Vector3((float)Math.Cos(angle) * distance, (float)Math.Sin(angle) * distance, 0);
            var candidate = World.GetNextPositionOnSidewalk(position + offset);
            if (candidate.DistanceTo(position) < 35f || candidate.DistanceTo(position) > 110f ||
                !InZone(candidate, contestedZone) || positions.Any(other => other.DistanceTo(candidate) < 7f))
                continue;
            positions.Add(candidate);
        }

        if (positions.Count != count) return false;
        var model = new Model(DefenderModels[owner]);
        foreach (Vector3 spawn in positions)
        {
            Ped defender = World.CreatePed(model, spawn);
            if (defender == null || !defender.Exists())
            {
                ClearDefenders();
                return false;
            }

            defenders.Add(defender);
            ((Entity)defender).IsPersistent = true;
            defender.Weapons.Give(wave == 0 ? WeaponHash.Pistol : WeaponHash.MicroSMG, 180, true, true);
            Function.Call(Hash.TASK_COMBAT_PED, defender, character, 0, 16);
        }
        return true;
    }

    internal static void Abandon()
    {
        if (Active) Finish("Conquête abandonnée.");
    }

    private static void Finish(string message)
    {
        ClearDefenders();
        contestedZone = null;
        owner = null;
        outsideSince = 0;
        cooldownUntil = Game.GameTime + 15000;
        GTA.UI.ShowSubtitle(message, 4500);
    }

    private static void ClearDefenders()
    {
        foreach (Ped defender in defenders)
            if (defender.Exists()) ((Entity)defender).Delete();
        defenders.Clear();
    }
}
