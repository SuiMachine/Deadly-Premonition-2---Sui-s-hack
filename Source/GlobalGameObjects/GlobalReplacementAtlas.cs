using Il2CppSystem.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			if(Instance == null)
			{
				var GlobalAtlasGO = new GameObject("GlobalAtlas");
				DontDestroyOnLoad(GlobalAtlasGO);
				Instance = GlobalAtlasGO.AddComponent<GlobalReplacementAtlas>();
			}
		}

		void Awake()
		{
			var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "xboxprompts"));
			if(assetBundle == null)
			{
				SuisHackMain.loggerInst.Msg("Fuck!");
				return;
			}

			var assets = assetBundle.LoadAsset("ReplacementAtlasXbox");
			if (assets != null)
			{
				var cast = assets.TryCast<GameObject>();
				if (cast != null)
					Atlas = cast.GetComponent<UIAtlas>();
			}
			else
				SuisHackMain.loggerInst.Msg("Fuck!");

			if(Atlas != null)
				SuisHackMain.loggerInst.Msg("Woohoo!!");

			assetBundle.Unload(false);

		}
	}
}
