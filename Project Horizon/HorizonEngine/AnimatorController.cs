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

namespace HorizonEngine
{
    public class AnimatorController : Asset
    {
        private List<AnimatorParameter> _parameters;
        private Dictionary<uint, Animation> _animations;
        private Dictionary<uint, List<AnimatorTransition>> _transitions;
        private Animation _defaultAnimation;

        internal AnimatorController(string name) : base(name, null)
        {
            _parameters = new List<AnimatorParameter>();
            _animations = new Dictionary<uint, Animation>();
            _transitions = new Dictionary<uint, List<AnimatorTransition>>();
            _defaultAnimation = null;
        }
        
        internal Animation defaultAnimation
        {
            get
            {
                return _defaultAnimation;
            }
        }

        internal ReadOnlyCollection<AnimatorParameter> parameters
        {
            get
            {
                return _parameters.AsReadOnly();
            }
        }

        internal Dictionary<uint, Animation>.ValueCollection animations
        {
            get
            {
                return _animations.Values;
            }
        }

        internal Dictionary<uint, List<AnimatorTransition>>.ValueCollection transitions
        {
            get
            {
                return _transitions.Values;
            }
        }

        internal void SetDefaultAnimation(Animation animation)
        {
            _defaultAnimation = animation;
        }

        internal void AddAnimation(Animation animation)
        {
            _animations.Add(animation.assetID, animation);
            _transitions.Add(animation.assetID, new List<AnimatorTransition>());
        }

        internal void RemoveAnimation(Animation animation)
        {
            _animations.Remove(animation.assetID);
            if (_defaultAnimation == animation) _defaultAnimation = null;
            _transitions.Remove(animation.assetID);
        }

        internal void AddParameter(AnimatorParameter parameter)
        {
            _parameters.Add(parameter);
        }

        internal void RemoveParameter(AnimatorParameter parameter)
        {
            _parameters.Remove(parameter);
        }

        internal void AddTransition(AnimatorTransition transition)
        {
            _transitions[transition.from.assetID].Add(transition);
        }

        internal void RemoveTransition(AnimatorTransition transition)
        {
            _transitions[transition.from.assetID].Remove(transition);
        }

        internal List<AnimatorTransition> GetTransitions(uint animation)
        {
            return _transitions[animation];
        }

        internal Animation GetAnimation(string name)
        {
            foreach(Animation animation in _animations.Values)
            {
                if (animation.name == name) return animation;
            }

            return null;
        }

        internal override void Reload()
        {
            foreach (var parameter in parameters) parameter.Reload();
            Assets.Load(this);
        }

        /*
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

        internal Dictionary<string, AnimatorTransition> transitions
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
                // Fix here
                //_animations.Add(animation, Assets.GetAnimation(animation));
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
        */
    }
}
