using MelonLoader;
using System.Text.RegularExpressions;

namespace SuisHack.Hacks
{
	public static class ConfigParsing
	{
		public static bool ParseResolution(string text, out LemonTuple<int, int> desiredResolution)
		{
			desiredResolution = null;
			while (text.IndexOf(' ') >= 0)
			{
				var index = text.IndexOf(' ');
				text = text.Remove(index, 1);
			}

			var regex = new Regex(@"(\d+)x(\d+)");
			var match = regex.Match(text);
			if (match.Success)
			{
				var resX = match.Groups[1].Value;
				var resY = match.Groups[2].Value;
				if (uint.TryParse(resX, out uint desiredResolutionX))
				{
					uint desiredResolutionY;
					if (uint.TryParse(resY, out desiredResolutionY))
					{
						if (desiredResolutionX == 0 || desiredResolutionY == 0)
							return false;
						else
						{
							desiredResolution = new LemonTuple<int, int>
							{
								Item1 = (int)desiredResolutionX,
								Item2 = (int)desiredResolutionY
							};
							return true;
						}
					}
					else
						return false;
				}
				else
					return false;
			}
			else
				return false;
		}
	}
}
