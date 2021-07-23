using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HorizonEngine
{
    public abstract class Collider : Component
    {
        private TransformMatrix _transform;
        private Rigidbody _attachedRigidbody;
        private AABB _aabb;
        private bool _isTrigger = false;

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

    }
}
