using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorizonEngine
{
    public enum ComparisonType
    {
        Greater,
        Less,
        Equals,
        NotEquals
    }

    public abstract class AnimatorCondition
    {
        private ComparisonType _comparison;
        private object _targetValue;
        private string _parameter;

        protected AnimatorCondition(string parameter, ComparisonType comparison, object targetValue)
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
        }

        internal string parameter
        {
            get
            {
                return _parameter;
            }
        }

        internal object targetValue
        {
            get
            {
                return _targetValue;
            }
        }


    }

    public class IntCondition : AnimatorCondition
    {
        public IntCondition(string parameter, int targetValue, ComparisonType comparison) : base(parameter, comparison, targetValue) { }
    }

    public class FloatCondition : AnimatorCondition
    {
        public FloatCondition(string parameter, float targetValue, ComparisonType comparison) : base(parameter, comparison, targetValue) { }
    }

    public class BoolCondition : AnimatorCondition
    {
        public BoolCondition(string parameter, bool targetValue) : base(parameter, ComparisonType.Equals, targetValue) { } 
    }

    public class TriggerCondition : AnimatorCondition
    {
        public TriggerCondition(string parameter) : base(parameter, ComparisonType.Equals, true) { }
    }


}
