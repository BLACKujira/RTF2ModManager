using RTF2ModManager.Utils;
using System.Diagnostics;
using System.IO;

namespace RTF2ModManager.Models.Mods
{
    public class BPModule : ModModule
    {
        public BPModule(string sourcePath) : base(sourcePath)
        {
        }

        public string FilePath => Path.Combine(AppState.LogicModsPath, FileOrFolderName);
        public string SIGPath => Path.ChangeExtension(FilePath, ".sig");
        public override string StatusText => $"{L18nUtils.GetLocalizationText("UI.Phrases.BlueprintModule")} {FileOrFolderName} " +
            $"[{L18nUtils.GetLocalizationText(IsInstalled() ? "UI.Phrases.Installed" : "UI.Phrases.NotInstalled")}] " +
            $"[{L18nUtils.GetLocalizationText(HasSIG() ? "UI.Phrases.HasSignature" : "UI.Phrases.MissingSignature")}]";

        public override bool IsInstalled()
        {
            return File.Exists(FilePath) && File.Exists(SIGPath);
        }

        public bool HasSIG()
        {
            return File.Exists(SIGPath);
        }

        public override bool Install()
        {
            string sourceFile = Path.Combine(AppState.ModsSourcePath, SourcePath);
            string destFile = Path.Combine(AppState.LogicModsPath, FileOrFolderName);
            if(!File.Exists(destFile))
                File.Copy(sourceFile, destFile);

            string destSIGFile = Path.ChangeExtension(destFile, ".sig");
            if (!File.Exists(destSIGFile))
                File.Copy(AppState.TemplateSIGPath, destSIGFile);

            return true;
        }

        public override bool Uninstall()
        {
            string destFile = Path.Combine(AppState.LogicModsPath, FileOrFolderName);
            string destSIGFile = Path.ChangeExtension(destFile, ".sig");

            if (File.Exists(destFile))
                File.Delete(destFile);
            if (File.Exists(destSIGFile))
                File.Delete(destSIGFile);

            return true;

        }
    }
}
