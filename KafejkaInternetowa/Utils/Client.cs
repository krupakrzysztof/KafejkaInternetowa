using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace KafejkaInternetowa.Utils
{
    public class Client : BindableBase
    {
        public Client()
        {
            
        }

        private string addressIP;
        /// <summary>
        /// Adres IP klienta
        /// </summary>
        public string AddressIP
        {
            get => addressIP;
            set => SetProperty(ref addressIP, value, nameof(AddressIP));
        }

        private string machineName;
        /// <summary>
        /// Nazwa stanowiska
        /// </summary>
        public string MachineName
        {
            get => machineName;
            set => SetProperty(ref machineName, value, nameof(MachineName));
        }

        private double totalBytes;
        /// <summary>
        /// Ilość wykorzystanego pakietu
        /// </summary>
        public double TotalBytes
        {
            get => totalBytes;
            set => SetProperty(ref totalBytes, value, nameof(TotalBytes));
        }

        private double availablePackage;
        /// <summary>
        /// Ilość dostępnego pakietu
        /// </summary>
        public double AvailablePackage
        {
            get => availablePackage;
            set => SetProperty(ref availablePackage, value, nameof(AvailablePackage));
        }

        /// <summary>
        /// Nazwa pliku konfiguracyjnego
        /// </summary>
        private static string ConfigFile { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KafejkaInternetowa", "clients.xml");

        /// <summary>
        /// Odczytanie listy klientów z konfiguracji
        /// </summary>
        /// <returns></returns>
        internal static List<Client> GetClients()
        {
            List<Client> clients = new List<Client>();
            if (!Directory.Exists(Path.GetDirectoryName(ConfigFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFile));
                return clients;
            }
            if (File.Exists(ConfigFile))
            {
                try
                {
                    XElement xElement = XElement.Load(ConfigFile);
                    return xElement.GetClients();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return clients;
        }

        /// <summary>
        /// Zapisanie klienta w konfiguracji (edycja oraz dodanie nowego)
        /// </summary>
        /// <param name="client">Klient do zapisania</param>
        /// <returns></returns>
        internal static bool SaveClient(Client client)
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFile));
                return false;
            }
            XElement xElement = null;
            if (File.Exists(ConfigFile))
            {
                xElement = XElement.Load(ConfigFile);
            }
            else
            {
                xElement = new XElement("Clients");
            }
            xElement.SetClient(client);
            xElement.Save(ConfigFile);

            return true;
        }

        /// <summary>
        /// Usunięcie klienta z konfiguracji
        /// </summary>
        /// <param name="client">Klient do usunięcia</param>
        /// <returns></returns>
        internal static bool DeleteClient(Client client)
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFile));
                return false;
            }
            XElement xElement = null;
            if (File.Exists(ConfigFile))
            {
                xElement = XElement.Load(ConfigFile);
            }
            else
            {
                return false;
            }

            xElement.DeleteClient(client);
            xElement.Save(ConfigFile);

            return true;
        }
    }
}
