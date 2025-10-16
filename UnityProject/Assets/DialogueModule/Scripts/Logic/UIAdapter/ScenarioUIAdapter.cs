using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    public class ScenarioUIAdapter : MonoBehaviour
    {
        public event Action<InitData> onInit;
        public event Action<MessageData> onPlayText;
        public event Action onSkipTypingText;
        public event Action onNextLine;
        public event Action<List<SelectionData>, Action<string>> onShowSelections;
        public readonly ObservableValue<string> currentLine = new ObservableValue<string>();
        public readonly CharacterAdapter characterAdapter = new CharacterAdapter();
        private List<SelectionData> selections = new List<SelectionData>();
        private ControllerStatus controllerStatus = ControllerStatus.None;

        public void Init(InitData initData)
        {
            onInit?.Invoke(initData);
        }

        public void PlayText(string characterDisplayName, string fullText)
        {
            currentLine.Value = fullText;
            controllerStatus = ControllerStatus.TypingText;
            onPlayText?.Invoke(new MessageData() { name = characterDisplayName, message = fullText });
        }

        public void PlayTextEnd()
        {
            controllerStatus = ControllerStatus.WaitingInput;
        }

        public void OnNext()
        {
            if (controllerStatus == ControllerStatus.TypingText)
                onSkipTypingText?.Invoke();
            else if (controllerStatus == ControllerStatus.WaitingInput)
            {
                controllerStatus = ControllerStatus.None;
                onNextLine?.Invoke();
            }
        }

        internal void AddSelection(string jumpLabel, string textContent)
        {
            selections.Add(new SelectionData()
            {
                jumpLabel = jumpLabel,
                textContent = textContent
            });
        }

        public void ShowSelections()
        {
            var list = new List<SelectionData>(selections);
            selections.Clear();
            onShowSelections?.Invoke(list, ChooseSelection);
        }

        public void ChooseSelection(string jumpLabel)
        {
            var engine = GetComponent<DialogueEngine>();
            var labelData = engine.dataManager.GetLabelData(jumpLabel);
            if (labelData == null)
            {
                Debug.LogError($"Failed to find jump label name: {jumpLabel}!");
                return;
            }
            engine.scenarioManager.SetNextLabel(labelData);
        }

        enum ControllerStatus
        {
            None,
            TypingText,
            WaitingInput,
        }
    }
}
