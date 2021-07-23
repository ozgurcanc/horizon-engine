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
        public Collider collider;
        public Vector2 contactNormal;
        public Vector2 contactPoint;
        public float penetration;
    }
}
