using UnityEngine;

namespace DialogueModule
{
    class LayerItem : MonoBehaviour
    {
        public LayerSettings.Data data { get; set; }
        public int sortingOrder => (data?.order).GetValueOrDefault();

        public void Init(LayerSettings.Data data)
        {
            this.data = data;
            gameObject.name = data.layerName;
            var rectT = GetComponent<RectTransform>();
            rectT.anchoredPosition = new Vector2(data.x, data.y);
        }
    }
}