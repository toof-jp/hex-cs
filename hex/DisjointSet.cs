using System;
using System.Collections.Generic;
using System.Text;

namespace hex
{
    // verified
    // https://judge.yosupo.jp/submission/115166
    class DisjointSet
    {
        class Node
        {
            public int size;
            public int parent;

            public Node(int parent)
            {
                this.size = 1;
                this.parent = parent;
            }
        }

        Node[] nodes;

        public DisjointSet(int size)
        {
            this.nodes = new Node[size];
            for (int i = 0; i < size; i++)
            {
                nodes[i] = new Node(i);
            }
        }

        public int Find(int x)
        {
            if (nodes[x].parent == x)
                return x;
            return nodes[x].parent = Find(nodes[x].parent);
        }

        public void Unite(int x, int y)
        {
            x = Find(x);
            y = Find(y);

            if (x != y)
            {
                if (nodes[x].size < nodes[y].size)
                {
                    (x, y) = (y, x);
                }
                nodes[x].size += nodes[y].size;
                nodes[y].parent = x;
            }
        }

        public bool Same(int x, int y)
        {
            return Find(x) == Find(y);
        }
    }
}
