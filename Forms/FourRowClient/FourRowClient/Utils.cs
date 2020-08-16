using FourRowClient.FourRowServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FourRowClient
{
     public class Utils
    {
        public FourRowServiceClient Client { get; internal set; }


        public void pingServer()
        {
            TimeSpan receiveTimeout = this.Client.Endpoint.Binding.ReceiveTimeout;
            this.Client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 10);
            this.Client.ping();
            this.Client.Endpoint.Binding.ReceiveTimeout = receiveTimeout;
        }

        public object HashValue(string password)
        {
            using (SHA256 hashObj = SHA256.Create())
            {
                byte[] hashBytes = hashObj.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool AllTextboxesFilled(Grid mainGrid)
        {
            foreach (var item in mainGrid.Children)
            {
                if (item is TextBox)
                {
                    if (string.IsNullOrEmpty((item as TextBox).Text))
                    {
                        return false;
                    }
                }
                if (item is PasswordBox)
                {
                    if (string.IsNullOrEmpty((item as PasswordBox).Password))
                    {
                        return false;
                    }
                }

            }
            return true;
        }
    }
}
