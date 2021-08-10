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
    public static class Camera
    {
        private static GraphicsDeviceManager _graphics;
        private static Vector2 _position;
        private static float _rotation;
        private static float _width;
        private static float _height;
        private static Vector2 _resolution;
        private static Matrix _renderTransform;
        private static Matrix _worldToScreen;
        private static Matrix _screenToWorld;
        private static int _cullingMask;
        private static int _maxSortOrder;

        internal static void InitCamera(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _rotation = 0;
            _width = _height = 10;
            _resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _cullingMask = 0;
            _maxSortOrder = 1000;
        }

        internal static float scale
        {
            get
            {
                return 1000f;
            }
        }

        internal static int maxSortOrder
        {
            get
            {
                return _maxSortOrder;
            }
        }

        internal static void Update()
        {
            Matrix m = Matrix.CreateTranslation(-_position.X * scale, -_position.Y * scale, 0);
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(-_rotation));
            m *= Matrix.CreateScale(_resolution.X / (_width * scale), -_resolution.Y / (_height * scale), 1);
            m *= Matrix.CreateTranslation(_resolution.X / 2, _resolution.Y / 2, 0);
            _renderTransform = m;

            m = Matrix.CreateTranslation(-_position.X, -_position.Y, 0);
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(-_rotation));
            m *= Matrix.CreateScale(_resolution.X / _width, -_resolution.Y / _height, 1);
            m *= Matrix.CreateTranslation(_resolution.X / 2, _resolution.Y / 2, 0);
            _worldToScreen = m;

            _screenToWorld = Matrix.Invert(m);
        }

        internal static Matrix renderTransform
        {
            get
            {
                return _renderTransform;
            }
        }

        public static Vector2 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public static float rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value % 360f;
            }
        }

        public static float width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public static float height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public static Vector2 resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                _resolution = value;
                _graphics.PreferredBackBufferWidth = (int)value.X;
                _graphics.PreferredBackBufferHeight = (int)value.Y;
                _graphics.ApplyChanges();

            }
        }

        internal static int cullingMask
        {
            get
            {
                return _cullingMask;
            }
        }

        public static Vector2 WorldToScreenPoint(Vector2 position)
        {
            return Vector2.Transform(position, _worldToScreen);
        }

        public static Vector2 ScreenToWorldPoint(Vector2 position)
        {
            return Vector2.Transform(position, _screenToWorld);
        }

        public static void CullLayer(Layer layer, bool cull = true)
        {
            if (cull) _cullingMask |= 1 << (int)layer;
            else _cullingMask &= ~(1 << (int)layer);
        }
    }
}
