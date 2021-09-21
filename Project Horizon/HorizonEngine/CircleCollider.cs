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
    public class CircleCollider : Collider
    {
        private float _radius;

        public CircleCollider()
        {
            _radius = 1;
        }

        public float radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }

        internal override void UpdateCollider()
        {
            transformMatrix = new TransformMatrix(gameObject.position, gameObject.rotation);
            attachedRigidbody = gameObject.GetComponent<Rigidbody>();
            Vector2 r = new Vector2(radius, radius);
            aabb = new AABB(gameObject.position + r, gameObject.position - r);
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Circle Collider")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                Undo.RegisterAction(this, this.enabled, enabled, nameof(CircleCollider.enabled));
                this.enabled = enabled;
            }

            base.OnInspectorGUI();

            float radius = this.radius;
            ImGui.Text("Radius");
            ImGui.SameLine();
            if(ImGui.DragFloat("##radius" + id, ref radius))
            {
                Undo.RegisterAction(this, this.radius, radius, nameof(CircleCollider.radius));
                this.radius = radius;
            }
        }
    }
}
