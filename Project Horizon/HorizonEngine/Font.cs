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
    public class Font
    {
        private SpriteFont _font;

        internal Font(SpriteFont font)
        {
            _font = font;
        }

        internal SpriteFont font
        {
            get
            {
                return _font;
            }
        }
    }
}
