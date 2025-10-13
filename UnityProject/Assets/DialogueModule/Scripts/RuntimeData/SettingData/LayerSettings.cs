using System.Collections.Generic;

namespace DialogueModule
{
    class LayerSettings
    {
        public const string DEFAULT_LAYER_NAME = "Default Layer";
        internal IReadOnlyDictionary<string, Data> DataDict => dataDict;
        Dictionary<string, Data> dataDict = new Dictionary<string, Data>();

        public void Init(StringGridDictionary settings)
        {
            dataDict.Clear();
            var defaultLayerData = new Data() 
            {
                layerName = DEFAULT_LAYER_NAME,
                type = "Character",
                x = 0,
                y = 0,
                order = 0,
            };
            dataDict.Add(string.Empty, defaultLayerData); //use string empty as key
            foreach (var grid in settings.Values)
            {
                var headerRow = grid.GetRow(0);
                if (headerRow == null)
                    throw new System.Exception($"header row is null for grid {grid.Name}");
                for (int i = 1; i < grid.RowCount; i++)
                {
                    var row = grid.GetRow(i);
                    if (row.IsEmpty || row.IsCommentOut)
                        continue;
                    var d = new Data(headerRow, row);
                    dataDict.Add(d.layerName, d);
                }
            }
        }

        public class Data
        {
            public string layerName;
            public string type;
            public float x;
            public float y;
            public int order;

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
                        case "LayerName":
                            layerName = value;
                            break;
                        case "Type":
                            type = value;
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
                            if (!int.TryParse(value, out order))
                                throw new System.InvalidCastException($"Error from Character sheet: header {header}, cell {value}, row {row}");
                            break;
                        default:
                            break;
                    }
                }
            }

            internal Data() { }
        }
    }
}