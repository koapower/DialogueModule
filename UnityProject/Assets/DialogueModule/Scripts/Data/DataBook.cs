using UnityEngine;

[CreateAssetMenu(fileName = "DataBook", menuName = "Dialogue/Data Book")]
public class DataBook : ScriptableObject
{
    [SerializeField]
    private StringGridDictionary characterData = new StringGridDictionary();

    public StringGridDictionary CharacterData => characterData;

    public void SetCharacterData(StringGridDictionary data)
    {
        characterData = data;
    }

    public bool TryGetCharacter(string characterName, out StringGrid grid)
    {
        return characterData.TryGetValue(characterName, out grid);
    }

    public int CharacterCount => characterData.Count;
}
