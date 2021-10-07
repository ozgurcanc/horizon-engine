using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;

namespace HorizonEngine
{
    public class BoxCollider : Collider
    {
        private Vector2 _halfSize;

        public Vector2 halfSize
        {
            get
            {
                return _halfSize;
            }
        }

        internal override void UpdateCollider()
        {
            transformMatrix = new TransformMatrix(gameObject.position, gameObject.rotation);
            attachedRigidbody = gameObject.GetComponent<Rigidbody>();
            _halfSize = gameObject.size / 2;
            aabb = new AABB(transformMatrix.TransformPoint(halfSize), transformMatrix.TransformPoint(new Vector2(halfSize.X, -halfSize.Y)),
                transformMatrix.TransformPoint(new Vector2(-halfSize.X, halfSize.Y)), transformMatrix.TransformPoint(new Vector2(-halfSize.X, -halfSize.Y)));
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Box Collider")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                Undo.RegisterAction(this, this.enabled, enabled, nameof(BoxCollider.enabled));
                this.enabled = enabled;
            }

            base.OnInspectorGUI();
        }
    }
}
