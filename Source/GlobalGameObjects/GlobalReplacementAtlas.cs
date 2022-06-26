using Il2CppSystem.IO;
using System;
using UnityEngine;

namespace SuisHack.GlobalGameObjects
{
	[MelonLoader.RegisterTypeInIl2Cpp]
	public class GlobalReplacementAtlas : MonoBehaviour
	{
		public enum ReplacementType
		{
			Basic,
			KeyboardAndMouse
		}

		public static GlobalReplacementAtlas Instance { get; private set; }
		private UIAtlas Atlas;
		public GlobalReplacementAtlas(IntPtr ptr) : base(ptr) { }
		public IPromptCache Cache { get; private set; }
		public ReplacementType ReplacementTypeUsed { get; private set; }

		public static void Initialize()
		{
			if (Instance == null)
			{
				var GlobalAtlasGO = new GameObject("GlobalAtlas");
				DontDestroyOnLoad(GlobalAtlasGO);
				Instance = GlobalAtlasGO.AddComponent<GlobalReplacementAtlas>();
			}
		}

		void Awake()
		{
			if (SuisHackMain.Settings.Entry_Other_Prompts.Value != "" || SuisHackMain.Settings.Input_Override.Value != ExposedSettings.InputType.Original)
			{
				var assetBundlePath = SuisHackMain.Settings.Entry_Other_Prompts.Value;
				this.ReplacementTypeUsed = ReplacementType.Basic;
				if (SuisHackMain.Settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
				{
					this.ReplacementTypeUsed = ReplacementType.KeyboardAndMouse;
					assetBundlePath = "keyboard";
				}

				string path = Path.Combine(Path.Combine(Application.streamingAssetsPath, "prompts"), assetBundlePath);
				SuisHackMain.loggerInst.Msg("Trying to load replacement prompts using bundle: " + path);

				var assetBundle = AssetBundle.LoadFromFile(path);
				if (assetBundle == null)
				{
					SuisHackMain.loggerInst.Error("Failed to load asset bundle!");
					return;
				}

				//Load game object
				var assets = assetBundle.LoadAsset("prompts_go");
				if (assets != null)
				{
					var cast = assets.TryCast<GameObject>();
					if (cast != null)
					{
						if (cast.GetComponent<UIAtlas>() != null)
						{
							var instanitate = GameObject.Instantiate<GameObject>(cast);
							instanitate.transform.SetParent(this.transform);
							instanitate.transform.localPosition = Vector3.zero;
							instanitate.transform.localRotation = Quaternion.identity;
							Atlas = instanitate.GetComponentInChildren<UIAtlas>();

							switch (ReplacementTypeUsed)
							{
								case ReplacementType.KeyboardAndMouse:
									Cache = new PromptCacheKeyboard(Atlas);
									break;
								default:
									Cache = new PromptCacheBasic(Atlas);
									break;
							}
						}
						else
						{
							SuisHackMain.loggerInst.Error("Atlas Game object was found, but UIAtlas was null. Crap!");
							return;
						}
					}
					else
					{
						SuisHackMain.loggerInst.Error("Atlas game object cast failed - object might be invalid.");
						return;
					}
				}
				else
					SuisHackMain.loggerInst.Error("Failed to find Xbox atlas... fuck!");

				if (Atlas != null)
					SuisHackMain.loggerInst.Msg("Replacement atlas loaded correctly!!");
			}
		}

		public void Replace(UISprite instance, Enum keyEnum)
		{
			var replacement = Cache.GetReplacement(keyEnum);
			if (replacement != null)
			{
				instance.atlas = Atlas;
				instance.SetAtlasSprite(replacement);
			}
		}
	}
}