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
    internal class GameObjectInspector : Inspector
    {
        private GameObject _gameObject;

        internal GameObjectInspector(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public override void Draw()
        {
            if (!ImGui.CollapsingHeader("Game Object")) return;

            string name = _gameObject.name;
            ImGui.Text("Name");
            ImGui.SameLine();
            if (ImGui.InputText("##name", ref name, 100))
            {
                _gameObject.name = name;
            }
            
            bool activeSelf = _gameObject.activeSelf;
            ImGui.Text("Active");
            ImGui.SameLine();
            if (ImGui.Checkbox("##active", ref activeSelf))
            {
                _gameObject.activeSelf = activeSelf;
            }

            int layer = (int)_gameObject.layer;
            string[] layers = Enum.GetNames(typeof(Layer));
            ImGui.Text("Layer");
            ImGui.SameLine();
            if(ImGui.Combo("##Layer", ref layer, layers, layers.Length))
            {
                _gameObject.layer = (Layer)layer;
            }
                

            ImGui.Separator();
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            Vector2 position = _gameObject.position;           
            ImGui.Text("Position");
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if (ImGui.DragFloat("##PositionX", ref position.X))
            {
                _gameObject.position = position;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if (ImGui.DragFloat("##PositionY", ref position.Y))
            {
                _gameObject.position = position;
            }

            float rotation = _gameObject.rotation;
            ImGui.Text("Rotation");
            ImGui.SameLine();
            if(ImGui.DragFloat("##Rotation", ref rotation))
            {
                _gameObject.rotation = rotation;
            }

            Vector2 size = _gameObject.size;
            ImGui.Text("Size");
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if(ImGui.DragFloat("##SizeX", ref size.X))
            {
                _gameObject.size = size;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if(ImGui.DragFloat("##SizeY", ref size.Y))
            {
                _gameObject.size = size;
            }
                        
            ImGui.PopItemWidth();
            ImGui.Separator();
        }
    }
}
