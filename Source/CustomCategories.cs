using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using RUI.Icons.Selectable;

using static CustomCategories.Utils;

namespace CustomCategories;


[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class CustomCategoriesPatcher : MonoBehaviour
{
    public void Start()
    {
        Log("Starting...");
        Icons.GenerateIconDatabase();
        Categories.GenerateDatabase();
        Parts.GeneratePartDatabase();

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

                int count = 0;
                foreach (AvailablePart part in PartLoader.Instance.loadedParts)
                {
                    if (category.Filter.FilterCriteria.Invoke(part))
                    {
                        count++;
                    }
                }
                
                if (count > 0)
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
        List<PartCategorizer.Category> copy = [];
        for (int index = 0; index < category.subcategories.Count - 1; index++)
        {
            copy.Add(category.subcategories[index]);
        }
        foreach (PartCategorizer.Category subCategory in copy)
        {
            subCategory.DeleteSubcategory();
        }
        
        work.Invoke();
        
        // Once we have added out categories, we can remove the last remaining original one
        if (PartCategorizer.Instance.filters[0].subcategories.Count > 1)
        {
            PartCategorizer.Instance.filters[0].subcategories[0].DeleteSubcategory();
        }
        
        // Refresh
        Log("Refreshing...");
        PartCategorizer.Instance.filters[0].RebuildSubcategoryButtons();
    }
}
