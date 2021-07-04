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
    internal class TransformMatrix
    {
        private Vector2[] _columns;

        internal TransformMatrix(Vector2 position, float rotation)
        {
            _columns = new Vector2[3];
            float cosa = (float)Math.Cos(rotation);
            float sina = (float)Math.Sin(rotation);

            _columns[0].X = cosa;
            _columns[0].Y = sina;
            _columns[1].X = -sina;
            _columns[1].Y = cosa;
            _columns[2] = position;
        }
        
        internal Vector2 GetColumn(int index)
        {
            return _columns[index];
        }

        internal Vector2 TransformPoint(Vector2 point)
        {
            Vector2 temp = new Vector2();
            temp.X = point.X * _columns[0].X + point.Y * _columns[1].X + _columns[2].X;
            temp.Y = point.X * _columns[0].Y + point.Y * _columns[1].Y + _columns[2].Y;
            return temp;
        }

        internal Vector2 TransformNormal(Vector2 point)
        {
            Vector2 temp = new Vector2();
            temp.X = point.X * _columns[0].X + point.Y * _columns[1].X;
            temp.Y = point.X * _columns[0].Y + point.Y * _columns[1].Y;
            return temp;
        }

        internal Vector2 InverseTransformPoint(Vector2 point)
        {
            point -= _columns[2];
            Vector2 temp = new Vector2();
            temp.X = _columns[0].X * point.X + _columns[0].Y * point.Y;
            temp.Y = _columns[1].X * point.X + _columns[1].Y * point.Y;
            return temp;

        }
    }
}
