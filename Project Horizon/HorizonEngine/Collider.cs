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
    public abstract class Collider : Component
    {
        [JsonIgnore]
        private PhysicsMaterial _physicsMaterial;
        private uint _assetID;
        [JsonIgnore]
        private TransformMatrix _transform;
        [JsonIgnore]
        private Rigidbody _attachedRigidbody;
        [JsonIgnore]
        private AABB _aabb;
        private bool _isTrigger = false;

        public PhysicsMaterial physicsMaterial
        {
            get
            {
                return _physicsMaterial;
            }
            set
            {
                _physicsMaterial = value;
                _assetID = value == null ? 0 : _physicsMaterial.assetID;
            }
        }

        public bool isTrigger
        {
            get
            {
                return _isTrigger;
            }
            set
            {
                _isTrigger = value;
            }
        }
        
        internal TransformMatrix transformMatrix
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
            }
        }

        public Rigidbody attachedRigidbody
        {
            get
            {
                return _attachedRigidbody;
            }
            internal set
            {
                _attachedRigidbody = value;
            }
        }

        internal AABB aabb
        {
            get
            {
                return _aabb;
            }
            set
            {
                _aabb = value;
            }
        }

        internal abstract void UpdateCollider();

        internal override void EnableComponent()
        {
            Scene.EnableComponent(this);
        }

        internal override void DisableComponent()
        {
            Scene.DisableComponent(this);
        }

        public override void OnLoad()
        {
            physicsMaterial = Assets.GetPhysicsMaterial(_assetID);
        }

        public override void OnInspectorGUI()
        {
            string id = this.GetType().Name + componetID.ToString();

            bool isTrigger = this.isTrigger;
            ImGui.Text("Is Trigger");
            ImGui.SameLine();
            if (ImGui.Checkbox("##isTrigger" + id, ref isTrigger))
            {
                Undo.RegisterAction(this, this.isTrigger, isTrigger, nameof(Collider.isTrigger));
                this.isTrigger = isTrigger;
            }

            string physicsMaterialName = _physicsMaterial == null ? "None" : _physicsMaterial.name;
            ImGui.Text("Physics Material");
            ImGui.SameLine();
            ImGui.Text(physicsMaterialName);
            //ImGui.SameLine();
            if (ImGui.Button("Select Physics Material"))
            {
                ImGui.OpenPopup("select_physics_material");
                SearchBar.Clear();
            }

            if (ImGui.BeginPopup("select_physics_material"))
            {
                SearchBar.Draw();

                if (ImGui.Selectable("None"))
                {
                    Undo.RegisterAction(this, this.physicsMaterial, null, nameof(Collider.physicsMaterial));
                    this.physicsMaterial = null;
                }

                foreach (PhysicsMaterial physicsMaterial in Assets.physicsMaterials)
                {
                    if (SearchBar.PassFilter(physicsMaterial.name) && ImGui.Selectable(physicsMaterial.name))
                    {
                        Undo.RegisterAction(this, this.physicsMaterial, physicsMaterial, nameof(Collider.physicsMaterial));
                        this.physicsMaterial = physicsMaterial;
                    }
                }

                ImGui.EndPopup();
            }
        }

    }
}
