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
    public static class Time
    {
        private static float _deltaTime;
        private static int _fps;
        private static int _frameCount;
        private static float _timeCounter;       

        internal static void Update(GameTime gameTime)
        {
            _deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCount++;
            _timeCounter += _deltaTime;
            if (_timeCounter >= 1.0f)
            {
                _fps = _frameCount;
                _frameCount = 0;
                _timeCounter = 0f;
            }
        }

        public static float deltaTime
        {
            get
            {
                return _deltaTime;
            }
        }

        public static int fps
        {
            get
            {
                return _fps;
            }
        }
    }
}
