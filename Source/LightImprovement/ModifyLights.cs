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
						for (int i = 0; i < lights.Length; i++)
						{
							lights[i].shadows = LightShadows.Soft;
							if(lights[i].gameObject.name.ToLower().StartsWith("fan"))
							{
								lights[i].shadowNearPlane = 0.5f;
							}
						}
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
	}
}
