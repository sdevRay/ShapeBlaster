using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ShapeBlaster
{
	static class EntityManager
	{
		static List<Entity> entities = new List<Entity>();
		static List<Entity> addedEntities = new List<Entity>();
		static List<Enemy> enemies = new List<Enemy>();
		static List<Bullet> bullets = new List<Bullet>();

		static bool isUpdating;
		public static int Count { get { return entities.Count(); } }

		// public method for adding entities
		public static void Add(Entity entity)
		{
			if (!isUpdating)
			{
				AddEntity(entity);
			}
			else
			{
				addedEntities.Add(entity);
			}
		}

		//With this setup, if you try to add a bullet while we're updating existing entities, it will be queued up and added after the updating has finished. We add it in the foreach loop with a call to our helper method, AddEntities(), which adds it not only to the entities list, but also to the bullets list. If you call entities.Add() instead of AddEntities in that foreach loop, the bullet won't be added to the bullets list and won't get collision detection.

		// private helper method to add entities to all the correct lists
		private static void AddEntity(Entity entity)
		{
			entities.Add(entity);
			
			if (entity is Bullet)
			{
				bullets.Add(entity as Bullet);
			}
			else if (entity is Enemy)
			{
				enemies.Add(entity as Enemy);
			}
		}

		//Remember, if you modify a list while iterating over it, you will get an exception.The above code takes care of this by queuing up any entities added during updating in a separate list, and adding them after it finishes updating the existing entities
		public static void Update()
		{
			isUpdating = true;

			HandleCollisions();

			foreach (var entity in entities)
			{
				entity.Update();
			}

			isUpdating = false;

			foreach(var entity in addedEntities)
			{
				AddEntity(entity);
			}

			addedEntities.Clear();

			// remove any expired entities
			entities = entities.Where(e => !e.IsExpired).ToList();
			bullets = bullets.Where(b => !b.IsExpired).ToList();
			enemies = enemies.Where(en => !en.IsExpired).ToList();
		}

		static void HandleCollisions()
		{
			// handle collisions between enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				for (int j = i + 1; j < enemies.Count; j++)
				{
					if(IsColliding(enemies[i], enemies[j]))
					{
						enemies[i].HandleCollision(enemies[j]);
						enemies[j].HandleCollision(enemies[i]);
					}
				}
			}

			// handle collisions between bullets and enemies
			for(int i = 0; i < enemies.Count; i++)
			{
				for (int j = 0; j < bullets.Count; j++)
				{
					if(IsColliding(enemies[i], bullets[j]))
					{
						enemies[i].WasShot();
						bullets[j].IsExpired = true;
					}
				}
			}

			// handle collisions between the player and enemeis
			for(int i = 0; i < enemies.Count; i++)
			{
				if(enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
				{
					PlayerShip.Instance.Kill();
					enemies.ForEach(x => x.WasShot());
					break;
				}
			}
		}

		// To determine if two circles overlap, simply check if the distance between them is less than the sum of their radii. Our method optimizes this slightly by checking if the square of the distance is less than the square of the sum of the radii. Remember that it's a bit faster to compute the distance squared than the actual distance.
		private static bool IsColliding(Entity a, Entity b)
		{
			float radius = a.Radius + b.Radius;
			return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (var entity in entities)
			{
				entity.Draw(spriteBatch);
			}
		}
	}
}
