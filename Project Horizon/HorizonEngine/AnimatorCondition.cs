using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ImGuiNET;

namespace HorizonEngine
{
    public enum ComparisonType
    {
        Equals,
        NotEquals,
        Greater,
        Less,
    }

    [JsonObject(MemberSerialization.Fields)]
    public abstract class AnimatorCondition
    {
        private ComparisonType _comparison;
        private object _targetValue;
        private AnimatorParameter _parameter;

        protected AnimatorCondition(AnimatorParameter parameter, ComparisonType comparison, object targetValue)
        {
            _comparison = comparison;
            _targetValue = targetValue;
            _parameter = parameter;
        }

        internal ComparisonType comparison
        {
            get
            {
                return _comparison;
            }
            set
            {
                _comparison = value;
            }
        }

        internal object targetValue
        {
            get
            {
                return _targetValue;
            }
            set
            {
                _targetValue = value;
            }
        }

        internal AnimatorParameter parameter
        {
            get
            {
                return _parameter;
            }
        }

        internal abstract bool CheckCondition();

        internal abstract void OnAnimatorGUI();

    }

    public class IntCondition : AnimatorCondition
    {
        public IntCondition(IntParameter parameter, int targetValue, ComparisonType comparison) : base(parameter, comparison, targetValue) { }

        internal override bool CheckCondition()
        {
            if (comparison == ComparisonType.Equals) return parameter.value.Equals(targetValue);
            else if (comparison == ComparisonType.NotEquals) return !parameter.value.Equals(targetValue);
            else if (comparison == ComparisonType.Greater) return (int)parameter.value > (int)targetValue;
            else return (int)parameter.value < (int)targetValue;
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.Text(parameter.name);
            ImGui.SameLine();
            int comparison = (int)this.comparison;
            string[] comparisons = Enum.GetNames(typeof(ComparisonType));
            if(ImGui.Combo("##combo", ref comparison, comparisons, comparisons.Length))
            {
                this.comparison = (ComparisonType)comparison;
            }
            ImGui.SameLine();

            int targetValue = (int)this.targetValue;
            if(ImGui.InputInt("##targetValue", ref targetValue))
            {
                this.targetValue = targetValue;
            }
        }
    }

    public class FloatCondition : AnimatorCondition
    {
        public FloatCondition(FloatParameter parameter, float targetValue, ComparisonType comparison) : base(parameter, comparison, targetValue) { }

        internal override bool CheckCondition()
        {
            if (comparison == ComparisonType.Equals) return parameter.value.Equals(targetValue);
            else if (comparison == ComparisonType.NotEquals) return !parameter.value.Equals(targetValue);
            else if (comparison == ComparisonType.Greater) return (float)parameter.value > (float)targetValue;
            else return (float)parameter.value < (float)targetValue;
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.Text(parameter.name);
            ImGui.SameLine();
            int comparison = (int)this.comparison;
            string[] comparisons = Enum.GetNames(typeof(ComparisonType));
            if (ImGui.Combo("##combo", ref comparison, comparisons, comparisons.Length))
            {
                this.comparison = (ComparisonType)comparison;
            }
            ImGui.SameLine();

            float targetValue = (float)this.targetValue;
            if (ImGui.InputFloat("##targetValue", ref targetValue))
            {
                this.targetValue = targetValue;
            }
        }
    }

    public class BoolCondition : AnimatorCondition
    {
        public BoolCondition(BoolParameter parameter, bool targetValue) : base(parameter, ComparisonType.Equals, targetValue) { }

        internal override bool CheckCondition()
        {
            return parameter.value.Equals(targetValue);
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.Text(parameter.name);
            ImGui.SameLine();
            bool targetValue = (bool)this.targetValue;
            if(ImGui.Checkbox("##targetValue", ref targetValue))
            {
                this.targetValue = targetValue;
            }
        }
    }

    public class TriggerCondition : AnimatorCondition
    {
        public TriggerCondition(TriggerParameter parameter) : base(parameter, ComparisonType.Equals, true) { }

        internal override bool CheckCondition()
        {
            return parameter.value.Equals(targetValue);
        }

        internal override void OnAnimatorGUI()
        {
            ImGui.Text(parameter.name);
        }
    }


}
