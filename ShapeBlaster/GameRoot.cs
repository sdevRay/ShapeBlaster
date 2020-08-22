﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Configuration;
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

			// The base.Initialize() method will call the LoadContent() method of the Game class. So anything that depends on content being loaded should go after the call to base.Initialize(). I will clarify this in the tutorial.
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

			Input.Update();
			EntityManager.Update();
			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			EntityManager.Draw(_spriteBatch);

			// draw the customer mouse cursor
			_spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
			_spriteBatch.End();
			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
