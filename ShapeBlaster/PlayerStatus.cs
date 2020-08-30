using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace ShapeBlaster
{

	// In Shape Blaster, you will begin with four lives, and will gain an additional life every 2000 points. You receive points for destroying enemies, with different types of enemies being worth different amounts of points. Each enemy destroyed also increases your score multiplier by one. If you don't kill any enemies within a short amount of time, your multiplier will be reset. The total amount of points received from each enemy you destroy is the number of points the enemy is worth multiplied by your current multiplier. If you lose all your lives, the game is over and you start a new game with your score reset to zero.
	static class PlayerStatus
	{
		// amount of time it takes, in seconds, for a multiplier to expire
		private const float multiplierExpiryTime = 0.8f;
		private const int maxMultiplier = 20;

		public static int Lives { get; private set; }
		public static int Score { get; private set; }
		public static int Multiplier { get; private set; }

		private static float multiplierTimeLeft; // time until the current multiplier expires
		private static int scoreForExtraLife; // score required to gain an extra life

		//static constructor
		static PlayerStatus()
		{
			Reset();
		}

		public static void Reset()
		{
			Score = 0;
			Multiplier = 1;
			Lives = 4;
			scoreForExtraLife = 2000;
			multiplierTimeLeft = 0;
		}

		public static void Update()
		{
			if(Multiplier > 1)
			{
				// update the multiplier time
				if ((multiplierTimeLeft -= (float)GameRoot.GameTime.ElapsedGameTime.TotalSeconds) <= 0)
				{
					multiplierTimeLeft = multiplierExpiryTime;
					ResetMultiplier();
				}
			}
		}

		public static void AddPoints(int basePoints)
		{
			if (PlayerShip.Instance.IsDead)
			{
				return;
			}

			Score += basePoints * Multiplier;
			while(Score >= scoreForExtraLife)
			{
				scoreForExtraLife += 2000;
				Lives++;
			}
		}

		public static void IncreaseMultiplier()
		{
			if (PlayerShip.Instance.IsDead)
			{
				return;
			}

			multiplierTimeLeft = multiplierExpiryTime;
			if(Multiplier < maxMultiplier)
			{
				Multiplier++;
			}
		}

		public static void ResetMultiplier()
		{
			Multiplier = 1;
		}

		public static void RemoveLife()
		{
			Lives--;
		}
	}
}
