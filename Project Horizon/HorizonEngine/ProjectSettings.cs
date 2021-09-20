using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HorizonEngine
{
    [JsonObject(MemberSerialization.Fields)]
    internal class ProjectSettings
    {
        private uint _assetID;
        private string _currentScene;

        public uint availableAssetID
        {
            get
            {
                _assetID++;
                return _assetID;
            }
        }

        public string currentScene
        {
            get
            {
                return _currentScene;
            }
            set
            {
                _currentScene = value;
            }
        }
    }
}
