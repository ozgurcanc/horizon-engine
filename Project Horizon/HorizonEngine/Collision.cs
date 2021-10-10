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
    public class Collision
    {
        private Collider _collider;
        private Vector2 _contactNormal;
        private Vector2 _contactPoint;
        private float _penetration;

        internal Collision(Collider collider, Vector2 contactNormal, Vector2 contactPoint, float penetration)
        {
            _collider = collider;
            _contactNormal = contactNormal;
            _contactPoint = contactPoint;
            _penetration = penetration;
        }

        public Collider collider
        {
            get
            {
                return _collider;
            }
        }

        public Vector2 contactNormal
        {
            get
            {
                return _contactNormal;
            }
        }

        public Vector2 contactPoint
        {
            get
            {
                return _contactPoint;
            }
        }

        public float penetration
        {
            get
            {
                return _penetration;
            }
        }
    }
}
