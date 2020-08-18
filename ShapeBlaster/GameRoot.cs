using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Viewport = Microsoft.Xna.Framework.Graphics.Viewport;

namespace ShapeBlaster
{
	public class GameRoot : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public static GameRoot Instance { get; private set; }
		public static Viewport Viewport => Instance._graphics.GraphicsDevice.Viewport;
		public static Vector2 ScreenSize => new Vector2(Instance._graphics.PreferredBackBufferWidth, Instance._graphics.PreferredBackBufferHeight);

		public GameRoot()
		{
			Instance = this;
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();

			EntityManager.Add(PlayerShip.Instance);
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			Art.Load(Content);

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			EntityManager.Update();
			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			EntityManager.Draw(_spriteBatch);
			_spriteBatch.End();
			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
