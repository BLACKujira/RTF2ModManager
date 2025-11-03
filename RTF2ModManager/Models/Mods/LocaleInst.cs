using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RTF2ModManager.Models.Mods
{
    public class LocaleInst
    {
        Dictionary<string, string> localeDictionary;

        public string? this[string key] => localeDictionary.TryGetValue(key, out var value) ? value : null;

        public LocaleInst(string localeJSON)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(localeJSON);

            localeDictionary = Flatten(data!); // 把嵌套结构变平（UI.Buttons.Install -> value）
        }

        private static Dictionary<string, string> Flatten(Dictionary<string, object> dict, string prefix = "")
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in dict)
            {
                string key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";
                if (kvp.Value is JsonElement elem)
                {
                    if (elem.ValueKind == JsonValueKind.Object)
                    {
                        var nested = JsonSerializer.Deserialize<Dictionary<string, object>>(elem.GetRawText());
                        foreach (var child in Flatten(nested!, key))
                            result[child.Key] = child.Value;
                    }
                    else
                    {
                        result[key] = elem.ToString() ?? "";
                    }
                }
            }
            return result;
        }
    }
}
