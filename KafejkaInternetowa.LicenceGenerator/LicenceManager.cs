using KafejkaInternetowa.Utils;
using System;
using System.IO;

namespace KafejkaInternetowa.LicenceGenerator
{
    internal static class LicenceManager
    {
        /// <summary>
        /// Katalog, w którym będą zapisywane wygenerowane licencje
        /// </summary>
        private static string LicenceDirectory { get; } = "Licencje";

        /// <summary>
        /// Generowanie pliku licencyjnego
        /// </summary>
        /// <param name="validTo">Termin ważności</param>
        /// <param name="clientCount">Dopuszczalna liczba klientów</param>
        /// <param name="number">Numer seryjny</param>
        public static void GenerateLicence(DateTime validTo, int clientCount, string number)
        {
            Licence lic = Licence.GetLicence;
            lic.ValidTo = validTo;
            lic.ClientCount = clientCount;
            lic.Number = number;
            if (!Directory.Exists(LicenceDirectory))
            {
                Directory.CreateDirectory(LicenceDirectory);
            }
            lic.Save(Path.Combine(LicenceDirectory, $"{lic.Number}.xml"));
        }
    }
}
