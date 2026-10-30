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
            if (index >= LSL.playerWeapons.Count || index >= LSL.playerWeps.Count) return false;
            XElement weaponXml = LSL.playerWeapons[index];
            if (weaponXml == null) return false;
            if (!Enum.TryParse(LSL.playerWeps[index], out WeaponHash weapon) ||
                (int)weapon == -1569615261) return false;
            LSL.vehWeaponsXML = XDocument.Load(FilePath);
            var occupiedIds = LSL.vehWeaponsXML.Descendants("weapon")
                .Select(w => (string)w.Attribute("id"))
                .Where(idText => idText != null)
                .ToList();
            int id = 0;
            while (occupiedIds.Contains(id.ToString())) id++;
            weaponXml.SetAttributeValue("id", id);
            LSL.vehWeaponsXML.Element("weapons")?.Add(new XElement(weaponXml));
            LSL.vehWeaponsXML.Save(FilePath);
            Game.Player.Character.Weapons.Remove(weapon);
            string weaponName = LSL.playerWeps[index];
            LSL.playerWeapons.RemoveAt(index);
            LSL.playerWeps.RemoveAt(index);
            car.vehWeapons.Add(new XElement(weaponXml));
            car.vehWeps.Add(weaponName);
        }
        else
        {
            if (index >= car.vehWeapons.Count || index >= car.vehWeps.Count) return false;
            XElement selected = car.vehWeapons[index];
            XElement hash = selected.Element("HASH");
            if (hash == null || !Enum.TryParse(hash.Value, out WeaponHash weapon) ||
                (int)weapon == -1569615261) return false;
            LSL.vehWeaponsXML = XDocument.Load(FilePath);
            // Une meme arme peut exister plusieurs fois : retrouver son ID avant son hash.
            string selectedId = (string)selected.Attribute("id");
            XElement stored = LSL.vehWeaponsXML.Descendants("weapon")
                .FirstOrDefault(w => selectedId != null && (string)w.Attribute("id") == selectedId);
            if (stored == null)
                stored = LSL.vehWeaponsXML.Descendants("weapon")
                    .FirstOrDefault(w => (string)w.Element("HASH") == hash.Value);
            if (stored == null) return false;
            int ammo = 0;
            int.TryParse((string)stored.Element("AMMO"), out ammo);
            Game.Player.Character.Weapons.Give(weapon, ammo, true, false);
            Game.Player.Character.Weapons.Select(weapon, true);
            foreach (XElement component in stored.Elements("COMPONENT"))
            {
                if (Enum.TryParse(component.Value, out WeaponComponentHash parsed))
                    Game.Player.Character.Weapons.Current.Components[parsed].Active = true;
            }
            LSL.playerWeapons.Add(new XElement(stored));
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
