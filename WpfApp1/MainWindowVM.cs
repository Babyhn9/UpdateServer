using ClientLib;
using CrossData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ML.Advanced;
namespace ML
{
    public enum DownloadType
    {
        NotNeeded,
        Recomended,
        Necessarily,
        Untagged


    }

    public class MainWindowVM : VMBase
    {
        private Client client;
        private Command _downloadML;
        public Task UpdateTask;

        private string pathToDownloadFolder = "./download";
        private string pathToML = "./ML";
        private string pathToMLExe = "./ML/MicroLoans";
        private string pathToCurrectDesc = "./desc.txt";

        private Visibility _showProgressBar = Visibility.Collapsed;
        private DownloadType _dType = DownloadType.Untagged;

        private string _desc = "";

        private bool _enableDownload;
        private bool _enableStart;
        private Command _startCommand;
        private Task DownloadTask = null;

        private event Action<object> OnNeedStartML;

        public DownloadType DownloadType
        {
            get => _dType;
            set
            {
                _dType = value;
                PropertyChange();
            }
        }



        public int MinFiles
        {
            get => 1;
        }

        public int AllFiles
        {
            get => client.FilesOnServer;

        }

        public int CurrectFiles
        {
            get => client.DownloadedFilesCount;

        }

        public string Description
        {
            get => _desc;
            set
            {
                _desc = value;
                PropertyChange();
            }
        }

        public Command StartMLCommand
        {
            get => _startCommand ?? (_startCommand = new Command(StartML));
        }

        private void StartML(object obj)
        {
            Process.Start(Path.GetFullPath(pathToMLExe));
            Application.Current.Shutdown();
        }

        public SolidColorBrush DownloadColor
        {
            get
            {
                switch (DownloadType)
                {
                    case DownloadType.NotNeeded: return Brushes.Green;
                    case DownloadType.Recomended: return Brushes.Yellow;
                    case DownloadType.Necessarily: return Brushes.Red;
                }
                return Brushes.Red;

            }

        }

        public string DownloadNecessityText
        {
            get
            {
                switch (DownloadType)
                {
                    case DownloadType.NotNeeded: return "Последняя версия";
                    case DownloadType.Recomended: return "Есть новая версия";
                    case DownloadType.Necessarily: return "Необходимо обновление";
                }

                return "Проверка...";
            }

        }

        public bool EnableDownload
        {
            get => _enableDownload;
            set
            {
                _enableDownload = value;
                PropertyChange();
            }
        }

        public bool EnableStart
        {
            get => _enableStart;
            set
            {
                _enableStart = value;
                PropertyChange();
            }
        }

        public Visibility IsDownloaded
        {
            get => _showProgressBar;
            set
            {
                _showProgressBar = value;
                PropertyChange();
            }
        }



        public Command DownloadNewML
        {
            get => _downloadML ?? (_downloadML = new Command(StartDownload, CanDownload));
        }

        private bool CanDownload(object arg) => DownloadType != DownloadType.NotNeeded;




        /// <summary>
        /// если параметр type false, отрубает доступ к кнопкам и наоборот 
        /// </summary>
        /// <param name="type"></param>
        /// 
        private void ChangeUI(bool type)
        {
            EnableDownload = type;
            EnableStart = type;

        }

        public MainWindowVM()
        {
            client = new Client(pathToDownloadFolder);

            OnNeedStartML += StartML;
            client.OnNewFileDownload += Client_OnNewFileDownload;
            client.OnDownloadStart += Client_OnDownloadStart;
            client.OnDownloadEnd += Client_OnDownloadEnd;
            ChangeUI(false);
        }



        /// <summary>
        /// обрабатывает дополнительную информацию с сервера
        /// </summary>
        public void CheckAll()
        {
            try
            {
                var version = client.GetVersionFromServer();
                var currectVersion = GetCurrectVersion();
                Description = client.GetDescription();
                ChangeUI(false);
                if (version == currectVersion)
                {
                    DownloadType = DownloadType.NotNeeded;
                    EnableStart = true;
                }
                else
                {
                    if (client.IsNeededDownload())
                    {
                        DownloadType = DownloadType.Necessarily;
                        EnableDownload = true;
                    }
                    else
                    {
                        DownloadType = DownloadType.Recomended;
                        ChangeUI(true);
                    }
                }

                PropertyChange(nameof(DownloadNecessityText));
                PropertyChange(nameof(DownloadColor));

            }
            catch (SocketException e)
            {
                MessageBox.Show("Сервер недоступен, проверте соединение или обратитесь к администратору", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }



        public async void StartDownload(object obj)
        {879898
            IsDownloaded = Visibility.Visible;
            ChangeUI(false);
            await Task.Factory.StartNew(() => client.StartDownload());
            OnNeedStartML.Invoke(null);
        }


        /// <summary>
        /// Вызывается перед тем после того, как с сервера была получена вся дополнительная информация, но до начала скачивания файлов
        /// </summary>
        private void Client_OnDownloadStart()
        {
            PropertyChange(nameof(CurrectFiles));
            PropertyChange(nameof(AllFiles));
        }

       
        /// <summary>
        /// Вызывается при окончании скачивания файлов с сервера
        /// </summary>
        private void Client_OnDownloadEnd()
        {
            PropertyChange(nameof(CurrectFiles));

            DirectoryCopy(pathToDownloadFolder, pathToML, true);
            Directory.Delete(pathToDownloadFolder, true);

            DownloadType = DownloadType.NotNeeded;

            PropertyChange(nameof(DownloadNecessityText));
            PropertyChange(nameof(DownloadColor));

            IsDownloaded = Visibility.Collapsed;


            EnableStart = true;
            CreateNewDec(client.GetDescription());
           
        }



        /// <summary>
        /// Вызывается каждый раз, когда скачался новый файл
        /// </summary>
        private void Client_OnNewFileDownload()
        {
            PropertyChange(nameof(CurrectFiles));
        }


        /// <summary>
        /// Вытаскивает из описания текущую "мнимую версию" программы
        /// </summary>
        /// <returns></returns>
        private string GetCurrectVersion()
        {
            FileStream file = null;
            if (!File.Exists(pathToCurrectDesc))
            {
                using (file = File.Create(pathToCurrectDesc))
                    file.Write("{0}".GetBytes(), 0, "{0}".Length);
            }
            return StringChecker.GetVersionFromText(File.ReadAllText(pathToCurrectDesc));
        }


        /// <summary>
        /// Сохраняет полученное с сервера описание локально
        /// </summary>
        /// <param name="text"></param>
        private void CreateNewDec(string text)
        {
            if (!File.Exists(pathToCurrectDesc))
                File.Create(pathToCurrectDesc);

            File.WriteAllText(pathToCurrectDesc, text);

            PropertyChange(nameof(Description));
        }

        private string GetCurrectDesc()
        {
            FileStream file = null;
            if (!File.Exists(pathToCurrectDesc))
            {
                using (file = File.Create(pathToCurrectDesc))
                    file.Write("{0}".GetBytes(), 0, "{0}".Length);
            }

            return File.ReadAllText(pathToCurrectDesc);
        }



        /// <summary>
        /// Копирует дирректорию
        /// </summary>
        /// <param name="sourceDirName">источник</param>
        /// <param name="destDirName">конечный путь</param>
        /// <param name="copySubDirs">целиком или только верхний уровень</param>
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            else
            {
                Directory.Delete(destDirName, true);
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        private void KillMLIfExist()
        {
            var processes = Process.GetProcessesByName("MicroLoans");
        }

    }
}
