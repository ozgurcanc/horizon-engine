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
        private Collider[] _colliders;
        private Rigidbody[] _rigidbodies;
        private float _friction;
        private float _restitution;
        private float _penetration;
        private Vector2 _contactPoint;
        private Vector2 _contactNormal;
        private bool _isTrigger;

        internal Contact(Collider collider1, Collider collider2, Vector2 contactPoint, Vector2 contactNormal, float penetration)
        {
            _colliders = new Collider[2];
            _rigidbodies = new Rigidbody[2];
            _colliders[0] = collider1;
            _colliders[1] = collider2;
            _rigidbodies[0] = collider1.attachedRigidbody;
            _rigidbodies[1] = collider2.attachedRigidbody;
            _contactPoint = contactPoint;
            _contactNormal = contactNormal;
            _penetration = penetration;
            //_friction = friction;
            //_restitution = restitution;
            _isTrigger = collider1.isTrigger || collider2.isTrigger;

            float friction1 = collider1.physicsMaterial == null ? 0f : collider1.physicsMaterial.friction;
            float friction2 = collider2.physicsMaterial == null ? 0f : collider2.physicsMaterial.friction;
            if (Physics.frictionBlendMode == PhysicsMaterialBlendMode.Average) _friction = (friction1 + friction2) / 2f;
            else if (Physics.frictionBlendMode == PhysicsMaterialBlendMode.Minumum) _friction = Math.Min(friction1, friction2);
            else if (Physics.frictionBlendMode == PhysicsMaterialBlendMode.Maximum) _friction = Math.Max(friction1, friction2);
            else if (Physics.frictionBlendMode == PhysicsMaterialBlendMode.Multiply) _friction = friction1 * friction2;

            float restitution1 = collider1.physicsMaterial == null ? 0f : collider1.physicsMaterial.restitution;
            float restitution2 = collider2.physicsMaterial == null ? 0f : collider2.physicsMaterial.restitution;
            if (Physics.restitutionBlendMode == PhysicsMaterialBlendMode.Average) _restitution = (restitution1 + restitution2) / 2f;
            else if (Physics.restitutionBlendMode == PhysicsMaterialBlendMode.Minumum) _restitution = Math.Min(restitution1, restitution2);
            else if (Physics.restitutionBlendMode == PhysicsMaterialBlendMode.Maximum) _restitution = Math.Max(restitution1, restitution2);
            else if (Physics.restitutionBlendMode == PhysicsMaterialBlendMode.Multiply) _restitution = restitution1 * restitution2;

            if (float.IsNaN(contactNormal.X) || float.IsNaN(contactNormal.Y))
            {
                Rigidbody rigidbody1 = collider1.attachedRigidbody;
                Rigidbody rigidbody2 = collider2.attachedRigidbody;
                Collider one = null, two = null;
                if(rigidbody1.gameObject.GetComponent<BoxCollider>() != null && rigidbody2.gameObject.GetComponent<BoxCollider>() != null)
                {
                    Debug.WriteLine("box-box");
                    one = rigidbody1.gameObject.GetComponent<BoxCollider>();
                    two = rigidbody2.gameObject.GetComponent<BoxCollider>();
                }
                if (rigidbody1.gameObject.GetComponent<CircleCollider>() != null && rigidbody2.gameObject.GetComponent<BoxCollider>() != null)
                {
                    Debug.WriteLine("box-circle");
                    one = rigidbody1.gameObject.GetComponent<CircleCollider>();
                    two = rigidbody2.gameObject.GetComponent<BoxCollider>();
                }
                if (rigidbody1.gameObject.GetComponent<BoxCollider>() != null && rigidbody2.gameObject.GetComponent<CircleCollider>() != null)
                {
                    Debug.WriteLine("box-circle");
                    one = rigidbody1.gameObject.GetComponent<BoxCollider>();
                    two = rigidbody2.gameObject.GetComponent<CircleCollider>();
                }
                if (rigidbody1.gameObject.GetComponent<CircleCollider>() != null && rigidbody2.gameObject.GetComponent<CircleCollider>() != null)
                {
                    Debug.WriteLine("circle-circle");
                    one = rigidbody1.gameObject.GetComponent<CircleCollider>();
                    two = rigidbody2.gameObject.GetComponent<CircleCollider>();
                }
            

                Debug.WriteLine(rigidbody1.gameObject.position);
                Debug.WriteLine(rigidbody2.gameObject.position);
               // throw new NotImplementedException();
                CollisionSystem.ResolveCollision(one, two);
                
            }
            
            
        }

        internal void ResolveContact()
        {
            if (_isTrigger) return;

            if(_rigidbodies[0] == null)
            {
                _rigidbodies[0] = _rigidbodies[1];
                _rigidbodies[1] = null;
                _contactNormal = -_contactNormal;
            }

            bool haveRigidbody = _rigidbodies[1] != null;

            if (_penetration < 0.05f) _penetration = 0f;
            Vector2 distance = _penetration * _contactNormal * 0.4f;

            float totalMass = _rigidbodies[0].inverseMass;
            Vector2 relativeVelocity = _rigidbodies[0].velocity;

            if (haveRigidbody)
            {
                totalMass += _rigidbodies[1].inverseMass;
                relativeVelocity -= _rigidbodies[1].velocity;
            }
            
            if (totalMass == 0) Debug.WriteLine("Nan");         
            float j = -(1 + _restitution);       
            j *= Vector2.Dot(relativeVelocity, _contactNormal);
            if (j < 0)
                return;

            Vector2 relativePosition1 = _contactPoint - _rigidbodies[0].position;
            Vector2 totalInertia = Cross(_rigidbodies[0].inverseInertia * Cross(relativePosition1, _contactNormal), relativePosition1);
            Vector2 totalVelocity1 = _rigidbodies[0].velocity + Cross(_rigidbodies[0].angularVelocity, relativePosition1);
            Vector2 t = totalVelocity1;

            Vector2 relativePosition2 = Vector2.Zero;
            if (haveRigidbody)
            {
                relativePosition2 = _contactPoint - _rigidbodies[1].position;
                totalInertia += Cross(_rigidbodies[1].inverseInertia * Cross(relativePosition2, _contactNormal), relativePosition2);
                Vector2 totalVelocity2 = _rigidbodies[1].velocity + Cross(_rigidbodies[1].angularVelocity, relativePosition2);
                t -= totalVelocity2;
            }           

            float d = totalMass + Vector2.Dot(totalInertia, _contactNormal);
            j /= d;

            t = Cross(Cross(_contactNormal, t), _contactNormal);
            float len = t.Length();
            if (len != 0) t.Normalize();
            else t = Vector2.Zero;

            float jt = -Vector2.Dot(relativeVelocity, t);
            jt /= d;

            Vector2 tangentImpulse = jt * t * _friction * 0.1f;
            Vector2 impulse = j * _contactNormal + tangentImpulse;

            _rigidbodies[0].velocity += impulse * _rigidbodies[0].inverseMass;
            _rigidbodies[0].angularVelocity += Cross(impulse, relativePosition1) * _rigidbodies[0].inverseInertia;
            _rigidbodies[0].position += (_rigidbodies[0].inverseMass / totalMass) * distance;

            if(haveRigidbody)
            {
                _rigidbodies[1].velocity -= impulse * _rigidbodies[1].inverseMass;
                _rigidbodies[1].angularVelocity -= Cross(impulse, relativePosition2) * _rigidbodies[1].inverseInertia;
                _rigidbodies[1].position -= (_rigidbodies[1].inverseMass / totalMass) * distance;
            }
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

        internal Collision GetCollisionData(int collider)
        {
            Collision collision = new Collision();
            collision.collider = collider == 0 ? _colliders[1] : _colliders[0];
            collision.contactNormal = _contactNormal;
            collision.contactPoint = _contactPoint;
            collision.penetration = _penetration;

            return collision;
        }

        internal Tuple<Collider, Collider> GetContactPair()
        {
            return Tuple.Create(_colliders[0], _colliders[1]);
        }
    }
}
