using RTF2ModManager.Models;
using RTF2ModManager.Models.Mods;
using System.IO;

namespace RTF2ModManager.Utils
{
    public static class L18nUtils
    {
        static Dictionary<SupportedLanguages, LocaleInst> localeInstDictionary;

        public static void Initialize()
        {
            localeInstDictionary = new Dictionary<SupportedLanguages, LocaleInst>() {
                { SupportedLanguages.en, new LocaleInst(LoadLocaleJson("en")) },
                { SupportedLanguages.zhs, new LocaleInst(LoadLocaleJson("zh_Hans")) },
                { SupportedLanguages.zht, new LocaleInst(LoadLocaleJson("zh_Hant")) },
                { SupportedLanguages.ja, new LocaleInst(LoadLocaleJson("ja")) }
            };
        }

        static string LoadLocaleJson(string fileNameNoExt)
        {
            string path = Path.Combine(AppState.LocaleJSONPath, $"{fileNameNoExt}.json");
            if (!File.Exists(path))
                throw new FileNotFoundException($"Localization File Not Found: {path}");

            return File.ReadAllText(path);
        }

        public static string GetLocalizationText(string key, params string[] args)
        {
            string? returnValue = null;

            if (localeInstDictionary.TryGetValue(AppState.CurrentLanguage, out var localeInst))
            {
                returnValue = localeInst[key];
            }

            if (string.IsNullOrEmpty(returnValue) && AppState.CurrentLanguage != SupportedLanguages.en)
            {
                if (localeInstDictionary.TryGetValue(SupportedLanguages.en, out var localeInstEn))
                {
                    returnValue = localeInstEn[key];
                }
            }

            if (string.IsNullOrEmpty(returnValue))
            {
                return key;
            }
            else
            {
                return string.Format(returnValue, args);
            }
        }
    }
}
