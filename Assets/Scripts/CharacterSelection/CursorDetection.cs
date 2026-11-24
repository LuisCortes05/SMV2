using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class CursorDetection : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("0 para Player 1, 1 para Player 2")]
    public int playerIndex = 0;

    private GraphicRaycaster gr;
    private Canvas parentCanvas;
    private PointerEventData pointerEventData = new PointerEventData(null);

    private PlayerInput playerInput;
    private InputAction submitAction;
    private InputAction cancelAction;

    public Transform currentCharacter;
    public Transform token;
    public bool hasToken = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        submitAction = playerInput.actions["Submit"];
        cancelAction = playerInput.actions["Cancel"];
    }
    void Start()
    {
        gr = GetComponentInParent<GraphicRaycaster>();
        parentCanvas = gr.GetComponent<Canvas>();
        CharacterSelectionManager.instance.ShowCharacterInSlot(playerIndex, null);
    }

    void Update()
    {
        if (submitAction.WasPressedThisFrame())
        {
            if (hasToken && currentCharacter != null)
            {
                TokenFollow(false);
                Characters chosen = CharacterSelectionManager.instance.characters[currentCharacter.GetSiblingIndex()];
                CharacterSelectionManager.instance.ConfirmCharacter(playerIndex, chosen);
            }
        }

        // CANCEL
        if (cancelAction.WasPressedThisFrame())
        {
            if (!hasToken)
            {
                CharacterSelectionManager.instance.CancelCharacter(playerIndex);
                TokenFollow(true);
            }
        }

        if (hasToken)
        {
            token.position = transform.position;
        }

        // Usa coordenadas directas si el Canvas es Overlay, o convierte con la cámara del Canvas
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            pointerEventData.position = transform.position;
        }
        else
        {
            pointerEventData.position = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, transform.position);
        }

        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);

        if (hasToken)
        {
            Transform raycastCharacter = null;

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.transform.Find("selectedBorder") != null)
                {
                    raycastCharacter = result.gameObject.transform;
                    break;
                }
            }

            if (raycastCharacter != null)
            {
                if (raycastCharacter != currentCharacter)
                {
                    ClearCurrentBorder();
                    SetCurrentCharacter(raycastCharacter);
                }
            }
            else if (currentCharacter != null)
            {
                ClearCurrentBorder();
                SetCurrentCharacter(null);
            }
        }
    }

    private void ClearCurrentBorder()
    {
        if (currentCharacter != null)
        {
            Image border = currentCharacter.Find("selectedBorder").GetComponent<Image>();
            border.DOKill();
            border.color = Color.clear;
        }
    }

    void SetCurrentCharacter(Transform tf)
    {
        if (tf != null)
        {
            tf.Find("selectedBorder").GetComponent<Image>().color = Color.white;
            tf.Find("selectedBorder").GetComponent<Image>().DOColor(Color.red, .7f).SetLoops(-1);
        }

        currentCharacter = tf;

        if (tf != null)
        {
            int index = tf.GetSiblingIndex();
            Characters character = CharacterSelectionManager.instance.characters[index];
            CharacterSelectionManager.instance.ShowCharacterInSlot(0, character);
        }
        else
        {
            CharacterSelectionManager.instance.ShowCharacterInSlot(0, null);
        }
    }

    void TokenFollow(bool trigger)
    {
        hasToken = trigger;
    }
}
