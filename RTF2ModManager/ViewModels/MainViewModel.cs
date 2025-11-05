using RTF2ModManager.Models;
using RTF2ModManager.Models.Mods;
using RTF2ModManager.Utils;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;
using ToolTip = System.Windows.Controls.ToolTip;

namespace RTF2ModManager
{
    public partial class MainWindow : Window
    {
        // 数据结构
        public class ModInfo
        {
            public string Name { get; set; }
            public string Status { get; set; }
        }

        bool Initialized = false;

        public MainWindow()
        {
            InitializeComponent();
            L18nUtils.Initialize();
            AppState.DetectAndSetLanguage();

            AppState.DetectAndSetGamePath();

            if (!string.IsNullOrEmpty(AppState.GamePath))
            {
                GamePathTextBox.Text = AppState.GamePath;
            }

            UpdateLanguageSelector();
            UpdateUILocalization();

            Activated += MainWindow_Activated;

            Initialized = true;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            RefreshUE4SSStatus();
            RefreshModList();
            UpdateUIState();
        }

        private void UpdateLanguageSelector()
        {
            // 假设 AppState.CurrentLanguage 是 SupportedLanguages 类型
            string langTag = AppState.CurrentLanguage.ToString(); // 例如 "en", "zhs", ...

            foreach (ComboBoxItem item in LanguageSelector.Items)
            {
                if (item.Tag?.ToString() == langTag)
                {
                    LanguageSelector.SelectedItem = item;
                    break;
                }
            }
        }

        // 统一控制 UI 可用状态
        private void UpdateUIState()
        {
            bool hasGamePath = !string.IsNullOrEmpty(AppState.GamePath);

            // 除了手动/自动选择游戏目录按钮以外，其它全部禁用
            InstallUE4SSButton.IsEnabled = hasGamePath;
            UninstallUE4SSButton.IsEnabled = hasGamePath;
            ManualInjectButton.IsEnabled = hasGamePath;
            RestoreInjectionButton.IsEnabled = hasGamePath;
            InstallOrRepairButton.IsEnabled = false;
            UninstallButton.IsEnabled = false;
            ModList.IsEnabled = hasGamePath;
            ModDetailBox.IsEnabled = hasGamePath;

            // 如果没设置路径，清空列表与提示
            if (!hasGamePath)
            {
                UE4SSStatusText.Text = L18nUtils.GetLocalizationText("UI.Messages.SelectGamePath");
                UE4SSStatusText.Foreground = System.Windows.Media.Brushes.DarkRed;
                ModList.ItemsSource = null;
                ModDetailBox.Text = L18nUtils.GetLocalizationText("UI.Messages.SelectGamePath");
            }
        }

        private void UpdateUILocalization()
        {
            Title = L18nUtils.GetLocalizationText("UI.Elements.Title");

            // ===== 顶部 =====
            LanguageLabel.Text = L18nUtils.GetLocalizationText("UI.Phrases.Language");

            // ===== 游戏目录设置 =====
            GamePathGroup.Header = L18nUtils.GetLocalizationText("UI.Phrases.GamePathSettings");
            AutoSearchButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.AutoSearch");
            ManualSelectButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.ManualSelect");
            GamePathTextBox.Text = string.IsNullOrEmpty(AppState.GamePath)
                ? L18nUtils.GetLocalizationText("UI.Messages.SelectGamePath")
                : AppState.GamePath;

            // ===== UE4SS 管理 =====
            UE4SSGroup.Header = L18nUtils.GetLocalizationText("UI.Phrases.UE4SSStatusManage");
            InstallUE4SSButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Install");
            UninstallUE4SSButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Uninstall");
            ManualInjectButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.ManualInject");
            RestoreInjectionButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.RestoreInjection");

            // ===== Mod 管理 =====
            ModGroup.Header = L18nUtils.GetLocalizationText("UI.Phrases.ModManage");
            ModNameColumn.Header = L18nUtils.GetLocalizationText("UI.Phrases.ModName");
            ModStatusColumn.Header = L18nUtils.GetLocalizationText("UI.Phrases.InstallState");
            InstallOrRepairButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Install");
            UninstallButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Uninstall");

            // ===== 快捷功能 =====
            QuickActionsGroup.Header = L18nUtils.GetLocalizationText("UI.Phrases.Shortcuts");
            OpenModsFileButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.OpenModsTxt");
            OpenSaveFolderButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.OpenSaveFolder");
            QuickLaunchButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.QuickLaunch");

            ApplyTooltips();
        }

