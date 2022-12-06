using System;
using System.Collections.Generic;
using System.Text;

namespace hex
{
    class Sarsa
    {
        const int EPISODE_LEN = 50_000_000;
        const int WIN_RATES_INTERVAL = 50_000;
        const double LEARNING_RATE = 0.86;
        const double DISCOUNT_RATE = 0.996;
        const double EPSILON = 0.22;
        const int BOARD_SIZE = 4;
        const long S_LEN = 43046721; // 3 ^ (4 ^ 2)  
        const int A_LEN = BOARD_SIZE * BOARD_SIZE;

        private Random rnd;
        double[,] q;
        Hex hex;
        double[] winRates;

        public Sarsa(Random rnd)
        {
            this.rnd = rnd;
            q = new double[S_LEN, A_LEN];
            winRates = new double[EPISODE_LEN / WIN_RATES_INTERVAL];
        }

        // 先手(Black)を学習して勝率を返す
        public double[] Learning()
        {
            int win = 0;

            for (int i = 0; i < EPISODE_LEN; i++)
            {
                hex = new Hex(BOARD_SIZE, rnd);

                while (hex.GetStateIsEnd() == Color.None)
                {
                    long state = hex.GetStateID();
                    int action = ActionSelect();

                    hex.DoAction(action);

                    hex.DoRandomAction();

                    var color = hex.GetStateIsEnd();

                    if (color == Color.None) // ゲームの進行中
                    {
                        long next_state = hex.GetStateID();
                        int next_action = ActionSelect();

                        q[state, action] =
                            (1 - LEARNING_RATE) * q[state, action]
                            + LEARNING_RATE * (DISCOUNT_RATE * q[next_state, next_action]);
                    }
                    else // 終状態
                    {
                        int r;
                        if (color == Color.Black) // 勝利
                        {
                            win++;
                            r = 1;
                        }
                        else // 敗北
                        {
                            r = -1;
                        }

                        q[state, action] =
                           (1 - LEARNING_RATE) * q[state, action]
                           + LEARNING_RATE * r;

                        if (i % WIN_RATES_INTERVAL == WIN_RATES_INTERVAL - 1)
                        {
                            winRates[i / WIN_RATES_INTERVAL] = (double)win / WIN_RATES_INTERVAL;
                            win = 0;
                        }


                        if (i % 100_000 == 0)
                        {
                            Console.Write('_');
                        }

                        break;
                    }
                }
            }

            Console.WriteLine();
            return winRates;
        }

        private int ActionSelect()
        {
            if (rnd.NextDouble() <= EPSILON)
            {
                return hex.GetRandomAvailableHand();
            }
            else
            {
                return MaxQ();
            }
        }

        // Q値が最大となるactionを返す
        private int MaxQ()
        {
            var hands = hex.GetAvailableHands();
            var state = hex.GetStateID();

            System.Diagnostics.Debug.Assert(hands.Count != 0);

            int action = -1;
            double maxQ = double.MinValue;

            foreach (var hand in hands)
            {
                if (q[state, hand] > maxQ)
                {
                    action = hand;
                    maxQ = q[state, hand];
                }
            }

            System.Diagnostics.Debug.Assert(action != -1);

            return action;
        }

        // 学習済みのAIと対戦
        // 人間は後手
        // Learning()を実行してからこの関数を実行する
        public void VsSarsa()
        {
            hex = new Hex(BOARD_SIZE, rnd);
            while (hex.GetStateIsEnd() == Color.None)
            {
                hex.PrintBoard();
                if (hex.GetTurn() % 2 == 1)
                {
                    int y, x;
                    (y, x) = Util.ReadAction();
                    hex.DoAction(y, x);
                }
                else
                {
                    int action = MaxQ();
                    hex.DoAction(action);
                }
            }
            hex.PrintBoard();
        }
    }
}
