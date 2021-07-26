using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using KafejkaInternetowa.Messages;
using KafejkaInternetowa.Models;
using KafejkaInternetowa.Utils;
using KafejkaInternetowa.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KafejkaInternetowa.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            LoadCommands();
            Clients = Client.GetClients().ToObservableCollection();
            if (!Tools.Licence.IsDemo && Clients.Count > Tools.Licence.ClientCount)
            {
                MessageBox.Show("Ilość klientów jest większa niż pozwala licencja. Należy usunąć nadmiarową ilość i uruchomić program ponownie", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ListenerThread = new Thread(new ThreadStart(MainModel.StartListener));
                ListenerThread.Start();
            }
            Messenger.Default.Register<UpdateClientUsedMessage>(this, OnUpdateClientUsedMessageReceived);
        }

        /// <summary>
        /// Wątek, w którym pracuje klient TCP (nasłuchiwanie na wiadomości)
        /// </summary>
        private Thread ListenerThread { get; set; }

        private ObservableCollection<Client> clients;
        /// <summary>
        /// Lista klientów obsługiwanych przez program
        /// </summary>
        public ObservableCollection<Client> Clients
        {
            get => clients;
            set => SetProperty(ref clients, value, nameof(Clients));
        }

        private Client selectedClient;
        /// <summary>
        /// Aktualnie zaznaczony klient
        /// </summary>
        public Client SelectedClient
        {
            get => selectedClient;
            set => SetProperty(ref selectedClient, value, nameof(SelectedClient));
        }

        /// <summary>
        /// Komenda uruchamiająca się po załadowaniu okna
        /// </summary>
        public ICommand OnLoadedCommand { get; private set; }

        /// <summary>
        /// Komenda dodająca nowego klienta
        /// </summary>
        public ICommand NewClientCommand { get; private set; }

        /// <summary>
        /// Komenda edytująca zaznaczonego klienta
        /// </summary>
        public ICommand EditClientCommand { get; private set; }
        
        /// <summary>
        /// Komenda uruchamiająca się podczas zamykania aplikacji
        /// </summary>
        public ICommand OnClosingCommand { get; private set; }

        /// <summary>
        /// Komenda wysyłająca informacje o konieczności zablokowania stanowiska
        /// </summary>
        public ICommand LockCommand { get; private set; }

        /// <summary>
        /// Komenda zwiększająca ilość pakietu dla zaznaczonego klienta
        /// </summary>
        public ICommand AddPackageCommand { get; private set; }

        /// <summary>
        /// Komenda żądająca odświeżenia informacji o wykorzystaniu pakietu do zaznaczonego klienta
        /// </summary>
        public ICommand RefreshCommand { get; private set; }

        /// <summary>
        /// Komenda otwierająca okno licencji
        /// </summary>
        public ICommand LicenceCommand { get; private set; }

        public ICommand AboutCommand { get; private set; }

        /// <summary>
        /// Okno edycji klienta (zainicjowane, aby <see cref="Messenger" /> zarejestrował odbiorcę wiadomości)
        /// </summary>
        private ClientView ClientView { get; set; } = new ClientView();

        /// <summary>
        /// Okno zwiększania pakietu (zainicjowane, aby <see cref="Messenger" /> zarejestrował odbiorcę wiadomości)
        /// </summary>
        private IncreasePackageView IncreasePackageView { get; set; } = new IncreasePackageView();

        private Random Random { get; } = new Random();

        /// <summary>
        /// Tekst wyświetlający numer licencji
        /// </summary>
        public string LicenceLabel => $"Numer: {(Tools.Licence.IsDemo ? "DEMO" : Tools.Licence.Number)}";

        /// <summary>
        /// Powiązanie metod z komendami
        /// </summary>
        private void LoadCommands()
        {
            OnLoadedCommand = new DelegateCommand(OnLoaded);
            NewClientCommand = new DelegateCommand(NewClient, CanNewClient);
            EditClientCommand = new DelegateCommand(EditClient, CanEditClient);
            OnClosingCommand = new DelegateCommand(OnClosing);
            LockCommand = new DelegateCommand(LockMachine, CanEditClient);
            AddPackageCommand = new DelegateCommand(AddPackage, CanEditClient);
            RefreshCommand = new DelegateCommand(RefreshMachineInfo, CanEditClient);
            LicenceCommand = new DelegateCommand(LoadLicence);
            AboutCommand = new DelegateCommand<Window>(About);
        }

        /// <summary>
        /// <para>
        /// Reakcja na otrzymanie wiadomości <see cref="UpdateClientUsedMessage" />
        /// </para>
        /// <para>
        /// Przypisanie wartości wykorzystanego pakietu przez klienta
        /// </para>
        /// </summary>
        /// <param name="message">Wiadomość z informacją o wykorzystaniu pakietu</param>
        private void OnUpdateClientUsedMessageReceived(UpdateClientUsedMessage message)
        {
            Task.Run(() =>
            {
                if (Clients != null)
                {
                    Client client = Clients.FirstOrDefault(x => x.AddressIP == message.IPAddress);
                    if (client != null)
                    {
                        if (Tools.Licence.IsDemo)
                        {
                            double add = Random.Next(1, 9) / 10.0;
                            client.TotalBytes = message.UsedPackage + add;
                            client.AvailablePackage = message.AvailablePackage + add;
                        }
                        else
                        {
                            client.TotalBytes = message.UsedPackage;
                            client.AvailablePackage = message.AvailablePackage;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Metoda uruchamiana po załadowaniu okna
        /// </summary>
        private void OnLoaded()
        {
            // przypisanie okna nadrzędnego do okna zwiększania ilości pakietu
            IncreasePackageView.Owner = Application.Current.MainWindow;
        }

        /// <summary>
        /// Metoda uruchamiająca dodawanie nowego klienta
        /// </summary>
        private void NewClient()
        {
            Messenger.Default.Send(new EditClientMessage(new Client(), true));
            ClientView.ShowDialog();
            Clients = Client.GetClients().ToObservableCollection();
        }

        private bool CanNewClient()
        {
            return Tools.Licence.IsDemo ? true : Clients.Count == Tools.Licence.ClientCount ? false : true;
        }

        /// <summary>
        /// Metoda uruchamiająca edycję zaznaczonego klienta
        /// </summary>
        private void EditClient()
        {
            Messenger.Default.Send(new EditClientMessage(SelectedClient, false));
            ClientView.ShowDialog();
            Clients = Client.GetClients().ToObservableCollection();
        }

        private bool CanEditClient()
        {
            return SelectedClient != null;
        }

        /// <summary>
        /// Metoda uruchamiająca się podczas zamykania aplikacji
        /// </summary>
        private void OnClosing()
        {
            ClientView.CanExit = true;
            ClientView.Close();
            IncreasePackageView.CanExit = true;
            IncreasePackageView.Close();
            Environment.Exit(0);
        }

        /// <summary>
        /// Metoda wysyłająca informacje do zaznaczonego klienta o tym aby się zablokował
        /// </summary>
        private void LockMachine()
        {
            MainModel.SendLockInfo(SelectedClient);
            SelectedClient.AvailablePackage = 0;
            SelectedClient.TotalBytes = 0;
        }

        /// <summary>
        /// Metoda wysyłająca informację o zwiększeniu pakietu
        /// </summary>
        private void AddPackage()
        {
            Messenger.Default.Send(new IncreasePackageMessage(SelectedClient));
            IncreasePackageView.ShowDialog();
        }

        /// <summary>
        /// Metoda żądająca odświeżenia informacji o wykorzystanym pakiecie
        /// </summary>
        private void RefreshMachineInfo()
        {

        }

        /// <summary>
        /// Otwarcie okna do zarządzania licencją
        /// </summary>
        private void LoadLicence()
        {
            new LicenceInfoView().ShowDialog();
            RaisePropertyChanged(nameof(LicenceLabel));
        }

        private void About(Window window)
        {
            new HelpView()
            {
                Owner = window
            }.ShowDialog();
        }
    }
}
