using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DialogueModule
{
    public class ScenarioUIAdapter
    {
        public event Action onPlayText;
        public event Action onSkipTypingText;
        public event Action onNextLine;
        public event Action<List<SelectionData>> onShowSelections;
        public readonly ObservableValue<string> currentLine = new ObservableValue<string>();
        public readonly CharacterAdapter characterAdapter = new CharacterAdapter();
        private List<SelectionData> selections = new List<SelectionData>();
        private ControllerStatus controllerStatus = ControllerStatus.None;

        public void PlayText(string fullText)
        {
            currentLine.Value = fullText;
            controllerStatus = ControllerStatus.TypingText;
            onPlayText?.Invoke();
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
                onNextLine?.Invoke();
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
            onShowSelections?.Invoke(list);
        }

        enum ControllerStatus
        {
            None,
            TypingText,
            WaitingInput,
        }
    }
}
