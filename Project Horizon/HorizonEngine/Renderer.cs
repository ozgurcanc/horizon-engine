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
    public abstract class Renderer : Component
    {
        internal abstract void Draw(SpriteBatch spriteBatch);

        internal Rectangle rect
        {
            get
            {
                float scale = Camera.scale;
                return new Rectangle((int)((gameObject.position.X) * scale), (int)((gameObject.position.Y) * scale), (int)(gameObject.size.X * scale), (int)(gameObject.size.Y * -scale));
            }
        }
    }
}
