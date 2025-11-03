using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RTF2ModManager.Models; // 假设 AppState 在 Models 下

namespace RTF2ModManager.Utils
{
    public static class LUAModListUtils
    {
        /// <summary>
        /// 将文件内容按行读入 List
        /// </summary>
        private static List<string> ReadAllLines()
        {
            if (!File.Exists(AppState.LUAModListFilePath))
                throw new FileNotFoundException(L18nUtils.GetLocalizationText("UI.Phrases.ModListMissing"), AppState.LUAModListFilePath);

            return File.ReadAllLines(AppState.LUAModListFilePath).ToList();
        }

        /// <summary>
        /// 将 List 写回文件，保证 Keybinds : 1 在末尾
        /// </summary>
        private static void WriteAllLines(List<string> lines)
        {
            // 分离 Keybinds 行
            string keybindsLine = lines.FirstOrDefault(l => l.Trim().StartsWith("Keybinds :"));
            if (keybindsLine != null)
            {
                lines.Remove(keybindsLine);
                lines.Add(keybindsLine); // 放末尾
            }

            File.WriteAllLines(AppState.LUAModListFilePath, lines);
        }

        public static bool AddMod(string modName, bool active = true)
        {
            var lines = ReadAllLines();
            string value = active ? "1" : "0";
            string lineContent = $"{modName} : {value}";

            // 检查是否已存在
            if (lines.Any(l => l.StartsWith(modName + " :"))) return false;

            // 找到 Built-in keybinds 注释行
            int builtInIndex = lines.FindIndex(l => l.Trim().StartsWith("; Built-in keybinds"));
            if (builtInIndex == -1)
            {
                // 如果找不到注释，就加到文件末尾
                lines.Add(lineContent);
            }
            else
            {
                // 向上回溯跳过空行
                int insertIndex = builtInIndex;
                while (insertIndex > 0 && string.IsNullOrWhiteSpace(lines[insertIndex - 1]))
                {
                    insertIndex--;
                }

                // 在空行上方插入
                lines.Insert(insertIndex, lineContent);
            }

            WriteAllLines(lines);
            return true;
        }

        public static bool RemoveMod(string modName)
        {
            var lines = ReadAllLines();
            int index = lines.FindIndex(l => l.StartsWith(modName + " :"));
            if (index == -1) return false;

            lines.RemoveAt(index);
            WriteAllLines(lines);
            return true;
        }

        public static bool IsOnList(string modName)
        {
            var lines = ReadAllLines();
            return lines.Any(l => l.StartsWith(modName + " :"));
        }

        public static bool IsActive(string modName)
        {
            var lines = ReadAllLines();
            string line = lines.FirstOrDefault(l => l.StartsWith(modName + " :"));
            if (line == null) return false;

            string[] parts = line.Split(':');
            if (parts.Length < 2) return false;

            return parts[1].Trim() == "1";
        }

        public static bool SetActiveState(string modName, bool active)
        {
            var lines = ReadAllLines();
            int index = lines.FindIndex(l => l.StartsWith(modName + " :"));
            if (index == -1) return false;

            lines[index] = $"{modName} : {(active ? "1" : "0")}";
            WriteAllLines(lines);
            return true;
        }
    }
}
