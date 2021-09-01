using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using ImGuiNET;

namespace HorizonEngine
{
    public class Sprite : Renderer
    {
        [JsonIgnore]
        private HorizonEngine.Texture _texture;
        private uint _assetID;

        public Sprite()
        {
            _texture = null;
            _assetID = 0;
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
                _assetID = value == null ? 0 : _texture.assetID;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) return;

            spriteBatch.Draw(_texture.texture, rect, _texture.sourceRectangle, color, MathHelper.ToRadians(gameObject.rotation), new Vector2(_texture.sourceRectangle.Width / 2, texture.sourceRectangle.Height / 2), (SpriteEffects)flipState, layerDepth);
        }

        public override void OnLoad()
        {
            _texture = Assets.GetTexture(_assetID);
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Sprite")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                this.enabled = enabled;
            }

            string textureName = _texture == null ? "None" : _texture.name;
            ImGui.Text("Texture");
            ImGui.SameLine();
            ImGui.Text(textureName);
            ImGui.SameLine();
            if(ImGui.Button("select texture"))
            {
                ImGui.OpenPopup("select_texture");
            }

            if(ImGui.BeginPopup("select_texture"))
            {
                if (ImGui.Selectable("None")) this.texture = null;

                foreach (HorizonEngine.Texture texture in Assets.textures)
                {
                    if (ImGui.Selectable(texture.name)) this.texture = texture;
                }

                ImGui.EndPopup();
            }

            base.OnInspectorGUI();
        }
    }
}
