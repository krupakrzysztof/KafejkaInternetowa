using System.ComponentModel;
using System.Windows;

namespace KafejkaInternetowa.Views
{
    /// <summary>
    /// Interaction logic for ClientView.xaml
    /// </summary>
    public partial class ClientView : Window
    {
        public ClientView()
        {
            InitializeComponent();
            Closing += ClientView_Closing;
        }

        /// <summary>
        /// Informacja czy okno może zostać zamknięte
        /// </summary>
        internal bool CanExit { get; set; }

        private void ClientView_Closing(object sender, CancelEventArgs e)
        {
            if (!CanExit)
            {
                e.Cancel = true;
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
