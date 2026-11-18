using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Suggar Mayhem/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public GameObject previewPrefab; //Sprite para la pantalla de seleccion
    public GameObject battlePrefab; //Prefab jugable en batalla
    public bool isUnlocked = false;
}
