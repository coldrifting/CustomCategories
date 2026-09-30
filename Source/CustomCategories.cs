using System;
using System.Collections.Generic;
using CustomCategories.Data;
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

        Log("Replacing Subcategories...");
        ReplaceSubCategories(PartCategorizer.Instance.filters[0], () =>
        {
            foreach (CategoryDef category in Categories.GetCategories())
            {
                Icon icon = Icons.GetIcon(category.Icon);
                PartCategorizer.Category newSubCategory = new(
                    buttonType,
                    displayType,
                    category.Name,
                    category.DisplayName,
                    icon,
                    PartCategorizer.Instance.colorFilterFunction,
                    PartCategorizer.Instance.colorIcons,
                    category.Filter);

                if (category.ShouldCategoryBeShown())
                {
                    PartCategorizer.Instance.filters[0].AddSubcategory(newSubCategory);
                }
            }
        });
        Log("Done!");
    }

    public void ReplaceSubCategories(PartCategorizer.Category category, Action work)
    {
        // Delete all but 1 subcategory to keep UI from breaking
        List<PartCategorizer.Category> categories = [];
        for (int index = 0; index < category.subcategories.Count - 1; index++)
        {
            categories.Add(category.subcategories[index]);
        }
        foreach (PartCategorizer.Category subCategory in categories)
        {
            subCategory.DeleteSubcategory();
        }
        
        work.Invoke();
        
        // Once we have added our categories, we can remove the last remaining original one
        if (PartCategorizer.Instance.filters[0].subcategories.Count > 1)
        {
            PartCategorizer.Instance.filters[0].subcategories[0].DeleteSubcategory();
        }
        
        // Refresh
        Log("Refreshing...");
        PartCategorizer.Instance.filters[0].RebuildSubcategoryButtons();
    }
}
