using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Newtonsoft.Json;
using System.IO;

namespace HorizonEngine
{
    public class AudioClip : Asset
    {
        [JsonIgnore]
        private SoundEffect _audio;

        internal AudioClip(string name, string source) : base(name, source)
        {
            _audio = SoundEffect.FromFile(Path.Combine(Application.assetsPath, source));
        }

        internal SoundEffect audio
        {
            get
            {
                return _audio;
            }
        }

        internal override void Delete()
        {
            Assets.Delete(this);
            File.Delete(Path.Combine(Application.assetsPath, this.source));
        }

        internal override void Reload()
        {
            _audio = SoundEffect.FromFile(Path.Combine(Application.assetsPath, this.source));
            Assets.Load(this);
        }
    }
}
