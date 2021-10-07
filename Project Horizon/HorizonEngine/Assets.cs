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
using System.IO;
using Newtonsoft.Json;

namespace HorizonEngine
{
    public static class Assets
    {
        [JsonObject(MemberSerialization.Fields)]
        private class AssetsSaveData
        {
            public uint nextAssetID;
            public AssetsDirectory rootDirectory;
        }

        private static bool _isModified;
        private static uint _nextAssetID;
        private static AssetsDirectory _rootDirectory;
        private static Dictionary<string, Texture2D> _sourceTextures;
        private static Dictionary<uint, HorizonEngine.Texture> _textures;
        private static Dictionary<uint, Animation> _animations;
        private static Dictionary<uint, Font> _fonts;
        private static Dictionary<uint, RenderTexture> _renderTextures;
        private static Dictionary<uint, AnimatorController> _animatorControllers;
        private static Dictionary<uint, AudioClip> _audioClips;

        static Assets()
        {
            _sourceTextures = new Dictionary<string, Texture2D>();
            _textures = new Dictionary<uint, Texture>();
            _animations = new Dictionary<uint, Animation>();
            _fonts = new Dictionary<uint, Font>();
            _renderTextures = new Dictionary<uint, RenderTexture>();
            _animatorControllers = new Dictionary<uint, AnimatorController>();
            _audioClips = new Dictionary<uint, AudioClip>();
            _rootDirectory = new AssetsDirectory("Assets");
        }

        internal static uint availableAssetID
        {
            get
            {
                _nextAssetID++;
                return _nextAssetID;
            }
        }

        internal static void Save()
        {
            AssetsSaveData assetsSaveData = new AssetsSaveData();
            assetsSaveData.nextAssetID = _nextAssetID;
            assetsSaveData.rootDirectory = _rootDirectory;
            _isModified = false;
            File.WriteAllText(Path.Combine(Application.projectPath, "Assets.json"), JsonConvert.SerializeObject(assetsSaveData));
        }

        internal static void Load()
        {
            AssetsSaveData assetsSaveData = JsonConvert.DeserializeObject<AssetsSaveData>(File.ReadAllText(Path.Combine(Application.projectPath, "Assets.json")));
            _nextAssetID = assetsSaveData.nextAssetID;
            _rootDirectory = assetsSaveData.rootDirectory;
            _rootDirectory.Reload();
        }

        internal static AssetsDirectory rootDirectory
        {
            get
            {
                return _rootDirectory;
            }
        }

        internal static bool isModified
        {
            get
            {
                return _isModified;
            }
        }

        internal static void SetModified()
        {
            _isModified = true;
        }

        internal static Dictionary<uint, Font>.ValueCollection fonts
        {
            get
            {
                return _fonts.Values;
            }
        }

        internal static Font CreateFont(string name, string source)
        {
            Font font = new Font(name, source);
            _fonts.Add(font.assetID, font);
            return font;
        }

        internal static Font GetFont(uint id) 
        {
            if (_fonts.ContainsKey(id))
                return _fonts[id];
           
            return null;  
        }

        internal static void Load(Font font)
        {
            _fonts.Add(font.assetID, font);
        }


        internal static Dictionary<uint, HorizonEngine.Texture>.ValueCollection textures
        {
            get
            {
                return _textures.Values;
            }
        }

        internal static HorizonEngine.Texture CreateTexture(string name, string source, Vector4? sourceRectangle = null)
        {
            HorizonEngine.Texture texture = new HorizonEngine.Texture(name, source, sourceRectangle);
            _textures.Add(texture.assetID, texture);
            return texture;
        }

        internal static HorizonEngine.Texture GetTexture(uint id)
        {
            if (_textures.ContainsKey(id))
                return _textures[id];

            return null;
        }

        internal static void Load(HorizonEngine.Texture texture)
        {
            _textures.Add(texture.assetID, texture);
        }

