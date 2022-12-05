using System;
using System.Collections.Generic;
using System.Text;

namespace hex
{
    class Util
    {
        public static (int, int) ReadAction()
        {
            int y = ReadInt("y: ");
            int x = ReadInt("x: ");

            return (y, x);
        }

        static int ReadInt(String text)
        {
            while (true)
            {
                Console.Write(text);
                var s = Console.ReadLine();
                try
                {
                    return int.Parse(s);
                }
                catch (Exception)
                {
                    Console.WriteLine("数値を入力してください");
                }
            }
        }
    }
}
