using System;

namespace DialogueModule
{
    public class ScenarioUIAdapter
    {
        public event Action onPlayText;
        public event Action onSkipTypingText;
        public event Action onNextLine;
        public readonly ObservableValue<string> currentLine  = new ObservableValue<string>();
        public readonly CharacterAdapter characterAdapter = new CharacterAdapter();
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
            if(controllerStatus == ControllerStatus.TypingText)
                onSkipTypingText?.Invoke();
            else if (controllerStatus == ControllerStatus.WaitingInput)
                onNextLine?.Invoke();
        }

        enum ControllerStatus
        {
            None,
            TypingText,
            WaitingInput,
        }
    }
}
