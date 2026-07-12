using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FAFramework.Utility
{
    public class FileUtility
    {
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                try
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest, true);
                }
                catch { }
            }

            foreach (string folder in folders)
            {
                try
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
                catch { }
            }
        }

        public static void DeleteAllFile(string directory, Func<string, bool> compare, bool includeSubDirectories)
        {
            try
            {
                var files = Directory.GetFiles(directory);

                foreach (var item in files)
                {
                    if (compare(item) == true)
                    {
                        File.Delete(item);
                    }
                }

                //var dir = Directory.GetDirectories(directory);
                //if (dir != null)
                //{
                //    foreach (var item in dir)
                //    {
                //        DeleteAllFile(item, compare, includeSubDirectories);                        
                //    }
                //}
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }
    }
}
