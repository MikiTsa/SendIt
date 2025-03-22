using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SendIt
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContactsListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (contactsListView.SelectedItem != null)
            {
                string contactName = ((ListViewItem)contactsListView.SelectedItem).Content.ToString();
                MessageBox.Show($"Selected contact: {contactName}", "Contact Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}