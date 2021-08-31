﻿using System;
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
    internal static class AssetsWindow
    {
        private static bool _enabled;
        private static ImGuiTreeNodeFlags _leafNodeFlag;
        private static ImGuiTreeNodeFlags _innerNodeFlag;
        private static AssetsDirectory _selectedDirectory;
        private static AssetsDirectory _rootDirectory;
        private static int _pushID;

        static AssetsWindow()
        {
            _selectedDirectory = null;
            _leafNodeFlag = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            _innerNodeFlag = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;

            _rootDirectory = new AssetsDirectory("Assets");
            _selectedDirectory = _rootDirectory;
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

            if (!ImGui.Begin("Assets", ref _enabled, ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.End();
                return;
            }

            string name = _selectedDirectory.name;
            ImGui.Text("Current Folder");
            ImGui.SameLine();
            ImGuiInputTextFlags renameFlags = _selectedDirectory == _rootDirectory ? ImGuiInputTextFlags.ReadOnly : ImGuiInputTextFlags.None;
            if (ImGui.InputText("##directoryName", ref name, 100, renameFlags))
            {
                _selectedDirectory.name = name;
            }

            ImGui.Separator();

            ImGui.Columns(2);

            ImGui.BeginChild("directory");

            ShowDirectory(_rootDirectory);

            ImGui.EndChild();

            ImGui.SameLine();

            ImGui.NextColumn();

            ImGui.BeginChild("file");

            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.MenuItem("New Folder"))
                {
                    AssetsDirectory newDirectory = new AssetsDirectory("New Folder");
                    newDirectory.parent = _selectedDirectory;
                }
                if (ImGui.MenuItem("Delete Folder", _selectedDirectory.parent != null))
                {
                    AssetsDirectory deletedDirectory = _selectedDirectory;
                    _selectedDirectory = _selectedDirectory.parent;
                    deletedDirectory.Destroy();
                }
                ImGui.EndPopup();
            }

            ImGui.EndChild();

        }

        private static void ShowDirectory(AssetsDirectory directory)
        {
            ImGuiTreeNodeFlags flags;
            bool isLeaf = directory.isLeaf;
            flags = isLeaf ? _leafNodeFlag : _innerNodeFlag;

            if (directory == _selectedDirectory) flags |= ImGuiTreeNodeFlags.Selected;

            ImGui.PushID(_pushID++);

            bool nodeOpen = ImGui.TreeNodeEx(directory.name, flags);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                _selectedDirectory = directory;
            }

            if (ImGui.BeginDragDropSource())
            {
                ImGui.SetDragDropPayload("Directory", IntPtr.Zero, 0);
                ImGui.Text(directory.name);
                ImGui.EndDragDropSource();
            }
            if (ImGui.BeginDragDropTarget())
            {
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ImGui.GetDragDropPayload().IsDataType("Directory"))
                {
                    _selectedDirectory.parent = directory;
                }
                ImGui.EndDragDropTarget();
            }

            if (!isLeaf && nodeOpen)
            {
                foreach (AssetsDirectory subdirectory in directory.subdirectories.ToArray())
                    ShowDirectory(subdirectory);
                ImGui.TreePop();
            }

            ImGui.PopID();
        }
    }
}
