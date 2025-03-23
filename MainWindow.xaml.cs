using SendIt.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SendIt
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Contact> Contacts { get; set; }
        private Contact _selectedContact;

        public Contact SelectedContact
        {
            get { return _selectedContact; }
            set { _selectedContact = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            Contacts = new ObservableCollection<Contact>
            {
                new Contact("Contact 1", "Online", "/Images/avatar1.png", "contact1@example.com", "+1 234 567 890", DateTime.Now.AddMinutes(-5)),
                new Contact("Contact 2", "Away", "/Images/avatar2.png", "contact2@example.com", "+1 234 567 891", DateTime.Now.AddHours(-1)),
                new Contact("Contact 3", "Busy", "/Images/avatar3.png", "contact3@example.com", "+1 234 567 892", DateTime.Now.AddHours(-3))
            };

            DataContext = this;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContactsListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (contactsListView.SelectedItem != null)
            {
                Contact selectedContact = (Contact)contactsListView.SelectedItem;
                MessageBox.Show($"Selected contact: {selectedContact.Nickname}", "Contact Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (contactsListView.SelectedItem != null)
            {
                Contact selectedContact = (Contact)contactsListView.SelectedItem;

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