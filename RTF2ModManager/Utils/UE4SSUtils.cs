using RTF2ModManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Utils
{
    public static class UE4SSUtils
    {
        public static string soursePath = "UE4SS";
        public static string SourseFolder => Path.Combine(AppContext.BaseDirectory, soursePath);
        public static string InjectBackUpPath => Path.Combine(AppState.Win64Path, "RTypeFinal2-Win64-Shipping.exe~");

        public static UE4SSStatus GetStatus()
        {
            if(string.IsNullOrEmpty(AppState.GamePath))
            {
                return new UE4SSStatus
                {
                    ue4ssDLLExists = false,
                    modsTXTExists = false,
                    logicModsExists = false,
                    staticConstructObjectLUAExists = false,
                    injectBackUpExists = false
                };
            }

            string ue4ssDLLPath = Path.Combine(AppState.Win64Path, "UE4SS.dll");
            string modsTXTPath = AppState.LUAModListFilePath;
            string logicModsPath = AppState.LogicModsPath;
            string staticConstructObjectLUAPath = Path.Combine(AppState.Win64Path, "UE4SS_Signatures", "StaticConstructObject.lua");
            string injectBackUpPath = InjectBackUpPath;

            return new UE4SSStatus
            {
                ue4ssDLLExists = File.Exists(ue4ssDLLPath),
                modsTXTExists = File.Exists(modsTXTPath),
                logicModsExists = Directory.Exists(logicModsPath),
                staticConstructObjectLUAExists = File.Exists(staticConstructObjectLUAPath),
                injectBackUpExists = File.Exists(injectBackUpPath)
            };
        }

        public static bool Install()
        {
            string destPath = AppState.Win64Path;
            IOUtils.CopyDirectory(SourseFolder, destPath);
            return true;
        }

        public static bool Uninstall()
        {
            File.Delete(Path.Combine(AppState.Win64Path, "UE4SS.dll"));
            if (File.Exists(InjectBackUpPath))
                RestoreInject();

            return true;
        }

        public static bool ManuallyInject()
        {
            string setdllSource = Path.Combine(SourseFolder, "setdll.exe");
            string setdllDest = Path.Combine(AppState.Win64Path, "setdll.exe");
            string modDll = "UE4SS.dll";
            string gameExe = "RTypeFinal2-Win64-Shipping.exe";

            // 1. 检查源程序是否存在
            if (!File.Exists(setdllSource))
                throw new FileNotFoundException(
                    L18nUtils.GetLocalizationText("UI.Messages.SetDllNotFound"),
                    setdllSource
                );

            // 2. 如果备份已存在，则跳过注入（与 BAT 中逻辑一致）
            if (File.Exists(InjectBackUpPath))
                throw new InvalidOperationException(
                        L18nUtils.GetLocalizationText("UI.Messages.InjectBackupSkip", Path.GetFileName(InjectBackUpPath))
                    );

            // 3. 复制 setdll.exe 到游戏目录（覆盖）
            File.Copy(setdllSource, setdllDest, true);

            // 4. 启动 setdll.exe 执行注入
            var psi = new ProcessStartInfo
            {
                FileName = setdllDest,
                Arguments = $"-d:{modDll} {gameExe}",
                WorkingDirectory = AppState.Win64Path,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var proc = new Process { StartInfo = psi })
            {
                try
                {
                    // 捕获输出（可根据需要写入日志或绑定到 UI）
                    //proc.OutputDataReceived += (s, e) =>
                    //{
                    //    if (!string.IsNullOrEmpty(e.Data))
                    //        Logger?.LogInfo($"setdll: {e.Data}");
                    //};
                    //proc.ErrorDataReceived += (s, e) =>
                    //{
                    //    if (!string.IsNullOrEmpty(e.Data))
                    //        Logger?.LogError($"setdll ERR: {e.Data}");
                    //};

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    // 等待退出（设定超时，例如 60 秒）
                    const int timeoutMs = 60_000;
                    bool exited = proc.WaitForExit(timeoutMs);
                    if (!exited)
                    {
                        // 尝试杀死进程（谨慎）
                        try { proc.Kill(true); } catch { }
                        throw new TimeoutException(
                            L18nUtils.GetLocalizationText("UI.Messages.InjectTimeout")
                        );
                    }

                    // 检查返回码
                    if (proc.ExitCode != 0)
                    {
                        throw new Exception(
                            L18nUtils.GetLocalizationText("UI.Messages.InjectNonZeroExit", proc.ExitCode.ToString())
                        );
                    }
                }
                catch (Exception)
                {
                    // 可选：删除刚复制到游戏目录的 setdll.exe 以保持整洁
                    try { if (File.Exists(setdllDest)) File.Delete(setdllDest); } catch { }
                    throw;
                }
            }

            // 5. 注入后检测备份文件是否生成（等待短时间，多次检测）
            const int checkAttempts = 10;
            const int checkIntervalMs = 500; // 0.5s * 10 = 5s 总等待
            bool backupFound = false;
            for (int i = 0; i < checkAttempts; i++)
            {
                if (File.Exists(InjectBackUpPath))
                {
                    backupFound = true;
                    break;
                }
                Thread.Sleep(checkIntervalMs);
            }
            return backupFound;
        }

        public static bool RestoreInject()
        {
            if (!File.Exists(InjectBackUpPath))
                throw new FileNotFoundException(
                    L18nUtils.GetLocalizationText("UI.Messages.InjectBackupMissing"),
                    InjectBackUpPath
                );
            string gameExePath = AppState.TrueEXEPath;
            if (File.Exists(gameExePath))
                File.Delete(gameExePath);
            File.Copy(InjectBackUpPath, gameExePath, true);
            File.Delete(InjectBackUpPath);
            return true;
        }
    
        public static string GetStatusText(UE4SSStatus status)
        {
            List<string> InfoTextList = new List<string>();

            if (!string.IsNullOrEmpty(AppState.GamePath))
            {
                if (status.ue4ssDLLExists)
                {
                    InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Phrases.Installed"));
                    if (!status.logicModsExists)
                    {
                        InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Messages.LogicModsMissing"));
                    }
                    if (!status.staticConstructObjectLUAExists)
                    {
                        InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Messages.AOBMissing"));
                    }
                    if (status.injectBackUpExists)
                    {
                        InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Messages.ManuallyInjected"));
                    }
                    else
                    {
                        InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Messages.TryManualInject"));
                    }
                }
                else
                {
                    InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Phrases.NotInstalled"));
                    if (status.injectBackUpExists)
                    {
                        InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Messages.InjectBackupResidual"));
                    }
                }
            }
            else
            {
                InfoTextList.Add(L18nUtils.GetLocalizationText("UI.Messages.SelectGamePath"));
            }

            return string.Join(" | ", InfoTextList);
        }
    }
}
