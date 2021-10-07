using System;
using HorizonEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Project_Horizon_DX
{
    class TestScene
    {

        class Test : Behaviour
        {
            public override void Update()
            {
                if(Input.GetKeyDown(Keys.Q))
                {
                    gameObject.GetComponent<Animator>().SetTrigger("Trigger");
                }
                if (Input.GetKeyDown(Keys.R))
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
                if (Input.GetKeyDown(Keys.A))
                {
                    gameObject.GetComponent<AudioSource>().Play();
                }
                if (Input.GetKeyDown(Keys.S))
                {
                    gameObject.GetComponent<AudioSource>().Pause();
                }
                if (Input.GetKeyDown(Keys.D))
                {
                    gameObject.GetComponent<AudioSource>().Resume();
                }
                if (Input.GetKeyDown(Keys.F))
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

            //using (var game = new Scene())
            //  game.Run();

            var game = new Engine();
            game.Run();
        }
    }
}

