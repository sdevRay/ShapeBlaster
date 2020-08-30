using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using System;
using Quaternion = Microsoft.Xna.Framework.Quaternion;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ShapeBlaster
{
	class PlayerShip : Entity
	{
		int framesUntilRespawn = 0;
		public bool IsDead { get { return framesUntilRespawn > 0; } }

		const int cooldownFrames = 6;
		int cooldownRemaining = 0;
		static Random rand = new Random();

		private static PlayerShip instance;
		public static PlayerShip Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new PlayerShip();
				}

				return instance;
			}
		}

		private PlayerShip()
		{
			image = Art.Player;
			Position = GameRoot.ScreenSize / 2;
			Radius = 10;
		}

		public void Kill()
		{
			framesUntilRespawn = 60;
			EnemySpawner.Reset();
			PlayerStatus.RemoveLife();
		}

		public override void Update()
		{
			if (IsDead)
			{
				if(PlayerStatus.Lives == 0)
				{
					PlayerStatus.Reset();
					Position = GameRoot.ScreenSize / 2;
				}

				framesUntilRespawn--;
				return;
			}

			// This will make the ship move at a speed up to eight pixels per frame, clamp its position so it can't go off-screen, and rotate the ship to face the direction it's moving.
			const float speed = 8;
			Velocity = speed * Input.GetMovementDirection();
			Position += Velocity;
			Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

			if(Velocity.LengthSquared() > 0)
			{
				// ToAngle() is a simple extension method defined in our Extensions class like so:
				Orientation = Velocity.ToAngle();
			}

			// This code creates two bullets that travel parallel to each other. It adds a small amount of randomness to the direction. This makes the shots spread out a little bit like a machine gun. We add two random numbers together because this makes their sum more likely to be centered (around zero) and less likely to send bullets far off. We use a quaternion to rotate the initial position of the bullets in the direction they're travelling.

			var aim = Input.GetAimDirection();

			if(aim.LengthSquared() > 0 && cooldownRemaining <= 0)
			{
				cooldownRemaining = cooldownFrames;

				float aimAngle = aim.ToAngle();
				var aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

				float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
				Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

				Vector2 offset = Vector2.Transform(new Vector2(25, -8), aimQuat);
				EntityManager.Add(new Bullet(Position + offset, vel));

				offset = Vector2.Transform(new Vector2(25, 8), aimQuat);
				EntityManager.Add(new Bullet(Position + offset, vel));
			}

			if(cooldownRemaining > 0)
			{
				cooldownRemaining--;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!IsDead)
			{
				base.Draw(spriteBatch);
			}
		}
	}
}
