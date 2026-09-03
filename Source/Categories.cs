using System.Collections.Generic;
using UniLinq;
using static CustomCategories.Utils;

namespace CustomCategories;

public class CategoryDef
{
    public string Name;
    public string DisplayName;
    public string Icon;
    public EditorPartListFilter<AvailablePart> Filter;
    public int Priority;
    public bool Hidden;
}

public static class Categories
{
    private static readonly Dictionary<string, CategoryDef> AllCategories = [];

    public static List<CategoryDef> GetCategories()
    {
        return AllCategories.Values
            .Where(s => !s.Hidden)
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.DisplayName)
            .ThenBy(s => s.Name)
            .ToList();
    }
    
    public static void GenerateDatabase()
    {
        Log("Generating Category Database...");
        
        // Load user settings from config
        UrlDir.UrlConfig[] configs = GameDatabase.Instance.GetConfigs("CUSTOM_CATEGORY");

        switch (configs.Length)
        {
            case 0:
                Log("Did not find any user config settings.");
                return;
            case > 1:
                Log("Found multiple category settings. Using first occurrence");
                break;
        }

        ConfigNode settings = configs[0].config;
        
        ConfigNode[] categoryDefinitions = settings.GetNodes();
        foreach (ConfigNode categoryDefinition in categoryDefinitions)
        {
            if (categoryDefinition == null || categoryDefinition.name == null || categoryDefinition.name != "SUB_CATEGORY")
            {
                continue;
            }

            string categoryName = "";
            if (categoryDefinition.TryGetValue("name", ref categoryName))
            {
                string categoryDisplayName = "";
                if (!categoryDefinition.TryGetValue("displayName", ref categoryDisplayName))
                {
                    categoryDisplayName = categoryName;
                }

                string iconName = "";
                string icn = categoryDefinition.TryGetValue("icon", ref iconName) ? iconName : GetDefaultIcon(categoryName.ToLower());

                int priority = -1;
                categoryDefinition.TryGetValue("priority", ref priority);
                
                if (priority == -1)
                {
                    priority = GetDefaultPriority(categoryName);
                }

                bool hidden = false;
                categoryDefinition.TryGetValue("hidden", ref hidden);
                
                AllCategories.Add(categoryName.ToLower(), new CategoryDef
                {
                    Name = categoryName,
                    DisplayName = categoryDisplayName,
                    Icon = icn,
                    Priority = priority,
                    Hidden = hidden,
                    Filter = new EditorPartListFilter<AvailablePart>("Function_CustomCategory_" + categoryName, part =>
                    {
                        if (HighLogic.CurrentGame.Mode != Game.Modes.SANDBOX && 
                            (!ResearchAndDevelopment.PartModelPurchased(part) || !ResearchAndDevelopment.PartTechAvailable(part)))
                        {
                            return false;
                        }

                        string partCategory = Parts.GetPartCategory(part.name);

                        if (partCategory == categoryName.ToLower())
                        {
                            return true;
                        }
                        
                        // Special cases for default categories
                        if (categoryName == "fueltank" && partCategory == "propulsion")
                        {
                            foreach (var mi in part.moduleInfos)
                            {
                                if (mi.moduleName == "Engine")
                                {
                                    return false;
                                }
                            }

                            return true;  
                        }

                        if (categoryName == "engine" && partCategory == "propulsion")
                        {
                            foreach (var mi in part.moduleInfos)
                            {
                                if (mi.moduleName == "Engine")
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    })
                });
            }
        }

        AddDefaultCategories();
    }

    public static bool IsDefaultCategory(string categoryName)
    {
        if (categoryName == "Propulsion")
        {
            return true;
        }

        return GetDefaultPriority(categoryName) < 17;
    }
    
    private static string GetDefaultDisplayName(string categoryName)
    {
        return categoryName switch
        {
            "pods" => "Pods",
            "fueltank" => "Fuel Tank",
            "engine" => "Engine",
            "control" => "Command and Control",
            "structural" => "Structural",
            "robotics" => "Robotics",
            "coupling" => "Coupling",
            "payload" => "Payload",
            "aero" => "Aerodynamics",
            "ground" => "Ground",
            "thermal" => "Thermal",
            "electrical" => "Electrical",
            "communication" => "Communication",
            "science" => "Science",
            "cargo" => "Cargo",
            "utility" => "Utility",
            _ => ""
        };
    }
    
    private static int GetDefaultPriority(string categoryName)
    {
        return categoryName switch
        {
            "pods" => 1,
            "fueltank" => 2,
            "engine" => 3,
            "control" => 4,
            "structural" => 5,
            "robotics" => 6,
            "coupling" => 7,
            "payload" => 8,
            "aero" => 9,
            "ground" => 10,
            "thermal" => 11,
            "electrical" => 12,
            "communication" => 13,
            "science" => 14,
            "cargo" => 15,
            "utility" => 16,
            _ => 17
        };
    }

    private static string GetDefaultIcon(string categoryName)
    {
        return categoryName switch
        {
            "pods" => "stockIcon_pods",
            "fueltank" => "stockIcon_fueltank",
            "engine" => "stockIcon_engine",
            "control" => "stockIcon_cmdctrl",
            "structural" => "stockIcon_structural",
            "robotics" => "serenityIcon_robotics",
            "coupling" => "stockIcon_coupling",
            "payload" => "stockIcon_payload",
            "aero" => "stockIcon_aerodynamics",
            "ground" => "stockIcon_ground",
            "thermal" => "stockIcon_thermal",
            "electrical" => "stockIcon_electrical",
            "communication" => "stockIcon_communication",
            "science" => "stockIcon_science",
            "cargo" => "stockIcon_cargo",
            "utility" => "stockIcon_utility",
            _ => "stockIcon_fallback"
        };
    }

    private static CategoryDef GetDefaultCategory(string categoryName)
    {
        return new CategoryDef {
            Name = categoryName, 
            DisplayName = GetDefaultDisplayName(categoryName),  
            Icon = GetDefaultIcon(categoryName), 
            Priority = GetDefaultPriority(categoryName),
            Hidden = false,
        };
    }

    private static void AddDefaultCategories()
    {
        if (!AllCategories.ContainsKey("pods")) AllCategories.Add("pods", GetDefaultCategory("pods"));
        if (!AllCategories.ContainsKey("fueltank")) AllCategories.Add("fueltank", GetDefaultCategory("fueltank"));
        if (!AllCategories.ContainsKey("engine")) AllCategories.Add("engine", GetDefaultCategory("engine"));
        if (!AllCategories.ContainsKey("control")) AllCategories.Add("control", GetDefaultCategory("control"));
        if (!AllCategories.ContainsKey("structural")) AllCategories.Add("structural", GetDefaultCategory("structural"));
        if (!AllCategories.ContainsKey("robotics")) AllCategories.Add("robotics", GetDefaultCategory("robotics"));
        if (!AllCategories.ContainsKey("coupling")) AllCategories.Add("coupling", GetDefaultCategory("coupling"));
        if (!AllCategories.ContainsKey("payload")) AllCategories.Add("payload", GetDefaultCategory("payload"));
        if (!AllCategories.ContainsKey("aero")) AllCategories.Add("aero", GetDefaultCategory("aero"));
        if (!AllCategories.ContainsKey("ground")) AllCategories.Add("ground", GetDefaultCategory("ground"));
        if (!AllCategories.ContainsKey("thermal")) AllCategories.Add("thermal", GetDefaultCategory("thermal"));
        if (!AllCategories.ContainsKey("electrical")) AllCategories.Add("electrical", GetDefaultCategory("electrical"));
        if (!AllCategories.ContainsKey("communication")) AllCategories.Add("communication", GetDefaultCategory("communication"));
        if (!AllCategories.ContainsKey("science")) AllCategories.Add("science", GetDefaultCategory("science"));
        if (!AllCategories.ContainsKey("cargo")) AllCategories.Add("cargo", GetDefaultCategory("cargo"));
        if (!AllCategories.ContainsKey("utility")) AllCategories.Add("utility", GetDefaultCategory("utility"));
    }
}