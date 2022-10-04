using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;

namespace CustomCategories
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class CustomCategories : MonoBehaviour
    {
        private bool initalized = false;

        // More permanent reference to the Filter by Function category
        private const string CategoryButtonLocId = "#autoLOC_453547";

        private readonly Dictionary<string, SubCategory> subCategories = new Dictionary<string, SubCategory>();

        public void Awake()
        {
            GameEvents.onGUIEditorToolbarReady.Add(Start);
        }

        public void Start()
        {
            GameEvents.onGUIEditorToolbarReady.Remove(Start);
            if (!initalized)
                Init();
        }

        /// <summary>
        /// Main entry point of mod. Replaces all Filter by Function subcategories
        /// with new ones generated based on existing categories of unlocked parts.
        /// Also uses custom config data to create new categories.
        /// </summary>
        private void Init()
        {
            initalized = true;

            PartCategorizer.Category mainCategory = PartCategorizer.Instance.filters.Find(
                    f => f.button.categorydisplayName == CategoryButtonLocId);

            if (mainCategory != null)
            {
                // Wipe out all old subcategories
                if (mainCategory.button.activeButton.CurrentState == KSP.UI.UIRadioButton.State.True)
                {
                    PartCategorizer.Category subcat = mainCategory.subcategories.Find(
                            c => c.button.activeButton.CurrentState == KSP.UI.UIRadioButton.State.True);
                    if (subcat != null)
                    {
                        subcat.OnFalseSUB(subcat);
                    }
                    PartCategorizer.Instance.scrollListSub.Clear(false);
                }
                mainCategory.subcategories.Clear();

                // Create an icon database before we start create subcategories
                Icons.GenerateIconDatabase();

                // Generate sub categories based on installed parts
                GenerateSubCategories();

                // Load any user config files
                LoadUserConfig();

                // Add the new subcategories
                List<SubCategory> newSubCatagories = subCategories.Values.ToList();
                newSubCatagories.Sort();

                foreach (var subCategory in newSubCatagories)
                {
                    if (subCategory.PartFilter.Any())
                    {
                        PartCategorizer.AddCustomSubcategoryFilter(
                            mainCategory, subCategory.Name, subCategory.DisplayName, subCategory.Icon, subCategory.Filter);
                    }
                }
            }
        }

        /// <summary>
        /// Generates categories based on part tag data
        /// </summary>
        private void GenerateSubCategories()
        {
            subCategories.Clear();

            // Get available parts
            HashSet<AvailablePart> parts = new HashSet<AvailablePart>(PartLoader.LoadedPartsList.Where(p => ResearchAndDevelopment.PartModelPurchased(p)));

            foreach (AvailablePart part in parts)
            {
                if (part != null && part.partConfig != null)
                {
                    string partCategory = part.category.ToString();

                    // Don't include deprecated parts
                    if (partCategory == "none")
                        continue;

                    // Use Stock category values if there is no custom tag
                    string category = "default";
                    if (!part.partConfig.TryGetValue("categoryCustom", ref category))
                    {
                        category = partCategory;

                        // Handle duplicate category
                        if (part.category.ToString() == "Propulsion")
                            category = "FuelTank";
                    }

                    // Assign to subcategory container
                    if (subCategories.TryGetValue(category, out SubCategory subcat))
                    {
                        subcat.AddPart(part.name);
                    }
                    else
                    {
                        SubCategory sc = new SubCategory(category);
                        sc.AddPart(part.name);

                        subCategories.Add(category, sc);
                    }
                }
            }

        }

        /// <summary>
        /// Loads user config settings to customize auto-generated categories
        /// </summary>
        private void LoadUserConfig()
        {
            // Load user settings from config
            var configs = GameDatabase.Instance.GetConfigs("CUSTOM_CATEGORY");

            if (configs.Length == 0)
            {
                Debug.Log("[CustomCategories] Did not find any user config settings.");
                return;
            }

            if (configs.Length > 1)
                Debug.Log("[CustomCategories] Found multiple category settings. Using first occurance");

            ConfigNode modSettings = configs[0].config;

            if (modSettings != null)
            {
                // Custom sorting order based on position in config node
                string[] categories = modSettings.GetValues();
                for (int i = 0; i < categories.Length; i++)
                {
                    if (subCategories.TryGetValue(categories[i], out SubCategory subCat))
                    {
                        subCat.Order = i;
                    }
                }

                // Custom icons and display names
                ConfigNode[] subCatNodes = modSettings.GetNodes();
                foreach (ConfigNode subCatNode in subCatNodes)
                {
                    if (subCatNode != null && subCatNode.name != null && subCatNode.name == "SUB_CATEGORY")
                    {
                        string subcatName = "";
                        if (subCatNode.TryGetValue("name", ref subcatName))
                        {
                            if (subCategories.TryGetValue(subcatName, out SubCategory subcat))
                            {
                                string iconName = "";
                                if (subCatNode.TryGetValue("icon", ref iconName))
                                {
                                    subcat.Icon = Icons.GetIcon(iconName);
                                }

                                string displayName = "";
                                if (subCatNode.TryGetValue("displayName", ref displayName))
                                {
                                    subcat.DisplayName = displayName;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
