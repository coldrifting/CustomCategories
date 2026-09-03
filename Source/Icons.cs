using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;
using RUI.Icons.Selectable;

using static CustomCategories.Utils;

namespace CustomCategories;

public static class Icons
{
    private static readonly Icon Fallback = new("fallback", Texture2D.blackTexture, Texture2D.whiteTexture);
    private static readonly Dictionary<string, Icon> IconLookup = new();

    public static void GenerateIconDatabase()
    {
        Log("Generating Icon Database...");
        
        GameDatabase.TextureInfo textureInfo;
        Dictionary<string, GameDatabase.TextureInfo> textureDB = new();

        
        for (int i = GameDatabase.Instance.databaseTexture.Count - 1; i >= 0; i--)
        {
            textureInfo = GameDatabase.Instance.databaseTexture[i];
            if (textureInfo.texture != null && textureInfo.texture.width == 32 && textureInfo.texture.height == 32)
            {
                textureDB.Add(textureInfo.name, textureInfo);
            }
        }

        foreach (KeyValuePair<string, GameDatabase.TextureInfo> kvp in textureDB)
        {
            if (kvp.Value.name.Contains("_selected"))
            {
                continue;
            }

            string name = kvp.Value.name.Split('/', '\\').Last();

            Icon icon = textureDB.TryGetValue(kvp.Value.name + "_selected", out textureInfo) 
                ? new Icon(name, kvp.Value.texture, textureInfo.texture) 
                : new Icon(name, InvertTexture(kvp.Value.texture), kvp.Value.texture);
            
            IconLookup.TryAdd(icon.name, icon);
        }

        HashSet<string> allIcons = [];
        foreach (KeyValuePair<string, Icon> kvp in PartCategorizer.Instance.iconLoader.iconDictionary)
        {
            if (kvp.Key.StartsWith("stockIcon_") || kvp.Key.StartsWith("serenityIcon_"))
            {
                allIcons.Add(kvp.Key);
                IconLookup.TryAdd(kvp.Key, kvp.Value);
            }
        }
        foreach (string iconName in IconLookup.Keys)
        {
            allIcons.Add(iconName);
        }
        
        Log($"Icon Database Successfully Generated. Found {allIcons.Count} Icons");
        Log("Valid Icon Names Listed Below:");
        foreach (string icon in allIcons)
        {
            Log(icon);
        }
    }

    public static Icon GetIcon(string iconName)
    {
        if (IconLookup.TryGetValue(iconName, out Icon icon))
        {
            return icon;
        }

        Icon questionMark = PartCategorizer.Instance.iconLoader.GetIcon("stockIcon_fallback");
        if (questionMark != null)
        {
            return questionMark;
        }

        return Fallback;
    }
    
    private static Texture2D InvertTexture(Texture2D original)
    {
        Texture2D readableTex = CreateReadableTexture(original);
        
        Color[] pixels = readableTex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = 1.0f - pixels[i].r;
            pixels[i].g = 1.0f - pixels[i].g;
            pixels[i].b = 1.0f - pixels[i].b;
        }

        readableTex.SetPixels(pixels);
        readableTex.Apply();

        return readableTex;
    }

    private static Texture2D CreateReadableTexture(Texture2D unreadableTexture)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            unreadableTexture.width,
            unreadableTexture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(unreadableTexture, renderTex, new Vector2(1, 1), new Vector2(1, 1));
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableTex = new(unreadableTexture.width, unreadableTexture.height);
        readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        
        return readableTex;
    }
}

