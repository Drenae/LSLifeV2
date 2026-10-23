namespace LSlife.HeatBars;

// Calcul de la pression du quartier ; l'affichage est centralisé dans Hud.cs.
internal static class AreaHeatBar
{
	public static float GetPercentage()
	{
		return LSL.areas[LSL.areaIndex].Heat * 0.001f;
	}
}
