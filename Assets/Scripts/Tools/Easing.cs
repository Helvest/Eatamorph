using UnityEngine;

/// <summary>
/// Class for easings calculus
/// </summary>
public static class Easing
{
	private const float PI = Mathf.PI;
	private const float HalfPI = Mathf.PI / 2;
	private const float DoublePI = Mathf.PI * 2;
	private const float HalfOne = .5f;

	/// <summary>
	/// Calculate the Ease from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing float</returns>
	public static float Ease(float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		switch (part)
		{
			default:
			case EasingPart.NoEase:
				return linearStep;
			case EasingPart.EaseIn:
				return EaseIn(linearStep, type);
			case EasingPart.EaseOut:
				return EaseOut(linearStep, type);
			case EasingPart.EaseInOut:
				return EaseInOut(linearStep, type);
			case EasingPart.EaseOutIn:
				return EaseOutIn(linearStep, type);
		}
	}

	/// <summary>
	/// Calculate the Ease position between two Vector2
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing Vector2</returns>
	public static Vector2 EaseVector2(Vector2 from, Vector2 to, float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		return Vector2.LerpUnclamped(from, to, Ease(linearStep, part, type));
	}

	/// <summary>
	/// Calculate the Ease position between two Vector3
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing Vector3</returns>
	public static Vector3 EaseVector3(Vector3 from, Vector3 to, float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		return Vector3.LerpUnclamped(from, to, Ease(linearStep, part, type));
	}

	/// <summary>
	/// Calculate the Ease position between two Vector4
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing Vector4</returns>
	public static Vector4 EaseVector4(Vector4 from, Vector4 to, float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		return Vector4.LerpUnclamped(from, to, Ease(linearStep, part, type));
	}

	/// <summary>
	/// Calculate the Ease rotation between two Quaternion
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing Quaternion</returns>
	public static Quaternion EaseQuaternion(Quaternion from, Quaternion to, float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		return Quaternion.LerpUnclamped(from, to, Ease(linearStep, part, type));
	}

	/// <summary>
	/// Calculate the Ease position between two Color32
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing Color32</returns>
	public static Color32 EaseColor32(Color32 from, Color32 to, float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		return Color32.LerpUnclamped(from, to, Ease(linearStep, part, type));
	}

	/// <summary>
	/// Calculate the Ease position between two Colors
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="part">Easing Part</param>
	/// <param name="type">Easing Type</param>
	/// <returns>A easing Color</returns>
	public static Color EaseColor(Color from, Color to, float linearStep, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		return Color.LerpUnclamped(from, to, Ease(linearStep, part, type));
	}

	/// <summary>
	/// Calculate a Ease In from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="type">Easing Type</param>
	public static float EaseIn(float linearStep, EasingType type)
	{
		switch (type)
		{
			case EasingType.Step:
				return Mathf.Round(linearStep);
			default:
			case EasingType.Linear:
				return linearStep;
			case EasingType.Sine:
				return Sine.EaseIn(linearStep);
			case EasingType.Quadratic:
				return Power.EaseIn(linearStep, 2);
			case EasingType.Cubic:
				return Power.EaseIn(linearStep, 3);
			case EasingType.Quartic:
				return Power.EaseIn(linearStep, 4);
			case EasingType.Quintic:
				return Power.EaseIn(linearStep, 5);
			case EasingType.Elastic:
				return Elastic.EaseIn(linearStep);
			case EasingType.Bounce:
				return Bounce.EaseIn(linearStep);
			case EasingType.Back:
				return Back.EaseIn(linearStep);
			case EasingType.Expo:
				return Expo.EaseIn(linearStep);
			case EasingType.Circ:
				return Circ.EaseIn(linearStep);
		}
	}

	/// <summary>
	/// Calculate a Ease Out from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="type">Easing Type</param>
	public static float EaseOut(float linearStep, EasingType type)
	{
		switch (type)
		{
			case EasingType.Step:
				return Mathf.Round(linearStep);
			default:
			case EasingType.Linear:
				return linearStep;
			case EasingType.Sine:
				return Sine.EaseOut(linearStep);
			case EasingType.Quadratic:
				return Power.EaseOut(linearStep, 2);
			case EasingType.Cubic:
				return Power.EaseOut(linearStep, 3);
			case EasingType.Quartic:
				return Power.EaseOut(linearStep, 4);
			case EasingType.Quintic:
				return Power.EaseOut(linearStep, 5);
			case EasingType.Elastic:
				return Elastic.EaseOut(linearStep);
			case EasingType.Bounce:
				return Bounce.EaseOut(linearStep);
			case EasingType.Back:
				return Back.EaseOut(linearStep);
			case EasingType.Expo:
				return Expo.EaseOut(linearStep);
			case EasingType.Circ:
				return Circ.EaseOut(linearStep);
		}

	}

