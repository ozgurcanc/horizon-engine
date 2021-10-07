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
        private uint _assetID;
        [JsonIgnore]
        AnimatorController _animatorController;
        [JsonIgnore]
        private Dictionary<string, List<AnimatorParameter>> _parameters;
        [JsonIgnore]
        private Dictionary<AnimatorParameter, AnimatorParameter> _parameterMap;
        [JsonIgnore]
        private Animation _currentAnimation;
        [JsonIgnore]
        private Animation _nextAnimation;
        private int _currentFrame;
        private float _totalDuration;
        private float _currentDuration;
        private float _transitionDuration;

        public Animator()
        {
            _currentAnimation = _nextAnimation = null;
            _parameters = new Dictionary<string, List<AnimatorParameter>>();
            _assetID = 0;
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
                _assetID = value == null ? 0 : _animatorController.assetID;
                if (value != null)
                {
                    _parameters = new Dictionary<string, List<AnimatorParameter>>();
                    _parameterMap = new Dictionary<AnimatorParameter, AnimatorParameter>();
                    foreach (AnimatorParameter parameter in _animatorController.parameters)
                    {
                        if (!_parameters.ContainsKey(parameter.name)) 
                            _parameters.Add(parameter.name, new List<AnimatorParameter>());

                        AnimatorParameter parameterCopy = parameter.Copy();
                        _parameters[parameter.name].Add(parameterCopy);
                        _parameterMap.Add(parameter, parameterCopy);
                    }

                    Animation anim = value.defaultAnimation;
                    if (anim == null) this.Play(null);
                    else this.Play(value.defaultAnimation.name);
                }
                else
                {
                    this.Play(null);
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
            _parameters[name][0].value = value;
        }

        public void SetTrigger(string name)
        {
            _parameters[name][0].value = true;
        }

        public void SetInteger(string name, int value)
        {
            _parameters[name][0].value = value;
        }

        public void SetFloat(string name, float value)
        {
            _parameters[name][0].value = value;
        }

        internal void AnimationUpdate(float deltaTime)
        {
            if (_currentAnimation == null) return;

            _totalDuration += deltaTime;
            _currentDuration += deltaTime;

            float currentExitTime = (_totalDuration / _currentAnimation.duration) % 1f;
            float previousExitTime = currentExitTime - deltaTime / _currentAnimation.duration;

            if (_currentDuration >= _currentAnimation.frameDuration)
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
                foreach (AnimatorTransition x in animatorController.GetTransitions(_currentAnimation.assetID))
                {
                    float exitTime = previousExitTime < 0f ? x.exitTime % 1f : x.exitTime;
                    if ((!x.hasExitTime || (previousExitTime <= exitTime && exitTime <= currentExitTime)) && CheckCondition(x.conditions))
                    {
                        //SetCurrentAnimation(x.Item1);
                        _nextAnimation = x.to;
                        _transitionDuration = x.duration;
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
            if(name == null)
            {
                _currentAnimation = null;
                _nextAnimation = null;
                return;
            }
            _currentAnimation = animatorController.GetAnimation(name);
            _currentFrame = 0;
            _currentDuration = 0f;
            _totalDuration = 0f;
            //var sprite = gameObject.GetComponent<Sprite>();
            //if (sprite != null) sprite.texture = _currentAnimation[0];
            _nextAnimation = null;
        }
        private bool CheckCondition(IList<AnimatorCondition> conditions)
        {
            bool result = true;
            List<AnimatorParameter> triggers = new List<AnimatorParameter>();

            foreach(AnimatorCondition condition in conditions)
            {
                ComparisonType comparison = condition.comparison;
                AnimatorParameter parameter = _parameterMap[condition.parameter];

                if (comparison == ComparisonType.Equals)
                {
                    result &= parameter.value.Equals(condition.targetValue);
                }
                else if (comparison == ComparisonType.NotEquals)
                {
                    result &= !parameter.value.Equals(condition.targetValue);
                }
                else if (comparison == ComparisonType.Greater)
                {
                    if (parameter is IntParameter) result &= (int)parameter.value > (int)condition.targetValue;
                    else result &= (float)parameter.value > (float)condition.targetValue;
                }
                else if (comparison == ComparisonType.Less)
                {
                    if (parameter is IntParameter) result &= (int)parameter.value < (int)condition.targetValue;
                    else result &= (float)parameter.value < (float)condition.targetValue;
                }

                if (parameter is TriggerParameter) triggers.Add(parameter);
            }

            if(result)
            {
                foreach (var x in triggers) x.value = false;
            }

            return result;
        }

        public override void OnLoad()
        {
            this.animatorController = Assets.GetAnimatorController(_assetID);
        }

        internal override void EnableComponent()
        {
            Scene.EnableComponent(this);
        }

        internal override void DisableComponent()
        {
            Scene.DisableComponent(this);
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
                Undo.RegisterAction(this, this.enabled, enabled, nameof(Animator.enabled));
                this.enabled = enabled;
            }

            string controller = _animatorController == null ? "None" : _animatorController.name;
            ImGui.Text("Controller");
            ImGui.SameLine();
            ImGui.Text(controller);
            ImGui.SameLine();
            if (ImGui.Button("select controller"))
            {
                ImGui.OpenPopup("select_controller");
                SearchBar.Clear();
            }

            if (ImGui.BeginPopup("select_controller"))
            {
                SearchBar.Draw();

                if (ImGui.Selectable("None"))
                {
                    Undo.RegisterAction(this, this.animatorController, null, nameof(Animator.animatorController));
                    this.animatorController = null;
                }

                foreach (AnimatorController animatorController in Assets.animatorControllers)
                {
                    if (SearchBar.PassFilter(animatorController.name) && ImGui.Selectable(animatorController.name))
                    {
                        Undo.RegisterAction(this, this.animatorController, animatorController, nameof(Animator.animatorController));
                        this.animatorController = animatorController;
                    }
                }

                ImGui.EndPopup();
            }
        }
    }
}
