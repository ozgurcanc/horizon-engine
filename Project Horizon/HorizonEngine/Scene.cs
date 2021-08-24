using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace HorizonEngine
{
    public class Scene
    {
        //private GraphicsDeviceManager _graphics;
        //private SpriteBatch _spriteBatch;

        private static Scene _main;
        private List<GameObject> _gameObjects;
        private IScene _nextScene;
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

        internal static Scene main
        {
            get
            {
                return _main;
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

        internal static void EnableComponent(Component component)
        {
            Scene scene = Scene.main;
            if (component is Renderer)
            {
                component.componetID = scene._renderers.Count;
                scene._renderers.Add((Renderer)component);
            }
            else if(component is Behaviour)
            {
                component.componetID = scene._behaviours.Count;
                scene._behaviours.Add((Behaviour)component);
                ((Behaviour)component).OnEnable();
                if(component.startFlag)
                {
                    scene._startBehaviours.Add((Behaviour)component);
                    component.startFlag = false;
                }
            }
            else if(component is Rigidbody)
            {
                component.componetID = scene._rigidbodies.Count;
                scene._rigidbodies.Add((Rigidbody)component);
            }
            else if (component is Collider)
            {
                component.componetID = scene._colliders.Count;
                scene._colliders.Add((Collider)component);
            }
            else if (component is Animator)
            {
                component.componetID = scene._animators.Count;
                scene._animators.Add((Animator)component);
            }
            else if (component is Camera)
            {
                component.componetID = scene._cameras.Count;
                scene._cameras.Add((Camera)component);
            }
        }

        internal static void DisableComponent(Component component)
        {
            Scene scene = Scene.main;
            if (component is Renderer)
            {
                int lastIndex = scene._renderers.Count - 1;
                Renderer temp = scene._renderers[lastIndex];
                temp.componetID = component.componetID;
                scene._renderers[component.componetID] = temp;
                scene._renderers.RemoveAt(lastIndex);
            }
            else if (component is Behaviour)
            {
                int lastIndex = scene._behaviours.Count - 1;
                Behaviour temp = scene._behaviours[lastIndex];
                temp.componetID = component.componetID;
                scene._behaviours[component.componetID] = temp;
                scene._behaviours.RemoveAt(lastIndex);
                ((Behaviour)component).OnDisable();
            }
            else if (component is Rigidbody)
            {
                int lastIndex = scene._rigidbodies.Count - 1;
                Rigidbody temp = scene._rigidbodies[lastIndex];
                temp.componetID = component.componetID;
                scene._rigidbodies[component.componetID] = temp;
                scene._rigidbodies.RemoveAt(lastIndex);
            }
            else if (component is Collider)
            {
                int lastIndex = scene._colliders.Count - 1;
                Collider temp = scene._colliders[lastIndex];
                temp.componetID = component.componetID;
                scene._colliders[component.componetID] = temp;
                scene._colliders.RemoveAt(lastIndex);
            }
            else if(component is Animator)
            {
                int lastIndex = scene._animators.Count - 1;
                Animator temp = scene._animators[lastIndex];
                temp.componetID = component.componetID;
                scene._animators[component.componetID] = temp;
                scene._animators.RemoveAt(lastIndex);
            }
            else if (component is Camera)
            {
                int lastIndex = scene._cameras.Count - 1;
                Camera temp = scene._cameras[lastIndex];
                temp.componetID = component.componetID;
                scene._cameras[component.componetID] = temp;
                scene._cameras.RemoveAt(lastIndex);
            }
        }

        internal Scene(ContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager)
        {
            //_graphics = new GraphicsDeviceManager(this);
            //Camera.InitCamera(graphicsDeviceManager);
            Assets.InitAssets(contentManager);
            //Content.RootDirectory = "Content";
            //IsMouseVisible = true;

            _gameObjects = new List<GameObject>();
            _nextScene = null;
            _renderers = new List<Renderer>();
            _behaviours = new List<Behaviour>();
            _rigidbodies = new List<Rigidbody>();
            _colliders = new List<Collider>();
            _contactPairs = new HashSet<Tuple<Collider, Collider>>();
            _mouseOverColliders = new HashSet<Collider>();
            _startBehaviours = new List<Behaviour>();
            _animators = new List<Animator>();
            _cameras = new List<Camera>();
            _main = this;

            GameObject mainCamera = Scene.CreateGameObject("Main Camera");
            mainCamera.DontDestroyOnLoad();
            mainCamera.AddComponent<Camera>();
        }
        
        public static void Load<T>() where T : IScene, new()
        {
            Scene.main._nextScene = new T();
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
            int lastIndex = Scene.main._gameObjects.Count - 1;
            GameObject temp = Scene.main._gameObjects[lastIndex];
            temp.gameObjectID = gameObject.gameObjectID;
            Scene.main._gameObjects[gameObject.gameObjectID] = temp;
            Scene.main._gameObjects.RemoveAt(lastIndex);
            gameObject.Destroy();
        }

        internal void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            
            if(Input.GetKeyDown(Keys.H))
            {
                Debug.WriteLine("gameobjects : " + _gameObjects.Count);
                Debug.WriteLine("behaviours : " + _behaviours.Count);
                Debug.WriteLine("sprites : " + _renderers.Count);
                Debug.WriteLine("");
            }

            if(_nextScene != null)
            {
                var w = new System.Diagnostics.Stopwatch();
                w.Start();

                List<GameObject> dontDestroyOnLoad = _gameObjects.FindAll(x => x.dontDestroyOnLoad && x.parent == null);
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

                dontDestroyOnLoad.ForEach(x => x.OnLoad());
                _nextScene.Load();
                _nextScene = null;

                w.Stop();
                Debug.WriteLine($"Execution Time: {w.ElapsedMilliseconds} ms");
            }


            // TODO: Add your update logic here
            /*
            _fps++;
            _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeCounter > 1.0)
            {
                //Debug.WriteLine(_fps);
                _timeCounter = _fps = 0;
            }
            */

            Time.Update(gameTime);
            float deltaTime = Time.deltaTime;

            foreach (Camera camera in _cameras) camera.Update();
            Input.Update();
            var watch = new Stopwatch();
            watch.Start();
            Collider[] colliders = _colliders.ToArray();



            /*
            foreach (var x in colliders) x.UpdateCollider();
            for (int i = 0; i < colliders.Length; i++)
                for (int j = i + 1; j < colliders.Length; j++)
                {
                    Contact c = CollisionSystem.ResolveCollision(colliders[i], colliders[j]);
                    if (c != null) contacts.Add(c);
                }
            
            foreach (var x in contacts) x.ResolveContact();
            */

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
                bool isTrigger = c.isTrigger;
                var x = c.GetContactPair();
                contactPairs.Add(x);
                if (_contactPairs.Remove(x) || _contactPairs.Remove(Tuple.Create(x.Item2, x.Item1)))
                {
                    // Stay
                    var colliderOneBehaviours = x.Item1.gameObject.behaviours;
                    var cooliderTwoBehaviours = x.Item2.gameObject.behaviours;
                    if(isTrigger)
                    {
                        foreach (var y in colliderOneBehaviours) y.OnTriggerStay(x.Item2);
                        foreach (var y in cooliderTwoBehaviours) y.OnTriggerStay(x.Item1);
                    }
                    else
                    {
                        foreach (var y in colliderOneBehaviours) y.OnCollisionStay(c.GetCollisionData(0));
                        foreach (var y in cooliderTwoBehaviours) y.OnCollisionStay(c.GetCollisionData(1));
                    }
                    
                }
                else
                {
                    // Enter
                    var colliderOneBehaviours = x.Item1.gameObject.behaviours;
                    var cooliderTwoBehaviours = x.Item2.gameObject.behaviours;
                    if(isTrigger)
                    {
                        foreach (var y in colliderOneBehaviours) y.OnTriggerEnter(x.Item2);
                        foreach (var y in cooliderTwoBehaviours) y.OnTriggerEnter(x.Item1);
                    } 
                    else
                    {
                        foreach (var y in colliderOneBehaviours) y.OnCollisionEnter(c.GetCollisionData(0));
                        foreach (var y in cooliderTwoBehaviours) y.OnCollisionEnter(c.GetCollisionData(1));
                    }
                }
            }

            foreach(var x in _contactPairs)
            {
                // Exit
                bool isTrigger = x.Item1.isTrigger || x.Item2.isTrigger;
                var colliderOneBehaviours = x.Item1.gameObject.behaviours;
                var cooliderTwoBehaviours = x.Item2.gameObject.behaviours;
                if(isTrigger)
                {
                    foreach (var y in colliderOneBehaviours) y.OnTriggerExit(x.Item2);
                    foreach (var y in cooliderTwoBehaviours) y.OnTriggerExit(x.Item1);
                }
                else
                {
                    foreach (var y in colliderOneBehaviours) y.OnCollisionExit(x.Item2);
                    foreach (var y in cooliderTwoBehaviours) y.OnCollisionExit(x.Item1);
                }
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

            if(Input.GetMouseButtonDown(0))
            {
                _mouseClickedColliders = new Collider[mouseOverColliders.Count];
                mouseOverColliders.CopyTo(_mouseClickedColliders);
                foreach(var x in _mouseClickedColliders)
                {
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseDown();
                }
                   
            }
            else if(Input.GetMouseButton(0))
            {
                foreach (var x in _mouseClickedColliders)
                {
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseDrag();
                }
            }
            else if(Input.GetMouseButtonUp(0))
            {
                foreach (var x in _mouseClickedColliders)
                {
                    var behaviours = x.gameObject.behaviours;
                    foreach (var y in behaviours) y.OnMouseUp();
                }
                _mouseClickedColliders = null;
            }
    
            //Debug.WriteLine(contacts.Count);
            //Debug.WriteLine(bvh.ReselveCollision());
            //Debug.WriteLine("---");


            watch.Stop();
            //Debug.WriteLine(watch.ElapsedMilliseconds);

            foreach (var x in _rigidbodies.ToArray()) x.UpdatePhysics(deltaTime);
            foreach (var x in _startBehaviours.ToArray()) x.Start();
            _startBehaviours.Clear();
            foreach (var x in _behaviours.ToArray()) x.Update();
            foreach (var x in _animators.ToArray()) x.AnimationUpdate(deltaTime);
            //_updatables.ForEach(x => x.Update(gameTime));
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            //Debug.WriteLine(Vector2.Transform(new Vector2(-5 * Camera.scale, +5 * Camera.scale), _camera.renderTransform));
            foreach(Camera camera in _cameras)
            {
                camera.Begin();
                // TODO: Add your drawing code here
                //Debug.WriteLine(Vector2.Transform(new Vector2(0, 0), _camera.worldToScreen));
                spriteBatch.Begin(transformMatrix: camera.renderTransform, sortMode: SpriteSortMode.FrontToBack);
                //_drawables.ForEach(x => x.Draw(_spriteBatch));
                foreach (var x in _renderers.ToArray()) if ((camera.cullingMask & (1 << (int)x.gameObject.layer)) == 0) x.Draw(spriteBatch);
                spriteBatch.End();

                camera.End();
            }
            
        }
    }
}
