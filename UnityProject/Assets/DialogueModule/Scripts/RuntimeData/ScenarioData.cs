using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DialogueModule
{
    class ScenarioData
    {
        public string name { get; private set; }
        public StringGrid grid { get; private set; }
        private List<CommandBase> commandList = new List<CommandBase>();
        private Dictionary<string, LabelData> scenarioLabelDict = new Dictionary<string, LabelData>();

        public ScenarioData(StringGrid grid)
        {
            name = grid.Name;
            this.grid = grid;
            Init();
        }

        private void Init()
        {
            UpdateCommandList();
            UpdateLabelDict();
        }

        private void UpdateCommandList()
        {
            commandList.Clear();
            var gridInfo = grid.CreateGridInfo();
            var firstDataRowIndex = grid.GetFirstDataRowIndex();
            for (int i = firstDataRowIndex; i < grid.Rows.Count; i++)
            {
                var row = grid.Rows[i];
                var command = DataParser.ParseCommand(gridInfo, row);
                if (command != null)
                    commandList.Add(command);
            }
        }

        private void UpdateLabelDict()
        {
            scenarioLabelDict.Clear();
            if (commandList.Count == 0)
                return;

            //data before first label is labeled as the sheet's name.
            var label = name;
            var commandIndex = 0;
            var commandListForThisLabel = new List<CommandBase>();
            do
            {
                while (commandIndex < commandList.Count)
                {
                    var cmd = commandList[commandIndex];
                    if (cmd is CommandScenarioLabel) //found next label
                        break;
                    commandListForThisLabel.Add(cmd);
                    commandIndex++;
                }
                if (scenarioLabelDict.ContainsKey(label))
                    Debug.LogError($"Duplicated scenario label {label} at sheet {name}. please avoid creating labels with same names.");
                else
                {
                    var labelData = new LabelData(label, commandListForThisLabel);
                    scenarioLabelDict[label] = labelData;
                }

                if (commandIndex >= commandList.Count)
                    break;

                commandListForThisLabel = new List<CommandBase>();
                var labelCmd = commandList[commandIndex] as CommandScenarioLabel;
                label = labelCmd.labelName;
                commandIndex++;
            } while (commandIndex < commandList.Count);
        }
    }
}