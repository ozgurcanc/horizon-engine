using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


namespace HorizonEngine
{
    public class Text : Renderer
    {
        private string _text;
        private Font _font;
        private Vector2 _halfSizeOfText;

        public Text()
        {
            _text = "Text";
            _font = null;
        }
        
        private void UpdateSizeOfText()
        {
            if (_font == null) return;

            _halfSizeOfText = _font.font.MeasureString(_text) / 2f;
        }

        public Font font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                UpdateSizeOfText();
            }
        }

        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                UpdateSizeOfText();
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            if (_font == null) return;

            spriteBatch.DrawString(_font.font, _text, new Vector2(rect.X, rect.Y), color, MathHelper.ToRadians(gameObject.rotation), _halfSizeOfText, new Vector2(gameObject.size.X, -gameObject.size.Y) * 10f, (SpriteEffects)flipState, layerDepth);
        }
    }
}
