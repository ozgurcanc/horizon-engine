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
    public static class Physics
    {
        private static Vector2 _gravity;
        private static int[] _ignoreMask;

        static Physics()
        {
            _ignoreMask = new int[32];
        }

        public static Vector2 gravity
        {
            get
            {
                return _gravity;
            }
            set
            {
                _gravity = value;
            }
        }

        internal static int[] ignoreMask
        {
            get
            {
                return _ignoreMask;
            }
        }

        public static void IgnoreLayerCollision(Layer layer1, Layer layer2, bool ignore = true)
        {
            if(ignore)
            {
                _ignoreMask[(int)layer1] |= 1 << (int)layer2;
                _ignoreMask[(int)layer2] |= 1 << (int)layer1;
            }
            else
            {
                _ignoreMask[(int)layer1] &= ~(1 << (int)layer2);
                _ignoreMask[(int)layer2] &= ~(1 << (int)layer1);
            }
        }
    }


    public enum ForceMode
    {
        Force,
        Impulse
    }
}
