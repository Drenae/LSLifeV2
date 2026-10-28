using System;
using System.Xml.Linq;
using GTA;

namespace LSlife;

// Transfert de gilets entre l'inventaire et le coffre, sans reference NativeUI.
internal static class VehicleArmourTransfer
{
    internal static bool Transfer(bool deposit, int requested)
    {
        var car = LSL.pStashVehicle;
        if (car == null || Game.Player == null || Game.Player.Character == null || requested <= 0)
            return false;

        int carried = Math.Max(0, LSL.playerArmours);
        int stored = Math.Max(0, car.drugCarArmour);
        if (deposit)
        {
            if (requested > carried) return false;
            LSL.playerArmours = carried - requested;
            car.drugCarArmour = stored + requested;
        }
        else
        {
            if (requested > stored) return false;
            int maxCarried = Game.Player.Character.Armor < 100 ? 6 : 5;
            if (carried + requested > maxCarried) return false;
            int toWear = Game.Player.Character.Armor < 100 ? 1 : 0;
            if (toWear != 0) Game.Player.Character.Armor = 100;
            LSL.playerArmours = carried + requested - toWear;
            car.drugCarArmour = stored - requested;
        }

        // Mettre a jour les deux sections XML sans reconstruire les sauvegardes.
        if (LSL.LSLifeSave != null)
        {
            foreach (XElement section in LSL.LSLifeSave.Descendants())
            {
                if (section.Name != "drugCarStash" && section.Name != "playerInventory") continue;
                XElement armour = section.Element("armour");
                if (armour == null) continue;
                armour.Value = (section.Name == "drugCarStash"
                    ? car.drugCarArmour : LSL.playerArmours).ToString();
            }
            LSL.LSLifeSave.Save("scripts\\LSLife\\LSLifeSave.xml");
        }
        return true;
    }
}
