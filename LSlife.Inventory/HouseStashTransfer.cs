using System;
using System.Xml.Linq;
using GTA;
using GTA.Native;

namespace LSlife;

// Transferts vers une planque independants des callbacks NativeUI.
internal static class HouseStashTransfer
{
    internal static void Deposit(int kind, int requested)
    {
        var house = LSL.HouseHandler.CurrentHouse;
        if (house == null || requested < 0) return;
        int amount;
        switch (kind)
        {
            case 0:
                amount = Math.Min(requested, LSL.PlayerInventory["Weed"]);
                house.Weed += amount; LSL.PlayerInventory["Weed"] -= amount;
                break;
            case 1:
                amount = Math.Min(requested, LSL.PlayerInventory["Crack"]);
                house.Crack += amount; LSL.PlayerInventory["Crack"] -= amount;
                break;
            case 2:
                amount = Math.Min(requested, LSL.PlayerInventory["Cocaine"]);
                house.Cocain += amount; LSL.PlayerInventory["Cocaine"] -= amount;
                break;
            case 3:
                amount = Math.Min(requested, LsFunctions.pWeedOunce);
                house.Weed += amount * 28; LsFunctions.pWeedOunce -= amount;
                break;
            case 4:
                amount = Math.Min(requested, LsFunctions.pCrackOunce);
                house.Crack += amount * 28; LsFunctions.pCrackOunce -= amount;
                break;
            case 5:
                amount = Math.Min(requested, LsFunctions.pCocaineOunce);
                house.Cocain += amount * 28; LsFunctions.pCocaineOunce -= amount;
                break;
            case 6:
                amount = Math.Min(requested, Game.Player.Money);
                house.Money += amount; Game.Player.Money -= amount;
                break;
            case 8:
                if (requested >= LSL.playerWeapons.Count || requested >= LSL.playerWeps.Count) return;
                XElement weaponXml = LSL.playerWeapons[requested];
                if (weaponXml == null) return;
                if (!Enum.TryParse(LSL.playerWeps[requested], out WeaponHash weapon) ||
                    (int)weapon == -1569615261) return;
                int id = 0;
                foreach (XElement existing in house.Weapons)
                {
                    if (existing.Name != "weapon") continue;
                    id++;
                    if (existing.FirstAttribute != null && existing.FirstAttribute.Value != id.ToString()) break;
                }
                weaponXml.SetAttributeValue("id", id);
                house.Weapons.Add(weaponXml);
                house.Weps.Add(LSL.playerWeps[requested]);
                Game.Player.Character.Weapons.Remove(weapon);
                LSL.playerWeapons.RemoveAt(requested);
                LSL.playerWeps.RemoveAt(requested);
                break;
            default: return;
        }
        LsFunctions.UpdateInventory();
        LSL.HouseHandler.UpdateMenu(house);
        LSL.HouseHandler.SaveHouse(house);
    }

    internal static void Withdraw(int kind, int requested)
    {
        var house = LSL.HouseHandler.CurrentHouse;
        if (house == null || requested < 0) return;
        int amount;
        switch (kind)
        {
            case 0:
                amount = Math.Min(requested, house.Weed);
                house.Weed -= amount; LSL.PlayerInventory["Weed"] += amount;
                break;
            case 1:
                amount = Math.Min(requested, house.Crack);
                house.Crack -= amount; LSL.PlayerInventory["Crack"] += amount;
                break;
            case 2:
                amount = Math.Min(requested, house.Cocain);
                house.Cocain -= amount; LSL.PlayerInventory["Cocaine"] += amount;
                break;
            case 6:
                amount = Math.Min(requested, house.Money);
                house.Money -= amount; Game.Player.Money += amount;
                break;
            case 8:
                if (requested >= house.Weapons.Count || requested >= house.Weps.Count) return;
                XElement w = house.Weapons[requested];
                XElement hash = w.Element("HASH");
                if (hash == null || !Enum.TryParse(hash.Value, out WeaponHash weapon)) return;
                int ammo = 0;
                int.TryParse((string)w.Element("AMMO"), out ammo);
                Game.Player.Character.Weapons.Give(weapon, ammo, true, false);
                LSL.playerWeapons.Add(new XElement(w));
                LSL.playerWeps.Add(hash.Value);
                house.Weapons.RemoveAt(requested);
                house.Weps.RemoveAt(requested);
                break;
            default: return;
        }
        LsFunctions.UpdateInventory();
        LSL.HouseHandler.SaveHouse(house);
    }
}
