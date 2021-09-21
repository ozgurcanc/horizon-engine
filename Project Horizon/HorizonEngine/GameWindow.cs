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
        private static int _resolutionX;
        private static int _resolutionY;
        private static ImGUIRenderer _guiRenderer;
        private static bool _isPlaying;
        private static bool _isPaused;

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

        internal static bool isRunning
        {
            get
            {
                return _isPlaying && !_isPaused;
            }
        }

        internal static void Init(ImGUIRenderer guiRenderer)
        {
            _guiRenderer = guiRenderer;
            CreateRenderTarget();
        }

        internal static void Draw()
        {
            if (!enabled) return; 

            ImGui.Begin("Game", ref _enabled, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            Vector2 resolution = Screen.resolution;
            ImGui.SameLine();
            if (ImGui.Button(resolution.X.ToString() + "x" + resolution.Y.ToString()))
            {
                _resolutionX = (int)resolution.X;
                _resolutionY = (int)resolution.Y;
                ImGui.OpenPopup("resolution");
            }
            if (ImGui.BeginPopup("resolution"))
            {
                ImGui.DragInt("Width", ref _resolutionX);
                ImGui.DragInt("Height", ref _resolutionY);
                if (ImGui.Button("Apply")) 
                {
                    Screen.resolution = new Vector2(_resolutionX, _resolutionY);
                    CreateRenderTarget();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            if(ImGui.Checkbox("Play", ref _isPlaying))
            {
                //if (_isPlaying) Scene.Save();
                //Scene.Load(Scene.name);
            }
            ImGui.SameLine();
            if(ImGui.Checkbox("Pause", ref _isPaused))
            {

            }

            ImGui.BeginChild("Image");
            System.Numerics.Vector2 windowSize = ImGui.GetWindowSize();
            //windowSize.X -= ImGui.GetScrollX();
            //windowSize.Y -= ImGui.GetScrollY();

            float aspectRatio = resolution.X / resolution.Y;

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

            ImGui.EndChild();
            ImGui.End();
        }

        private static void CreateRenderTarget()
        {
            if(_sceneImage != IntPtr.Zero)
            {
                _guiRenderer.UnbindTexture(_sceneImage);
            }
            RenderTarget2D renderTarget = Screen.CreateRenderTarget((int)Screen.resolution.X, (int)Screen.resolution.Y);
            _sceneImage = _guiRenderer.BindTexture(renderTarget);
            Screen.defaultRenderTarget = renderTarget;
        }
    }
}
