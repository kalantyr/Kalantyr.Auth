using System.Windows;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.AdminTool.Windows
{
    public partial class UserWindow
    {
        public LoginPasswordDto LoginPassword { get; private set; }

        public UserWindow()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            LoginPassword = new LoginPasswordDto
            {
                Login = _tbUserName.Text,
                Password = _pbPassword.Password
            };
            DialogResult = true;
        }
    }
}
