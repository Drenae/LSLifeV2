using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Native;

namespace LSlife;

internal static class WorkerWeaponTransfer
{
	private const string DealersPath = "scripts\\LSLife\\LSLife_Dealers.xml";
	private static readonly WeaponHash Unarmed = unchecked((WeaponHash)(-1569615261));

	public static void GiveWeaponToWorker(int index)
	{
		PlayerDealer worker = LSL.DealerHandler?.currentDealer;
		Ped recipient = worker?.Ped;
		Ped player = Game.Player.Character;
		if (!LsFunctions.IsLivePed(recipient) || !LsFunctions.IsLivePed(player) ||
			index < 0 || index >= LSL.playerWeps.Count ||
			!Enum.TryParse((string)LSL.playerWeps[index], out WeaponHash incoming) || incoming == Unarmed)
			return;

		WeaponHash outgoing = recipient.Weapons.Current.Hash;
		if (outgoing == Unarmed)
			outgoing = recipient.Weapons.BestWeapon.Hash;

		bool exchange = outgoing != Unarmed;
		if (!CanTransfer(player, recipient, incoming, outgoing, exchange))
		{
			UI.ShowSubtitle("Échange impossible : vérifie les armes déjà possédées.", 3500);
			return;
		}

		if (!LsFunctions.HandOverGoods(player, recipient) || LSL.DealerHandler.currentDealer != worker ||
			worker.Ped != recipient || Game.Player.Character != player ||
			!CanTransfer(player, recipient, incoming, outgoing, exchange))
			return;

		XElement incomingData = LsFunctions.PedWeapons(player, preserveAmmo: true)
			.FirstOrDefault(weapon => HasHash(weapon, incoming));
		XElement outgoingData = exchange
			? LsFunctions.PedWeapons(recipient, preserveAmmo: true).FirstOrDefault(weapon => HasHash(weapon, outgoing))
			: null;
		if (incomingData == null || (exchange && outgoingData == null)) return;

		XDocument document = XmlSaveFile.Load(DealersPath);
		XElement dealer = document.Descendants("dealer")
			.FirstOrDefault(element => (string)element.Element("id") == worker.ID.ToString());
		if (dealer == null)
		{
			UI.ShowSubtitle("Échange impossible : employé introuvable dans la sauvegarde.", 3500);
			return;
		}

		dealer.Elements("weapon")
			.Where(weapon => HasHash(weapon, incoming) || (exchange && HasHash(weapon, outgoing))).Remove();
		incomingData.SetAttributeValue("exactAmmo", true);
		dealer.Add(incomingData);

		// Verify both weapons can be granted before removing either original.
		WeaponHash selectedPlayerWeapon = player.Weapons.Current.Hash;
		WeaponHash selectedWorkerWeapon = recipient.Weapons.Current.Hash;
		bool committed = false;
		try
		{
			recipient.Weapons.Give(incoming, 0, false, true);
			if (exchange) player.Weapons.Give(outgoing, 0, false, true);
			if (!recipient.Weapons.HasWeapon(incoming) || (exchange && !player.Weapons.HasWeapon(outgoing)))
				return;

			XmlSaveFile.Save(document, DealersPath);
			committed = true;
		}
		catch (IOException error)
		{
			LsFunctions.DbugString(error.Message);
			UI.ShowSubtitle("Échange annulé : sauvegarde de l’employé impossible.", 3500);
			return;
		}
		finally
		{
			if (!committed)
			{
				recipient.Weapons.Remove(incoming);
				recipient.Weapons.Select(selectedWorkerWeapon);
				if (exchange) player.Weapons.Remove(outgoing);
				player.Weapons.Select(selectedPlayerWeapon);
			}
		}

		player.Weapons.Remove(incoming);
		if (exchange)
		{
			recipient.Weapons.Remove(outgoing);
			LsFunctions.ApplyExactWeaponData(player, outgoing, outgoingData);
		}
		LsFunctions.ApplyExactWeaponData(recipient, incoming, incomingData);
		worker.dWeapons = dealer.Elements("weapon").Select(weapon => new XElement(weapon)).ToList();

		// A swap can keep the same weapon count, so the tick's count check is insufficient.
		var weapons = LsFunctions.PedWeapons(player, preserveAmmo: true);
		LSL.playerWeapons.Clear();
		LSL.playerWeps.Clear();
		foreach (XElement weapon in weapons)
		{
			weapon.SetAttributeValue("id", LSL.playerWeapons.Count);
			LSL.playerWeapons.Add(weapon);
			LSL.playerWeps.Add((string)weapon.Element("HASH"));
		}
		UI.ShowSubtitle(exchange ? "Armes échangées." : "Arme donnée à l’employé.", 2500);
	}

	private static bool CanTransfer(Ped player, Ped worker, WeaponHash incoming, WeaponHash outgoing, bool exchange) =>
		LsFunctions.IsLivePed(player) && LsFunctions.IsLivePed(worker) &&
		player.Weapons.HasWeapon(incoming) && !worker.Weapons.HasWeapon(incoming) &&
		(!exchange || (worker.Weapons.HasWeapon(outgoing) && !player.Weapons.HasWeapon(outgoing)));

	private static bool HasHash(XElement weapon, WeaponHash expected) =>
		Enum.TryParse((string)weapon.Element("HASH"), out WeaponHash hash) && hash == expected;
}
