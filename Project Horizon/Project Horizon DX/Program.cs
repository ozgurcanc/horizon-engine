using System;
using HorizonEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Project_Horizon_DX
{
    class TestScene : IScene
    {

        public void Load()
        {
            //Screen.resolution = new Vector2(1600, 900);
            //Camera.main.position = new Vector2(0, 0) ;
            Camera.main.width = 10;
            Camera.main.height = 10;


            GameObject g = Scene.CreateGameObject("1");
            for (int i = 0; i < 4; i++) Assets.LoadTexture("Idle" + i, "Idle", new Vector4(i * 0.25f, 0, 0.25f, 1));
            for (int i = 0; i < 8; i++) Assets.LoadTexture("Run" + i, "Run", new Vector4(i * (1f / 8f), 0, (1f / 8f), 1));

            Assets.LoadAnimation("Idle", 2f, true, "Idle0", "Idle1", "Idle2", "Idle3");
            Assets.LoadAnimation("Run", 0.5f, true, "Run0", "Run1", "Run2", "Run3", "Run4", "Run5", "Run6", "Run7");

            g.AddComponent<Sprite>().texture = Assets.GetTexture("Run0");
            g.size = new Vector2(5, 5);

            var animator = g.AddComponent<Animator>();
            animator.SetAnimations("Run", "Idle");
            animator.SetParameters(new BoolParameter("run", false), new TriggerParameter("trigger"));
            animator.SetTransition("Idle", "Run", false, 0f, 0f, new TriggerCondition("trigger"));
            animator.SetTransition("Run", "Idle", true, 1f, 0f, new TriggerCondition("trigger"));

            Assets.LoadSpriteFont("font", "File");

            g = Scene.CreateGameObject("2");
            g.position = new Vector2(0, 3);
            g.AddComponent<Text>().font = Assets.GetSpriteFont("font");
            g.GetComponent<Text>().text = "testing testing testing";


            g = Scene.CreateGameObject("test");

            Scene.CreateGameObject("testchild1").parent = g;
            Scene.CreateGameObject("testchild2").parent = g;
            Scene.CreateGameObject("testchild3").parent = g;

            g = Scene.CreateGameObject("parent");
            var k = Scene.CreateGameObject("child");
            k.parent = g;

            g = Scene.CreateGameObject("grandchild");
            g.parent = k;
        }
    }

    public static class Program
    {
        [STAThread]
        static void Main()
        {

            //using (var game = new Scene())
            //  game.Run();

            var game = new Engine();
            Scene.Load<TestScene>();
            game.Run();
        }
    }
}

