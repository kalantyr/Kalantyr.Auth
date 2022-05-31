using System;
using System.Collections.Generic;
using System.Windows;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.AdminTool
{
    public partial class App
    {
        public static readonly IDictionary<Environment, TokenInfo> Tokens = new Dictionary<Environment, TokenInfo>();

        public static void ShowError(Exception error)
        {
            MessageBox.Show(error.Message, error.GetBaseException().GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
