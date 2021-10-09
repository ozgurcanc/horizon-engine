using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace HorizonEngine
{
    public class Scene
    {
        private static Scene _main;
        private string _name;
        private List<GameObject> _gameObjects;
        private List<Renderer> _renderers;
        private List<Behaviour> _behaviours;
        private List<Rigidbody> _rigidbodies;
        private List<Collider> _colliders;
        private HashSet<Tuple<Collider, Collider>> _contactPairs;
        private HashSet<Collider> _mouseOverColliders;
        private Collider[] _mouseClickedColliders;
        private List<Behaviour> _startBehaviours;
        private List<Animator> _animators;
        private List<Camera> _cameras;
        private List<AudioSource> _audioSources;
        private static Tuple<string, List<GameObject>> _cache;

        internal static Scene main
        {
            get
            {
                return _main;
            }
        }

        internal static string name
        {
            get
            {
                return Scene.main._name;
            }
        }

        internal static IList<GameObject> gameObjects
        {
            get
            {
                return main._gameObjects.AsReadOnly();
            }
        }

        internal static void Register(GameObject gameObject)
        {
            gameObject.gameObjectID = Scene.main._gameObjects.Count;
            Scene.main._gameObjects.Add(gameObject);
        }

        internal static void EnableComponent(Renderer component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._renderers.Count;
            scene._renderers.Add(component);
        }

        internal static void EnableComponent(Behaviour component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._behaviours.Count;
            scene._behaviours.Add(component);
            component.OnEnable();
            if (component.startFlag)
            {
                scene._startBehaviours.Add(component);
                component.startFlag = false;
            }
        }

        internal static void EnableComponent(Rigidbody component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._rigidbodies.Count;
            scene._rigidbodies.Add(component);
        }

        internal static void EnableComponent(Collider component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._colliders.Count;
            scene._colliders.Add(component);
        }

        internal static void EnableComponent(Animator component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._animators.Count;
            scene._animators.Add(component);
        }

        internal static void EnableComponent(Camera component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._cameras.Count;
            scene._cameras.Add(component);
        }

        internal static void EnableComponent(AudioSource component)
        {
            Scene scene = Scene.main;
            component.componetID = scene._audioSources.Count;
            scene._audioSources.Add(component);
        }

        internal static void DisableComponent(Renderer component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._renderers.Count - 1;
            Renderer temp = scene._renderers[lastIndex];
            temp.componetID = component.componetID;
            scene._renderers[component.componetID] = temp;
            scene._renderers.RemoveAt(lastIndex);
        }

        internal static void DisableComponent(Behaviour component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._behaviours.Count - 1;
            Behaviour temp = scene._behaviours[lastIndex];
            temp.componetID = component.componetID;
            scene._behaviours[component.componetID] = temp;
            scene._behaviours.RemoveAt(lastIndex);
            (component).OnDisable();
        }

        internal static void DisableComponent(Rigidbody component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._rigidbodies.Count - 1;
            Rigidbody temp = scene._rigidbodies[lastIndex];
            temp.componetID = component.componetID;
            scene._rigidbodies[component.componetID] = temp;
            scene._rigidbodies.RemoveAt(lastIndex);
        }

        internal static void DisableComponent(Collider component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._colliders.Count - 1;
            Collider temp = scene._colliders[lastIndex];
            temp.componetID = component.componetID;
            scene._colliders[component.componetID] = temp;
            scene._colliders.RemoveAt(lastIndex);
        }

        internal static void DisableComponent(Animator component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._animators.Count - 1;
            Animator temp = scene._animators[lastIndex];
            temp.componetID = component.componetID;
            scene._animators[component.componetID] = temp;
            scene._animators.RemoveAt(lastIndex);
        }

        internal static void DisableComponent(Camera component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._cameras.Count - 1;
            Camera temp = scene._cameras[lastIndex];
            temp.componetID = component.componetID;
            scene._cameras[component.componetID] = temp;
            scene._cameras.RemoveAt(lastIndex);
        }

        internal static void DisableComponent(AudioSource component)
        {
            Scene scene = Scene.main;
            int lastIndex = scene._audioSources.Count - 1;
            AudioSource temp = scene._audioSources[lastIndex];
            temp.componetID = component.componetID;
            scene._audioSources[component.componetID] = temp;
            scene._audioSources.RemoveAt(lastIndex);
            (component).Stop();
        }

        internal static void Save()
        {
            //if (Scene.name == null) return;

            List<GameObject> rootGameObjects = Scene.main._gameObjects.FindAll(x => x.parent == null);

            File.WriteAllText(Path.Combine(Application.scenesPath, Scene.main._name), JsonConvert.SerializeObject(rootGameObjects));
        }

        public static void Load(string name)
        {
            Scene scene = Scene.main;
            List<GameObject> dontDestroyOnLoad = scene._gameObjects.FindAll(x => x.dontDestroyOnLoad && x.parent == null);
            scene.Clear();
            scene._name = name;

            List<GameObject> rootGameObjects = JsonConvert.DeserializeObject<List<GameObject>>(File.ReadAllText(Path.Combine(Application.scenesPath, Scene.main._name)));
            rootGameObjects.ForEach(x => x.OnLoad());

            dontDestroyOnLoad.ForEach(x => x.OnLoad());
        }

        internal static void Reload()
        {
            Scene scene = Scene.main;
            List<GameObject> rootGameObjects = scene._gameObjects.FindAll(x => x.parent == null);
            scene.Clear();
            rootGameObjects.ForEach(x => x.OnLoad());
        }

        internal static void NewScene(string name)
        {
            Scene scene = Scene.main;
            scene._name = name;
            scene.Clear();
            Scene.CreateGameObject("Camera").AddComponent<Camera>();
            Save();
        }

        internal static void DeleteScene()
        {
            File.Delete(Path.Combine(Application.scenesPath, Scene.name));
            _main._name = null;
            _main.Clear();
        }

        internal static void Rename(string name)
        {
            string source = Path.Combine(Application.scenesPath, Scene.name);
            string destination = Path.Combine(Application.scenesPath, name);
            File.Move(source, destination);
            _main._name = name;
        }

        internal Scene()
        {
            _gameObjects = new List<GameObject>();
            _renderers = new List<Renderer>();
            _behaviours = new List<Behaviour>();
            _rigidbodies = new List<Rigidbody>();
            _colliders = new List<Collider>();
            _contactPairs = new HashSet<Tuple<Collider, Collider>>();
            _mouseOverColliders = new HashSet<Collider>();
            _startBehaviours = new List<Behaviour>();
            _animators = new List<Animator>();
            _cameras = new List<Camera>();
            _audioSources = new List<AudioSource>();
            _main = this;
        }

        public static Camera mainCamera
        {
            get
            {
                return Scene.main._cameras.Find(x => x.renderTarget == null);
            }
        }       

        public static GameObject CreateGameObject(string name = "GameObject")
        {
            GameObject gameObject = new GameObject(name);
            gameObject.gameObjectID = Scene.main._gameObjects.Count;
            Scene.main._gameObjects.Add(gameObject);
            return gameObject;
        }

        public static GameObject Find(string name)
        {
            foreach(GameObject gameObject in Scene.main._gameObjects)
            {
                if (gameObject.name == name)
                    return gameObject;
            }

            return null;
        }

        public static GameObject Clone(GameObject gameObject)
        {
            GameObject clone = gameObject.Clone();
            clone.parent = null;
            clone.gameObjectID = Scene.main._gameObjects.Count;
            Scene.main._gameObjects.Add(clone);
            return clone;
        }

        public static void Destroy(GameObject gameObject)
        {
            gameObject.parent = null;
            DestroyInternal(gameObject);
        }

        internal static void DestroyInternal(GameObject gameObject)
        {
            int lastIndex = Scene.main._gameObjects.Count - 1;
            GameObject temp = Scene.main._gameObjects[lastIndex];
            temp.gameObjectID = gameObject.gameObjectID;
            Scene.main._gameObjects[gameObject.gameObjectID] = temp;
            Scene.main._gameObjects.RemoveAt(lastIndex);
            gameObject.Destroy();
        }

        internal void Update(GameTime gameTime)
        {
            
            if(Input.IsKeyDown(Keys.H))
            {
                Debug.WriteLine("gameobjects : " + _gameObjects.Count);
                Debug.WriteLine("renderers : " + _renderers.Count);
                Debug.WriteLine("behaviours : " + _behaviours.Count);
                Debug.WriteLine("rigidbodies : " + _rigidbodies.Count);
                Debug.WriteLine("colliders : " + _colliders.Count);
                Debug.WriteLine("animators : " + _animators.Count);
                Debug.WriteLine("camera : " + _cameras.Count);
                Debug.WriteLine("audio sources : " + _audioSources.Count);
                Debug.WriteLine("");               
            }

            Time.Update(gameTime);
            float deltaTime = Time.deltaTime;

            Input.Update();
            Collider[] colliders = _colliders.ToArray();

            List<Contact> contacts = new List<Contact>();
            BVH bvh = new BVH(colliders);
            var possibleContacts = bvh.GetPossibleContacts();
            var contactPairs = new HashSet<Tuple<Collider, Collider>>();

            foreach (var x in possibleContacts)
            {
                Contact contact = CollisionSystem.ResolveCollision(x.Item1, x.Item2);
                if (contact != null) contacts.Add(contact);
            }

            contacts.ForEach(x => x.ResolveContact());

            foreach (var c in contacts)
            {
                var x = c.GetContactPair();
                contactPairs.Add(x);
                var colliderOneBehaviours = x.Item1.gameObject.behaviours;
                var cooliderTwoBehaviours = x.Item2.gameObject.behaviours;

                if (_contactPairs.Remove(x) || _contactPairs.Remove(Tuple.Create(x.Item2, x.Item1)))
                {
                    // Stay
                    foreach (var y in colliderOneBehaviours) y.OnCollisionStay(c.GetCollisionData(0));
                    foreach (var y in cooliderTwoBehaviours) y.OnCollisionStay(c.GetCollisionData(1));
                }
                else
                {
                    // Enter
                    foreach (var y in colliderOneBehaviours) y.OnCollisionBegin(c.GetCollisionData(0));
                    foreach (var y in cooliderTwoBehaviours) y.OnCollisionBegin(c.GetCollisionData(1));
                }
            }

            foreach(var x in _contactPairs)
            {
                // Exit
                var colliderOneBehaviours = x.Item1.gameObject.behaviours;
                var cooliderTwoBehaviours = x.Item2.gameObject.behaviours;

                foreach (var y in colliderOneBehaviours) y.OnCollisionEnd(x.Item2);
                foreach (var y in cooliderTwoBehaviours) y.OnCollisionEnd(x.Item1);
            }

            _contactPairs = contactPairs;


            var mouseOverColliders = bvh.MouseCollision();

            foreach(var x in mouseOverColliders)
            {
                if(_mouseOverColliders.Remove(x))
                {
                    //Stay
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseOver();
                }
                else
                {
                    //enter
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseEnter();
                }
            }

            foreach(var x in _mouseOverColliders)
            {
                var behaviours = x.gameObject.behaviours;
                foreach (var y in behaviours) y.OnMouseExit();
            }

            _mouseOverColliders = mouseOverColliders;

            if(Input.IsMouseButtonDown(MouseButton.Left))
            {
                _mouseClickedColliders = new Collider[mouseOverColliders.Count];
                mouseOverColliders.CopyTo(_mouseClickedColliders);
                foreach(var x in _mouseClickedColliders)
                {
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseDown();
                }
                   
            }
            else if(Input.IsMouseButton(MouseButton.Left))
            {
                foreach (var x in _mouseClickedColliders)
                {
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseDrag();
                }
            }
            else if(Input.IsMouseButtonUp(MouseButton.Left))
            {
                foreach (var x in _mouseClickedColliders)
                {
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseUp();
                }
                _mouseClickedColliders = null;
            }

            foreach (var x in _rigidbodies.ToArray()) x.UpdatePhysics(deltaTime);
            foreach (var x in _startBehaviours.ToArray()) x.Start();
            _startBehaviours.Clear();
            foreach (var x in _behaviours.ToArray()) x.Update();
            foreach (var x in _animators.ToArray()) x.AnimationUpdate(deltaTime);
            foreach (var x in _audioSources.ToArray()) x.UpdateAudio();
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach(Camera camera in _cameras)
            {
                camera.PreRender();
                spriteBatch.Begin(transformMatrix: camera.renderTransform, sortMode: SpriteSortMode.FrontToBack);
                foreach (var x in _renderers.ToArray()) if ((camera.cullingMask & (1 << (int)x.gameObject.layer)) == 0) x.Draw(spriteBatch);
                spriteBatch.End();
            }

            foreach (Camera camera in _cameras) camera.Update();
        }

        internal static void BeginPlayMode()
        {
            List<GameObject> rootGameObjects = Scene.main._gameObjects.FindAll(x => x.parent == null);
            _cache = Tuple.Create(Scene.name, rootGameObjects);

            Scene scene = Scene.main;
            scene.Clear();
            rootGameObjects = JsonConvert.DeserializeObject<List<GameObject>>(JsonConvert.SerializeObject(rootGameObjects));
            rootGameObjects.ForEach(x => x.OnLoad());
        }

        internal static void EndPlayMode()
        {
            Scene scene = Scene.main;
            scene.Clear();
            List<GameObject> rootGameObjects = _cache.Item2;
            scene._name = _cache.Item1;
            rootGameObjects.ForEach(x => x.OnLoad());
            _cache = null;
        }

        internal static void PausePlayMode()
        {
            Scene.main._audioSources.ForEach(x => x.Pause());
        }

        internal static void ResumePlayMode()
        {
            Scene.main._audioSources.ForEach(x => x.Resume());
        }

        private void Clear()
        {
            _gameObjects.Clear();
            _renderers.Clear();
            _behaviours.Clear();
            _rigidbodies.Clear();
            _colliders.Clear();
            _contactPairs.Clear();
            _mouseOverColliders.Clear();
            _mouseClickedColliders = null;
            _startBehaviours.Clear();
            _animators.Clear();
            _cameras.Clear();
            _audioSources.ForEach(x => x.Stop());
            _audioSources.Clear();
        }
    }
}
