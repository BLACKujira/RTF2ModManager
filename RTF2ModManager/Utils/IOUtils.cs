using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Utils
{
    public static class IOUtils
    {
        /// <summary>
        /// 递归复制目录及文件
        /// </summary>
        public static void CopyDirectory(string sourceDir, string destDir, bool overwrite = false)
        {
            if(!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            // 复制所有文件
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                
                if(!File.Exists(destFile) || overwrite)
                    File.Copy(file, destFile, overwrite: true);
            }

            // 递归复制子目录
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destSubDir = Path.Combine(destDir, dirName);
                CopyDirectory(dir, destSubDir);
            }
        }
    }
}
