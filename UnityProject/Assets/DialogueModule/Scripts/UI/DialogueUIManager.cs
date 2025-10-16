using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    public class DialogueUIManager : MonoBehaviour
    {
        [SerializeField]
        private List<MonoBehaviour> bindableSources = new List<MonoBehaviour>();
        [SerializeField]
        private DialogueEngine engine;

        private readonly List<IScenarioBindable> _bindables = new List<IScenarioBindable>();

        private void Awake()
        {
            CacheBindables();
            Bind();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void CacheBindables()
        {
            _bindables.Clear();

            foreach (var source in bindableSources)
            {
                if (source == null)
                {
                    Debug.LogWarning($"{name}: Missing reference in bindableSources list.");
                    continue;
                }

                if (source is IScenarioBindable bindable)
                {
                    _bindables.Add(bindable);
                }
                else
                {
                    Debug.LogWarning($"{source.name} does not implement IScenarioBindable.");
                }
            }
        }

        private void Bind()
        {
            foreach (var bindables in _bindables) 
            {
                bindables?.BindToScenario(engine.adapter);
            }
        }

        private void Unbind()
        {
            foreach (var bindables in _bindables)
            {
                bindables?.UnbindFromScenario(engine.adapter);
            }
        }
    }
}