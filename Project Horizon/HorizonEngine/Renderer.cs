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
    public abstract class Renderer : Component
    {
        private Color _color;
        private int _flipState;
        private float _sortingOrder;
        public Renderer()
        {
            _color = Color.White;
            _flipState = 0;
            _sortingOrder = 0f;
        }

        public bool flipX
        {
            get
            {
                return _flipState % 2 == 1;
            }
            set
            {
                if (value) _flipState |= 1;
                else _flipState &= ~1;
            }
        }

        public bool flipY
        {
            get
            {
                return _flipState >= 2;
            }
            set
            {
                if (value) _flipState |= 2;
                else _flipState &= ~2;
            }
        }

        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public int sortingOrder
        {
            get
            {
                return (int)(_sortingOrder * Camera.maxSortOrder);
            }
            set
            {
                _sortingOrder = value / (float)Camera.maxSortOrder;
            }
        }

        internal float layerDepth
        {
            get
            {
                return _sortingOrder;
            }
        }
        
        internal int flipState
        {
            get
            {
                return _flipState;
            }
        }

        internal Rectangle rect
        {
            get
            {
                float scale = Camera.scale;
                return new Rectangle((int)((gameObject.position.X) * scale), (int)((gameObject.position.Y) * scale), (int)(gameObject.size.X * scale), (int)(gameObject.size.Y * -scale));
            }
        }

        internal abstract void Draw(SpriteBatch spriteBatch);


    }
}
