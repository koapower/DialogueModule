using System;
using System.Linq;
using UnityEngine;

namespace DialogueModule
{
    [AddComponentMenu("Dialogue Module/Dialogue Engine")]
    public class DialogueEngine : MonoBehaviour
    {
        internal DataManager dataManager => GetComponent<DataManager>();
        internal ScenarioManager scenarioManager => GetComponent<ScenarioManager>();
        public ScenarioUIAdapter adapter => GetComponent<ScenarioUIAdapter>();
        public bool isLoading => scenarioManager.isLoading;

        private void Awake()
        {
            //debug
            Init();
        }

        public void Init()
        {
            dataManager.Init();
            adapter.Init(PrepareExposedInitData());
        }

        public void StartDialogue(string label)
        {
            if (label.Length > 1 && label[0] == '*')
            {
                label = label.Substring(1);
            }
            scenarioManager.StartScenario(label);
        }

        private InitData PrepareExposedInitData()
        {
            var result = new InitData();
            var layerSettings = dataManager.settingDataManager.layerSettings;
            result.DEFAULT_LAYER_NAME = LayerSettings.DEFAULT_LAYER_NAME;
            result.layerSettingDatas = layerSettings.DataDict.Values.ToList();
            
            return result;
        }
    }
}