	/// <summary>
	/// Calculate a Ease InOut from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="easeInType">Easing Type for the In</param>
	/// <param name="easeOutType">Easing Type for the Out</param>
	public static float EaseInOut(float linearStep, EasingType easeInType, EasingType easeOutType)
	{
		return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
	}

	/// <summary>
	/// Calculate a Ease InOut from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="type">Easing Type</param>
	public static float EaseInOut(float linearStep, EasingType type)
	{
		switch (type)
		{
			case EasingType.Step:
				return Mathf.Round(linearStep);
			default:
			case EasingType.Linear:
				return linearStep;
			case EasingType.Sine:
				return Sine.EaseInOut(linearStep);
			case EasingType.Quadratic:
				return Power.EaseInOut(linearStep, 2);
			case EasingType.Cubic:
				return Power.EaseInOut(linearStep, 3);
			case EasingType.Quartic:
				return Power.EaseInOut(linearStep, 4);
			case EasingType.Quintic:
				return Power.EaseInOut(linearStep, 5);
			case EasingType.Elastic:
				return Elastic.EaseInOut(linearStep);
			case EasingType.Bounce:
				return Bounce.EaseInOut(linearStep);
			case EasingType.Back:
				return Back.EaseInOut(linearStep);
			case EasingType.Expo:
				return Expo.EaseInOut(linearStep);
			case EasingType.Circ:
				return Circ.EaseInOut(linearStep);
		}
	}

	/// <summary>
	/// Calculate a Ease OutIn from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="easeInType">Easing Type for the In</param>
	/// <param name="easeOutType">Easing Type for the Out</param>
	public static float EaseOutIn(float linearStep, EasingType easeInType, EasingType easeOutType)
	{
		return linearStep < 0.5 ? EaseOutIn(linearStep, easeInType) : EaseOutIn(linearStep, easeOutType);
	}

	/// <summary>
	/// Calculate a Ease OutIn from a pourcent
	/// </summary>
	/// <param name="linearStep">Pourcent on the ease</param>
	/// <param name="type">Easing Type</param>
	public static float EaseOutIn(float linearStep, EasingType type)
	{
		switch (type)
		{
			case EasingType.Step:
				return Mathf.Round(linearStep);
			default:
			case EasingType.Linear:
				return linearStep;
			case EasingType.Sine:
				return Sine.EaseOutIn(linearStep);
			case EasingType.Quadratic:
				return Power.EaseOutIn(linearStep, 2);
			case EasingType.Cubic:
				return Power.EaseOutIn(linearStep, 3);
			case EasingType.Quartic:
				return Power.EaseOutIn(linearStep, 4);
			case EasingType.Quintic:
				return Power.EaseOutIn(linearStep, 5);
			case EasingType.Elastic:
				return Elastic.EaseOutIn(linearStep);
			case EasingType.Bounce:
				return Bounce.EaseOutIn(linearStep);
			case EasingType.Back:
				return Back.EaseOutIn(linearStep);
			case EasingType.Expo:
				return Expo.EaseOutIn(linearStep);
			case EasingType.Circ:
				return Circ.EaseOutIn(linearStep);
		}
	}

	private static class Sine
	{
		public static float EaseIn(float t)
		{
			return Mathf.Sin(t * HalfPI - HalfPI) + 1;
		}

		public static float EaseOut(float t)
		{
			return Mathf.Sin(t * HalfPI);
		}

		public static float EaseInOut(float t)
		{
			if (t == 0)
			{
				return 0;
			}

			if (t == HalfOne)
			{
				return HalfOne;
			}

			if (t == 1)
			{
				return 1;
			}

			return (Mathf.Sin(t * PI - HalfPI) + 1) / 2;
		}

		public static float EaseOutIn(float t)
		{
			if (t == 0)
			{
				return 0;
			}

			if (t == HalfOne)
			{
				return HalfOne;
			}

			if (t == 1)
			{
				return 1;
			}

			return t < HalfOne ?
				EaseOut(t * 2) * HalfOne :
				EaseIn((t * 2) - 1) * HalfOne + HalfOne;
		}
	}

	private static class Power
	{
		public static float EaseIn(float t, int power)
		{
			return Mathf.Pow(t, power);
		}

		public static float EaseOut(float t, int power)
		{
			float sign = power % 2 == 0 ? -1 : 1;
			return (sign * (Mathf.Pow(t - 1, power) + sign));
		}

		public static float EaseInOut(float t, int power)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseIn(t * 2, power) * HalfOne :
				EaseOut(t * 2 - 1, power) * HalfOne + HalfOne;
		}

