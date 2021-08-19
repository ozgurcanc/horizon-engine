﻿using System;
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
        private bool _startFlag = true;
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

        internal bool startFlag
        {
            get
            {
                return _startFlag;
            }
            set
            {
                _startFlag = value;
            }
        }

        internal Component Clone()
        {
            Component component = (Component)this.MemberwiseClone();
            component._startFlag = true;
            return component;
        }
    }

    public interface IScene
    {
        void Load();
    }


}
