using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using SpriteFontPlus;
using Newtonsoft.Json;
using System.IO;

namespace HorizonEngine
{
    public static class Graphics
    {
        [JsonObject(MemberSerialization.Fields)]
        private class GraphicsSettings
        {
            public Vector2 resolution;
            public bool isFullScreen;
            public bool verticalSynchronization;
            public bool multiSampling;
        }

        private static GraphicsDeviceManager _graphics;
        private static Vector2 _resolution;
        private static Vector2 _fullScreenResolution;
        private static bool _isFullScreen;
        private static bool _verticalSynchronization;
        private static bool _multiSampling;
        private static RenderTarget2D _defaultRenderTarget;

        internal static void Init(GraphicsDeviceManager graphics)
        {
            _defaultRenderTarget = null;
            _graphics = graphics;
            _graphics.ApplyChanges();
            _fullScreenResolution = new Vector2(_graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width, _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
            LoadSettings();           
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
                return isFullScreen ? _fullScreenResolution : _resolution;
            }
            set
            {
                _resolution = value;
                if (isFullScreen || Application.isEditor) return;
                _graphics.PreferredBackBufferWidth = (int)value.X;
                _graphics.PreferredBackBufferHeight = (int)value.Y;
                _graphics.ApplyChanges();
            }
        }

        public static bool isFullScreen
        {
            get
            {
                return _isFullScreen;
            }
            set
            {
                _isFullScreen = value;
                if (Application.isEditor) return;
                _graphics.IsFullScreen = value;
                if(_isFullScreen)
                {
                    _graphics.PreferredBackBufferWidth = (int)_fullScreenResolution.X;
                    _graphics.PreferredBackBufferHeight = (int)_fullScreenResolution.Y;
                }
                else
                {
                    _graphics.PreferredBackBufferWidth = (int)resolution.X;
                    _graphics.PreferredBackBufferHeight = (int)resolution.Y;
                }
                _graphics.ApplyChanges();
            }
        }

        public static bool verticalSynchronization
        {
            get
            {
                return _verticalSynchronization;
            }
            set
            {
                _verticalSynchronization = value;
                if (Application.isEditor) return;
                _graphics.SynchronizeWithVerticalRetrace = value;
                _graphics.ApplyChanges();
            }
        }

        public static bool multiSampling
        {
            get
            {
                return _multiSampling;
            }
            set
            {
                _multiSampling = value;
                if (Application.isEditor) return;
                _graphics.PreferMultiSampling = value;
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

        internal static void SaveSettings()
        {
            GraphicsSettings graphicsSettings = new GraphicsSettings();
            graphicsSettings.resolution = resolution;
            graphicsSettings.isFullScreen = isFullScreen;
            graphicsSettings.verticalSynchronization = verticalSynchronization;
            graphicsSettings.multiSampling = multiSampling;
            File.WriteAllText(Path.Combine(Application.projectPath, "GraphicsSettings.json"), JsonConvert.SerializeObject(graphicsSettings));
        }

        internal static void LoadSettings()
        {
            GraphicsSettings graphicsSettings = JsonConvert.DeserializeObject<GraphicsSettings>(File.ReadAllText(Path.Combine(Application.projectPath, "GraphicsSettings.json")));
            resolution = graphicsSettings.resolution;
            isFullScreen = graphicsSettings.isFullScreen;
            verticalSynchronization = graphicsSettings.verticalSynchronization;
            multiSampling = graphicsSettings.multiSampling;
        }
    }
}
