using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

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

    }
}
