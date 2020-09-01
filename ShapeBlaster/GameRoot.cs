using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
		public static GameTime GameTime { get; private set; }

		public GameRoot()
		{
			Instance = this;
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = false;
		}

		protected override void Initialize()
		{
			// The base.Initialize() method will call the LoadContent() method of the Game class. So anything that depends on content being loaded should go after the call to base.Initialize(). I will clarify this in the tutorial.
			base.Initialize();

			EntityManager.Add(PlayerShip.Instance);
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(Sound.Music);
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			Art.Load(Content);
			Sound.Load(Content);
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			Input.Update();
			EntityManager.Update();
			EnemySpawner.Update();
			PlayerStatus.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// Draw the entities, sort by texture for better batching
			_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			EntityManager.Draw(_spriteBatch);
			_spriteBatch.End();

			// draw the UI
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			_spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
			DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
			DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

			// draw the customer mouse cursor
			_spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

			if (PlayerStatus.IsGameOver)
			{
				string text = "Game Over\n" +
					"Your Score: " + PlayerStatus.Score + "\n" +
					"High Score: " + PlayerStatus.HighScore;

				Vector2 textSize = Art.Font.MeasureString(text);
				_spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize, Color.White);
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		// DrawRightAlignedString() is a helper method for drawing text aligned on the right side of the screen. 
		private void DrawRightAlignedString(string text, float y)
		{
			var textWidth = Art.Font.MeasureString(text).X;
			_spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
		}
	}
}
