using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.IO;

namespace HorizonEngine
{
    public static class Assets
    {
        private static string _path;
        private static uint _assetID;
        private static Dictionary<string, Texture2D> _sourceTextures;
        private static Dictionary<uint, HorizonEngine.Texture> _textures;
        private static Dictionary<uint, Animation> _animations;
        private static Dictionary<uint, Font> _fonts;
        private static Dictionary<uint, RenderTexture> _renderTextures;
        private static Dictionary<uint, AnimatorController> _animatorControllers;

        static Assets()
        {
            _sourceTextures = new Dictionary<string, Texture2D>();
            _textures = new Dictionary<uint, Texture>();
            _animations = new Dictionary<uint, Animation>();
            _fonts = new Dictionary<uint, Font>();
            _renderTextures = new Dictionary<uint, RenderTexture>();
            _animatorControllers = new Dictionary<uint, AnimatorController>();
            _assetID = 1;
            _path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets");
        }

        internal static string path
        {
            get
            {
                return _path;
            }
        }

        internal static uint availableAssetID
        {
            get
            {
                return _assetID++;
            }
        }

        internal static Dictionary<uint, Font>.ValueCollection fonts
        {
            get
            {
                return _fonts.Values;
            }
        }

        internal static Font CreateFont(string name, string source)
        {
            Font font = new Font(name, source);
            _fonts.Add(font.assetID, font);
            return font;
        }

        public static Font GetFont(uint id) 
        {
            if (_fonts.ContainsKey(id))
                return _fonts[id];
           
            return null;  
        }

        internal static void Load(Font font)
        {
            _fonts.Add(font.assetID, font);
        }

        public static Animation GetAnimation(uint id) { return null; }
        public static RenderTexture GetRenderTexture(uint id) { return null; }
        public static AnimatorController GetAnimatorController(uint id) { return null; }
        public static HorizonEngine.Texture GetTexture(uint id) { return null; }



        /*

        public static void LoadTexture(string name, string sourceTexture, Vector4? sourceRectangle = null)
        {
            Texture2D texture;
            if(_sourceTextures.ContainsKey(sourceTexture))
            {
                texture = _sourceTextures[sourceTexture];
            }
            else
            {
                texture = _contentManager.Load<Texture2D>(sourceTexture);
                _sourceTextures.Add(sourceTexture, texture);
            }

            int width = texture.Width;
            int height = texture.Height;
            Rectangle source;
            if(sourceRectangle == null)
            {
                source = new Rectangle(0, 0, width, height);
            }
            else
            {
                Vector4 value = sourceRectangle.Value;
                source = new Rectangle((int)(width * value.X), (int)(height * value.Y), (int)(width * value.Z), (int)(height * value.W));
            }
            //Rectangle source = sourceRectangle == null ? new Rectangle(0, 0, texture.Width, texture.Height) : sourceRectangle;
            _textures.Add(name, new HorizonEngine.Texture(name, texture, source));
        }

        public static void LoadAnimation(string name, float duration, bool loop, params string[] frameTextures)
        {
            int length = frameTextures.Length;
            HorizonEngine.Texture[] frames = new HorizonEngine.Texture[length];

            for (int i = 0; i < length; i++)
                frames[i] = GetTexture(frameTextures[i]);

            _animations.Add(name, new Animation(name, duration, loop, frames));
        }

        public static void LoadFont(string name, string source)
        {
            _fonts.Add(name, new Font(name, _contentManager.Load<SpriteFont>(source)));
        }

        public static void LoadRenderTexture(string name, int width, int height)
        {
            _renderTextures.Add(name, new RenderTexture(name, width, height));
        }

        public static void LoadAnimatorController(string name)
        {
            _animatorControllers.Add(name, new AnimatorController(name));
        }

        public static HorizonEngine.Texture GetTexture(string name)
        {
            if(_textures.ContainsKey(name))
                return _textures[name];

            if (_renderTextures.ContainsKey(name))
                return _renderTextures[name];

            return null;
        }

        public static Font GetFont(string name)
        {
            if (_fonts.ContainsKey(name))
                return _fonts[name];

            return null;
        }

        public static RenderTexture GetRenderTexture(string name)
        {
            if (_renderTextures.ContainsKey(name))
                return _renderTextures[name];

            return null;
        }

        public static AnimatorController GetAnimatorController(string name)
        {
            if (_animatorControllers.ContainsKey(name))
                return _animatorControllers[name];

            return null;
        }

        internal static Animation GetAnimation(string name)
        {
            if (_animations.ContainsKey(name))
                return _animations[name];

            return null;
        }
        */
    }
}
