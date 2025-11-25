using Hardcodet.Wpf.TaskbarNotification;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Interop;

namespace PhotoTranslationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private TaskbarIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _notifyIcon = (TaskbarIcon)FindResource("MyNotifyIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); // Dọn bộ nhớ
            base.OnExit(e);
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow == null)
            {
                MainWindow main = new MainWindow();
                Application.Current.MainWindow = main;
                main.Show();
            }
            else
            {
                MainWindow main = (MainWindow)Application.Current.MainWindow;
                if (!main.IsVisible)
                {
                    main.Show();
                }
                main.WindowState = WindowState.Normal;
                main.Activate();
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate(); // đưa lên trước
            }
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _notifyIcon?.Dispose();  // Giải phóng icon

            if (Application.Current.MainWindow != null)
            {
                var handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                GlobalHotkey.Unregister(handle);
            }

            Application.Current.Shutdown();

        }
    }

}
