using UnityEngine;

namespace CustomCategories.Data;

public static class Utils
{
    public static void Log(string message)
    {
        Debug.Log("[CustomCategories] " + message);
    }
    
    public static string ToTitleCase(string text)
    {
        return text.Length switch
        {
            0 => text,
            1 => text.ToUpper(),
            _ => text[0].ToString().ToUpper() + text.Substring(1)
        };
    }
}