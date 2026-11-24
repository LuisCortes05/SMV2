using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticSlotSelection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 artworkOriginalSize = transform.Find("artwork").GetComponent<RectTransform>().sizeDelta;

        int random = Random.Range(0, CharacterSelectionManager.instance.characters.Count - 1);

        Characters randomChar = CharacterSelectionManager.instance.characters[random];

        CharacterSelectionManager.instance.ShowCharacterInSlot(transform.GetSiblingIndex(), randomChar);

        transform.Find("artwork").GetComponent<RectTransform>().sizeDelta = artworkOriginalSize;
        transform.Find("artwork").GetComponent<RectTransform>().sizeDelta *= randomChar.zoom;
    }
}
