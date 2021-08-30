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
        private static IEditor _inspectorObject;

        static InspectorWindow()
        {
            _inspectorObject = null;
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

            if (_inspectorObject != null)
                _inspectorObject.OnInspectorGUI();

            ImGui.End();
        }

        internal static void Inspect(IEditor editorObject)
        {
            _inspectorObject = editorObject;
        }
    }
}
