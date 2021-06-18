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


        private List<GameObject> gameObjects = new List<GameObject>();

        public Scene()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsFixedTimeStep = false;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _timeCounter = _fps = 0;



            // Testing

            GameObject a = new GameObject("1");
            GameObject b = new GameObject("2");
            GameObject c = new GameObject("3");

            a.Size = new Vector2(100, 100);
            b.Size = new Vector2(50, 50);
            c.Size = new Vector2(50, 50);

            b.Position = new Vector2(0, 0);
            c.Position = new Vector2(50, 50);

            gameObjects.Add(a);
            gameObjects.Add(b);
            gameObjects.Add(c);

            b.Parent = a;
            c.Parent = b;

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _spriteFont = Content.Load<SpriteFont>("TestFont");
            _texture = Content.Load<Texture2D>("TestTexture");
            box = Content.Load<Texture2D>("Box");
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


            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.A))
            {
                gameObjects[0].Position += new Vector2(100, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (state.IsKeyDown(Keys.S))
            {
                gameObjects[1].Position += new Vector2(100, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (state.IsKeyDown(Keys.D))
            {
                gameObjects[2].Position += new Vector2(100, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (state.IsKeyDown(Keys.W))
            {
                gameObjects[2].Parent = gameObjects[0];
            }
            if (state.IsKeyDown(Keys.E))
            {
                gameObjects[0].Position = new Vector2(500, 500);
                gameObjects[0].Size = new Vector2(300, 100);
            }
            if (state.IsKeyDown(Keys.R))
            {
                gameObjects[0].Size = new Vector2(350, 350);
                gameObjects[0].Position = new Vector2(500, 0);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //gameObjects.ForEach(x => _spriteBatch.DrawString(_spriteFont, x.Name, x.Position, Color.White));
            //_spriteBatch.DrawString(_spriteFont, "Hello", new Vector2(300, 300), Color.White);
            _spriteBatch.Draw(box, new Rectangle((int)gameObjects[0].Position.X, (int)gameObjects[0].Position.Y, (int)gameObjects[0].Size.X, (int)gameObjects[0].Size.Y), Color.White);
            _spriteBatch.Draw(box, new Rectangle((int)gameObjects[1].Position.X, (int)gameObjects[1].Position.Y, (int)gameObjects[1].Size.X, (int)gameObjects[1].Size.Y), Color.Red);
            _spriteBatch.Draw(box, new Rectangle((int)gameObjects[2].Position.X, (int)gameObjects[2].Position.Y, (int)gameObjects[2].Size.X, (int)gameObjects[2].Size.Y), Color.Green);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
