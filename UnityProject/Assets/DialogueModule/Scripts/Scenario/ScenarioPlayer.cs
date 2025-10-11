using System.Collections;
using UnityEngine;

namespace DialogueModule
{
    class ScenarioPlayer : MonoBehaviour
    {
        DialogueEngine engine;
        public LabelData currentLabelData { get; private set; }
        LabelData nextLabel; //TODO
        public bool isPlaying { get; set; }
        public bool isPaused { get; set; }
        public bool isLoading { get; private set; }

        internal void Init(DialogueEngine engine)
        {
            this.engine = engine;
        }

        internal void Clear()
        {
            isPlaying = false;
            StopAllCoroutines();
        }

        void End()
        {
            isPlaying = false;
            currentLabelData = null;
            engine.scenarioManager.EndScenario();
        }

        internal void StartScenario(LabelData labelData)
        {
            if (labelData.commands.Count == 0)
            {
                Debug.Log($"No command found in labelData {labelData.name}, will skip start scenario");
                return;
            }
            StartCoroutine(StartScenarioAsync(labelData));
        }

        IEnumerator StartScenarioAsync(LabelData labelData)
        {
            isPlaying = true;
            yield return new WaitUntil(() => !engine.isLoading);
            currentLabelData = labelData;
            do
            {
                var runLabelCoroutine = StartCoroutine(StartLabelData());
                if (runLabelCoroutine != null)
                    yield return runLabelCoroutine;

                currentLabelData = nextLabel;
                nextLabel = null;
            } while (currentLabelData != null);

            End();
        }

        IEnumerator StartLabelData()
        {
            foreach (var cmd in currentLabelData.commands)
            {
                cmd.Execute(engine);
                yield return new WaitUntil(() => !isPaused);
                yield return new WaitUntil(() => !cmd.isWaiting);
            }
        }

    }
}