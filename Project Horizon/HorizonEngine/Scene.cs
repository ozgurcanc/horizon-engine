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
        private List<IDrawable> _drawables;
        private List<IUpdatable> _updatables;
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
            //_graphics = new GraphicsDeviceManager(this);
            _camera = new Camera(new GraphicsDeviceManager(this));
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

            _camera.Update();
            Input.Update();
            foreach (var x in _updatables.ToArray()) x.Update(gameTime);
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
            foreach (var x in _drawables.ToArray()) x.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
