namespace FileUploadSystem.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.IO;
    using System.Web;

    using System.Reflection;
    using FileUploadSystem.Common;

    public static class FileManipulator
    {
        public static void GenerateStorage()
        {
            for (int i = 0; i < GlobalConstants.NumberOfUploadSubdirectoryes; i++)
            {
                Directory.CreateDirectory(GlobalConstants.UploadDirectory + "\\" + i);
            }
        }

        public static void UploadFile(HttpPostedFileBase uploadedFile, int id, string fileExtension)
        {
            var path = Path.Combine(
                GetFilePath(id, fileExtension));

            uploadedFile.SaveAs(path);
        }

        public static void UploadFile(string uploadedFile, int id, string fileExtension)
        {
            var path = Path.Combine(
                GetFilePath(id, fileExtension));

            File.Copy(uploadedFile, path, true);
        }

        public static byte[] DownloadFile(int id, string fileExtension)
        {
            string fileAdress = GetFilePath(id, fileExtension);

            return File.ReadAllBytes(fileAdress);
        }

        public static void DeleteFile(int id, string fileExtension)
        {
            File.Delete(GetFilePath(id, fileExtension));
        }


        public static bool CheckFileValidity(int id, string fileExtension, long size)
        {
            var path = GetFilePath(id, fileExtension);
            var isIt = File.Exists(path);
            if (isIt)
            {
                var length = new FileInfo(path).Length;
                if (length == size)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetFilePath(int id, string fileExtension)
        {
            var path = string.Format("{0}\\{1}\\{2}{3}", 
                GlobalConstants.UploadDirectory, 
                id % GlobalConstants.NumberOfUploadSubdirectoryes, 
                id, 
                fileExtension);

            return path;
        }
    }
}