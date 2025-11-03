using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Utils
{
    public static class GamePathUtils
    {
        private const string RelativeGamePath = @"Program Files (x86)\Steam\steamapps\common\R-Type Final 2";
        private const string GameExeName = "RTypeFinal2.exe";

        /// <summary>
        /// 在所有分区上搜索固定路径下的游戏目录
        /// </summary>
        public static string? AutoDetectGamePath()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady) continue;
                    if (drive.DriveType != DriveType.Fixed && drive.DriveType != DriveType.Removable) continue;

                    string testPath = Path.Combine(drive.Name, RelativeGamePath);
                    string testExe = Path.Combine(testPath, GameExeName);

                    if (File.Exists(testExe))
                    {
                        Console.WriteLine($"找到游戏目录: {testPath}");
                        return testPath;
                    }
                }
                catch
                {
                    // 某些盘符可能拒绝访问，忽略即可
                }
            }

            return null;
        }
    }
}
