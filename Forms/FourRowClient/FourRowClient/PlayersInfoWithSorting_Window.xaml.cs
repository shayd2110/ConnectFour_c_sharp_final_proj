using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
    /// this class represent window that present users statics with sorting options
    /// </summary>
    public partial class PlayersInfoWithSorting_Window : Window
    {
        //Data members
        public FourRowServiceClient Client { get; internal set; }
        private Utils utils = new Utils();
        public PlayersInfoWithSorting_Window()

        {
            InitializeComponent();
            utils.Client = Client;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.Client = Client;
                utils.pingServer();
                List<string> allUsersStats = Client.getAllUsersGamesHistory();

                if (allUsersStats.Count == 0)
                    allUsersStats.Add("\t\t\tthere is no games that played yet\n\t\t\tthere for no statics for users yet");
                lbUsersStats.ItemsSource = allUsersStats;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString() );
            }
        }

        //sorting options

        private void byGames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.pingServer();
                List<string> allUsersStatsByGames = Client.getAllUsersGamesHistoryOrderedByGames();

                if (allUsersStatsByGames.Count == 0)
                    allUsersStatsByGames.Add("\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                lbUsersStats.ItemsSource = allUsersStatsByGames;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }

        private void byName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.pingServer();
                List<string> allUsersStatsByName = Client.getAllUsersGamesHistoryOrderedByName();

                if (allUsersStatsByName.Count == 0)
                    allUsersStatsByName.Add("\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                lbUsersStats.ItemsSource = allUsersStatsByName;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }

        private void byPoints_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.pingServer();
                List<string> allUsersStatsByPoints = Client.getAllUsersGamesHistoryOrderedByPoints();

                if (allUsersStatsByPoints.Count == 0)
                    allUsersStatsByPoints.Add("\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                lbUsersStats.ItemsSource = allUsersStatsByPoints;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }

        private void byWins_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.pingServer();
                List<string> allUsersStatsByWins = Client.getAllUsersGamesHistoryOrderedByWins();

                if (allUsersStatsByWins.Count == 0)
                    allUsersStatsByWins.Add("\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                lbUsersStats.ItemsSource = allUsersStatsByWins;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString());
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }

        private void byLoses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                utils.pingServer();
                List<string> allUsersStatsByLoses = Client.getAllUsersGamesHistoryOrderedByLoses();

                if (allUsersStatsByLoses.Count == 0)
                    allUsersStatsByLoses.Add("\t\t\tthere is no games that played yet\n\t\t\t there for no statics for users yet");

                lbUsersStats.ItemsSource = allUsersStatsByLoses;
            }
            catch (FaultException<DbException> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString());
            }
            catch (FaultException<Exception> fault)
            {
                MessageBox.Show(fault.Message + "\n" + "Type: " + fault.GetType().ToString() );
            }
            catch (TimeoutException)
            {
                MessageBox.Show("server is disconnected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Type: " + ex.GetType().ToString());
            }
        }
    }
}
