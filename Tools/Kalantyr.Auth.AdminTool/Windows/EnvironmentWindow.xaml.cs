using System;
using System.Windows;

namespace Kalantyr.Auth.AdminTool.Windows
{
    public partial class EnvironmentWindow
    {
        private readonly Environment _environment;

        public EnvironmentWindow()
        {
            InitializeComponent();
        }

        public EnvironmentWindow(Environment environment): this()
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _tbConnectionString.Text = environment.DbConnectionString;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            _environment.DbConnectionString = _tbConnectionString.Text;
            DialogResult = true;
        }
    }
}
