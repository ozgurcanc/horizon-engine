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
using System.IO;

namespace HorizonEngine
{
    internal static class MainMenuBar
    {
        private static string _sceneName;

        internal static void Draw()
        {
            bool newSceneFlag = false;
            bool renameSceneFlag = false;
            bool deleteSceneFlag = false;

            if(ImGui.BeginMainMenuBar())
            {
                if(ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Scene")) { newSceneFlag = true; }                  
                    if(ImGui.BeginMenu("Open Scene"))
                    {
                        var scenes = Directory.GetFiles(Application.scenesPath).Select(x => Path.GetFileName(x));
                        foreach(var x in scenes)
                        {
                            if(ImGui.MenuItem(x))
                            {
                                Scene.Load(x);
                            }
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Rename Scene", Scene.name != null)) { renameSceneFlag = true; }
                    if (ImGui.MenuItem("Delete Scene", Scene.name != null)) { deleteSceneFlag = true; }
                    if (ImGui.MenuItem("Save Scene", Scene.name != null)) { Scene.Save(); }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Exit")) { Application.Quit(); }
                    ImGui.EndMenu();
                }
                if(ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z", false, Undo.canUndo)) { Undo.PerformUndo();  }
                    if (ImGui.MenuItem("Redo", "CRTL+Y", false, Undo.canRedo)) { Undo.PerformRedo();  }
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
                    if (ImGui.MenuItem("Animation", null, AnimationWindow.enabled)) { AnimationWindow.enabled = !AnimationWindow.enabled; }
                    if (ImGui.MenuItem("Animator", null, AnimatorWindow.enabled)) { AnimatorWindow.enabled = !AnimatorWindow.enabled; }
                    if (ImGui.MenuItem("Sprite Editor", null, SpriteEditorWindow.enabled)) { SpriteEditorWindow.enabled = !SpriteEditorWindow.enabled; }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();

                if (newSceneFlag)
                {
                    ImGui.OpenPopup("New Scene");
                    _sceneName = "New Scene";
                }

                if(ImGui.BeginPopupModal("New Scene"))
                {
                    ImGui.InputText("Scene Name", ref _sceneName, 100);

                    bool sceneNameExists = File.Exists(Path.Combine(Application.scenesPath, _sceneName));
                    if (sceneNameExists)
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(1f, 0, 0, 1f), "Scene name already exists.");
                    }
                    
                    if(ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Create"))
                    {
                        if(!sceneNameExists)
                        {
                            Scene.NewScene(_sceneName);
                            ImGui.CloseCurrentPopup();
                        }
                    }
                    ImGui.EndPopup();
                }

                if(renameSceneFlag)
                {
                    _sceneName = Scene.name;
                    ImGui.OpenPopup("Rename Scene");
                }

                if(ImGui.BeginPopupModal("Rename Scene"))
                {
                    ImGui.InputText("Scene Name", ref _sceneName, 100);

                    bool sceneNameExists = File.Exists(Path.Combine(Application.scenesPath, _sceneName));
                    if (sceneNameExists)
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(1f, 0, 0, 1f), "Scene name already exists.");
                    }

                    if (ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Rename"))
                    {
                        if (!sceneNameExists)
                        {
                            Scene.Rename(_sceneName);
                            ImGui.CloseCurrentPopup();
                        }
                    }

                    ImGui.EndPopup();
                }

                if (deleteSceneFlag)
                {
                    _sceneName = Scene.name;
                    ImGui.OpenPopup("Delete Scene");
                }

                if (ImGui.BeginPopupModal("Delete Scene"))
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1f, 0, 0, 1f), Scene.name + " will be deleted.");

                    if (ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Delete"))
                    {
                        Scene.DeleteScene();
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
            }
        }
    }
}
