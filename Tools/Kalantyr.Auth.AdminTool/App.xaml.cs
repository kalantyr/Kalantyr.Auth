using System;
using System.Windows;

namespace Kalantyr.Auth.AdminTool
{
    public partial class App
    {
        public static void ShowError(Exception error)
        {
            MessageBox.Show(error.Message, error.GetBaseException().GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
