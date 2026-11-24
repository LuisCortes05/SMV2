using UnityEngine;
using UnityEngine.InputSystem;

public class ArenaMatchSetup : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform spawnPointP1; // Colócalo a la izquierda de la arena
    [SerializeField] private Transform spawnPointP2; // Colócalo a la derecha de la arena

    [Header("Fallback Prefabs (Por si pruebas Arena directamente)")]
    [SerializeField] private Characters defaultCharacterP1;
    [SerializeField] private Characters defaultCharacterP2;

    [Header("Scene References")]
    [SerializeField] private GameManager gameManager;

    [Header("Orientation")]
    [Tooltip("Actívalo si tus personajes son 2D y deben voltear su escala X para mirar a la izquierda")]
    [SerializeField] private bool flipP2ScaleX = true;

    private void Awake()
    {
        Characters charP1 = MatchData.selectedCharacters[0] != null
            ? MatchData.selectedCharacters[0]
            : defaultCharacterP1;

        Characters charP2 = MatchData.selectedCharacters[1] != null
            ? MatchData.selectedCharacters[1]
            : defaultCharacterP2;

        // 1. Instanciar personajes en sus respectivas posiciones (Izquierda P1, Derecha P2)
        GameObject p1Instance = Instantiate(charP1.characterPrefab, spawnPointP1.position, spawnPointP1.rotation);
        GameObject p2Instance = Instantiate(charP2.characterPrefab, spawnPointP2.position, spawnPointP2.rotation);

        p1Instance.name = $"P1_{charP1.characterName}";
        p2Instance.name = $"P2_{charP2.characterName}";

        // Orientar al Player 2 hacia la izquierda
        if (flipP2ScaleX)
        {
            Vector3 scale = p2Instance.transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            p2Instance.transform.localScale = scale;
        }
        else
        {
            p2Instance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        // 2. Configurar PlayerHealth y enlazar con GameManager[cite: 10, 20]
        PlayerHealth p1Health = p1Instance.GetComponent<PlayerHealth>();
        PlayerHealth p2Health = p2Instance.GetComponent<PlayerHealth>();

        p1Health.playerID = 1;
        p1Health.gm = gameManager;

        p2Health.playerID = 2;
        p2Health.gm = gameManager;

        // 3. Cruzar referencias de daño entre rivales en Personaje[cite: 11, 16]
        Personaje p1Combat = p1Instance.GetComponent<Personaje>();
        Personaje p2Combat = p2Instance.GetComponent<Personaje>();

        if (p1Combat != null) p1Combat.SetRivalHealth(p2Health);
        if (p2Combat != null) p2Combat.SetRivalHealth(p1Health);

        // 4. Registrar jugadores en el GameManager antes de StartNewRound()[cite: 10]
        gameManager.RegisterPlayers(p1Health, p2Health, p1Instance.transform, p2Instance.transform, charP1, charP2);

        // 5. Asignar los mandos de Xbox a cada personaje instanciado[cite: 7]
        PlayerInput p1Input = p1Instance.GetComponent<PlayerInput>();
        PlayerInput p2Input = p2Instance.GetComponent<PlayerInput>();

        if (p1Input != null && Gamepad.all.Count >= 1)
            p1Input.SwitchCurrentControlScheme("Gamepad", Gamepad.all[0]);

        if (p2Input != null && Gamepad.all.Count >= 2)
            p2Input.SwitchCurrentControlScheme("Gamepad", Gamepad.all[1]);
    }
}
