using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LSlife;

public class AreaDealer
{
	private bool DEBUG = false;

	public Ped dPed;

	public Vehicle Vehicle;

	public Enums.DStates CurrentState;

	public Enums.DStates NewState = Enums.DStates.Normal;

	public Enums.DealState dealState;

	public bool Detected;

	private Vector3 servePos = Vector3.Zero;

	public LsTimer cTimer = new LsTimer(5, timeOut: true, start: false);

	public Ped cPed;

	private int time;

	public string Area;

	public int LastCustomer { get; private set; }

	public int LastOffer { get; private set; }

	private Blip recruitmentDebugBlip;

	// Repere de test : actif uniquement lorsque DEBUG est active.
	public void UpdateRecruitmentDebugMarker(bool enabled)
	{
		bool valid = (Entity)(object)dPed != (Entity)null && ((Entity)dPed).Exists() && ((Entity)dPed).IsAlive;
		if (!enabled || !valid || CurrentState == Enums.DStates.AttackPlayer)
		{
			if (recruitmentDebugBlip != null && recruitmentDebugBlip.Exists()) recruitmentDebugBlip.Delete();
			recruitmentDebugBlip = null;
			return;
		}
		// Reutiliser le blip du dealer lorsqu'il existe deja, au lieu d'abandonner le suivi.
		Blip existing = ((Entity)dPed).AttachedBlip;
		if (recruitmentDebugBlip != null && recruitmentDebugBlip.Exists()) return;
		if (existing != null && existing.Exists())
		{
			recruitmentDebugBlip = existing;
		}
		else
		{
			recruitmentDebugBlip = ((Entity)dPed).AddBlip();
		}
		if (recruitmentDebugBlip == null || !recruitmentDebugBlip.Exists())
		{
			LSLifeLog.Warning("Dealer", "Blip DEBUG introuvable pour ped=" + dPed.Handle + ", quartier=" + Area);
			return;
		}
		recruitmentDebugBlip.Name = "Dealer recrutable (DEBUG)";
		recruitmentDebugBlip.Color = (BlipColor)2;
		recruitmentDebugBlip.IsShortRange = false;
		recruitmentDebugBlip.IsFriendly = false;
	}
