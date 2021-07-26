using KafejkaInternetowa.Utils;

namespace KafejkaInternetowa.Messages
{
    /// <summary>
    /// Wiadomość informująca jakiemu klientowi jest zwiększany pakiet
    /// </summary>
    public class IncreasePackageMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">Klient, któremu zwiększany jest pakiet</param>
        public IncreasePackageMessage(Client client)
        {
            Client = client;
        }

        /// <summary>
        /// Klient, któremu zwiększany jest pakiet
        /// </summary>
        public Client Client { get; }
    }
}
