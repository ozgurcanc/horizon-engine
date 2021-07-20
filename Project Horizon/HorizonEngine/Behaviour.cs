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
    public abstract class Behaviour : Component
    {
        public abstract void Update(float deltaTime);

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }
    }
}
