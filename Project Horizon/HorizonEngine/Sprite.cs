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
    public class Sprite : Renderer
    {
        private HorizonEngine.Texture _texture;
        private Color _color;
        private int _flipState;

        public Sprite()
        {
            _texture = null;
            _color = Color.White;
            _flipState = 0;
        }

        public bool flipX
        {
            get
            {
                return _flipState % 2 == 1;
            }
            set
            {
                if (value) _flipState |= 1;
                else _flipState &= ~1;
            }
        }

        public bool flipY
        {
            get
            {
                return _flipState >= 2;
            }
            set
            {
                if (value) _flipState |= 2;
                else _flipState &= ~2;
            }
        }

        public HorizonEngine.Texture texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;
            }
        }

        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) return;

            spriteBatch.Draw(_texture.texture, rect, _texture.sourceRectangle, _color, MathHelper.ToRadians(gameObject.rotation), new Vector2(_texture.sourceRectangle.Width / 2, texture.sourceRectangle.Height / 2), (SpriteEffects)_flipState, 0f);
        }
    }
}
