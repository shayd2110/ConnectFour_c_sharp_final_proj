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
using System.Windows.Threading;

namespace FourRowClient
{
    /// <summary>
    /// Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        public Utils utils = new Utils();
        private string Opponent = (string)null;

        public FourRowServiceClient Client { get; internal set; }

        public string Username { get; internal set; }

        public ClientCallback Callback { get; internal set; }

        public WaitingWindow()
        {
            InitializeComponent();
            utils.Client = Client;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbHeader.Text = "Connect 4 Game waiting room\nplease choose player to invite, or click one of the search buttons";
            try
            {
                timer.Interval = TimeSpan.FromSeconds(5.0);
                timer.Tick += (EventHandler)((_param1, _param2) => UpdateConnectedPlayers());
                timer.Start();
                utils.Client = Client;
                Callback.answer2Challenge += new Action<string>(answer2Challenge);
                Callback.opponentDecline += new Action(OpponentDecline);
                Callback.startGameChoosedGuy += new Action(StartGame4Me);
                Callback.startGameOpponent += new Action(StartGame4Opponent);
                Callback.okGoBackToLife += new Action(GoBackToLife);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                utils.pingServer();
                if (Opponent == null)
                    Client.clientDisconnected(Username);
                else
                    Client.clientDisconnectedBeforeGame(Username, Opponent, Username);
            }
            catch (FaultException<Exception> ex)
            {
            }
            catch (Exception ex)
            {
            }
            Environment.Exit(Environment.ExitCode);
        }

        private void UpdateConnectedPlayers()
        {
            try
            {
                List<string> userlList = Client.getClientsThatNotPlayNow().ToList();
                userlList.Remove(Username);
                lbUsers.ItemsSource = userlList.Count == 0 ? null : userlList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                timer.Stop();
            }
        }

