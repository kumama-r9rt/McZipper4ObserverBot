using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McZiper4ObserverBot
{
    public static class ZipFormatCheckService
    {
        public static bool CheckWorldDataFormat(string path)
        {
            if (!Directory.Exists(path)) return false;
            DirectoryInfo worlddir = new DirectoryInfo(path);
            List<FileInfo> fs = worlddir.GetFiles().ToList();
            foreach(FileInfo file in fs)
            {
                if(file.Name == "level.dat")
                    return true;
            }
            return false;
        }
    }
}
