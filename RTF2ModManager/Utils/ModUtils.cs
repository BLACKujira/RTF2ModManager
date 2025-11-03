using RTF2ModManager.Models;
using RTF2ModManager.Models.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Utils
{
    public static class ModUtils
    {
        public static int UninstallAllModsHasBPMoudle()
        {
            int uninstallCount = 0;
            foreach(var mod in AppState.Mods)
            {
                if(mod.Status != ModStatus.NotInstalled || mod.Status != ModStatus.Unknown)
                {
                    mod.Uninstall();
                    uninstallCount++;
                }
            }

            return uninstallCount;
        }
    }
}
