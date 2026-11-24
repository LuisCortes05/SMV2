using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSelectionManager : MonoBehaviour
{
    private GridLayoutGroup gridLayout;
    [HideInInspector]
    public Vector2 slotArtworkSize;
    private float slotArtworkDefaultX;

    public static CharacterSelectionManager instance;

    [Header("Character List")]
    public List<Characters > characters = new List<Characters>();

    [Space]
    [Header("Public References")]
    public GameObject charCellPrefab;
    //public GameObject gridBgPrefab;
    public Transform playerSlotsContainer;

    [Space]
    [Header("Current Confirmed Character")]
    public Characters confirmedCharacter;

    private void Awake()
    {
        instance = this;

        Transform defaultArtwork = playerSlotsContainer.GetChild(0).Find("artWork");
        slotArtworkSize = defaultArtwork.GetComponent<RectTransform>().sizeDelta;
        slotArtworkDefaultX = defaultArtwork.localPosition.x;
    }
    private void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        GetComponent<RectTransform>().sizeDelta = new Vector2(gridLayout.cellSize.x * 5, 
            gridLayout.cellSize.y * 2);
        //RectTransform gridBG = Instantiate(gridBgPrefab, transform.parent).GetComponent<RectTransform>();
        //gridBG.transform.SetSiblingIndex(transform.GetSiblingIndex());
        //gridBG.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        slotArtworkSize = playerSlotsContainer.GetChild(0).Find("artWork").GetComponent<RectTransform>().sizeDelta;
        slotArtworkDefaultX = playerSlotsContainer.GetChild(0).Find("artWork").localPosition.x;

        foreach (Characters character in characters)
        {
            SpawnCharacterCell(character);
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

        //Vector2 pixelSize = new Vector2(artWork.sprite.texture.width, artWork.sprite.texture.height);
        //Vector2 pixelPivot = artWork.sprite.pivot;
        //Vector2 uiPivot = new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);

        artWork.GetComponent<RectTransform>().pivot = uiPivot(artWork.sprite);
        artWork.GetComponent<RectTransform>().sizeDelta *= character.zoom;
    }

    public void ShowCharacterInSlot(int player, Characters character)
    {
        bool nullChar = (character == null);

        Color alpha = nullChar ? Color.clear : Color.white;
        Sprite artwork = nullChar ? null : character.characterImage;
        string name = nullChar ? string.Empty : character.characterName;
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

        slot.Find("name").GetComponent<TextMeshProUGUI>().text = name;
        slot.Find("player").GetComponentInChildren<TextMeshProUGUI>().text = playernickname;
        slot.Find("iconAndPx").GetComponentInChildren<TextMeshProUGUI>().text = playernumber;
    }

    public void ConfirmCharacter(int player, Characters character)
    {
        if (confirmedCharacter == null)
        {
            confirmedCharacter = character;
            playerSlotsContainer.GetChild(player).DOPunchPosition(Vector3.down * 3, .3f, 10, 1);
        }
    }

    public Vector2 uiPivot(Sprite sprite)
    {
        Vector2 pixelSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 pixelPivot = sprite.pivot;
        return new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);
    }
}
