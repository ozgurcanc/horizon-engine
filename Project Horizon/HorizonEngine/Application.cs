using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace HorizonEngine
{
    public static class Application
    {
        private static bool _isEditor;

        public static bool isEditor
        {
            get
            {
                return _isEditor;
            }
            internal set
            {
                _isEditor = value;
            }
        }
    }
}
