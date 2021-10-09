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
    public abstract class Application : Game
    {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;
        protected Scene _scene;

        private static bool _isEditor;
        private static string _projectPath;
        private static string _assetsPath;
        private static string _scenesPath;
        private static bool _isQuit;

        protected Application(bool isEditorApplication)
        {
            _isEditor = isEditorApplication; 
            // Paths Init
            _projectPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Project");
            _assetsPath = Path.Combine(_projectPath, "Assets");
            _scenesPath = Path.Combine(_projectPath, "Scenes");

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
            };

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;

            Graphics.Init(_graphics);          

            _scene = new Scene();        

            // App Init 

            if (!Directory.Exists(_projectPath))
            {
                Directory.CreateDirectory(_projectPath);
                Directory.CreateDirectory(_assetsPath);
                Directory.CreateDirectory(_scenesPath);
                Assets.Save();
            }

            Assets.Load();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public static bool isEditor
        {
            get
            {
                return _isEditor;
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

        internal static void LoadScene(string name)
        {
            GameWindow.isPlaying = false;
            Scene.Load(name);
            Undo.Reset();
        }

        internal static void NewScene(string name)
        {
            GameWindow.isPlaying = false;
            Scene.NewScene(name);
            Undo.Reset();
        }

        internal static void SaveScene()
        {
            GameWindow.isPlaying = false;
            Scene.Save();
            Assets.Save();
            Undo.SceneSaved();
        }

        internal static void RenameScene(string name)
        {
            GameWindow.isPlaying = false;
            Scene.Rename(name);
        }

        internal static void DeleteScene()
        {
            GameWindow.isPlaying = false;
            Scene.DeleteScene();
            Undo.Reset();
        }
        

        public static void Quit()
        {
            _isQuit = true;
        }

    }
}
