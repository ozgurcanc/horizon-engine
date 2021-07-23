using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace HorizonEngine
{
    public class GameObject
    {
        private bool _activeSelf;
        private bool _activeInHierarchy;
        private string _name;
        private Vector2 _position;
        private float _rotation;
        private GameObject _parent;
        private List<GameObject> _childs;
        private Vector2 _size;
        private Dictionary<Type, Component> _components;
        private Layer _layer;
        private List<Behaviour> _behaviours;

        internal GameObject(string name = "GameObject")
        {
            _childs = new List<GameObject>();
            _components = new Dictionary<Type, Component>();
            _behaviours = new List<Behaviour>();
            _parent = null;
            _name = name;
            _activeSelf = _activeInHierarchy = true;
            _layer = 0;
        }

        public bool activeSelf
        {
            get
            {
                return _activeSelf;
            }
            set
            {
                if (_activeSelf == value) return;
                _activeSelf = value;
                UpdateHierarchy(this);
            }
        }


        public bool activeInHierarchy
        {
            get
            {
                return _activeInHierarchy;
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
                Vector2 change = value - _position;
                _position = value;
                _childs.ForEach(x => x.position += change);
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
                if (_parent != null) _parent._childs.Remove(this);
                _parent = value;
                if(value != null ) _parent._childs.Add(this);
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

        public Layer layer
        {
            get
            {
                return _layer;
            }
            set
            {
                _layer = value;
            }
        }

        internal IList<Behaviour> behaviours
        {
            get
            {
                return _behaviours.AsReadOnly();
            }
        }

        public T AddComponent<T>() where T : Component, new()
        {
            Debug.Assert(_components.ContainsKey(typeof(T)) == false);
            Component component = new T();
            component.gameObject = this;
            _components.Add(typeof(T), component);
            if (component is Behaviour) _behaviours.Add((Behaviour)component);

            Scene.EnableComponent(component);

            return (T)component;
        }

        public T GetComponent<T>() where T : Component
        {
            if(_components.ContainsKey(typeof(T)))
            {
                return (T)_components[typeof(T)];
            }

            return null;
        }

        private void UpdateHierarchy(GameObject gameObject)
        {
            bool previous = gameObject._activeInHierarchy;
            gameObject._activeInHierarchy = gameObject._activeSelf;
            if (gameObject.parent != null) gameObject._activeInHierarchy &= gameObject._parent._activeInHierarchy;
            if (previous != gameObject._activeInHierarchy)
            {
                var components = gameObject._components.Values;
                foreach (Component temp in components)
                {
                    if (!temp.enabled) continue;
                    if (gameObject._activeInHierarchy) Scene.EnableComponent(temp);
                    else Scene.DisableComponent(temp);
                }
                gameObject._childs.ForEach(x => UpdateHierarchy(x));
            }
        }
    }
}
