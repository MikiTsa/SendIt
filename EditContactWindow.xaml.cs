using Microsoft.Win32;
using SendIt.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SendIt
{
    public partial class EditContactWindow : Window
    {
        private MainViewModel _viewModel;
        public event EventHandler WindowClosed;

        public EditContactWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Make sure this window always appears above main window
            Owner = Application.Current.MainWindow;
        }

        private void ImgAvatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedContact == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select an avatar image",
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedAvatarPath = openFileDialog.FileName;
                _viewModel.SelectedContact.Avatar = new BitmapImage(new Uri(selectedAvatarPath));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}