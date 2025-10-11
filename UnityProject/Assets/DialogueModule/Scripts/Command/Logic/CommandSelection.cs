using UnityEngine;

namespace DialogueModule
{
    class CommandSelection : CommandBase
    {
        string jumpLabel;
        public CommandSelection(GridInfo grid, StringGridRow row) : base(CommandID.Selection, row)
        {
            if(!DataParser.TryParseScenarioLabel(grid, row, ColumnName.Arg1, out jumpLabel))
                Debug.LogError($"Failed to parse scenario label from sheet name {grid.gridName}, Arg1 column, row {row}");
        }

        public override void Execute(DialogueEngine engine)
        {

        }
    }
}