using System;
using System.Xml.Linq;
using GTA;

namespace LSlife;

// Transferts de produits et d'argent dans le coffre, sans elements NativeUI.
internal static class VehicleStashTransfer
{
    internal static bool Transfer(bool deposit, int kind, int requested)
    {
        var car = LSL.pStashVehicle;
        if (car == null || kind < 0 || kind > 6 || requested <= 0) return false;

        int playerAmount;
        int storedAmount;
        switch (kind)
        {
            case 0: playerAmount = LSL.PlayerInventory["Weed"]; storedAmount = car.drugCarWeed; break;
            case 1: playerAmount = LSL.PlayerInventory["Crack"]; storedAmount = car.drugCarCrack; break;
            case 2: playerAmount = LSL.PlayerInventory["Cocaine"]; storedAmount = car.drugCarCocain; break;
            case 3: playerAmount = LsFunctions.pWeedOunce; storedAmount = car.drugCarWeedOz; break;
            case 4: playerAmount = LsFunctions.pCrackOunce; storedAmount = car.drugCarCrackOz; break;
            case 5: playerAmount = LsFunctions.pCocaineOunce; storedAmount = car.drugCarCocainOz; break;
            default: playerAmount = Game.Player.Money; storedAmount = car.drugCarMoney; break;
        }
        int amount = Math.Min(requested, Math.Max(0, deposit ? playerAmount : storedAmount));
        if (amount <= 0) return false;
        int nextPlayer = deposit ? playerAmount - amount : playerAmount + amount;
        int nextStored = deposit ? storedAmount + amount : storedAmount - amount;

        switch (kind)
        {
            case 0: LSL.PlayerInventory["Weed"] = nextPlayer; car.drugCarWeed = nextStored; break;
            case 1: LSL.PlayerInventory["Crack"] = nextPlayer; car.drugCarCrack = nextStored; break;
            case 2: LSL.PlayerInventory["Cocaine"] = nextPlayer; car.drugCarCocain = nextStored; break;
            case 3: LsFunctions.pWeedOunce = nextPlayer; car.drugCarWeedOz = nextStored; break;
            case 4: LsFunctions.pCrackOunce = nextPlayer; car.drugCarCrackOz = nextStored; break;
            case 5: LsFunctions.pCocaineOunce = nextPlayer; car.drugCarCocainOz = nextStored; break;
            default: Game.Player.Money = nextPlayer; car.drugCarMoney = nextStored; break;
        }

        string[] inventoryKeys = { "weed", "crack", "cocain", "weedOz", "crackOz", "cocaineOz" };
        string[] vehicleKeys = { "weed", "crack", "cocain", "weedOz", "crackOz", "cocainOz", "money" };
        const string path = "scripts\\LSLife\\LSLifeSave.xml";
        LSL.LSLifeSave = XDocument.Load(path);
        foreach (XElement element in LSL.LSLifeSave.Descendants())
        {
            if (kind < 6 && element.Name == "playerInventory")
            {
                XElement field = element.Element(inventoryKeys[kind]);
                if (field != null) field.Value = nextPlayer.ToString();
            }
            else if (element.Name == "drugCarStash")
            {
                XElement field = element.Element(vehicleKeys[kind]);
                if (field != null) field.Value = nextStored.ToString();
            }
        }
        LSL.LSLifeSave.Save(path);
        car.UpdateCarStash();
        return true;
    }
}
