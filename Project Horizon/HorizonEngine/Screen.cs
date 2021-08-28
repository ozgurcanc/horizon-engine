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
        private static RenderTarget2D _defaultRenderTarget;

        internal static void Init(GraphicsDeviceManager graphics)
        {
            _defaultRenderTarget = null;
            _graphics = graphics;
            _resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        internal static RenderTarget2D defaultRenderTarget
        {
            set
            {
                if(_defaultRenderTarget != null) _defaultRenderTarget.Dispose();
                _defaultRenderTarget = value;
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
                if (Application.isEditor) return;
                _graphics.PreferredBackBufferWidth = (int)value.X;
                _graphics.PreferredBackBufferHeight = (int)value.Y;
                _graphics.ApplyChanges();
            }
        }

        internal static void SetRenderTarget(RenderTexture renderTexture)
        {
            if(renderTexture == null)
                _graphics.GraphicsDevice.SetRenderTarget(_defaultRenderTarget);
            else
                _graphics.GraphicsDevice.SetRenderTarget(renderTexture.renderTexture);
        }

        internal static void Clear(Color color)
        {
            _graphics.GraphicsDevice.Clear(color);
        }

        internal static void Begin()
        {
            _graphics.GraphicsDevice.SetRenderTarget(_defaultRenderTarget);
        }

        internal static void End()
        {
            _graphics.GraphicsDevice.SetRenderTarget(null);
        }

        internal static RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return new RenderTarget2D(_graphics.GraphicsDevice, width, height);
        }
    }
}
