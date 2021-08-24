using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using MonoGame.ImGui;
using ImGuiNET;

namespace HorizonEngine
{
    internal static class GameWindow
    {
        private static IntPtr _sceneImage;
        private static bool _enabled;

        internal static bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        internal static void Init(ImGUIRenderer guiRenderer, GraphicsDevice graphicsDevice)
        {
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, (int)Screen.resolution.X, (int)Screen.resolution.Y);
            _sceneImage = guiRenderer.BindTexture(renderTarget);
            Camera.main.renderTarget = renderTarget;
        }

        internal static void Draw()
        {
            if (!enabled) return; 

            ImGui.Begin("Game", ref _enabled, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            System.Numerics.Vector2 windowSize = ImGui.GetWindowSize();
            //windowSize.X -= ImGui.GetScrollX();
            //windowSize.Y -= ImGui.GetScrollY();

            float aspectRatio = Screen.resolution.X / Screen.resolution.Y;

            float imageWidth = windowSize.X;
            float imageHeight = imageWidth / aspectRatio;           

            if(imageHeight > windowSize.Y)
            {
                imageHeight = windowSize.Y;
                imageWidth = imageHeight * aspectRatio;
            }
            
            System.Numerics.Vector2 imageSize = new System.Numerics.Vector2(imageWidth, imageHeight);
            ImGui.SetCursorPos((windowSize - imageSize) / 2f);

            ImGui.Image(_sceneImage, imageSize);
            ImGui.End();
        }
    }
}
