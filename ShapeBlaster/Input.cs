using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ShapeBlaster
{
	static class Input
	{
		private static KeyboardState _keyboardState, _lastKeyboardState;
		private static MouseState _mouseState, _lastMouseState;
		private static GamePadState _gamepadState, _lastGamepadState;

		private static bool _isAimingwithMouse = false;

		public static Vector2 MousePosition => new Vector2(_mouseState.X, _mouseState.Y);
	
		public static void Update()
		{
			_lastMouseState = _mouseState;
			_lastKeyboardState = _keyboardState;
			_lastGamepadState = _gamepadState;

			_keyboardState = Keyboard.GetState();
			_mouseState = Mouse.GetState();
			_gamepadState = GamePad.GetState(PlayerIndex.One);

			// arrow keys are being pressed, or right thumbstick has been moved
			if(new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(k => _keyboardState.IsKeyDown(k)) || _gamepadState.ThumbSticks.Right != Vector2.Zero)
			{
				_isAimingwithMouse = false; 
			} // mouse has been moved
			else if (MousePosition != new Vector2(_lastMouseState.X, _lastMouseState.Y))
			{
				_isAimingwithMouse = true;
			}
		}

		// check if a key was just pressed down
		public static bool WasKeyPressed(Keys key)
		{
			return _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
		}

		public static bool WasButtonPressed(Buttons button)
		{
			return _lastGamepadState.IsButtonUp(button) && _gamepadState.IsButtonDown(button);
		}

		public static Vector2 GetMovementDirection()
		{
			Vector2 direction = _gamepadState.ThumbSticks.Left;
			direction.Y *= -1;

			if (_keyboardState.IsKeyDown(Keys.A))
				direction.X -= 1;
			if (_keyboardState.IsKeyDown(Keys.D))
				direction.X += 1;
			if (_keyboardState.IsKeyDown(Keys.W))
				direction.Y -= 1;
			if (_keyboardState.IsKeyDown(Keys.S))
				direction.Y += 1;

			// clamp the length of the vector to a maximum of 1
			//You may also notice in GetMovementDirection() I wrote direction.LengthSquared() > 1. Using LengthSquared() is a small performance optimization; computing the square of the length is a bit faster than computing the length itself because it avoids the relatively slow square root operation. You'll see code using the squares of lengths or distances throughout the program. In this particular case, the performance difference is negligible, but this optimization can make a difference when used in tight loops.
			if (direction.LengthSquared() > 1)
				direction.Normalize();

			return direction;
		}

		public static Vector2 GetAimDirection()
		{
			if (_isAimingwithMouse)
				return GetMouseAimDirection();

			Vector2 direction = _gamepadState.ThumbSticks.Right;
			direction.Y *= -1;

			if (_keyboardState.IsKeyDown(Keys.Left))
				direction.X -= 1;
			if (_keyboardState.IsKeyDown(Keys.Right))
				direction.X += 1;
			if (_keyboardState.IsKeyDown(Keys.Up))
				direction.Y -= 1;
			if (_keyboardState.IsKeyDown(Keys.Down))
				direction.Y += 1;

			// if theres no aim input, return zero. otherwise normalize the direcction to have a length of 1
			if (direction == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			else
			{
				return Vector2.Normalize(direction); // wtf does this mean?
			}
		}

		private static Vector2 GetMouseAimDirection()
		{
			Vector2 direction = MousePosition - PlayerShip.Instance.Posisiton;

			if(direction == Vector2.Zero)
			{
				return Vector2.Zero;		
			}
			else
			{
				return Vector2.Normalize(direction);
			}
		}
		
		public static bool WasBombButtonPressed()
		{
			return WasButtonPressed(Buttons.LeftTrigger) || WasButtonPressed(Buttons.RightTrigger) || WasKeyPressed(Keys.Space);
		}
	}
}
