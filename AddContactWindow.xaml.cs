using Microsoft.Win32;
using SendIt.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SendIt
{
    public partial class AddContactWindow : Window
    {
        private string _selectedAvatarPath = "/Images/default_avatar.png";
        private bool _isCustomAvatar = false;

        public Contact NewContact { get; private set; }

        public AddContactWindow()
        {
            InitializeComponent();
            dpLastSeen.SelectedDate = DateTime.Now;
            btnAdd.Click += BtnAdd_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void ImgAvatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select an avatar image",
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedAvatarPath = openFileDialog.FileName;
                _isCustomAvatar = true;
                imgAvatar.Source = new BitmapImage(new Uri(_selectedAvatarPath));
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                string nickname = txtNickname.Text;
                string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Online";
                string email = txtEmail.Text;
                string phoneNumber = txtPhoneNumber.Text;
                DateTime lastSeen = dpLastSeen.SelectedDate ?? DateTime.Now;

                BitmapImage avatar;
                if (_isCustomAvatar)
                {
                    avatar = new BitmapImage(new Uri(_selectedAvatarPath));
                }
                else
                {
                    avatar = new BitmapImage(new Uri(_selectedAvatarPath, UriKind.Relative));
                }

                NewContact = new Contact(nickname, status, _isCustomAvatar ? _selectedAvatarPath : _selectedAvatarPath, email, phoneNumber, lastSeen);

                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text))
            {
                MessageBox.Show("Nickname cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
                if (!Regex.IsMatch(txtEmail.Text, emailPattern))
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Email cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Phone number cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}