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
    internal static class CollisionSystem
    {
        public static Contact ResolveCollision(Collider collider1, Collider collider2)
        {
            Contact contact = null;
            
            if(collider1 is CircleCollider && collider2 is CircleCollider)
            {
                contact = CircleAndCircle((CircleCollider)collider1, (CircleCollider)collider2);
            }
            else if(collider1 is CircleCollider && collider2 is BoxCollider)
            {
                contact = BoxAndCircle((BoxCollider)collider2, (CircleCollider)collider1);
            }
            else if (collider1 is BoxCollider && collider2 is CircleCollider)
            {
                contact = BoxAndCircle((BoxCollider)collider1, (CircleCollider)collider2);
            }
            else
            {
                contact = BoxAndBox((BoxCollider)collider1, (BoxCollider)collider2);
            }

            return contact;
        }

        public static bool Intersect(Collider collider, Vector2 point)
        {
            if(collider is BoxCollider)
            {
                return PointAndBox((BoxCollider)collider, point);
            }
            else
            {
                return PointAndCircle((CircleCollider)collider, point);
            }
        }

        private static bool PointAndCircle(CircleCollider collider, Vector2 point)
        {
            Vector2 distance = collider.transformMatrix.GetColumn(2) - point;
            return distance.LengthSquared() <= collider.radius * collider.radius;
        }

        private static bool PointAndBox(BoxCollider collider, Vector2 point)
        {
            Vector2 localPoint = collider.transformMatrix.InverseTransformPoint(point);
            return Math.Abs(localPoint.X) <= collider.halfSize.X && Math.Abs(localPoint.Y) <= collider.halfSize.Y;
        }

        private static Contact CircleAndCircle(CircleCollider collider1, CircleCollider collider2)
        {
            Vector2 center1 = collider1.transformMatrix.GetColumn(2);
            Vector2 center2 = collider2.transformMatrix.GetColumn(2);

            Vector2 midline = center1 - center2;
            float distance = midline.Length();
            float radiusSum = collider1.radius + collider2.radius;

            if (distance >= radiusSum)
                return null;

            float penetration;
            Vector2 normal;
            Vector2 contactPoint;

            if(distance == 0.0f)
            {
                penetration = collider1.radius;
                normal = new Vector2(1, 0);
                contactPoint = center1;
            }
            else
            {
                penetration = radiusSum - distance;
                normal = midline / distance;
                contactPoint = center1 + midline * 0.5f;               
            }

            Contact contact = new Contact
                (
                    collider1,
                    collider2,
                    contactPoint,
                    normal,
                    penetration
                );

            return contact;
        }

        private static Contact BoxAndCircle(BoxCollider box, CircleCollider circle)
        {
            Vector2 center = circle.transformMatrix.GetColumn(2);
            Vector2 localCenter = box.transformMatrix.InverseTransformPoint(center);

            float w = box.halfSize.X;
            float h = box.halfSize.Y;
            Vector2[] normals = { new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0) };
            Vector2[] vertex = { new Vector2(-w, -h), new Vector2(w, -h), new Vector2(w, h), new Vector2(-w, h) };
            float[] distanceToVertex = { -h - localCenter.Y, -w + localCenter.X, -h + localCenter.Y, -w - localCenter.X };

            float sep = float.MinValue;
            int faceIndex = -1;
            for (int i = 0; i < 4; i++)
            {
                if (distanceToVertex[i] > circle.radius)
                    return null;

                if (distanceToVertex[i] > sep)
                {
                    sep = distanceToVertex[i];
                    faceIndex = i;
                }
            }

            Vector2 vertex1 = vertex[faceIndex];
            Vector2 vertex2 = vertex[(faceIndex + 1) % 4];
            int edge1 = (faceIndex + 3) % 4;
            int edge2 = (faceIndex + 1) % 4;
            
            Vector2 contactPoint;
            Vector2 contactNormal;
            float penetration = circle.radius - sep;

            if (sep < 0.001f)
            {
                contactNormal = -box.transformMatrix.TransformNormal(normals[faceIndex]);
                contactPoint = contactNormal * circle.radius + center;
                penetration = circle.radius;
            }      
            else if (distanceToVertex[edge1] >= 0.0f)
            {          
                contactNormal = vertex1 - localCenter;
                if (contactNormal.Length() > circle.radius) return null;
                contactNormal = box.transformMatrix.TransformNormal(contactNormal);
                contactNormal.Normalize();
                vertex1 = box.transformMatrix.TransformPoint(vertex1);
                contactPoint = vertex1;
            }
            else if (distanceToVertex[edge2] >= 0.0f)
            {
                contactNormal = vertex2 - localCenter;
                if (contactNormal.Length() > circle.radius) return null;
                contactNormal = box.transformMatrix.TransformNormal(contactNormal);
                contactNormal.Normalize();
                vertex2 = box.transformMatrix.TransformPoint(vertex2);
                contactPoint = vertex2;
            }
            else
            {
                contactNormal = normals[faceIndex];
                if (Vector2.Dot(localCenter - vertex1, contactNormal) > circle.radius) return null;
                contactNormal = -box.transformMatrix.TransformNormal(contactNormal);
                contactPoint = contactNormal * circle.radius + center;
            }

            return new Contact
                (
                    box,
                    circle,
                    contactPoint,
                    contactNormal,
                    penetration
                );          
        }

        private static Contact BoxAndBox(BoxCollider collider1, BoxCollider collider2)
        {
            Vector2 toCenter = collider2.transformMatrix.GetColumn(2) - collider1.transformMatrix.GetColumn(2);
            float penetration = float.MaxValue;
            int bestAxis = -1;

            if (!TryAxis(collider1, collider2, collider1.transformMatrix.GetColumn(0), toCenter, 0, ref penetration, ref bestAxis))
                return null;
            if (!TryAxis(collider1, collider2, collider1.transformMatrix.GetColumn(1), toCenter, 1, ref penetration, ref bestAxis))
                return null;
            if (!TryAxis(collider1, collider2, collider2.transformMatrix.GetColumn(0), toCenter, 2, ref penetration, ref bestAxis))
                return null;
            if (!TryAxis(collider1, collider2, collider2.transformMatrix.GetColumn(1), toCenter, 3, ref penetration, ref bestAxis))
                return null;

            if (bestAxis == -1) return null; // ?

            if(bestAxis > 1)
            {
                bestAxis -= 2;
                toCenter *= -1;
                BoxCollider temp = collider1;
                collider1 = collider2;
                collider2 = temp;
            }

            Vector2 normal = collider1.transformMatrix.GetColumn(bestAxis);
            if (Vector2.Dot(normal, toCenter) > 0) normal *= -1;

            Vector2 vertex = collider2.halfSize;
            if (Vector2.Dot(collider2.transformMatrix.GetColumn(0), normal) < 0) vertex.X = -vertex.X;
            if (Vector2.Dot(collider2.transformMatrix.GetColumn(1), normal) < 0) vertex.Y = -vertex.Y;

            Contact contact = new Contact
                (
                    collider1,
                    collider2,
                    collider2.transformMatrix.TransformPoint(vertex),
                    normal,
                    penetration
                );

            return contact;

        }
        
        private static bool TryAxis(BoxCollider collider1, BoxCollider collider2, Vector2 axis, Vector2 toCenter, int axisIndex, ref float penetration, ref int bestAxis)
        {
            if (axis.LengthSquared() < 0.001f) return true;
            axis.Normalize();

            float pen = PenetrationOnAxis(collider1, collider2, axis, toCenter);

            if (pen < 0) return false;
            if(pen < penetration)
            {
                penetration = pen;
                bestAxis = axisIndex;
            }
            return true;
        }

        private static float PenetrationOnAxis(BoxCollider collider1, BoxCollider collider2, Vector2 axis, Vector2 toCenter)
        {
            float oneProject = collider1.halfSize.X * Math.Abs(Vector2.Dot(axis, collider1.transformMatrix.GetColumn(0)))
                + collider1.halfSize.Y * Math.Abs(Vector2.Dot(axis, collider1.transformMatrix.GetColumn(1)));
            float twoProject = collider2.halfSize.X * Math.Abs(Vector2.Dot(axis, collider2.transformMatrix.GetColumn(0)))
                + collider2.halfSize.Y * Math.Abs(Vector2.Dot(axis, collider2.transformMatrix.GetColumn(1)));

            float distance = Math.Abs(Vector2.Dot(toCenter, axis));

            return oneProject + twoProject - distance;
        }
    }
}
