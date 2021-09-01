using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.IO;

namespace HorizonEngine
{
    public class Font : Asset
    {
        [JsonIgnore]
        private SpriteFont _font;

        internal Font(string name, string source) : base(name, source)
        {
            _font = Screen.CreateSpriteFont(Path.Combine(Assets.path, source));
        }

        internal SpriteFont font
        {
            get
            {
                return _font;
            }
        }

        internal override void Reload()
        {
            _font = Screen.CreateSpriteFont(Path.Combine(Assets.path, source));
            Assets.Load(this);
        }
    }
}
