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
using Newtonsoft.Json;
using System.Windows.Forms;

namespace HorizonEngine
{
    internal static class AssetsWindow
    {
        private static bool _enabled;
        private static ImGuiTreeNodeFlags _leafNodeFlag;
        private static ImGuiTreeNodeFlags _innerNodeFlag;
        private static AssetsDirectory _selectedDirectory;
        private static Asset _selectedAsset;
        private static int _pushID;
        private static bool _clicked;

        internal static void Init()
        {
            //_selectedDirectory = null;
            _leafNodeFlag = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            _innerNodeFlag = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;
            _selectedDirectory = Assets.rootDirectory;
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
            _clicked = false;
            bool deleteAssetFlag = false;
            bool deleteDirectoryFlag = false;

            string windowName = Assets.isModified ? "Assets*###1" : "Assets###1";
            if (!ImGui.Begin(windowName, ref _enabled, ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.End();
                return;
            }

            string name = _selectedDirectory.name;
            ImGui.Text("Current Folder");
            ImGui.SameLine();
            ImGuiInputTextFlags renameFlags = _selectedDirectory == Assets.rootDirectory ? ImGuiInputTextFlags.ReadOnly : ImGuiInputTextFlags.None;
            if (ImGui.InputText("##directoryName", ref name, 100, renameFlags))
            {
                _selectedDirectory.name = name;
            }

            ImGui.Separator();

            ImGui.Columns(2);

            ImGui.BeginChild("directory");

            ShowDirectory(Assets.rootDirectory);

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
                    deleteDirectoryFlag = true;
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Import New Font"))
                {
                    ImportFont();
                }
                if (ImGui.MenuItem("Import New Texture"))
                {
                    ImportTexture();
                }
                if (ImGui.MenuItem("Import New Audio"))
                {
                    ImportAudio();
                }
                if (ImGui.MenuItem("New Render Texture"))
                {
                    RenderTexture renderTexture = Assets.CreateRenderTexture("RenderTexture", 400, 400);
                    _selectedDirectory.AddAsset(renderTexture);
                }
                if (ImGui.MenuItem("New Animation"))
                {
                    Animation animation = Assets.CreateAnimation("Animation");
                    _selectedDirectory.AddAsset(animation);
                }
                if (ImGui.MenuItem("New Animator Controller"))
                {
                    AnimatorController animatorController = Assets.CreateAnimatorController("AnimatorController");
                    _selectedDirectory.AddAsset(animatorController);
                }
                if (ImGui.MenuItem("New Physics Material"))
                {
                    PhysicsMaterial physicsMaterial = Assets.CreatePhysicsMaterial("Physics Material");
                    _selectedDirectory.AddAsset(physicsMaterial);
                }
                ImGui.Separator();
                if (ImGui.MenuItem("Delete", _selectedAsset != null))
                {
                    deleteAssetFlag = true;
                }
                ImGui.EndPopup();               
            }

            if(deleteDirectoryFlag)
            {
                ImGui.OpenPopup("Delete Directory");               
            }
            if (ImGui.BeginPopupModal("Delete Directory"))
            {
                ImGui.Text(_selectedDirectory.name + " will be deleted");
                ImGui.TextColored(new System.Numerics.Vector4(1f, 0f, 0f, 1f), "You cannot undo this action");
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Delete"))
                {
                    GameWindow.isPlaying = false;
                    AssetsDirectory deletedDirectory = _selectedDirectory;
                    _selectedDirectory = _selectedDirectory.parent;
                    deletedDirectory.Destroy();
                    Scene.Reload();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }


            if (deleteAssetFlag)
            {
                ImGui.OpenPopup("Delete Asset");
            }
            if (ImGui.BeginPopupModal("Delete Asset"))
            {
                ImGui.Text(_selectedAsset.name + " will be deleted");
                ImGui.TextColored(new System.Numerics.Vector4(1f, 0f, 0f, 1f), "You cannot undo this action");
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Delete"))
                {
                    GameWindow.isPlaying = false;
                    _selectedDirectory.DeleteAsset(_selectedAsset);
                    Scene.Reload();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            foreach (Asset asset in _selectedDirectory.assets)
            {
                ShowAsset(asset);
            }

            ImGui.EndChild();

            if (!_clicked && (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right)))
            {
                _selectedAsset = null;
                InspectorWindow.Inspect(null);
            }

        }

        private static void ShowAsset(Asset asset)
        {
            bool isTexture = asset is HorizonEngine.Texture;
            bool isInner = isTexture && ((HorizonEngine.Texture)asset).internalTextures.Count > 0;
            ImGuiTreeNodeFlags flags = isInner ? _innerNodeFlag : _leafNodeFlag;

            if (_selectedAsset != null && asset.assetID == _selectedAsset.assetID) flags |= ImGuiTreeNodeFlags.Selected;

            ImGui.PushID(asset.assetID.ToString());

            bool nodeOpen = ImGui.TreeNodeEx(asset.name, flags);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                _selectedAsset = asset;
                _clicked = true;
                InspectorWindow.Inspect(asset);
            }

            if(!isTexture || (isTexture && ((HorizonEngine.Texture)asset).isOriginal))
            {
                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);
                    ImGui.Text(asset.name);
                    ImGui.EndDragDropSource();
                }
            }