        // 把所有 tooltip 的赋值集中到一个方法，方便切语言时重新调用
        private void ApplyTooltips()
        {
            // 简单文本
            InstallUE4SSButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.InstallUE4SS");
            UninstallUE4SSButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.UninstallUE4SS");

            // 多行 / 需要换行的提示，使用 ToolTip 包装 TextBlock 以支持换行和最大宽度
            var fixTooltipText = L18nUtils.GetLocalizationText("UI.Tips.FixUE4SS");
            UninstallUE4SSButton.ToolTip = new ToolTip
            {
                Content = new TextBlock
                {
                    Text = fixTooltipText,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 360
                }
            };

            // 其它按钮
            ManualInjectButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.ManualInjectUE4SS");
            RestoreInjectionButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.RestoreInjectionUE4SS");

            InstallOrRepairButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.InstallMod");
            UninstallButton.ToolTip = new ToolTip
            {
                Content = new TextBlock
                {
                    Text = L18nUtils.GetLocalizationText("UI.Tips.UninstallMod"),
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 360
                }
            };

            // 快捷功能组
            OpenModsFileButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.OpenModsTxt");
            OpenSaveFolderButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.OpenSaveFolder");
            QuickLaunchButton.ToolTip = L18nUtils.GetLocalizationText("UI.Tips.QuickLaunch");

            // 可选：控制 tooltip 行为（显示延迟/持续时间）
            ToolTipService.SetInitialShowDelay(this, 100);    // ms
            ToolTipService.SetShowDuration(this, 60000);      // ms
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Initialized) return;

            if (LanguageSelector.SelectedItem is ComboBoxItem item)
            {
                string langCode = item.Tag.ToString();
                AppState.CurrentLanguage = Enum.Parse<SupportedLanguages>(langCode);
            }

            UpdateUILocalization();
            RefreshUE4SSStatus();
            RefreshModList();
            UpdateUIState();
        }

        #region 游戏目录设置
        // 自动搜索游戏路径
        private void AutoSearch_Click(object sender, RoutedEventArgs e)
        {
            AppState.DetectAndSetGamePath();

            if (!string.IsNullOrEmpty(AppState.GamePath))
            {
                GamePathTextBox.Text = AppState.GamePath;
            }

            RefreshUE4SSStatus();
            RefreshModList();
            UpdateUIState();
        }

        // 手动选择游戏目录
        private void ManualSelect_Click(object sender, RoutedEventArgs e)
        {
            AppState.SelectGamePath();

            if (!string.IsNullOrEmpty(AppState.GamePath))
            {
                GamePathTextBox.Text = AppState.GamePath;
            }

            RefreshUE4SSStatus();
            RefreshModList();
            UpdateUIState();
        }

        #endregion

        #region UE4SS状态管理
        private void RefreshUE4SSStatus()
        {
            UE4SSStatus uE4SSStatus = UE4SSUtils.GetStatus();
            // 这里可以添加实际的状态检测逻辑
            UE4SSStatusText.Text = UE4SSUtils.GetStatusText(uE4SSStatus);

            if(uE4SSStatus.ue4ssDLLExists)
            {
                InstallUE4SSButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Fix");
            }
            else
            {
                InstallUE4SSButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Install");
            }
        }

        private void InstallUE4SS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UE4SSUtils.Install();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.InstallUE4SSFailedWithError"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RefreshUE4SSStatus();
            RefreshModList();

