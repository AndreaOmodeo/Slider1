using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Slider1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string message = e.Exception.ToString();
            if (message.Length > 2048)
                message = message.Substring(0, 2048);
            if (MessageBox.Show(message,
                "Errore non gestito, OK per tentare di continuare",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Error,
                MessageBoxResult.Cancel) == MessageBoxResult.OK)
                e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create, but don't show the main window.
            MainWindow win = new MainWindow();
            if (e.Args.Length > 0)
            {
                string file = e.Args[0];
                if (System.IO.File.Exists(file))
                {
                    if (file.ToLower().EndsWith(".zip"))
                        win.LoadDataFromArchive(file);
                }
            }
            else
            {
                // (Perform alternate initialization here when
                // no command-line arguments are supplied.)
            }
            // This window will automatically be set as the Application.MainWindow.
            win.Show();
        }
    }
}
