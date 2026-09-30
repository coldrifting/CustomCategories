using System.Collections.Generic;

using static CustomCategories.Data.Utils;

namespace CustomCategories.Data;

public static class Parts
{
    private static readonly Dictionary<string, string> PartCategories = new();
    
    public static void GeneratePartDatabase()
    {
        Log("Generating Parts Database...");
        
        foreach (AvailablePart part in PartLoader.Instance.loadedParts)
        {
            if (part.partConfig == null)
            {
                continue;
            }

            if (part.category == global::PartCategories.none)
            {
                continue;
            }
            
            string cat = "";
            string catLowercase = (part.partConfig.TryGetValue("categoryCustom", ref cat) ? cat : part.category.ToString()).ToLower();

            if (catLowercase == "-1" || catLowercase == "none")
            {
                continue;
            }

            if (catLowercase == "propulsion")
            {
                bool hasEngine = false;
                foreach (AvailablePart.ModuleInfo mi in part.moduleInfos)
                {
                    if (mi.moduleName == "Engine")
                    {
                        hasEngine = true;
                        break;
                    }
                }

                catLowercase = hasEngine ? "engine" : "fueltank";
            }
            
            PartCategories.Add(part.name, catLowercase);
        }
        
        Log("Updated part categories");
    }

    public static string GetPartCategory(string part)
    {
        return PartCategories.GetValueOrDefault(part, "none");
    }

    public static bool IsPartDeprecated(AvailablePart part)
    {
        if (part == null || string.IsNullOrEmpty(part.partUrl))
        {
            return false;
        }

        return part.partUrl.Contains("zDeprecated");
    }
}