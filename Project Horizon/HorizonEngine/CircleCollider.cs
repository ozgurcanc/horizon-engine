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
    }
}
