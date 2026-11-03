using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
/// <summary> 
/// Notas de bugs:
/// Aun hay que corregir los rounds
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float maxTime = 99f;
    private float currentTime;

    [Header("Rounds and Wins")]
    [SerializeField] private int roundsToWin = 2;
    private int p1Wins = 0;
    private int p2Wins = 0;
    private bool roundActive = false;

    [Header("Players Tracking")]
    [SerializeField] private PlayerHealth player1Health;
    [SerializeField] private PlayerHealth player2Health;
    [SerializeField] private Transform player1Transform;
    [SerializeField] private Transform player2Transform;

    private Vector3 p1StartPos;
    private Vector3 p2StartPos;

    [Header("UI Manager")]
    [SerializeField] private Slider HPPlayer1;
    [SerializeField] private Slider HPPlayer2;

    [Header("Panels")]
    private GameObject panelVictory;
    private TextMeshProUGUI winnerTXT;

    [Header("References")]
    SceneManager scManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Guardar posiciones iniciales para el reset
        p1StartPos = player1Transform.position;
        p2StartPos = player2Transform.position;

        if (panelVictory != null) panelVictory.SetActive(false);

        StartNewRound();
    }

    #region Players Health & UI
    public void UpdateHealtBar(int playerID, float currentHealth, float maxHealth)
    {
        float healthPercentege = currentHealth = maxHealth;

        if (playerID == 1)
            HPPlayer1.value = healthPercentege;
        else if (playerID == 2)
            HPPlayer2.value = healthPercentege;
    }
    #endregion

    #region Game Flow
    private void StartNewRound()
    {
        // 1. Restaurar Posiciones
        player1Transform.position = p1StartPos;
        player2Transform.position = p2StartPos;

        // 2. Restaurar Vida
        player1Health.ResetHealth();
        player2Health.ResetHealth();

        // 3. Restaurar Timer
        currentTime = maxTime;
        timeText.text = currentTime.ToString("f0");

        roundActive = true;
        StartCoroutine(Timer());
    }

    public void PlayerDefeated(int defeatedPlayerID)
    {
        if (!roundActive) return;

        // Asignar victoria
        if (defeatedPlayerID == 1) p2Wins++;
        else if (defeatedPlayerID == 2) p1Wins++;

        EvaluateRoundEnd();
    }

    private void TimeOut()
    {
        Debug.Log("Tiempo Agotado, gano");
        EvaluateRoundEnd();
    }

    private void EvaluateRoundEnd()
    {
        roundActive = false;
        StopAllCoroutines(); // Detener el timer actual

        if (p1Wins >= roundsToWin)
        {
            EndGame(1);
        }
        else if (p2Wins >= roundsToWin)
        {
            EndGame(2);
        }
        else
        {
            StartCoroutine(RoundTransition());
        }
    }

    private IEnumerator RoundTransition()
    {
        // Aquí podrías mostrar un texto de "Round X... FIGHT!"
        yield return new WaitForSeconds(3f); // Esperar 3 segundos antes del siguiente round
        StartNewRound();
    }

    private void EndGame(int winnerID)
    {
        if (panelVictory != null) panelVictory.SetActive(true);
        if (winnerTXT != null) winnerTXT.text = $"¡Jugador {winnerID} Gana!";
        Debug.Log($"Game over. Ganador: Jugador {winnerID}");
    }
    #endregion

    #region Corutines
    private IEnumerator Timer()
    {
        while (currentTime > 0 && roundActive)
        {
            currentTime -= Time.deltaTime;
            timeText.text = currentTime.ToString("f0");
            yield return null; // Esperar al siguiente frame en lugar de saltar de segundo en segundo
        }

        if (currentTime <= 0 && roundActive)
        {
            timeText.text = "0";
            TimeOut();
        }
    }
    #endregion
}
