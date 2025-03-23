using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SendIt.Models
{
    public class Contact : INotifyPropertyChanged
    {
        private string _nickname;
        private string _status;
        private BitmapImage _avatar;
        private string _email;
        private string _phoneNumber;
        private DateTime _lastSeen;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nickname)));
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOnline)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusColor)));
            }
        }

        public BitmapImage Avatar
        {
            get { return _avatar; }
            set
            {
                _avatar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Avatar)));
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhoneNumber)));
            }
        }

        public DateTime LastSeen
        {
            get { return _lastSeen; }
            set
            {
                _lastSeen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastSeen)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastSeenFormatted)));
            }
        }

        public bool IsOnline => Status == "Online";

        public string LastSeenFormatted => LastSeen.ToString("g");

        public Brush StatusColor
        {
            get
            {
                return Status switch
                {
                    "Online" => Brushes.Green,
                    "Away" => Brushes.Orange,
                    "Busy" => Brushes.Red,
                    "Invisible" => Brushes.Gray,
                    _ => Brushes.Gray
                };
            }
        }

        public Contact(string nickname, string status, string avatarPath, string email, string phoneNumber, DateTime lastSeen)
        {
            Nickname = nickname;
            Status = status;
            Avatar = new BitmapImage(new Uri(avatarPath, UriKind.Relative));
            Email = email;
            PhoneNumber = phoneNumber;
            LastSeen = lastSeen;
        }
    }
}