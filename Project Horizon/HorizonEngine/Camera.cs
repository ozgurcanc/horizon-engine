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
    public class Camera : Component
    {
        private static Camera _main;
        private float _width;
        private float _height;
        //private Vector2 _resolution;
        private Matrix _renderTransform;
        private Matrix _worldToScreen;
        private Matrix _screenToWorld;
        private int _cullingMask;      
        private Color _clearColor;
        private RenderTarget2D _renderTarget;

        public Camera()
        {
            _width = _height = 10;
            //_resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _cullingMask = 0;
            _clearColor = Color.CornflowerBlue;
            _renderTarget = null;
            if (_main == null) _main = this;
        }

        public static Camera main
        {
            get
            {
                return _main;
            }
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
                return 1000;
            }
        }

        internal void Update()
        {
            Vector2 resolution = this.resolution;
            Matrix m = Matrix.CreateTranslation(-gameObject.position.X * scale, -gameObject.position.Y * scale, 0);
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(-gameObject.rotation));
            m *= Matrix.CreateScale(resolution.X / (_width * scale), -resolution.Y / (_height * scale), 1);
            m *= Matrix.CreateTranslation(resolution.X / 2, resolution.Y / 2, 0);
            _renderTransform = m;

            m = Matrix.CreateTranslation(-gameObject.position.X, -gameObject.position.Y, 0);
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(-gameObject.rotation));
            m *= Matrix.CreateScale(resolution.X / _width, -resolution.Y / _height, 1);
            m *= Matrix.CreateTranslation(resolution.X / 2, resolution.Y / 2, 0);
            _worldToScreen = m;

            _screenToWorld = Matrix.Invert(m);
        }

        internal void PreRender()
        {
            Screen.SetRenderTarget(_renderTarget);
            Screen.Clear(_clearColor);
        }

        internal Matrix renderTransform
        {
            get
            {
                return _renderTransform;
            }
        }

        public Color clearColor
        {
            get
            {
                return _clearColor;
            }
            set
            {
                _clearColor = value;
            }
        }

        public Vector2 position
        {
            get
            {
                return gameObject.position;
            }
            set
            {
                gameObject.position = value;
            }
        }

        public float rotation
        {
            get
            {
                return gameObject.rotation;
            }
            set
            {
                gameObject.rotation = value % 360f;
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
                return _renderTarget == null ? Screen.resolution : new Vector2(_renderTarget.Width, _renderTarget.Height);
            }
        }

        internal int cullingMask
        {
            get
            {
                return _cullingMask;
            }
        }

        internal RenderTarget2D renderTarget
        {
            get
            {
                return _renderTarget;
            }
            set
            {
                _renderTarget = value;
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

        public void CullLayer(Layer layer, bool cull = true)
        {
            if (cull) _cullingMask |= 1 << (int)layer;
            else _cullingMask &= ~(1 << (int)layer);
        }
    }
}
