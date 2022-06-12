using System.Text.RegularExpressions;

namespace SuisHack.Hacks
{
	public static class ConfigParsing
	{
		public static bool ParseResolution(string text, out (uint X, uint Y) desiredResolution)
		{
			desiredResolution = (0, 0);
			while(text.IndexOf(' ') >= 0)
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
				if (uint.TryParse(resX, out desiredResolution.X))
				{
					if (uint.TryParse(resY, out desiredResolution.Y))
					{
						if (desiredResolution.X == 0 || desiredResolution.Y == 0)
							return false;
						else
							return true;
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
