using System;
using System.Data.Common;
using System.ServiceModel;
using System.Windows;
using FourRowClient.FourRowServiceReference;

namespace FourRowClient
{
    /// <summary>
    ///     this class represent window that present users statics with sorting options
    /// </summary>
    public partial class PlayersInfoWithSortingWindow
    {
        private readonly Utils utils = new Utils();

        public PlayersInfoWithSortingWindow()

        {
            InitializeComponent();
            utils.Client = Client;
        }

        //Data members
        public FourRowServiceClient Client { get; internal set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.Client = Client;
                utils.PingServer();
                var allUsersStats = Client.GetAllUsersGamesHistory();

                if (allUsersStats.Count == 0)
                    allUsersStats.Add(
                        "\t\t\tthere is no games that played yet\n\t\t\tthere for no statics for users yet");
                LbUsersStats.ItemsSource = allUsersStats;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType());
            }
        }

        //sorting options

        private void ByGames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.PingServer();
                var allUsersStatsByGames = Client.GetAllUsersGamesHistoryOrderedByGames();

                if (allUsersStatsByGames.Count == 0)
                    allUsersStatsByGames.Add(
                        "\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                LbUsersStats.ItemsSource = allUsersStatsByGames;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType());
            }
        }

        private void ByName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.PingServer();
                var allUsersStatsByName = Client.GetAllUsersGamesHistoryOrderedByName();

                if (allUsersStatsByName.Count == 0)
                    allUsersStatsByName.Add(
                        "\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                LbUsersStats.ItemsSource = allUsersStatsByName;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType());
            }
        }

        private void ByPoints_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.PingServer();
                var allUsersStatsByPoints = Client.GetAllUsersGamesHistoryOrderedByPoints();

                if (allUsersStatsByPoints.Count == 0)
                    allUsersStatsByPoints.Add(
                        "\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                LbUsersStats.ItemsSource = allUsersStatsByPoints;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType());
            }
        }

        private void ByWins_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.PingServer();
                var allUsersStatsByWins = Client.GetAllUsersGamesHistoryOrderedByWins();

                if (allUsersStatsByWins.Count == 0)
                    allUsersStatsByWins.Add(
                        "\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                LbUsersStats.ItemsSource = allUsersStatsByWins;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType());
            }
        }

        private void ByLoses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.PingServer();
                var allUsersStatsByLoses = Client.GetAllUsersGamesHistoryOrderedByLoses();

                if (allUsersStatsByLoses.Count == 0)
                    allUsersStatsByLoses.Add(
                        "\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                LbUsersStats.ItemsSource = allUsersStatsByLoses;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType());
            }
        }
    }
}