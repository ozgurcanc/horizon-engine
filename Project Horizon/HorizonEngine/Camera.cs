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
    public class Camera
    {
        private GraphicsDeviceManager _graphics;
        private Vector2 _position;
        private float _rotation;
        private float _width;
        private float _height;
        private Vector2 _resolution;
        private Matrix _renderTransform;
        private Matrix _worldToScreen;
        private Matrix _screenToWorld;
        

        internal Camera(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _rotation = 0;
            _width = _height = 10;
            _resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        internal static float scale
        {
            get
            {
                return 1000f;
            }
        }

        internal void Update()
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

        internal Matrix renderTransform
        {
            get
            {
                return _renderTransform;
            }
        }

        public Vector2 position
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

        public float rotation
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

        public float width
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

        public float height
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

        public Vector2 resolution
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

        public Vector2 WorldToScreenPoint(Vector2 position)
        {
            return Vector2.Transform(position, _worldToScreen);
        }

        public Vector2 ScreenToWorldPoint(Vector2 position)
        {
            return Vector2.Transform(position, _screenToWorld);
        }
    }
}
