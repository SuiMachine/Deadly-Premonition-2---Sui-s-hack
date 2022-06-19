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


		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			base.OnSceneWasLoaded(buildIndex, sceneName);
			if (Settings == null)
				return;

			if (Settings.Entry_Other_FixGeometry.Value)
			{
				var objects = UnityEngine.Resources.FindObjectsOfTypeAll<ElectricPoleModel>();

				for (int i = 0; i < objects.Length; i++)
				{
					var obj = objects[i];
					if (obj.GetComponent<Components.WireRendererCorrection>() == null)
						obj.gameObject.AddComponent<Components.WireRendererCorrection>();
				}
			}
		}

		public void ApplySettings()
		{
			Settings = new ExposedSettings();
		}
	}
}
