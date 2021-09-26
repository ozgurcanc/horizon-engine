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
    public class Camera : Component
    {
        private float _width;
        private float _height;
        //private Vector2 _resolution;
        private Matrix _renderTransform;
        private Matrix _worldToScreen;
        private Matrix _screenToWorld;
        private int _cullingMask;      
        private Color _backgroundColor;
        [JsonIgnore]
        private RenderTexture _renderTexture;
        private uint _assetID;

        public Camera()
        {
            _width = _height = 10;
            //_resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _cullingMask = 0;
            _backgroundColor = Color.CornflowerBlue;
            _renderTexture = null;
            _assetID = 0;
        }

        internal static float scale
        {
            get
            {
                return 1000f;
            }
        }

        internal static int maxSortOrder
        {
            get
            {
                return 1000;
            }
        }

        internal void Update()
        {
            Vector2 resolution = this.resolution;
            Matrix m = Matrix.CreateTranslation(-gameObject.position.X * scale, -gameObject.position.Y * scale, 0);
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(-gameObject.rotation));
            m *= Matrix.CreateScale(resolution.X / (_width * scale), -resolution.Y / (_height * scale), 1);
            m *= Matrix.CreateTranslation(resolution.X / 2, resolution.Y / 2, 0);
            _renderTransform = m;

            m = Matrix.CreateTranslation(-gameObject.position.X, -gameObject.position.Y, 0);
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(-gameObject.rotation));
            m *= Matrix.CreateScale(resolution.X / _width, -resolution.Y / _height, 1);
            m *= Matrix.CreateTranslation(resolution.X / 2, resolution.Y / 2, 0);
            _worldToScreen = m;

            _screenToWorld = Matrix.Invert(m);
        }

        internal void PreRender()
        {
            Screen.SetRenderTarget(_renderTexture);
            Screen.Clear(_backgroundColor);
        }

        internal Matrix renderTransform
        {
            get
            {
                return _renderTransform;
            }
        }

        public Color backgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
            }
        }

        public Vector2 position
        {
            get
            {
                return gameObject.position;
            }
            set
            {
                gameObject.position = value;
            }
        }

        public float rotation
        {
            get
            {
                return gameObject.rotation;
            }
            set
            {
                gameObject.rotation = value % 360f;
            }
        }

        public float width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public float height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public Vector2 resolution
        {
            get
            {
                return _renderTexture == null ? Screen.resolution : new Vector2(_renderTexture.width, _renderTexture.height);
            }
        }

        internal int cullingMask
        {
            get
            {
                return _cullingMask;
            }
        }

        public RenderTexture renderTarget
        {
            get
            {
                return _renderTexture;
            }
            set
            {
                _renderTexture = value;
                _assetID = value == null ? 0 : _renderTexture.assetID;
            }
        }

        public Vector2 WorldToScreenPoint(Vector2 position)
        {
            return Vector2.Transform(position, _worldToScreen);
        }

        public Vector2 ScreenToWorldPoint(Vector2 position)
        {
            return Vector2.Transform(position, _screenToWorld);
        }

        public void CullLayer(Layer layer, bool cull = true)
        {
            if (cull) _cullingMask |= 1 << (int)layer;
            else _cullingMask &= ~(1 << (int)layer);
        }

        public override void OnLoad()
        {
            _renderTexture = Assets.GetRenderTexture(_assetID);
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Camera")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                Undo.RegisterAction(this, this.enabled, enabled, nameof(Camera.enabled));
                this.enabled = enabled;
            }

            float width = this.width;
            ImGui.Text("Width");
            ImGui.SameLine();
            if(ImGui.DragFloat("##cameraWidth" + id, ref width))
            {
                Undo.RegisterAction(this, this.width, width, nameof(Camera.width));
                this.width = width;
            }

            float height = this.height;
            ImGui.Text("Height");
            ImGui.SameLine();
            if (ImGui.DragFloat("##cameraHeight" + id, ref height))
            {
                Undo.RegisterAction(this, this.height, height, nameof(Camera.height));
                this.height = height;
            }

            Vector4 backgroundColor = this.backgroundColor.ToVector4();
            System.Numerics.Vector4 numColor = new System.Numerics.Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W);
            ImGui.Text("Background Color");
            //ImGui.SameLine();
            if (ImGui.ColorPicker4("##backgroundColor" + id, ref numColor))
            {
                Color temp = new Color(numColor.X, numColor.Y, numColor.Z, numColor.W);
                Undo.RegisterAction(this, this.backgroundColor, temp, nameof(Camera.backgroundColor));
                this.backgroundColor = temp;
            }

            string renderTarget = this.renderTarget == null ? "None" : this.renderTarget.name;
            ImGui.Text("Render Target");
            ImGui.SameLine();
            ImGui.Text(renderTarget);
            ImGui.SameLine();
            if(ImGui.Button("select render texture"))
            {
                ImGui.OpenPopup("select_render_target");
                SearchBar.Clear();
            }

            if(ImGui.BeginPopup("select_render_target"))
            {
                SearchBar.Draw();

                if (ImGui.Selectable("None"))
                {
                    Undo.RegisterAction(this, this.renderTarget, null, nameof(Camera.renderTarget));
                    this.renderTarget = null;
                }

                foreach(RenderTexture renderTexture in Assets.renderTextures)
                {
                    if (SearchBar.PassFilter(renderTexture.name) && ImGui.Selectable(renderTexture.name))
                    {
                        Undo.RegisterAction(this, this.renderTarget, renderTexture, nameof(Camera.renderTarget));
                        this.renderTarget = renderTexture;
                    }
                }

                ImGui.EndPopup();
            }

        }
    }
}
