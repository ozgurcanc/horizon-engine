using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Newtonsoft.Json;
using ImGuiNET;

namespace HorizonEngine
{
    public class AudioSource : Component
    {
        [JsonIgnore]
        private AudioClip _clip;
        [JsonIgnore]
        private SoundEffectInstance _clipInstance;
        private uint _assetID;
        private bool _loop;
        private bool _mute;
        private float _volume;
        private float _pitch;
        private float _pan;     

        public AudioSource()
        {
            _volume = 1f;
            _clip = null;
        }

        public AudioClip clip
        {
            get
            {
                return _clip;
            }
            set
            {
                Stop();
                _clip = value;
                if(value == null)
                {
                    _clipInstance = null;
                    _assetID = 0;
                }
                else
                {
                    _clipInstance = _clip.audio.CreateInstance();
                    _assetID = value.assetID;
                    _clipInstance.IsLooped = _loop;
                    _clipInstance.Volume = _mute ? 0f : _volume;
                    _clipInstance.Pitch = _pitch;
                    _clipInstance.Pan = _pan;
                }
            }
        }

        public bool loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
                if (_clipInstance != null) _clipInstance.IsLooped = _loop;
            }
        }

        public bool mute
        {
            get
            {
                return _mute;
            }
            set
            {
                _mute = value;
                if (_clipInstance != null) _clipInstance.Volume = _mute ? 0f : _volume;
            }
        }

        public float volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = MathHelper.Clamp(value, 0f, 1f);
                if (_clipInstance != null) _clipInstance.Volume = _mute ? 0f : _volume;
            }
        }

        public float pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                _pitch = MathHelper.Clamp(value, -1f, 1f);
                if (_clipInstance != null) _clipInstance.Pitch = _pitch;
            }
        }

        public float pan
        {
            get
            {
                return _pan;
            }
            set
            {
                _pan = MathHelper.Clamp(value, -1f, 1f);
                if (_clipInstance != null) _clipInstance.Pan = _pan;
            }
        }

        internal void UpdateAudio()
        {
            if (_clipInstance == null) return;
        }

        public void Play()
        {
            if (_clipInstance == null || !enabled) return;
            _clipInstance.Play();
        }

        public void Pause()
        {
            if (_clipInstance == null) return;
            _clipInstance.Pause();
        }

        public void Resume()
        {
            if (_clipInstance == null) return;
            _clipInstance.Resume();
        }

        public void Stop()
        {
            if (_clipInstance == null) return;
            _clipInstance.Stop();
        }

        public override void OnLoad()
        {
            this.clip = Assets.GetAudioClip(_assetID);
        }

        public override void OnInspectorGUI()
        {
            if (!ImGui.CollapsingHeader("Audio Source")) return;

            string id = this.GetType().Name + componetID.ToString();

            bool enabled = this.enabled;
            ImGui.Text("Enabled");
            ImGui.SameLine();
            if (ImGui.Checkbox("##enabled" + id, ref enabled))
            {
                Undo.RegisterAction(this, this.enabled, enabled, nameof(AudioSource.enabled));
                this.enabled = enabled;
            }

            //ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);

            bool mute = this.mute;
            ImGui.Text("Mute");
            ImGui.SameLine();
            if (ImGui.Checkbox("##mute" + id, ref mute))
            {
                Undo.RegisterAction(this, this.mute, mute, nameof(AudioSource.mute));
                this.mute = mute;
            }

            bool loop = this.loop;
            ImGui.Text("Loop");
            ImGui.SameLine();
            if(ImGui.Checkbox("##loop" + id, ref loop))
            {
                Undo.RegisterAction(this, this.loop, loop, nameof(AudioSource.loop));
                this.loop = loop;
            }

            float volume = this.volume;
            ImGui.Text("Volume");
            ImGui.SameLine();
            if(ImGui.SliderFloat("##volume" + id, ref volume, 0f, 1f))
            {
                Undo.RegisterAction(this, this.volume, volume, nameof(AudioSource.volume));
                this.volume = volume;
            }

            float pitch = this.pitch;
            ImGui.Text("Pitch");
            ImGui.SameLine();
            if (ImGui.SliderFloat("##pitch" + id, ref pitch, -1f, 1f))
            {
                Undo.RegisterAction(this, this.pitch, pitch, nameof(AudioSource.pitch));
                this.pitch = pitch;
            }

            float pan = this.pan;
            ImGui.Text("Pan");
            ImGui.SameLine();
            if (ImGui.SliderFloat("##pan" + id, ref pan, -1f, 1f))
            {
                Undo.RegisterAction(this, this.pan, pan, nameof(AudioSource.pan));
                this.pan = pan;
            }

            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.25f);

            string clipName = _clip == null ? "None" : _clip.name;
            ImGui.Text("Font");
            ImGui.SameLine();
            ImGui.Text(clipName);
            ImGui.SameLine();
            if (ImGui.Button("Select AudioClip"))
            {
                ImGui.OpenPopup("select_audio_clip");
            }

            if (ImGui.BeginPopup("select_audio_clip"))
            {
                if (ImGui.Selectable("None"))
                {
                    Undo.RegisterAction(this, this.clip, null, nameof(AudioSource.clip));
                    this.clip = null;
                }

                foreach (AudioClip clip in Assets.audioClips)
                {
                    if (ImGui.Selectable(clip.name))
                    {
                        Undo.RegisterAction(this, this.clip, clip, nameof(AudioSource.clip));
                        this.clip = clip;
                    }
                }

                ImGui.EndPopup();
            }
            
            ImGui.PopItemWidth();
        }
    }
}
