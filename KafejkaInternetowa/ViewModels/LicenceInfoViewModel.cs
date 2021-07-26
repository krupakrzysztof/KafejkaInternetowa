using DevExpress.Mvvm;
using KafejkaInternetowa.Utils;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace KafejkaInternetowa.ViewModels
{
    public class LicenceInfoViewModel : ViewModelBase
    {
        public LicenceInfoViewModel()
        {
            LoadCommands();
        }

        /// <summary>
        /// Numer seryjny
        /// </summary>
        public string Number => Tools.Licence.Number;

        /// <summary>
        /// Dopuszczalna liczba klientów
        /// </summary>
        public int ClientCount => Tools.Licence.ClientCount;

        /// <summary>
        /// Termin ważności licencji
        /// </summary>
        public string ValidTo => Tools.Licence.ValidTo.ToString("dd.MM.yyyy");

        public ICommand LoadLicenceCommand { get; private set; }

        /// <summary>
        /// Domyślna lokalizacja pliku licencyjnego
        /// </summary>
        private static string LicenceFilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KafejkaInternetowa", "licence.xml");

        private void LoadCommands()
        {
            LoadLicenceCommand = new DelegateCommand(LoadLicence);
        }

        private void LoadLicence()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                AddExtension = true,
                DefaultExt = "*.xml",
                Filter = "XML|*.xml"
            };

            openFileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                File.WriteAllText(LicenceFilePath, File.ReadAllText(openFileDialog.FileName), Encoding.UTF8);
                Tools.CheckLicence();
                RaisePropertyChanged();
            }
        }
    }
}
