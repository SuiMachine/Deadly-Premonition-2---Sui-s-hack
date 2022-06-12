using HarmonyLib;

namespace SuisHack.Hacks.ConfigOptionManagerHack
{
	[HarmonyPatch(typeof(ConfigOptionManager), "ApplySetting")]
	public static class ConfigOptionsManagerHookApply
	{
		public static void Postfix()
		{
			SuisHack.SuisHackMain.loggerInst.Msg("Applying!");
		}
	}

	[HarmonyPatch(typeof(ConfigOptionManager), "Init")]
	public static class ConfigOptionsManagerHookInit
	{
		public static void Postfix()
		{
			SuisHack.SuisHackMain.loggerInst.Msg("YYYY!");
		}
	}
}
