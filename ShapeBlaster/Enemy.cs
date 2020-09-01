using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ShapeBlaster
{
	class Enemy : Entity
	{
		private static Random rand = new Random();
		private int timeUntilStart = 60;
		public bool IsActive { get { return timeUntilStart <= 0; } }
		public int PointValue { get; private set; }

		//Note that a behaviour has the type IEnumerator<int>, not IEnumerable<int>. You can think of the IEnumerable as the template for the behaviour and the IEnumerator as the running instance. The IEnumerator remembers where we are in the behaviour and will pick up where it left off when you call its MoveNext() method. Each frame we'll go through all the behaviours the enemy has and call MoveNext() on each of them. If MoveNext() returns false, it means the behaviour has completed so we should remove it from the list.
		private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();

		public Enemy(Texture2D image, Vector2 position)
		{
			this.image = image;
			Position = position;
			Radius = image.Width / 2f;
			color = Color.Transparent;
		}

		private void AddBehaviour(IEnumerable<int> behaviour)
		{
			behaviours.Add(behaviour.GetEnumerator());
		}

		private void ApplyBehaviours()
		{
			for (int i = 0; i < behaviours.Count; i++)
			{
				// When you remove an entry in a list, the size of the entire list gets reduced. This means that the count of the items in the list will also get reduced. If you don't decrement i by one, the for loop will end up going at least one loop longer than it should, i will equal the Count of the list +1, and you'll end up with an out of bounds exception. This is fixed by removing at i--because it removes the entry at i, THEN decrements i by one, preventing the later out of bounds exception, or skipping an entity.

				if (!behaviours[i].MoveNext())
				{
					behaviours.RemoveAt(i--);
				}
			}
		}

		//This method will push the current enemy away from the other enemy.The closer they are, the harder it will be pushed, because the magnitude of (d / d.LengthSquared()) is just one over the distance.
		public void HandleCollision(Enemy other)
		{
			var d = Position - other.Position;
			Velocity += 10 * d / (d.LengthSquared() + 1);
		}

		public static Enemy CreateSeeker(Vector2 position)
		{
			var enemy = new Enemy(Art.Seeker, position);
			enemy.PointValue = 1;
			enemy.AddBehaviour(enemy.FollowPlayer());

			return enemy;
		}

		public static Enemy CreateWanderer(Vector2 position)
		{
			var enemy = new Enemy(Art.Wanderer, position);
			enemy.PointValue = 2;
			enemy.AddBehaviour(enemy.MoveRandomly());

			return enemy;
		}

		// This simply makes the enemy accelerate towards the player at a constant rate. The friction we added earlier will ensure it eventually tops out at some max speed (5 pixels per frame when acceleration is 1 since \(0.8 \times 5 + 1 = 5\)). Each frame, this method will run until it hits the yield statement and will then resume where it left off next frame.

		// You may be wondering why we bothered with iterators at all, since we could have accomplished the same task more easily with a simple delegate. Using iterators pays off with more complex methods that would otherwise require us to store state in member variables in the class.
		IEnumerable<int> FollowPlayer(float acceleration = 1f)
		{
			while (true)
			{
				Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
				if (Velocity != Vector2.Zero)
					Orientation = Velocity.ToAngle();

				yield return 0;
			}
		}

		// To make an enemy that moves randomly, we'll have it choose a direction and then make small random adjustments to that direction. However, if we adjust the direction every frame, the movement will be jittery, so we'll only adjust the direction periodically. If the enemy runs into the edge of the screen, we'll have it choose a new random direction that points away from the wall.
		IEnumerable<int> MoveRandomly()
		{
			float direction = rand.NextFloat(0, MathHelper.TwoPi);

			while (true)
			{
				direction += rand.NextFloat(-0.1f, 0.1f);
				direction = MathHelper.WrapAngle(direction);

				for(int i = 0; i < 6; i++)
				{
					Velocity += MathUtil.FromPolar(direction, 0.4f);
					Orientation -= 0.05f;

					var bounds = GameRoot.Viewport.Bounds;
					bounds.Inflate(-image.Width, -image.Height);

					//if the enemy is outside the bounds, make it move away from the edge
					if (!bounds.Contains(Position.ToPoint()))
					{
						direction = (GameRoot.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
					}

					yield return 0;
				}
			}
		}

		// For example, below is a behaviour that makes an enemy move in a square pattern: 
		// What's nice about this is that it not only saves us some instance variables, but it also structures the code in a very logical way. You can see right away that the enemy will move right, then down, then left, then up, and then repeat. If you were to implement this method as a state machine instead, the control flow would be less obvious.
		IEnumerable<int> MoveInASqaure()
		{
			const int framesPerSide = 30;
			while (true)
			{
				// move right for 30 frames
				for (int i = 0; i < framesPerSide; i++)
				{
					Velocity = Vector2.UnitX;
					yield return 0;
				}

				// move down
				for (int i = 0; i < framesPerSide; i++)
				{
					Velocity = Vector2.UnitY;
					yield return 0;
				}

				// move left
				for (int i = 0; i < framesPerSide; i++)
				{
					Velocity = -Vector2.UnitX;
					yield return 0;
				}

				// move up
				for (int i = 0; i < framesPerSide; i++)
				{
					Velocity = -Vector2.UnitY;
					yield return 0;
				}
			}
		}

		// This code will make enemies fade in for 60 frames and will allow their velocity to function. Multiplying the velocity by 0.8 fakes a friction-like effect. If we make enemies accelerate at a constant rate, this friction will cause them to smoothly approach a maximum speed. I like the simplicity and smoothness of this type of friction, but you may want to use a different formula depending on the effect you want.
		public override void Update()
		{
			if(timeUntilStart <= 0)
			{
				ApplyBehaviours();
			}
			else
			{
				timeUntilStart--;
				color = Color.White * (1 - timeUntilStart / 60f);
			}

			Position += Velocity;
			Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

			Velocity *= 0.8f;
		}

		public void WasShot()
		{
			IsExpired = true;

			// To play sounds in XNA, you can simply call the Play() method on a SoundEffect. This method also provides an overload that allows you to adjust the volume, pitch and pan of the sound. A trick to make our sounds more varied is to adjust these quantities on each play.
			Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);

			PlayerStatus.AddPoints(PointValue);
			PlayerStatus.IncreaseMultiplier();
		}
	}
}
