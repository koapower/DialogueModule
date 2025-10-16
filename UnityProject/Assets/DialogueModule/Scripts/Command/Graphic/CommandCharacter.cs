using UnityEngine;

namespace DialogueModule
{
    class CommandCharacter : CommandBase
    {
        public string characterId;
        public bool isHiding;
        public string layerName;
        public string textContent;
        public CharacterSettingData characterSettingData;

        public CommandCharacter(GridInfo grid, StringGridRow row) : base(CommandID.Character, row)
        {
            characterId = DataParser.GetCell(grid, row, ColumnName.Arg1);
            isHiding = DataParser.GetCell(grid, row, ColumnName.Arg2) == "Off";
            layerName = DataParser.GetCell(grid, row, ColumnName.Arg3);
            textContent = DataParser.GetCell(grid, row, ColumnName.Text);
        }

        public override void Execute(DialogueEngine engine)
        {
            if (!engine.dataManager.settingDataManager.characterSettings.DataDict.TryGetValue(characterId, out characterSettingData))
            {
                Debug.LogError($"Could not find character setting for character ID: {characterId}");
                return;
            }

            if (isHiding)
            {
                engine.adapter.characterAdapter.HideLayer(layerName);
            }
            else
            {
                Sprite sprite = null;
                if (!string.IsNullOrEmpty(characterSettingData.fileName))
                {
                    sprite = Resources.Load<Sprite>(characterSettingData.fileName); //will just use sync process for now
                }
                engine.adapter.characterAdapter.ShowCharacter(layerName, characterSettingData, sprite);
            }

            if (!string.IsNullOrEmpty(textContent))
            {
                engine.adapter.PlayText(characterSettingData.displayName, textContent);
                isWaiting = true;
            }
        }
    }
}