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
    public static class Audio
    {
        private static AudioListener _audioListener;

        static Audio()
        {
            _audioListener = new AudioListener();
        }

        public static AudioListener audioListener
        {
            get
            {
                return _audioListener;
            }
        }

        public static float masterVolume
        {
            get
            {
                return SoundEffect.MasterVolume;
            }
            set
            {
                SoundEffect.MasterVolume = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        public static float dopplerScale
        {
            get
            {
                return SoundEffect.DopplerScale;
            }
            set
            {
                SoundEffect.DopplerScale = Math.Max(value, 0f);
            }
        }

        public static float speedOfSound
        {
            get
            {
                return SoundEffect.SpeedOfSound;
            }
            set
            {
                SoundEffect.SpeedOfSound = Math.Max(value, 0f);
            }
        }

        public static float distanceScale
        {
            get
            {
                return SoundEffect.DistanceScale;
            }
            set
            {
                SoundEffect.DistanceScale = Math.Max(value, 0f);
            }
        }
    }
}
