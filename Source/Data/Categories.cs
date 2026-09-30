using System.Collections.Generic;
using UniLinq;

using static CustomCategories.Data.Utils;

namespace CustomCategories.Data;

public class CategoryDef
{
    public string Name;
    public string DisplayName;
    public string Icon;
    public int Priority;
    public bool Hidden;
    public EditorPartListFilter<AvailablePart> Filter;

    public bool ShouldCategoryBeShown()
    {
        if (Hidden)
        {
            return false;
        }
        
        int count = 0;
        foreach (AvailablePart part in PartLoader.Instance.loadedParts)
        {
            string partCategory = Parts.GetPartCategory(part.name);
            if (partCategory == "-1" || partCategory.ToLower() == "none")
            {
                continue;
            }

            // Ignore deprecated parts that do not have modified custom categories
            if (Parts.IsPartDeprecated(part) && Parts.GetPartCategory(part.name).ToLower().Equals(part.category.ToString().ToLower()))
            {
                continue;
            }

            if (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX || 
                ResearchAndDevelopment.Instance == null ||
                ResearchAndDevelopment.PartModelPurchased(part) ||
                ResearchAndDevelopment.PartTechAvailable(part) || 
                ResearchAndDevelopment.IsExperimentalPart(part))
            {
                if (Filter.FilterCriteria.Invoke(part))
                {
                    count++;
                }
            }
        }

        return count > 0;
    }
}


public static class Categories
{
    private static readonly Dictionary<string, CategoryDef> AllCategories = [];

    private static readonly List<CategoryDef> DefaultCategories =
    [
        new() { Name = "pods",          DisplayName = "Pods and Probes",     Icon = "stockIcon_pods",          Priority = 1,  Filter = new EditorPartListFilter<AvailablePart>("Function_Pods", part => Parts.GetPartCategory(part.name) == "pods")},
        new() { Name = "fueltank",      DisplayName = "Fuel Tanks",          Icon = "stockIcon_fueltank",      Priority = 2,  Filter = new EditorPartListFilter<AvailablePart>("Function_FuelTank", part => Parts.GetPartCategory(part.name) == "fueltank")},
        new() { Name = "engine",        DisplayName = "Engines",             Icon = "stockIcon_engine",        Priority = 3,  Filter = new EditorPartListFilter<AvailablePart>("Function_Engine", part => Parts.GetPartCategory(part.name) == "engine")},
        new() { Name = "control",       DisplayName = "Command and Control", Icon = "stockIcon_cmdctrl",       Priority = 4,  Filter = new EditorPartListFilter<AvailablePart>("Function_Control", part => Parts.GetPartCategory(part.name) == "control")},
        new() { Name = "structural",    DisplayName = "Structural",          Icon = "stockIcon_structural",    Priority = 5,  Filter = new EditorPartListFilter<AvailablePart>("Function_Structural", part => Parts.GetPartCategory(part.name) == "structural")},
        new() { Name = "coupling",      DisplayName = "Coupling",            Icon = "stockIcon_coupling",      Priority = 6,  Filter = new EditorPartListFilter<AvailablePart>("Function_Coupling", part => Parts.GetPartCategory(part.name) == "coupling")},
        new() { Name = "payload",       DisplayName = "Payload",             Icon = "stockIcon_payload",       Priority = 7,  Filter = new EditorPartListFilter<AvailablePart>("Function_Payload", part => Parts.GetPartCategory(part.name) == "payload")},
        new() { Name = "aero",          DisplayName = "Aero",                Icon = "stockIcon_aerodynamics",  Priority = 8,  Filter = new EditorPartListFilter<AvailablePart>("Function_Aero", part => Parts.GetPartCategory(part.name) == "aero")},
        new() { Name = "ground",        DisplayName = "Ground",              Icon = "stockIcon_ground",        Priority = 9,  Filter = new EditorPartListFilter<AvailablePart>("Function_Ground", part => Parts.GetPartCategory(part.name) == "ground")},
        new() { Name = "thermal",       DisplayName = "Thermal",             Icon = "stockIcon_thermal",       Priority = 10, Filter = new EditorPartListFilter<AvailablePart>("Function_Thermal", part => Parts.GetPartCategory(part.name) == "thermal")},
        new() { Name = "electrical",    DisplayName = "Electrical",          Icon = "stockIcon_electrical",    Priority = 11, Filter = new EditorPartListFilter<AvailablePart>("Function_Electrical", part => Parts.GetPartCategory(part.name) == "electrical")},
        new() { Name = "communication", DisplayName = "Communication",       Icon = "stockIcon_communication", Priority = 12, Filter = new EditorPartListFilter<AvailablePart>("Function_Communication", part => Parts.GetPartCategory(part.name) == "communication")},
        new() { Name = "science",       DisplayName = "Science",             Icon = "stockIcon_science",       Priority = 13, Filter = new EditorPartListFilter<AvailablePart>("Function_Science", part => Parts.GetPartCategory(part.name) == "science")},
        new() { Name = "cargo",         DisplayName = "Cargo",               Icon = "stockIcon_cargo",         Priority = 14, Filter = new EditorPartListFilter<AvailablePart>("Function_Cargo", part => Parts.GetPartCategory(part.name) == "cargo")},
        new() { Name = "robotics",      DisplayName = "Robotics",            Icon = "serenityIcon_robotics",   Priority = 15, Filter = new EditorPartListFilter<AvailablePart>("Function_Robotics", part => Parts.GetPartCategory(part.name) == "robotics")},
        new() { Name = "utility",       DisplayName = "Utility",             Icon = "stockIcon_utility",       Priority = 16, Filter = new EditorPartListFilter<AvailablePart>("Function_Utility", part => Parts.GetPartCategory(part.name) == "utility")},
    ];
    
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

