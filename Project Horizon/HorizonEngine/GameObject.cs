using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Newtonsoft.Json;

namespace HorizonEngine
{
    [JsonObject(MemberSerialization.Fields)]
    public class GameObject
    {
        private int _gameObjectID;
        private bool _dontDestroyOnLoad;
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
            _size = new Vector2(1, 1);
            _dontDestroyOnLoad = false;
        }

        internal int gameObjectID
        {
            get
            {
                return _gameObjectID;
            }
            set
            {
                _gameObjectID = value;
            }
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

        public int childCount
        {
            get
            {
                return _childs.Count;
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
                var temp = value;
                while(temp != null) 
                {
                    if (temp == this) return;
                    temp = temp.parent;
                }
                if (_parent != null) _parent._childs.Remove(this);
                _parent = value;
                if(value != null ) _parent._childs.Add(this);
                UpdateHierarchy(this);
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

        internal bool dontDestroyOnLoad
        {
            get
            {
                return _dontDestroyOnLoad;
            }
        }

        internal IList<Behaviour> behaviours
        {
            get
            {
                return _behaviours.AsReadOnly();
            }
        }

        internal void Destroy()
        {
            _behaviours.Clear();
            foreach (Component component in _components.Values) component.enabled = false;
            _components.Clear();
            foreach (GameObject child in _childs.ToArray()) Scene.Destroy(child);
            _childs.Clear();
            parent = null;
        }

        internal void OnLoad()
        {
            Scene.Register(this);

            foreach (Component component in _components.Values)
            {
                component.OnLoad();
                if (activeInHierarchy && component.enabled) Scene.EnableComponent(component);
            }                    

            foreach (GameObject child in _childs) child.OnLoad();
        }

        internal GameObject Clone()
        {
            GameObject clone = (GameObject)this.MemberwiseClone();
            clone._behaviours = new List<Behaviour>();
            clone._components = new Dictionary<Type, Component>();
            clone._childs = new List<GameObject>();           
            
            foreach (Component component in _components.Values)
            {
                Component temp = component.Clone();
                temp.gameObject = clone;
                clone._components.Add(temp.GetType(), temp);
                if (temp is Behaviour) clone._behaviours.Add((Behaviour)temp);
                if (temp.enabled && clone.activeInHierarchy) Scene.EnableComponent(temp);
            }

            foreach (GameObject child in _childs) Scene.Clone(child).parent = clone;

            return clone;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            Debug.Assert(_components.ContainsKey(typeof(T)) == false);
            Component component = new T();
            component.gameObject = this;
            _components.Add(typeof(T), component);
            if (component is Behaviour) _behaviours.Add((Behaviour)component);

            if(_activeInHierarchy) Scene.EnableComponent(component);

            return (T)component;
        }

        public void RemoveComponent<T>() where T : Component, new()
        {
            Type type = typeof(T);
            if(_components.ContainsKey(type))
            {
                Component component = _components[type];
                if (component is Behaviour) _behaviours.Remove((Behaviour)component);
                _components.Remove(type);
                component.enabled = false;
            }
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

        public GameObject GetChild(int index)
        {
            if (index < childCount)
                return _childs[index];

            return null;
        }

        public GameObject GetChild(string name)
        {
            foreach(GameObject child in _childs)
            {
                if(child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        public void DetachChildren()
        {
            foreach (GameObject child in _childs.ToArray())
                child.parent = null;
        }

        public bool IsChildOf(GameObject parent)
        {
            GameObject child = this;
            while(child != null)
            {
                if (child == parent) return true;
                child = child.parent;
            }

            return false;
        }

        public void DontDestroyOnLoad()
        {
            _dontDestroyOnLoad = true;
        }
    }
}
