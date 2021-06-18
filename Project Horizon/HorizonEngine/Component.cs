using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace HorizonEngine
{
    public abstract class Component
    {
        private GameObject _gameObject;

        public GameObject gameObject
        {
            get
            {
                return _gameObject;
            }
            internal set
            {
                _gameObject = value;
            }
        }
    }

    public interface ISceneStarter
    {
        void Start();
    }

    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }

    public interface IUpdatable
    {
        void Update(GameTime gameTime);
    }


}
