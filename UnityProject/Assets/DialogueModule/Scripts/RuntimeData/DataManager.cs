using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    class DataManager : MonoBehaviour
    {
        [SerializeField] private SettingsBook settingsBook;
        [SerializeField] private ScenarioBook scenarioBook;

        SettingDataManager settingDataManager = new SettingDataManager();
        Dictionary<string, ScenarioData> scenarioDataDict = new Dictionary<string, ScenarioData>();

        public void Init()
        {
            settingDataManager.Init(settingsBook);
            InitScenarios();
        }

        private void InitScenarios()
        {
            scenarioDataDict.Clear();
            foreach (var grid in scenarioBook.ScenarioData.Values)
            {
                var scenarioData = new ScenarioData(grid);
                scenarioDataDict[grid.Name] = scenarioData;
            }
        }

        public LabelData GetLabelData(string label)
        {
            foreach (var sData in scenarioDataDict.Values)
            {
                if (sData.ScenarioLabelDict.TryGetValue(label, out var labelData))
                {
                    return labelData;
                }
            }

            return null;
        }
    }
}