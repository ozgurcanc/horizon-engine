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
    internal static class AnimatorWindow
    {
        private static bool _enabled;
        private static AnimatorController _animatorController;
        private static Animation _currentAnimation;
        private static int _transitionFrom;
        private static int _transitionTo;

        static AnimatorWindow()
        {
            _animatorController = null;
            _currentAnimation = null;
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

            ImGui.Begin("Animator", ref _enabled, ImGuiWindowFlags.MenuBar);

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.RadioButton("Open Animator Controller", true))
                {
                    ImGui.OpenPopup("select_animator_controller");
                }

                if (ImGui.BeginPopup("select_animator_controller"))
                {
                    foreach (AnimatorController animatorController in Assets.animatorControllers)
                    {
                        if (ImGui.Selectable(animatorController.name)) Open(animatorController);
                    }
                    ImGui.EndPopup();
                }

                ImGui.EndMenuBar();
            }

            if (_animatorController == null) return;

            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            ImGui.Text("name");
            ImGui.SameLine();
            string name = _animatorController.name;
            if (ImGui.InputText("##animatorControllerName", ref name, 100))
            {
                _animatorController.name = name;
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("Add Animation", true))
            {
                ImGui.OpenPopup("add_new_animation");
            }

            if(ImGui.BeginPopup("add_new_animation"))
            {
                foreach(Animation animation in Assets.animations)
                {
                    if (ImGui.Selectable(animation.name)) _animatorController.AddAnimation(animation);
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("Add Parameter", true))
            {
                ImGui.OpenPopup("add_new_parameter");
            }

            if (ImGui.BeginPopup("add_new_parameter"))
            {

                if (ImGui.Selectable("Int")) { _animatorController.AddParameter(new IntParameter("Int", 0)); }
                if (ImGui.Selectable("Float")) { _animatorController.AddParameter(new FloatParameter("Float", 0f)); }
                if (ImGui.Selectable("Bool")) { _animatorController.AddParameter(new BoolParameter("Bool", false)); }
                if (ImGui.Selectable("Trigger")) { _animatorController.AddParameter(new TriggerParameter("Trigger")); }

                ImGui.EndPopup();
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("Add Transition", true))
            {
                _transitionFrom = 0;
                _transitionTo = 0;
                ImGui.OpenPopup("add_new_transition");
            }

            if(ImGui.BeginPopup("add_new_transition"))
            {
                var anims = _animatorController.animations;
                string[] animations = anims.Select(x => x.name).ToArray();
                ImGui.Combo("from", ref _transitionFrom, animations, animations.Length);
                ImGui.Combo("to", ref _transitionTo, animations, animations.Length);
                ImGui.Separator();
                if(ImGui.Button("OK"))
                {
                    if(animations.Length > 0)
                        _animatorController.AddTransition(new AnimatorTransition(anims.ElementAt(_transitionFrom), anims.ElementAt(_transitionTo)));
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ImGui.PopItemWidth();
            ImGui.Separator();

            ImGui.Columns(3);

            ImGui.BeginChild("controller_animations");
            ImGui.Text("Animations");
            ImGui.Separator();
            int i = 0;
            foreach(Animation anim in _animatorController.animations)
            {
                i++;
                ImGui.PushID(i);
                if (ImGui.RadioButton("##delete", true)) _animatorController.RemoveAnimation(anim);
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Delete");
                    ImGui.EndTooltip();
                }
                ImGui.SameLine();
                if (ImGui.Selectable(anim.name, anim == _animatorController.defaultAnimation)) { }              

                if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    _currentAnimation = anim;
                    ImGui.OpenPopup("animation_settings");
                }

                if (ImGui.BeginPopup("animation_settings"))
                {
                    if (ImGui.Selectable("Make Default"))
                    {
                        _animatorController.SetDefaultAnimation(_currentAnimation);
                    }
                    ImGui.EndPopup();
                }

                ImGui.PopID();
            }          
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.NextColumn();
            ImGui.BeginChild("controler_parameters");
            ImGui.Text("Parameters");
            ImGui.Separator();
            i = 0;
            foreach(AnimatorParameter parameter in _animatorController.parameters.ToArray())
            {
                i++;
                ImGui.PushID(i);
                if (ImGui.RadioButton("##delete", true)) _animatorController.RemoveParameter(parameter);
                if(ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Delete");
                    ImGui.EndTooltip();
                }
                ImGui.SameLine();
                parameter.OnAnimatorGUI();
                ImGui.PopID();
            }
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.NextColumn();
            ImGui.BeginChild("controler_transitions");
            ImGui.Text("Transitions");
            ImGui.Separator();
            i = 0;
            foreach (List<AnimatorTransition> transitionList in _animatorController.transitions)
            {               
                foreach (AnimatorTransition transition in transitionList.ToArray())
                {
                    i++;
                    ImGui.PushID(i);
                    if (ImGui.RadioButton("##delete", true)) _animatorController.RemoveTransition(transition);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted("Delete");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    transition.OnAnimatorGUI(_animatorController.parameters);
                    ImGui.PopID();
                }
            }           
            ImGui.EndChild();

            ImGui.End();
        }

        internal static void Open(AnimatorController animatorController)
        {
            _animatorController = animatorController;
        }
    }
}
