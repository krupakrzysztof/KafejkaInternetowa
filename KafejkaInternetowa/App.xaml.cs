using KafejkaInternetowa.Utils;
using System.Windows;

namespace KafejkaInternetowa
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Tools.CheckLicence();
            base.OnStartup(e);
        }
    }
}
