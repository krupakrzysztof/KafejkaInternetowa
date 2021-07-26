using DevExpress.Mvvm;
using KafejkaInternetowa.Klient.Messages;
using KafejkaInternetowa.Klient.Models;
using KafejkaInternetowa.Klient.Views;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace KafejkaInternetowa.Klient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            LoadCommands();
            LoadData();
        }

        /// <summary>
        /// Wątek odpowiedzialny za sprawdzania wykorzystanego pakietu
        /// </summary>
        private Thread CheckUsedPackageThread { get; set; }

        /// <summary>
        /// Wątek odpowiedzialny za nasłuchiwanie na wiadomości TCP
        /// </summary>
        private Thread ListenerThread { get; set; }

        /// <summary>
        /// Komenda do zamknięcia aplikacji
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// Komenda sprawdzająca adres IP na wybranej karcie sieciowej
        /// </summary>
        public ICommand RefreshIpCommand { get; private set; }

        private double usedPackage;
        /// <summary>
        /// Ilość wykorzystanego pakietu
        /// </summary>
        public double UsedPackage
        {
            get => usedPackage;
            set => SetProperty(ref usedPackage, value, nameof(DisplayPackageValue));
        }

        private double availablePackage;
        /// <summary>
        /// Ilość dostępnego pakietu
        /// </summary>
        public double AvailablePackage
        {
            get => availablePackage;
            set => SetProperty(ref availablePackage, value, nameof(DisplayPackageValue));
        }

        /// <summary>
        /// Ilość wysłanych bajtów w momencie uruchomienia sprawdzania ruchu
        /// </summary>
        private long StartupSentBytes { get; set; }
        
        /// <summary>
        /// Ilość odebranych bajtów w momecnie uruchomienia sprawdzania ruchu
        /// </summary>
        private long StartUpReceivedBytes { get; set; }

        /// <summary>
        /// Tekst wyświetlany w aplikacji
        /// </summary>
        public string DisplayPackageValue => $"{UsedPackage}/{AvailablePackage} GB";

        /// <summary>
        /// Widok zablokowanego stanowiska
        /// </summary>
        private LockedView LockedView { get; set; }

        /// <summary>
        /// Nasłuchiwany interfesj sieciowy
        /// </summary>
        private volatile NetworkInterface networkInterface;

        /// <summary>
        /// Odczytanie podstawowych informacji w modelu
        /// </summary>
        private void LoadData()
        {
            Messenger.Default.Register<AvailablePackageMessage>(this, OnPackageSizeReceived);
            Messenger.Default.Register<LockMessage>(this, OnLockMessageReceived);

            CheckUsedPackageThread = new Thread(new ThreadStart(CheckUsedPackage));
            CheckUsedPackageThread.Start();
            ListenerThread = new Thread(new ThreadStart(MainModel.StartListener));
            ListenerThread.Start();

            ShowLockedView();
        }

        /// <summary>
        /// Powiązanie metod z komendami
        /// </summary>
        private void LoadCommands()
        {
            ExitCommand = new DelegateCommand(ExitApp);
            RefreshIpCommand = new DelegateCommand<NetworkInterface>(CheckIpAddress);
        }

        /// <summary>
        /// <para>
        /// Reakcja na wiadomość typu <see cref="AvailablePackage" />
        /// </para>
        /// <para>
        /// Zwiększenie ilości dostępnego pakietu
        /// </para>
        /// </summary>
        /// <param name="message">Wiadomość zawierająca ilość dostepnego pakietu</param>
        private void OnPackageSizeReceived(AvailablePackageMessage message)
        {
            if (UsedPackage >= AvailablePackage)
            {
                AvailablePackage = message.PackageSize;
                SetStartupBytes(networkInterface);
                UsedPackage = 0;
            }
            else
            {
                AvailablePackage += message.PackageSize;
            }
            MainModel.SendNetworkInfo(UsedPackage == 0 ? 0.0001 : UsedPackage, AvailablePackage);
            HideLockedView();
        }

        /// <summary>
        /// <para>
        /// Reakcja na wiadomość typu <see cref="LockMessage" />
        /// </para>
        /// <para>
        /// Wyświetlenie ekranu blokady
        /// </para>
        /// </summary>
        /// <param name="message">Wiadomość informacująca o konieczności zalobkowania stanowiska</param>
        private void OnLockMessageReceived(LockMessage message)
        {
            AvailablePackage = 0;
            ShowLockedView();
        }

        /// <summary>
        /// Wyświetlenie ekranu blokady
        /// </summary>
        private void ShowLockedView()
        {
            if (LockedView == null)
            {
                LockedView = new LockedView();
            }
            LockedView.Dispatcher.Invoke(() =>
            {
                LockedView.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// Ukrycie ekranu blokady
        /// </summary>
        private void HideLockedView()
        {
            LockedView.Dispatcher.Invoke(() =>
            {
                LockedView.Visibility = Visibility.Hidden;
            });
        }

        /// <summary>
        /// Zakończenie działania aplikacji
        /// </summary>
        private void ExitApp()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Sprawdzanie ilości wykonanego ruchu sieciowego na nasłuchiwanej karcie
        /// </summary>
        private void CheckUsedPackage()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Sieć jest niedostępna", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string interfaceName = ConfigModel.GetInterface();
            networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x => x.Name == interfaceName);
            if (networkInterface == null)
            {
                MessageBox.Show($"Nie odnaleziono karty sieciowej o nazwie: {interfaceName}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CheckIpAddress(networkInterface);
            SetStartupBytes(networkInterface);

            int counter = 0;
            while (true)
            {
                counter++;
                IPv4InterfaceStatistics interfaceStatistics = networkInterface.GetIPv4Statistics();
                UsedPackage = Math.Round((interfaceStatistics.BytesSent - StartupSentBytes + interfaceStatistics.BytesReceived - StartUpReceivedBytes) / (double)1024 / 1024 / 1024, 3);

                if (counter == 5)
                {
                    MainModel.SendNetworkInfo(UsedPackage, AvailablePackage);
                    counter = 0;
                }

                if (UsedPackage >= AvailablePackage)
                {
                    if (!LockedView.IsVisible)
                    {
                        ShowLockedView();
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Ustawienie startowych wartości informacji o ruchu na karcie
        /// </summary>
        /// <param name="networkInterface"></param>
        private void SetStartupBytes(NetworkInterface networkInterface)
        {
            IPv4InterfaceStatistics interfaceStatistics = networkInterface.GetIPv4Statistics();
            StartUpReceivedBytes = interfaceStatistics.BytesReceived;
            StartupSentBytes = interfaceStatistics.BytesSent;
        }

        /// <summary>
        /// Sprawdzenie adresu IP na karcie sieciowej
        /// </summary>
        /// <param name="networkInterface"></param>
        private void CheckIpAddress(NetworkInterface networkInterface)
        {
            MainModel.IpAddress = networkInterface.GetIPProperties().UnicastAddresses.FirstOrDefault().Address.ToString();
        }
    }
}
