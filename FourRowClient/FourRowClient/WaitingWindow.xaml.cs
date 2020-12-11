using System;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FourRowClient.FourRowServiceReference;

namespace FourRowClient
{
    /// <summary>
    ///     Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow 
    {
        private string opponent;
        private readonly DispatcherTimer timer;
        public Utils utils;

        public WaitingWindow()
        {
            utils = new Utils();
            timer = new DispatcherTimer();
            opponent = null;
            InitializeComponent();
            utils.Client = Client;
        }

        public FourRowServiceClient Client { get; internal set; }

        public string Username { get; internal set; }

        public ClientCallback Callback { get; internal set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TbHeader.Text =
                "Connect 4 Game waiting room\nplease choose player to invite, or click one of the search buttons";
            try
            {
                timer.Interval = TimeSpan.FromSeconds(5.0);
                timer.Tick += (EventHandler) ((param1, param2) => UpdateConnectedPlayers());
                timer.Start();
                utils.Client = Client;
                Callback.answer2Challenge += Answer2Challenge;
                Callback.opponentDecline += OpponentDecline;
                Callback.startGameChoosedGuy += StartGame4Me;
                Callback.startGameOpponent += StartGame4Opponent;
                Callback.okGoBackToLife += GoBackToLife;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                utils.PingServer();
                if (opponent == null)
                    Client.ClientDisconnected(Username);
                else
                    Client.ClientDisconnectedBeforeGame(Username, opponent, Username);
            }
            catch (FaultException<Exception> )
            {
            }
            catch (Exception)
            {
                // ignored
            }

            Environment.Exit(Environment.ExitCode);
        }

        private void UpdateConnectedPlayers()
        {
            try
            {
                var userlList = Client.GetClientsThatNotPlayNow().ToList();
                userlList.Remove(Username);
                LbUsers.ItemsSource = userlList.Count == 0 ? null : userlList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                timer.Stop();
            }
        }

        private void AllRegisterdButton_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                utils.PingServer();
                var allUsersInDb = Client.GetAllUsersInDb();
                var res2Show = $"There is {allUsersInDb.Count} registered players in the system \n";
                var i = 0;
                if (allUsersInDb.Count == 0)
                {
                    MessageBox.Show("there is no users now");
                }
                else
                {
                    foreach (var _ in allUsersInDb)
                        if (i == 0)
                            ++i;
                        else
                            res2Show += allUsersInDb[i++] + "\n";
                    MessageBox.Show(res2Show);
                    AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                        TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
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

        private void AllHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                utils.PingServer();
                var gameList = Client.AllTheGamesThatPlayesSoFar();
                var res2Show = "\n";
                var i = 0;
                if (gameList.Count == 0)
                {
                    MessageBox.Show("there is no games that played now");
                }
                else
                {
                    foreach (var _ in gameList)
                        res2Show += gameList[i++] + "\n";
                    MessageBox.Show(res2Show);
                    AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                        TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
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

        private void AllLiveGamesButton_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                utils.PingServer();
                var liveGamesList = Client.AllTheGamesThatPlayesNow();
                var res2Show = "\n";
                var i = 0;
                if (liveGamesList.Count == 0)
                {
                    MessageBox.Show("there is no games that played now");
                }
                else
                {
                    foreach (var _ in liveGamesList)
                        res2Show += liveGamesList[i++] + "\n";
                    MessageBox.Show(res2Show);
                    AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                        TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
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

        private void PlayersGamesSumButton_Click(object sender, RoutedEventArgs e)
        {
            new PlayersInfoWithSortingWindow
            {
                Client = Client
            }.Show();
        }

        private void AllUsersInDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.PingServer();
                var allUsersInDb = Client.GetAllUsersInDb();
                var res2Show = "\n";
                var i = 0;
                if (allUsersInDb.Count == 0)
                {
                    MessageBox.Show("there is no users in DB");
                }
                else
                {
                    foreach (var _ in allUsersInDb)
                        res2Show += allUsersInDb[i++] + "\n";
                    MessageBox.Show(res2Show);
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

        private void Challenge_button_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                timer.Stop();
                opponent = LbUsers.SelectedItem as string;
                ChallengeButton.IsEnabled = false;
                LbUsers.IsEnabled = false;
                MessageBox.Show("request sent to " + opponent + " \n wait for an answer ", "notice",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                utils.PingServer();
                Client.WantToPlayWithClient(Username, opponent);
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                    TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                    TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                    TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        private void lbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            switch (LbUsers.SelectedItems.Count)
            {
                case 1:
                    PlayersGamesSumButton.IsEnabled = true;
                    ChallengeButton.IsEnabled = true;
                    TowPlayersGameButton.IsEnabled = false;
                    break;
                case 2:
                    TowPlayersGameButton.IsEnabled = true;
                    PlayersGamesSumButton.IsEnabled = false;
                    ChallengeButton.IsEnabled = false;
                    break;
                default:
                    PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = false;
                    break;
            }

            try
            {
                if (LbUsers.SelectedItem == null || LbUsers.SelectedItems.Count > 1)
                {
                    TbInfo.Clear();
                }
                else
                {
                    timer.Stop();
                    var selectedItem = LbUsers.SelectedItem as string;
                    utils.PingServer();
                    var clientInfoList = Client.AllTheGamesOfSomeClient(selectedItem);
                    if (clientInfoList.Count == 0)
                        TbInfo.Text = selectedItem + " has not played yet";
                    else
                        foreach (var str in clientInfoList)
                            TbInfo.Text += str + "\n";
                    timer.Start();
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
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        private void TowPlayersGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LbUsers.SelectedItem == null || LbUsers.SelectedItems.Count != 2)
                    return;
                var playersNames = new string[2];
                var index = 0;
                foreach (var selectedItem in LbUsers.SelectedItems)
                {
                    playersNames[index] = selectedItem as string;
                    ++index;
                }

                utils.PingServer();
                var gamesBetweenPlayers = Client.AllTheGamesBetweenTwoClients(playersNames[0], playersNames[1]);
                var res2Show = "\n";
                var num1 = 0;
                if (gamesBetweenPlayers.Count == 0)
                {
                    MessageBox.Show(" there was no games yet between " + playersNames[0] + " and " + playersNames[1]);
                }
                else
                {
                    foreach (var _ in gamesBetweenPlayers)
                        res2Show += gamesBetweenPlayers[num1++] + "\n";
                    MessageBox.Show(res2Show);
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

        private void OpponentDecline()
        {
            MessageBox.Show("hey " + Username + ", " + opponent + " decline to play with you", "notice",
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
            GoBackToLife();
        }

        private void StartGame4Me()
        {
            try
            {
                var gq = new GameWindow
                {
                    Title = Username + " vs " + opponent,
                    UserName = Username,
                    ChoosedName = Username,
                    OpponentName = opponent,
                    Client = Client,
                    Callback = Callback,
                    ww = this
                };
                var gameWindow2 = gq;
                Hide();
                gameWindow2.Show();
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                    TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        private void StartGame4Opponent()
        {
            try
            {
                var gw = new GameWindow
                {
                    Title = Username + " vs " + opponent,
                    UserName = Username,
                    ChoosedName = opponent,
                    OpponentName = Username,
                    Client = Client,
                    Callback = Callback,
                    ww = this
                };
                var gameWindow2 = gw;
                Hide();
                gameWindow2.Show();
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled =
                    TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        private void Answer2Challenge(string theGuy)
        {
            try
            {
                if (opponent != null)
                {
                    utils.PingServer();
                    Client.OpponentDeclineToPlay(theGuy, Username);
                }
                else
                {
                    timer.Stop();
                    opponent = theGuy;
                    ChallengeButton.IsEnabled = false;
                    LbUsers.IsEnabled = false;
                    if (MessageBox.Show(
                        "           hey " + Username + " \n " + theGuy +
                        " want to play with you \n- do you want to play with him ?", "notice", MessageBoxButton.YesNo,
                        MessageBoxImage.Asterisk) == MessageBoxResult.No)
                    {
                        GoBackToLife();
                        utils.PingServer();
                        Client.OpponentDeclineToPlay(theGuy, Username);
                    }
                    else
                    {
                        utils.PingServer();
                        new Thread(() => Client.OpponentAcceptToPlay(theGuy, Username)).Start();
                    }
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
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType());
            }
        }

        public void GoBackToLife()
        {
            ChallengeButton.IsEnabled = true;
            LbUsers.IsEnabled = true;
            opponent = null;
            timer.Start();
        }
    }
}