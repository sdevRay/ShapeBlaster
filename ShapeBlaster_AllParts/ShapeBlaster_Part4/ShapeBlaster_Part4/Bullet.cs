﻿//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
	class Bullet : Entity
	{
		private static Random rand = new Random();

		public Bullet(Vector2 position, Vector2 velocity)
		{
			image = Art.Bullet;
			Position = position;
			Velocity = velocity;
			Orientation = Velocity.ToAngle();
			Radius = 8;
		}

		public override void Update()
		{
			if (Velocity.LengthSquared() > 0)
				Orientation = Velocity.ToAngle();

			Position += Velocity;

			// delete bullets that go off-screen
			if (!GameRoot.Viewport.Bounds.Contains(Position.ToPoint()))
			{
				IsExpired = true;

				for (int i = 0; i < 30; i++)
					GameRoot.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
						new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });

			}
		}
	}
}
