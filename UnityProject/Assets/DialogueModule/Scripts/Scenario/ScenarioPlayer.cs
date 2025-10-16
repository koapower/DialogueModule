using System.Collections;
using UnityEngine;

namespace DialogueModule
{
    class ScenarioPlayer : MonoBehaviour
    {
        DialogueEngine engine;
        public LabelData currentLabelData { get; private set; }
        public LabelData nextLabel { get; set; }
        public bool isPlaying { get; set; }
        public bool isPaused { get; set; }
        public bool isLoading { get; private set; }
        private CommandBase currentCommand;

        internal void Init(DialogueEngine engine)
        {
            this.engine = engine;
            engine.adapter.onNextLine += OnNextLine;
        }

        internal void Clear()
        {
            isPlaying = false;
            StopAllCoroutines();
            engine.adapter.onNextLine -= OnNextLine;
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
                yield return StartCoroutine(StartLabelData());

                currentLabelData = nextLabel;
                nextLabel = null;
            } while (currentLabelData != null);

            End();
        }

        IEnumerator StartLabelData()
        {
            foreach (var cmd in currentLabelData.commands)
            {
                currentCommand = cmd;
                currentCommand.Execute(engine);
                yield return new WaitUntil(() => !isPaused);
                yield return new WaitUntil(() => !currentCommand.isWaiting);
            }
        }

        void OnNextLine()
        {
            if (currentCommand != null)
                currentCommand.isWaiting = false;
        }
    }
}