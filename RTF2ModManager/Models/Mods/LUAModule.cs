using RTF2ModManager.Utils;
using System.IO;

namespace RTF2ModManager.Models.Mods
{
    public class LUAModule : ModModule
    {
        public LUAModule(string sourcePath) : base(sourcePath)
        {
        }

        public string FolderPath => Path.Combine(AppState.ModsPath, FileOrFolderName);
        public override string StatusText => $"{L18nUtils.GetLocalizationText("UI.Phrases.LuaModule")} {FileOrFolderName} " +
            $"[{L18nUtils.GetLocalizationText(IsInstalled() ? "UI.Phrases.Installed" : "UI.Phrases.NotInstalled")}] " +
            $"[{(File.Exists(AppState.LUAModListFilePath) ? L18nUtils.GetLocalizationText(IsActive() ? "UI.Phrases.Active" : "UI.Phrases.Inactive") : L18nUtils.GetLocalizationText("UI.Phrases.ModListMissing"))}]";

        public override bool IsInstalled()
        {
            return Directory.Exists(FolderPath);
        }

        public bool IsActive()
        {
            return LUAModListUtils.IsActive(FileOrFolderName);
        }

        public override bool Install()
        {
            string sourceFolder = Path.Combine(AppState.ModsSourcePath, SourcePath);
            string destPath = Path.Combine(AppState.ModsPath,FileOrFolderName);
            IOUtils.CopyDirectory(sourceFolder, destPath);
            LUAModListUtils.AddMod(FileOrFolderName);
            return true;
        }

        public override bool Uninstall()
        {
            string destPath = Path.Combine(AppState.ModsPath, FileOrFolderName);
            if(Directory.Exists(destPath))
                Directory.Delete(destPath, recursive: true);
            LUAModListUtils.RemoveMod(FileOrFolderName);
            return true;
        }
    }
}
