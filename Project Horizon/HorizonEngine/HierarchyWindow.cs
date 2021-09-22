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
    internal static class HierarchyWindow
    {
        private static bool _enabled;
        private static ImGuiTreeNodeFlags _leafNodeFlag;
        private static ImGuiTreeNodeFlags _innerNodeFlag;
        private static int _selectedGameObjectId;
        private static GameObject _selectedGameObject;
        private static bool _dropped;
        private static bool _clicked;

        static HierarchyWindow()
        {
            _selectedGameObjectId = -1;
            _leafNodeFlag = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            _innerNodeFlag = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;
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

            _dropped = false;
            _clicked = false;

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(450, 450), ImGuiCond.FirstUseEver);
            if(!ImGui.Begin("Hierarchy", ref _enabled))
            {
                ImGui.End();
                return;
            }

            if (Scene.name == null) return;
            if (!ImGui.CollapsingHeader(Scene.name)) return;

            ImGui.BeginChild("main");

            foreach(GameObject gameObject in Scene.gameObjects)
            {
                if (gameObject.parent == null) ShowGameObject(gameObject);
            }

            if(ImGui.BeginPopupContextWindow())
            {
                if (ImGui.MenuItem("New GameObject")) 
                { 
                    var x = Scene.CreateGameObject();
                    Undo.RegisterAction(x, true);
                }
                if (ImGui.MenuItem("Delete", _selectedGameObject != null)) 
                {
                    Undo.RegisterAction(_selectedGameObject, false);
                    Scene.Destroy(_selectedGameObject);
                    _selectedGameObjectId = -1;
                    _selectedGameObject = null;
                    InspectorWindow.Inspect(null);
                }
                if (ImGui.MenuItem("Duplicate", _selectedGameObject != null)) 
                { 
                    var x = Scene.Clone(_selectedGameObject);
                    Undo.RegisterAction(x, true);
                }
                ImGui.EndPopup();
            }

            ImGui.EndChild();

            if (ImGui.BeginDragDropTarget())
            {
                if (!_dropped && ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ImGui.GetDragDropPayload().IsDataType("GameObject"))
                {
                    Undo.RegisterAction(_selectedGameObject, _selectedGameObject.parent, null, nameof(GameObject.parent));
                    _selectedGameObject.parent = null;
                }
                ImGui.EndDragDropTarget();
            }

            if (!_clicked && (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right)))
            {
                _selectedGameObjectId = -1;
                _selectedGameObject = null;
                InspectorWindow.Inspect(null);
            }
           
        }

        private static void ShowGameObject(GameObject gameObject)
        {
            ImGuiTreeNodeFlags flags;
            bool isLeaf = gameObject.childCount == 0;
            flags = isLeaf ? _leafNodeFlag : _innerNodeFlag;

            if (gameObject.gameObjectID == _selectedGameObjectId) flags |= ImGuiTreeNodeFlags.Selected;

            ImGui.PushID(gameObject.gameObjectID);

            bool nodeOpen = ImGui.TreeNodeEx(gameObject.name, flags);

            if(ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                _selectedGameObjectId = gameObject.gameObjectID;
                _selectedGameObject = gameObject;
                _clicked = true;
                InspectorWindow.Inspect(gameObject);
            }

            if(ImGui.BeginDragDropSource())
            {               
                ImGui.SetDragDropPayload("GameObject", IntPtr.Zero, 0);
                ImGui.Text(gameObject.name);
                ImGui.EndDragDropSource();
            }
            if(ImGui.BeginDragDropTarget())
            {
                if(ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ImGui.GetDragDropPayload().IsDataType("GameObject"))
                {
                    Undo.RegisterAction(_selectedGameObject, _selectedGameObject.parent, gameObject, nameof(GameObject.parent));
                    _selectedGameObject.parent = gameObject;
                    _dropped = true;
                }
                ImGui.EndDragDropTarget();
            }

            if(!isLeaf && nodeOpen)
            {
                for (int i = 0; i < gameObject.childCount; i++)
                    ShowGameObject(gameObject.GetChild(i));
                ImGui.TreePop();
            }

            ImGui.PopID();
        }
    }
}
