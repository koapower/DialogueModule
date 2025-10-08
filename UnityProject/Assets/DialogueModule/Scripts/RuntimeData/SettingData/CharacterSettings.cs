using System.Collections.Generic;

namespace DialogueModule
{
    class CharacterSettings
    {
        internal IReadOnlyDictionary<string, Data> DataDict => dataDict;
        Dictionary<string, Data> dataDict = new Dictionary<string, Data>();

        public void Init(StringGridDictionary characterSettings)
        {
            dataDict.Clear();
            foreach (var grid in characterSettings.Values)
            {
                var headerRow = grid.GetRow(0);
                if (headerRow == null)
                    throw new System.Exception($"header row is null for grid {grid.Name}");
                for (int i = 1; i < grid.RowCount; i++)
                {
                    var row = grid.GetRow(i);
                    if(row.IsEmpty || row.IsCommentOut)
                        continue;
                    var d = new Data(headerRow, row);
                    dataDict.Add(d.characterID, d);
                }
            }
        }

        public class Data
        {
            public string characterID;
            public string displayName;
            public float x;
            public float y;
            public float scale;
            public string fileName;

            public Data(StringGridRow headerRow, StringGridRow row)
            {
                for (int i = 0; i < headerRow.Length; i++)
                {
                    var header = headerRow.GetCell(i);
                    if (string.IsNullOrEmpty(header))
                        continue;
                    var value = row.GetCell(i);
                    if (string.IsNullOrEmpty(value))
                        continue;
                    switch (header)
                    {
                        case "CharacterName":
                            characterID = value;
                            break;
                        case "DisplayName":
                            displayName = value;
                            break;
                        case "X":
                            if (!float.TryParse(value, out x))
                                throw new System.InvalidCastException($"Error from Character sheet: header {header}, cell {value}, row {row}");
                            break;
                        case "Y":
                            if (!float.TryParse(value, out y))
                                throw new System.InvalidCastException($"Error from Character sheet: header {header}, cell {value}, row {row}");
                            break;
                        case "Scale":
                            if (!float.TryParse(value, out scale))
                                throw new System.InvalidCastException($"Error from Character sheet: header {header}, cell {value}, row {row}");
                            break;
                        case "FileName":
                            fileName = value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}