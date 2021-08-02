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
    public class Texture
    {
        private Texture2D _texture;
        private Rectangle _sourceRectangle;

        internal Texture(Texture2D texture, Rectangle sourceRectangle)
        {
            _texture = texture;
            _sourceRectangle = sourceRectangle;
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
    }
}
