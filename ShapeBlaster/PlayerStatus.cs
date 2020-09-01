using System.IO;

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
		public static bool IsGameOver { get { return Lives == 0; } }
		public static int HighScore { get; private set; }
		private static float multiplierTimeLeft; // time until the current multiplier expires
		private static int scoreForExtraLife; // score required to gain an extra life

		private const string highScoreFilename = "highscore.txt";

		//static constructor
		static PlayerStatus()
		{
			HighScore = LoadHighScore();
			Reset();
		}

		public static void Reset()
		{
			if (Score > HighScore)
				SaveHighScore(HighScore = Score);

			Score = 0;
			Multiplier = 1;
			Lives = 1;
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

		// The LoadHighScore() method first checks that the high score file exists, and then checks that it contains a valid integer. The second check will most likely never fail unless the user manually edits the high score file to something invalid, but it's good to be cautious.
		private static int LoadHighScore()
		{
			// Return the saved high score if possible and return 0 otherwise
			return File.Exists(highScoreFilename) && int.TryParse(File.ReadAllText(highScoreFilename), out int score) ? score : 0;
		}

		private static void SaveHighScore(int score)
		{
			File.WriteAllText(highScoreFilename, score.ToString());
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
