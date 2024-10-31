using System;
using System.IO;
using UnityEngine;

namespace SuisHack.GlobalGameObjects
{
	public class GlobalReplacementAtlas : MonoBehaviour
	{
		public GlobalReplacementAtlas(IntPtr ptr) : base(ptr) { }

		public enum ReplacementType
		{
			Basic,
			KeyboardAndMouse
		}

		public static GlobalReplacementAtlas Instance { get; private set; }
		private UIAtlas Atlas;
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
			this.hideFlags = HideFlags.HideAndDontSave;
			var settings = ExposedSettings.Instance;
			if (settings.Entry_Other_Prompts.Value != "" || settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
			{
				var assetBundlePath = settings.Entry_Other_Prompts.Value;
				this.ReplacementTypeUsed = ReplacementType.Basic;
				if (settings.Input_Override.Value == ExposedSettings.InputType.KeyboardAndMouse)
				{
					this.ReplacementTypeUsed = ReplacementType.KeyboardAndMouse;
					assetBundlePath = "keyboard";
				}

				string path = Path.Combine(Path.Combine(Application.streamingAssetsPath, "prompts"), assetBundlePath);
				Plugin.Message("Trying to load replacement prompts using bundle: " + path);

				var assetBundle = AssetBundle.LoadFromFile(path);
				if (assetBundle == null)
				{
					Plugin.Error("Failed to load asset bundle!");
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
							Plugin.Error("Atlas Game object was found, but UIAtlas was null. Crap!");
							return;
						}
					}
					else
					{
						Plugin.Error("Atlas game object cast failed - object might be invalid.");
						return;
					}
				}
				else
					Plugin.Error("Failed to find Xbox atlas... fuck!");

				if (Atlas != null)
					Plugin.Message("Replacement atlas loaded correctly!!");
			}
		}

		public void Replace(UISprite instance, Enum keyEnum)
		{
			if (Cache == null)
				return;

			var replacement = Cache.GetReplacement(keyEnum);

			if (replacement != null)
			{
				instance.atlas = Atlas;
				instance.SetAtlasSprite(replacement!);
			}
		}
	}
}