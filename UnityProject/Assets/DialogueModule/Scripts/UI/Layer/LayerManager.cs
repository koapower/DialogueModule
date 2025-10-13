using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace DialogueModule
{
    class LayerManager : MonoBehaviour
    {
        [SerializeField] private LayerItem bgLayer;
        [SerializeField] private LayerItem defaultLayer;
        private Dictionary<string, LayerItem> layerItemDict = new Dictionary<string, LayerItem>(); // doesn't include bg layer
        private List<LayerItem> layerItems = new List<LayerItem>();

        public void Init(LayerSettings settings)
        {
            layerItemDict[LayerSettings.DEFAULT_LAYER_NAME] = defaultLayer;
            layerItems.Add(defaultLayer);
            foreach (var d in settings.DataDict.Values)
            {
                var newItem = Instantiate(defaultLayer);
                newItem.Init(d);
                layerItemDict[d.layerName] = newItem;
                layerItems.Add(newItem);
            }
            UpdateLayerOrders();
        }

        public void UpdateLayerOrders()
        {
            layerItems.Sort((a, b) => a.sortingOrder.CompareTo(b.sortingOrder));

            for (int i = 0; i < layerItems.Count; i++)
                layerItems[i].transform.SetSiblingIndex(i);
        }
    }
}