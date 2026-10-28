namespace LSlife.HeatBars;

// Calcul de la pression du quartier ; l'affichage est centralisé dans Hud.cs.
internal static class AreaHeatBar
{
	// Niveau de reputation 1 atteint a 625 points selon CurrentRepLvl.
	// La jauge represente la progression vers le prochain niveau.
	public static float GetReputationPercentage()
	{
		if (LSL.areas == null || LSL.areaIndex < 0 || LSL.areaIndex >= LSL.areas.Count)
			return 0f;
		float reputation = System.Math.Max(0f, LSL.areas[LSL.areaIndex].Reputation);
		double level = LsFunctions.CurrentRepLvl(reputation);
		int currentLevel = (int)level;
		double lower = System.Math.Pow(currentLevel / 0.04d, 2d);
		double upper = System.Math.Pow((currentLevel + 1) / 0.04d, 2d);
		return upper > lower ? (float)((reputation - lower) / (upper - lower)) : 0f;
	}

	public static float GetPercentage()
	{
		if (LSL.areas == null || LSL.areaIndex < 0 || LSL.areaIndex >= LSL.areas.Count)
			return 0f;
		return System.Math.Max(0f, System.Math.Min(1f, LSL.areas[LSL.areaIndex].Heat * 0.001f));
	}
}
