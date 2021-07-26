using DevExpress.Mvvm;
using KafejkaInternetowa.Messages;
using KafejkaInternetowa.Utils;
using KafejkaInternetowa.Views;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace KafejkaInternetowa.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        public ClientViewModel()
        {
            Messenger.Default.Register<EditClientMessage>(this, OnClientReceived);
            LoadCommands();
        }

        private Client client;
        /// <summary>
        /// Edytowany klient
        /// </summary>
        public Client Client
        {
            get => client;
            set => SetProperty(ref client, value, nameof(Client));
        }

        private bool isNew;
        /// <summary>
        /// Informacja czy klient jest nowym klientem
        /// </summary>
        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value, nameof(IsNew));
        }

        /// <summary>
        /// Komenda do zapisania klienta
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Komenda do usunięcia klienta
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Komenda do powrotu do poprzedniego widoku
        /// </summary>
        public ICommand GoBackCommand { get; private set; }

        /// <summary>
        /// Reakcja na otrzymanie nowej wiadomości typu <see cref="EditClientMessage" />
        /// </summary>
        /// <param name="message">Wiadomość zawierająca klienta</param>
        private void OnClientReceived(EditClientMessage message)
        {
            Client = message.Client;
            IsNew = message.IsNew;
        }

        /// <summary>
        /// Powiązanie metod z komendami
        /// </summary>
        private void LoadCommands()
        {
            SaveCommand = new DelegateCommand<ClientView>(Save, CanSave);
            DeleteCommand = new DelegateCommand<ClientView>(Delete, CanDelete);
            GoBackCommand = new DelegateCommand<ClientView>(GoBack);
        }

        /// <summary>
        /// Zapisanie klienta w konfiguracji
        /// </summary>
        /// <param name="view"></param>
        private void Save(ClientView view)
        {
            // sprawdzenie czy wprowadzony adres IP jest prawidłowy
            Regex regex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            var test = regex.Match(Client.AddressIP);
            if (test.Captures.Count == 1)
            {
                Client.SaveClient(Client);
                view.Close();
            }
            else
            {
                MessageBox.Show("Wprowadź prawidłowy adres IP", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave(ClientView view)
        {
            return Client != null && !string.IsNullOrWhiteSpace(Client.AddressIP);
        }

        /// <summary>
        /// Usunięcie klienta z konfiguracji
        /// </summary>
        /// <param name="view"></param>
        private void Delete(ClientView view)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć klienta?", "Pytanie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Client.DeleteClient(Client);
                view.Close();
            }
        }

        private bool CanDelete(ClientView view)
        {
            return Client != null && !IsNew;
        }

        /// <summary>
        /// Powrót do poprzedniego widoku (zamknięcie obecnego okna)
        /// </summary>
        /// <param name="view"></param>
        private void GoBack(ClientView view)
        {
            view.Close();
        }
    }
}