        internal static Texture2D GetSourceTexture(string source)
        {
            if (_sourceTextures.ContainsKey(source))
                return _sourceTextures[source];

            Texture2D texture2D = Graphics.CreateTexture2D(Path.Combine(Application.assetsPath, source));
            _sourceTextures.Add(source, texture2D);
            return texture2D;
        }

        internal static Dictionary<uint, RenderTexture>.ValueCollection renderTextures
        {
            get
            {
                return _renderTextures.Values;
            }
        }

        internal static RenderTexture CreateRenderTexture(string name, int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture(name, width, height);
            _renderTextures.Add(renderTexture.assetID, renderTexture);
            _textures.Add(renderTexture.assetID, renderTexture);
            return renderTexture;
        }

        internal static RenderTexture GetRenderTexture(uint id)
        {
            if (_renderTextures.ContainsKey(id))
                return _renderTextures[id];

            return null;
        }

        internal static void Load(RenderTexture renderTexture)
        {
            _textures.Add(renderTexture.assetID, renderTexture);
            _renderTextures.Add(renderTexture.assetID, renderTexture);
        }

        internal static Dictionary<uint, Animation>.ValueCollection animations
        {
            get
            {
                return _animations.Values;
            }
        }

        internal static Animation CreateAnimation(string name, float duration = 1f, bool loop = true)
        {
            Animation animation = new Animation(name, duration, loop);
            _animations.Add(animation.assetID, animation);
            return animation;
        }

        internal static Animation GetAnimation(uint id)
        {
            if (_animations.ContainsKey(id))
                return _animations[id];

            return null;
        }

        internal static void Load(Animation animation)
        {
            _animations.Add(animation.assetID, animation);
        }

        internal static Dictionary<uint, AnimatorController>.ValueCollection animatorControllers
        {
            get
            {
                return _animatorControllers.Values;
            }
        }

        internal static AnimatorController CreateAnimatorController(string name)
        {
            AnimatorController animatorController = new AnimatorController(name);
            _animatorControllers.Add(animatorController.assetID, animatorController);
            return animatorController;
        }

        internal static AnimatorController GetAnimatorController(uint id)
        {
            if (_animatorControllers.ContainsKey(id))
                return _animatorControllers[id];

            return null;
        }

        internal static void Load(AnimatorController animatorController)
        {
            _animatorControllers.Add(animatorController.assetID, animatorController);
        }

        internal static Dictionary<uint, AudioClip>.ValueCollection audioClips
        {
            get
            {
                return _audioClips.Values;
            }
        }

        internal static AudioClip CreateAudioClip(string name, string source)
        {
            AudioClip audioClip = new AudioClip(name, source);
            _audioClips.Add(audioClip.assetID, audioClip);
            return audioClip;
        }

        internal static AudioClip GetAudioClip(uint id)
        {
            if (_audioClips.ContainsKey(id))
                return _audioClips[id];

            return null;
        }

        internal static void Load(AudioClip audioClip)
        {
            _audioClips.Add(audioClip.assetID, audioClip);
        }

        internal static void Delete(AudioClip audioClip)
        {
            _audioClips.Remove(audioClip.assetID);
        }

        internal static void Delete(Font font)
        {
            _fonts.Remove(font.assetID);
        }

        internal static void Delete(AnimatorController animatorController)
        {
            _animatorControllers.Remove(animatorController.assetID);
        }

        internal static void Delete(Animation animation)
        {
            _animations.Remove(animation.assetID);
            foreach (var x in _animatorControllers.Values) x.RemoveAnimation(animation);
        }

        internal static void Delete(HorizonEngine.Texture texture)
        {
            _textures.Remove(texture.assetID);
            foreach (var x in _animations.Values) x.RemoveFrame(texture);
        }

        internal static void Delete(RenderTexture renderTexture)
        {
            _renderTextures.Remove(renderTexture.assetID);
            _textures.Remove(renderTexture.assetID);
            foreach (var x in _animations.Values) x.RemoveFrame(renderTexture);
        }    
    }
}
