namespace KafejkaInternetowa.Messages
{
    /// <summary>
    /// Wiadomość informująca o aktualnym wykorzystaniu pakietu przez klienta
    /// </summary>
    public class UpdateClientUsedMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress">Adres IP klienta</param>
        /// <param name="usedPackage">Ilość wykorzystanego pakietu</param>
        /// <param name="availablePackage">Ilość dostępnego pakietu</param>
        public UpdateClientUsedMessage(string ipAddress, double usedPackage, double availablePackage)
        {
            IPAddress = ipAddress;
            UsedPackage = usedPackage;
            AvailablePackage = availablePackage;
        }

        /// <summary>
        /// Adres IP klienta
        /// </summary>
        public string IPAddress { get; }

        /// <summary>
        /// Ilość wykorzystanego pakietu
        /// </summary>
        public double UsedPackage { get; }

        /// <summary>
        /// Ilość dostępnego pakietu
        /// </summary>
        public double AvailablePackage { get; }
    }
}
