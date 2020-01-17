using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ML
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVM context = new MainWindowVM();

        public MainWindow()
        {
            DataContext = context;
            ContentRendered += StartCheckInfoOnServer;
            InitializeComponent();
        }

        private async void StartCheckInfoOnServer(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() => context.CheckAll());

            if (context.DownloadType == DownloadType.NotNeeded)
                context.StartMLCommand.Execute(null);
        }
    }
}
