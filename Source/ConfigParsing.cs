using System;
using System.Text.RegularExpressions;

namespace SuisHack
{
	public static class ConfigParsing
	{
		public static bool ParseResolution(string text, out Tuple<int, int> desiredResolution)
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
					if (uint.TryParse(resY, out uint desiredResolutionY))
					{
						if (desiredResolutionX == 0 || desiredResolutionY == 0)
							return false;
						else
						{
							desiredResolution = new Tuple<int, int>((int)desiredResolutionX, (int)desiredResolutionY);
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
