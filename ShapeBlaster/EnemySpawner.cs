using Microsoft.Xna.Framework;
using System;

namespace ShapeBlaster
{
	static class EnemySpawner
	{
		static Random rand = new Random();
		static float inverseSpawnChance = 60;

		// Each frame, there is a one in inverseSpawnChance of generating each type of enemy. The chance of spawning an enemy gradually increases until it reaches a maximum of one in twenty. Enemies are always created at least 250 pixels away from the player.
		public static void Update()
		{
			if(!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
			{
				if(rand.Next((int)inverseSpawnChance) == 0)
				{
					EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));
				}

				if(rand.Next((int)inverseSpawnChance) == 0)
				{
					EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
				}
			}

			//slowly increase the spawn rate as time progresses
			if(inverseSpawnChance > 20)
			{
				inverseSpawnChance -= 0.005f;
			}		
		}

		//Be careful about the while loop in GetSpawnPosition(). It will work efficiently as long as the area in which enemies can spawn is bigger than the area where they can't spawn. However, if you make the forbidden area too large, you will get an infinite loop.
		private static Vector2 GetSpawnPosition()
		{
			Vector2 pos;
			do
			{
				pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), rand.Next((int)GameRoot.ScreenSize.Y));
			}
			while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

			return pos;
		}

		public static void Reset()
		{
			inverseSpawnChance = 60;
		}
	}
}
