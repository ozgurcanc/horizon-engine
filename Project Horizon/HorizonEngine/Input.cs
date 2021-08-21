using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HorizonEngine
{
    public static class Input
    {
        private static KeyboardState _previousKeyboardState;
        private static KeyboardState _currentKeyboardState;
        private static MouseState _previousMouseState;
        private static MouseState _currentMouseState;

        internal static void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }

        public static Vector2 mousePosition
        {
            get
            {
                return _currentMouseState.Position.ToVector2();
            }
        }

        public static float mouseScrollDelta
        {
            get
            {
                return _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
            }
        }

        public static bool GetKeyDown(Keys key)
        {
            return _previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key);
        }

        public static bool GetKey(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public static bool GetKeyUp(Keys key)
        {
            return _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
        }

        public static bool GetMouseButton(int button)
        {
            if(button == 0)
            {
                return _currentMouseState.LeftButton == ButtonState.Pressed;
            }
            else if(button == 1)
            {
                return _currentMouseState.RightButton == ButtonState.Pressed;
            }
            else
            {
                return _currentMouseState.MiddleButton == ButtonState.Pressed;
            }
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (button == 0)
            {
                return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            }
            else if (button == 1)
            {
                return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released;
            }
            else
            {
                return _currentMouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Released;
            }
        }

        public static bool GetMouseButtonUp(int button)
        {
            if (button == 0)
            {
                return _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;
            }
            else if (button == 1)
            {
                return _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;
            }
            else
            {
                return _currentMouseState.MiddleButton == ButtonState.Released && _previousMouseState.MiddleButton == ButtonState.Pressed;
            }
        }
    }
}
