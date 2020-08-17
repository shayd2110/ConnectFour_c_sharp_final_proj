using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using FourRowClient.FourRowServiceReference;

namespace FourRowClient
{
    public class Utils
    {
        public FourRowServiceClient Client { get; internal set; }


        public void PingServer()
        {
            if (Client.Endpoint.Binding == null) return;
            var receiveTimeout = Client.Endpoint.Binding.ReceiveTimeout;
            Client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 0, 10);
            Client.Ping();
            if (Client.Endpoint.Binding != null) 
                Client.Endpoint.Binding.ReceiveTimeout = receiveTimeout;
        }

        public object HashValue(string password)
        {
            using (var hashObj = SHA256.Create())
            {
                var hashBytes = hashObj.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in hashBytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public bool AllTextboxesFilled(Grid mainGrid)
        {
            foreach (var item in mainGrid.Children)
            {
                if (item is TextBox)
                    if (string.IsNullOrEmpty((item as TextBox).Text))
                        return false;
                if (item is PasswordBox)
                    if (string.IsNullOrEmpty((item as PasswordBox).Password))
                        return false;
            }

            return true;
        }
    }
}