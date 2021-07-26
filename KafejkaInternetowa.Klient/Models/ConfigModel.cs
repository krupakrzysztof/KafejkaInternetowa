using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace KafejkaInternetowa.Klient.Models
{
    /// <summary>
    /// Model odpowiedzialny za obsługę konfiguracji
    /// </summary>
    public abstract class ConfigModel
    {
        /// <summary>
        /// Lokalizacja pliku konfiguracyjnego
        /// </summary>
        private static string ConfigFilename { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KafejkaInternetowa", "config.xml");

        /// <summary>
        /// Wyświetlenie listy dostępnych kart sieciowych
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetAvailableInterfaces()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return new List<string>();
            }

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            return interfaces.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Zapisanie adresu serwera oraz nasłuchiwanej karty sieciowej w konfiguracji
        /// </summary>
        /// <param name="selectedInterface">Nazwa nasłuchiwanej karty sieciowej</param>
        /// <param name="serverAddress">Adres IP serwera</param>
        internal static void Save(string selectedInterface, string serverAddress)
        {
            XElement xElement = null;
            if (!Directory.Exists(Path.GetDirectoryName(ConfigFilename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilename));
            }

            if (File.Exists(ConfigFilename))
            {
                xElement = XElement.Load(ConfigFilename);
            }
            else
            {
                xElement = new XElement("Config");
            }

            XElement interfaceNode = xElement.Descendants("InterfaceName").FirstOrDefault();
            if (interfaceNode != null)
            {
                interfaceNode.Value = selectedInterface;
            }
            else
            {
                interfaceNode = new XElement("InterfaceName", selectedInterface);
                xElement.Add(interfaceNode);
            }

            XElement serverNode = xElement.Descendants("ServerAddress").FirstOrDefault();
            if (serverNode != null)
            {
                serverNode.Value = serverAddress;
            }
            else
            {
                serverNode = new XElement("ServerAddress", serverAddress);
                xElement.Add(serverNode);
            }

            xElement.Save(ConfigFilename);
        }

        /// <summary>
        /// Odczytanie wartości z XML
        /// </summary>
        /// <param name="nodeName">Nazwa gałęzi do odczytania</param>
        /// <returns></returns>
        private static string GetConfigValue(string nodeName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigFilename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilename));
            }

            if (File.Exists(ConfigFilename))
            {
                XElement xElement = XElement.Load(ConfigFilename);
                XElement interfaceNode = xElement.Descendants(nodeName).FirstOrDefault();
                if (interfaceNode != null)
                {
                    return interfaceNode.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Odczytanie zapisanej nazwy karty sieciowej z konfiguracji
        /// </summary>
        /// <returns></returns>
        internal static string GetInterface()
        {
            return GetConfigValue("InterfaceName");
        }

        /// <summary>
        /// Odczytanie zapisanego adresu serwera z konfiguracji
        /// </summary>
        /// <returns></returns>
        internal static string GetServerAddress()
        {
            return GetConfigValue("ServerAddress");
        }
    }
}
