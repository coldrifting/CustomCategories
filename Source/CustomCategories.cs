using System.Collections.Generic;
using CustomCategories.Data;
using KSP.Localization;
using UnityEngine;
using KSP.UI.Screens;
using RUI.Icons.Selectable;

using static CustomCategories.Data.Utils;

namespace CustomCategories;


[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class CustomCategoriesPatcher : MonoBehaviour
{
    public void Start()
    {
        Log("Starting...");

        Icons.GenerateIconDatabase();
        Parts.GeneratePartDatabase();
        Categories.GenerateDatabase();

        const PartCategorizer.ButtonType buttonType = PartCategorizer.ButtonType.SUBCATEGORY;
        const EditorPartList.State displayType = EditorPartList.State.PartsList;

        string filterByFunctionText = Localizer.GetStringByTag("#autoLOC_453547");
        PartCategorizer.Category mainCategory = PartCategorizer.Instance.filters.Find(c => c.button.categoryName == filterByFunctionText);
        if (mainCategory == null)
        {
            Log("Unable to find Filter by Function category");
            return;
        }
        
        List<PartCategorizer.Category> categoriesToRemove = [];
        foreach (PartCategorizer.Category category in mainCategory.subcategories)
        {
            categoriesToRemove.Add(category);
        }
        
        Log("Adding Custom Subcategories...");
        foreach (CategoryDef category in Categories.GetCategories())
        {
            Icon icon = Icons.GetIcon(category.Icon);
            PartCategorizer.Category newSubCategory = new(
                buttonType,
                displayType,
                category.Name,
                category.Label,
                icon,
                PartCategorizer.Instance.colorFilterFunction,
                PartCategorizer.Instance.colorIcons,
                category.Filter);

            if (category.ShouldCategoryBeShown())
            {
                mainCategory.AddSubcategory(newSubCategory);
            }
        }
        
        mainCategory.InsertSubcategoryButtons();
        
        Log("Removing Stock Subcategories...");
        foreach (PartCategorizer.Category category in categoriesToRemove)
        {
            category.DeleteSubcategory();
        }
        
        Log("Done!");
    }
}
