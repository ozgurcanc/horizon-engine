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

        public Sprite()
        {
            _texture = null;
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

        internal override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) return;

            spriteBatch.Draw(_texture.texture, rect, _texture.sourceRectangle, color, MathHelper.ToRadians(gameObject.rotation), new Vector2(_texture.sourceRectangle.Width / 2, texture.sourceRectangle.Height / 2), (SpriteEffects)flipState, 0f);
        }
    }
}
