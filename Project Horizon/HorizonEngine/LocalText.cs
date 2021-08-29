using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HorizonEngine
{
    public class LocalText : Renderer
    {
        private string[] _texts;
        [JsonIgnore]
        private Font _font;
        private string _fontAssetID;
        private Vector2[] _halfSizeOfTexts;
        private static int _local;

        public LocalText()
        {
            _texts = null;
            _font = null;
            _fontAssetID = null;
        }

        public static int local
        {
            get
            {
                return _local;
            }
            set
            {
                _local = value;
            }
        }

        private void UpdateSizeOfText()
        {
            if (_font == null || _texts == null) return;

            int length = _texts.Length;
            _halfSizeOfTexts = new Vector2[length];
            for(int i=0; i<length; i++)
                _halfSizeOfTexts[i] = _font.font.MeasureString(_texts[i]) / 2f;
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
                _fontAssetID = value == null ? null : _font.name;
                UpdateSizeOfText();
            }
        }

        public string[] texts
        {
            set
            {
                _texts = value;
                UpdateSizeOfText();
            }
        }

        public string localText
        {
            get
            {
                return _texts[local];
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            if (_font == null || _texts == null) return;

            spriteBatch.DrawString(_font.font, _texts[_local], new Vector2(rect.X, rect.Y), color, MathHelper.ToRadians(gameObject.rotation), _halfSizeOfTexts[_local], new Vector2(gameObject.size.X, -gameObject.size.Y) * 10f, (SpriteEffects)flipState, layerDepth);
        }

        public override void OnLoad()
        {
            _font = Assets.GetFont(_fontAssetID);
        }
    }
}
