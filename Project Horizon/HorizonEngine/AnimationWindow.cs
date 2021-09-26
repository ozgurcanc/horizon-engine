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
    internal static class AnimationWindow
    {
        private static bool _enabled;
        private static Animation _animation;
        private static ImGUIRenderer _guiRenderer;
        private static bool _play;
        private static float _currentDuration;
        private static int _currentTextureIndex;
        private static IntPtr _image;

        internal static void Init(ImGUIRenderer guiRenderer)
        {
            _animation = null;
            _currentTextureIndex = 0;
            _guiRenderer = guiRenderer;
        }

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

        internal static void Draw()
        {
            if (!enabled) return;

            ImGui.Begin("Animation", ref _enabled, ImGuiWindowFlags.MenuBar);

            if(ImGui.BeginMenuBar())
            {
                if(ImGui.RadioButton("Open Animation", true))
                {
                    ImGui.OpenPopup("select_animation");
                    SearchBar.Clear();
                }

                if(ImGui.BeginPopup("select_animation"))
                {
                    SearchBar.Draw();
                    foreach(Animation animation in Assets.animations)
                    {
                        if (SearchBar.PassFilter(animation.name) && ImGui.Selectable(animation.name)) Open(animation);
                    }
                    ImGui.EndPopup();
                }

                ImGui.EndMenuBar();
            }

            if (_animation == null) return;

            Update();

            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            ImGui.Text("name");
            ImGui.SameLine();
            string name = _animation.name;
            if(ImGui.InputText("##animationName", ref name, 100))
            {
                _animation.name = name;
            }
            ImGui.SameLine();
            ImGui.Text("duration");
            ImGui.SameLine();
            float duration = _animation.duration;
            if(ImGui.InputFloat("##animationDuration", ref duration))
            {
                _animation.duration = duration;
            }
            ImGui.SameLine();
            ImGui.Text("Loop");
            ImGui.SameLine();
            bool loop = _animation.loop;
            if(ImGui.Checkbox("##animationBool", ref loop))
            {
                _animation.loop = loop;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Add new frame", true))
            {
                ImGui.OpenPopup("add_new_frame");
                SearchBar.Clear();
            }

            if(ImGui.BeginPopup("add_new_frame"))
            {
                SearchBar.Draw();
                foreach(HorizonEngine.Texture texture in Assets.textures)
                {
                    if (SearchBar.PassFilter(texture.name) && ImGui.Selectable(texture.name)) 
                        _animation.AddFrame(texture);
                }

                ImGui.EndPopup();
            }

            ImGui.PopItemWidth();
            ImGui.Separator();

            ImGui.Columns(2);

            if (ImGui.Checkbox("Play", ref _play))
            {

            }
            ImGui.Separator();

            ImGui.BeginChild("animation_image");            
            DrawImage();
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.NextColumn();

            ImGui.BeginChild("animation_frames");
            
            ImGui.Text("Frames");            
            ImGui.Separator();

            int animationCount = _animation.length;
            for(int i=0; i< animationCount; i++)
            {

                if(ImGui.Selectable(_animation[i].name, _currentTextureIndex == i))
                {
                    _currentTextureIndex = i;
                }

                if(ImGui.IsItemActive() && !ImGui.IsItemHovered())
                {
                    int next = i;
                    next += ImGui.GetMouseDragDelta().Y < 0f ? -1 : 1;
                    if(next >= 0 && next < animationCount)
                    {
                        _animation.SwapFrame(i, next);
                    }
                }

                if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    _currentTextureIndex = i;
                    if(!_play) ImGui.OpenPopup("delete_frame");
                }               
            }

            if (ImGui.BeginPopup("delete_frame"))
            {
                if (ImGui.Selectable("Delete"))
                {
                    _animation.RemoveFrame(_animation[_currentTextureIndex]);
                }
                ImGui.EndPopup();
            }

            ImGui.EndChild();

            ImGui.End();
        }

        private static void DrawImage()
        {
            int animationCount = _animation.length;
            if (_currentTextureIndex >= animationCount) return;
            HorizonEngine.Texture currentTexture = _animation[_currentTextureIndex];

            System.Numerics.Vector2 windowSize = ImGui.GetWindowSize();

            float aspectRatio = currentTexture.texture.Width / currentTexture.texture.Height;

            float imageWidth = windowSize.X;
            float imageHeight = imageWidth / aspectRatio;

            if (imageHeight > windowSize.Y)
            {
                imageHeight = windowSize.Y;
                imageWidth = imageHeight * aspectRatio;
            }

            System.Numerics.Vector2 imageSize = new System.Numerics.Vector2(imageWidth, imageHeight);
            ImGui.SetCursorPos((windowSize - imageSize) / 2f);

            if (_image != IntPtr.Zero) _guiRenderer.UnbindTexture(_image);
            _image = _guiRenderer.BindTexture(currentTexture.texture);
            ImGui.Image(_image, imageSize);
            //_guiRenderer.UnbindTexture(image);
        }

        private static void Update()
        {
            if (!_play) return;
            int animationCount = _animation.length;
            if (animationCount <= 0) return;

            _currentDuration += Time.deltaTime;
            if(_currentDuration >= _animation.frameDuration)
            {
                _currentDuration = 0f;
                _currentTextureIndex = (_currentTextureIndex + 1) % animationCount;
            }
        }

        internal static void Open(Animation animation)
        {
            _animation = animation;
        }
    }
}
