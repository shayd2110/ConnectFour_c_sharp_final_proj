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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        Utils utils;
        public Register()
        {
            InitializeComponent();
            utils = new Utils();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!utils.AllTextboxesFilled(mainGrid))
            {
                MessageBox.Show("Please insert User name and Password");
                return;
            }
            ClientCallback callback = new ClientCallback();
            FourRowServiceClient client = new FourRowServiceClient(new InstanceContext(callback));
            string userName = tbUsername.Text.Trim();
            string pass = tbPasswrd.Password.Trim();

            try
            {

                client.clientRegisterd(userName, utils.HashValue(pass).ToString());
            }
            catch (DbException dex)
            {
                throw new FaultException<DbException>(dex);
            }
            catch (FaultException<UserExistsFault> fault)
            {
                MessageBox.Show(fault.Detail.Details);
                return;
            }
            catch (Exception ex)
            {
                throw new FaultException<Exception>(ex);
            }
            this.Close();

        }
    }
}
