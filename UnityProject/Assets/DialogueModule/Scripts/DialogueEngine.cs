using System;
using UnityEngine;

namespace DialogueModule
{
    [AddComponentMenu("Dialogue Module/Dialogue Engine")]
    public class DialogueEngine : MonoBehaviour
    {
        public event Action onUIStart;
        internal DataManager dataManager => GetComponent<DataManager>();
        internal ScenarioManager scenarioManager => GetComponent<ScenarioManager>();
        public bool isLoading => scenarioManager.isLoading;

        private void Awake()
        {
            //debug
            Init();
        }

        public void Init()
        {
            dataManager.Init();
        }

        public void StartDialogue(string label)
        {
            onUIStart?.Invoke();
            if (label.Length > 1 && label[0] == '*')
            {
                label = label.Substring(1);
            }
            scenarioManager.StartScenario(label);
        }
    }
}