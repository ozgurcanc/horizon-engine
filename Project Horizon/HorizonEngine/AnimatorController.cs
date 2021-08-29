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

namespace HorizonEngine
{
    public class AnimatorController
    {
        private string _name;
        private Dictionary<string, AnimatorParameter> _parameters;
        private Dictionary<string, Animation> _animations;
        private Dictionary<string, List<Tuple<string, bool, float, float, AnimatorCondition[]>>> _transitions;
        private string _defaultAnimation;

        internal AnimatorController(string name)
        {
            _name = name;
            _parameters = new Dictionary<string, AnimatorParameter>();
            _animations = new Dictionary<string, Animation>();
            _transitions = new Dictionary<string, List<Tuple<string, bool, float, float, AnimatorCondition[]>>>();
        }

        internal string name
        {
            get
            {
                return _name;
            }
        }

        internal Dictionary<string, AnimatorParameter> parameters
        {
            get
            {
                return _parameters;
            }
        }

        internal Dictionary<string, Animation> animations
        {
            get
            {
                return _animations;
            }
        }

        internal Dictionary<string, List<Tuple<string, bool, float, float, AnimatorCondition[]>>> transitions
        {
            get
            {
                return _transitions;
            }
        }

        internal string defaultAnimation
        {
            get
            {
                return _defaultAnimation;
            }
        }

        public void SetAnimations(params string[] animations)
        {
            foreach (string animation in animations)
            {
                _animations.Add(animation, Assets.GetAnimation(animation));
                _transitions.Add(animation, new List<Tuple<string, bool, float, float, AnimatorCondition[]>>());
            }

            _defaultAnimation = animations[0];
        }

        public void SetParameters(params AnimatorParameter[] parameters)
        {
            foreach (AnimatorParameter parameter in parameters)
            {
                _parameters.Add(parameter.name, parameter);
            }
        }

        public void SetTransition(string from, string to, bool hasExitTime, float exitTime, float duration, params AnimatorCondition[] conditions)
        {
            _transitions[from].Add(Tuple.Create(to, hasExitTime, MathHelper.Clamp(exitTime, 0f, 1f), duration, conditions));
        }
    }
}
