using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;

namespace FourRowClient
{
    /// <summary>
    /// Interaction logic for WaitingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : Window
    {

        //Data members
        DispatcherTimer timer = new DispatcherTimer();

        public FourRowServiceClient Client { get; internal set; }
        public string Username { get; internal set; }

        public WaitingWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += delegate { UpdateConnectedPlayers(); };
            timer.Start();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                Client.clientDisconnected(Username);
            }
            catch (Exception)
            {
            }
            Environment.Exit(Environment.ExitCode);
         
        }

        private void UpdateConnectedPlayers()
        {
            try
            {
               List<string> clients = Client.getClientsThatNotPlayNow().ToList<string>();
                clients.Remove(Username);
                if (clients.Count == 0)
                    lbUsers.ItemsSource = null;
                else
                    lbUsers.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                timer.Stop();
            }
        }

        private void PingServer()
        {
            TimeSpan ts = Client.Endpoint.Binding.ReceiveTimeout;
            Client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 10);
            Client.ping();
            Client.Endpoint.Binding.ReceiveTimeout = ts;
        }

        private void AllRegisterdButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllHistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllLiveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TowPlayersGamedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayersGamesSumButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Top10Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
