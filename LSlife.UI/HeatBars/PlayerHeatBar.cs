namespace LSlife.HeatBars;

// Calcul de la pression du joueur ; l'affichage est centralisé dans Hud.cs.
internal static class PlayerHeatBar
{
	public static float GetPercentage()
	{
		return LSL.playerHeat > 0f ? LSL.playerHeat * 0.01f : 0f;
	}
}
