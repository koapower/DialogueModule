using UnityEngine;

namespace DialogueModule
{
    public class CharacterLayerData
    {
        public string LayerName { get; set; }
        public string CharacterId { get; set; }
        public string DisplayName { get; set; }
        public Sprite Sprite { get; set; }

        public string Expression { get; set; }
        public Vector2 Position { get; set; }
        public float Alpha { get; set; } = 1f;

        public CharacterLayerData(string layerName, string characterId, string displayName, Sprite sprite)
        {
            LayerName = layerName;
            CharacterId = characterId;
            DisplayName = displayName;
            Sprite = sprite;
        }

        public CharacterLayerData Clone()
        {
            return new CharacterLayerData(LayerName, CharacterId, DisplayName, Sprite)
            {
                Expression = Expression,
                Position = Position,
                Alpha = Alpha
            };
        }
    }
}