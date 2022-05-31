using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Kalantyr.Auth.Client;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.AdminTool.Windows
{
    public partial class LoginWindow
    {
        private readonly Environment _environment;

        public LoginWindow()
        {
            InitializeComponent();
        }

        public LoginWindow(Environment environment): this()
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        private async void OnOkClick(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                Cursor = Cursors.Wait;

                var ua = new UserActions(_environment);
                await ua.LoginByPasswordAsync(_tbLogin.Text, _pbPassword.Password, tokenSource.Token);

                DialogResult = true;
            }
            catch (Exception exception)
            {
                App.ShowError(exception);
            }
            finally
            {
                Cursor = null;
            }
        }
    }
}
