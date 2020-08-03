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

namespace FourRowClient
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Button clickedButton = (Button)sender;
                int RowLocation = Convert.ToInt32(clickedButton.Name.Substring(6,1));
                int ColLocation = Convert.ToInt32(clickedButton.Name.Substring(8, 1));
            }
            catch (Exception)
            {

                throw;
            }
                
        }
    }
}
