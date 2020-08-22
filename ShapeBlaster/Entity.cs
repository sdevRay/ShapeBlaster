using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeBlaster
{
	//All our entities(enemies, bullets and the player's ship) have some basic properties such as an image and a position. IsExpired will be used to indicate that the entity has been destroyed and should be removed from any lists holding a reference to it.

	abstract class Entity
	{
		protected Texture2D image;

		protected Color color = Color.White;

		public Vector2 Posisiton, Velocity;
		public float Orientation;
		public float Radius = 20; // used for circular collision detection
		public bool IsExpired; // true if the entity was destroyed and should be deleted

		public Vector2 Size
		{
			get
			{
				return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
			}
		}

		public abstract void Update();
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(image, Posisiton, null, color, Orientation, Size / 2f, 1f, SpriteEffects.None, 0);
		}
	}
}
