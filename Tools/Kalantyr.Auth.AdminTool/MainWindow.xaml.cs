using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Kalantyr.Auth.AdminTool.Windows;

namespace Kalantyr.Auth.AdminTool
{
    public partial class MainWindow
    {
        public Environment SelectedEnvironment => Config.Instance.Environments.FirstOrDefault();

        public MainWindow()
        {
            InitializeComponent();
            TuneControls();
        }

        private void OnEditEnvironmentsClick(object sender, RoutedEventArgs e)
        {
            var window = new EnvironmentWindow(SelectedEnvironment) { Owner = this };
            window.ShowDialog();
        }

        private void OnLoginWithPasswordClick(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow(SelectedEnvironment) { Owner = this };
            if (window.ShowDialog() == true)
                TuneControls();
        }

        private void TuneControls()
        {
            _miUserLogout.IsEnabled = App.Tokens.ContainsKey(SelectedEnvironment);
            _miUserSetPassword.IsEnabled = _miUserLogout.IsEnabled;
            _miAdminMigrate.IsEnabled = _miUserLogout.IsEnabled;
        }

        private async void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
            {
                Cursor = Cursors.Wait;

                var ua = new UserActions(SelectedEnvironment);
                await ua.LogoutAsync(tokenSource.Token);

                TuneControls();
                MessageBox.Show("Done", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void OnSetPasswordClick(object sender, RoutedEventArgs e)
        {
            var window = new SetPasswordWindow(SelectedEnvironment) { Owner = this };
            window.ShowDialog();
        }

        private async void OnAdminMigrateClick(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            try
            {
                Cursor = Cursors.Wait;

                var aa = new AdminActions(SelectedEnvironment);
                await aa.MigrateAsync(tokenSource.Token);

                TuneControls();
                MessageBox.Show("Done", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
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

        private async void OnCreateClick(object sender, RoutedEventArgs e)
        {
            var window = new UserWindow { Owner = this };
            if (window.ShowDialog() == true)
            {
                var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                try
                {
                    Cursor = Cursors.Wait;

                    var ua = new UserActions(SelectedEnvironment);
                    await ua.CreateAsync(window.LoginPassword.Login, window.LoginPassword.Password, tokenSource.Token);

                    TuneControls();
                    MessageBox.Show("Done", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
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
}
