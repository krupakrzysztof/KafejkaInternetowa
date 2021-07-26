using DevExpress.Mvvm;
using System;
using System.Windows;
using System.Windows.Input;

namespace KafejkaInternetowa.LicenceGenerator
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            LoadCommands();
            ValidTo = DateTime.Now.AddYears(1);
            ClientCount = 1;
        }

        private string number;
        /// <summary>
        /// Numer generowanej licencji
        /// </summary>
        public string Number
        {
            get => number;
            set => SetProperty(ref number, value, nameof(Number));
        }

        private DateTime validTo;
        /// <summary>
        /// Termin wazności licencji
        /// </summary>
        public DateTime ValidTo
        {
            get => validTo;
            set => SetProperty(ref validTo, value, nameof(ValidTo));
        }

        private int clientCount;
        /// <summary>
        /// Dopuszczalna liczba klientów
        /// </summary>
        public int ClientCount
        {
            get => clientCount;
            set => SetProperty(ref clientCount, value, nameof(ClientCount));
        }

        public ICommand GenerateCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }

        private void LoadCommands()
        {
            GenerateCommand = new DelegateCommand(GenerateLicence, CanGenerateLicence);
            LoadCommand = new DelegateCommand(LoadLicence);
        }

        private void GenerateLicence()
        {
            LicenceManager.GenerateLicence(ValidTo, ClientCount, Number);
            MessageBox.Show("Licencja wygenerowana", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanGenerateLicence()
        {
            return ClientCount > 0 && ValidTo > DateTime.Now && ValidTo != DateTime.MaxValue && ValidTo != DateTime.MinValue;
        }

        private void LoadLicence()
        {

        }
    }
}
