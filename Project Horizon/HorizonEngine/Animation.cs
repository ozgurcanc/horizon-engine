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
                Assets.SetModified();
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
                Assets.SetModified();
            }
        }

        internal void AddFrame(HorizonEngine.Texture frame)
        {
            _frames.Add(frame);
            Assets.SetModified();
        }

        internal void RemoveFrame(HorizonEngine.Texture frame)
        {
            _frames.Remove(frame);
            Assets.SetModified();
        }

        internal void SwapFrame(int index1, int index2)
        {
            var x = _frames[index1];
            _frames[index1] = _frames[index2];
            _frames[index2] = x;
            Assets.SetModified();
        }      

        internal override void Reload()
        {
            Assets.Load(this);
        }

        internal override void Delete()
        {
            Assets.Delete(this);
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
