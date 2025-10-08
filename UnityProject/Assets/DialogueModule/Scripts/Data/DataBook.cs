using UnityEngine;

[CreateAssetMenu(fileName = "DataBook", menuName = "Dialogue/Data Book")]
public class DataBook : ScriptableObject
{
    [SerializeField]
    private StringGridDictionary characterData = new StringGridDictionary();

    [SerializeField]
    private StringGridDictionary layerData = new StringGridDictionary();

    public StringGridDictionary CharacterData => characterData;
    public StringGridDictionary LayerData => layerData;

    public void SetCharacterData(StringGridDictionary data)
    {
        characterData = data;
    }

    public void SetLayerData(StringGridDictionary data)
    {
        layerData = data;
    }

    public bool TryGetCharacter(string characterName, out StringGrid grid)
    {
        return characterData.TryGetValue(characterName, out grid);
    }

    public bool TryGetLayer(string layerName, out StringGrid grid)
    {
        return layerData.TryGetValue(layerName, out grid);
    }

    public int CharacterCount => characterData.Count;
    public int LayerCount => layerData.Count;
}
