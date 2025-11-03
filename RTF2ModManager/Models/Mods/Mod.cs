using RTF2ModManager.Models.Mods;
using RTF2ModManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Models.Mods
{
    public class Mod
    {
        public Mod(string name, string description, string version, string author, ModModule[] modModules)
        {
            Name = name;
            Description = description;
            Version = version;
            Author = author;
            ModModules = modModules;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Version { get; private set; }
        public string Author { get; private set; }
        public virtual ModStatus Status => GetStatus();
        public virtual string StatusDetailText => GetStatusText();
        public ModModule[] ModModules { get; private set; }

        public static string GetStatusText(ModStatus status)
        {
            switch (status)
            {
                case ModStatus.NotInstalled:
                    return L18nUtils.GetLocalizationText("UI.Phrases.NotInstalled");
                case ModStatus.Installed:
                    return L18nUtils.GetLocalizationText("UI.Phrases.Installed");
                case ModStatus.PartiallyInstalled:
                    return L18nUtils.GetLocalizationText("UI.Phrases.PartiallyInstalled");
                default:
                    return L18nUtils.GetLocalizationText("UI.Phrases.Unknown");
            }
        }

        ModStatus GetStatus()
        {
            bool allInstalled = ModModules.All(m => m.IsInstalled());
            bool noneInstalled = ModModules.All(m => !m.IsInstalled());
            if (allInstalled)
            {
                return ModStatus.Installed;
            }
            else if (noneInstalled)
            {
                return ModStatus.NotInstalled;
            }
            else
            {
                return ModStatus.PartiallyInstalled;
            }
        }

        string GetStatusText()
        {
            List<string> lines = new List<string>();
            lines.Add($"{Name} {Version}");
            lines.Add(L18nUtils.GetLocalizationText(Description));
            lines.Add($"{L18nUtils.GetLocalizationText("UI.Phrases.Author")}: {Author}");
            lines.Add("");
            foreach (var module in ModModules)
            {
                lines.Add(module.StatusText);
            }
            return string.Join(Environment.NewLine, lines);
        }

        public bool Install()
        {
            foreach (var module in ModModules)
            {
                if (!module.IsInstalled())
                {
                    module.Install();
                }
            }
            return true;
        }

        public bool Uninstall()
        {
            foreach (var module in ModModules)
            {
                if (module.IsInstalled())
                {
                    module.Uninstall();
                }
            }

            return true;
        }
    }
}
