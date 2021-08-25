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
    internal static class InspectorWindow
    {
        private static bool _enabled;
        private static List<Inspector> _inspectors;

        static InspectorWindow()
        {
            _inspectors = new List<Inspector>();
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
            if (!_enabled) return;

            ImGui.Begin("Inspector", ref _enabled);

            foreach(Inspector inspector in _inspectors)
            {
                inspector.Draw();
            }

            ImGui.End();
        }

        internal static void Inspect(GameObject gameObject)
        {
            _inspectors.Clear();
            _inspectors.Add(new GameObjectInspector(gameObject));
        }
    }
}
