using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace HorizonEngine
{
    public class Scene : Game
    {
        //private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int _fps;
        private double _timeCounter;
        private SpriteFont _spriteFont;
        private Texture2D _texture;
        private Texture2D box;

        private static Scene _main;
        private List<GameObject> _gameObjects;
        private Dictionary<string, Texture2D> _textures;
        private ISceneStarter _sceneStarter;
        private List<Renderer> _renderers;
        private List<Behaviour> _behaviours;
        private List<Rigidbody> _rigidbodies;
        private List<Collider> _colliders;

        internal static Scene main
        {
            get
            {
                return _main;
            }
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
        }

        public Scene(ISceneStarter sceneStarter)
        {
            //_graphics = new GraphicsDeviceManager(this);
            Camera.InitCamera(new GraphicsDeviceManager(this));
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _gameObjects = new List<GameObject>();
            _sceneStarter = sceneStarter;
            _textures = new Dictionary<string, Texture2D>();
            _renderers = new List<Renderer>();
            _behaviours = new List<Behaviour>();
            _rigidbodies = new List<Rigidbody>();
            _colliders = new List<Collider>();
            _main = this;
        }     

        public static GameObject CreateGameObject(string name = "GameObject")
        {
            GameObject gameObject = new GameObject(name);
            Scene.main._gameObjects.Add(gameObject);
            return gameObject;
        }

        public static Texture2D GetTexture(string name)
        {
            Scene scene = Scene.main;
            if(scene._textures.ContainsKey(name))
            {
                return scene._textures[name];
            }

            Texture2D texture = main.Content.Load<Texture2D>(name);
            scene._textures.Add(name, texture);
            return texture;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsFixedTimeStep = false;
            //TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / 30f));
            Camera.resolution = new Vector2(1280, 720);

            _timeCounter = _fps = 0;
            _sceneStarter.Start();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here           
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _fps++;
            _timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeCounter > 1.0)
            {
                //Debug.WriteLine(_fps);
                _timeCounter = _fps = 0;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Camera.Update();
            Input.Update();
            var watch = new Stopwatch();
            watch.Start();
            Collider[] colliders = _colliders.ToArray();

            List<Contact> contacts = new List<Contact>();
            
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
            
            if(colliders.Length > 0)
            {
                BVH bvh = new BVH(colliders);
                bvh.ReselveCollision();
            }        
            //Debug.WriteLine(contacts.Count);
            //Debug.WriteLine(bvh.ReselveCollision());
            //Debug.WriteLine("---");


            watch.Stop();
            //Debug.WriteLine(watch.ElapsedMilliseconds);

            foreach (var x in _rigidbodies.ToArray()) x.UpdatePhysics(deltaTime);
            foreach (var x in _behaviours.ToArray()) x.Update(deltaTime);
            //_updatables.ForEach(x => x.Update(gameTime));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Debug.WriteLine(Vector2.Transform(new Vector2(-5 * Camera.scale, +5 * Camera.scale), _camera.renderTransform));
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            //Debug.WriteLine(Vector2.Transform(new Vector2(0, 0), _camera.worldToScreen));
            _spriteBatch.Begin(transformMatrix: Camera.renderTransform);
            //_drawables.ForEach(x => x.Draw(_spriteBatch));
            foreach (var x in _renderers.ToArray()) if ((Camera.cullingMask & (1 << (int)x.gameObject.layer)) == 0) x.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
