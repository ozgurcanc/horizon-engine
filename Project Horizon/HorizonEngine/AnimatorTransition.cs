using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Newtonsoft.Json;
using ImGuiNET;

namespace HorizonEngine
{
    [JsonObject(MemberSerialization.Fields)]
    internal class AnimatorTransition
    {
        private Animation _from;
        private Animation _to;
        private bool _hasExitTime;
        private float _exitTime;
        private float _duration;
        private List<AnimatorCondition> _conditions;

        internal AnimatorTransition(Animation from, Animation to)
        {
            _from = from;
            _to = to;
            _hasExitTime = false;
            _exitTime = 0f;
            _duration = 0f;
            _conditions = new List<AnimatorCondition>();
        }

        internal Animation from
        {
            get
            {
                return _from;
            }
        }

        internal Animation to
        {
            get
            {
                return _to;
            }
        }

        internal bool hasExitTime
        {
            get
            {
                return _hasExitTime;
            }
            set
            {
                _hasExitTime = value;
            }
        }

        internal float exitTime
        {
            get
            {
                return _exitTime;
            }
            set
            {
                _exitTime = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        internal float duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value >= 0f ? value : 0f;
            }
        }

        internal ReadOnlyCollection<AnimatorCondition> conditions
        {
            get
            {
                return _conditions.AsReadOnly();
            }
        }

        internal void AddCondition(AnimatorCondition condition)
        {
            _conditions.Add(condition);
        }

        internal void RemoveCondition(AnimatorCondition condition)
        {
            _conditions.Remove(condition);
        }

        internal void OnAnimatorGUI(ReadOnlyCollection<AnimatorParameter> parameters)
        {
            if (!ImGui.CollapsingHeader(_from.name + " -> " + _to.name))
                return;

            float duration = this.duration;
            ImGui.Text("Duration");
            ImGui.SameLine();
            if (ImGui.InputFloat("##duration", ref duration))
            {
                this.duration = duration;
            }

            bool hasExitTime = this.hasExitTime;
            ImGui.Text("Has Exit Time");
            ImGui.SameLine();
            if(ImGui.Checkbox("##hasExitTime", ref hasExitTime))
            {
                this.hasExitTime = hasExitTime;
            }

            float exitTime = this.exitTime;
            ImGui.Text("Exit Time");
            ImGui.SameLine();
            if(ImGui.SliderFloat("##exitTime", ref exitTime, 0f, 1f))
            {
                this.exitTime = exitTime;
            }

            if(ImGui.RadioButton("Add Condition", true))
            {
                ImGui.OpenPopup("add_condition_to_transition");
            }

            if(ImGui.BeginPopup("add_condition_to_transition"))
            {
                foreach(AnimatorParameter parameter in parameters)
                {
                    if(ImGui.Selectable(parameter.name))
                    {
                        _conditions.Add(parameter.Condition());
                    }
                }
                ImGui.EndPopup();
            }

            ImGui.Text("Conditions");
            ImGui.Separator();

            int i = 0;
            foreach(AnimatorCondition condition in _conditions)
            {
                i++;
                ImGui.PushID(i);
                condition.OnAnimatorGUI();
                ImGui.PopID();
            }

        }
    }
}
