using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Models.Mods
{
    public abstract class ModModule
    {
        public string SourcePath { get; }
        public virtual string FileOrFolderName => System.IO.Path.GetFileName(SourcePath);

        public abstract string StatusText { get; }

        protected ModModule(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public abstract bool IsInstalled();

        public abstract bool Install();

        public abstract bool Uninstall();
    }
}
