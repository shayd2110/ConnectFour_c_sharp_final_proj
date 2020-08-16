using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


/*FourRowClient namespace*/
namespace FourRowClient
{
    /*MainWindow class*/
    /// <summary>
    /// login/register window
    /// </summary>
    public partial class Login : Window
    {
        DoubleAnimation da = new DoubleAnimation();
        Utils utils;
        /*constructor*/
        public Login()
        {
            InitializeComponent();
            utils = new Utils();



        }/*end of constructor*/

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            ClientCallback callback = new ClientCallback();
            FourRowServiceClient client = new FourRowServiceClient(new InstanceContext(callback));
            if (!utils.AllTextboxesFilled(mainGrid))
            {
                MessageBox.Show("Please insert User name and Password");
                return;
            }
            string userName = tbUsername.Text.Trim();
            string pass = tbPasswrd.Password.Trim();

            try
            {
                client.clientConnected(userName, utils.HashValue(pass).ToString());
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FaultException<UserDoesntExistsFault> ex)
            {
               MessageBox.Show(ex.Detail.Details);
                tbPasswrd.Clear();
                tbUsername.Clear();
                return;
            }
            catch (FaultException<UserAlreadyConnectedFault> ex)
            {
                MessageBox.Show(ex.Detail.Details);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            WaitingWindow waitingWindow = null;
            try
            {
                WaitingWindow ww = new WaitingWindow();
                ww.Username = userName;
                ww.Client = client;
                ww.Callback = callback;
                ww.Title = "Wellcome " + userName;
                waitingWindow = ww;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                client.clientDisconnected(userName);
            }
            this.Close();
            waitingWindow.Show();

        }
        

        //private void Button_Register_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!AllTextboxesFilled())
        //    {
        //        MessageBox.Show("Please insert User name and Password");
        //        return;
        //    }
        //    ClientCallback callback = new ClientCallback();
        // FourRowServiceClient client = new FourRowServiceClient(new InstanceContext(callback));
        //    string userName = tbUsername.Text.Trim();
        //    string pass = tbPasswrd.Password.Trim();

        //    ////Hashing thing
        //    //ASCIIEncoding ascii = new ASCIIEncoding( );
        //    //var data = Encoding.ASCII.GetBytes(pass);
        //    //var sha1 = new SHA1CryptoServiceProvider();
        //    //var sha1data = sha1.ComputeHash(data);
        //    //string hashedPassword = ascii.GetString(sha1data);

        //    try
        //    {
               
        //        client.clientRegisterd(userName, HashValue(pass).ToString());
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }

        //    tbPasswrd.Clear();
        //    tbUsername.Clear();

        //    da.From = 10;
        //    da.To = 14;
        //    da.AutoReverse = true;
        //    da.RepeatBehavior = new RepeatBehavior(3);
        //    da.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        //    tbRegistred.Visibility = Visibility.Visible;
        //    tbRegistred.BeginAnimation(TextBlock.FontSizeProperty, da);

        //}

        

        private void ClearUsers_Click(object sender, RoutedEventArgs e)
        {
            ClientCallback callback = new ClientCallback();
            FourRowServiceClient client = new FourRowServiceClient(new InstanceContext(callback));
            try
            {
                client.clearUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            Register R = new Register();
            R.Show();
        }

        private void disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            ClientCallback callback = new ClientCallback();
            FourRowServiceClient Client = new FourRowServiceClient(new InstanceContext(callback));
            if (string.IsNullOrEmpty(tbUsername.Text))
            {
                MessageBox.Show("insert valid username to disconnet THIS BUTTON IS FOR DEBUG USAGE ONLY!!");
            }
            else
            {
                string userName = tbUsername.Text.Trim();
                string password = tbPasswrd.Password.Trim();
                try
                {
                    Client.clientDisconnected(userName);
                }
                catch (DbException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                catch (FaultException<UserDoesntExistsFault> ex)
                {
                    MessageBox.Show(ex.Detail.Details);
                    tbPasswrd.Clear();
                    tbUsername.Clear();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                MessageBox.Show(userName + " disconnected successfully");
            }
        }
    }/*end of -MainWindow- class*/

}/*end of -FourRowClient- namespace*/
