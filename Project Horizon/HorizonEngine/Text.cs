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
using ImGuiNET;

namespace HorizonEngine
{
    public class Text : Renderer
    {
        private string _text;
        [JsonIgnore]
        private Font _font;
        private string _fontAssetID;
        private Vector2 _halfSizeOfText;

        public Text()
        {
            _text = "Text";
            _font = null;
            _fontAssetID = null;
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
                _fontAssetID = value == null ? null : _font.name;
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

        public override void OnLoad()
        {
            font = Assets.GetFont(_fontAssetID);
        }

        public override void OnInspectorGUI()
        {
            
            if (!ImGui.CollapsingHeader("Text")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                this.enabled = enabled;
            }

            string fontName = _font == null ? "None" : _font.name;
            ImGui.Text("Font");
            ImGui.SameLine();
            ImGui.Text(fontName);

            string text = this.text;
            ImGui.Text("Text");
            ImGui.SameLine();
            if (ImGui.InputText("##text" + id, ref text, 400))
            {
                this.text = text;
            }

            base.OnInspectorGUI();          
        }
    }
}
