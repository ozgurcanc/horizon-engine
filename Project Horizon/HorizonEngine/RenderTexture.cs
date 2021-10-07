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
        private int _width;
        private int _height;

        internal RenderTexture(string name, int width, int height) : base(name, null, Graphics.CreateRenderTarget(width, height), null)
        {
            _width = width;
            _height = height;
        }

        internal RenderTarget2D renderTexture
        {
            get
            {
                return (RenderTarget2D)this.texture;
            }
        }

        internal override void Reload()
        {
            this.texture = Graphics.CreateRenderTarget(_width, _height);
            Assets.Load(this);
        }

        internal override void Delete()
        {
            Assets.Delete(this);
        }
    }
}
