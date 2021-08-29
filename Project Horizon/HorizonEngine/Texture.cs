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
        private string _name;
        private Texture2D _texture;
        private Rectangle _sourceRectangle;

        internal Texture(string name, Texture2D texture, Rectangle sourceRectangle)
        {
            _name = name;
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

        internal string name
        {
            get
            {
                return _name;
            }
        }
    }
}
