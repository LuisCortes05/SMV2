using UnityEngine;

public class GameSessionData : MonoBehaviour
{
    public static GameSessionData Instance { get; private set; }
    public CharacterData p1Character;
    public CharacterData p2Character;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelections(CharacterData p1, CharacterData p2)
    {
        p1Character = p1;
        p2Character = p2;
    }
}
