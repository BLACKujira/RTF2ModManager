using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTF2ModManager.Models.Mods
{
    public static class ModList
    {
        public static Mod[] GetMods()
        {
            return new Mod[] {
                GTypeOriginMod(),
                SimpleBossLifeBarMod(),
                RTF2DebugToolsMod(),
                FPSPlayerMod()
            };
        }

        static Mod GTypeOriginMod()
        {
            return new Mod("G-Type Origin",
                "Mods.Description.GTypeOrigin",
                "v1.0.0",
                "BlackWhale",
                new ModModule[]
                {
                    new LUAModule("GTypeOrigin/LUA/GTypeOrigin_AddEnum"),
                    new BPModule("GTypeOrigin/BP/GTypeOrigin.pak"),
                    new PAKModule("GTypeOrigin/PAK/GTypeOrigin_StageList_P.pak")
                });
        }

        static Mod SimpleBossLifeBarMod()
        {
            return new Mod("Simple Boss Life Bar",
                "Mods.Description.SimpleBossLifeBar",
                "v1.0.0",
                "BlackWhale",
                new ModModule[]
                {
                    new BPModule("SimpleBossLifeBar/BP/SimpleBossLifeBar.pak"),
                });
        }

        static Mod RTF2DebugToolsMod()
        {
            return new Mod("RTF2 Debug Tools",
                "Mods.Description.RTF2DebugTools",
                "v1.0.0",
                "BlackWhale",
                new ModModule[]
                {
                    new LUAModule("RTF2DebugTools/LUA/RTF2DebugTools"),
                });
        }

        static Mod FPSPlayerMod()
        {
            return new Mod("FPS Player",
                "Mods.Description.FPSPlayer",
                "v1.0.0",
                "BlackWhale",
                new ModModule[]
                {
                    new BPModule("FPSPlayer/BP/FPSPlayer.pak"),
                });
        }
    }
}
