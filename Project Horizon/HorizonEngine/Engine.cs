using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using MonoGame.ImGui;
using ImGuiNET;
using Newtonsoft.Json;

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
            Application.isEditor = true;
            _scene = new Scene(this.Content, _graphics);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsFixedTimeStep = false;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            _guiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            GameWindow.Init(_guiRenderer);
            AnimationWindow.Init(_guiRenderer);

            GameWindow.enabled = true;
            InspectorWindow.enabled = true;
            HierarchyWindow.enabled = true;
            AssetsWindow.enabled = true;
            AnimationWindow.enabled = true;
            AnimatorWindow.enabled = true;

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
            InspectorWindow.Draw();
            AssetsWindow.Draw();
            AnimationWindow.Draw();
            AnimatorWindow.Draw();

            _guiRenderer.EndLayout();

            base.Draw(gameTime);
        }
    }
}
