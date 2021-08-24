using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace HorizonEngine
{
    public static class Screen
    {
        private static GraphicsDeviceManager _graphics;
        private static Vector2 _resolution;

        internal static void Init(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        internal static GraphicsDeviceManager graphics
        {
            get
            {
                return _graphics;
            }
        }

        public static Vector2 resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;
                _graphics.PreferredBackBufferWidth = (int)value.X;
                _graphics.PreferredBackBufferHeight = (int)value.Y;
                _graphics.ApplyChanges();
            }
        }
    }
}
