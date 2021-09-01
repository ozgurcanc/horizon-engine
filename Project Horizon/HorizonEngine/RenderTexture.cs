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
        internal RenderTexture(string name, int widht, int height) : base(name, null, Screen.CreateRenderTarget(widht, height), null)
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
