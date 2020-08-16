using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FourRowClient
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(byte.MaxValue, (byte)0, (byte)0));
        private SolidColorBrush yellow = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, (byte)0));
        private SolidColorBrush currentPlayer = new SolidColorBrush();
        private SolidColorBrush myColor = new SolidColorBrush();
        private SolidColorBrush opponentColor = new SolidColorBrush();
        private Utils utils = new Utils();
        public WaitingWindow ww;
        private int[,] board = new int[7, 6];
        private int[] tempLoc = new int[2];
        private double loc = 0;
        private int colorFlip = 0;
        public bool myTurn = true;
        public bool isOnline = true;

        public FourRowServiceClient Client { get; set; }

        public ClientCallback Callback { get; internal set; }

        public MoveResult CurrentResult { get; set; }

        public string UserName { get; set; }

        public string ChoosedName { get; set; }

        public string OpponentName { get; set; }

        public GameWindow()
        {
            InitializeComponent();
            Array.Clear(board, 0, board.Length);
        }

        private void GameWin_Loaded(object sender, RoutedEventArgs e)
        {
            utils.Client = Client;
            lbXagainstY.Visibility = Visibility.Visible;
            lbXagainstY.Content = !(UserName == ChoosedName) ? " " + UserName + " VS " + ChoosedName + " " : " " + UserName + " VS " + OpponentName + " ";
            lbturn1.Visibility = lbturn2.Visibility = Visibility.Visible;
            lbUserName.Content = "hey " + UserName;
            lbUserName.Visibility = Visibility.Visible;
            try
            {
                if (UserName == ChoosedName)
                {
                    lbturn1.Content = lbturn2.Content = " it's\nyour\nturn!";
                    lbUserName.Foreground = (Brush)Brushes.Red;
                    myColor = red;
                    opponentColor = yellow;
                }
                else
                {
                    lbturn1.Content = lbturn2.Content = " it's\n" + ChoosedName + "\nturn!";
                    lbUserName.Foreground = (Brush)Brushes.Yellow;
                    myColor = yellow;
                    opponentColor = red;
                }
                CurrentResult = MoveResult.Nothing;
                Callback.opponentQuited += new Action(OpponentQuited);
                Callback.updateGame += new Action<int, double, double>(UpdateGame);
                Callback.endGame += new Action<string>(EndGame);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EndGame(string outcome)
        {
            Game_Canvas.IsEnabled = false;
            lbturn1.Content = lbturn2.Content = "Game\n Over!";
            if (outcome == "draw")
            {
                CurrentResult = MoveResult.Draw;
                MessageBox.Show("Draw! nobody won", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CurrentResult = MoveResult.YouLose;
                MessageBox.Show("You lost this game. \nbetter luck next time", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void UpdateGame(int row, double X, double Y)
        {
            try
            {
                Client.setMyPos(ChoosedName, OpponentName, UserName, row);
                Ball ball = new Ball(new Point(X, Y), 70.0, 70.0, 20.0, 20.0);
                ball.El.Fill = (Brush)opponentColor;
                Canvas.SetTop(ball.El, 18.0);
                Canvas.SetLeft(ball.El, ball.X);
                Game_Canvas.Children.Add(ball.El);
                loc = Client.getMyPos(ChoosedName, OpponentName, UserName);
                ThreadPool.QueueUserWorkItem(new WaitCallback(AnimateBall), ball);
                Thread.Sleep(100);
                lbturn2.Content = lbturn1.Content = "Your\n Turn!";
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void OpponentQuited()
        {
            Game_Canvas.IsEnabled = false;
            lbturn1.Content = lbturn2.Content = "Game\n Over!";
            CurrentResult = MoveResult.YouWon;
            MessageBox.Show("Your Opponent" + OpponentName + " quit\n You won this game", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private int GetLocation(Ball b, bool wantJustCol)
        {
            int row = -1;
            double x = b.X;
            int col = x < 60.0 ? 1 : (x < 150.0 ? 2 
                                    : (x < 245.0 ? 3 
                                    : (x < 340.0 ? 4 
                                    : (x < 435.0 ? 5 
                                    : (x < 520.0 ? 6 
                                    : 7)))));
            if (wantJustCol)
                return col;
            for (int i = 5; i >= 0; --i)
            {
                if (board[col, i] == 0)
                {
                    board[col, i] = b.El.Fill == yellow ? 1 : 2;
                    row = i;
                    break;
                }
                if (row == -1)
                {
                    tempLoc[0] = col;
                    tempLoc[1] = row;
                }
            }
            switch (row)
            {
                case 0:
                    row = 4;
                    break;
                case 1:
                    row = 58;
                    break;
                case 2:
                    row = 110;
                    break;
                case 3:
                    row = 165;
                    break;
                case 4:
                    row = 220;
                    break;
                case 5:
                    row = 272;
                    break;
            }
            return row;
        }

        private bool Tie()
        {
            int j = 0;
            for (int i = 0; i < 7; ++i)
            {
                if (board[i, j] == 0)
                    return false;
            }
            return true;
        }

        private void Game_Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point p = e.GetPosition((IInputElement)Game_Canvas);
                Ball b = new Ball(p, 70.0, 70.0, 20.0, 20.0);
                b.El.Fill = (Brush)myColor;
                int location = GetLocation(b, true);
                utils.pingServer();
                Tuple<MoveResult, int> tuple = Client.reportMove(ChoosedName, OpponentName, UserName, location, p.X, p.Y);
                int pos = tuple.Item2;
                MoveResult moveResult = tuple.Item1;
                switch (moveResult)
                {
                    case MoveResult.NotYourTurn:
                        MessageBox.Show("It's not your turn, please wait for " + OpponentName + " to play", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        break;
                    case MoveResult.IllegalMove:
                        MessageBox.Show("Illegal move!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        break;
                    default:
                        CurrentResult = moveResult;
                        Client.setMyPos(ChoosedName, OpponentName, UserName, pos);
                        Canvas.SetTop(b.El, 18.0);
                        Canvas.SetLeft(b.El, b.X);
                        Game_Canvas.Children.Add(b.El);
                        Client.getMyPos(ChoosedName, OpponentName, UserName);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AnimateBall), b);
                        Thread.Sleep(100);
                        if (CurrentResult == MoveResult.GameOn)
                        {
                            lbturn1.Content = lbturn2.Content = UserName == ChoosedName ? ("its\n" + OpponentName + "\nturn!") : ("its\n" + ChoosedName + "\nturn!");
                            break;
                        }
                        Game_Canvas.IsEnabled = false;
                        lbturn2.Content = lbturn1.Content = "Game\nOver!";
                        if (CurrentResult == MoveResult.YouWon)
                        {
                            MessageBox.Show("   " + UserName + "\n   you won   ", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("it's a draw", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                        Client.killTheGame(ChoosedName, OpponentName);
                        break;
                }
            }
            catch (FaultException<DbException> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void AnimateBall(object state)
        {
            try
            {
                Ball b = state as Ball;
                b.Y = 14.0;
                loc = Client.getMyPos(ChoosedName, OpponentName, UserName);
                while (b.Y != loc)
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        double x = b.X;
                        if (b.X < 70.0)
                        {
                            b.X = 14.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        else if (b.X < 160.0)
                        {
                            b.X = 113.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        else if (b.X < 250.0)
                        {
                            b.X = 205.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        else if (b.X < 340.0)
                        {
                            b.X = 297.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        else if (b.X < 430.0)
                        {
                            b.X = 391.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        else if (b.X < 520.0)
                        {
                            b.X = 487.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        else
                        {
                            b.X = 579.0;
                            b.Y += b.YMove;
                            if (b.Y + b.YMove > loc)
                                b.Y = loc;
                        }
                        Canvas.SetTop(b.El, b.Y);
                        Canvas.SetLeft(b.El, b.X);
                    }));
                    Thread.Sleep(15);
                }
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

       


        private void GameWin_Closing(object sender, EventArgs e)
        {
            try
            {
                utils.pingServer();
                if (CurrentResult == MoveResult.Nothing || CurrentResult == MoveResult.GameOn)
                {
                    new Thread((ThreadStart)(() => Client.clientDisconnectedThrowGame(ChoosedName, OpponentName, UserName))).Start();
                    ww.Show();
                    ww.GoBackToLife();
                }
                else
                {
                    new Thread((ThreadStart)(() => Client.getMeBackToWaitingList(UserName))).Start();
                    ww.Show();
                    ww.GoBackToLife();
                }
            }
            catch (TimeoutException ex)
            {
            }
            catch (Exception ex)
            {
            }
        }
    }
}
