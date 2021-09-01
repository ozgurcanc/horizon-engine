﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace HorizonEngine
{
    public class Animation : Asset
    {
        private HorizonEngine.Texture[] _frames;
        private float _duration;
        private bool _loop;

        internal Animation(string name, float duration, bool loop, HorizonEngine.Texture[] frames) : base(name, null)
        {
            _duration = duration;
            _frames = frames;
            _loop = loop;
        }

        internal HorizonEngine.Texture this[int index]
        {
            get
            {
                return _frames[index];
            }
        }

        internal float duration
        {
            get
            {
                return _duration;
            }
        }

        internal int length
        {
            get
            {
                return _frames.Length;
            }
        }

        internal bool loop
        {
            get
            {
                return _loop;
            }
        }
    }
}
