namespace KafejkaInternetowa.Klient.Messages
{
    /// <summary>
    /// Wiadomość informująca o ilości dostępnego pakietu
    /// </summary>
    public class AvailablePackageMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageSize">Ilość dostępnego pakietu</param>
        public AvailablePackageMessage(double packageSize)
        {
            PackageSize = packageSize;
        }

        /// <summary>
        /// Ilość dostępnego pakietu
        /// </summary>
        public double PackageSize { get; }
    }
}
