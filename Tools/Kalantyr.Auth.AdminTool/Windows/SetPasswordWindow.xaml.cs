using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Kalantyr.Auth.AdminTool.Windows
{
    public partial class SetPasswordWindow
    {
        private readonly Environment _environment;

        public SetPasswordWindow()
        {
            InitializeComponent();
        }

        public SetPasswordWindow(Environment environment): this()
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
                await ua.SetPasswordAsync(_pbOldPassword.Password, _pbNewPassword.Password, tokenSource.Token);

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
