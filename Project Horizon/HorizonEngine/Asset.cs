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
    public abstract class Asset
    {
        private string _name;

        public Asset(string name)
        {
            _name = name;
        }

        public string name
        {
            get
            {
                return _name;
            }
        }
    }
}
