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
    internal static class SpriteEditorWindow
    {
        private static bool _enabled;
        private static HorizonEngine.Texture _texture;
        private static HorizonEngine.Texture _selectedTexture;
        private static ImGUIRenderer _guiRenderer;
        private static IntPtr _image;
        private static int _column;
        private static int _row;
        private static int _pushID;

        internal static void Init(ImGUIRenderer guiRenderer)
        {
            _texture = null;
            _guiRenderer = guiRenderer;
            _column = _row = 1;
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

            _pushID = 0;

            ImGui.Begin("Sprite Editor", ref _enabled, ImGuiWindowFlags.MenuBar);

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.RadioButton("Open Sprite", true))
                {
                    ImGui.OpenPopup("select_texture");
                    SearchBar.Clear();
                }

                if (ImGui.BeginPopup("select_texture"))
                {
                    SearchBar.Draw();
                    foreach (Texture sprite in Assets.textures)
                    {
                        if (sprite is RenderTexture) continue;
                        if (!sprite.isOriginal) continue;
                        if (SearchBar.PassFilter(sprite.name) && ImGui.Selectable(sprite.name))
                        {
                            Open(sprite);
                        }
                    }
                    ImGui.EndPopup();
                }

                ImGui.EndMenuBar();
            }

            if (_texture == null) return;

            ImGui.Columns(2);

            ImGui.BeginChild("left");
            DrawImage();
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.NextColumn();

            ImGui.BeginChild("rigth");
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);
            ImGui.Text("Sprite");

            string name = _selectedTexture.name;
            ImGui.Text("Name");
            ImGui.SameLine();
            if (ImGui.InputText("##name", ref name, 100))
            {
                _selectedTexture.name = name;
            }

            bool rectangleModified = false;
            Rectangle rectangle = _selectedTexture.sourceRectangle;
            ImGui.Text("X");
            ImGui.SameLine();
            if (ImGui.DragInt("##PosX", ref rectangle.X))
            {
                rectangleModified = true;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if (ImGui.DragInt("##PosY", ref rectangle.Y))
            {
                rectangleModified = true;
            }

            ImGui.Text("W");
            ImGui.SameLine();
            if (ImGui.DragInt("##Width", ref rectangle.Width))
            {
                rectangleModified = true;
            }
            ImGui.SameLine();
            ImGui.Text("H");
            ImGui.SameLine();
            if (ImGui.DragInt("##Height", ref rectangle.Height))
            {
                rectangleModified = true;
            }

            if (rectangleModified) _selectedTexture.sourceRectangle = rectangle;

            ImGui.Text("Column");
            ImGui.SameLine();
            if(ImGui.DragInt("##spriteColumn", ref _column))
            {
                if (_column < 1) _column = 1;
            }
            ImGui.SameLine();
            ImGui.Text("Row");
            ImGui.SameLine();
            if (ImGui.DragInt("##spriteRow", ref _row))
            {
                if (_row < 1) _row = 1;
            }
            if(ImGui.Button("Slice"))
            {
                Slice();
            }
            ImGui.PopItemWidth();
            ImGui.Separator();
            ShowTexture(_texture);
            ImGui.EndChild();

            ImGui.End();
        }

        internal static void Open(Texture sprite)
        {
            _texture = sprite.baseTexture;
            _selectedTexture = sprite;
        }

        private static void DrawImage()
        {
            System.Numerics.Vector2 windowSize = ImGui.GetWindowSize();

            float aspectRatio = _texture.texture.Width / _texture.texture.Height;

            float imageWidth = windowSize.X;
            float imageHeight = imageWidth / aspectRatio;

            if (imageHeight > windowSize.Y)
            {
                imageHeight = windowSize.Y;
                imageWidth = imageHeight * aspectRatio;
            }

            System.Numerics.Vector2 imageSize = new System.Numerics.Vector2(imageWidth, imageHeight);
            ImGui.SetCursorPos((windowSize - imageSize) / 2f);

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            System.Numerics.Vector2 bottomLeft = ImGui.GetCursorPos() + ImGui.GetWindowPos();

            bottomLeft.X += ((float)_selectedTexture.sourceRectangle.X / _texture.width) * imageSize.X;
            bottomLeft.Y += ((float)_selectedTexture.sourceRectangle.Y / _texture.height) * imageSize.Y;

            System.Numerics.Vector2 rectSize = imageSize;
            rectSize.X *= (float)_selectedTexture.width / _texture.width;
            rectSize.Y *= (float)_selectedTexture.height / _texture.height;

            drawList.AddRect(bottomLeft, bottomLeft + rectSize, 0xFF00FF00, 0, ImDrawCornerFlags.All, 2f);

            if (_image != IntPtr.Zero) _guiRenderer.UnbindTexture(_image);
            _image = _guiRenderer.BindTexture(_texture.texture);
            ImGui.Image(_image, imageSize);

            float widthPerColumn = rectSize.X / (float)_column;
            System.Numerics.Vector2 topLeft = bottomLeft + new System.Numerics.Vector2(0, rectSize.Y);
            for (int i=1; i<_column; i++)
            {
                System.Numerics.Vector2 padding = new System.Numerics.Vector2(i * widthPerColumn, 0f);
                drawList.AddLine(bottomLeft + padding, topLeft + padding, 0xFF00FF00, 2f);
            }

            float heightPerRow = rectSize.Y / (float)_row;
            System.Numerics.Vector2 bottomRight = bottomLeft + new System.Numerics.Vector2(rectSize.X, 0f);
            for (int i = 1; i < _row; i++)
            {
                System.Numerics.Vector2 padding = new System.Numerics.Vector2(0f, i * heightPerRow);
                drawList.AddLine(bottomLeft + padding, bottomRight + padding, 0xFF00FF00, 2f);
            }
        }

        private static void ShowTexture(HorizonEngine.Texture texture)
        {
            var internalTextures = texture.internalTextures;
            bool isLeaf = internalTextures.Count == 0;
            ImGuiTreeNodeFlags flags = isLeaf ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen: ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;

            if (texture == _selectedTexture) flags |= ImGuiTreeNodeFlags.Selected;

            ImGui.PushID(_pushID++);

            bool nodeOpen = ImGui.TreeNodeEx(texture.name, flags);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                _selectedTexture = texture;
            }

            if (!isLeaf && nodeOpen)
            {
                foreach (HorizonEngine.Texture internalTexture in internalTextures)
                    ShowTexture(internalTexture);
                ImGui.TreePop();
            }

            ImGui.PopID();
        }

        private static void Slice()
        {
            int k = 0;
            float width = 1f / _column;
            float height = 1f / _row;
            for(int j = 0; j < _row; j++)
            {
                for (int i = 0; i < _column; i++)
                {
                    k++;
                    Vector4 sourceRectangle = new Vector4(i * width, j * height, width, height);
                    float z = _selectedTexture.width / (float)_texture.width;
                    float w = _selectedTexture.height / (float)_texture.height;
                    sourceRectangle.Z *= z;
                    sourceRectangle.W *= w;
                    sourceRectangle.X = sourceRectangle.X * z + _selectedTexture.sourceRectangle.X / (float)_texture.width;
                    sourceRectangle.Y = sourceRectangle.Y * w + _selectedTexture.sourceRectangle.Y / (float)_texture.height;
                    _texture.CreateInternalTexture(_selectedTexture.name + "_" + k.ToString(), sourceRectangle);
                }
            }

            _row = 1;
            _column = 1;
        }
    }
}
