using DevExpress.Mvvm;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace KafejkaInternetowa.ViewModels
{
    public class HelpViewModel : ViewModelBase
    {
        public HelpViewModel()
        {
            ContactCommand = new DelegateCommand(Contact);
            CloseCommand = new DelegateCommand<Window>(x => x.Close());
        }

        /// <summary>
        /// Komenda uruchamiająca wysyłkę maila
        /// </summary>
        public ICommand ContactCommand { get; private set; }

        /// <summary>
        /// Komenda zamykająca okno
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        /// <summary>
        /// Nazwa aplikacji
        /// </summary>
        public string AssemblyName { get; } = "Kafejka internetowa";

        /// <summary>
        /// Wersja aplikacji
        /// </summary>
        public string Version { get; } = "1.0.0.0";

        /// <summary>
        /// Data kompilacji
        /// </summary>
        public string CompileTime { get; } = "16:22 06.01.2019";

        /// <summary>
        /// Imię autora
        /// </summary>
        public string AuthorName { get; } = "Krzysztof Krupa";

        /// <summary>
        /// Adres kontaktowy do autora
        /// </summary>
        public string AuthorContact { get; } = "master.kosfsd@gmail.com";

        /// <summary>
        /// Uruchomienie wysyłki maila do autora
        /// </summary>
        private void Contact()
        {
            Process.Start($"mailto:{AuthorContact}");
        }
    }
}
