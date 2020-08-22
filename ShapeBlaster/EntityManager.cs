using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ShapeBlaster
{
	static class EntityManager
	{
		static List<Entity> entities = new List<Entity>();

		static bool isUpdating;
		static List<Entity> addedEntities = new List<Entity>();
		public static int Count { get { return entities.Count(); } }

		//Remember, if you modify a list while iterating over it, you will get an exception.The above code takes care of this by queuing up any entities added during updating in a separate list, and adding them after it finishes updating the existing entities

		public static void Add(Entity entity)
		{
			if (!isUpdating)
				entities.Add(entity);
			else
				addedEntities.Add(entity);
		}

		public static void Update()
		{
			isUpdating = true;

			foreach(var entity in entities)
			{
				entity.Update();
			}

			isUpdating = false;

			foreach(var entity in addedEntities)
			{
				entities.Add(entity);
			}

			addedEntities.Clear();

			// remove any expired entities
			entities = entities.Where(e => !e.IsExpired).ToList();
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
