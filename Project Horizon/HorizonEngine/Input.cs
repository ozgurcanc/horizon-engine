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

        internal static void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
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
    }
}
