using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace KafejkaInternetowa.Utils
{
    public class Licence
    {
        private Licence()
        {
            if (!File.Exists(LicenceFilePath))
            {
                IsDemo = true;
            }
            else
            {
                XElement xElement = XElement.Load(LicenceFilePath);
                Number = xElement.Attribute(nameof(Number)).Value;
                ValidTo = DateTime.Parse(xElement.Attribute(nameof(ValidTo)).Value);
                ClientCount = int.Parse(xElement.Attribute(nameof(ClientCount)).Value);
                Signarute = xElement.Attribute(nameof(Signarute)).Value;
            }
        }

        /// <summary>
        /// Utworzenie instancji licencji
        /// </summary>
        public static Licence GetLicence => new Licence();

        /// <summary>
        /// Utworzenie instacji licencji demo
        /// </summary>
        public static Licence GetDemo => new Licence()
        {
            IsDemo = true
        };

        /// <summary>
        /// Numer seryjny
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Termin ważności licencji
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// Maksylana ilość klientów
        /// </summary>
        public int ClientCount { get; set; }

        /// <summary>
        /// Podpis licencji
        /// </summary>
        public string Signarute { get; set; }

        /// <summary>
        /// Czy program działa na wersji demo
        /// </summary>
        public bool IsDemo { get; private set; }

        /// <summary>
        /// Lokalizacja pliku licencji
        /// </summary>
        private static string LicenceFilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KafejkaInternetowa", "licence.xml");

        /// <summary>
        /// Zapisanie pliku licencji
        /// </summary>
        public void Save()
        {
            Save(LicenceFilePath);
        }

        /// <summary>
        /// Zapisanie pliku licencji
        /// </summary>
        /// <param name="path">Ścieżka w której zostanie zapisany plik</param>
        public void Save(string path)
        {
            Signarute = LicTools.GenerateSignature(this);
            File.WriteAllText(path, ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Usunięcie pliku licencji z domyślnej lokalizacji
        /// </summary>
        public void Remove()
        {
            File.Delete(LicenceFilePath);
        }

        /// <summary>
        /// Sprawdzenie czy licencja jest prawidłowa
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return Signarute == LicTools.GenerateSignature(this);
        }

        /// <summary>
        /// Zwrócenie licencji w formie XML bez podpisu
        /// </summary>
        /// <returns></returns>
        internal string GetLicenceWithoutSignature()
        {
            XElement xElement = XElement.Parse(ToString());
            xElement.Attribute(nameof(Signarute)).Remove();
            return xElement.ToString();
        }

        public override string ToString()
        {
            XElement xElement = new XElement(nameof(Licence));
            xElement.SetAttributeValue(nameof(Number), Number);
            xElement.SetAttributeValue(nameof(ValidTo), ValidTo.ToString("dd.MM.yyyy"));
            xElement.SetAttributeValue(nameof(ClientCount), ClientCount);
            xElement.SetAttributeValue(nameof(Signarute), Signarute);
            return xElement.ToString();
        }
    }
}
