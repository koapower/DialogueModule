using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    class SelectionManager : MonoBehaviour
    {
        [SerializeField] private SelectionItem itemPrefab;
        private List<SelectionItem> selectionItems = new List<SelectionItem>();

        private void Awake()
        {
            itemPrefab.gameObject.SetActive(false);
        }

        private void ClearAll()
        {
            foreach (var item in selectionItems)
            {
                GameObject.Destroy(item);
            }
            selectionItems.Clear();
        }

        public void UpdateItems(List<SelectionData> datas, Action<string> chooseAction)
        {
            ClearAll();
            foreach (var d in datas)
            {
                var s = Instantiate(itemPrefab);
                s.content.text = d.textContent;
                s.btn.onClick.RemoveAllListeners();
                s.btn.onClick.AddListener(() =>
                {
                    chooseAction?.Invoke(d.jumpLabel);
                    s.btn.onClick.RemoveAllListeners();
                });
                selectionItems.Add(s);
            }
        }
    }
}