using System;
using System.Data.Common;
using System.ServiceModel;
using System.Windows;
using FourRowClient.FourRowServiceReference;

namespace FourRowClient
{
    /// <summary>
    ///     Interaction logic for Register.xaml
    /// </summary>
    public partial class Register
    {
        private readonly Utils utils;

        public Register()
        {
            InitializeComponent();
            utils = new Utils();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!utils.AllTextboxesFilled(MainGrid))
            {
                MessageBox.Show("Please insert User name and Password");
                return;
            }

            var callback = new ClientCallback();
            var client = new FourRowServiceClient(new InstanceContext(callback));
            var userName = TbUsername.Text.Trim();
            var pass = TbPasswrd.Password.Trim();

            try
            {
                client.ClientRegistered(userName, utils.HashValue(pass).ToString());
            }
            catch (DbException dex)
            {
                MessageBox.Show(dex.Message);
            }
            catch (FaultException<UserExistsFault> fault)
            {
                MessageBox.Show(fault.Detail.Details);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Close();
        }
    }
}