using System.Collections.Generic;

namespace LSlife;

// Donnees de jeu conservees independamment de toute bibliotheque de menus.
public partial class LSL
{
	public static bool deliveriesEnabled = true;
	public static List<dynamic> meleeWeps = new List<object> { "Bat", "Knife", "Dagger" };
	public static List<int> meleePrice = new List<int> { 5, 100, 2000 };
	public static List<dynamic> pistolWeps = new List<object> { "Pistol", "Pistol50", "HeavyPistol", "Ammo" };
	public static List<int> pistolPrice = new List<int> { 350, 3750, 4000, 50 };
	public static List<dynamic> shotgunWeps = new List<object> { "SawnOffShotgun", "PumpShotgun" };
	public static List<dynamic> assaultRifleWeps = new List<object> { "AssaultRifle", "CarbineRifle" };
	public static List<dynamic> smgWeps = new List<object> { "MicroSMG", "SMG" };
	public static List<dynamic> mgWeps = new List<object> { "MG" };
	public static List<dynamic> sniperWeps = new List<object> { "SniperRifle" };
	public static List<dynamic> heavyWeps = new List<object> { "RPG" };
	public static List<dynamic> thrownWeps = new List<object> { "Grenade" };
	public static readonly List<dynamic> multiplier = new List<object> { 1, 10, 28, 100, 280, 1000, 2800, 10000, 100000 };
}
