using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Diagnostics;
using ImGuiNET;

namespace HorizonEngine
{
    public class Texture : Asset
    {
        [JsonIgnore]
        private Texture2D _texture;
        private Rectangle _sourceRectangle;
        private bool _isOriginal;
        private List<HorizonEngine.Texture> _internalTextures;

        internal Texture(string name, string source, Vector4? sourceRectangle) : this(name, source, Assets.GetSourceTexture(source), sourceRectangle)
        {
            
        }

        protected Texture(string name, string source, Texture2D texture, Vector4? sourceRectangle) : base(name, source)
        {
            _internalTextures = new List<Texture>();
            _texture = texture;
            _isOriginal = true;

            int width = texture.Width;
            int height = texture.Height;

            if (sourceRectangle == null)
            {
                _sourceRectangle = new Rectangle(0, 0, width, height);
            }
            else
            {
                Vector4 value = sourceRectangle.Value;
                _sourceRectangle = new Rectangle((int)(width * value.X), (int)(height * value.Y), (int)(width * value.Z), (int)(height * value.W));
            }
        }

        protected internal Texture2D texture
        {
            get
            {
                return _texture;
            }
            protected set
            {
                _texture = value;
            }
        }

        internal Rectangle sourceRectangle
        {
            get
            {
                return _sourceRectangle;
            }
            set
            {
                if (isOriginal) return;
                _sourceRectangle.X = MathHelper.Clamp(value.X, 0, _texture.Width - 1);
                _sourceRectangle.Y = MathHelper.Clamp(value.Y, 0, _texture.Height - 1);
                _sourceRectangle.Width = MathHelper.Clamp(value.Width, 1, _texture.Width - _sourceRectangle.X);
                _sourceRectangle.Height = MathHelper.Clamp(value.Height, 1, _texture.Height - _sourceRectangle.Y);
            }
        }

        public int width
        {
            get
            {
                return _sourceRectangle.Width;
            }
        }

        public int height
        {
            get
            {
                return _sourceRectangle.Height;
            }
        }

        internal bool isOriginal
        {
            get
            {
                return _isOriginal;
            }
        }

        internal ReadOnlyCollection<HorizonEngine.Texture> internalTextures
        {
            get
            {
                return _internalTextures.AsReadOnly();
            }
        }

        internal void CreateInternalTexture(string name, Vector4 sourceRectangle)
        {
            /*
            float z = this.width / (float)texture.Width;
            float w = this.height / (float)texture.Height;
            sourceRectangle.Z *= z;
            sourceRectangle.W *= w;
            sourceRectangle.X = sourceRectangle.X * z + this.sourceRectangle.X / (float)texture.Width;
            sourceRectangle.Y = sourceRectangle.Y * w + this.sourceRectangle.Y / (float)texture.Height;
            */
            HorizonEngine.Texture internalTex = Assets.CreateTexture(name, this.source, sourceRectangle);
            internalTex._isOriginal = false;
            _internalTextures.Add(internalTex);
        }

        internal override void Reload()
        {
            _internalTextures.ForEach(x => x.Reload());
            _texture = Assets.GetSourceTexture(source);
            Assets.Load(this);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(!(this is RenderTexture))
            {
                if (ImGui.Button("Open in Sprite Editor"))
                {
                    SpriteEditorWindow.Open(this);
                }
            }
        }
    }
}
