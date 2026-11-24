using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    [SerializeField] private Characters player1Data;
    [SerializeField] private Characters player2Data;

    private Vector3 p1StartPos;
    private Vector3 p2StartPos;

    [Header("UI Manager")]
    [SerializeField] private Slider HPPlayer1;
    [SerializeField] private Slider HPPlayer2;

    [Header("Round Victory UI")]
    [SerializeField] private GameObject panelRoundWinner;
    [SerializeField] private Image roundBannerImage;
    [SerializeField] private TextMeshProUGUI textRoundWinner; // Respaldo si falta sprite o hay empate

    [Header("Match Victory UI")]
    [SerializeField] private GameObject panelVictory;
    [SerializeField] private Image matchBackgroundIconImage; // Fondo con characterIcon
    [SerializeField] private Image matchBannerImage;         // Cartel frontal de victoria
    [SerializeField] private TextMeshProUGUI winnerTXT;        // Respaldo si falta cartel
    [SerializeField] private GameObject victoryReturnButton;

    [Header("UI Fallbacks (Blindaje opcional)")]
    [SerializeField] private Sprite defaultRoundBanner;
    [SerializeField] private Sprite drawRoundBanner;
    [SerializeField] private Sprite defaultMatchBanner;
    [SerializeField] private Sprite defaultCharacterIcon;

    [Header("KO Effect")]
    [SerializeField] private KOEffect koEffect;
    private Coroutine timerCoroutine;
    #endregion

    [Header("Debug / Testing")]
    [SerializeField] private float testDamageAmount = 25f;

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            DamagePlayer2ForTest();
        }
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            DamagePlayer1ForTest();
        }
    }
#endif

    /// <summary>
    /// Resta vida para prueba, borrar esto despues
    /// </summary>
    public void DamagePlayer2ForTest()
    {
        if (!roundActive || player2Health == null) return;
        player2Health.TakeDamage(testDamageAmount);
    }

    public void DamagePlayer1ForTest()
    {
        if (!roundActive || player1Health == null) return;
        player1Health.TakeDamage(testDamageAmount);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player1Data == null && MatchData.selectedCharacters[0] != null)
            player1Data = MatchData.selectedCharacters[0];

        if (player2Data == null && MatchData.selectedCharacters[1] != null)
            player2Data = MatchData.selectedCharacters[1];
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

        //Restaurar Animaciones
        player1Transform.GetComponent<Personaje>()?.CancelarCombo();
        player2Transform.GetComponent<Personaje>()?.CancelarCombo();

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
        ShowMatchWinnerUI(winnerID);
        Debug.Log($"Game over. Ganador: Jugador {winnerID}");
    }
    #endregion

    #region Victory Banners & Safeguards
    private Characters GetCharacterData(int playerID)
    {
        if (playerID == 1) return player1Data;
        if (playerID == 2) return player2Data;
        return null;
    }

    private void ShowRoundWinnerUI(int roundWinnerID)
    {
        if (panelRoundWinner == null)
        {
            Debug.LogWarning("GameManager: Falta asignar 'panelRoundWinner' en el Inspector.");
            return;
        }

        panelRoundWinner.SetActive(true);

        Characters winnerChar = GetCharacterData(roundWinnerID);
        Sprite bannerToUse = null;

        if (roundWinnerID == 0)
        {
            bannerToUse = drawRoundBanner;
        }
        else if (winnerChar != null && winnerChar.roundVictoryBanner != null)
        {
            bannerToUse = winnerChar.roundVictoryBanner;
        }
        else
        {
            if (winnerChar != null)
                Debug.LogWarning($"[Blindaje] '{winnerChar.characterName}' no tiene asignado 'roundVictoryBanner'. Usando respaldo.");

            bannerToUse = defaultRoundBanner;
        }

        // Mostrar banner si existe, o activar texto de respaldo
        bool hasBanner = (roundBannerImage != null && bannerToUse != null);

        if (roundBannerImage != null)
        {
            roundBannerImage.gameObject.SetActive(hasBanner);
            if (hasBanner)
            {
                roundBannerImage.sprite = bannerToUse;
                roundBannerImage.preserveAspect = true;
            }
        }

        if (textRoundWinner != null)
        {
            textRoundWinner.gameObject.SetActive(!hasBanner);
            if (!hasBanner)
            {
                string winnerName = (winnerChar != null && !string.IsNullOrEmpty(winnerChar.characterName))
                    ? winnerChar.characterName
                    : $"Jugador {roundWinnerID}";

                textRoundWinner.text = (roundWinnerID == 0)
                    ? "¡Empate!"
                    : $"¡{winnerName} gana el round!";
            }
        }
    }

    private void ShowMatchWinnerUI(int winnerID)
    {
        if (panelVictory == null)
        {
            Debug.LogWarning("GameManager: Falta asignar 'panelVictory' en el Inspector.");
            return;
        }

        panelVictory.SetActive(true);

        Characters winnerChar = GetCharacterData(winnerID);

        if (victoryReturnButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(victoryReturnButton);
        }

        Sprite bgIconToUse = (winnerChar != null && winnerChar.characterImage != null)
            ? winnerChar.characterImage
            : defaultCharacterIcon;

        if (winnerChar != null && winnerChar.characterImage == null)
            Debug.LogWarning($"[Blindaje] '{winnerChar.characterName}' no tiene asignado 'characterIcon'.");

        if (matchBackgroundIconImage != null)
        {
            bool hasBgIcon = (bgIconToUse != null);
            matchBackgroundIconImage.gameObject.SetActive(hasBgIcon);
            if (hasBgIcon)
            {
                matchBackgroundIconImage.sprite = bgIconToUse;
                matchBackgroundIconImage.preserveAspect = true;
            }
        }

        Sprite matchBannerToUse = (winnerChar != null && winnerChar.matchVictoryBanner != null)
            ? winnerChar.matchVictoryBanner
            : defaultMatchBanner;

        if (winnerChar != null && winnerChar.matchVictoryBanner == null)
            Debug.LogWarning($"[Blindaje] '{winnerChar.characterName}' no tiene asignado 'matchVictoryBanner'. Usando respaldo.");

        bool hasMatchBanner = (matchBannerImage != null && matchBannerToUse != null);

        if (matchBannerImage != null)
        {
            matchBannerImage.gameObject.SetActive(hasMatchBanner);
            if (hasMatchBanner)
            {
                matchBannerImage.sprite = matchBannerToUse;
                matchBannerImage.preserveAspect = true;
            }
        }

        if (winnerTXT != null)
        {
            winnerTXT.gameObject.SetActive(!hasMatchBanner);
            if (!hasMatchBanner)
            {
                string winnerName = (winnerChar != null && !string.IsNullOrEmpty(winnerChar.characterName))
                    ? winnerChar.characterName
                    : $"Jugador {winnerID}";

                winnerTXT.text = $"¡{winnerName} Gana la Partida!";
            }
        }
    }
    #endregion

    #region Corutines
    private IEnumerator RoundTransition(int roundWinnerID)
    {
        ShowRoundWinnerUI(roundWinnerID);
        yield return new WaitForSeconds(3f);
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

public void RegisterPlayers(
        PlayerHealth p1Health,
        PlayerHealth p2Health,
        Transform p1Transform,
        Transform p2Transform,
        Characters p1Data = null,
        Characters p2Data = null)
    {
        player1Health = p1Health;
        player2Health = p2Health;
        player1Transform = p1Transform;
        player2Transform = p2Transform;

        if (p1Data != null) player1Data = p1Data;
        if (p2Data != null) player2Data = p2Data;
    }
}
