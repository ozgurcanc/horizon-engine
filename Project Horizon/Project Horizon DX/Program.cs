using System;
using HorizonEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Project_Horizon_DX
{
    class TestScene
    {

        class Test : Behaviour
        {
            public override void Update()
            {
                if(Input.IsKeyDown(Keys.Q))
                {
                    gameObject.GetComponent<Animator>().SetTrigger("Trigger");
                }
                if (Input.IsKeyDown(Keys.R))
                {
                    gameObject.DontDestroyOnLoad();
                    Scene.Load("Test Scene");
                }

            }
        }

        class AudioTest : Behaviour
        {
            public override void Update()
            {
                if (Input.IsKeyDown(Keys.A))
                {
                    gameObject.GetComponent<AudioSource>().Play();
                }
                if (Input.IsKeyDown(Keys.S))
                {
                    gameObject.GetComponent<AudioSource>().Pause();
                }
                if (Input.IsKeyDown(Keys.D))
                {
                    gameObject.GetComponent<AudioSource>().Resume();
                }
                if (Input.IsKeyDown(Keys.F))
                {
                    gameObject.GetComponent<AudioSource>().Stop();
                }

            }
        }  
    }

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            //var game = new GameApp("Main Scene");
            //game.Run();

            var editor = new EditorApp();
            editor.Run();           
        }
    }
}

