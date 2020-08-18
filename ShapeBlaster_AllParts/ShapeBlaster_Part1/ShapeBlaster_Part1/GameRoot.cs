//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShapeBlaster
{
	public class GameRoot : Microsoft.Xna.Framework.Game
	{
		// some helpful static properties
		public static GameRoot Instance { get; private set; }
		public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
		public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
		public static GameTime GameTime { get; private set; }

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public GameRoot()
		{
			Instance = this;
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
		}

		protected override void Initialize()
		{
			base.Initialize();

			EntityManager.Add(PlayerShip.Instance);

			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(Sound.Music);
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Art.Load(Content);
			Sound.Load(Content);
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;
			Input.Update();

			// Allows the game to exit
			if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
				this.Exit();

			EntityManager.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			EntityManager.Draw(spriteBatch);

			// draw the custom mouse cursor
			spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
