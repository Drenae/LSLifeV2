using System;
using System.Xml.Linq;
using GTA;

namespace LSlife;

// Transferts de produits et d'argent dans le coffre, sans elements NativeUI.
internal static class VehicleStashTransfer
{
    internal static bool Transfer(bool deposit, int kind, decimal requested)
    {
        var car = LSL.pStashVehicle;
        if (car == null || kind < 0 || kind > 6 || requested <= 0) return false;

        decimal playerAmount;
        decimal storedAmount;
        switch (kind)
        {
            case 0: playerAmount = LSL.PlayerInventory["Weed"]; storedAmount = car.drugCarWeed; break;
            case 1: playerAmount = LSL.PlayerInventory["Crack"]; storedAmount = car.drugCarCrack; break;
            case 2: playerAmount = LSL.PlayerInventory["Cocaine"]; storedAmount = car.drugCarCocain; break;
            case 3: playerAmount = LsFunctions.playerBulkWeedGrams; storedAmount = car.carBulkWeedGrams; break;
            case 4: playerAmount = LsFunctions.playerBulkCrackGrams; storedAmount = car.carBulkCrackGrams; break;
            case 5: playerAmount = LsFunctions.playerBulkCocaineGrams; storedAmount = car.carBulkCocaineGrams; break;
            default: playerAmount = Game.Player.Money; storedAmount = car.drugCarMoney; break;
        }
        decimal amount = Math.Min(requested, Math.Max(0, deposit ? playerAmount : storedAmount));
        if (amount <= 0) return false;
        decimal nextPlayer = deposit ? playerAmount - amount : playerAmount + amount;
        decimal nextStored = deposit ? storedAmount + amount : storedAmount - amount;

        switch (kind)
        {
            case 0: LSL.PlayerInventory["Weed"] = (int)nextPlayer; car.drugCarWeed = (int)nextStored; break;
            case 1: LSL.PlayerInventory["Crack"] = (int)nextPlayer; car.drugCarCrack = (int)nextStored; break;
            case 2: LSL.PlayerInventory["Cocaine"] = (int)nextPlayer; car.drugCarCocain = (int)nextStored; break;
            case 3: LsFunctions.playerBulkWeedGrams = nextPlayer; car.carBulkWeedGrams = nextStored; break;
            case 4: LsFunctions.playerBulkCrackGrams = nextPlayer; car.carBulkCrackGrams = nextStored; break;
            case 5: LsFunctions.playerBulkCocaineGrams = nextPlayer; car.carBulkCocaineGrams = nextStored; break;
            default: Game.Player.Money = (int)nextPlayer; car.drugCarMoney = (int)nextStored; break;
        }

        string[] inventoryKeys = { "weed", "crack", "cocain", "weedOz", "crackOz", "cocaineOz" };
        string[] vehicleKeys = { "weed", "crack", "cocain", "weedOz", "crackOz", "cocainOz", "money" };
        const string path = "scripts\\LSLife\\LSLifeSave.xml";
        LSL.LSLifeSave = XmlSaveFile.Load(path);
        foreach (XElement element in LSL.LSLifeSave.Descendants())
        {
            if (kind < 6 && element.Name == "playerInventory")
            {
                XElement field = element.Element(inventoryKeys[kind]);
                if (kind >= 3 && kind <= 5)
                {
                    if (field == null)
                    {
                        field = new XElement(inventoryKeys[kind]);
                        element.Add(field);
                    }
                    LsFunctions.WriteStockGrams(field, nextPlayer);
                }
                else if (field != null) field.Value = ((int)nextPlayer).ToString();
            }
            else if (element.Name == "drugCarStash")
            {
                XElement field = element.Element(vehicleKeys[kind]);
                if (kind >= 3 && kind <= 5)
                {
                    if (field == null)
                    {
                        field = new XElement(vehicleKeys[kind]);
                        element.Add(field);
                    }
                    LsFunctions.WriteStockGrams(field, nextStored);
                }
                else if (field != null) field.Value = ((int)nextStored).ToString();
            }
        }
        XmlSaveFile.Save(LSL.LSLifeSave, path);
        car.UpdateCarStash();
        return true;
    }
}
