using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Characters : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public Sprite characterIcon;
    public float zoom = 1;

    public bool isPlayable = false;

    [Header("CombatPrefab")]
    public GameObject characterPrefab;

    [Header("Victory Banners")]
    [Tooltip("Sprite que aparece al ganar un round")]
    public Sprite roundVictoryBanner;

    [Tooltip("Cartel principal que aparece al ganar la partida completa")]
    public Sprite matchVictoryBanner;
}
