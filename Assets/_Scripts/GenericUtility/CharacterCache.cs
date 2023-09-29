using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterCustomizer;

public static class CharacterCache
{
    private static Dictionary<string, string> cache;

    public static void Setup()
    {
        CharacterCache.cache = new Dictionary<string, string>();
    }

    public static void UpdateCache(string username, string settingsJSON)
    {
        CharacterCache.cache[username] = settingsJSON;
    }

    public static string GetCachedSettings(string username)
    {
        if (CharacterCache.cache.ContainsKey(username))
        {
            return CharacterCache.cache[username];
        }
        else
        {
            Debug.LogError("Username " + username + " was not found in the cache.");
            return string.Empty;
        }        
    }

    public static bool IsCached(string username)
    {
        return CharacterCache.cache.ContainsKey(username);
    }
}
