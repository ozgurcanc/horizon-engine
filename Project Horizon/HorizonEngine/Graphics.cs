using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using SpriteFontPlus;
using System.IO;

namespace HorizonEngine
{
    public static class Graphics
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

        public static bool isFullScreen
        {
            get
            {
                return _graphics.IsFullScreen;
            }
            set
            {
                _graphics.IsFullScreen = value;
                _graphics.ApplyChanges();
            }
        }

        public static bool verticalSynchronization
        {
            get
            {
                return _graphics.SynchronizeWithVerticalRetrace;
            }
            set
            {
                _graphics.SynchronizeWithVerticalRetrace = value;
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

        internal static SpriteFont CreateSpriteFont(string fullPath)
        {
            var fontBake = TtfFontBaker.Bake(File.ReadAllBytes(fullPath), 25, 1024, 1024, new[]
                                    {
                                        CharacterRange.BasicLatin,
                                        CharacterRange.Latin1Supplement,
                                        CharacterRange.LatinExtendedA,
                                        CharacterRange.Cyrillic
                                    });

            return fontBake.CreateSpriteFont(_graphics.GraphicsDevice);
        }

        internal static Texture2D CreateTexture2D(string fullPath)
        {
            FileStream stream = new FileStream(fullPath, FileMode.Open);
            Texture2D texture = Texture2D.FromStream(_graphics.GraphicsDevice, stream);
            stream.Close();
            return texture;
        }
    }
}