        private void AllRegisterdButton_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                utils.pingServer();
                List<string> allUsersInDb = Client.GetAllUsersInDB();
                string res2Show = $"There is {allUsersInDb.Count} registerd players in the system \n";
                int i = 0;
                if (allUsersInDb.Count == 0)
                {
                    MessageBox.Show("there is no users now");
                }
                else
                {
                    foreach (string str in allUsersInDb)
                    {
                        if (i == 0)
                            ++i;
                        else
                            res2Show += allUsersInDb[i++] + "\n";
                    }
                    MessageBox.Show(res2Show);
                    AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
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

        private void AllHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                utils.pingServer();
                List<string> gameList = Client.allTheGamesThatPlayesSoFar();
                string res2Show = "\n";
                int i = 0;
                if (gameList.Count == 0)
                {
                    MessageBox.Show("there is no games that played now");
                }
                else
                {
                    foreach (string str in gameList)
                        res2Show += gameList[i++] + "\n";
                    MessageBox.Show(res2Show);
                    AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
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

        private void AllLiveButton_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                utils.pingServer();
                List<string> liveGamesList = Client.allTheGamesThatPlayesNow();
                string res2Show = "\n";
                int i = 0;
                if (liveGamesList.Count == 0)
                {
                    MessageBox.Show("there is no games that played now");
                }
                else
                {
                    foreach (string str in liveGamesList)
                        res2Show +=  liveGamesList[i++] + "\n";
                    MessageBox.Show(res2Show);
                    AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
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

        private void PlayersGamesSumButton_Click(object sender, RoutedEventArgs e) => new PlayersInfoWithSorting_Window()
        {
            Client = Client
        }.Show();

        private void AllUsersInDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.pingServer();
                List<string> allUsersInDb = Client.GetAllUsersInDB();
                string res2Show = "\n";
                int i = 0;
                if (allUsersInDb.Count == 0)
                {
                    MessageBox.Show("there is no users in DB");
                }
                else
                {
                    foreach (string str in allUsersInDb)
                        res2Show += allUsersInDb[i++] + "\n";
                    MessageBox.Show(res2Show);
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

        private void Challenge_button_Click(object sender, RoutedEventArgs e)
        {
            AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = false;
            try
            {
                timer.Stop();
                Opponent = lbUsers.SelectedItem as string;
                Challenge_button.IsEnabled = false;
                lbUsers.IsEnabled = false;
                MessageBox.Show("request sent to " + Opponent + " \n wait for an answer ", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                utils.pingServer();
                Client.wantToPlayWithClient(Username, Opponent);
            }
            catch (FaultException<Exception> ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show("server is disconnected...\n" + ex.Message);
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
        }

        private void lbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (lbUsers.SelectedItems.Count)
            {
                case 1:
                    PlayersGamesSumButton.IsEnabled = true;
                    Challenge_button.IsEnabled = true;
                    TowPlayersGameButton.IsEnabled = false;
                    break;
                case 2:
                    TowPlayersGameButton.IsEnabled = true;
                    PlayersGamesSumButton.IsEnabled = false;
                    Challenge_button.IsEnabled = false;
                    break;
                default:
                    PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = false;
                    break;
            }
            try
            {
                if (lbUsers.SelectedItem == null || lbUsers.SelectedItems.Count > 1)
                {
                    tbInfo.Clear();
                }
                else
                {
                    timer.Stop();
                    string selectedItem = lbUsers.SelectedItem as string;
                    utils.pingServer();
                    List<string> clientInfoList = Client.allTheGamesOfSomeClient(selectedItem);
                    if (clientInfoList.Count == 0)
                    {
                        tbInfo.Text = selectedItem + " has not played yet";
                    }
                    else
                    {
                        foreach (string str in clientInfoList)
                        {
                           
                            tbInfo.Text += str + "\n";
                        }
                    }
                    timer.Start();
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
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void TowPlayersGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbUsers.SelectedItem == null || lbUsers.SelectedItems.Count != 2)
                    return;
                string[] playersNames = new string[2];
                int index = 0;
                foreach (var selectedItem in lbUsers.SelectedItems)
                {
                    playersNames[index] = selectedItem as string;
                    ++index;
                }
                utils.pingServer();
                List<string> gamesBetweenPlayers = Client.allTheGamesBetweenTwoClients(playersNames[0], playersNames[1]);
                string res2Show = "\n";
                int num1 = 0;
                if (gamesBetweenPlayers.Count == 0)
                {
                    MessageBox.Show(" there was no games yet between " + playersNames[0] + " and " + playersNames[1]);
                }
                else
                {
                    foreach (string str in gamesBetweenPlayers)
                        res2Show += gamesBetweenPlayers[num1++] + "\n";
                    MessageBox.Show(res2Show);
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
                MessageBox.Show("server is disconnected...\n" +ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void OpponentDecline()
        {
            MessageBox.Show("hey " + Username + ", " + Opponent + " decline to play with you", "notice", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            GoBackToLife();
        }

        private void StartGame4Me()
        {
            try
            {
                GameWindow gq = new GameWindow();
                gq.Title = Username + " vs " + Opponent;
                gq.UserName = Username;
                gq.ChoosedName = Username;
                gq.OpponentName = Opponent;
                gq.Client = Client;
                gq.Callback = Callback;
                gq.ww = this;
                GameWindow gameWindow2 = gq;
                Hide();
                gameWindow2.Show();
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void StartGame4Opponent()
        {
            try
            {
                GameWindow gw = new GameWindow();
                gw.Title = Username + " vs " + Opponent;
                gw.UserName = Username;
                gw.ChoosedName = Opponent;
                gw.OpponentName = Username;
                gw.Client = Client;
                gw.Callback = Callback;
                gw.ww = this;
                GameWindow gameWindow2 = gw;
                Hide();
                gameWindow2.Show();
                AllRegisterdButton.IsEnabled = AllHistoryButton.IsEnabled = PlayersGamesSumButton.IsEnabled = TowPlayersGameButton.IsEnabled = AllLiveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        private void answer2Challenge(string theGuy)
        {
            try
            {
                if (Opponent != null)
                {
                    utils.pingServer();
                    Client.opponentDeclineToPlay(theGuy, Username);
                }
                else
                {
                    timer.Stop();
                    Opponent = theGuy;
                    Challenge_button.IsEnabled = false;
                    lbUsers.IsEnabled = false;
                    if (MessageBox.Show("           hey " + Username + " \n " + theGuy + " want to play with you \n- do you want to play with him ?", "notice", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.No)
                    {
                        GoBackToLife();
                        utils.pingServer();
                        Client.opponentDeclineToPlay(theGuy, Username);
                    }
                    else
                    {
                        utils.pingServer();
                        new Thread((ThreadStart)(() => Client.opponentAcceptToPlay(theGuy, Username))).Start();
                    }
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
                MessageBox.Show("server is disconnected...\n" +ex.Message);
                timer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nType: " + ex.GetType().ToString());
            }
        }

        public void GoBackToLife()
        {
            Challenge_button.IsEnabled = true;
            lbUsers.IsEnabled = true;
            Opponent = (string)null;
            timer.Start();
        }
    }
}
