using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
/// <summary> 
/// Notas de bugs:
/// Aun hay que corregir los rounds
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables
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
    [SerializeField] private GameObject panelVictory;
    [SerializeField] private TextMeshProUGUI winnerTXT;
    [SerializeField] private GameObject panelRoundWinner;
    [SerializeField] private TextMeshProUGUI textRoundWinner;

    [Header("References")]
    SceneManager scManager;

    [Header("KO Effect")]
    [SerializeField] private KOEffect koEffect;
    private Coroutine timerCoroutine;
    #endregion

    [Header("Debug / Testing")]
    [SerializeField] private float testDamageAmount = 25f;

#if UNITY_EDITOR
    private void Update()
    {
        // Presiona la tecla K en Play Mode para quitarle vida al Player 2
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            DamagePlayer2ForTest();
        }
    }
#endif

    /// <summary>
    /// Resta vida al Player 2. Se puede asignar al OnClick() de un Button en el Canvas.
    /// </summary>
    public void DamagePlayer2ForTest()
    {
        if (!roundActive || player2Health == null) return;

        player2Health.TakeDamage(testDamageAmount);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Guardar posiciones iniciales para el reset
        p1StartPos = player1Transform.position;
        p2StartPos = player2Transform.position;

        if (panelVictory != null) panelVictory.SetActive(false);
        if (panelRoundWinner != null) panelRoundWinner.SetActive(false);

        StartNewRound();
    }

    #region Players Health & UI
    public void UpdateHealthBar(int playerID, float currentHealth, float maxHealth)
    {
        if (playerID == 1)
        {
            HPPlayer1.maxValue = maxHealth;
            HPPlayer1.value = currentHealth;
        }
        else if (playerID == 2)
        {
            HPPlayer2.maxValue = maxHealth;
            HPPlayer2.value = currentHealth;
        }
    }
    #endregion

    #region Game Flow
    private void StartNewRound()
    {
        if (koEffect != null) koEffect.StopKOEffect(); // Detener efecto KO si está activo
        // Restaurar Posiciones
        player1Transform.position = p1StartPos;
        player2Transform.position = p2StartPos;

        // Restaurar Vida
        player1Health.ResetHealth();
        player2Health.ResetHealth();

        // Restaurar Timer
        currentTime = maxTime;
        timeText.text = currentTime.ToString("f0");

        roundActive = true;
        timerCoroutine = StartCoroutine(Timer());
    }

    public void PlayerDefeated(int defeatedPlayerID)
    {
        if (!roundActive) return;

        if(koEffect != null)
        {
            Vector3 defeatedPos = (defeatedPlayerID == 1)
                ? player1Transform.position
                : player2Transform.position;
            koEffect.PlayKOEffect(defeatedPos);
        }

        int roundWinnerID = 0;

        // Asignar victoria
        if (defeatedPlayerID == 1)
        {
            p2Wins++;
            roundWinnerID = 2;
        } 
        else if (defeatedPlayerID == 2)
        {
            p1Wins++;
            roundWinnerID = 1;
        }

        EvaluateRoundEnd(roundWinnerID);
    }

    private void TimeOut()
    {
        int roundWinnerID = 0;

        if (player1Health.CurrentHealth > player2Health.CurrentHealth)
        {
            p1Wins++;
            roundWinnerID = 1;
        }
        else if (player2Health.CurrentHealth > player1Health.CurrentHealth)
        {
            p2Wins++;
            roundWinnerID = 2;
        }
        EvaluateRoundEnd(roundWinnerID);
    }

    private void EvaluateRoundEnd(int roundWinnerID)
    {
        roundActive = false;

        if(timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

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
            StartCoroutine(RoundTransition(roundWinnerID));
        }
    }

    private void EndGame(int winnerID)
    {
        if (panelRoundWinner != null) panelRoundWinner.SetActive(false);
        if (panelVictory != null) panelVictory.SetActive(true);
        if (winnerTXT != null) winnerTXT.text = $"¡Jugador {winnerID} Gana!";
        Debug.Log($"Game over. Ganador: Jugador {winnerID}");
    }
    #endregion

    #region Corutines
    private IEnumerator RoundTransition(int roundWinnerID)
    {
        if(panelRoundWinner != null)
        {
            panelRoundWinner.SetActive(true);
            if(textRoundWinner != null)
            {
                textRoundWinner.text = (roundWinnerID == 0) 
                    ? "¡Empate!" 
                    : $"¡Jugador {roundWinnerID} gana el round!";
            }
        }
        yield return new WaitForSeconds(3f); // Esperar 3 segundos antes del siguiente round
        if (panelRoundWinner != null) panelRoundWinner.SetActive(false);
        StartNewRound();
    }
    private IEnumerator Timer()
    {
        while (currentTime > 0 && roundActive)
        {
            currentTime -= Time.deltaTime;
            timeText.text = Mathf.Max(0f, currentTime).ToString("f0");
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
