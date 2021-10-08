using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.IO;
using ImGuiNET;

namespace HorizonEngine
{
    public class PhysicsMaterial : Asset
    {
        private float _friction;
        private float _restitution;

        internal PhysicsMaterial(string name) : base(name, null)
        {
            _friction = 0f;
            _restitution = 0f;
        }

        public float friction
        {
            get
            {
                return _friction;
            }
            internal set
            {
                _friction = MathHelper.Clamp(value, 0f, 1f);
                Assets.SetModified();
            }
        }

        public float restitution
        {
            get
            {
                return _restitution;
            }
            internal set
            {
                _restitution = MathHelper.Clamp(value, 0f, 1f);
                Assets.SetModified();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            float friction = this.friction;
            ImGui.Text("Friction");
            ImGui.SameLine();
            if (ImGui.SliderFloat("##friction", ref friction, 0f, 1f))
            {
                this.friction = friction;
            }

            float restitution = this.restitution;
            ImGui.Text("Restitution");
            ImGui.SameLine();
            if (ImGui.SliderFloat("##restitution", ref restitution, 0f, 1f))
            {
                this.restitution = restitution;
            }

            ImGui.PopItemWidth();
        }

        internal override void Reload()
        {
            Assets.Load(this);
        }

        internal override void Delete()
        {
            Assets.Delete(this);
        }      
    }
}
