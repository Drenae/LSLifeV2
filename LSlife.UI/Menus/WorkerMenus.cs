using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LSlife.OldCustomers;
using LSlife.Shots;
using NativeUI;


namespace LSlife;

internal static partial class LsMenus
{
	public static void WArmour_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (LSL.playerArmours <= 0)
		{
			return;
		}
		LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
		PlayerDealer currentDealer = LSL.DealerHandler.currentDealer;
		if (currentDealer != null)
		{
			Ped ped = currentDealer.Ped;
			if (((ped != null) ? new int?(ped.Armor) : ((int?)null)) < 200)
			{
				LSL.DealerHandler.currentDealer.Ped.Armor = 200;
				LSL.DealerHandler.currentDealer.dArmour = LSL.DealerHandler.currentDealer.Ped.Armor;
				LSL.playerArmours--;
				if (LSL.playerArmours == 0)
				{
					selectedItem.Text = "Plus de gilet disponible";
				}
				LsFunctions.PlayTheSound();
				LsFunctions.UpdateInventory();
				LsFunctions.SetWorkerMenuHealth(LSL.DealerHandler.currentDealer.Ped);
				return;
			}
		}
		UI.ShowSubtitle("J'ai déjà assez de protection.", 4000);
	}

		public static void WWeapons_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		GiveWeaponToWorker(LSL.wWeapons.Index);
	}

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
			UI.Notify("Transfert d'arme");
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
								if (!(e3.Name == "COMPONENT") || !Enum.TryParse<WeaponComponent>(e3.Value, out WeaponComponent wepcomp))
								{
									continue;
								}
								PlayerDealer currentDealer3 = LSL.DealerHandler.currentDealer;
								if (currentDealer3 != null)
								{
									Ped ped2 = currentDealer3.Ped;
									if (ped2 != null)
									{
										ped2.Weapons.Current.SetComponent(wepcomp, true);
									}
								}
							}
							Game.Player.Character.Weapons.Remove(wep);
							LSL.playerWeapons.Remove(w);
							LSL.playerWeps.RemoveAt(index);
							if (LSL.DEBUG)
							{
								UI.Notify("Enregistrement de l'arme du dealer : " + ((object)wep/*cast due to constrained. prefix*/).ToString() + " munitions : " + ammo);
							}
							break;
						}
						return;
					}
				}
				if (LSL.DEBUG)
				{
					UI.Notify("Aucune correspondance dans playerWeapons");
				}
			}
			else if (LSL.DEBUG)
			{
				UI.Notify("Identifiant d'arme introuvable : " + LSL.playerWeps[index]);
			}
		}
		else if (LSL.DEBUG)
		{
			UI.Notify("Aucune arme trouvée");
		}
	}

	internal static void WOrder_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		if (LSL.DealerHandler.currentDealer != null)
		{
			Enum.TryParse<Enums.PdStates>(LSL.wOrder.Items[LSL.wOrder.Index].ToString(), out LSL.DealerHandler.currentDealer.NewState);
			LSL.wOrder.Items = LSL.DealerHandler.currentDealer.Orders();
			LSL.wOrder.Index = 0;
		}
	}

	public static void WConfirm_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int i = 0;
		using (List<PlayerDealer>.Enumerator enumerator = LSL.DealerHandler.dealers.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != LSL.DealerHandler.currentDealer)
			{
				i++;
			}
		}
		bool change = false;
		if (LSL.wWeed.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Weed"] += LSL.wWeed.Value;
			LSL.PlayerInventory["Weed"] -= LSL.wWeed.Value;
			LSL.wWeed.Value = 0;
			LSL.wWeed.Text = "Cannabis " + LsFunctions.GramsToOz(LSL.wWeed.Value);
			change = true;
		}
		if (LSL.wCrack.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Crack"] += LSL.wCrack.Value;
			LSL.PlayerInventory["Crack"] -= LSL.wCrack.Value;
			LSL.wCrack.Value = 0;
			LSL.wCrack.Text = "Crack " + LsFunctions.GramsToOz(LSL.wCrack.Value);
			change = true;
		}
		if (LSL.wCocaine.Value > 0)
		{
			LSL.DealerHandler.dealers[i].Drugs["Cocaine"] += LSL.wCocaine.Value;
			LSL.PlayerInventory["Cocaine"] -= LSL.wCocaine.Value;
			LSL.wCocaine.Value = 0;
			LSL.wCocaine.Text = "Cocaïne " + LsFunctions.GramsToOz(LSL.wCocaine.Value);
			change = true;
		}
		if (change)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			LsFunctions.UpdateInventory();
			LSL.DealerHandler.dealers[i].SetDrugDesc();
			LSL.DealerHandler.needsSave = true;
		}
	}

	public static void WCollect_Activated(UIMenu sender, UIMenuItem selectedItem)
	{
		int i = 0;
		using (List<PlayerDealer>.Enumerator enumerator = LSL.DealerHandler.dealers.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current != LSL.DealerHandler.currentDealer)
			{
				i++;
			}
		}
		if (LSL.DealerHandler.dealers[i].Money >= 100)
		{
			LsFunctions.HandOverGoods(LSL.player.Character, LSL.DealerHandler.currentDealer.Ped);
			LSL.DealerHandler.dealers[i].GiveMoneyToPlayer();
			LSL.DealerHandler.dealers[i].SetDrugDesc();
			LSL.WorkerMainMenu.CurrentSelection = 3;
		}
	}

	public static void WCocaine_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.wCocaine.Text = "Cocaïne " + LsFunctions.GramsToOz(sender.Value);
	}

	public static void WCrack_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.wCrack.Text = "Crack " + LsFunctions.GramsToOz(sender.Value);
	}

	public static void WWeed_OnSliderChanged(UIMenuSliderItem sender, int newIndex)
	{
		LSL.wWeed.Text = "Cannabis " + LsFunctions.GramsToOz(sender.Value);
	}
}
