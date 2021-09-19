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
        private static AssetsDirectory _rootDirectory;
        private static int _selectedAssetID;
        private static Asset _dragAsset;
        private static int _pushID;

        static AssetsWindow()
        {
            _selectedDirectory = null;
            _leafNodeFlag = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            _innerNodeFlag = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;

            _rootDirectory = new AssetsDirectory("Assets");
            _selectedDirectory = _rootDirectory;

            _selectedAssetID = -1;
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

        internal static void Save()
        {
            File.WriteAllText("assets.json", JsonConvert.SerializeObject(_rootDirectory));
            Assets.Save();
        }

        internal static void Load()
        {
            Assets.Load();
            _rootDirectory = JsonConvert.DeserializeObject<AssetsDirectory>(File.ReadAllText("assets.json"));
            _selectedDirectory = _rootDirectory;
            _rootDirectory.Reload();
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
                ImGui.Separator();
                if (ImGui.MenuItem("Import New Font"))
                {
                    ImportFont();
                }
                if (ImGui.MenuItem("Import New Texture"))
                {
                    ImportTexture();
                }
                if(ImGui.MenuItem("New Render Texture"))
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
                ImGui.EndPopup();               
            }

            foreach (Asset asset in _selectedDirectory.assets)
            {
                ShowAsset(asset);
            }

            ImGui.EndChild();

        }

        private static void ShowAsset(Asset asset)
        {
            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

            if (asset.assetID == _selectedAssetID) flags |= ImGuiTreeNodeFlags.Selected;

            ImGui.PushID(asset.assetID.ToString());

            bool nodeOpen = ImGui.TreeNodeEx(asset.name, flags);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                _selectedAssetID = (int)asset.assetID;
            }

            if (ImGui.BeginDragDropSource())
            {
                _dragAsset = asset;
                ImGui.SetDragDropPayload("Asset", IntPtr.Zero, 0);
                ImGui.Text(asset.name);
                ImGui.EndDragDropSource();
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
                    _selectedDirectory.RemoveAsset(_dragAsset);
                    directory.AddAsset(_dragAsset);
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
                InitialDirectory = Assets.path,
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
                File.Copy(openFileDialog.FileName, Path.Combine(Assets.path, openFileDialog.SafeFileName), true);
                Font font = Assets.CreateFont(openFileDialog.SafeFileName, openFileDialog.SafeFileName);
                _selectedDirectory.AddAsset(font);
            }
        }

        private static void ImportTexture()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Assets.path,
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
                    File.Copy(openFileDialog.FileNames[i], Path.Combine(Assets.path, openFileDialog.SafeFileNames[i]), true);
                    HorizonEngine.Texture texture = Assets.CreateTexture(openFileDialog.SafeFileNames[i], openFileDialog.SafeFileNames[i]);
                    _selectedDirectory.AddAsset(texture);
                }               
            }
        }
    }
}
