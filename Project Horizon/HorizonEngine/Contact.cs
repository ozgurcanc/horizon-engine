using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace HorizonEngine
{
    internal class Contact
    {
        public static int i = 0;
        private Rigidbody[] _rigidbodies;
        private float _friction;
        private float _restitution;
        private float _penetration;
        private Vector2 _contactPoint;
        private Vector2 _contactNormal;

        internal Contact(Rigidbody rigidbody1, Rigidbody rigidbody2, Vector2 contactPoint, Vector2 contactNormal, float penetration, float friction, float restitution)
        {
            _rigidbodies = new Rigidbody[2];
            _rigidbodies[0] = rigidbody1;
            _rigidbodies[1] = rigidbody2;
            _contactPoint = contactPoint;
            _contactNormal = contactNormal;
            _penetration = penetration;
            _friction = friction;
            _restitution = restitution;
            
            /*
            if (float.IsNaN(contactNormal.X) || float.IsNaN(contactNormal.Y))
            {
                if(rigidbody1.gameObject.GetComponent<BoxCollider>() != null && rigidbody2.gameObject.GetComponent<BoxCollider>() != null)
                    Debug.WriteLine("box-box");
                if (rigidbody1.gameObject.GetComponent<CircleCollider>() != null && rigidbody2.gameObject.GetComponent<BoxCollider>() != null)
                    Debug.WriteLine("box-circle");
                if (rigidbody1.gameObject.GetComponent<BoxCollider>() != null && rigidbody2.gameObject.GetComponent<CircleCollider>() != null)
                    Debug.WriteLine("circle-box");
                if (rigidbody1.gameObject.GetComponent<CircleCollider>() != null && rigidbody2.gameObject.GetComponent<CircleCollider>() != null)
                    Debug.WriteLine("circle-circle");

                Debug.WriteLine(rigidbody1.gameObject.position);
                Debug.WriteLine(rigidbody2.gameObject.position);
                throw new NotImplementedException();
            }
            */
            
        }

        internal void ResolveContact()
        {
            //Debug.WriteLine("intersect");
            //Debug.WriteLine(_contactPoint);
            //if (float.IsNaN(_contactNormal.X) || float.IsNaN(_contactNormal.Y)) return;

            if (_penetration < 0.05f) _penetration = 0f;
            Vector2 distance = _penetration * _contactNormal * 0.4f;

            float totalMass = _rigidbodies[0].inverseMass + _rigidbodies[1].inverseMass;
            if (totalMass == 0) Debug.WriteLine("Nan");

            _rigidbodies[0].position += (_rigidbodies[0].inverseMass / totalMass) * distance;
            _rigidbodies[1].position -= (_rigidbodies[1].inverseMass / totalMass) * distance;

            float j = -(1 + 0f);
            Vector2 relativeVelocity = _rigidbodies[0].velocity - _rigidbodies[1].velocity;
            j *= Vector2.Dot(relativeVelocity, _contactNormal);
            if (j < 0)
                return;
            Vector2 relativePosition1 = _contactPoint - _rigidbodies[0].position;
            Vector2 relativePosition2 = _contactPoint - _rigidbodies[1].position;

            Vector2 totalInertia = Cross(_rigidbodies[0].inverseMass * Cross(relativePosition1, _contactNormal), relativePosition1);
            totalInertia += Cross(_rigidbodies[1].inverseMass * Cross(relativePosition2, _contactNormal), relativePosition2);

            float d = totalMass + Vector2.Dot(totalInertia, _contactNormal);
            j /= d;

            Vector2 totalVelocity1 = _rigidbodies[0].velocity + Cross(_rigidbodies[0].angularVelocity, relativePosition1);
            Vector2 totalVelocity2 = _rigidbodies[1].velocity + Cross(_rigidbodies[1].angularVelocity, relativePosition2);

            Vector2 t = totalVelocity1 - totalVelocity2;
            t = Cross(Cross(_contactNormal, t), _contactNormal);
            float len = t.Length();
            if (len != 0) t.Normalize();
            else t = Vector2.Zero;

            float jt = -Vector2.Dot(relativeVelocity, t);
            jt /= d;

            float ff = 0.01f;
            Vector2 tangentImpulse = jt * t * ff;
            Vector2 impulse = j * _contactNormal + tangentImpulse;

            //Debug.WriteLine(impulse);

            _rigidbodies[0].velocity += impulse * _rigidbodies[0].inverseMass;
            _rigidbodies[1].velocity -= impulse * _rigidbodies[1].inverseMass;

            _rigidbodies[0].angularVelocity += Cross(impulse, relativePosition1) * _rigidbodies[0].inverseInertia;
            _rigidbodies[1].angularVelocity -= Cross(impulse, relativePosition1) * _rigidbodies[1].inverseInertia;
        }

        private float Cross(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        private Vector2 Cross(Vector2 v, float f)
        {
            return new Vector2(f * v.Y, -f * v.X);
        }

        private Vector2 Cross(float f, Vector2 v)
        {
            return new Vector2(-f * v.Y, f * v.X);
        }
    }
}
