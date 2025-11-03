using RTF2ModManager.Models.Mods;
using RTF2ModManager.Utils;
using System.Globalization;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace RTF2ModManager.Models
{
    public static class AppState
    {
        public static string GamePath { get; private set; }

        public static void SetGamePath(string gamePath)
        {
            GamePath = gamePath ?? throw new ArgumentNullException(nameof(gamePath));
        }

        public static string Win64Path => Path.Combine(GamePath, "RTypeFinal2", "Binaries", "Win64");
        public static string ModsPath => Path.Combine(GamePath, "RTypeFinal2", "Binaries", "Win64", "Mods");
        public static string PaksPath => Path.Combine(GamePath, "RTypeFinal2", "Content", "Paks");
        public static string LogicModsPath => Path.Combine(GamePath, "RTypeFinal2", "Content", "Paks", "LogicMods");
        public static string LUAModListFilePath => Path.Combine(ModsPath, "mods.txt");
        public static string TemplateSIGPath => Path.Combine(PaksPath, "pakchunk0-WindowsNoEditor.sig");
        public static string TrueEXEPath => Path.Combine(Win64Path, "RTypeFinal2-Win64-Shipping.exe");
        public static string ShellEXEPath => Path.Combine(GamePath, "RTypeFinal2.exe");

        public static string ModsSourcePath => Path.Combine(AppContext.BaseDirectory, "Mods");
        public static string LocaleJSONPath => Path.Combine(AppContext.BaseDirectory, "Locales");

        public static Mod[] Mods = ModList.GetMods();
        public static SupportedLanguages CurrentLanguage = SupportedLanguages.zhs;

        public static void DetectAndSetGamePath()
        {
            string? detectedPath = Utils.GamePathUtils.AutoDetectGamePath();
            if (detectedPath != null)
            {
                SetGamePath(detectedPath);
            }
            else
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.AutoDetectGamePathFailed"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Tips"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        public static void SelectGamePath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    if (File.Exists(Path.Combine(selectedPath, "RTypeFinal2.exe")))
                    {
                        SetGamePath(selectedPath);
                    }
                    else
                    {
                        MessageBox.Show(
                            L18nUtils.GetLocalizationText("UI.Messages.InvalidGamePath"),
                            L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        public static void DetectAndSetLanguage()
        {
            try
            {
                // 获取系统当前 UI 语言（如 zh-CN、zh-TW、en-US、ja-JP）
                string lang = CultureInfo.CurrentUICulture.Name.ToLowerInvariant();

                // 精确匹配和模糊匹配
                if (lang.StartsWith("zh-cn") || lang.StartsWith("zh-hans"))
                {
                    CurrentLanguage = SupportedLanguages.zhs; // 简体中文
                }
                else if (lang.StartsWith("zh-tw") || lang.StartsWith("zh-hant") || lang.StartsWith("zh-hk") || lang.StartsWith("zh-mo"))
                {
                    CurrentLanguage = SupportedLanguages.zht; // 繁体中文
                }
                else if (lang.StartsWith("ja"))
                {
                    CurrentLanguage = SupportedLanguages.ja; // 日语
                }
                else if (lang.StartsWith("en"))
                {
                    CurrentLanguage = SupportedLanguages.en; // 英语
                }
                else
                {
                    CurrentLanguage = SupportedLanguages.en; // 默认回退
                }
            }
            catch
            {
                // 出现异常也使用英文作为默认语言
                CurrentLanguage = SupportedLanguages.en;
            }
        }
    }
}
