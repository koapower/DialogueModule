using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    [CreateAssetMenu(fileName = "SettingsBook", menuName = "Dialogue/Settings Book")]
    public class SettingsBook : ScriptableObject
    {
    [SerializeField]
    private Dictionary<string, StringGridDictionary> settings = new Dictionary<string, StringGridDictionary>();

    public void SetSettings(string key, StringGridDictionary data)
    {
        if (settings.ContainsKey(key))
        {
            settings[key] = data;
        }
        else
        {
            settings.Add(key, data);
        }
    }

    public bool TryGetSettings(string key, out StringGridDictionary data)
    {
        return settings.TryGetValue(key, out data);
    }

    public void ClearSettings()
    {
        settings.Clear();
    }

    public int GetSettingsCount(string key)
    {
        if (settings.TryGetValue(key, out StringGridDictionary data))
        {
            return data.Count;
        }
        return 0;
    }

    public int TotalSettingsTypes => settings.Count;
    }
}
