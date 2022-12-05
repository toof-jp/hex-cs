using System;
using System.Collections.Generic;
using System.Text;

namespace hex
{
    enum Color
    {
        Black, // 先手
        White, // 後手
        None
    }

    class Hex
    {
        private int boardSize;
        private int turn;
        private Color[,] board;
        private Graph graph;
        private Random rnd;

        public Hex(int boardSize, Random rnd)
        {
            this.boardSize = boardSize;
            this.board = new Color[boardSize, boardSize];
            this.graph = new Graph(boardSize);
            this.rnd = rnd;

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    board[y, x] = Color.None;
                }
            }
        }

        public Color GetColor(int y, int x)
        {
            return board[y, x];
        }

        public int GetTurn()
        {
            return turn;
        }

        public long GetStateID()
        {
            System.Diagnostics.Debug.Assert(Math.Pow(3, boardSize * boardSize) <= System.Int64.MaxValue, "boardSizeが大きいのでオーバーフロー");

            long state = 0;
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    state *= 3;
                    state += (long)board[y, x];
                }
            }
            return state;
        }

        // ゲームが終了していなければ、Color.Noneを返す
        // ゲームが終了していれば、勝者の色を返す
        public Color GetStateIsEnd()
        {
            int n = boardSize * boardSize;
            if (graph.ds.Same(n + 0, n + 2))
            {
                return Color.Black;
            }
            else if (graph.ds.Same(n + 1, n + 3))
            {
                return Color.White;
            }
            else
            {
                return Color.None;
            }
        }

        public List<int> GetAvailableHands()
        {
            var list = new List<int>();

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == Color.None)
                    {
                        list.Add(i * boardSize + j);
                    }
                }
            }

            return list;
        }

        public int GetRandomAvailableHand()
        {
            var list = GetAvailableHands();
            System.Diagnostics.Debug.Assert(list.Count != 0);
            int index = rnd.Next(list.Count);
            return list[index];
        }

        // 着手に成功すればtrueを返す
        public bool DoAction(int y, int x)
        {
            if (!(0 <= y && y < boardSize && 0 <= x && x < boardSize))
            {
                return false;
            }
            if (GetColor(y, x) != Color.None)
            {
                return false;
            }

            Color nowColor = turn % 2 == 0 ? Color.Black : Color.White;
            board[y, x] = nowColor;
            graph.Set(y, x, nowColor);
            turn++;
            return true;
        }

        // 着手に成功すればtrueを返す
        public bool DoAction(int pos)
        {
            int y = pos / boardSize;
            int x = pos % boardSize;
            return DoAction(y, x);
        }

        public void DoRandomAction()
        {
            if (GetStateIsEnd() != Color.None)
            {
                return;
            }
            DoAction(GetRandomAvailableHand());
        }

        public void PrintBoard()
        {
            Console.WriteLine("turn: " + turn);
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = boardSize - i - 1; j > 0; j--)
                {
                    Console.Write(" ");
                }
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == Color.Black)
                    {
                        Console.Write("●");
                    }
                    else if (board[i, j] == Color.White)
                    {
                        Console.Write("◯");
                    }
                    else
                    {
                        Console.Write("_ ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    class Graph
    {
        private int boardSize;
        private Node[] node;
        public DisjointSet ds;

        class Node
        {
            public List<int> adjacencyList;
            public Color color;

            public Node()
            {
                this.adjacencyList = new List<int>();
                this.color = Color.None;
            }
        }

        public Graph(int boardSize)
        {
            this.boardSize = boardSize;
            int n = boardSize * boardSize;
            this.node = new Node[n + 4];
            this.ds = new DisjointSet(n + 4);

            for (int i = 0; i < n + 4; i++)
            {
                node[i] = new Node();
            }

            // マスとマスをつなぐ辺(グラフ)を作成
            for (int i = 0; i < n; i++)
            {
                int y = i / boardSize;
                int x = i % boardSize;

                int[] dy = { -1, -1, 0, 0, 1, 1 };
                int[] dx = { -1, 0, -1, 1, 0, 1 };
                // 左上, 上, 左, 右, 下, 右下

                for (int j = 0; j < dy.Length; j++)
                {
                    int next_y = y + dy[j];
                    int next_x = x + dx[j];
                    if (0 <= next_y && next_y < boardSize && 0 <= next_x && next_x < boardSize)
                    {
                        node[i].adjacencyList.Add(GetIndex(next_y, next_x));
                    }
                }
            }

            // 辺(端)とマスをつなぐ辺(グラフ)を作成
            // 上
            node[n + 0].color = Color.Black;
            for (int i = 0; i < boardSize; i++)
            {
                int index = GetIndex(0, i);
                node[n + 0].adjacencyList.Add(index);
                node[index].adjacencyList.Add(n + 0);
            }

            // 右
            node[n + 1].color = Color.White;
            for (int i = 0; i < boardSize; i++)
            {
                int index = GetIndex(i, boardSize - 1);
                node[n + 1].adjacencyList.Add(index);
                node[index].adjacencyList.Add(n + 1);
            }

            // 下
            node[n + 2].color = Color.Black;
            for (int i = 0; i < boardSize; i++)
            {
                int index = GetIndex(boardSize - 1, i);
                node[n + 2].adjacencyList.Add(index);
                node[index].adjacencyList.Add(n + 2);
            }

            // 左
            node[n + 3].color = Color.White;
            for (int i = 0; i < boardSize; i++)
            {
                int index = GetIndex(i, 0);
                node[n + 3].adjacencyList.Add(index);
                node[index].adjacencyList.Add(n + 3);
            }
        }

        public void Set(int y, int x, Color c)
        {
            int index = GetIndex(y, x);
            node[index].color = c;
            foreach (int i in node[index].adjacencyList)
            {
                if (node[i].color == c)
                {
                    ds.Unite(index, i);
                }
            }
        }

        private int GetIndex(int y, int x)
        {
            return boardSize * y + x;
        }
    }
}
