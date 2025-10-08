using UnityEngine;

namespace DialogueModule
{
    class DataManager : MonoBehaviour
    {
        [SerializeField] private SettingsBook settingsBook;
        [SerializeField] private ScenarioBook scenarioBook;

        SettingDataManager settingDataManager = new SettingDataManager();

        public void Init()
        {
            settingDataManager.Init(settingsBook);
        }


    }
}