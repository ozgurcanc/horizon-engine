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

namespace HorizonEngine
{
    public static class Assets
    {
        private static ContentManager _contentManager;
        private static Dictionary<string, Texture2D> _sourceTextures;
        private static Dictionary<string, HorizonEngine.Texture> _textures;


        static internal void InitAssets(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _sourceTextures = new Dictionary<string, Texture2D>();
            _textures = new Dictionary<string, Texture>();
        }

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
            _textures.Add(name, new HorizonEngine.Texture(texture, source));
        }

        public static HorizonEngine.Texture GetTexture(string name)
        {
            if(_textures.ContainsKey(name))
                return _textures[name];

            return null;
        }
    }
}
