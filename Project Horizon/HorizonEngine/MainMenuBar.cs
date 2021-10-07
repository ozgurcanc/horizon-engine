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
using System.IO;

namespace HorizonEngine
{
    internal static class MainMenuBar
    {
        private static string _sceneName;
        private static Action<string> _actionAfterSave;

        internal static void Draw()
        {
            bool newSceneFlag = false;
            bool renameSceneFlag = false;
            bool deleteSceneFlag = false;
            bool saveScenePopUp = false;

            if(ImGui.BeginMainMenuBar())
            {
                if(ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Scene")) 
                    {                      
                        newSceneFlag = true; 
                    }                  
                    if (ImGui.BeginMenu("Open Scene"))
                    {
                        var scenes = Directory.GetFiles(Application.scenesPath).Select(x => Path.GetFileName(x));
                        foreach(var x in scenes)
                        {
                            if(ImGui.MenuItem(x))
                            {
                                _actionAfterSave = Application.LoadScene;
                                _sceneName = x;
                                saveScenePopUp = true;
                            }
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Rename Scene", Scene.name != null)) 
                    {
                        renameSceneFlag = true; 
                    }
                    if (ImGui.MenuItem("Delete Scene", Scene.name != null)) 
                    {
                        deleteSceneFlag = true; 
                    }
                    if (ImGui.MenuItem("Save Scene", Scene.name != null)) 
                    {
                        Application.SaveScene();
                    }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Exit")) { Application.Quit(); }
                    ImGui.EndMenu();
                }
                if(ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z", false, Undo.canUndo)) { Undo.PerformUndo();  }
                    if (ImGui.MenuItem("Redo", "CRTL+Y", false, Undo.canRedo)) { Undo.PerformRedo();  }
                    ImGui.Separator();
                    /*
                    if (ImGui.MenuItem("Cut", "CTRL+X")) { }
                    if (ImGui.MenuItem("Copy", "CTRL+C")) { }
                    if (ImGui.MenuItem("Paste", "CTRL+V")) { }
                    ImGui.Separator();
                    */
                    if (ImGui.MenuItem("Play", null, GameWindow.isPlaying)) { GameWindow.isPlaying = !GameWindow.isPlaying; }
                    if (ImGui.MenuItem("Pause", null, GameWindow.isPaused)) { GameWindow.isPaused = !GameWindow.isPaused; }

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
                            _actionAfterSave = Application.NewScene;
                            saveScenePopUp = true;
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
                            Application.RenameScene(_sceneName);
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
                        Application.DeleteScene();
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }

                if (saveScenePopUp)
                {
                    if(Undo.isSceneModified) ImGui.OpenPopup("Scene Have Been Modified");
                    else _actionAfterSave.Invoke(_sceneName);
                }

                if (ImGui.BeginPopupModal("Scene Have Been Modified"))
                {
                    ImGui.Text("Do you want to save changes in the scene.");
                    ImGui.Text("Your changes will be lost if you don't save.");
                    ImGui.NewLine();

                    if (ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Don't Save"))
                    {
                        _actionAfterSave.Invoke(_sceneName);
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Save"))
                    {
                        Application.SaveScene();
                        _actionAfterSave.Invoke(_sceneName);
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
            }
        }
    }
}
