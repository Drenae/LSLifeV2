using System;
using GTA;
using GTA.Native;

namespace LSlife;

// Fonctions reutilisables pour les menus personnalises.
internal static class CustomMenuShared
{
    // Point d'entree unique pour les interactions du monde et le HUD.
    internal static bool IsAnyGameplayMenuOpen()
    {
        return CustomLeaderNegotiationMenu.Visible
            || CustomZeeMenu.Visible
            || CustomStashMenu.Visible
            || CustomCustomerMenu.Visible
            || CustomWeaponDealerMenu.Visible
            || CustomHireDealerMenu.Visible
            || CustomHudOptionsMenu.Visible
            || CustomWorkerMenu.Visible;
    }

    // Les touches du menu doivent rester lisibles via IS_DISABLED_CONTROL_*,
    // sans declencher leurs actions GTA (phares, radio, telephone, conduite, etc.).
    // A appeler chaque frame avant les ticks des menus.
    internal static void BlockGameplayControlsWhileMenuOpen()
    {
        if (!IsAnyGameplayMenuOpen()) return;
        Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
        Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 1);
        Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 2);
        // Le stick gauche reste reserve aux deplacements a pied, pas aux menus.
        // Les commandes du D-pad et les boutons de validation restent bloques.
        if (!Game.Player.Character.IsInVehicle())
        {
            foreach (int control in new[] { 30, 31, 32, 33, 34, 35 })
            {
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, control, true);
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 2, control, true);
            }
        }
    }

    public static string FrenchWeaponName(string weapon)
    {
        switch (weapon.ToUpperInvariant())
        {
            case "FIREEXTINGUISHER": return "Extincteur";
            case "PETROLCAN": return "Bidon d'essence";
            case "STUNGUN": return "Pistolet paralysant";
            case "STICKYBOMB": return "Bombe collante";
            case "MINIGUN": return "Minigun";
            case "BAT": return "Batte de baseball";
            case "KNIFE": return "Couteau";
            case "DAGGER": return "Dague";
            case "GOLFCLUB": return "Club de golf";
            case "HAMMER": return "Marteau";
            case "GUSENBERG": return "Mitraillette Gusenberg";
            case "PISTOL": return "Pistolet";
            case "COMBATPISTOL": return "Pistolet de combat";
            case "APPISTOL": return "Pistolet automatique";
            case "PISTOL50": return "Pistolet calibre .50";
            case "SNSPISTOL": return "Pistolet compact";
            case "HEAVYPISTOL": return "Pistolet lourd";
            case "VINTAGEPISTOL": return "Pistolet vintage";
            case "MICROSMG": return "Micro-mitraillette";
            case "SMG": return "Mitraillette";
            case "ASSAULTSMG": return "Mitraillette d'assaut";
            case "COMBATPDW": return "ADP de combat";
            case "ASSAULTRIFLE": return "Fusil d'assaut";
            case "CARBINERIFLE": return "Carabine";
            case "ADVANCEDRIFLE": return "Fusil avance";
            case "SPECIALCARBINE": return "Carabine speciale";
            case "BULLPUPRIFLE": return "Fusil bullpup";
            case "PUMPSHOTGUN": return "Fusil a pompe";
            case "SAWNOFFSHOTGUN": return "Fusil a canon scie";
            case "ASSAULTSHOTGUN": return "Fusil a pompe d'assaut";
            case "HEAVYSHOTGUN": return "Fusil a pompe lourd";
            case "SNIPERRIFLE": return "Fusil de precision";
            case "HEAVYSNIPER": return "Fusil de precision lourd";
            case "MG": return "Mitrailleuse";
            case "COMBATMG": return "Mitrailleuse de combat";
            default: return weapon;
        }
    }


    internal sealed class HorizontalRepeat
    {
        private int heldDirection;
        private int nextRepeatAt;
        public void Reset() { heldDirection = 0; nextRepeatAt = 0; }
        public int Poll(int intervalMs = 65, int initialDelayMs = 350)
        {
            bool left = Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, 174);
            bool right = Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, 175);
            int direction = left == right ? 0 : right ? 1 : -1;
            if (direction == 0) { Reset(); return 0; }
            int now = Game.GameTime;
            if (heldDirection != direction) { heldDirection = direction; nextRepeatAt = now + initialDelayMs; return direction; }
            if (now < nextRepeatAt) return 0;
            nextRepeatAt = now + intervalMs;
            return direction;
        }
    }
}
