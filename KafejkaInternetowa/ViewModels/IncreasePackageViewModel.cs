using DevExpress.Mvvm;
using KafejkaInternetowa.Messages;
using KafejkaInternetowa.Utils;
using KafejkaInternetowa.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace KafejkaInternetowa.ViewModels
{
    public class IncreasePackageViewModel : ViewModelBase
    {
        public IncreasePackageViewModel()
        {
            Messenger.Default.Register<IncreasePackageMessage>(this, OnIncreaseMessageReceived);
            LoadCommands();
            LoadData();
        }

        /// <summary>
        /// Klient, któremu zwiększony zostanie pakiet
        /// </summary>
        private Client Client { get; set; }

        /// <summary>
        /// Reakcja na otrzymanie wiadomości typu <see cref="IncreasePackageMessage" />
        /// </summary>
        /// <param name="message">Wiadomość zawierająca klienta</param>
        private void OnIncreaseMessageReceived(IncreasePackageMessage message)
        {
            Client = message.Client;
        }

        /// <summary>
        /// Komenda potwierdzająca zwiększenie pakietu
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        private ObservableCollection<double> packageSizes;
        /// <summary>
        /// Lista dostępnych wielkości pakietu
        /// </summary>
        public ObservableCollection<double> PackageSizes
        {
            get => packageSizes;
            set => SetProperty(ref packageSizes, value, nameof(PackageSizes));
        }

        private double selectedPackageSize;
        /// <summary>
        /// Aktualnie wybrana wielkość pakietu
        /// </summary>
        public double SelectedPackageSize
        {
            get => selectedPackageSize;
            set => SetProperty(ref selectedPackageSize, value, nameof(SelectedPackageSize));
        }

        private Random Random { get; } = new Random();

        /// <summary>
        /// Powiązanie metod z komendami
        /// </summary>
        private void LoadCommands()
        {
            ConfirmCommand = new DelegateCommand<IncreasePackageView>(Confirm, CanConfirm);
        }

        /// <summary>
        /// Odczytanie podstawowych informacji
        /// </summary>
        private void LoadData()
        {
            PackageSizes = new ObservableCollection<double>();
            for (int i = 1; i < 10; i++)
            {
                PackageSizes.Add(i / 10.0);
            }
        }

        /// <summary>
        /// Zatwierdzenie zwiększenia pakietu
        /// </summary>
        /// <param name="view"></param>
        private void Confirm(IncreasePackageView view)
        {
            try
            {
                XElement xElement = new XElement("Package");
                if (Tools.Licence.IsDemo)
                {
                    xElement.Add(new XElement("PackageSize", SelectedPackageSize + (Random.Next(1, 9) / 10.0)));
                }
                else
                {
                    xElement.Add(new XElement("PackageSize", SelectedPackageSize));
                }
                new TcpClient(Client.AddressIP, 34400).Client.Send(Encoding.UTF8.GetBytes(xElement.ToString()));
                SelectedPackageSize = 0;
                view.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show($"Nie udało się zwiększyć pakietu na stanowisku {Client.MachineName}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanConfirm(IncreasePackageView view)
        {
            return SelectedPackageSize > 0;
        }
    }
}
