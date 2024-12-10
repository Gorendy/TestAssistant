using System.Windows;

namespace TestClient
{
    public partial class MsgDialog : Window
    {
        public MsgDialog(string title, string message) {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;
        }
        public MsgDialog(string message) {
            InitializeComponent();
            MessageText.Text = message;
        }

        public static void showInfo(string message) {
            new MsgDialog("Info", message).ShowDialog();
        }

        public static void showWarn(string message) {
            new MsgDialog("Warn", message).ShowDialog();
        }

        public static void showError(string message) {
            new MsgDialog("Error", message).ShowDialog();
        }

        private void submit(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}