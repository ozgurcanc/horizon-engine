using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HorizonEngine
{
    class GameObject
    {
        private string _name;
        private Vector2 _position;
        private GameObject _parent;
        private List<GameObject> _childs;
        private Vector2 _size;

        public GameObject(string name = "GameObject")
        {
            _childs = new List<GameObject>();
            _parent = null;
            _name = name;
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                Vector2 change = value - _position;
                _position = value;
                _childs.ForEach(x => x.Position += change);
            }
        }

        public Vector2 Size
        {
            get
            {
                return _size;
            }
            set
            {
                Vector2 change = new Vector2(value.X / _size.X, value.Y / Size.Y);
                _size = value;
                _childs.ForEach(x => { x.Size *= change; x.Position = _position + (x.Position - _position) * change; });
            }
        }

        public GameObject Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null)
                {
                    _parent._childs.Remove(this);
                }
                _parent = value;
                _parent._childs.Add(this);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
