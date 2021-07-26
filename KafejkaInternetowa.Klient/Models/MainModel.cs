using DevExpress.Mvvm;
using KafejkaInternetowa.Klient.Messages;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace KafejkaInternetowa.Klient.Models
{
    public abstract class MainModel
    {
        /// <summary>
        /// Adres IP nasłuchiwanej karty sieciowej
        /// </summary>
        internal static string IpAddress { get; set; }

        /// <summary>
        /// Wysłanie informacji o wykorzystaniu pakietu do serwera na port 35400
        /// </summary>
        /// <param name="usedPackage">Ilość wykorzystanego pakietu w GB</param>
        internal static void SendNetworkInfo(double usedPackage, double availablePackage)
        {
            XElement xElement = new XElement("Network");
            xElement.Add(new XElement("IP", IpAddress ?? string.Empty));
            xElement.Add(new XElement("MachineName", Environment.MachineName));
            xElement.Add(new XElement("Used", usedPackage));
            xElement.Add(new XElement("Available", availablePackage));
            try
            {
                new TcpClient(ConfigModel.GetServerAddress(), 35400).Client.Send(Encoding.UTF8.GetBytes(xElement.ToString()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Uruchomienie nasłuchiwania na porcie 34400
        /// </summary>
        internal static void StartListener()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 34400);
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
                                string size = xElement.Descendants("PackageSize").FirstOrDefault()?.Value;
                                if (!string.IsNullOrWhiteSpace(size))
                                {
                                    Messenger.Default.Send(new AvailablePackageMessage(double.Parse(size, CultureInfo.InvariantCulture)));
                                }
                                else
                                {
                                    string lockInfo = xElement.Descendants("Lock").FirstOrDefault()?.Value;
                                    if (!string.IsNullOrWhiteSpace(lockInfo))
                                    {
                                        Messenger.Default.Send(new LockMessage());
                                    }
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
    }
}
