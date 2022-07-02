using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.GlobalGameObjects
{
	public interface IPromptCache
	{
		UISpriteData GetReplacement(Enum keyEnum);
	}

	public enum GamepadKeyIcons
	{
		X,
		Y,
		A,
		A2,
		B
	}

	public abstract class PromptCache<T> : IPromptCache where T : Enum
	{
		Dictionary<T, UISpriteData> translation = new Dictionary<T, UISpriteData>();

		public PromptCache(UIAtlas atlas)
		{
			translation.Clear();
			var enums = Enum.GetValues(typeof(T));
			foreach (var en in enums)
			{
				var enumToAdd = (T)en;
				var result = GetSpriteDataFromAtlas(atlas, enumToAdd.ToString());
				if (result != null)
				{
					translation.Add(enumToAdd, result);
				}
			}

			SuisHackMain.loggerInst.Msg("Atlas cache created!");
		}

		UISpriteData GetSpriteDataFromAtlas(UIAtlas atlas, string enumName)
		{
			for (int i = 0; i < atlas.spriteList.Count; i++)
			{
				if (atlas.spriteList[i].name == enumName)
				{
					return atlas.spriteList[i];
				}
			}
			return null;
		}

		public UISpriteData GetReplacement(Enum keyEnum)
		{
			if (translation.TryGetValue((T)keyEnum, out UISpriteData result))
			{
				return result;
			}
			return null;
		}
	}

	public class PromptCacheBasic : PromptCache<GamepadKeyIcons>
	{
		public PromptCacheBasic(UIAtlas atlas) : base(atlas) { }
	}

	public class PromptCacheKeyboard : PromptCache<KeyCode>
	{
		public PromptCacheKeyboard(UIAtlas atlas) : base(atlas) { }
	}
}
