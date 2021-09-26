using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Newtonsoft.Json;
using ImGuiNET;
using System.Linq;
using System.Reflection;

namespace HorizonEngine
{
    [JsonObject(MemberSerialization.Fields)]
    public class GameObject : IEditor
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
            //_behaviours.Clear();
            if(activeInHierarchy)
            {
                foreach (Component component in _components.Values)
                {
                    if (component.enabled) Scene.DisableComponent(component);
                }
            }
            //_components.Clear();
            foreach (GameObject child in _childs.ToArray()) Scene.DestroyInternal(child);
            //_childs.Clear();
            //parent = null;
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

        internal void UndoComponent(Component component)
        {
            Debug.Assert(_components.ContainsKey(component.GetType()) == false);
            component.gameObject = this;
            _components.Add(component.GetType(), component);
            if (component is Behaviour) _behaviours.Add((Behaviour)component);
            if (_activeInHierarchy && component.enabled) Scene.EnableComponent(component);
        }

        public T AddComponent<T>() where T : Component, new()
        {
            Debug.Assert(_components.ContainsKey(typeof(T)) == false);
            Component component = new T();
            component.gameObject = this;
            _components.Add(typeof(T), component);
            if (component is Behaviour) _behaviours.Add((Behaviour)component);

            if(activeInHierarchy) Scene.EnableComponent(component);

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
                if (activeInHierarchy && component.enabled) Scene.DisableComponent(component);
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

        public void OnInspectorGUI()
        {
            //if (!ImGui.CollapsingHeader("Game Object")) return;
            ImGui.Text("Game Object");

            string name = this.name;
            ImGui.Text("Name");
            ImGui.SameLine();
            if (ImGui.InputText("##name", ref name, 100))
            {
                Undo.RegisterAction(this, this.name, name, nameof(GameObject.name));
                this.name = name;
            }

            bool activeSelf = this.activeSelf;
            ImGui.Text("Active");
            ImGui.SameLine();
            if (ImGui.Checkbox("##active", ref activeSelf))
            {
                Undo.RegisterAction(this, this.activeSelf, activeSelf, nameof(GameObject.activeSelf));
                this.activeSelf = activeSelf;
            }

            int layer = (int)this.layer;
            string[] layers = Enum.GetNames(typeof(Layer));
            ImGui.Text("Layer");
            ImGui.SameLine();
            if (ImGui.Combo("##Layer", ref layer, layers, layers.Length))
            {
                Undo.RegisterAction(this, this.layer, (Layer)layer, nameof(GameObject.layer));
                this.layer = (Layer)layer;
            }


            ImGui.Separator();
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            Vector2 position = this.position;
            ImGui.Text("Position");
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if (ImGui.DragFloat("##PositionX", ref position.X))
            {
                Undo.RegisterAction(this, this.position, position, nameof(GameObject.position));
                this.position = position;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if (ImGui.DragFloat("##PositionY", ref position.Y))
            {
                Undo.RegisterAction(this, this.position, position, nameof(GameObject.position));
                this.position = position;
            }

            float rotation = this.rotation;
            ImGui.Text("Rotation");
            ImGui.SameLine();
            if (ImGui.DragFloat("##Rotation", ref rotation))
            {
                Undo.RegisterAction(this, this.rotation, rotation, nameof(GameObject.rotation));
                this.rotation = rotation;
            }

            Vector2 size = this.size;
            ImGui.Text("Size");
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if (ImGui.DragFloat("##SizeX", ref size.X))
            {
                Undo.RegisterAction(this, this.size, size, nameof(GameObject.size));
                this.size = size;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if (ImGui.DragFloat("##SizeY", ref size.Y))
            {
                Undo.RegisterAction(this, this.size, size, nameof(GameObject.size));
                this.size = size;
            }

            ImGui.PopItemWidth();
            ImGui.Separator();

            foreach(Component component in _components.Values)
            {
                component.OnInspectorGUI();
            }

            ImGui.Separator();

            if(ImGui.Button("Add Component"))
            {
                ImGui.OpenPopup("add_component");
                SearchBar.Clear();
            }

            if(ImGui.BeginPopup("add_component"))
            {
                var components = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Component)) && !type.IsAbstract);

                SearchBar.Draw();

                foreach(Type component in components)
                {
                    if(SearchBar.PassFilter(component.Name) && ImGui.Selectable(component.Name))
                    {
                        MethodInfo method = typeof(GameObject).GetMethod(nameof(GameObject.AddComponent));
                        MethodInfo generic = method.MakeGenericMethod(component);
                        var x = generic.Invoke(this, null);
                        Undo.RegisterAction((Component)x, true);
                    }
                }

                ImGui.EndPopup();
            }

            ImGui.Separator();

            if(ImGui.Button("Remove Component"))
            {
                ImGui.OpenPopup("remove_component");
                SearchBar.Clear();
            }

            if(ImGui.BeginPopup("remove_component"))
            {
                var components = _components.Values;
                SearchBar.Draw();

                foreach (Component component in components)
                {
                    if(SearchBar.PassFilter(component.GetType().Name) && ImGui.Selectable(component.GetType().Name))
                    {
                        Undo.RegisterAction(component, false);
                        MethodInfo method = typeof(GameObject).GetMethod(nameof(GameObject.RemoveComponent));
                        MethodInfo generic = method.MakeGenericMethod(component.GetType());
                        generic.Invoke(this, null);
                    }
                }

                ImGui.EndPopup();
            }
        }
    }
}
