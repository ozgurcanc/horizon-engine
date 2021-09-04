using System;
using System.Collections.Generic;
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
    public abstract class AnimatorParameter
    {
        private string _name;
        private object _value;

        protected AnimatorParameter(string name, object value)
        {
            _name = name;
            _value = value;
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

        internal object value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        internal abstract AnimatorParameter Copy();

        internal abstract AnimatorCondition Condition();

        internal abstract void OnAnimatorGUI();

        internal virtual void Reload() { }
    }


    public class IntParameter : AnimatorParameter
    {
        public IntParameter(string name, int value) : base(name, value) { }
       
        internal override AnimatorParameter Copy()
        {
            return new IntParameter(this.name, (int)this.value);
        }

        internal override AnimatorCondition Condition()
        {
            return new IntCondition(this, 0, ComparisonType.Equals);
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            string name = this.name;
            if (ImGui.InputText("##parameterName", ref name, 100))
            {
                this.name = name;
            }
            ImGui.SameLine();
            int value = (int)this.value;
            if (ImGui.InputInt("##parameterValue", ref value))
            {
                this.value = value;
            }
            ImGui.PopItemWidth();
        }

        internal override void Reload()
        {
            this.value = (int)(Int64)this.value;
        }
    }

    public class FloatParameter : AnimatorParameter
    {
        public FloatParameter(string name, float value) : base(name, value) { }

        internal override AnimatorParameter Copy()
        {
            return new FloatParameter(this.name, (float)this.value);
        }

        internal override AnimatorCondition Condition()
        {
            return new FloatCondition(this, 0, ComparisonType.Equals);
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            string name = this.name;
            if (ImGui.InputText("##parameterName", ref name, 100))
            {
                this.name = name;
            }
            ImGui.SameLine();
            float value = (float)this.value;
            if (ImGui.InputFloat("##parameterValue", ref value))
            {
                this.value = value;
            }
            ImGui.PopItemWidth();
        }

        internal override void Reload()
        {
            this.value = (float)(double)this.value;
        }
    }

    public class BoolParameter : AnimatorParameter
    {
        public BoolParameter(string name, bool value) : base(name, value) { }

        internal override AnimatorParameter Copy()
        {
            return new BoolParameter(this.name, (bool)this.value);
        }

        internal override AnimatorCondition Condition()
        {
            return new BoolCondition(this, false);
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            string name = this.name;
            if (ImGui.InputText("##parameterName", ref name, 100))
            {
                this.name = name;
            }
            ImGui.SameLine();
            bool value = (bool)this.value;
            if (ImGui.Checkbox("##parameterValue", ref value))
            {
                this.value = value;
            }
            ImGui.PopItemWidth();
        }
    }

    public class TriggerParameter : AnimatorParameter
    {
        public TriggerParameter(string name) : base(name, false) { }

        internal override AnimatorParameter Copy()
        {
            return new TriggerParameter(this.name);
        }

        internal override AnimatorCondition Condition()
        {
            return new TriggerCondition(this);
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            string name = this.name;
            if (ImGui.InputText("##parameterName", ref name, 100))
            {
                this.name = name;
            }
            ImGui.SameLine();
            bool value = (bool)this.value;
            if (ImGui.Checkbox("##parameterValue", ref value))
            {
                this.value = value;
            }
            ImGui.PopItemWidth();
        }
    }
}
