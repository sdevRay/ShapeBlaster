using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
	class Bullet : Entity
	{
		public Bullet(Vector2 position, Vector2 velocity)
		{
			image = Art.Bullet;
			Position = position;
			Velocity = velocity;
			Orientation = Velocity.ToAngle();
			Radius = 8;
		}

		public override void Update()
		{
			// Using LengthSquared() is a small performance optimization; computing the square of the length is a bit faster than computing the length itself because it avoids the relatively slow square root operation. You'll see code using the squares of lengths or distances throughout the program. In this particular case, the performance difference is negligible, but this optimization can make a difference when used in tight loops.
			if (Velocity.LengthSquared() > 0)
			{
				Orientation = Velocity.ToAngle();
			}

			Position += Velocity;

			// delete bullets that go off-screen
			if (!GameRoot.Viewport.Bounds.Contains(Position.ToPoint()))
				IsExpired = true;
		}
	}
}
