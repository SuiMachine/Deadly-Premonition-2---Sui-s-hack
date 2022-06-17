using MelonLoader;

namespace SuisHack
{
	public class SuisHackMain : MelonMod
	{
		public static HarmonyLib.Harmony harmonyInst;
		public static MelonLogger.Instance loggerInst;
		public static ExposedSettings Settings;

		public override void OnApplicationLateStart()
		{
			LoggerInstance.Msg("Loading Sui's Hack loaded");
			base.OnApplicationLateStart();
			loggerInst = LoggerInstance;
			ApplySettings();
			SettingsGUI.Initialize();
			GlobalGameObjects.GlobalReplacementAtlas.Initialize();
			LoggerInstance.Msg("Sui's Hack loaded");
		}

		public void ApplySettings()
		{
			Settings = new ExposedSettings();
		}
	}
}
