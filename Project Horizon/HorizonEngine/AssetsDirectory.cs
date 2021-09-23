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
using Newtonsoft.Json;

namespace HorizonEngine
{
    [JsonObject(MemberSerialization.Fields)]
    internal class AssetsDirectory
    {
        private string _name;
        private AssetsDirectory _parent;
        private List<AssetsDirectory> _subdirectories;
        private List<Asset> _assets;

        internal AssetsDirectory(string name)
        {
            _parent = null;
            _name = name;
            _subdirectories = new List<AssetsDirectory>();
            _assets = new List<Asset>();
        }

        internal bool isLeaf
        {
            get
            {
                return _subdirectories.Count == 0;
            }
        }

        internal AssetsDirectory parent
        {
            get
            {
                return _parent;
            }
            set
            {
                var temp = value;
                while (temp != null)
                {
                    if (temp == this) return;
                    temp = temp.parent;
                }
                if (_parent != null) _parent._subdirectories.Remove(this);
                _parent = value;
                if (value != null) _parent._subdirectories.Add(this);
                Assets.SetModified();
            }
        }

        internal string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                Assets.SetModified();
            }
        }

        internal IReadOnlyCollection<AssetsDirectory> subdirectories
        {
            get
            {
                return _subdirectories.AsReadOnly();
            }
        }

        internal IReadOnlyCollection<Asset> assets
        {
            get
            {
                return _assets.AsReadOnly();
            }
        }

        internal void Destroy()
        {
            this.parent = null;
            _assets.ForEach(x => x.Delete());
            foreach (var subDirectory in _subdirectories.ToArray()) subDirectory.Destroy();
            Assets.Save();
        }

        internal void Reload()
        {
            _assets.ForEach(x => x.Reload());
            _subdirectories.ForEach(x => x.Reload());
        }


        internal void AddAsset(Asset asset)
        {
            _assets.Add(asset);
            Assets.Save();
        }

        internal void DeleteAsset(Asset asset)
        {
            _assets.Remove(asset);
            asset.Delete();
            Assets.Save();
        }

        internal void MoveAsset(Asset asset, AssetsDirectory destination)
        {
            _assets.Remove(asset);
            destination.AddAsset(asset);
        }

    }
}
