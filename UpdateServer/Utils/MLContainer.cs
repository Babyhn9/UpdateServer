using CrossData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServer.Utils
{
    public class MLContainer
    {
        private static string pathToML = "./ML";

        private static List<byte[]> FilesAsBinnary;
        private static BinaryFormatter Formatter;
        private static string PathToDescrtiption = "./description.txt";
        private static string PathToAdvancedInfo = "./info.txt";
        public MLContainer()
        {
            Formatter = new BinaryFormatter();
            FilesAsBinnary = new List<byte[]>();
            UpdateBinnaryMemory();
        }

        public static void SetDescription(string path)
        {
            File.Copy(path, PathToDescrtiption, true);
        }

        public static string GetDescription => File.ReadAllText(PathToDescrtiption);


        private static void UpdateBinnaryMemory()
        {
            FilesAsBinnary.Clear();

            var paths = GetFilePaths();



            foreach (var path in paths)
            {
                MLFileInfo info = new MLFileInfo();

                info.FileName = path.Substring(path.LastIndexOf("\\") + 1);
                info.BinaryRealize = ConvertFileToBytes(path);

                var stream = new MemoryStream();
                Formatter.Serialize(stream, info);
                FilesAsBinnary.Add(stream.GetBuffer());
            }
        }


        public static void SetNewML(string path)
        {
            if (Directory.Exists(pathToML))
                Directory.Delete(pathToML, true);

            Directory.CreateDirectory(pathToML);

            DirectoryCopy(path, pathToML, true);

            UpdateBinnaryMemory();

            var descText = GetDescription;
            var oldVersion = StringChecker.GetVersionFromText(descText);
            var newVersion = "";

            if (string.IsNullOrEmpty(oldVersion))
            {
                newVersion = "1";
            }
            else
            {
                newVersion = (int.Parse(oldVersion) + 1).ToString();
            }

            var oldText = "{" + oldVersion + "}";
            var newText = "{" + newVersion + "}";
            descText = descText.Replace(oldVersion, newVersion);

            ChangeDesc(descText);

            Console.WriteLine("Копирование выполненно");
        }

        

        /// <summary>
        /// Возвращает путь всех доступных файлов
        /// </summary>
        /// <returns></returns>
        public static string[] GetFilePaths()
        {
            return Directory.GetFiles(pathToML);
        }

        public byte[][] GetFiles()
        {
            var result = new byte[FilesAsBinnary.Count][];
            FilesAsBinnary.CopyTo(result, 0);
            return result;

        }

        public static void SetRecommend(bool isRecomended = false) => File.WriteAllText(PathToAdvancedInfo, isRecomended.ToString());
        public static string GetReccomended() => File.ReadAllText(PathToAdvancedInfo);
        /// <summary>
        /// возвращает количество файлов
        /// </summary>
        /// <returns></returns>
        public int FilesCount()
        {
            return FilesAsBinnary.Count;
        }

        public string GetFilesSizes()
        {
            var files = FilesAsBinnary;
            var result = "";

            for (int i = 0; i < files.Count; i++)
                result += files[i].Length + ((i == files.Count - 1) ? "" : "|");

            return result;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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

        public static byte[] ConvertFileToBytes(string path)
        {
            var file = File.Open(path, FileMode.Open);

            var bytes = new byte[file.Length];

            file.Read(bytes, 0, (int)file.Length);

            file.Close();
            return bytes;
        }


        private static void ChangeDesc(string text)
        {
            using (var fs = File.Open(PathToDescrtiption, FileMode.Create, FileAccess.Write))
            {
                var textAsByte = Encoding.UTF8.GetBytes(text);
                fs.Write(textAsByte, 0 , textAsByte.Length);
            }


        }

    }
}
