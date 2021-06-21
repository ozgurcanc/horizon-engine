using System;
using HorizonEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Project_Horizon
{
    class Test : Behaviour
    {
        public GameObject[] _gameObjects = new GameObject[3];
        public override void Update(float deltaTime)
        {
            if (Input.GetKey(Keys.A))
            {
                _gameObjects[0].position -= new Vector2(2, 0) * deltaTime;
            }
            if (Input.GetKey(Keys.S))
            {
                _gameObjects[0].position -= new Vector2(0, 2) * deltaTime;
                //_gameObjects[1].position += new Vector2(2, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Input.GetKey(Keys.D))
            {
                _gameObjects[0].position += new Vector2(2, 0) * deltaTime;
                //_gameObjects[2].position += new Vector2(2, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Input.GetKey(Keys.W))
            {
                _gameObjects[0].position += new Vector2(0, 2) * deltaTime;
                //_gameObjects[2].parent = _gameObjects[0];
            }
            if (Input.GetKeyDown(Keys.E))
            {
                //_gameObjects[0].position = new Vector2(500, 500);
                //_gameObjects[0].size = new Vector2(300, 100);
                Scene.camera.width = 40;
                Scene.camera.height = 40;
            }
            if (Input.GetKey(Keys.R))
            {
                _gameObjects[0].size = new Vector2(20, 20);
                //_gameObjects[0].position = new Vector2(500, 0);
            }

            if(Input.GetKeyDown(Keys.Z))
            {
                _gameObjects[0].activeSelf = !_gameObjects[0].activeSelf;
            }
            if (Input.GetKeyDown(Keys.X))
            {
                _gameObjects[1].activeSelf = !_gameObjects[1].activeSelf;
            }
            if (Input.GetKeyDown(Keys.C))
            {
                _gameObjects[2].activeSelf = !_gameObjects[2].activeSelf;
            }
            if (Input.GetKeyDown(Keys.V))
            {
                _gameObjects[2].parent = null;
            }
            if (Input.GetKeyDown(Keys.B))
            {
                _gameObjects[2].parent = _gameObjects[0];
            }
            if (Input.GetKeyDown(Keys.Y))
            {
                //Debug.WriteLine(Mouse.GetState().Position.ToVector2());
                _gameObjects[0].position = Scene.camera.ScreenToWorldPoint(Mouse.GetState().Position.ToVector2());
                Debug.WriteLine(Mouse.GetState().Position.ToVector2());
                Debug.WriteLine(_gameObjects[0].position);
                Debug.WriteLine(Scene.camera.WorldToScreenPoint(_gameObjects[0].position));
            }
            if (Input.GetKey(Keys.K))
            {
                //Debug.WriteLine(Mouse.GetState().Position.ToVector2());
                Scene.camera.rotation += 20f * deltaTime;
            }

        }
    }

    class Benchmark : ISceneStarter
    {
        class BenchmarkScript : Behaviour
        {
            void Test(float deltaTime)
            {
                /*
                if (Input.GetKey(Keys.A))
                {
                    gameObject.position -= new Vector2(100, 0) * deltaTime;
                }
                if (Input.GetKey(Keys.D))
                {
                    gameObject.position += new Vector2(100, 0) * deltaTime;
                }
                if (Input.GetKey(Keys.W))
                {
                    gameObject.position += new Vector2(0, 100) * deltaTime;
                }
                if (Input.GetKey(Keys.S))
                {
                    gameObject.position -= new Vector2(0, 100) * deltaTime;
                }
                if(Input.GetKey(Keys.R))
                {
                    //this.enabled = false;
                    gameObject.rotation += 30f * deltaTime;
                }
                if (Input.GetKeyDown(Keys.T))
                {
                    gameObject.GetComponent<Sprite>().enabled = false;
                }
                if (Input.GetKeyDown(Keys.K))
                {
                    gameObject.GetComponent<Sprite>().enabled = true;
                }
                if (Input.GetKeyDown(Keys.X))
                {
                    gameObject.activeSelf = false;
                }
                if(Input.GetKeyDown(Keys.Y))
                {
                    gameObject.position = Scene.camera.ScreenToWorldPoint(Mouse.GetState().Position.ToVector2());
                    //Debug.WriteLine(Mouse.GetState().Position.ToVector2());
                    //Debug.WriteLine(gameObject.position);
                    //Debug.WriteLine(Scene.camera.WorldToScreenPoint(gameObject.position));
                }
                */
            }
            public override void Update(float deltaTime)
            {
                //Test(deltaTime);

                Vector2 pos = Scene.camera.position;
                float w = Scene.camera.width / 2;
                float h = Scene.camera.height / 2;


                if (gameObject.position.X > (pos.X + w) || gameObject.position.X < (pos.X - w))
                {
                    gameObject.GetComponent<Rigidbody>().velocity *= new Vector2(-1, 0);
                }
                if (gameObject.position.Y > (pos.Y + h) || gameObject.position.Y < (pos.Y - h))
                {
                    gameObject.GetComponent<Rigidbody>().velocity *= new Vector2(0, -1);
                }
                if (Input.GetKeyDown(Keys.S))
                {
                    gameObject.GetComponent<Rigidbody>().enabled = false;
                }
                if (Input.GetKeyDown(Keys.D))
                {
                    gameObject.GetComponent<Rigidbody>().enabled = true;
                }
                //Debug.WriteLine(gameObject.GetComponent<Rigidbody>().velocity);
            }
        }
        public void Start()
        {
            int count = 1000;
            Random r = new Random();

            Camera cam = Scene.camera;
            cam.position = new Vector2(350, 350);
            cam.width = 1000;
            cam.height = 1000;
            cam.rotation = 0;
            

            for (int i=0; i<count; i++)
            {
                GameObject temp = Scene.CreateGameObject("1");
                temp.position = new Vector2(r.Next(100, 600), r.Next(100, 600));
                temp.size = new Vector2(10, 10);
                temp.AddComponent<BenchmarkScript>();
                temp.AddComponent<Sprite>().texture = Scene.GetTexture("Box");
                Rigidbody rb = temp.AddComponent<Rigidbody>();
                rb.velocity = new Vector2(r.Next(100, 150), r.Next(100, 150));
                if (i % 2 == 0) rb.velocity *= new Vector2(-1, -1);
                rb.angularVelocity = r.Next(30, 60);
            }
        }
    }

    class NewGame : ISceneStarter
    {
        public void Start()
        {
            GameObject a = Scene.CreateGameObject("1");
            GameObject b = Scene.CreateGameObject("2");
            GameObject c = Scene.CreateGameObject("3");

            Camera cam = Scene.camera;
            cam.width = 20;
            cam.height = 20;

            a.size = new Vector2(10, 10);
            b.size = new Vector2(5, 5);
            c.size = new Vector2(5, 5);

            a.position = new Vector2(0, 0);
            b.position = new Vector2(-2.5f, 2.5f);
            c.position = new Vector2(2.5f, -2.5f);

            b.parent = a;
            c.parent = b;

            Sprite s_a = a.AddComponent<Sprite>();
            s_a.texture = Scene.GetTexture("Box");

            Sprite s_b = b.AddComponent<Sprite>();
            s_b.texture = Scene.GetTexture("Box");
            s_b.color = Color.Red;

            Sprite s_c = c.AddComponent<Sprite>();
            s_c.texture = Scene.GetTexture("Box");
            s_c.color = Color.Green;

            /*
            s_c.enabled = false;
            s_b.enabled = false;
            s_a.enabled = false;
            */

            Test t = a.AddComponent<Test>();
            t._gameObjects[0] = a;
            t._gameObjects[1] = b;
            t._gameObjects[2] = c;

            Component ss = s_c;
            Debug.WriteLine(ss.GetType());
            ss = t;
            Debug.WriteLine(ss.GetType());

            /*
            GameObject d = Scene.CreateGameObject("4");
            Sprite s_d = d.AddComponent<Sprite>();
            s_d.texture = Scene.GetTexture("TestTexture");
            d.position = new Vector2(300, 300);
            d.size = new Vector2(300, 300);

            s_d.flipX = true;
            s_d.flipX = true;
            s_d.flipX = false;
            s_d.flipY = true;
            s_d.flipY = false;
            s_d.flipY = false;
            */
        }
    }

    public static class Program
    {
        [STAThread]
        static void Main()
        {

            //using (var game = new Scene())
            //  game.Run();

            var game = new Scene(new Benchmark());
            game.Run();
        }
    }
}
