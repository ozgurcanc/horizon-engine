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
    public class AABB
    {
        private Vector2 _min;
        private Vector2 _max;

        public AABB()
        {
            _min = _max = Vector2.Zero;
        }

        public AABB(Vector2 position)
        {
            _min = position;
            _max = position;
        }

        public AABB(Vector2 position1, Vector2 position2)
        {
            _min = Vector2.Min(position1, position2);
            _max = Vector2.Max(position1, position2);
        }


        public Vector2 center
        {
            get
            {
                return (_min + _max) * 0.5f;
            }
        }

        public Vector2 min
        {
            get
            {
                return _min;
            }
        }

        public Vector2 max
        {
            get
            {
                return _max;
            }
        }

        public static AABB Union(AABB aabb1, AABB aabb2)
        {
            AABB union = new AABB();
            union._min = Vector2.Min(aabb1._min, aabb2._min);
            union._max = Vector2.Max(aabb1._max, aabb2._max);
            return union;
        }

        public static AABB Union(AABB aabb, Vector2 position)
        {
            AABB union = new AABB();
            union._min = Vector2.Min(aabb._min, position);
            union._max = Vector2.Max(aabb._max, position);
            return union;
        }

        public static bool Intersect(AABB aabb1, AABB aabb2)
        {
            if (aabb1._min.X > aabb2._max.X || aabb2._min.X > aabb1._max.X)
                return false;
            if (aabb1._min.Y > aabb2._max.Y || aabb2._min.Y > aabb1._max.Y)
                return false;

            return true;
        }

        public int MaximumExtent()
        {
            Vector2 diagonal = _max - _min;
            if (diagonal.X > diagonal.Y)
                return 0;
            return 1;
        }

    }
}
