using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using ImGuiNET;

namespace HorizonEngine
{
    public abstract class Renderer : Component
    {
        private Color _color;
        private int _flipState;
        private float _sortingOrder;
        public Renderer()
        {
            _color = Color.White;
            _flipState = 0;
            _sortingOrder = 0f;
        }

        public bool flipX
        {
            get
            {
                return _flipState % 2 == 1;
            }
            set
            {
                if (value) _flipState |= 1;
                else _flipState &= ~1;
            }
        }

        public bool flipY
        {
            get
            {
                return _flipState >= 2;
            }
            set
            {
                if (value) _flipState |= 2;
                else _flipState &= ~2;
            }
        }

        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public int sortingOrder
        {
            get
            {
                return (int)(_sortingOrder * Camera.maxSortOrder);
            }
            set
            {
                _sortingOrder = value / (float)Camera.maxSortOrder;
            }
        }

        internal float layerDepth
        {
            get
            {
                return _sortingOrder;
            }
        }
        
        internal int flipState
        {
            get
            {
                return _flipState;
            }
        }

        internal Rectangle rect
        {
            get
            {
                float scale = Camera.scale;
                return new Rectangle((int)((gameObject.position.X) * scale), (int)((gameObject.position.Y) * scale), (int)(gameObject.size.X * scale), (int)(gameObject.size.Y * -scale));
            }
        }

        internal abstract void Draw(SpriteBatch spriteBatch);

        internal override void EnableComponent()
        {
            Scene.EnableComponent(this);
        }

        internal override void DisableComponent()
        {
            Scene.DisableComponent(this);
        }

        public override void OnInspectorGUI()
        {
            string id = this.GetType().Name + componetID.ToString();

            bool flipX = this.flipX;
            bool flipY = this.flipY;
            ImGui.Text("Flip");
            ImGui.SameLine();
            if (ImGui.Checkbox("##flipx" + id, ref flipX))
            {
                Undo.RegisterAction(this, this.flipX, flipX, nameof(Renderer.flipX));
                this.flipX = flipX;
            }
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if (ImGui.Checkbox("##flipy" + id, ref flipY))
            {
                Undo.RegisterAction(this, this.flipY, flipY, nameof(Renderer.flipY));
                this.flipY = flipY;
            }
            ImGui.SameLine();
            ImGui.Text("Y");

            int sortingOrder = this.sortingOrder;
            ImGui.Text("Sorting Order");
            ImGui.SameLine();
            if (ImGui.DragInt("##order" + id, ref sortingOrder))
            {
                Undo.RegisterAction(this, this.sortingOrder, sortingOrder, nameof(Renderer.sortingOrder));
                this.sortingOrder = sortingOrder;
            }

            Vector4 color = this.color.ToVector4();
            System.Numerics.Vector4 numColor = new System.Numerics.Vector4(color.X, color.Y, color.Z, color.W);
            ImGui.Text("Color");
            ImGui.SameLine();
            if (ImGui.ColorPicker4("##color" + id, ref numColor))
            {
                Color temp = new Color(numColor.X, numColor.Y, numColor.Z, numColor.W);
                Undo.RegisterAction(this, this.color, temp, nameof(Renderer.color));
                this.color = temp;
            }
        }

    }
}
