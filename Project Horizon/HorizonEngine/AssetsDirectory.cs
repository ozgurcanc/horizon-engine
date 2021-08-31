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
    internal class AssetsDirectory
    {
        private string _name;
        private AssetsDirectory _parent;
        private List<AssetsDirectory> _subdirectories;

        internal AssetsDirectory(string name)
        {
            _parent = null;
            _name = name;
            _subdirectories = new List<AssetsDirectory>();
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
            }
        }

        internal IReadOnlyCollection<AssetsDirectory> subdirectories
        {
            get
            {
                return _subdirectories.AsReadOnly();
            }
        }

        internal void Destroy()
        {
            this.parent = null;
        }




    }
}
