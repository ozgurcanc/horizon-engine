using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using MonoGame.ImGui;
using ImGuiNET;

namespace HorizonEngine
{
    public class Engine : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Scene _scene;
        private ImGUIRenderer _guiRenderer;

        public Engine()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Screen.Init(_graphics);
            _scene = new Scene(this.Content, _graphics);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsFixedTimeStep = false;
            Screen.resolution = new Vector2(1600, 900);

            _guiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            GameWindow.Init(_guiRenderer, _graphics.GraphicsDevice);

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

            _scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            Screen.Begin();
            Screen.Clear(Color.DarkBlue);

            _scene.Draw(_spriteBatch);

            Screen.End();

            _guiRenderer.BeginLayout(gameTime);
            ImGui.DockSpaceOverViewport();
                      
            ImGui.ShowDemoWindow();
            
            MainMenuBar.Draw();
            HierarchyWindow.Draw();
            GameWindow.Draw();

            _guiRenderer.EndLayout();

            base.Draw(gameTime);
        }
    }
}
