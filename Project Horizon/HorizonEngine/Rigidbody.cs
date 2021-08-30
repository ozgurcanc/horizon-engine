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
    public class Rigidbody : Component
    {
        private Vector2 _velocity;
        private float _angularVelocity;
        private Vector2 _forceAccumulator;
        private float _torqueAccumulator;
        private float _inverseMass;
        private float _inverseInertia;
        private float _linearDrag;
        private float _angularDrag;
        private float _gravityScale;

        public Rigidbody()
        {
            _velocity = _forceAccumulator = Vector2.Zero;
            _angularVelocity = _torqueAccumulator = _linearDrag = _angularDrag = 0f;
            _inverseMass = _inverseInertia = _gravityScale = 1f;
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
                gameObject.rotation = value;
            }
        }

        public Vector2 velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
            }
        }

        public float angularVelocity
        {
            get
            {
                return _angularVelocity;
            }
            set
            {
                _angularVelocity = value;
            }
        }

        public float gravityScale
        {
            get
            {
                return _gravityScale;
            }
            set
            {
                _gravityScale = value;
            }
        }   

        public float linearDrag
        {
            get
            {
                return _linearDrag;
            }
            set
            {
                _linearDrag = value;
            }
        }

        public float angularDrag
        {
            get
            {
                return _angularDrag;
            }
            set
            {
                _angularDrag = value;
            }
        }

        public float mass
        {
            get
            {
                return _inverseMass == 0 ? 0 : 1 / _inverseMass;
            }
            set
            {
                _inverseMass = value == 0 ? 0 : 1 / value;
            }
        }

        public float inertia
        {
            get
            {
                return _inverseInertia == 0 ? 0 : 1 / _inverseInertia;
            }
            set
            {
                _inverseInertia = value == 0 ? 0 : 1 / value;
            }
        }

        internal float inverseMass
        {
            get
            {
                return _inverseMass;
            }
        }

        internal float inverseInertia
        {
            get
            {
                return _inverseInertia;
            }
        }

        internal void UpdatePhysics(float deltaTime)
        {
            Vector2 linearAcceleration = Physics.gravity * _gravityScale;
            linearAcceleration += _forceAccumulator * _inverseMass;
            float angularAcceleration = _torqueAccumulator * _inverseInertia;

            _velocity += linearAcceleration * deltaTime;
            _angularVelocity += angularAcceleration * deltaTime;

            float linearDamping = (1 - _linearDrag * deltaTime);
            float angularDamping = (1 - _angularDrag * deltaTime);
            if (linearDamping < 0) linearDamping = 0;
            if (angularDamping < 0) angularDamping = 0;
            _velocity *= linearDamping;
            _angularVelocity *= angularDamping;

            

            gameObject.position += _velocity * deltaTime;
            gameObject.rotation += _angularVelocity * deltaTime;

            _forceAccumulator = Vector2.Zero;
            _torqueAccumulator = 0f;
        }

        public void AddForce(Vector2 force, ForceMode forceMode = ForceMode.Force)
        {
            if (forceMode == ForceMode.Force)
                _forceAccumulator += force;
            else
                _velocity += force * _inverseMass;
        }

        public void AddTorque(float torque, ForceMode forceMode = ForceMode.Force)
        {
            if (forceMode == ForceMode.Force)
                _torqueAccumulator += torque;
            else
                _angularVelocity += torque * _inverseInertia;
        }

        public void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode forceMode = ForceMode.Force)
        {
            Vector2 rel = position - gameObject.position;
            float torque = rel.X * force.Y - rel.Y * force.X;
            if(forceMode == ForceMode.Force)
            {
                _forceAccumulator += force;
                _torqueAccumulator += torque;
            }
            else
            {
                _velocity += force * _inverseMass;
                _angularVelocity += torque * _inverseInertia;
            }
           
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Rigidbody")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                this.enabled = enabled;
            }

            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            Vector2 velocity = this.velocity;
            ImGui.Text("Velocity");
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if (ImGui.DragFloat("##velocityX" + id, ref velocity.X))
            {
                this.velocity = velocity;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if (ImGui.DragFloat("##velocityY" + id, ref velocity.Y))
            {
                this.velocity = velocity;
            }

            float angularVelocity = this.angularVelocity;
            ImGui.Text("Angular Velocity");
            ImGui.SameLine();
            if (ImGui.DragFloat("##angularVel" + id, ref angularVelocity))
            {
                this.angularVelocity = angularVelocity;
            }

            float linearDrag = this.linearDrag;
            ImGui.Text("Linear Drag");
            ImGui.SameLine();
            if(ImGui.DragFloat("##linearDrag" + id, ref linearDrag))
            {
                this.linearDrag = linearDrag;
            }

            float angularDrag = this.angularDrag;
            ImGui.Text("Angular Drag");
            ImGui.SameLine();
            if (ImGui.DragFloat("##angularDrag" + id, ref angularDrag))
            {
                this.angularDrag = angularDrag;
            }

            float mass = this.mass;
            ImGui.Text("Mass");
            ImGui.SameLine();
            if(ImGui.DragFloat("##mass" + id, ref mass))
            {
                this.mass = mass;
            }

            float inertia = this.inertia;
            ImGui.Text("Inertia");
            ImGui.SameLine();
            if (ImGui.DragFloat("##inertia" + id, ref inertia))
            {
                this.inertia = inertia;
            }

            float gravityScale = this.gravityScale;
            ImGui.Text("Gravity Scale");
            ImGui.SameLine();
            if (ImGui.DragFloat("##gravityScale" + id, ref gravityScale))
            {
                this.gravityScale = gravityScale;
            }

            ImGui.PopItemWidth();

        }

    }
}
