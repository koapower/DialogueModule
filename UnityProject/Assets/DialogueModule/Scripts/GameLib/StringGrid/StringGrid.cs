using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    [System.Serializable]
    public class StringGrid
    {
        public string Name { get; set; }
        public List<StringGridRow> Rows => rows;
        public int RowCount => rows.Count;

        [SerializeField]
        private List<StringGridRow> rows = new List<StringGridRow>();

        public StringGrid(string name = "")
        {
            Name = name;
        }

        public void AddRow(StringGridRow row)
        {
            rows.Add(row);
        }

        public StringGridRow GetRow(int index)
        {
            if (index < 0 || index >= rows.Count)
                return null;
            return rows[index];
        }

        public void Clear()
        {
            rows.Clear();
        }
    }
}