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

    [Header("CombatPrefab")]
    public GameObject characterPrefab;
}
