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
    public static class Random
    {
        private static System.Random _random;

        static Random()
        {
            _random = new System.Random();
        }

        public static int Range(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }

        public static float Range(float minInclusive, float maxExclusive)
        {
            return (float)_random.NextDouble() * (maxExclusive - minInclusive) + minInclusive;
        }
    }
}
