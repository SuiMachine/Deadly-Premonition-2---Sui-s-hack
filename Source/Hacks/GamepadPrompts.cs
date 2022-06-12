using HarmonyLib;
using Il2CppSystem.IO;
using SuisHack.GlobalGameObjects;
using System;
using UnhollowerBaseLib;
using UnityEngine;

namespace SuisHack.Hacks
{
	[HarmonyPatch]
	public static class GamepadPromptsFix
	{
		private static bool Initiated;
		private static bool Error;
		public const string ObjName_KeyA = "Hud_KeyGuide_A";
		public const string ObjName_KeyB = "Hud_KeyGuide_A";
		public const string ObjName_KeyX = "Hud_KeyGuide_A";
		public const string ObjName_KeyY = "Hud_KeyGuide_A";


		private class Replacements
		{
			public Texture2D Switch_A_Button = new Texture2D(128, 128);
			public Texture2D Switch_B_Button = new Texture2D(128, 128);
			public Texture2D Switch_X_Button = new Texture2D(128, 128);
			public Texture2D Switch_Y_Button = new Texture2D(128, 128);
		}

		private static Replacements m_Replacements;



		[HarmonyPostfix]
		[HarmonyPatch(typeof(UIKeyGuideItem), "Disp")]
		public static void DisplayPostifx(UIKeyGuideItem __instance)
		{

			if(!Initiated)
			{
				switch (SuisHackMain.Settings.prompts)
				{
					case ControllerPrompts.Xbox:
						InitiatePromptsXbox();
						break;
					case ControllerPrompts.PlayStation:
						InitiatePromptsPlaystation();
						break;
				}

				var sprites = __instance.m_buttonIcon.mAtlas.mSprites;
				for(int i=0; i<sprites.Count; i++)
				{
					var sprite = sprites[i];
				}
				Initiated = true;
			}
			if (Error)
				return;
/*
			if(SuisHackMain.Settings.prompts != ControllerPrompts.Switch)
			{
				if(__instance.name == ObjName_KeyA)
					 = m_Replacements.Switch_A_Button;
				else if (__instance.name == ObjName_KeyB)
					__instance.m_buttonIcon.mainTexture = m_Replacements.Switch_A_Button;
				else if (__instance.name == ObjName_KeyX)
					__instance.m_buttonIcon.mainTexture = m_Replacements.Switch_A_Button;
				else if (__instance.name == ObjName_KeyY)
					__instance.m_buttonIcon.mainTexture = m_Replacements.Switch_A_Button;
			}*/
		}

		private static void InitiatePromptsXbox()
		{
			try
			{
				SuisHackMain.loggerInst.Msg($"Loading replacements for Xbox controller!");
				var loadPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "Prompts"), "Xbox").Replace("\\", "/");
				m_Replacements = LoadReplacements(loadPath);
				SuisHackMain.loggerInst.Msg($"Successfully loaded replacements!");
			}
			catch (Exception e)
			{
				SuisHackMain.loggerInst.Error("Failed to initialize prompts: " + e);
				Error = true;
			}
		}

		private static void InitiatePromptsPlaystation()
		{
			try
			{
				var loadPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "Prompts"), "PlayStation").Replace("\\", "/");

			}
			catch (Exception e)
			{
				SuisHackMain.loggerInst.Error("Failed to initialize prompts: " + e);
				Error = true;
			}
		}

		private static Replacements LoadReplacements(string loadPath)
		{
			var pathA = Path.Combine(loadPath, "A.png").Replace("\\", "/");
			var pathB = Path.Combine(loadPath, "B.png").Replace("\\", "/");
			var pathX = Path.Combine(loadPath, "X.png").Replace("\\", "/");
			var pathY = Path.Combine(loadPath, "Y.png").Replace("\\", "/");
			if(!File.Exists(pathA) || !File.Exists(pathB) || !File.Exists(pathX) || !File.Exists(pathY))
			{
				throw new Exception("Require files don't exist!");
			}

			SuisHackMain.loggerInst.Msg($"Loading from {pathA})");
			var replacements = new Replacements();
			ImageConversion.LoadImage(replacements.Switch_A_Button, File.ReadAllBytes(pathA));
			if(replacements.Switch_A_Button == null)
			{
				SuisHackMain.loggerInst.Msg($"Failed to load  from {pathA})");
			}

			return replacements;
		}
	}
}
