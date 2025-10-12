using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueModule
{
    public enum CharacterLayerEventType
    {
        Show,
        Hide,
        Update,
    }

    public class CharacterLayerEvent
    {
        public CharacterLayerEventType EventType { get; }
        public CharacterLayerData Data { get; }

        public CharacterLayerEvent(CharacterLayerEventType eventType, CharacterLayerData data)
        {
            EventType = eventType;
            Data = data;
        }
    }

    public class CharacterAdapter
    {
        public event Action<CharacterLayerEvent> OnLayerChanged;

        private readonly Dictionary<string, CharacterLayerData> _layers = new Dictionary<string, CharacterLayerData>();

        public void ShowCharacter(string layerName, string characterId, string displayName, Sprite sprite)
        {
            if (_layers.ContainsKey(layerName))
            {
                HideLayer(layerName);
            }

            var data = new CharacterLayerData(layerName, characterId, displayName, sprite);
            _layers[layerName] = data;

            InvokeEvent(CharacterLayerEventType.Show, data);
        }

        public void HideLayer(string layerName)
        {
            if (!_layers.TryGetValue(layerName, out var data))
                return;

            InvokeEvent(CharacterLayerEventType.Hide, data);

            _layers.Remove(layerName);
        }

        public void HideAll()
        {
            var layerNames = new List<string>(_layers.Keys);
            foreach (var layerName in layerNames)
            {
                HideLayer(layerName);
            }
        }

        public void UpdateLayer(string layerName, Action<CharacterLayerData> updateAction)
        {
            if (!_layers.TryGetValue(layerName, out var data))
            {
                Debug.LogWarning($"Layer '{layerName}' does not have a character.");
                return;
            }

            updateAction?.Invoke(data);

            InvokeEvent(CharacterLayerEventType.Update, data);
        }

        public CharacterLayerData GetLayer(string layerName)
        {
            return _layers.TryGetValue(layerName, out var data) ? data.Clone() : null;
        }
        
        public CharacterLayerData GetLayerByCharacterId(string characterId)
        {
            foreach (var layerData in _layers.Values)
            {
                if (layerData.CharacterId == characterId)
                    return layerData;
            }
            return null;
        }

        public bool HasCharacterOnLayer(string layerName)
        {
            return _layers.ContainsKey(layerName);
        }

        public IEnumerable<string> GetAllLayerNames()
        {
            return _layers.Keys;
        }

        public Dictionary<string, CharacterLayerData> GetAllLayers()
        {
            var result = new Dictionary<string, CharacterLayerData>();
            foreach (var kvp in _layers)
            {
                result[kvp.Key] = kvp.Value.Clone();
            }
            return result;
        }

        public void Clear()
        {
            _layers.Clear();
        }

        private void InvokeEvent(CharacterLayerEventType eventType, CharacterLayerData data)
        {
            var evt = new CharacterLayerEvent(eventType, data.Clone());
            OnLayerChanged?.Invoke(evt);
        }
    }
}