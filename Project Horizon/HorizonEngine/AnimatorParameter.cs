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

namespace HorizonEngine
{
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
    }


    public class IntParameter : AnimatorParameter
    {
        public IntParameter(string name, int value) : base(name, value) { }
    }

    public class FloatParameter : AnimatorParameter
    {
        public FloatParameter(string name, float value) : base(name, value) { }
    }

    public class BoolParameter : AnimatorParameter
    {
        public BoolParameter(string name, bool value) : base(name, value) { }
    }

    public class TriggerParameter : AnimatorParameter
    {
        public TriggerParameter(string name) : base(name, false) { }
    }
}
