using System.Windows;

namespace KafejkaInternetowa.Klient.Views
{
    /// <summary>
    /// Interaction logic for LockedView.xaml
    /// </summary>
    public partial class LockedView : Window
    {
        public LockedView()
        {
            InitializeComponent();
            Closing += LockedView_Closing;
        }

        private void LockedView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
