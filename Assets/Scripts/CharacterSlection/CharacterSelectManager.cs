using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    #region Variables
    [Header("Roster & Grid")]
    [SerializeField] private List<CharacterData> roster = new List<CharacterData>(9);
    [SerializeField] private List<Image> slotPortraits = new List<Image>(9);
    [SerializeField] private List<GameObject> lockIcons = new List<GameObject>(9);
    [SerializeField] private int gridColumns = 3;

    [Header("Selection Frames")]
    [SerializeField] private RectTransform p1CursorFrame;
    [SerializeField] private RectTransform p2CursorFrame;
    [SerializeField] private Color p1Color = Color.blue;
    [SerializeField] private Color p2Color = Color.red;

    [Header("Spawn Points")]
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform p2Spawn;

    [Header("Combat Transition")]
    [SerializeField] private int battleScene;
    [SerializeField] private float countdownTime = 3f;
    [SerializeField] private TextMeshProUGUI countdownText;

    //Interno
    private int p1Index = 0;
    private int p2Index = 1;
    private int p1ConfirmedIndex = -1;
    private int p2ConfirmedIndex = -1;

    private GameObject p1PreviewInstance;
    private GameObject p2PreviewInstance;
    private Coroutine startMatchCoroutine;
    #endregion

    private void Start()
    {
        if(countdownText!= null)
            countdownText.gameObject.SetActive(false);

        // Colores de los marcos
        if(p1CursorFrame != null && p1CursorFrame.TryGetComponent(out Image img1))
            img1.color = p1Color;

        if(p2CursorFrame != null && p2CursorFrame.TryGetComponent(out Image img2))
            img2.color = p2Color;

        InitializeGrid();
        UpdateCursorPosition(1);
        UpdateCursorPosition(2);
    }

    #region Grid Initialization
    private void InitializeGrid()
    {
        for (int i = 0; i < roster.Count; i++)
        {
            bool unlocked = IsCharacterUnlocked(i);

            if (i < slotPortraits.Count && slotPortraits[i] != null)
            {
                slotPortraits[i].sprite = roster[i].portrait;

                //Oscurecemos retrato si esta bloqueado
                slotPortraits[i].color = unlocked ? Color.white : new Color(0.225f, 0.25f, 0.25f, 1f);
            }

            if(i < lockIcons.Count && lockIcons[i] != null)
            {
                lockIcons[i].SetActive(!unlocked);
            }
        }
    }
    /// <summary>
    /// Verificamos si el personaje en el indice dado esta desbloqueado.
    /// </summary>
    public bool IsCharacterUnlocked(int index)
    {
        if(index < 0 || index >= roster.Count) return false;
        if (roster[index].isUnlocked) return true;

        return PlayerPrefs.GetInt($"CharUnlocked_{index}", 0) == 1;
    }

    /// <summary>
    /// Llama esta funcion para desbloquear al resto de personajes
    /// IMPORTANTE: AUN NO ESTAN DISPONIBLES TODOS, POR LO TANTO NO SE PUEDEN DESBLOQUEAR TODOS, SOLO LOS QUE ESTAN DISPONIBLES EN EL JUEGO.
    /// </summary>

    public static void UnlockAllSecretCharacters(int totalCharacters = 9)
    {
        for(int i = 0; i < totalCharacters; i++)
        {
            PlayerPrefs.SetInt($"CharUnlocked_{i}", 1);
        }
        PlayerPrefs.Save();
    }
    #endregion

    #region Navigation and Selection
    public void OnPlayer1Navigate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || p1ConfirmedIndex != -1) return;
        MoveCursor(1, ctx.ReadValue<Vector2>());
    }
    public void OnPlayer2Navigate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || p2ConfirmedIndex != -1) return;
        MoveCursor(2, ctx.ReadValue<Vector2>());
    }

    public void OnPlayer1Confirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ConfirmSelection(1);
    }

    public void OnPlayer2Confirm(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ConfirmSelection(2);
    }
    public void OnPlayer1Cancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        CancelSelection(1);
    }

    public void OnPlayer2Cancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        CancelSelection(2);
    }

    private void MoveCursor(int playerID, Vector2 input)
    {
        int currentIndex = (playerID == 1) ? p1Index : p2Index;
        int row = currentIndex / gridColumns;
        int col = currentIndex % gridColumns;
        int totalRows = Mathf.CeilToInt((float)roster.Count / gridColumns);

        if (input.x > 0.5f) col = (col + 1) % gridColumns;
        else if (input.x < -0.5f) col = (col - 1 + gridColumns) % gridColumns;

        if (input.y < -0.5f) row = (row + 1) % totalRows;
        else if (input.y > 0.5f) row = (row - 1 + totalRows) % totalRows;

        int newIndex = Mathf.Clamp(row * gridColumns + col, 0, roster.Count - 1);

        if (playerID == 1) p1Index = newIndex;
        else p2Index = newIndex;

        UpdateCursorPosition(playerID);
    }

    private void UpdateCursorPosition(int playerID)
    {
        int index = (playerID == 1) ? p1Index : p2Index;
        RectTransform cursor = (playerID == 1) ? p1CursorFrame : p2CursorFrame;

        if (cursor != null && index < slotPortraits.Count && slotPortraits[index] != null)
        {
            cursor.position = slotPortraits[index].rectTransform.position;
        }
    }

    private void ConfirmSelection(int playerID)
    {
        int selectedIndex = (playerID == 1) ? p1Index : p2Index;

        // Checamos si ya esta confirmado
        if ((playerID == 1 && p1ConfirmedIndex != -1) || (playerID == 2 && p2ConfirmedIndex != -1))
            return;

        // Bloqueamos si uno de los personajes esta bloqueado
        if (!IsCharacterUnlocked(selectedIndex))
        {
            Debug.Log("Personaje bloqueado. Termina el juego para desbloquearlo.");
            return;
        }

        // Bloqueamos la selección si el otro jugador ya eligió este mismo personaje
        if ((playerID == 1 && p2ConfirmedIndex == selectedIndex) ||
            (playerID == 2 && p1ConfirmedIndex == selectedIndex))
        {
            Debug.Log("Este personaje ya fue seleccionado por el otro jugador.");
            return;
        }

        // Confirmar y spawnear vista previa
        if (playerID == 1)
        {
            p1ConfirmedIndex = selectedIndex;
            SpawnCharacterPreview(1, roster[selectedIndex]);
        }
        else
        {
            p2ConfirmedIndex = selectedIndex;
            SpawnCharacterPreview(2, roster[selectedIndex]);
        }

        CheckBothPlayersReady();
    }

    private void CancelSelection(int playerID)
    {
        // Si estaba corriendo el timer, detenerlo
        if (startMatchCoroutine != null)
        {
            StopCoroutine(startMatchCoroutine);
            startMatchCoroutine = null;
            if (countdownText != null) countdownText.gameObject.SetActive(false);
        }

        if (playerID == 1 && p1ConfirmedIndex != -1)
        {
            p1ConfirmedIndex = -1;
            if (p1PreviewInstance != null) Destroy(p1PreviewInstance);
        }
        else if (playerID == 2 && p2ConfirmedIndex != -1)
        {
            p2ConfirmedIndex = -1;
            if (p2PreviewInstance != null) Destroy(p2PreviewInstance);
        }
    }

    private void SpawnCharacterPreview(int playerID, CharacterData data)
    {
        if (data.previewPrefab == null) return;

        if (playerID == 1)
        {
            if (p1PreviewInstance != null) Destroy(p1PreviewInstance);
            p1PreviewInstance = Instantiate(data.previewPrefab, p1Spawn.position, p1Spawn.rotation);
        }
        else
        {
            if (p2PreviewInstance != null) Destroy(p2PreviewInstance);
            p2PreviewInstance = Instantiate(data.previewPrefab, p2Spawn.position, p2Spawn.rotation);
        }
    }

    private void CheckBothPlayersReady()
    {
        if (p1ConfirmedIndex != -1 && p2ConfirmedIndex != -1)
        {
            if (GameSessionData.Instance != null)
            {
                GameSessionData.Instance.SetSelections(roster[p1ConfirmedIndex], roster[p2ConfirmedIndex]);
            }

            startMatchCoroutine = StartCoroutine(StartMatchCountdown());
        }
    }

    private IEnumerator StartMatchCountdown()
    {
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        float timer = countdownTime;
        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = $"¡Combate en {Mathf.CeilToInt(timer)}!";

            timer -= Time.deltaTime;
            yield return null;
        }

        //SceneManager.ChangeScene(1);
    }
    #endregion
}
