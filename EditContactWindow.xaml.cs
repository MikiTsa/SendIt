using Microsoft.Win32;
using SendIt.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
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

            if (_viewModel.SelectedContact != null)
            {
                foreach (ComboBoxItem item in cmbStatus.Items)
                {
                    if (item.Content.ToString() == _viewModel.SelectedContact.Status)
                    {
                        cmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }

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

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedContact != null && cmbStatus.SelectedItem is ComboBoxItem selectedItem)
            {
                _viewModel.SelectedContact.Status = selectedItem.Content.ToString();
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