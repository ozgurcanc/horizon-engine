using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace HorizonEngine
{
    public class Texture : Asset
    {
        [JsonIgnore]
        private Texture2D _texture;
        private Rectangle _sourceRectangle;


        internal Texture(string name, string source, Vector4? sourceRectangle) : this(name, source, Assets.GetSourceTexture(source), sourceRectangle)
        {
            
        }

        protected Texture(string name, string source, Texture2D texture, Vector4? sourceRectangle) : base(name, source)
        {
            _texture = texture;

            int width = texture.Width;
            int height = texture.Height;

            if (sourceRectangle == null)
            {
                _sourceRectangle = new Rectangle(0, 0, width, height);
            }
            else
            {
                Vector4 value = sourceRectangle.Value;
                _sourceRectangle = new Rectangle((int)(width * value.X), (int)(height * value.Y), (int)(width * value.Z), (int)(height * value.W));
            }
        }

        internal Texture2D texture
        {
            get
            {
                return _texture;
            }
        }

        internal Rectangle sourceRectangle
        {
            get
            {
                return _sourceRectangle;
            }
        }

        public int width
        {
            get
            {
                return _texture.Width;
            }
        }

        public int height
        {
            get
            {
                return _texture.Height;
            }
        }

        internal override void Reload()
        {
            _texture = Assets.GetSourceTexture(source);
            Assets.Load(this);
        }
    }
}
