using System;
using System.Data.Common;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FourRowClient.FourRowServiceReference;

namespace FourRowClient
{
    /// <summary>
    ///     Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private readonly int[,] board = new int[7, 6];
        private int colorFlip = 0;
        // ReSharper disable once UnusedMember.Local
        private SolidColorBrush currentPlayer = new SolidColorBrush();
        public bool isOnline = true;
        private double loc;
        private SolidColorBrush myColor = new SolidColorBrush();
        public bool myTurn = true;
        private SolidColorBrush opponentColor = new SolidColorBrush();
        private readonly SolidColorBrush red = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0));
        private readonly int[] tempLoc = new int[2];
        private readonly Utils utils = new Utils();
        public WaitingWindow ww;
        private readonly SolidColorBrush yellow = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, 0));

        public GameWindow()
        {
            InitializeComponent();
            Array.Clear(board, 0, board.Length);
        }

        public FourRowServiceClient Client { get; set; }

        public ClientCallback Callback { get; internal set; }

        public MoveResult CurrentResult { get; set; }

        public string UserName { get; set; }

        public string ChoosedName { get; set; }

        public string OpponentName { get; set; }

        private void GameWin_Loaded(object sender, RoutedEventArgs e)
        {
            utils.Client = Client;
            LbXagainstY.Visibility = Visibility.Visible;
            LbXagainstY.Content = !(UserName == ChoosedName)
                ? " " + UserName + " VS " + ChoosedName + " "
                : " " + UserName + " VS " + OpponentName + " ";
            Lbturn1.Visibility = Lbturn2.Visibility = Visibility.Visible;
            LbUserName.Content = "hey " + UserName;
            LbUserName.Visibility = Visibility.Visible;
            try
            {
                if (UserName == ChoosedName)
                {
                    Lbturn1.Content = Lbturn2.Content = " it's your turn!";
                    LbUserName.Foreground = Brushes.Red;
                    myColor = red;
                    opponentColor = yellow;
                }
                else
                {
                    Lbturn1.Content = Lbturn2.Content = " it's " + ChoosedName + " turn!";
                    LbUserName.Foreground = Brushes.Yellow;
                    myColor = yellow;
                    opponentColor = red;
                }

                CurrentResult = MoveResult.Nothing;
                Callback.opponentQuitted += OpponentQuited;
                Callback.updateGame += UpdateGame;
                Callback.endGame += EndGame;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EndGame(string outcome)
        {
            GameCanvas.IsEnabled = false;
            Lbturn1.Content = Lbturn2.Content = "Game  Over!";
            if (outcome == "draw")
            {
                CurrentResult = MoveResult.Draw;
                MessageBox.Show("Draw! nobody won", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CurrentResult = MoveResult.YouLose;
                MessageBox.Show("You lost this game. \nbetter luck next time", "notice", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
        }

        private void UpdateGame(int row, double x, double y)
        {
            try
            {
                Client.SetMyPos(ChoosedName, OpponentName, UserName, row);
                var ball = new Ball(new Point(x, y), 70.0, 70.0, 20.0, 20.0);
                ball.El.Fill = opponentColor;
                Canvas.SetTop(ball.El, 18.0);
                Canvas.SetLeft(ball.El, ball.X);
                GameCanvas.Children.Add(ball.El);
                loc = Client.GetMyPos(ChoosedName, OpponentName, UserName);
                ThreadPool.QueueUserWorkItem(AnimateBall, ball);
                Thread.Sleep(100);
                Lbturn2.Content = Lbturn1.Content = "Your  Turn!";
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        private void OpponentQuited()
        {
            GameCanvas.IsEnabled = false;
            Lbturn1.Content = Lbturn2.Content = "Game  Over!";
            CurrentResult = MoveResult.YouWon;
            MessageBox.Show("Your Opponent" + OpponentName + " quit\n You won this game", "notice", MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
        }

        private int GetLocation(Ball b, bool wantJustCol)
        {
            var row = -1;
            var x = b.X;
            var col = x < 60.0 ? 1
                : x < 150.0 ? 2
                : x < 245.0 ? 3
                : x < 340.0 ? 4
                : x < 435.0 ? 5
                : x < 520.0 ? 6
                : 7;
            if (wantJustCol)
                return col;
            for (var i = 5; i >= 0; --i)
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
            var j = 0;
            for (var i = 0; i < 7; ++i)
                if (board[i, j] == 0)
                    return false;
            return true;
        }

        private void Game_Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var p = e.GetPosition(GameCanvas);
                var b = new Ball(p, 70.0, 70.0, 20.0, 20.0);
                b.El.Fill = myColor;
                var location = GetLocation(b, true);
                utils.PingServer();
                var tuple = Client.ReportMove(ChoosedName, OpponentName, UserName, location, p.X, p.Y);
                var pos = tuple.Item2;
                var moveResult = tuple.Item1;
                switch (moveResult)
                {
                    case MoveResult.NotYourTurn:
                        MessageBox.Show("It's not your turn, please wait for " + OpponentName + " to play", "notice",
                            MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        break;
                    case MoveResult.IllegalMove:
                        MessageBox.Show("Illegal move!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        break;
                    default:
                        CurrentResult = moveResult;
                        Client.SetMyPos(ChoosedName, OpponentName, UserName, pos);
                        Canvas.SetTop(b.El, 18.0);
                        Canvas.SetLeft(b.El, b.X);
                        GameCanvas.Children.Add(b.El);
                        Client.GetMyPos(ChoosedName, OpponentName, UserName);
                        ThreadPool.QueueUserWorkItem(AnimateBall, b);
                        Thread.Sleep(100);
                        if (CurrentResult == MoveResult.GameOn)
                        {
                            Lbturn1.Content = Lbturn2.Content = UserName == ChoosedName
                                ? "its " + OpponentName + " turn!"
                                : "its " + ChoosedName + " turn!";
                            break;
                        }

                        GameCanvas.IsEnabled = false;
                        Lbturn2.Content = Lbturn1.Content = "Game Over!";
                        if (CurrentResult == MoveResult.YouWon)
                            MessageBox.Show("   " + UserName + "\n   you won   ", "notice", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                        else
                            MessageBox.Show("it's a draw", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        Client.KillTheGame(ChoosedName, OpponentName);
                        break;
                }
            }
            catch (FaultException<DbException> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        private void AnimateBall(object state)
        {
            try
            {
                var b = state as Ball;
                if (b != null)
                {
                    b.Y = 14.0;
                    loc = Client.GetMyPos(ChoosedName, OpponentName, UserName);
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    while (b.Y != loc)
                    {
                        Dispatcher.Invoke(() =>
                        {
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
                        });
                        Thread.Sleep(15);
                    }
                }
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }


        private void GameWin_Closing(object sender, EventArgs e)
        {
            try
            {
                utils.PingServer();
                if (CurrentResult == MoveResult.Nothing || CurrentResult == MoveResult.GameOn)
                {
                    new Thread(() => Client.ClientDisconnectedThrowGame(ChoosedName, OpponentName, UserName)).Start();
                    ww.Show();
                    ww.GoBackToLife();
                }
                else
                {
                    new Thread(() => Client.GetMeBackToWaitingList(UserName)).Start();
                    ww.Show();
                    ww.GoBackToLife();
                }
            }
            catch (TimeoutException )
            {
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}