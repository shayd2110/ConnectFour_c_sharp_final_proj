using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;
using WcfFourRowService;

/*FourRowHost namespace*/
namespace FourRowHost
{
    /*MainWindow class*/
    public partial class MainWindow : Window
    {
        /*constructor*/
        public MainWindow()
        {
            InitializeComponent();

        }/*end of constructor*/

        /*ServiceHost object*/
        ServiceHost host;

        /*Window_Loaded method*/
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            host = new ServiceHost(typeof(FourRowService));
            host.Description.Behaviors.Add(
                new ServiceMetadataBehavior { HttpGetEnabled = true });
            try
            {
                host.Open();
                lb1.Content = "Service is running";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }/*end of -Window_Loaded- method*/

    }/*end of -MainWindow- class*/

}/*end of -FourRowHost- namespace*/
