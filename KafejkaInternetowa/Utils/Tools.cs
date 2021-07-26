using System;
using System.Windows;

namespace KafejkaInternetowa.Utils
{
    public static class Tools
    {
        /// <summary>
        /// Licencja programu
        /// </summary>
        internal static Licence Licence { get; private set; }

        /// <summary>
        /// Sprawdzenie licencji
        /// </summary>
        internal static void CheckLicence()
        {
            Licence = Licence.GetLicence;

            if (!Licence.IsDemo)
            {
                if (!Licence.IsValid())
                {
                    MessageBox.Show("Plik licencji został uszkodzony", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    Licence = Licence.GetDemo;
                }

                if ((Licence.ValidTo - DateTime.Now).Days < 0)
                {
                    MessageBox.Show($"Licencja wygasła w dniu {Licence.ValidTo.ToShortDateString()}", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                    Licence = Licence.GetDemo;
                }
                else if ((Licence.ValidTo - DateTime.Now).Days <= 30)
                {
                    MessageBox.Show($"Licencja wygaśnie dnia {Licence.ValidTo.ToShortDateString()}", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                
            }
        }
    }
}
