using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McZiper4ObserverBot
{
    public static class CreateZipService
    {
        public static async void CreateZip(
            string worldDataPath,
            List<string> pluginPath,
            List<string> modPath,
            string destp)
        {
            await Task.Run(() =>
            {
                string dest = Path.GetRandomFileName();
                DirectoryInfo s = Directory.CreateDirectory(dest);
                DirectoryInfo worlddir = s.CreateSubdirectory("w");
                if (!string.IsNullOrEmpty(worldDataPath))
                {
                    DirectoryInfo wd
                        = worlddir.CreateSubdirectory(Path.GetFileName(worldDataPath));
                    DirectoryCopy(worldDataPath, wd.FullName);
                }
                DirectoryInfo plugindir = s.CreateSubdirectory("p");
                DirectoryInfo moddir = s.CreateSubdirectory("m");
                string a = s.FullName + "\\" + "server.properties";
                StreamWriter sw
                    = new StreamWriter(
                        new FileStream(
                            a, FileMode.OpenOrCreate), Encoding.UTF8);

                
                foreach (var p in pluginPath)
                {
                    File.Copy(p, plugindir.FullName + "\\" + Path.GetFileName(p));
                }
                foreach (var m in modPath)
                {
                    File.Copy(m, moddir.FullName + "\\" + Path.GetFileName(m));
                }
                foreach (var p in PropertiesService.Properties.ToList())
                {
                    sw.WriteLine($"{p.Key}={p.Value.Value}");
                }
                sw.Close();
                ZipFile.CreateFromDirectory(
                    dest, 
                    destp + ".zip");
            });
        }
        public static void DirectoryCopy(string sourcePath, string destinationPath)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

            if (destinationDirectory.Exists == false)
            {
                destinationDirectory.Create();
                destinationDirectory.Attributes = sourceDirectory.Attributes;
            }
            foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
            {
                fileInfo.CopyTo(destinationDirectory.FullName + @"\" + fileInfo.Name, true);
            }

            foreach (System.IO.DirectoryInfo directoryInfo in sourceDirectory.GetDirectories())
            {
                DirectoryCopy(directoryInfo.FullName, destinationDirectory.FullName + @"\" + directoryInfo.Name);
            }
        }
    }
}