        foreach (CategoryDef category in DefaultCategories)
        {
            AllCategories.Add(category.Name, category);
        }
        
        // Load user settings from config
        UrlDir.UrlConfig[] categoryDefinitions = GameDatabase.Instance.GetConfigs("CUSTOM_CATEGORY");

        switch (categoryDefinitions.Length)
        {
            case 0:
                Log("No configs found");
                return;
        }
        
        foreach (UrlDir.UrlConfig config in categoryDefinitions)
        {
            ConfigNode categoryDefinition = config.config;
            
            string name = "";
            if (!categoryDefinition.TryGetValue("name", ref name))
            {
                Log("CUSTOM_CATEGORY node missing name. Skipping...");
                continue;
            }
            
            string displayName = null;
            categoryDefinition.TryGetValue("displayName", ref displayName);
            
            string icon = null;
            categoryDefinition.TryGetValue("icon", ref icon);
                
            int? priority = null;
            if (categoryDefinition.HasValue("priority"))
            {
                string priorityAsString = categoryDefinition.GetValue("priority");
                if (int.TryParse(priorityAsString, out int result))
                {
                    priority = result;
                }
                else
                {
                    Log($"Invalid value for setting priority: {priorityAsString}. Must be an integer (e.g. -1, 0, 1, 2, etc)");
                }
            }

            bool? hidden = null;
            if (categoryDefinition.HasValue("hidden"))
            {
                string boolAsString = categoryDefinition.GetValue("hidden");
                if (bool.TryParse(boolAsString, out bool result))
                {
                    hidden = result;
                }
                else
                {
                    Log($"Invalid value for setting hidden: {boolAsString}. Must be a boolean (True or False)");
                }
            }
            
            if (AllCategories.TryGetValue(name, out CategoryDef defaultCategory))
            {
                if (displayName != null)
                {
                    defaultCategory.DisplayName = displayName;
                }

                if (icon != null)
                {
                    defaultCategory.Icon = icon;
                }

                if (priority != null)
                {
                    defaultCategory.Priority = priority.Value;
                }
                
                if (hidden != null)
                {
                    defaultCategory.Hidden = hidden.Value;
                }
            }

            else
            {
                CategoryDef category = new()
                {
                    Name = name,
                    DisplayName = displayName ?? ToTitleCase(name),
                    Icon = icon ?? "stockIcon_fallback",
                    Priority = priority ?? int.MaxValue,
                    Hidden = hidden ?? false,
                    Filter = new EditorPartListFilter<AvailablePart>($"Function_Custom_{ToTitleCase(name)}", part => Parts.GetPartCategory(part.name) == name)
                };
                
                AllCategories.Add(category.Name, category);
            }
        }
    }
}