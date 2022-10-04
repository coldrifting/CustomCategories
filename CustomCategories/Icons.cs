using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;

namespace CustomCategories
{
    public static class Icons
    {
        public static Dictionary<string, RUI.Icons.Selectable.Icon> IconLookup = new Dictionary<string, RUI.Icons.Selectable.Icon>();

        public static void GenerateIconDatabase()
        {
            GameDatabase.TextureInfo textureInfo;
            var TextureDB = new Dictionary<string, GameDatabase.TextureInfo>();

            for (int i = GameDatabase.Instance.databaseTexture.Count - 1; i >= 0; i--)
            {
                textureInfo = GameDatabase.Instance.databaseTexture[i];
                if (textureInfo.texture != null && textureInfo.texture.width == 32 && textureInfo.texture.height == 32)
                {
                    TextureDB.Add(textureInfo.name, textureInfo);
                }
            }

            foreach (KeyValuePair<string, GameDatabase.TextureInfo> kvp in TextureDB)
            {
                if (kvp.Value.name.Contains("_selected"))
                {
                    continue;
                }

                Texture2D selectedTexture;
                if (TextureDB.TryGetValue(kvp.Value.name + "_selected", out textureInfo))
                {
                    selectedTexture = textureInfo.texture;
                }
                else
                {
                    selectedTexture = kvp.Value.texture;
                }

                string name = kvp.Value.name.Split(new char[] { '/', '\\' }).Last();
                var icon = new RUI.Icons.Selectable.Icon(name, kvp.Value.texture, selectedTexture, false);

                if (!IconLookup.ContainsKey(icon.name))
                    IconLookup.Add(icon.name, icon);
            }
        }

        public static RUI.Icons.Selectable.Icon GetIcon(string iconName)
        {
            if (iconName.StartsWith("stockIcon_") || iconName.StartsWith("serenityIcon_"))
            {
                if (PartCategorizer.Instance.iconLoader.iconDictionary.ContainsKey(iconName))
                    return PartCategorizer.Instance.iconLoader.iconDictionary[iconName];
            }

            if (IconLookup.TryGetValue(iconName, out RUI.Icons.Selectable.Icon icon))
            {
                return icon;
            }

            // Fallback icon if texture is not found
            return PartCategorizer.Instance.iconLoader.iconDictionary["stockIcon_fallback"];
        }
    }
}
