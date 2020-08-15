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
        private int turn;


        private bool AllNumEQ(int toCheck, params int[] numbers)
        {
            foreach (int num in numbers)
            {
                if (num != toCheck)
                    return false;
            }
            return true;
        }

        private int winnerPlayer(int player2Check)
        {
            //vertical win (|)
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            //-3  cuz there is just 3 hols from the top ot bottom
            {
                for (int col = 0; col < this.board.GetLength(1); col++)
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row + 1, col], this.board[row + 2, col], this.board[row + 3, col]))
                        return player2Check;
                }
            }


            //horizontal win (---)
            for (int row = 0; row < this.board.GetLength(0); row++)
            {
                for (int col = 0; col < this.board.GetLength(1) - 3; col++)
                //-3  cuz there is just 3 hols from the left or right
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row, col + 1], this.board[row, col + 2], this.board[row, col + 3]))
                        return player2Check;
                }
            }

            //***********************diagonal
            //top left win (\)
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < this.board.GetLength(1) - 3; col++)
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row + 1, col + 1], this.board[row + 2, col + 2], this.board[row + 3, col + 3]))
                        return player2Check;
                }
            }

            //top right win (/)
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int col = 3; col < this.board.GetLength(1); col++)
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row + 1, col - 1], this.board[row + 2, col - 2], this.board[row + 3, col - 3]))
                        return player2Check;
                }
            }


            return -1;
        }


    }/*end of -Game- class*/

}/*end of -WcfFourRowService- namespace*/
