using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LSlife;

internal static class Program
{
    private static int assertions;
    private static void Check(bool success, string message)
    {
        if (!success) throw new Exception(message);
        assertions++;
    }

    private static void Main()
    {
        string directory = Path.Combine(Path.GetTempPath(), "LSLife-GangTests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "gang.xml");
            var store = new PlayerGangStore(path);
            store.Load();
            Check(!store.IsBlocked && store.Current == null && !File.Exists(path), "Absence = aucun gang, aucune ecriture");
            Check(!store.Create("~r~Injection", "gold") && !File.Exists(path), "Refuser les codes GTA");
            Check(!store.Create(" ", "gold") && !store.Create("--", "gold"), "Refuser les noms vides");
            Check(!store.Create(new string('A', 25), "gold"), "Refuser nom trop long");
            Check(!store.Create("Paleto", "unknown"), "Refuser couleur inconnue");
            Check(store.Create("  Les Éperviers  ", "blue"), "Creation");
            Guid id = store.Current.Id;
            DateTime created = store.Current.CreatedUtc;
            Check(id != Guid.Empty && store.Current.Name == "Les Éperviers" && store.Current.LeaderId == "player", "Identite et chef");
            Check(store.Current.DealerIds.Count == 0 && store.Current.TerritoryIds.Count == 0, "Pas de rattachement anticipe");
            Check(!store.Create("Deuxieme", "red") && store.Current.Id == id, "Pas de double creation");
            var reloaded = new PlayerGangStore(path);
            reloaded.Load();
            Check(!reloaded.IsBlocked && reloaded.Current.Id == id && reloaded.Current.CreatedUtc == created, "Rechargement stable");
            Check(reloaded.UpdateIdentity("Paleto Crew", "purple"), "Modification");
            Check(reloaded.Current.Id == id && reloaded.Current.CreatedUtc == created && File.Exists(path + ".bak"), "ID stable et backup");
            Check(PlayerGangStore.Parse(XDocument.Load(path + ".bak")).Name == "Les Éperviers", "Backup contient l'etat precedent");

            var document = XDocument.Load(path);
            document.Root.Add(new XElement("futureExtension", "preserver"));
            document.Root.Element("members").Add(new XElement("dealer", new XElement("id", 17)));
            document.Root.Element("territories").Add(new XElement("territory", new XElement("zone", "PALETO")));
            document.Save(path);
            string externallyChanged = File.ReadAllText(path);
            Check(!reloaded.UpdateIdentity("Conflit", "red") && File.ReadAllText(path) == externallyChanged && reloaded.Current.Name == "Paleto Crew", "Conflit : aucun ecrasement ni mutation memoire");
            reloaded.Load();
            Check(reloaded.UpdateIdentity("Paleto", "green"), "Edition apres relecture");
            document = XDocument.Load(path);
            Check(document.Root.Element("futureExtension").Value == "preserver" && reloaded.Current.DealerIds.Single() == 17 && reloaded.Current.TerritoryIds.Single() == "PALETO", "Preserver extensions et references");

            string valid = File.ReadAllText(path);
            foreach (string invalid in new[] {
                "<broken", valid.Replace("version=\"1\"", "version=\"999\""),
                valid.Replace("<leaderId>player</leaderId>", "<leaderId>rival</leaderId>"),
                valid.Replace("<id>17</id>", "<id>-1</id>"),
                valid.Replace("<color>green</color>", "<color>bad</color>"),
                valid.Replace("<name>Paleto</name>", "<name>Paleto</name><name>Double</name>"),
                valid.Replace("<id>17</id>", "<id>99999999999999999999</id>"),
                valid.Replace("<dealer>", "<dealer><id>17</id>") })
            {
                File.WriteAllText(path, invalid);
                reloaded.Load();
                Check(reloaded.IsBlocked && reloaded.Current == null, "XML invalide bloque sans planter");
                Check(!reloaded.Create("Ecrasement", "gold") && File.ReadAllText(path) == invalid, "XML invalide intact");
            }
            File.WriteAllText(path, valid);
            reloaded.Load();
            Check(!reloaded.IsBlocked && reloaded.Current.Id == id, "Recuperation manuelle puis relecture");
            File.Delete(path);
            reloaded.Load();
            Check(reloaded.IsBlocked && !reloaded.Create("Ecrasement", "gold") && !File.Exists(path), "Backup orphelin protege");
            var failing = new PlayerGangStore(Path.Combine(directory, "absent", "gang.xml"));
            Check(!failing.Create("Paleto", "gold") && failing.Current == null, "Echec d'ecriture sans publication memoire");
            Console.WriteLine("PASS : " + assertions + " assertions de persistance (sans GTA).");
        }
        finally { Directory.Delete(directory, true); }
    }
}
