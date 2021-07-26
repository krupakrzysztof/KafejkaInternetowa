using System.ComponentModel;
using System.Windows;

namespace KafejkaInternetowa.Views
{
    /// <summary>
    /// Interaction logic for IncreasePackageView.xaml
    /// </summary>
    public partial class IncreasePackageView : Window
    {
        public IncreasePackageView()
        {
            InitializeComponent();
            Closing += IncreasePackageView_Closing;
        }

        /// <summary>
        /// Informacja czy okno może zostać zamknięte
        /// </summary>
        internal bool CanExit { get; set; }

        private void IncreasePackageView_Closing(object sender, CancelEventArgs e)
        {
            if (!CanExit)
            {
                e.Cancel = true;
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
