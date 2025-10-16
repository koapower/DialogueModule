using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueModule
{
    class ClickHandler : MonoBehaviour, IPointerClickHandler, IScenarioBindable
    {
        ScenarioUIAdapter adapter;

        public void BindToScenario(ScenarioUIAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void UnbindFromScenario(ScenarioUIAdapter adapter)
        {
            this.adapter = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            adapter.OnNext();
        }

    }
}
