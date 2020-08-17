using System;
using System.Data.Common;
using System.ServiceModel;
using System.Windows;
using FourRowClient.FourRowServiceReference;


/*FourRowClient namespace*/
namespace FourRowClient
{
    /*MainWindow class*/
    /// <summary>
    ///     login/register window
    /// </summary>
    public partial class Login 
    {


        private readonly Utils utils;

        /*constructor*/
        public Login()
        {
            InitializeComponent();
            utils = new Utils();
        } /*end of constructor*/

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            var callback = new ClientCallback();
            var client = new FourRowServiceClient(new InstanceContext(callback));
            if (!utils.AllTextboxesFilled(MainGrid))
            {
                MessageBox.Show("Please insert User name and Password");
                return;
            }

            var userName = TbUsername.Text.Trim();
            var pass = TbPasswrd.Password.Trim();

            try
            {
                client.ClientConnected(userName, utils.HashValue(pass).ToString());
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FaultException<UserDoesntExistsFault> ex)
            {
                MessageBox.Show(ex.Detail.Details);
                TbPasswrd.Clear();
                TbUsername.Clear();
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
                var ww = new WaitingWindow
                {
                    Username = userName,
                    Client = client,
                    Callback = callback,
                    Title = "Wellcome " + userName
                };
                waitingWindow = ww;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                client.ClientDisconnected(userName);
            }

            Close();
            waitingWindow?.Show();
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
            var callback = new ClientCallback();
            var client = new FourRowServiceClient(new InstanceContext(callback));
            try
            {
                client.ClearUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            var r = new Register();
            r.Show();
        }

        private void Disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            var callback = new ClientCallback();
            var client = new FourRowServiceClient(new InstanceContext(callback));
            if (string.IsNullOrEmpty(TbUsername.Text))
            {
                MessageBox.Show("insert valid username to disconnet THIS BUTTON IS FOR DEBUG USAGE ONLY!!");
            }
            else
            {
                var userName = TbUsername.Text.Trim();
                try
                {
                    client.ClientDisconnected(userName);
                }
                catch (DbException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (FaultException<UserDoesntExistsFault> ex)
                {
                    MessageBox.Show(ex.Detail.Details);
                    TbPasswrd.Clear();
                    TbUsername.Clear();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                MessageBox.Show(userName + " disconnected successfully");
            }
        }
    } /*end of -MainWindow- class*/
} /*end of -FourRowClient- namespace*/