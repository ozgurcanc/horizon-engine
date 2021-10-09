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
    public class EditorApp : Application
    {
        private ImGUIRenderer _guiRenderer;

        public EditorApp() : base(true)
        {
            //Editor Resolution
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _guiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            AssetsWindow.Init();
            GameWindow.Init(_guiRenderer);
            AnimationWindow.Init(_guiRenderer);
            SpriteEditorWindow.Init(_guiRenderer);

            GameWindow.enabled = true;
            InspectorWindow.enabled = true;
            HierarchyWindow.enabled = true;
            AssetsWindow.enabled = true;
            AnimationWindow.enabled = true;
            AnimatorWindow.enabled = true;
            SpriteEditorWindow.enabled = true;

            DefaultStyle();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Application.isQuit)
                Exit();

            if (GameWindow.isRunning)
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
            SpriteEditorWindow.Draw();
            ProjectSettingsWindow.Draw();

            _guiRenderer.EndLayout();

            base.Draw(gameTime);
        }

        private void DefaultStyle()
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            var colors = style.Colors;
            colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 1.000f);
            colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.500f, 0.500f, 0.500f, 1.000f);
            colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.180f, 0.180f, 0.180f, 1.000f);
            colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.280f, 0.280f, 0.280f, 0.000f);
            colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.313f, 0.313f, 0.313f, 1.000f);
            colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.266f, 0.266f, 0.266f, 1.000f);
            colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.000f, 0.000f, 0.000f, 0.000f);
            colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.160f, 0.160f, 0.160f, 1.000f);
            colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.200f, 0.200f, 0.200f, 1.000f);
            colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.280f, 0.280f, 0.280f, 1.000f);
            colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.148f, 0.148f, 0.148f, 1.000f);
            colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.148f, 0.148f, 0.148f, 1.000f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.148f, 0.148f, 0.148f, 1.000f);
            colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.195f, 0.195f, 0.195f, 1.000f);
            colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.160f, 0.160f, 0.160f, 1.000f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.277f, 0.277f, 0.277f, 1.000f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.300f, 0.300f, 0.300f, 1.000f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 1.000f);
            colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.391f, 0.391f, 0.391f, 1.000f);
            colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 0.000f);
            colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 0.156f);
            colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 0.391f);
            colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.313f, 0.313f, 0.313f, 1.000f);
            colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.469f, 0.469f, 0.469f, 1.000f);
            colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.469f, 0.469f, 0.469f, 1.000f);
            colors[(int)ImGuiCol.Separator] = colors[(int)ImGuiCol.Border];
            colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.391f, 0.391f, 0.391f, 1.000f);
            colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 0.250f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 0.670f);
            colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.098f, 0.098f, 0.098f, 1.000f);
            colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.352f, 0.352f, 0.352f, 1.000f);
            colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.195f, 0.195f, 0.195f, 1.000f);
            colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.098f, 0.098f, 0.098f, 1.000f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.195f, 0.195f, 0.195f, 1.000f);
            colors[(int)ImGuiCol.DockingPreview] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 0.781f);
            colors[(int)ImGuiCol.DockingEmptyBg] = new System.Numerics.Vector4(0.180f, 0.180f, 0.180f, 1.000f);
            colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.469f, 0.469f, 0.469f, 1.000f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(0.586f, 0.586f, 0.586f, 1.000f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(1.000f, 1.000f, 1.000f, 0.156f);
            colors[(int)ImGuiCol.DragDropTarget] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.NavHighlight] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.NavWindowingHighlight] = new System.Numerics.Vector4(1.000f, 0.391f, 0.000f, 1.000f);
            colors[(int)ImGuiCol.NavWindowingDimBg] = new System.Numerics.Vector4(0.000f, 0.000f, 0.000f, 0.586f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.000f, 0.000f, 0.000f, 0.586f);

            style.ChildRounding = 4.0f;
            style.FrameBorderSize = 1.0f;
            style.FrameRounding = 2.0f;
            style.GrabMinSize = 7.0f;
            style.PopupRounding = 2.0f;
            style.ScrollbarRounding = 12.0f;
            style.ScrollbarSize = 13.0f;
            style.TabBorderSize = 1.0f;
            style.TabRounding = 0.0f;
            style.WindowRounding = 4.0f;
        }
    }
}
