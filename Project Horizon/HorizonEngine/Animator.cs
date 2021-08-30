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
    public class Animator : Component
    {
        private string _animatorControllerAssetID;
        [JsonIgnore]
        AnimatorController _animatorController;
        [JsonIgnore]
        private Dictionary<string, AnimatorParameter> _parameters;
        [JsonIgnore]
        private Animation _currentAnimation;
        [JsonIgnore]
        private Animation _nextAnimation;
        private int _currentFrame;
        private float _frameDuration;
        private float _totalDuration;
        private float _currentDuration;
        private float _transitionDuration;

        public Animator()
        {
            _currentAnimation = _nextAnimation = null;
            _parameters = new Dictionary<string, AnimatorParameter>();
            _animatorControllerAssetID = null;
        }

        public AnimatorController animatorController
        {
            get
            {
                return _animatorController;
            }
            set
            {
                _animatorController = value;
                _animatorControllerAssetID = value == null ? null : _animatorController.name;
                if (value != null)
                {
                    _parameters = new Dictionary<string, AnimatorParameter>();
                    foreach (string key in _animatorController.parameters.Keys)
                    {
                        AnimatorParameter parameter = _animatorController.parameters[key];
                        if (parameter is IntParameter)
                        {
                            _parameters.Add(key, new IntParameter(parameter.name, (int)parameter.value));
                        }
                        else if (parameter is BoolParameter)
                        {
                            _parameters.Add(key, new BoolParameter(parameter.name, (bool)parameter.value));
                        }
                        else if (parameter is TriggerParameter)
                        {
                            _parameters.Add(key, new TriggerParameter(parameter.name));
                        }
                        else if (parameter is FloatParameter)
                        {
                            _parameters.Add(key, new FloatParameter(parameter.name, (float)parameter.value));
                        }
                    }

                    this.Play(value.defaultAnimation);
                }
            }
        }

        internal override Component Clone()
        {
            Animator clone = (Animator)base.Clone();
            clone.animatorController = clone.animatorController;
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
            if (_currentAnimation == null) return;

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
                foreach (var x in animatorController.transitions[_currentAnimation.name])
                {
                    float item3 = previousExitTime < 0f ? x.Item3 % 1f : x.Item3;
                    if ((!x.Item2 || (previousExitTime <= item3 && item3 <= currentExitTime)) && CheckCondition(x.Item5))
                    {
                        //SetCurrentAnimation(x.Item1);
                        _nextAnimation = animatorController.animations[x.Item1];
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

        public void Play(string name)
        {
            _currentAnimation = animatorController.animations[name];
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

        public override void OnLoad()
        {
            this.animatorController = Assets.GetAnimatorController(_animatorControllerAssetID);
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Animator")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                this.enabled = enabled;
            }

            string controller = _animatorController == null ? "None" : _animatorController.name;
            ImGui.Text("Controller");
            ImGui.SameLine();
            ImGui.Text(controller);
        }
    }
}
