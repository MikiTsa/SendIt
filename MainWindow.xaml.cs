using SendIt.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SendIt
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            DataContext = ViewModel;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContactsListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (contactsListView.SelectedItem != null)
            {
                var selectedContact = (Models.Contact)contactsListView.SelectedItem;
                MessageBox.Show($"Selected contact: {selectedContact.Nickname}", "Contact Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (contactsListView.SelectedItem != null)
            {
                var selectedContact = (Models.Contact)contactsListView.SelectedItem;

                selectedContact.Status = selectedContact.Status switch
                {
                    "Online" => "Away",
                    "Away" => "Busy",
                    "Busy" => "Invisible",
                    "Invisible" => "Online",
                    _ => "Online"
                };
            }
        }
    }
}