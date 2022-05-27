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
        }

        private async void OnDatabaseMigrateClick(object sender, RoutedEventArgs e)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            try
            {
                Cursor = Cursors.Wait;
                throw new NotImplementedException();
                MessageBox.Show("Done", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception error)
            {
                App.ShowError(error);
            }
            finally
            {
                Cursor = null;
            }
        }

        private void OnEditEnvironmentsClick(object sender, RoutedEventArgs e)
        {
            var window = new EnvironmentWindow(Config.Instance.Environments.First()) { Owner = this };
            window.ShowDialog();
        }
    }
}
