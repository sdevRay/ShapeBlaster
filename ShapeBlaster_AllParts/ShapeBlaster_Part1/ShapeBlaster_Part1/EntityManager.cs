//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeBlaster
{
	static class EntityManager
	{
		static List<Entity> entities = new List<Entity>();
		static List<Bullet> bullets = new List<Bullet>();

		static bool isUpdating;
		static List<Entity> addedEntities = new List<Entity>();

		public static int Count { get { return entities.Count; } }

		public static void Add(Entity entity)
		{
			if (!isUpdating)
				AddEntity(entity);
			else
				addedEntities.Add(entity);
		}

		private static void AddEntity(Entity entity)
		{
			entities.Add(entity);
			if (entity is Bullet)
				bullets.Add(entity as Bullet);
		}

		public static void Update()
		{
			isUpdating = true;

			foreach (var entity in entities)
				entity.Update();

			isUpdating = false;

			foreach (var entity in addedEntities)
				AddEntity(entity);

			addedEntities.Clear();

			entities = entities.Where(x => !x.IsExpired).ToList();
			bullets = bullets.Where(x => !x.IsExpired).ToList();
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			foreach (var entity in entities)
				entity.Draw(spriteBatch);
		}
	}
}