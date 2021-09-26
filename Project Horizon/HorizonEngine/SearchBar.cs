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
    internal static class SearchBar
    {
        private static ImGuiTextFilterPtr _filter;

        static SearchBar()
        {
            unsafe
            {
                _filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
            }
        }

        internal static void Draw()
        {
            _filter.Draw("##searchBar");
        }

        internal static bool PassFilter(string text)
        {
            return _filter.PassFilter(text);
        }

        internal static void Clear()
        {
            _filter.Clear();
        }
    }
}