		public static float EaseOutIn(float t, int power)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseOut(t * 2, power) * HalfOne :
				EaseIn(t * 2 - 1, power) * HalfOne + HalfOne;
		}
	}

	private static class Elastic
	{
		private const float p = DoublePI / .3f;
		private const float s = .3f / DoublePI * 1.57079633f;

		public static float EaseIn(float t)
		{
			if (t == 0 || t == 1)
			{
				return t;
			}

			return -(Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t - s) * p));
		}

		public static float EaseOut(float t)
		{
			if (t == 0 || t == 1)
			{
				return t;
			}

			return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * p) + 1;
		}

		public static float EaseInOut(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseIn(t * 2) * HalfOne :
				EaseOut(t * 2 - 1) * HalfOne + HalfOne;
		}

		public static float EaseOutIn(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseOut(t * 2) * HalfOne :
				EaseIn(t * 2 - 1) * HalfOne + HalfOne;
		}
	}

	private static class Bounce
	{
		private const float a = 7.5625f;
		private const float b = 2.75f;

		public static float EaseIn(float t)
		{
			return 1 - EaseOut(1 - t);
		}

		public static float EaseOut(float t)
		{
			if ((t /= 1) < 1 / b)
			{
				return (a * t * t);
			}
			else if (t < 2 / b)
			{
				return (a * (t -= (1.5f / b)) * t + .75f);
			}
			else if (t < 2.5 / b)
			{
				return (a * (t -= 2.25f / b) * t + .9375f);
			}

			return (a * (t -= (2.625f / b)) * t + .984375f);
		}

		public static float EaseInOut(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				(1 - EaseOut(1 - t * 2)) * HalfOne :
				EaseOut(t * 2 - 1) * HalfOne + HalfOne;
		}

		public static float EaseOutIn(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseOut(t * 2) * HalfOne :
				(1 - EaseOut(t * 2 - 2)) * HalfOne + HalfOne;
		}
	}

	private static class Back
	{
		private const float s = 1.70158f;
		private const float v = s * 1.525f;

		public static float EaseIn(float t)
		{
			return (t /= 1) * t * ((s + 1) * t - s);
		}

		public static float EaseOut(float t)
		{
			return ((t = t / 1 - 1) * t * ((s + 1) * t + s) + 1);
		}

		public static float EaseInOut(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseIn(t * 2) * HalfOne :
				EaseOut(t * 2 - 1) * HalfOne + HalfOne;
		}

		public static float EaseOutIn(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseOut(t * 2) * HalfOne :
				EaseIn(t * 2 - 1) * HalfOne + HalfOne;
		}
	}

	private static class Expo
	{
		public static float EaseIn(float t)
		{
			if (t == 0 || t == 1)
			{
				return t;
			}

			return Mathf.Pow(2, 10 * (t / 1 - 1));
		}

		public static float EaseOut(float t)
		{
			if (t == 0 || t == 1)
			{
				return t;
			}

			return 1 - Mathf.Pow(2, -10 * t / 1);
		}

		public static float EaseInOut(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				Mathf.Pow(2, 10 * (t * 2 / 1 - 1)) * HalfOne :
				(1 - Mathf.Pow(2, -10 * (t * 2 - 1) / 1)) * HalfOne + HalfOne;
		}

		public static float EaseOutIn(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				(1 - Mathf.Pow(2, -10 * t * 2 / 1)) * HalfOne :
				Mathf.Pow(2, 10 * ((t * 2 - 1) / 1 - 1)) * HalfOne + HalfOne;
		}
	}

	private static class Circ
	{
		public static float EaseIn(float t)
		{
			return -(Mathf.Sqrt(1 - (t /= 1) * t) - 1);
		}

		public static float EaseOut(float t)
		{
			return Mathf.Sqrt(1 - (t = t / 1 - 1) * t);
		}

		public static float EaseInOut(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseIn(t * 2) * HalfOne :
				EaseOut(t * 2 - 1) * HalfOne + HalfOne;
		}

		public static float EaseOutIn(float t)
		{
			if (t == 0 || t == HalfOne || t == 1)
			{
				return t;
			}

			return t < HalfOne ?
				EaseOut(t * 2) * HalfOne :
				EaseIn(t * 2 - 1) * HalfOne + HalfOne;
		}
	}
}

/// <summary>
/// Liste of possible part with ease
/// </summary>
public enum EasingPart
{
	NoEase,
	EaseIn,
	EaseOut,
	EaseInOut,
	EaseOutIn
}

/// <summary>
/// Liste of possible type of ease
/// </summary>
public enum EasingType
{
	Step,
	Linear,
	Sine,
	Quadratic,
	Cubic,
	Quartic,
	Quintic,
	Elastic,
	Bounce,
	Back,
	Expo,
	Circ
}
