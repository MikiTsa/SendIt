using SendIt.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text;

namespace SendIt
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }
        private SpeechManager _speechManager;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            DataContext = ViewModel;

            _speechManager = new SpeechManager();

            _speechManager.OpenAddContact += SpeechManager_OpenAddContact;
            _speechManager.AddDefaultContact += SpeechManager_AddDefaultContact;
            _speechManager.SelectContact += SpeechManager_SelectContact;
            _speechManager.RemoveContact += SpeechManager_RemoveContact;
            _speechManager.EditContact += SpeechManager_EditContact;
            _speechManager.SendMessage += SpeechManager_SendMessage;
            _speechManager.ChangeStatus += SpeechManager_ChangeStatus;

            UpdateAvailableCommands();

            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.SelectedContact) ||
                    e.PropertyName == nameof(ViewModel.CanEditOrRemove))
                {
                    UpdateAvailableCommands();
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAvailableCommands();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _speechManager.Dispose();
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

        private void ActivateSpeechMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _speechManager.StartListening();
            menuActivateSpeech.IsEnabled = false;
            menuDeactivateSpeech.IsEnabled = true;
            txtSpeechStatus.Text = "Voice commands active";
        }

        private void DeactivateSpeechMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _speechManager.StopListening();
            menuActivateSpeech.IsEnabled = true;
            menuDeactivateSpeech.IsEnabled = false;
            txtSpeechStatus.Text = "Voice commands inactive";
        }

        private void UpdateAvailableCommands()
        {
            _speechManager.UpdateAvailableCommands(ViewModel.CanEditOrRemove);

            var commandsText = new StringBuilder("Available commands: ");
            foreach (var command in _speechManager.AvailableCommands)
            {
                commandsText.Append($"'{command}', ");
            }

            if (_speechManager.AvailableCommands.Count > 0)
            {
                commandsText.Length -= 2;
            }

            txtAvailableCommands.Text = commandsText.ToString();
        }

        private void SpeechManager_OpenAddContact(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.AddContactCommand.Execute(null);
            });
        }

        private void SpeechManager_AddDefaultContact(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.AddStaticContactCommand.Execute(null);
                _speechManager.Speak("Default contact has been added to your list");
            });
        }

        private void SpeechManager_SelectContact(object sender, string indexStr)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (int.TryParse(indexStr, out int index) && index >= 0 && index < ViewModel.Contacts.Count)
                {
                    contactsListView.SelectedIndex = index;
                    contactsListView.ScrollIntoView(contactsListView.SelectedItem);
                    _speechManager.Speak($"Selected contact {ViewModel.Contacts[index].Nickname}");
                }
                else
                {
                    _speechManager.Speak("Could not select the specified contact");
                }
            });
        }

        private void SpeechManager_RemoveContact(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ViewModel.CanEditOrRemove)
                {
                    string contactName = ViewModel.SelectedContact.Nickname;
                    ViewModel.RemoveContactCommand.Execute(null);
                    _speechManager.Speak($"Contact {contactName} has been removed");
                }
                else
                {
                    _speechManager.Speak("Please select a contact first");
                }
            });
        }

        private void SpeechManager_EditContact(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ViewModel.CanEditOrRemove)
                {
                    ViewModel.EditContactCommand.Execute(null);
                }
                else
                {
                    _speechManager.Speak("Please select a contact first");
                }
            });
        }

        private void SpeechManager_SendMessage(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ViewModel.CanEditOrRemove)
                {
                    ViewModel.MessageText = "Hello from voice command!";
                    ViewModel.SendMessageCommand.Execute(null);
                    _speechManager.Speak("Message sent successfully");
                }
                else
                {
                    _speechManager.Speak("Please select a contact to send a message");
                }
            });
        }

        private void SpeechManager_ChangeStatus(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ViewModel.CanEditOrRemove)
                {
                    string oldStatus = ViewModel.SelectedContact.Status;
                    ChangeStatus_Click(null, null);
                    string newStatus = ViewModel.SelectedContact.Status;
                    _speechManager.Speak($"Status changed from {oldStatus} to {newStatus}");
                }
                else
                {
                    _speechManager.Speak("Please select a contact first");
                }
            });
        }
    }
}