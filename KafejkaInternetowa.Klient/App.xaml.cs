using System;
using System.Linq;
using System.Windows;

namespace KafejkaInternetowa.Klient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // sprawdzenie czy program został uruchomiony z parametrem "config", który uruchomi okno konfiguracji
            string[] args = Environment.GetCommandLineArgs();
            if (!string.IsNullOrWhiteSpace(args.FirstOrDefault(x => x == "config")))
            {
                StartupUri = new Uri("Views/ConfigView.xaml", UriKind.RelativeOrAbsolute);
            }

            base.OnStartup(e);
        }
    }
}
