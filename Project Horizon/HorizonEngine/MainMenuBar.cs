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
    internal static class MainMenuBar
    {

        internal static void Draw()
        {
            if(ImGui.BeginMainMenuBar())
            {
                if(ImGui.BeginMenu("File"))
                {
                    ImGui.EndMenu();
                }
                if(ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z")) { }
                    if (ImGui.MenuItem("Redo", "CRTL+Y", false, false)) { }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Cut", "CTRL+X")) { }
                    if (ImGui.MenuItem("Copy", "CTRL+C")) { }
                    if (ImGui.MenuItem("Paste", "CTRL+V")) { }

                    ImGui.EndMenu();
                }
                if(ImGui.BeginMenu("Window"))
                {
                    if (ImGui.MenuItem("Hierarchy", null, HierarchyWindow.enabled)) { HierarchyWindow.enabled = !HierarchyWindow.enabled; }
                    if (ImGui.MenuItem("Game", null, GameWindow.enabled)) { GameWindow.enabled = !GameWindow.enabled; }
                    if (ImGui.MenuItem("Inspector", null, InspectorWindow.enabled)) { InspectorWindow.enabled = !InspectorWindow.enabled; }
                    if (ImGui.MenuItem("Assets", null, AssetsWindow.enabled)) { AssetsWindow.enabled = !AssetsWindow.enabled; }

                    ImGui.EndMenu();
                }


                ImGui.EndMainMenuBar();
            }
        }
    }
}
