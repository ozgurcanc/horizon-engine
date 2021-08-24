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
            public bool hasRigidbody;
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
                temp.hasRigidbody = temp.collider.attachedRigidbody != null;
                _elements[i] = temp;            
            }

            //Debug.WriteLine(count);
            if(count > 0) BuildBVH(0, count);
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

        public HashSet<Tuple<Collider,Collider>> ReselveCollision()
        {
            HashSet<Tuple<Collider, Collider>> contactPairs = new HashSet<Tuple<Collider, Collider>>();
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
                                if((element.hasRigidbody || _elements[i].hasRigidbody) && (Physics.ignoreMask[(int)element.collider.gameObject.layer] & (1 << (int)_elements[i].collider.gameObject.layer)) == 0)
                                {                        
                                    Contact c = CollisionSystem.ResolveCollision(element.collider, _elements[i].collider);
                                    if (c != null)
                                    {
                                        if(!element.collider.isTrigger && !_elements[i].collider.isTrigger)
                                            contacts.Add(c);
                                        contactPairs.Add(Tuple.Create(element.collider, _elements[i].collider));
                                    }
                                }
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
            return contactPairs;
        }


        public List<Tuple<Collider, Collider>> GetPossibleContacts()
        {
            List<Tuple<Collider, Collider>> possibleContacts = new List<Tuple<Collider, Collider>>();
            int k = -1;
            foreach (LeafElement element in _elements)
            {
                k++;
                AABB aabb = element.aabb;
                int[] stack = new int[32];
                int pointer = 0;
                stack[0] = 0;

                while (pointer >= 0)
                {
                    BVHNode node = _nodes[stack[pointer]];

                    if (node.child[0] == 0)
                    {
                        for (int i = node.start; i < node.end; i++)
                        {
                            if (k > i)
                            {
                                if ((element.hasRigidbody || _elements[i].hasRigidbody) && (Physics.ignoreMask[(int)element.collider.gameObject.layer] & (1 << (int)_elements[i].collider.gameObject.layer)) == 0)
                                {
                                    possibleContacts.Add(Tuple.Create(element.collider, _elements[i].collider));
                                }
                            }
                        }
                        pointer--;
                    }
                    else
                    {
                        if (AABB.Intersect(node.aabb, aabb))
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

            return possibleContacts;
        }

        public HashSet<Collider> MouseCollision()
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            HashSet<Collider> touchingColliders = new HashSet<Collider>();

            if(_nodes.Count <= 0) return touchingColliders;

            int[] stack = new int[32];
            int pointer = 0;
            stack[0] = 0;

            while (pointer >= 0)
            {
                BVHNode node = _nodes[stack[pointer]];

                if (node.child[0] == 0)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        if (CollisionSystem.Intersect(_elements[i].collider, mouseWorldPos))
                        {
                            touchingColliders.Add(_elements[i].collider);
                        }
                    }
                    pointer--;
                }
                else
                {
                    if (AABB.Intersect(node.aabb, mouseWorldPos))
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

            return touchingColliders;
        }

    }
}
