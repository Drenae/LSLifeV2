using System;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Native;

namespace LSlife;

// Logique de transfert d'armes partagee, sans types ni evenements NativeUI.
internal static class WorkerWeaponTransfer
{
	public static void GiveWeaponToWorker(int index)
	{
		if (LSL.DealerHandler == null || LSL.DealerHandler.currentDealer == null ||
			LSL.DealerHandler.currentDealer.Ped == null || index < 0 ||
			index >= LSL.playerWeps.Count || index >= LSL.playerWeapons.Count) return;
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Invalid comparison between Unknown and I4
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		if (LSL.DEBUG)
		{
			GTA.UI.Notification.Show("Transfert d'arme");
		}
		if (LSL.playerWeps.Count > 0)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			WeaponHash wep = default;
			if (Enum.TryParse(LSL.playerWeps[index], out wep))
			{
				if ((int)wep == -1569615261)
				{
					return;
				}
				XElement w = LSL.playerWeapons[index];
				if (w != null)
				{
					XDocument dealersDoc = XDocument.Load("scripts\\LSLife\\LSLife_Dealers.xml");
					if (dealersDoc.Descendants().Count() <= 0)
					{
						return;
					}
					{
						foreach (XElement element in dealersDoc.Descendants())
						{
							if (!(element.Name == "dealer"))
							{
								continue;
							}
							bool found = false;
							foreach (XElement e in element.Elements())
							{
								if (e.Name == "id" && e.Value == LSL.DealerHandler.currentDealer.ID.ToString())
								{
									found = true;
									break;
								}
							}
							if (!found)
							{
								continue;
							}
							bool hasWeapon = false;
							XElement dWeapon = null;
							foreach (XElement e2 in element.Descendants())
							{
								if (e2.Name == "HASH" && e2.Value == ((object)wep/*cast due to constrained. prefix*/).ToString())
								{
									hasWeapon = true;
									dWeapon = e2.Parent;
									break;
								}
							}
							if (hasWeapon)
							{
								dWeapon.ReplaceWith(w);
							}
							else
							{
								element.Add(w);
							}
							dealersDoc.Save("scripts\\LSLife\\LSLife_Dealers.xml");
							int ammo = 1000;
							foreach (XElement e3 in w.Descendants())
							{
								if (e3.Name == "AMMO")
								{
									int.TryParse(e3.Value, out ammo);
									PlayerDealer currentDealer = LSL.DealerHandler.currentDealer;
									if (currentDealer != null)
									{
										currentDealer.Ped.Weapons.Give(wep, ammo, true, true);
									}
									PlayerDealer currentDealer2 = LSL.DealerHandler.currentDealer;
									if (currentDealer2 != null)
									{
										Ped ped = currentDealer2.Ped;
										if (ped != null)
										{
											ped.Weapons.Select(wep);
										}
									}
								}
								if (!(e3.Name == "COMPONENT") || !Enum.TryParse<WeaponComponentHash>(e3.Value, out WeaponComponentHash wepcomp))
								{
									continue;
								}
								PlayerDealer currentDealer3 = LSL.DealerHandler.currentDealer;
								if (currentDealer3 != null)
								{
									Ped ped2 = currentDealer3.Ped;
									if (ped2 != null)
									{
										ped2.Weapons.Current.Components[wepcomp].Active = true;
									}
								}
							}
							Game.Player.Character.Weapons.Remove(wep);
							LSL.playerWeapons.Remove(w);
							LSL.playerWeps.RemoveAt(index);
							if (LSL.DEBUG)
							{
								GTA.UI.Notification.Show("Enregistrement de l'arme du dealer : " + ((object)wep/*cast due to constrained. prefix*/).ToString() + " munitions : " + ammo);
							}
							break;
						}
						return;
					}
				}
				if (LSL.DEBUG)
				{
					GTA.UI.Notification.Show("Aucune correspondance dans playerWeapons");
				}
			}
			else if (LSL.DEBUG)
			{
				GTA.UI.Notification.Show("Identifiant d'arme introuvable : " + LSL.playerWeps[index]);
			}
		}
		else if (LSL.DEBUG)
		{
			GTA.UI.Notification.Show("Aucune arme trouvée");
		}
	}
}
