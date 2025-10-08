using System;
using UnityEngine;

namespace DialogueModule
{
    class ScenarioManager : MonoBehaviour
    {
        public event Action onBeginScenario;
        public event Action onEndScenario;
        public event Action onPauseScenario;
        public event Action onEndOrPauseScenario;
        public bool isLoading => mainPlayer.isLoading;
        ScenarioPlayer mainPlayer;

        internal void StartScenario(string label)
        {
            ClearMainPlayer();
            CreateMainPlayer();
            onBeginScenario?.Invoke();
            mainPlayer.StartScenario(label);
        }

        private void ClearMainPlayer()
        {
            if (mainPlayer == null)
                return;

            mainPlayer.Clear();
            Destroy(mainPlayer);
            mainPlayer = null;
        }

        private void CreateMainPlayer()
        {
            var go = new GameObject("MainScenarioPlayer");
            var sp = go.AddComponent<ScenarioPlayer>();
            go.transform.parent = transform;
            mainPlayer = sp;
            sp.Init(GetComponent<DialogueEngine>());
        }
    }
}