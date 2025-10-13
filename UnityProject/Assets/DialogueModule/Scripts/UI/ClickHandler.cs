using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueModule
{
    class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public Action<PointerEventData> onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(eventData);
        }
    }
}
