using KafejkaInternetowa.Utils;

namespace KafejkaInternetowa.Messages
{
    /// <summary>
    /// Wiadomość informaująca jaki klient będzie edytowany
    /// </summary>
    public class EditClientMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">Edytowany klient</param>
        /// <param name="isNew">Informacja czy klient jest nowym klientem</param>
        public EditClientMessage(Client client, bool isNew)
        {
            Client = client;
            IsNew = isNew;
        }

        /// <summary>
        /// Edytowany klient
        /// </summary>
        public Client Client { get; }

        /// <summary>
        /// Informacja czy klient jest nowym klientem
        /// </summary>
        public bool IsNew { get; }
    }
}
