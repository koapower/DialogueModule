using UnityEngine;

namespace DialogueModule
{
    class CommandCharacter : CommandBase
    {
        public string characterId;
        public bool isHiding;
        public string layerName;
        public string textContent;

        public CommandCharacter(GridInfo grid, StringGridRow row) : base(CommandID.Character, row)
        {
            characterId  = DataParser.GetCell(grid, row, ColumnName.Arg1);
            isHiding = DataParser.GetCell(grid, row, ColumnName.Arg2) == "Off";
            layerName = DataParser.GetCell(grid, row, ColumnName.Arg3);
            textContent = DataParser.GetCell(grid, row, ColumnName.Text);
        }

        public override void Execute(DialogueEngine engine)
        {
            if(isHiding)
            {
                engine.adapter.characterAdapter.HideLayer(layerName);
            }
            else
            {
                if (!engine.dataManager.settingDataManager.characterSettings.DataDict.TryGetValue(characterId, out var charSettings))
                {
                    Debug.LogError($"Could not find character setting for character ID: {characterId}");
                    return;
                }
                Sprite sprite = null;
                if (!string.IsNullOrEmpty(charSettings.fileName))
                {
                    sprite = Resources.Load<Sprite>(charSettings.fileName); //will just use sync process for now
                }
                engine.adapter.characterAdapter.ShowCharacter(layerName, characterId, charSettings.displayName, sprite);
            }

            if(!string.IsNullOrEmpty(textContent))
            {
                engine.adapter.PlayText(textContent);
                isWaiting = true;
            }
        }
    }
}