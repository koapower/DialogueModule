using System.Collections;
using UnityEngine;

namespace DialogueModule
{
    class ScenarioPlayer : MonoBehaviour
    {
        DialogueEngine engine;
        public bool isPlaying {  get; set; }
        public bool isLoading { get; private set; }

        internal void Init(DialogueEngine engine)
        {
            this.engine = engine;
        }

        internal void StartScenario(string label)
        {
            StartCoroutine(StartScenarioAsync(label));
        }

        IEnumerator StartScenarioAsync(string label)
        {
            isPlaying = true;
            while (engine.isLoading) 
            {
                yield return null;
            }
            //todo 
        }

        internal void Clear()
        {
            isPlaying = false;
            //ResetOnJump();
            //WaitManager.Clear();
            //jumpManager.Clear();
            //CurrentLabelData = null;
            StopAllCoroutines();
        }
    }
}