            if (isInner && nodeOpen)
            {
                foreach (var internalTexture in ((Texture)asset).internalTextures)
                {
                    ShowAsset(internalTexture);
                }  
                ImGui.TreePop();
            }

            ImGui.PopID();
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
                _selectedAsset = null;
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
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ImGui.GetDragDropPayload().IsDataType("Asset"))
                {
                    _selectedDirectory.MoveAsset(_selectedAsset, directory);
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

        private static void ImportFont()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Application.assetsPath,
                Title = "Import New Font",

                CheckFileExists = true,
                CheckPathExists = true,

                //DefaultExt = "txt",
                Filter = "Font files (*.ttf)|*.ttf",
                //FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //openFileDialog.FileName;
                File.Copy(openFileDialog.FileName, Path.Combine(Application.assetsPath, openFileDialog.SafeFileName), true);
                Font font = Assets.CreateFont(openFileDialog.SafeFileName, openFileDialog.SafeFileName);
                _selectedDirectory.AddAsset(font);
            }
        }

        private static void ImportTexture()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Application.assetsPath,
                Title = "Import New Texture",

                CheckFileExists = true,
                CheckPathExists = true,

                //DefaultExt = "txt",
                Filter = "Image files (*.png, *jpg)|*.png; *.jpg",
                //FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true,

                Multiselect = true,
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //openFileDialog.FileName;
                for(int i=0; i<openFileDialog.FileNames.Length; i++)
                {
                    File.Copy(openFileDialog.FileNames[i], Path.Combine(Application.assetsPath, openFileDialog.SafeFileNames[i]), true);
                    HorizonEngine.Texture texture = Assets.CreateTexture(openFileDialog.SafeFileNames[i], openFileDialog.SafeFileNames[i]);
                    _selectedDirectory.AddAsset(texture);
                }               
            }
        }

        private static void ImportAudio()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Application.assetsPath,
                Title = "Import New Audio",

                CheckFileExists = true,
                CheckPathExists = true,

                //DefaultExt = "txt",
                Filter = "Audio files (*.wav)|*.wav",
                //FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true,

                Multiselect = true,
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //openFileDialog.FileName;
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    File.Copy(openFileDialog.FileNames[i], Path.Combine(Application.assetsPath, openFileDialog.SafeFileNames[i]), true);
                    AudioClip audioClip = Assets.CreateAudioClip(openFileDialog.SafeFileNames[i], openFileDialog.SafeFileNames[i]);
                    _selectedDirectory.AddAsset(audioClip);
                }
            }
        }
    }
}
