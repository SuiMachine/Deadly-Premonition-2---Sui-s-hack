using MelonLoader.Preferences;

namespace SuisHack
{
	public class PowerOfTwoValidatorWithRange : ValueValidator
	{
		public int MinValue { get; }
		public int MaxValue { get; }

		public PowerOfTwoValidatorWithRange(int minValue, int maxValue)
		{
			if (minValue >= maxValue)
				throw new System.ArgumentException($"Min value ({minValue}) must be less than max value ({maxValue})!");

			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}

		public override bool IsValid(object value)
		{
			int number = (int)value;
			double log = Math.Log(number, 2d);
			double pow = Math.Pow(2, Math.Round(log));
			return pow == number;
		}

		public override object EnsureValid(object value)
		{
			int number = (int)value;
			if (number > MaxValue)
				return MaxValue;
			if (number < MinValue)
				return MinValue;
			return number;
		}
	}
}
