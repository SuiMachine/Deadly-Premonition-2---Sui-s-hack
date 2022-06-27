using System;
using UnhollowerBaseLib;
using UnityEngine;

namespace SuisHack.LightImprovement
{
	static class ModifyLights
	{
		public static void ModifyOnSceneLoad(string sceneLowercase)
		{
			if (SuisHackMain.Settings.Entry_Other_LightImprovements.Value == ExposedSettings.LightImprovements.All)
			{
				var lights = GameObject.FindObjectsOfType<Light>();

				switch (sceneLowercase)
				{
					//bar - has to fan shadows
					case "map_003_1_alexus":
						ApplyImprovementsAlexus(lights);
						break;
					default:
						for (int i = 0; i < lights.Length; i++)
						{
							lights[i].shadows = LightShadows.Soft;

						}
						break;
				}
			}
		}

		private static void ApplyImprovementsAlexus(Il2CppArrayBase<Light> lights)
		{
			for (int i = 0; i < lights.Length; i++)
			{
				lights[i].shadows = LightShadows.Soft;
				if (lights[i].gameObject.name.ToLower().StartsWith("fan"))
				{
					lights[i].shadowNearPlane = 0.7f;
				}
			}

			var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
			for (int i = 0; i < meshRenderers.Length; i++)
			{
				var name = meshRenderers[i].gameObject.name.ToLower();
				if (name.StartsWith("chair") || name.StartsWith("table") || name.StartsWith("booth"))
				{
					meshRenderers[i].castShadows = true;
				}
			}
		}
	}
}
