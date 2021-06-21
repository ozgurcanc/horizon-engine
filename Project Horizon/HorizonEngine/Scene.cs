﻿using Microsoft.Xna.Framework;
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
        private Camera _camera;

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
        }

        public Scene(ISceneStarter sceneStarter)
        {
            //_graphics = new GraphicsDeviceManager(this);
            _camera = new Camera(new GraphicsDeviceManager(this));
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _gameObjects = new List<GameObject>();
            _sceneStarter = sceneStarter;
            _textures = new Dictionary<string, Texture2D>();
            _renderers = new List<Renderer>();
            _behaviours = new List<Behaviour>();
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

        public static Camera camera
        {
            get
            {
                return Scene.main._camera;
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsFixedTimeStep = false;
            _camera.resolution = new Vector2(1280, 720);

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
                Debug.WriteLine(_fps);
                _timeCounter = _fps = 0;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _camera.Update();
            Input.Update();
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
            _spriteBatch.Begin(transformMatrix: _camera.renderTransform);
            //_drawables.ForEach(x => x.Draw(_spriteBatch));
            foreach (var x in _renderers.ToArray()) x.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
