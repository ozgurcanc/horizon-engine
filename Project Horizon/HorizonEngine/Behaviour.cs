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
    public abstract class Behaviour : Component
    {
        public virtual void Update() { }

        public virtual void Start() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        public virtual void OnTriggerEnter(Collider collider) { }

        public virtual void OnTriggerStay(Collider collider) { }

        public virtual void OnTriggerExit(Collider collider) { }

        public virtual void OnCollisionEnter(Collision collision) { }

        public virtual void OnCollisionStay(Collision collision) { }

        public virtual void OnCollisionExit(Collider collider) { }

        public virtual void OnMouseEnter() { }

        public virtual void OnMouseExit() { }

        public virtual void OnMouseOver() { }

        public virtual void OnMouseDown() { }

        public virtual void OnMouseDrag() { }

        public virtual void OnMouseUp() { }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader(this.GetType().Name)) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                this.enabled = enabled;
            }
        }

    }
}
