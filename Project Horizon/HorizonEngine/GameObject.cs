using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HorizonEngine
{
    public class GameObject
    {
        private string _name;
        private Vector2 _position;
        private GameObject _parent;
        private List<GameObject> _childs;
        private Vector2 _size;
        private List<Component> _components;

        internal GameObject(string name = "GameObject")
        {
            _childs = new List<GameObject>();
            _components = new List<Component>();
            _parent = null;
            _name = name;
        }

        public Vector2 position
        {
            get
            {
                return _position;
            }
            set
            {
                Vector2 change = value - _position;
                _position = value;
                _childs.ForEach(x => x.position += change);
            }
        }

        public Vector2 size
        {
            get
            {
                return _size;
            }
            set
            {
                Vector2 change = new Vector2(value.X / _size.X, value.Y / size.Y);
                _size = value;
                _childs.ForEach(x => { x.size *= change; x.position = _position + (x.position - _position) * change; });
            }
        }

        public GameObject parent
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

        public string name
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

        internal Rectangle rect
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, (int)size.X, (int)size.Y);
            }
        }

        public T AddComponent<T>() where T : Component, new()
        {
            Component component = new T();
            component.gameObject = this;
            _components.Add(component);

            Scene.EnableComponent(component);

            return (T)component;
        }
    }
}
