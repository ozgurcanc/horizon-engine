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
        private GraphicsDeviceManager _graphics;
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
        private List<IDrawable> _drawables;
        private List<IUpdatable> _updatables;

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
            if (component is IDrawable)
            {
                component.drawableID = scene._drawables.Count;
                scene._drawables.Add((IDrawable)component);
            }
            if(component is IUpdatable)
            {
                component.updatableID = scene._updatables.Count;
                scene._updatables.Add((IUpdatable)component);
            }
        }

        internal static void DisableComponent(Component component)
        {
            Scene scene = Scene.main;
            if (component is IDrawable)
            {
                int lastIndex = scene._drawables.Count - 1;
                IDrawable temp = scene._drawables[lastIndex];
                ((Component)temp).drawableID = component.drawableID;
                scene._drawables[component.drawableID] = temp;
                scene._drawables.RemoveAt(lastIndex);
            }
            if (component is IUpdatable)
            {
                int lastIndex = scene._updatables.Count - 1;
                IUpdatable temp = scene._updatables[lastIndex];
                ((Component)temp).updatableID = component.updatableID;
                scene._updatables[component.updatableID] = temp;
                scene._updatables.RemoveAt(lastIndex);
            }
        }

        public Scene(ISceneStarter sceneStarter)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _gameObjects = new List<GameObject>();
            _sceneStarter = sceneStarter;
            _textures = new Dictionary<string, Texture2D>();
            _drawables = new List<IDrawable>();
            _updatables = new List<IUpdatable>();
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
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

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

            Input.Update();
            foreach (var x in _updatables.ToArray()) x.Update(gameTime);
            //_updatables.ForEach(x => x.Update(gameTime));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //_drawables.ForEach(x => x.Draw(_spriteBatch));
            foreach (var x in _drawables.ToArray()) x.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
