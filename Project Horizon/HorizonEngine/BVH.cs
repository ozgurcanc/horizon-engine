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
    internal class BVH
    {
        class BVHNode
        {
            public AABB aabb;
            public int[] child = new int[2];
            public int start = 0;
            public int end = 0;
        }

        class LeafElement
        {
            public AABB aabb;
            public Collider collider;
        }

        private LeafElement[] _elements;
        private List<BVHNode> _nodes;

        public BVH(Collider[] colliders)
        {
            int count = colliders.Length;
            _elements = new LeafElement[count];
            _nodes = new List<BVHNode>();

            for (int i=0; i<count; i++)
            {
                colliders[i].UpdateCollider();
                LeafElement temp = new LeafElement();
                temp.aabb = colliders[i].aabb;
                temp.collider = colliders[i];
                _elements[i] = temp;
            }

            //Debug.WriteLine(count);
            BuildBVH(0, count);
        }

        private int BuildBVH(int start, int end)
        {
            //Debug.WriteLine(start + " " + end);
            int nodeIndex = _nodes.Count;
            BVHNode node = new BVHNode();

            AABB bounds = _elements[start].aabb;
            AABB centerBounds = new AABB(_elements[start].aabb.center);

            for (int i = start +1; i < end; i++)
            {
                AABB temp = _elements[i].aabb;
                bounds = AABB.Union(bounds, temp);
                centerBounds = AABB.Union(centerBounds, temp.center);
            }

            int elementCount = end - start;

            if(elementCount <= 2 || centerBounds.max == centerBounds.min)
            {
                node.aabb = bounds;
                node.start = start;
                node.end = end;
                node.child[0] = 0;
                node.child[1] = 0;

                _nodes.Add(node);
                return nodeIndex;
            }

            int splitAxis = centerBounds.MaximumExtent();

            int mid = start - 1;

            if(splitAxis == 0)
            {
                float center = centerBounds.center.X;
                for (int i = start; i < end; i++)
                {
                    AABB temp = _elements[i].aabb;
                    if (temp.center.X < center)
                    {
                        mid++;
                        LeafElement swap = _elements[i];
                        _elements[i] = _elements[mid];
                        _elements[mid] = swap;
                    }
                }
            }
            else
            {
                float center = centerBounds.center.Y;
                for (int i = start; i < end; i++)
                {
                    AABB temp = _elements[i].aabb;
                    if (temp.center.Y < center)
                    {
                        mid++;
                        LeafElement swap = _elements[i];
                        _elements[i] = _elements[mid];
                        _elements[mid] = swap;
                    }
                }
            }
      
            _nodes.Add(node);
            node.child[0] = BuildBVH(start, mid + 1);
            node.child[1] = BuildBVH(mid + 1, end);

            node.aabb = AABB.Union(_nodes[node.child[0]].aabb, _nodes[node.child[1]].aabb);

            return nodeIndex;
        }

        public int ReselveCollision()
        {
            List<Contact> contacts = new List<Contact>();
            int k = -1;
            foreach(LeafElement element in _elements)
            {
                k++;
                AABB aabb = element.aabb;
                int[] stack = new int[32];
                int pointer = 0;
                stack[0] = 0;

                while(pointer >= 0)
                {
                    BVHNode node = _nodes[stack[pointer]];
                    
                    if(node.child[0] == 0)
                    {
                        for(int i=node.start; i<node.end; i++)
                        {
                            if(k > i)
                            {
                                Contact c = CollisionSystem.ResolveCollision(element.collider, _elements[i].collider); ;
                                if (c != null) contacts.Add(c);
                            }                                
                        }
                        pointer--;
                    }
                    else
                    {
                        if(AABB.Intersect(node.aabb, aabb))
                        {
                            stack[pointer] = node.child[0];
                            pointer++;
                            stack[pointer] = node.child[1];
                        }
                        else
                        {
                            pointer--;
                        }
                    }
                }
            }

            foreach (var x in contacts) x.ResolveContact();
            return contacts.Count;
        }

    }
}
