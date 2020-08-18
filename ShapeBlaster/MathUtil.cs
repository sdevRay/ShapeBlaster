using Microsoft.Xna.Framework;
using System;

namespace ShapeBlaster
{
	static class MathUtil
	{
		// creates a Vector2 from an angle and magnitude.
		public static Vector2 FromPolar(float angle, float magnitude)
		{
			return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
		}
	}
}
