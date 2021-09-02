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
using Newtonsoft.Json;

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

        internal static void Save()
        {
            File.WriteAllText("assetID.json", JsonConvert.SerializeObject(_assetID));
        }

        internal static void Load()
        {
            _assetID = JsonConvert.DeserializeObject<uint>(File.ReadAllText("assetID.json"));
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
                _assetID++;
                return _assetID;
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


        internal static Dictionary<uint, HorizonEngine.Texture>.ValueCollection textures
        {
            get
            {
                return _textures.Values;
            }
        }

        internal static HorizonEngine.Texture CreateTexture(string name, string source, Vector4? sourceRectangle = null)
        {
            HorizonEngine.Texture texture = new HorizonEngine.Texture(name, source, sourceRectangle);
            _textures.Add(texture.assetID, texture);
            return texture;
        }

        internal static HorizonEngine.Texture GetTexture(uint id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id];

            return null;
        }

        internal static void Load(HorizonEngine.Texture texture)
        {
            _textures.Add(texture.assetID, texture);
        }

        internal static Texture2D GetSourceTexture(string source)
        {
            if (_sourceTextures.ContainsKey(source))
                return _sourceTextures[source];

            Texture2D texture2D = Screen.CreateTexture2D(Path.Combine(Assets.path, source));
            _sourceTextures.Add(source, texture2D);
            return texture2D;
        }

        internal static Dictionary<uint, RenderTexture>.ValueCollection renderTextures
        {
            get
            {
                return _renderTextures.Values;
            }
        }

        internal static RenderTexture CreateRenderTexture(string name, int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture(name, width, height);
            _renderTextures.Add(renderTexture.assetID, renderTexture);
            _textures.Add(renderTexture.assetID, renderTexture);
            return renderTexture;
        }

        internal static RenderTexture GetRenderTexture(uint id)
        {
            if (_renderTextures.ContainsKey(id))
                return _renderTextures[id];

            return null;
        }

        internal static void Load(RenderTexture renderTexture)
        {
            _textures.Add(renderTexture.assetID, renderTexture);
            _renderTextures.Add(renderTexture.assetID, renderTexture);
        }

        internal static Dictionary<uint, Animation>.ValueCollection animations
        {
            get
            {
                return _animations.Values;
            }
        }

        internal static Animation CreateAnimation(string name, float duration = 1f, bool loop = true)
        {
            Animation animation = new Animation(name, duration, loop);
            _animations.Add(animation.assetID, animation);
            return animation;
        }

        internal static Animation GetAnimation(uint id)
        {
            if (_animations.ContainsKey(id))
                return _animations[id];

            return null;
        }

        internal static void Load(Animation animation)
        {
            _animations.Add(animation.assetID, animation);
        }

        public static AnimatorController GetAnimatorController(uint id) { return null; }


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
