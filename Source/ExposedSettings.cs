using UnityEngine;

namespace SuisHack
{
	public enum ControllerPrompts
	{
		Switch,
		Xbox,
		PlayStation
	}

	public class ExposedSettings
	{
		public (int X, int Y)? Resolution;
		public int RefreshRate;
		public FullScreenMode DisplayMode = FullScreenMode.FullScreenWindow;
		public float LOD_Bias = 2f;
		public int PixelLightCount = 4;
		public int TextureQuality = 0;
		public bool RealtimeReflectionProbes = true;
		public int vSyncCount = 1;

		public bool SkipIntros;
		public ControllerPrompts prompts;
	}
}
