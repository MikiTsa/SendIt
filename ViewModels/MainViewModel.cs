using SendIt.Commands;
using SendIt.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SendIt.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Contact> _contacts;
        private Contact _selectedContact;
        private string _messageText;
        private ObservableCollection<string> _conversation;

        public ObservableCollection<Contact> Contacts
        {
            get { return _contacts; }
            set 
            { 
                _contacts = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Contacts)));
            }
        }

        public Contact SelectedContact
        {
            get { return _selectedContact; }
            set 
            { 
                _selectedContact = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedContact)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanEditOrRemove)));
            }
        }

        public string MessageText
        {
            get { return _messageText; }
            set 
            { 
                _messageText = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MessageText)));
            }
        }

        public ObservableCollection<string> Conversation
        {
            get { return _conversation; }
            set 
            { 
                _conversation = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Conversation)));
            }
        }

        public bool CanEditOrRemove => SelectedContact != null;

        public ICommand AddStaticContactCommand { get; private set; }
        public ICommand EditStaticContactCommand { get; private set; }
        public ICommand RemoveContactCommand { get; private set; }
        public ICommand SendMessageCommand { get; private set; }

        public MainViewModel()
        {
            Contacts = new ObservableCollection<Contact>
            {
                new Contact("Contact 1", "Online", "/Images/avatar1.png", "contact1@example.com", "+1 234 567 890", DateTime.Now.AddMinutes(-5)),
                new Contact("Contact 2", "Away", "/Images/avatar2.png", "contact2@example.com", "+1 234 567 891", DateTime.Now.AddHours(-1)),
                new Contact("Contact 3", "Busy", "/Images/avatar3.png", "contact3@example.com", "+1 234 567 892", DateTime.Now.AddHours(-3))
            };
            
            Conversation = new ObservableCollection<string>();
            
            AddStaticContactCommand = new RelayCommand(AddStaticContact);
            EditStaticContactCommand = new RelayCommand(EditStaticContact, param => CanEditOrRemove);
            RemoveContactCommand = new RelayCommand(RemoveContact, param => CanEditOrRemove);
            SendMessageCommand = new RelayCommand(SendMessage);
        }

        private void AddStaticContact(object parameter)
        {
            Contacts.Add(new Contact(
                "New Contact", 
                "Online", 
                "/Images/avatar2.png", 
                "newcontact@example.com", 
                "+1 234 567 893", 
                DateTime.Now));
        }

        private void EditStaticContact(object parameter)
        {
            if (SelectedContact != null)
            {
                SelectedContact.Nickname = "Updated Name";
                SelectedContact.Status = "Away";
            }
        }

        private void RemoveContact(object parameter)
        {
            if (SelectedContact != null)
            {
                Contacts.Remove(SelectedContact);
            }
        }

        private void SendMessage(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(MessageText) && SelectedContact != null)
            {
                Conversation.Add($"You: {MessageText}");
                MessageText = string.Empty;
            }
        }
    }
}