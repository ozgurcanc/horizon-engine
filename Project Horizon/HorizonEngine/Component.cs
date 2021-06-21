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
        private bool _enabled = true;
        private int _componentID;

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

        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (value == _enabled) return;
                _enabled = value;
                if(_gameObject.activeInHierarchy)
                {
                    if (_enabled) Scene.EnableComponent(this);
                    else Scene.DisableComponent(this);
                }
            }
        }

        internal int componetID
        {
            get
            {
                return _componentID;
            }
            set
            {
                _componentID = value;
            }
        }
    }

    public interface ISceneStarter
    {
        void Start();
    }


}
