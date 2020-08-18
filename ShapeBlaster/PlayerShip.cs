namespace ShapeBlaster
{
	class PlayerShip : Entity
	{
		private static PlayerShip _instance;
		public static PlayerShip Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new PlayerShip();
				}

				return _instance;
			}
		}

		private PlayerShip()
		{
			image = Art.Player;
			Posisiton = GameRoot.ScreenSize / 2;
			Radius = 10;
		}

		public override void Update()
		{
			// ship logic goes here
		}
	}
}
