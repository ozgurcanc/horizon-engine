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
    static class Physics
    {
        private static Vector2 _gravity;

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
    }


    public enum ForceMode
    {
        Force,
        Impulse
    }
}
