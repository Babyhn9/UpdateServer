using CrossData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;


namespace ClientLib
{
    public class Client
    {
        /// <summary>
        /// ДА ТУТ НЕТ ЧИСТОГО SRP, и что?
        /// Это не я пишу плохой код, это просто патерн фасад :D
        /// </summary>
        private List<MLFileInfo> DownloadedFiles;
        private BinaryFormatter Formatter;
        private List<Connector> Connectors = new List<Connector>();


        private string downloadFolder;
        private bool Connected = true;

        public event Action OnEthernetError;
        public event Action OnNewFileDownload;
        public event Action OnDownloadStart;
        public event Action OnDownloadEnd;

        public int FilesOnServer = 0;

        public int DownloadedFilesCount
        {
            get => DownloadedFiles.Count;
        }





        public Client(string pathToML = "./download")
        {
            downloadFolder = pathToML;

            Formatter = new BinaryFormatter();
            DownloadedFiles = new List<MLFileInfo>();
        }

        public void StartDownload()
        {
            FilesOnServer = GetFileCountFromServer();
            var fileSizes = GetFilesSize(FilesOnServer);

            OnDownloadStart?.Invoke();

            GetFilesFromServer(FilesOnServer, fileSizes);


            MemoryFlush();
            OnDownloadEnd?.Invoke();
        }


        private void GetFilesFromServer(int FilesOnServer, int[] fileSizes)
        {
            var connector = new Connector(CreateSocet());

            connector.Send("file");

            for (int i = 0; i < FilesOnServer; i++)
            {
                var beatByte = false;
                while (!beatByte)
                {
                    var result = connector.WaitAllAnswer(fileSizes[i]);
                    try
                    {
                        AddFileInMemory(result);
                        connector.Send("ok");
                        beatByte = true;

                    }
                    catch
                    {
                        connector.Send("re");
                        beatByte = false;
                    }

                }
                OnNewFileDownload?.Invoke();


            }

        }



        private int[] GetFilesSize(int filesCount)
        {
            var Connector = new Connector(CreateSocet());
            Connector.Send("file-size");

            int mustbeBytes = 0;
            bool beatByte = true;

            while (beatByte)
            {
                try
                {
                    mustbeBytes = int.Parse(Connector.WaitAnswer());
                    Connector.Send("ok");
                    beatByte = false;
                }
                catch
                {
                    Connector.Send("re");
                }
            }


            beatByte = true;

            while (beatByte)
            {
                try
                {
                    var answer = Connector.ParseByte(Connector.WaitAllAnswer(mustbeBytes)).Split('|').Select(e => int.Parse(e)).ToArray();
                    Connector.Send("ok");
                    beatByte = false;
                    return answer;
                }
                catch
                {
                    Connector.Send("re");
                }


            }

            return null;

        }

        public int GetFileCountFromServer()
        {
            var connector = new Connector(CreateSocet());
            //while (!connector.Client.Connected)
            //    Thread.Sleep(100);

            connector.Send("files");

            bool beatByte = true;
            int result = 0;

            while (beatByte)
            {
                try
                {
                    result = int.Parse(connector.WaitAnswer());
                    connector.Send("ok");
                    beatByte = false;
                }
                catch
                {
                    Console.WriteLine("Потеря данных о колличестве файлов");
                    connector.Send("re");
                }
            }

            return result;
        }

        public bool IsNeededDownload()
        {
            var beatByte = true;
            bool result = default;
            var connector = CreateConnector();
           
            connector.Send("req");
           
            while(beatByte)
            {
                try
                {
                    var servResult = connector.WaitAnswer();
                    result =  bool.Parse(servResult);
                    connector.Send("ok");
                    beatByte = false;
                }
                catch
                {
                    connector.Send("re");
                }
                
            }

            return result;

        }

        public string GetVersionFromServer()
        {
            var connector = new Connector(CreateSocet());
            connector.Send("ver");
            var answer = connector.WaitAnswer();
            return answer;
        }


        public string GetDescription()
        {
            var connector = new Connector(CreateSocet());
            connector.Send("desc");
            var result = connector.WaitAnswer();
            connector.Send("ok");
            return result;
        }


        private void SetConnection(Socket socket)
        {

            try
            {
                try
                {
                    socket.Connect("ip", port);
                    Connected = false;
                }

                catch
                {
                    try
                    {
                        socket.Connect("ip", port);
                        Connected = false;
                    }
                    catch
                    {
                        Console.WriteLine("Подключение недоступно, проверте интернет-соединение, или убедитесь что сервер доступен");
                        Connected = false;
                    }

                }
            }
            catch { Connected = false; Console.WriteLine("Подключение недоступно, проверте интернет-соединение, или убедитесь что сервер доступен"); }

            if (Connected)
                Console.WriteLine("Соединение с сервером установленно");
        }




        private MLFileInfo AddFileInMemory(byte[] file)
        {
            var memory = new MemoryStream(file);
            var mlInfo = (MLFileInfo)Formatter.Deserialize(memory);
            Console.WriteLine($"Файл {mlInfo.FileName} размером в {mlInfo.BinaryRealize.Length / 1024} КБ успешно скачан");
            DownloadedFiles.Add(mlInfo);
            return mlInfo;
        }


        /// <summary>
        /// Выгружает файлы из памяти на ЖД
        /// </summary>
        private void MemoryFlush()
        {
            if (Directory.Exists(downloadFolder))
                Directory.Delete(downloadFolder, true);

            Directory.CreateDirectory(downloadFolder);

            foreach (var fileInfo in DownloadedFiles)
            {
                var fs = File.Create(Path.Combine(downloadFolder, fileInfo.FileName));
                fs.Write(fileInfo.BinaryRealize, 0, fileInfo.BinaryRealize.Length);
                fs.Close();
            }
        }

        private Socket CreateSocet()
        {
            var socet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socet.ReceiveTimeout = 3000;
            socet.SendTimeout = 3000;
            SetConnection(socet);

            return socet;
        }

        private Connector CreateConnector() => new Connector(CreateSocet());
    }
}
