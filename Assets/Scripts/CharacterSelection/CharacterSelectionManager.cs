using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSelectionManager : MonoBehaviour
{
    private GridLayoutGroup gridLayout;
    [HideInInspector] public Vector2 slotArtworkSize;
    private float slotArtworkDefaultX;

    public static CharacterSelectionManager instance;

    [Header("Character List")]
    public List<Characters> characters = new List<Characters>();

    [Header("Public References")]
    public GameObject charCellPrefab;
    public Transform playerSlotsContainer;

    [Header("Player Cursors (New Input System)")]
    [SerializeField] private PlayerInput cursorP1Input;
    [SerializeField] private PlayerInput cursorP2Input;

    [Header("Confirmed Characters")]
    public Characters[] confirmedCharacters = new Characters[2];

    [Header("Countdown & Scene Transition")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownSeconds = 3f;
    [SerializeField] private string arenaSceneName = "Arena";
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        instance = this;

        Transform defaultArtwork = playerSlotsContainer.GetChild(0).Find("artWork");
        slotArtworkSize = defaultArtwork.GetComponent<RectTransform>().sizeDelta;
        slotArtworkDefaultX = defaultArtwork.localPosition.x;

        if (countdownPanel != null)
            countdownPanel.SetActive(false);
    }
    private void Start()
    {
        AssignControllersToCursors();

        gridLayout = GetComponent<GridLayoutGroup>();
        GetComponent<RectTransform>().sizeDelta = new Vector2(
            gridLayout.cellSize.x * 5,
            gridLayout.cellSize.y * 2
        );

        foreach (Characters character in characters)
        {
            SpawnCharacterCell(character);
        }
    }

    private void AssignControllersToCursors()
    {
        if (cursorP1Input != null && Gamepad.all.Count >= 1)
        {
            cursorP1Input.SwitchCurrentControlScheme("Gamepad", Gamepad.all[0]);
        }

        if (cursorP2Input != null && Gamepad.all.Count >= 2)
        {
            cursorP2Input.SwitchCurrentControlScheme("Gamepad", Gamepad.all[1]);
        }
    }

    private void SpawnCharacterCell(Characters character)
    {
        GameObject charCell = Instantiate(charCellPrefab, transform);
        charCell.name = character.characterName;

        Image artWork = charCell.transform.Find("artWork").GetComponent<Image>();
        TextMeshProUGUI name = charCell.transform.Find("nameRect").GetComponentInChildren<TextMeshProUGUI>();

        artWork.sprite = character.characterImage;
        name.text = character.characterName;

        // Oscurece el retrato en la cuadrícula si el personaje aún no es jugable
        artWork.color = character.isPlayable ? Color.white : new Color(0.35f, 0.35f, 0.35f, 1f);

        artWork.GetComponent<RectTransform>().pivot = uiPivot(artWork.sprite);
        artWork.GetComponent<RectTransform>().sizeDelta *= character.zoom;
    }

    public void ShowCharacterInSlot(int player, Characters character)
    {
        bool nullChar = (character == null);

        Color alpha = nullChar ? Color.clear : Color.white;
        Sprite artwork = nullChar ? null : character.characterImage;
        string charName = nullChar ? string.Empty : character.characterName;
        string playernickname = "Player " + (player + 1).ToString();
        string playernumber = "P" + (player + 1).ToString();

        Transform slot = playerSlotsContainer.GetChild(player);
        Transform slotArtwork = slot.Find("artWork");
        Transform slotIcon = slot.Find("icon");

        slotArtwork.DOKill();

        Sequence s = DOTween.Sequence();
        s.Append(slotArtwork.DOLocalMoveX(slotArtworkDefaultX - 300f, .05f).SetEase(Ease.OutCubic));
        s.AppendCallback(() =>
        {
            Image artworkImg = slotArtwork.GetComponent<Image>();
            RectTransform artworkRect = slotArtwork.GetComponent<RectTransform>();

            artworkImg.sprite = artwork;
            artworkImg.color = alpha;

            if (artwork != null)
            {
                artworkRect.pivot = uiPivot(artwork);
                artworkRect.sizeDelta = slotArtworkSize * character.zoom;
            }
        });
        s.Append(slotArtwork.DOLocalMoveX(slotArtworkDefaultX + 300f, 0));
        s.Append(slotArtwork.DOLocalMoveX(slotArtworkDefaultX, .05f).SetEase(Ease.OutCubic));

        if (slotIcon != null)
        {
            Image iconImg = slotIcon.GetComponent<Image>();
            if (nullChar)
            {
                iconImg.DOFade(0, 0);
            }
            else
            {
                iconImg.sprite = character.characterIcon;
                iconImg.DOFade(.3f, 0);
            }
        }

        slot.Find("name").GetComponent<TextMeshProUGUI>().text = charName;
        slot.Find("player").GetComponentInChildren<TextMeshProUGUI>().text = playernickname;
        slot.Find("iconAndPx").GetComponentInChildren<TextMeshProUGUI>().text = playernumber;
    }

    public void ConfirmCharacter(int player, Characters character)
    {
        if (character == null || !character.isPlayable) return;

        if (confirmedCharacters[player] == null)
        {
            confirmedCharacters[player] = character;
            playerSlotsContainer.GetChild(player).DOPunchPosition(Vector3.down * 3, .3f, 10, 1);

            // Si ambos jugadores ya eligieron, iniciar conteo hacia atrás
            if (confirmedCharacters[0] != null && confirmedCharacters[1] != null)
            {
                if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
                countdownCoroutine = StartCoroutine(CountdownToArena());
            }
        }
    }

    public void CancelCharacter(int player)
    {
        confirmedCharacters[player] = null;

        // Si cualquiera presiona B durante el conteo, se cancela el cambio de escena
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        if (countdownPanel != null)
            countdownPanel.SetActive(false);
    }

    private IEnumerator CountdownToArena()
    {
        MatchData.selectedCharacters[0] = confirmedCharacters[0];
        MatchData.selectedCharacters[1] = confirmedCharacters[1];

        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        float remaining = countdownSeconds;

        while (remaining > 0f)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(remaining).ToString();

            remaining -= Time.deltaTime;
            yield return null;
        }

        if (countdownText != null)
            countdownText.text = "¡FIGHT!";

        yield return new WaitForSeconds(0.5f);

        // Detiene todos los tweens de la UI antes de destruir los objetos de esta escena
        DOTween.KillAll();

        UnityEngine.SceneManagement.SceneManager.LoadScene(arenaSceneName);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }

    public Vector2 uiPivot(Sprite sprite)
    {
        Vector2 pixelSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 pixelPivot = sprite.pivot;
        return new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);
    }
}
