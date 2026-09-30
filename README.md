# CustomCategories
KSP mod that adds the ability to use extra part sorting categories in the editor

# Description
This mod allows you to create custom categories for sorting parts in the VAB and SPH. You can create new categories, give categories new icons, and change the display name of existing categories. Unlike other category mods, categories created in this mod are exclusive, so each part only exists in a single category.

Why use this mod vs the CommunityCategoryKit or FilterExtensions? When using the CCK to move parts to new categories, they become unsearchable. You can get around this by using the FilterExtensions mod, but you lose the ability to use Module Manager to create categories, and creating exlusive categories can become quite complex very quickly.

# Configuration
This mod provides a very simply way to define new categories. If you define a categoryCustom tag inside a part (usually via a Module Manager patch), the mod will move that part to the defined category, creating that category if it does not already exist. Otherwise, the stock category value will be used.

Here's an example of a possible config file:

```
// An example of creating a new category
CUSTOM_CATEGORY
{
  name = resources // A unique id for the category
  icon = stockIcon_Utility // Use a stock icon. Search the KSP log for '[CustomCategories] Valid Icon Names Listed Below:' to see a valid list
  priority = 1 // Determines the ordering of categories. Default categories default to 1-16, based on their vanilla ordering, unless you change them
}
  
// And an example of editing an existing category
CUSTOM_CATEGORY
{
  name = fueltank
  icon = customFuelTankIcon // The filename without extension of a 32 by 32 pixel image somewhere in the GameData folder. Case Sensitive.
  label = Fuel Tanks // The name of the category as shown in the in-game tooltip
  priority = 2
}

CUSTOM_CATEGORY
{
  name = utility
  hidden = true // Override existing category names so that they show up with spaces in-game
}
```

And a small module manager patch to assign a part to a new custom category:

```
@PART[oreTank]
{
    %categoryCustom = Resources // Use the category name, not the label name
}
```

If you don't have a config file defined, all categories should function as they do in a vanilla install.

# Compatibility
I have not tested it, but this mod is very likely incompatible with the FilterExtensions and CommunityCategoryKit mods, since they do the same thing as this mod.

# Issues
Please feel free to post any issues here on GitHub, or post a message in the KSP forum thread.

# Credits
Thanks to the FilterExtensions and CommunityCategoryKit authors for posting their code online, it made it much easier to create this mod.
I'd also like to give a shout out to the Kramax Plugin Reload mod, which let me rapidly test code changes without restarting the game.