            UE4SSStatus status = UE4SSUtils.GetStatus();
            if (status.ue4ssDLLExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.InstallUE4SSSuccess"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Success"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.InstallUE4SSFailed"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UninstallUE4SS_Click(object sender, RoutedEventArgs e)
        {
            UE4SSStatus current = UE4SSUtils.GetStatus();
            if (!current.ue4ssDLLExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.UE4SSNotInstalled"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                UE4SSUtils.Uninstall();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.UninstallUE4SSFailedWithError"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RefreshUE4SSStatus();
            RefreshModList();

            UE4SSStatus status = UE4SSUtils.GetStatus();
            if (!status.ue4ssDLLExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.UninstallUE4SSSuccess"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Success"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.UninstallUE4SSFailed"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManualInject_Click(object sender, RoutedEventArgs e)
        {
            UE4SSStatus pre = UE4SSUtils.GetStatus();
            if (!pre.ue4ssDLLExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.UE4SSNotInstalled"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (pre.injectBackUpExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.InjectBackupExists"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                UE4SSUtils.ManuallyInject();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.ManualInjectFailedWithError"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RefreshUE4SSStatus();
            RefreshModList();

            UE4SSStatus status = UE4SSUtils.GetStatus();
            if (status.injectBackUpExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.ManualInjectSuccess"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Success"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.ManualInjectFailed"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreInjection_Click(object sender, RoutedEventArgs e)
        {
            UE4SSStatus pre = UE4SSUtils.GetStatus();
            if (!pre.injectBackUpExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.NoInjectBackup"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                UE4SSUtils.RestoreInject();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.RestoreInjectFailedWithError"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RefreshUE4SSStatus();
            RefreshModList();

            UE4SSStatus status = UE4SSUtils.GetStatus();
            if (!status.injectBackUpExists)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.RestoreInjectSuccess"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Success"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.RestoreInjectFailed"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Mod管理
        // 刷新Mod列表
        private void RefreshModList()
        {
            if (string.IsNullOrEmpty(AppState.GamePath)) return;
            ModList.ItemsSource = AppState.Mods.Select(m => new ModInfo { Name = m.Name, Status = Mod.GetStatusText(m.Status) });
            ModDetailBox.Text = L18nUtils.GetLocalizationText("UI.Messages.SelectModToView");
        }

        private void InstallOrRepair_Click(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedItem is not ModInfo selectedMod)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.SelectModFirst"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mod = AppState.Mods.FirstOrDefault(m => m.Name == selectedMod.Name);
            if (mod == null)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.ModNotFound"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                InstallOrRepairButton.IsEnabled = false;
                UninstallButton.IsEnabled = false;
                ModDetailBox.Text = L18nUtils.GetLocalizationText("UI.Messages.InstallingMod");

                bool success = mod.Install();

                if (success)
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.InstallSuccess", mod.Name),
                        L18nUtils.GetLocalizationText("UI.Phrases.Complete"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.InstallFailed", mod.Name),
                        L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.InstallError", ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                InstallOrRepairButton.IsEnabled = true;
                UninstallButton.IsEnabled = true;
                RefreshModList();
            }
        }

        private async void UninstallMod_Click(object sender, RoutedEventArgs e)
        {
            if (ModList.SelectedItem is not ModInfo selectedMod)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.SelectModFirst"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Warning"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mod = AppState.Mods.FirstOrDefault(m => m.Name == selectedMod.Name);
            if (mod == null)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.ModNotFound"),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                InstallOrRepairButton.IsEnabled = false;
                UninstallButton.IsEnabled = false;
                ModDetailBox.Text = L18nUtils.GetLocalizationText("UI.Messages.UninstallingMod");

                bool success =  mod.Uninstall();

                if (success)
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.UninstallSuccess", mod.Name),
                        L18nUtils.GetLocalizationText("UI.Phrases.Uninstall"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.UninstallFailed", mod.Name),
                        L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    L18nUtils.GetLocalizationText("UI.Messages.UninstallError", ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                InstallOrRepairButton.IsEnabled = true;
                UninstallButton.IsEnabled = true;
                RefreshModList();
            }
        }

        private void ModList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModList.SelectedItem is ModInfo selectedMod)
            {
                // 通过 Mod 名找到完整 Mod 对象
                var mod = AppState.Mods.FirstOrDefault(m => m.Name == selectedMod.Name);
                if (mod != null)
                {
                    ModDetailBox.Text = mod.StatusDetailText;
                    ModStatus status = mod.Status;
                    if(status == ModStatus.NotInstalled)
                    {
                        InstallOrRepairButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Install");
                    }
                    else
                    {
                        InstallOrRepairButton.Content = L18nUtils.GetLocalizationText("UI.Phrases.Fix");
                    }

                    UE4SSStatus uE4SSStatus = UE4SSUtils.GetStatus();
                    if (uE4SSStatus.ue4ssDLLExists == false)
                    {
                        if (mod.ModModules.Any(m => m is not PAKModule))
                        {
                            ModDetailBox.Text += "\n\n" + L18nUtils.GetLocalizationText("UI.Messages.UE4SSMissingForNonPak");

                            if (mod.ModModules.Any(m => m is BPModule && m.IsInstalled()))
                            {
                                ModDetailBox.Text += "\n" + L18nUtils.GetLocalizationText("UI.Messages.BPModulePartialEffect");
                            }

                            InstallOrRepairButton.IsEnabled = false;
                        }
                        else
                        {
                            ModDetailBox.Text += "\n\n" + L18nUtils.GetLocalizationText("UI.Messages.PakModUsableWithoutUE4SS");
                            InstallOrRepairButton.IsEnabled = true;
                        }
                    }
                    else
                    {
                        InstallOrRepairButton.IsEnabled = true;
                    }
                    UninstallButton.IsEnabled = true;
                }
                else
                {
                    ModDetailBox.Text = L18nUtils.GetLocalizationText("UI.Messages.ModDetailNotFound");
                    InstallOrRepairButton.IsEnabled = false;
                    UninstallButton.IsEnabled = false;
                }
            }
            else
            {
                ModDetailBox.Text = L18nUtils.GetLocalizationText("UI.Messages.SelectModToView");
                InstallOrRepairButton.IsEnabled = false;
                UninstallButton.IsEnabled = false;
            }
        }

        #endregion

        #region 快捷功能
        private void OpenModsFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = AppState.LUAModListFilePath;
                if (!File.Exists(path))
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.ModListMissing"),
                        L18nUtils.GetLocalizationText("UI.Phrases.Warning"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.OpenFileFailed"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string saveFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    @"Saved Games\RTypeFinal2\Saved\SaveGames\");

                if (!Directory.Exists(saveFolder))
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.SaveFolderNotFound"),
                        L18nUtils.GetLocalizationText("UI.Phrases.Warning"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Process.Start(new ProcessStartInfo(saveFolder) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.OpenFolderFailed"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QuickLaunch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string exePath = AppState.ShellEXEPath;
                if (!File.Exists(exePath))
                {
                    MessageBox.Show(
                        L18nUtils.GetLocalizationText("UI.Messages.GameExeNotFound"),
                        L18nUtils.GetLocalizationText("UI.Phrases.Warning"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Process.Start(new ProcessStartInfo(exePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(L18nUtils.GetLocalizationText("UI.Messages.LaunchGameFailed"), ex.Message),
                    L18nUtils.GetLocalizationText("UI.Phrases.Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
