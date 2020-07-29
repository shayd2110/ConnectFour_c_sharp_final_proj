using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;

namespace FourRowForms
{
    public partial class GameFormGui : Form
    {
        private Rectangle[] boardCols;
        private const int seven = 7;
        private int[,] board;
        private int turn;


        public GameFormGui()
        {
            InitializeComponent();
            this.boardCols = new Rectangle[seven];

            this.board = new int[seven - 1, seven];
            this.turn = 1;

        }
        private void GameFormGui_paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Blue, 24, 24, 340, 300);
            for (int i = 0; i < seven - 1; i++)
            {
                for (int j = 0; j < seven; j++)
                {
                    if (i == 0)
                        this.boardCols[j] = new Rectangle(32 + 48 * j, 24, 32, 300);
                    e.Graphics.FillEllipse(Brushes.White, 32 + 48 * j, 32 + 48 * i, 32, 32);
                }
            }
        }

        private void GameFormGui_MouseClick(object sender, MouseEventArgs e)
        {
            int colIndex = this.ColNum(e.Location);
            if (colIndex != -1)
            {
                int rowIndex = this.EmptyRow(colIndex);
                if (rowIndex != -1)
                {
                    this.board[rowIndex, colIndex] = this.turn;
                    if (this.turn == 1)
                    {
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(Brushes.Red, 32 + 48 * colIndex, 32 + 48 * rowIndex, 32, 32);
                    }
                    else
                    {
                        Graphics g = this.CreateGraphics();
                        g.FillEllipse(Brushes.Yellow, 32 + 48 * colIndex, 32 + 48 * rowIndex, 32, 32);
                    }
                    int winner = this.winnerPlayer(this.turn);
                    if (winner != -1)
                    {
                        string player = (winner == 1) ? "Red" : "Yellow";
                        MessageBox.Show(player + " player  has won");
                        Application.Restart();
                    }
                    this.turn = (this.turn == 1) ?  2 :  1;
                   
                }

            }


        }

        private int ColNum(Point mouse)
        {
            for (int i = 0; i < this.boardCols.Length; i++)
            {
                if ((mouse.X >= this.boardCols[i].X) && (mouse.Y >= this.boardCols[i].Y))
                {
                    if ((mouse.X <= this.boardCols[i].X + this.boardCols[i].Width) && (mouse.Y <= this.boardCols[i].Y + this.boardCols[i].Height))
                        return i;
                }
            }
            return -1;
        }

        private int EmptyRow(int col)
        {
            for (int i = seven - 2; i >= 0; i--)
            {
                if (this.board[i, col] == 0)
                    return i;

            }
            return -1;
        }

        private bool AllNumEQ (int toCheck, params int[] numbers)
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
            for (int row  = 0; row  < this.board.GetLength(0)-3; row ++)
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
                for (int col = 0; col < this.board.GetLength(1)-3; col++)
                //-3  cuz there is just 3 hols from the left or right
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row, col+1], this.board[row, col+2], this.board[row, col+3]))
                        return player2Check;
                }
            }

            //***********************diagonal
            //top left win (\)
            for (int row = 0; row < this.board.GetLength(0)-3; row++)
            {
                for (int col = 0; col < this.board.GetLength(1) - 3; col++)
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row+1, col + 1], this.board[row+2, col + 2], this.board[row+3, col + 3]))
                        return player2Check;
                }
            }

            //top right win (/)
            for (int row = 0; row < this.board.GetLength(0) - 3; row++)
            {
                for (int col = 3; col < this.board.GetLength(1) ; col++)
                {
                    if (this.AllNumEQ(player2Check, this.board[row, col], this.board[row + 1, col - 1], this.board[row + 2, col - 2], this.board[row + 3, col - 3]))
                        return player2Check;
                }
            }


            return -1;
        }

    }
}
