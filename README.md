# CustomCategories
KSP mod that adds the ability to use extra part sorting categories in the editor

# Description
This mod allows you to create custom categories for sorting parts in the VAB and SPH. You can create new categories, give categories new icons, and change the display name of existing categories. Unlike other category mods, categories created in this mod are exclusive, so each part only exists in a single category.

Why use this mod vs the CommunityCategoryKit or FilterExtensions? When using the CCK to move parts to new categories, they become unsearchable. You can get around this by using the FilterExtensions mod, but you lose the ability to use Module Manager to create categories, and creating exlusive categories can become quite complex very quickly.

# Configuration
This mod provides a very simply way to define new categories. If you define a categoryCustom tag inside a part (usually via a Module Manager patch), the mod will move that part to the defined category, creating that category if it does not already exist. Otherwise, the stock category value will be used.

Here's an example of a possible config file:

```
CUSTOM_CATEGORY
{
    // The order of these category nodes determines the order in-game
    // If you wish to insert categories in-between default categories,
    // you should define all default categories here along with new ones 
    category = Pods
    category = Resources // New category
    category = FuelTank // Actual name of the Fuel Tanks category
  
    // These are optional
    SUB_CATEGORY
    {
      name = Resources // Used to map a subcategory node to a category. It doesn't have to be defined above
      icon = stockIcon_Utility // Use a stock icon
    }
  
    SUB_CATEGORY
    {
      name = FuelTank
      icon = customFuelTankIcon // Use a custom 32x32 pixel somewhere in the GameData folder
      displayName = Fuel Tanks // Override existing category names so that they show up with spaces in-game
    }
}
```

And a small module manager patch to assign a part to a new custom category:

```
@PART[oreTank]
{
    %categoryCustom = Resources // Use the category name, not the display name
}
```

Please note that if you don't have a config file defined, the mod should still work just fine and all vanilla categories should function as they usually do.

# Compatibility
I have not tested it, but this mod is very likely incompatible with the FilterExtensions and CommunityCategoryKit mods, since they do the same thing as this mod.

# Issues
Please feel free to post any issues here on GitHub, or post a message in the KSP forum thread.

# Credits
Thanks to the FilterExtensions and CommunityCategoryKit authors for posting their code online, it made it much easier to create this mod.
I'd also like to give a shout out to the Kramax Plugin Reload mod, which let me rapidly test code changes without restarting the game.
