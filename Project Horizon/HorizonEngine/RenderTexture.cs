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
    public class RenderTexture : HorizonEngine.Texture
    {
        internal RenderTexture(int widht, int height) : base(Screen.CreateRenderTarget(widht, height), new Rectangle(0, 0, widht, height))
        {

        }

        internal RenderTarget2D renderTexture
        {
            get
            {
                return (RenderTarget2D)this.texture;
            }
        }
    }
}
