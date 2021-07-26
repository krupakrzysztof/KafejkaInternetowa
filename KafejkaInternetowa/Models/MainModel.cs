using DevExpress.Mvvm;
using KafejkaInternetowa.Messages;
using KafejkaInternetowa.Utils;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace KafejkaInternetowa.Models
{
    public abstract class MainModel
    {
        /// <summary>
        /// Uruchomienie nasłuchiwania na porcie 35400
        /// </summary>
        internal static void StartListener()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 35400);
            tcpListener.Start();
            byte[] receiveBuffer = new byte[10025];

            while (true)
            {
                try
                {
                    using (TcpClient tcpClient = tcpListener.AcceptTcpClient())
                    {
                        using (NetworkStream networkStream = tcpClient.GetStream())
                        {
                            try
                            {
                                networkStream.ReadTimeout = 60;
                                int readBytes = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                                string clientData = Encoding.UTF8.GetString(receiveBuffer, 0, readBytes);
                                XElement xElement = XElement.Parse(clientData);
                                string ip = xElement.Descendants("IP").FirstOrDefault()?.Value;
                                double used = 0;
                                double available = 0;
                                try
                                {
                                    used = double.Parse(xElement.Descendants("Used").FirstOrDefault()?.Value, CultureInfo.InvariantCulture);
                                    available = double.Parse(xElement.Descendants("Available").FirstOrDefault()?.Value, CultureInfo.InvariantCulture);
                                }
                                catch
                                { }
                                if (!string.IsNullOrWhiteSpace(ip) && used > 0 && available > 0)
                                {
                                    Messenger.Default.Send(new UpdateClientUsedMessage(ip, used, available));
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    tcpListener.Stop();
                    Debug.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Wysłanie informacji do klienta o konieczności zablokowania stanowiska
        /// </summary>
        /// <param name="client">Klient do zablokowania</param>
        internal static void SendLockInfo(Client client)
        {
            XElement xElement = new XElement("LockInfo");
            xElement.Add(new XElement("Lock", true));
            try
            {
                new TcpClient(client.AddressIP, 34400).Client.Send(Encoding.UTF8.GetBytes(xElement.ToString()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
