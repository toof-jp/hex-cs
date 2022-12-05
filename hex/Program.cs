using System;
using System.Collections.Generic;

namespace hex
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Sarsa(new Random());

            var v = s.Learning();

            // 勝率をCSVで出力
            Console.WriteLine(String.Join(",", v));

            // while (true) s.VsSarsa();

            //VsRandom();
        }

        // ランダムAIと対戦
        static void VsRandom()
        {
            var rnd = new Random();
            var hex = new Hex(4, rnd);
            while (hex.GetStateIsEnd() == Color.None)
            {
                hex.PrintBoard();
                if (hex.GetTurn() % 2 == 0)
                {
                    int y, x;
                    (y, x) = Util.ReadAction();
                    hex.DoAction(y, x);
                }
                else
                {
                    hex.DoRandomAction();
                }
            }
            hex.PrintBoard();
        }
    }
}