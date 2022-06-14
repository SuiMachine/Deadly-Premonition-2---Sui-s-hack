using Il2CppSystem.IO;
using System;
using UnityEngine;

namespace SuisHack.GlobalGameObjects
{
	[MelonLoader.RegisterTypeInIl2Cpp]
	public class GlobalReplacementAtlas : MonoBehaviour
	{
		public static GlobalReplacementAtlas Instance { get; private set; }
		public UIAtlas Atlas { get; private set; }
		public GlobalReplacementAtlas(IntPtr ptr) : base(ptr) { }

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
			if (SuisHackMain.Settings.Entry_Other_Prompts.Value != "")
			{
				string path = Path.Combine(Path.Combine(Application.streamingAssetsPath, "Prompts"), SuisHackMain.Settings.Entry_Other_Prompts.Value);
				SuisHackMain.loggerInst.Msg("Trying to load replacement prompts using bundle: " + path);

				var assetBundle = AssetBundle.LoadFromFile(path);
				if (assetBundle == null)
				{
					SuisHackMain.loggerInst.Msg("Failed to load asset bundle!");
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
						}
						else
						{
							SuisHackMain.loggerInst.Msg("Atlas Game object was found, but UIAtlas was null. Crap!");
							return;
						}
					}
					else
					{
						SuisHackMain.loggerInst.Msg("Atlas game object cast failed - object might be invalid.");
						return;
					}
				}
				else
					SuisHackMain.loggerInst.Msg("Failed to find Xbox atlas... fuck!");

				if (Atlas != null)
					SuisHackMain.loggerInst.Msg("Replacement atlas loaded correctly!!");
			}

		}
	}
}
