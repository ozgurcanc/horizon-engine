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
    internal static class ProjectSettingsWindow
    {
        private static bool _enabled;
        private static int _selected;
        private static string[] _settings;

        static ProjectSettingsWindow()
        {
            _selected = -1;
            _settings = new string[] { "Audio", "Editor", "Graphics", "Game", "Physics", "Time" };
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

            ImGui.Begin("Project Settings", ref _enabled, ImGuiWindowFlags.MenuBar);

            ImGui.BeginChild("left", new System.Numerics.Vector2(150, 0), true);
            for(int i=0; i<_settings.Length; i++)
            {
                if(ImGui.Selectable(_settings[i], i == _selected))
                {
                    _selected = i;
                }
            }
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.BeginChild("right", new System.Numerics.Vector2(0, -ImGui.GetFrameHeightWithSpacing()));
            if (_selected == 0) AudioSettings();
            else if (_selected == 4) PhysicsSettings();
            ImGui.EndChild();
            if(ImGui.Button("Revert"))
            {
                Audio.LoadSettings();
                Physics.LoadSettings();
            }
            ImGui.SameLine();
            if(ImGui.Button("Save"))
            {
                Audio.SaveSettings();
                Physics.SaveSettings();
            }
            ImGui.EndGroup();

            ImGui.End();
        }

        private static void AudioSettings()
        {
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            float masterVolume = Audio.masterVolume;
            ImGui.Text("Master Volume");
            ImGui.SameLine();
            if(ImGui.DragFloat("##masterVolume", ref masterVolume, 0.02f))
            {
                Audio.masterVolume = masterVolume;
            }

            float dopplerScale = Audio.dopplerScale;
            ImGui.Text("Doppler Scale");
            ImGui.SameLine();
            if (ImGui.DragFloat("##dopplerScale", ref dopplerScale, 0.02f))
            {
                Audio.dopplerScale = dopplerScale;
            }

            float speedOfSound = Audio.speedOfSound;
            ImGui.Text("Speed of Sound");
            ImGui.SameLine();
            if (ImGui.DragFloat("##speedOfSound", ref speedOfSound, 0.02f))
            {
                Audio.speedOfSound = speedOfSound;
            }

            float distanceScale = Audio.distanceScale;
            ImGui.Text("Distance Scale");
            ImGui.SameLine();
            if (ImGui.DragFloat("##distanceScale", ref distanceScale, 0.02f))
            {
                Audio.distanceScale = distanceScale;
            }

            ImGui.PopItemWidth();
        }

        private static void PhysicsSettings()
        {
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.35f);

            Vector2 gravity = Physics.gravity;
            ImGui.Text("Gravity");
            ImGui.SameLine();
            ImGui.Text("X");
            ImGui.SameLine();
            if(ImGui.DragFloat("##gravityX", ref gravity.X))
            {
                Physics.gravity = gravity;
            }
            ImGui.SameLine();
            ImGui.Text("Y");
            ImGui.SameLine();
            if (ImGui.DragFloat("##gravityY", ref gravity.Y))
            {
                Physics.gravity = gravity;
            }

            float defaultFriction = Physics.defaultFriction;
            ImGui.Text("Default Friction");
            ImGui.SameLine();
            if (ImGui.DragFloat("##defaultFriction", ref defaultFriction, 0.02f))
            {
                Physics.defaultFriction = defaultFriction;
            }

            float defaultRestitution = Physics.defaultRestitution;
            ImGui.Text("Default Restitution");
            ImGui.SameLine();
            if (ImGui.DragFloat("##defaultRestitution", ref defaultRestitution, 0.02f))
            {
                Physics.defaultRestitution = defaultRestitution;
            }

            string[] physicsMaterialBlendModes = Enum.GetNames(typeof(PhysicsMaterialBlendMode));
            
            int frictionBlendMode = (int)Physics.frictionBlendMode;
            ImGui.Text("Friction Blend Mode");
            ImGui.SameLine();
            if (ImGui.Combo("##frictionBlendMode", ref frictionBlendMode, physicsMaterialBlendModes, physicsMaterialBlendModes.Length))
            {
                Physics.frictionBlendMode = (PhysicsMaterialBlendMode)frictionBlendMode;
            }

            int restitutionBlendMode = (int)Physics.restitutionBlendMode;
            ImGui.Text("Restitution Blend Mode");
            ImGui.SameLine();
            if (ImGui.Combo("##restitutionBlendMode", ref restitutionBlendMode, physicsMaterialBlendModes, physicsMaterialBlendModes.Length))
            {
                Physics.restitutionBlendMode = (PhysicsMaterialBlendMode)restitutionBlendMode;
            }

            ImGui.PopItemWidth();
        }
    }
}
