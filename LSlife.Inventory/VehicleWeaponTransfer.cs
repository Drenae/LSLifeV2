using System;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Native;

namespace LSlife;

// Stockage des armes dans le coffre sans appeler les evenements NativeUI.
internal static class VehicleWeaponTransfer
{
    private const string FilePath = "scripts\\LSLife\\LsLife_Vehicle_Weapons.xml";

    internal static bool Transfer(bool deposit, int index)
    {
        var car = LSL.pStashVehicle;
        if (car == null || index < 0) return false;
        if (deposit)
        {
            if (index >= LSL.playerWeapons.Count || index >= LSL.playerWeps.Count ||
                index >= LSL.putWeaponVeh.Items.Count) return false;
            XElement weaponXml = LSL.playerWeapons[index];
            if (weaponXml == null) return false;
            if (!Enum.TryParse(LSL.playerWeps[index], out WeaponHash weapon) ||
                (int)weapon == -1569615261) return false;
            LSL.vehWeaponsXML = XDocument.Load(FilePath);
            int id = 0;
            foreach (XElement existing in LSL.vehWeaponsXML.Descendants("weapon"))
            {
                id++;
                if (existing.FirstAttribute != null && existing.FirstAttribute.Value != id.ToString()) break;
            }
            weaponXml.SetAttributeValue("id", id);
            LSL.vehWeaponsXML.Element("weapons")?.Add(weaponXml);
            LSL.vehWeaponsXML.Save(FilePath);
            Game.Player.Character.Weapons.Remove(weapon);
            LSL.playerWeapons.RemoveAt(index);
            LSL.playerWeps.RemoveAt(index);
            car.vehWeapons.Add(weaponXml);
            car.vehWeps.Add(LSL.putWeaponVeh.Items[index]);
        }
        else
        {
            if (index >= car.vehWeapons.Count || index >= car.vehWeps.Count) return false;
            XElement selected = car.vehWeapons[index];
            XElement hash = selected.Element("HASH");
            if (hash == null || !Enum.TryParse(hash.Value, out WeaponHash weapon) ||
                (int)weapon == -1569615261) return false;
            LSL.vehWeaponsXML = XDocument.Load(FilePath);
            XElement stored = LSL.vehWeaponsXML.Descendants("weapon")
                .FirstOrDefault(w => (string)w.Element("HASH") == hash.Value);
            if (stored == null) return false;
            int ammo = 0;
            int.TryParse((string)stored.Element("AMMO"), out ammo);
            Game.Player.Character.Weapons.Give(weapon, ammo, true, false);
            Game.Player.Character.Weapons.Select(weapon, true);
            foreach (XElement component in stored.Elements("COMPONENT"))
            {
                if (Enum.TryParse(component.Value, out WeaponComponent parsed))
                    Game.Player.Character.Weapons.Current.SetComponent(parsed, true);
            }
            LSL.playerWeapons.Add(stored.Element("HASH"));
            LSL.playerWeps.Add(hash.Value);
            car.vehWeapons.RemoveAt(index);
            car.vehWeps.RemoveAt(index);
            stored.Remove();
            LSL.vehWeaponsXML.Save(FilePath);
        }
        car.UpdateCarStash();
        return true;
    }
}
