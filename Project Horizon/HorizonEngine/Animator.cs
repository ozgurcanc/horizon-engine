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
    public class Animator : Component
    {
        private Dictionary<string, AnimatorParameter> _parameters;
        private Dictionary<string, Animation> _animations;
        private Dictionary<string, List<Tuple<string, bool, float, float, AnimatorCondition[]>>> _transitions;
        private Animation _currentAnimation;
        private Animation _nextAnimation;
        private int _currentFrame;
        private float _frameDuration;
        private float _totalDuration;
        private float _currentDuration;
        private float _transitionDuration;

        public Animator()
        {
            _parameters = new Dictionary<string, AnimatorParameter>();
            _animations = new Dictionary<string, Animation>();
            _transitions = new Dictionary<string, List<Tuple<string, bool, float, float, AnimatorCondition[]>>>();
            _currentAnimation = _nextAnimation = null;
        }

        internal override Component Clone()
        {
            Animator clone = (Animator)base.Clone();
            Dictionary<string, AnimatorParameter> parameters = clone._parameters;
            clone._parameters = new Dictionary<string, AnimatorParameter>();
          
            foreach(string key in parameters.Keys)
            {
                AnimatorParameter parameter = parameters[key];
                if(parameter is IntParameter)
                {
                    clone._parameters.Add(key, new IntParameter(parameter.name, (int)parameter.value));
                }
                else if (parameter is BoolParameter)
                {
                    clone._parameters.Add(key, new BoolParameter(parameter.name, (bool)parameter.value));
                }
                else if (parameter is TriggerParameter)
                {
                    clone._parameters.Add(key, new TriggerParameter(parameter.name));
                }
                else if (parameter is FloatParameter)
                {
                    clone._parameters.Add(key, new FloatParameter(parameter.name, (float)parameter.value));
                }
            }           

            return clone;
        }

        public void SetBool(string name, bool value)
        {
            _parameters[name].value = value;
        }

        public void SetTrigger(string name)
        {
            _parameters[name].value = true;
        }

        public void SetInteger(string name, int value)
        {
            _parameters[name].value = value;
        }

        public void SetFloat(string name, float value)
        {
            _parameters[name].value = value;
        }

        internal void AnimationUpdate(float deltaTime)
        {
            _totalDuration += deltaTime;
            _currentDuration += deltaTime;

            float currentExitTime = (_totalDuration / _currentAnimation.duration) % 1f;
            float previousExitTime = currentExitTime - deltaTime / _currentAnimation.duration;

            if (_currentDuration >= _frameDuration)
            {
                int length = _currentAnimation.length;
                _currentFrame++;
                _currentDuration = 0f;

                if (_currentFrame >= length)
                {
                    if (_currentAnimation.loop) _currentFrame = 0;
                    else _currentFrame = length-1;
                }
                gameObject.GetComponent<Sprite>().texture = _currentAnimation[_currentFrame];
            }

            if (_nextAnimation == null)
            {
                foreach (var x in _transitions[_currentAnimation.name])
                {
                    float item3 = previousExitTime < 0f ? x.Item3 % 1f : x.Item3;
                    if ((!x.Item2 || (previousExitTime <= item3 && item3 <= currentExitTime)) && CheckCondition(x.Item5))
                    {
                        //SetCurrentAnimation(x.Item1);
                        _nextAnimation = _animations[x.Item1];
                        _transitionDuration = x.Item4;
                        return;
                    }
                }
            }
            else
            {
                _transitionDuration -= deltaTime;
                if(_transitionDuration <= 0.0f)
                {
                    Play(_nextAnimation.name);
                }
            }
                                
        }

        public void SetAnimations(params string[] animations)
        {
            foreach(string animation in animations)
            {
                _animations.Add(animation, Assets.GetAnimation(animation));
                _transitions.Add(animation, new List<Tuple<string, bool, float, float, AnimatorCondition[]>>());
            }

            Play(animations[0]);
        }

        public void SetParameters(params AnimatorParameter[] parameters)
        {
            foreach(AnimatorParameter parameter in parameters)
            {
                _parameters.Add(parameter.name, parameter);
            }
        }

        public void SetTransition(string from, string to, bool hasExitTime, float exitTime,float duration, params AnimatorCondition[] conditions)
        {
            _transitions[from].Add(Tuple.Create(to, hasExitTime, MathHelper.Clamp(exitTime, 0f, 1f), duration, conditions));
        }

        public void Play(string name)
        {
            _currentAnimation = _animations[name];
            _currentFrame = 0;
            _currentDuration = 0f;
            _totalDuration = 0f;
            _frameDuration = _currentAnimation.duration / _currentAnimation.length;
            gameObject.GetComponent<Sprite>().texture = _currentAnimation[0];
            _nextAnimation = null;
        }
        private bool CheckCondition(AnimatorCondition[] conditions)
        {
            if (conditions == null)
                return true;

            bool result = true;
            List<AnimatorParameter> triggerParameters = new List<AnimatorParameter>();

            foreach(AnimatorCondition condition in conditions)
            {
                AnimatorParameter parameter = _parameters[condition.parameter];
                ComparisonType comparison = condition.comparison;

                if (parameter is TriggerParameter) triggerParameters.Add(parameter);

                if(comparison == ComparisonType.Equals)
                {
                    result &= parameter.value.Equals(condition.targetValue);
                }
                else if(comparison == ComparisonType.NotEquals)
                {
                    result &= !parameter.value.Equals(condition.targetValue);
                }
                else if(comparison == ComparisonType.Greater)
                {
                    if (parameter is IntParameter) result &= (int)parameter.value > (int)condition.targetValue;
                    else result &= (float)parameter.value > (float)condition.targetValue;
                }
                else if(comparison == ComparisonType.Less)
                {
                    if (parameter is IntParameter) result &= (int)parameter.value < (int)condition.targetValue;
                    else result &= (float)parameter.value < (float)condition.targetValue;
                }
            }

            if(result)
            {
                foreach (var x in triggerParameters) x.value = false;
            }

            return result;
        }
    }
}
