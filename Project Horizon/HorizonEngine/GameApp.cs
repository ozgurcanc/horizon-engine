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
    public class GameApp : Application
    {
        public GameApp(string openingScene) : base()
        {
            LoadScene(openingScene);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Application.isQuit)
                Exit();

            _scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            Graphics.Begin();
            Graphics.Clear(Color.DarkBlue);
            _scene.Draw(_spriteBatch);
            Graphics.End();

            base.Draw(gameTime);
        }
    }
}
