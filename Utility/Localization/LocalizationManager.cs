using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SuspiciousGames.SellMe.Utility.Localization
{
    public static class LocalizationManager
    {
        private static HashSet<SystemLanguage> _supportedLanguages;
        private static Dictionary<string, string> _localizedText;
        private static string _missingTextString = "Localized text not found";
        private static SystemLanguage _currentSystemLanguage = SystemLanguage.Unknown;

        public static bool IsReady { get; private set; } = false;

        public static void Init()
        {
            _localizedText = new Dictionary<string, string>();
            _supportedLanguages = new HashSet<SystemLanguage>();

            var info = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources/Localization/"));

            foreach (var file in info.GetFiles())
            {
                _supportedLanguages.Add((SystemLanguage)Enum.Parse(typeof(SystemLanguage), file.Name.Split('.')[0]));
            }

            LoadLocalizedText(Application.systemLanguage);
        }

        public static void LoadLocalizedText(SystemLanguage systemLanguage)
        {
            if (_currentSystemLanguage == systemLanguage && systemLanguage != SystemLanguage.Unknown)
            {
                Debug.Log($"The language {systemLanguage} has been loaded already!");
                return;
            }

            if (systemLanguage == SystemLanguage.Unknown || !_supportedLanguages.Contains(systemLanguage))
            {
                Debug.Log($"The language {systemLanguage} is not supported, loading english...");
                systemLanguage = SystemLanguage.English;
            }

            _localizedText = new Dictionary<string, string>();
            string filePath = Path.Combine(Application.dataPath, "Resources/Localization/" + systemLanguage + ".json");

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

                if(loadedData == null)
                {
                    Debug.LogError("File is empty! No localization text set.");
                    return;
                }

                for (int i = 0; i < loadedData.items.Length; i++)
                {
                    _localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
                }

                Debug.Log("Data loaded, dictionary contains: " + _localizedText.Count + " entries");
            }
            else
            {
                Debug.LogError("Cannot find file!");
            }

            IsReady = true;
        }

        public static string GetLocalizedValue(string key)
        {
            string result = _missingTextString;
            if (_localizedText.ContainsKey(key))
            {
                result = _localizedText[key];
            }

            return result;
        }
    }
}