using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using KafejkaInternetowa.Klient.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KafejkaInternetowa.Klient.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        public ConfigViewModel()
        {
            LoadData();
            LoadCommands();
        }

        private ObservableCollection<string> availableInterfaces;
        /// <summary>
        /// Lista dostępnych kart sieciowych
        /// </summary>
        public ObservableCollection<string> AvailableInterfaces
        {
            get => availableInterfaces;
            set => SetProperty(ref availableInterfaces, value, nameof(AvailableInterfaces));
        }

        private string selectedInterface;
        /// <summary>
        /// Wybrana karta sieciowa
        /// </summary>
        public string SelectedInterface
        {
            get => selectedInterface;
            set => SetProperty(ref selectedInterface, value, nameof(SelectedInterface));
        }

        private string serverAddress;
        /// <summary>
        /// Adres IP serwera
        /// </summary>
        public string ServerAddress
        {
            get => serverAddress;
            set => SetProperty(ref serverAddress, value, nameof(ServerAddress));
        }

        /// <summary>
        /// Komenda do zapisania informacji w konfiguracji
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Odczytanie podstawowych informacji w modelu
        /// </summary>
        private void LoadData()
        {
            availableInterfaces = ConfigModel.GetAvailableInterfaces().ToObservableCollection();
            SelectedInterface = ConfigModel.GetInterface() ?? AvailableInterfaces.FirstOrDefault();
            ServerAddress = ConfigModel.GetServerAddress() ?? string.Empty;
        }

        /// <summary>
        /// Powiązanie metod z komendami
        /// </summary>
        private void LoadCommands()
        {
            SaveCommand = new DelegateCommand(Save);
        }

        /// <summary>
        /// Zapisanie wpisanych informacji w konfiguracji
        /// </summary>
        private void Save()
        {
            ConfigModel.Save(SelectedInterface, ServerAddress);
            MessageBox.Show("Informacje zapisane poprawnie", "Sukces", MessageBoxButton.OK);
        }
    }
}
