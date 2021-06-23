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
    public class BoxCollider : Collider
    {
        private Vector2 _halfSize;

        public Vector2 halfSize
        {
            get
            {
                return _halfSize;
            }
        }

        internal override void UpdateCollider()
        {
            transformMatrix = new TransformMatrix(gameObject.position, gameObject.rotation);
            rigidbody = gameObject.GetComponent<Rigidbody>();
            _halfSize = gameObject.size / 2;
        }
    }
}
