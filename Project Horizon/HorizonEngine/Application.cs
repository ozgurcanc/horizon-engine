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
    public static class Application
    {
        private static bool _isEditor;
        private static string _projectPath;
        private static string _assetsPath;
        private static string _scenesPath;
        private static ProjectSettings _projectSettings;
        private static bool _isQuit;

        public static bool isEditor
        {
            get
            {
                return _isEditor;
            }
            internal set
            {
                _isEditor = value;
            }
        }

        internal static bool isQuit
        {
            get
            {
                return _isQuit;
            }
        }

        internal static string assetsPath
        {
            get
            {
                return _assetsPath;
            }
        }

        internal static string scenesPath
        {
            get
            {
                return _scenesPath;
            }
        }

        internal static string projectPath
        {
            get
            {
                return _projectPath;
            }
        }

        internal static uint availableAssetID
        {
            get
            {
                return _projectSettings.availableAssetID;
            }
        }

        internal static void Init()
        {
            _projectPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Project");
            _assetsPath = Path.Combine(_projectPath, "Assets");
            _scenesPath = Path.Combine(_projectPath, "Scenes");

            string projectSettingsPath = Path.Combine(_projectPath, "ProjectSettings.json");

            if (!Directory.Exists(_projectPath))
            {
                Directory.CreateDirectory(_projectPath);
                Directory.CreateDirectory(_assetsPath);
                Directory.CreateDirectory(_scenesPath);
                _projectSettings = new ProjectSettings();
                File.WriteAllText(projectSettingsPath, JsonConvert.SerializeObject(_projectSettings));
                Assets.Save();
            }
            
            _projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(projectSettingsPath));
            Assets.Load();            
        }

        public static void Quit()
        {
            _isQuit = true;
        }

    }
}
