using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CursorDetection : MonoBehaviour
{
    private GraphicRaycaster gr;
    private Canvas parentCanvas;
    private PointerEventData pointerEventData = new PointerEventData(null);

    public Transform currentCharacter;
    public Transform token;
    public bool hasToken;

    void Start()
    {
        gr = GetComponentInParent<GraphicRaycaster>();
        parentCanvas = gr.GetComponent<Canvas>();
        CharacterSelectionManager.instance.ShowCharacterInSlot(0, null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentCharacter != null)
            {
                TokenFollow(false);
                CharacterSelectionManager.instance.ConfirmCharacter(0, CharacterSelectionManager.instance.characters[currentCharacter.GetSiblingIndex()]);
            }
        }

        // CANCEL
        if (Input.GetKeyDown(KeyCode.X))
        {
            CharacterSelectionManager.instance.confirmedCharacter = null;
            TokenFollow(true);
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

            // Recorre todos los elementos bajo el cursor hasta encontrar una celda válida
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
                    if (currentCharacter != null)
                    {
                        currentCharacter.Find("selectedBorder").GetComponent<Image>().DOKill();
                        currentCharacter.Find("selectedBorder").GetComponent<Image>().color = Color.clear;
                    }
                    SetCurrentCharacter(raycastCharacter);
                }
            }
            else
            {
                if (currentCharacter != null)
                {
                    currentCharacter.Find("selectedBorder").GetComponent<Image>().DOKill();
                    currentCharacter.Find("selectedBorder").GetComponent<Image>().color = Color.clear;
                    SetCurrentCharacter(null);
                }
            }
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
