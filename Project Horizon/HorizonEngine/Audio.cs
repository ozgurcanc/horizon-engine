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
using System.IO;

namespace HorizonEngine
{
    public static class Audio
    {
        [JsonObject(MemberSerialization.Fields)]
        private class AudioSettings
        {
            public float masterVolume;
            public float dopplerScale;
            public float speedOfSound;
            public float distanceScale;
        }

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
                SoundEffect.DistanceScale = Math.Max(value, 0.001f);
            }
        }

        internal static void SaveSettings()
        {
            AudioSettings audioSettings = new AudioSettings();
            audioSettings.masterVolume = masterVolume;
            audioSettings.dopplerScale = dopplerScale;
            audioSettings.speedOfSound = speedOfSound;
            audioSettings.distanceScale = distanceScale;            
            File.WriteAllText(Path.Combine(Application.projectPath, "AudioSettings.json"), JsonConvert.SerializeObject(audioSettings));
        }

        internal static void LoadSettings()
        {
            AudioSettings audioSettings = JsonConvert.DeserializeObject<AudioSettings>(File.ReadAllText(Path.Combine(Application.projectPath, "AudioSettings.json")));
            masterVolume = audioSettings.masterVolume;
            dopplerScale = audioSettings.dopplerScale;
            speedOfSound = audioSettings.speedOfSound;
            distanceScale = audioSettings.distanceScale;
        }
    }
}
