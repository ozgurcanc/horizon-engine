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
        private Rigidbody _rigidbody;
        
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

        internal Rigidbody rigidbody
        {
            get
            {
                return _rigidbody;
            }
            set
            {
                _rigidbody = value;
            }
        }

        internal abstract void UpdateCollider();

    }
}
