using System.Collections.Generic;

using static CustomCategories.Utils;

namespace CustomCategories;

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
            
            PartCategories.Add(part.name, catLowercase);
        }
        
        Utils.Log("Updated part categories");
    }

    public static string GetPartCategory(string part)
    {
        return PartCategories.GetValueOrDefault(part, "-1");
    }
}