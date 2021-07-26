using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KafejkaInternetowa.Utils
{
    /// <summary>
    /// Klasa rozszerzeń dla klasy <see cref="XElement" />
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Odczytanie klientów z XML
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static List<Client> GetClients(this XElement xElement)
        {
            List<Client> clients = new List<Client>();

            foreach (XElement item in xElement.Descendants("Client"))
            {
                Client client = new Client()
                {
                    AddressIP = item.Descendants(nameof(client.AddressIP)).FirstOrDefault().Value,
                    MachineName = item.Descendants(nameof(client.MachineName)).FirstOrDefault().Value
                };

                clients.Add(client);
            }

            return clients;
        }

        /// <summary>
        /// Zapisanie klienta w XML
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="client">Klient do zapisania</param>
        /// <returns></returns>
        public static XElement SetClient(this XElement xElement, Client client)
        {
            XElement clientXElement = xElement.Descendants("Client").FirstOrDefault(x => x.Descendants(nameof(client.AddressIP)).FirstOrDefault().Value == client.AddressIP);
            if (clientXElement == null)
            {
                clientXElement = new XElement("Client");
                xElement.Add(clientXElement);
            }
            clientXElement.SetElementValue(nameof(client.AddressIP), client.AddressIP);
            clientXElement.SetElementValue(nameof(client.MachineName), client.MachineName ?? string.Empty);

            return clientXElement;
        }

        /// <summary>
        /// Usunięcie klienta z XML
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="client">Klient do usunięcia</param>
        public static void DeleteClient(this XElement xElement, Client client)
        {
            XElement clientXElement = xElement.Descendants("Client").FirstOrDefault(x => x.Descendants(nameof(client.AddressIP)).FirstOrDefault().Value == client.AddressIP);
            if (clientXElement != null)
            {
                clientXElement.Remove();
            }
        }
    }
}
