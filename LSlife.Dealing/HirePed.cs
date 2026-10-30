using GTA;
using LSlife.OldCustomers;

namespace LSlife;

public class HirePed
{
	private Ped PotentialWorker;

	private int TimeOfOffer = Game.GameTime;

	private bool Declined;

	private bool Accepted;

	private bool TimedOut;

	public bool Destroy { get; private set; }

	public HirePed(Ped _ped)
	{
		PotentialWorker = _ped;
		OldCustomerHandler.RemoveIfPedOldCustomer(_ped);
		if (LSL.entitiesCleanUp.Contains((Entity)(object)_ped))
		{
			LSL.entitiesCleanUp.Remove((Entity)(object)_ped);
		}
		PotentialWorker.Task.ClearAll();
		PotentialWorker.BlockPermanentEvents = true;
		((Entity)PotentialWorker).IsPersistent = true;
		PotentialWorker.Task.Pause(10000);
	}

	public void OnTick()
	{
		if (!Accepted)
		{
			if ((float)Game.GameTime > LSL.displayHelpTimer + 2000f)
			{
				LSL.displayHelpTimer = Game.GameTime;
				LsFunctions.DisplayHelpText("Recruter cet employé : ~INPUT_CONTEXT~ pour accepter ou ~INPUT_CONTEXT_SECONDARY~ pour refuser.");
			}
			if (Game.GameTime > TimeOfOffer + 10000)
			{
				TimedOut = true;
			}
			else if (GTA.Input.Controls.IsDisabledControlJustPressed(GTA.Input.ControlType.Frontend, (Control)52))
			{
				Declined = true;
			}
			else if (GTA.Input.Controls.IsDisabledControlJustPressed(GTA.Input.ControlType.Frontend, (Control)51))
			{
				Accepted = true;
				GTA.UI.Screen.ShowSubtitle("Super, je te suis.", 6000);
			}
			if (Declined || TimedOut)
			{
				Destroy = true;
				LsFunctions.RemovePedFromWorld(PotentialWorker, clearTask: true);
				GTA.UI.Screen.ShowSubtitle("OK, peut-être une prochaine fois.", 6000);
			}
		}
		else
		{
			LSL.DealerHandler.AddDealer(new PlayerDealer(PotentialWorker));
			Destroy = true;
		}
	}
}
