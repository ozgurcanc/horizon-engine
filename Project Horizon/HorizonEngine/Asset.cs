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
        private string _source;
        private uint _assetID;

        public Asset(string name, string source)
        {
            _name = name;
            _source = source;
            _assetID = Assets.availableAssetID;
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        internal uint assetID
        {
            get
            {
                return _assetID;
            }
        }

        internal string source
        {
            get
            {
                return _source;
            }
        }
    }
}
