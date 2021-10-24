using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace IRLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            window.Show();
            MainWindow.DataContext = new MainWindowViewModel(DialogCoordinator.Instance, ConfigIO.ReadFromDisk(), new IRacingExeChecker());
        }
    }
}
