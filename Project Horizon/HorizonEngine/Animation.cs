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
using ImGuiNET;

namespace HorizonEngine
{
    public class Animation : Asset
    {
        private List<HorizonEngine.Texture> _frames;
        private float _duration;
        private bool _loop;

        internal Animation(string name, float duration, bool loop) : base(name, null)
        {
            _duration = duration;
            _loop = loop;
            _frames = new List<HorizonEngine.Texture>();
        }

        internal HorizonEngine.Texture this[int index]
        {
            get
            {
                return _frames[index];
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
                _duration = value >= 0 ? value : 0f;
            }
        }

        internal float frameDuration
        {
            get
            {
                return _frames.Count > 0 ? _duration / _frames.Count : _duration;
            }
        }

        internal int length
        {
            get
            {
                return _frames.Count;
            }
        }

        internal bool loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
            }
        }

        internal void AddFrame(HorizonEngine.Texture frame)
        {
            _frames.Add(frame);
        }

        internal void RemoveFrame(HorizonEngine.Texture frame)
        {
            _frames.Remove(frame);
        }

        internal void SwapFrame(int index1, int index2)
        {
            var x = _frames[index1];
            _frames[index1] = _frames[index2];
            _frames[index2] = x;
        }      

        internal override void Reload()
        {
            Assets.Load(this);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (ImGui.Button("Open in Animation Window"))
            {
                AnimationWindow.Open(this);
            }
        }
    }
}
