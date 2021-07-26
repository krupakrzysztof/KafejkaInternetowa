using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

namespace KafejkaInternetowa.Klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;
            Closing += MainView_Closing;
        }

        private void MainView_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
