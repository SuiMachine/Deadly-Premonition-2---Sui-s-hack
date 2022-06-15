using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;

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

#if DEBUG && FALSE
		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			LoggerInstance.Msg("Dumping all mono behaviours!");
			var monoBehaviours = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<MonoBehaviour>());
			foreach(var obj in monoBehaviours)
			{
				LoggerInstance.Msg($"{obj.name}");
			}
			base.OnSceneWasInitialized(buildIndex, sceneName);
		}
#endif

		public void ApplySettings()
		{
			Settings = new ExposedSettings();
			Hacks.PostProcessLayerHook.Antialiasing = Settings.Entry_Antialiasing.Value;
		}
	}
}
