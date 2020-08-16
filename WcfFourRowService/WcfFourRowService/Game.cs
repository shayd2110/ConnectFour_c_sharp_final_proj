using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*WcfFourRowService namespace*/
namespace WcfFourRowService
{
    /*Game class*/
    /// <summary>
    /// class that represent the rules and such for four row game
    /// </summary>
    class Game
    {
        private int[,] board;
        private int turn = 1;
        private const int rows = 6;
        private const int cols = 7;
        private int moveCnt = 0;

        public int PosPlayer1 { get; set; }

        public int PosPlayer2 { get; set; }

        public Game()
        {
            board = new int[6, 7];
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 7; ++j)
                    board[i, j] = 0;
            }

            moveCnt = 0;
            PosPlayer1 = PosPlayer2 = 0;
        }

        private bool AllNumEQ(int toCheck, params int[] numbers)
        {
            foreach (int number in numbers)
            {
                if (number != toCheck)
                    return false;
            }
            return true;
        }

        private bool AllNumInAnyColEQ(int toCheck)
        {
            bool check;

            for (int i = 0; i <= cols - 1; i++)
            {
                check = false;

                for (int j = 0; j <= rows - 1; j++)
                {
                    if (board[j, i] == toCheck)
                    {
                        check = true;
                        break;
                    }
                }

                if (!check)
                    return false;
            }

            return true;
        }

        private bool winnerPlayer()
        {
            int turn = this.turn;
            //vertical win (|)
            for (int row = 0; row < board.GetLength(0) - 3; row++)
            //-3  cuz there is just 3 hols from the top ot bottom
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (AllNumEQ(turn, board[row, col], board[row + 1, col], board[row + 2, col], board[row + 3, col]))
                        return true;
                }
            }
            for (int row = 0; row < board.GetLength(0); ++row)
            {
                for (int col = 0; col < board.GetLength(1) - 3; ++col)
                {
                    if (AllNumEQ(turn, board[row, col], board[row, col + 1], board[row, col + 2], board[row, col + 3]))
                        return true;
                }
            }
            for (int row = 0; row < board.GetLength(0) - 3; ++row)
            {
                for (int col = 0; col < board.GetLength(1) - 3; ++col)
                {
                    if (AllNumEQ(turn, board[row, col], board[row + 1, col + 1], board[row + 2, col + 2], board[row + 3, col + 3]))
                        return true;
                }
            }
            for (int row = 0; row < board.GetLength(0) - 3; ++row)
            {
                for (int col = 3; col < board.GetLength(1); ++col)
                {
                    if (AllNumEQ(turn, board[row, col], board[row + 1, col - 1], board[row + 2, col - 2], board[row + 3, col - 3]))
                        return true;
                }
            }
            return false;
        }

        internal Tuple<int, int> CalcPoints(string currentPlayer, string opponent, string winner)
        {
            int pointsPlayer1 = 0;
            int pointsPlayer2 = 0;

            if (winner == "draw")
            {
                pointsPlayer1 = 100 + (((rows * cols / 2) + 1) * 10);
                pointsPlayer2 = 100 + (((rows * cols / 2)) * 10);
            }
            else
            {
                int winnerNumber = winner == currentPlayer ? 1 : 2;

                pointsPlayer1 += winnerNumber == 1 ? 1000 : (moveCnt / 2) * 10;
                pointsPlayer2 += winnerNumber == 2 ? 1000 : (moveCnt / 2) * 10;

               
                if (AllNumInAnyColEQ(1))
                    pointsPlayer1 += 100;

                if (AllNumInAnyColEQ(2))
                    pointsPlayer2 += 100;
            }
            return Tuple.Create(pointsPlayer1, pointsPlayer2);
        }

        internal Tuple<MoveResult, int> verifyMove(
          string currentPlayer,
          string opponent,
          string whoMoved,
          int location)
        {
            int playerNumber = whoMoved == currentPlayer ? 1 : 2;
            int row = -1;

            if (turn != playerNumber)
                return Tuple.Create(MoveResult.NotYourTurn, row);

            int loc = location - 1;
            if (board[0, loc] != 0)
                return Tuple.Create<MoveResult, int>(MoveResult.IllegalMove, row);

            for (int i = rows - 1; i >= 0; --i)
            {
                if (board[i, loc] == 0)
                {
                    board[i, loc] = turn;
                    row = i;
                    break;
                }
            }

            int posInCanvas = getPosInCanvas(row);

            if (winnerPlayer())
                return Tuple.Create(MoveResult.YouWon, posInCanvas);

            ++moveCnt;

            if (moveCnt == rows * cols)
                return Tuple.Create(MoveResult.Draw, posInCanvas);

            turn = turn == 1 ? 2 : 1;

            return Tuple.Create(MoveResult.GameOn, posInCanvas);
        }

        private int getPosInCanvas(int row)
        {
            switch (row)
            {
                case 0:
                    return 18;
                case 1:
                    return 93;
                case 2:
                    return 168;
                case 3:
                    return 243;
                case 4:
                    return 318;
                case 5:
                    return 396;
                default:
                    return -1;
            }
        }

        internal int getPos(string currentPlayer, string whoWantIt) => whoWantIt == currentPlayer ? PosPlayer1 : PosPlayer2;

        internal void setPos(string currentPlayer, string whoWantIt, int pos)
        {
            if (whoWantIt == currentPlayer)
                PosPlayer1 = pos;
            else
                PosPlayer2 = pos;
        }


    }/*end of -Game- class*/

}/*end of -WcfFourRowService- namespace*/
