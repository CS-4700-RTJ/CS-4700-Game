using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
[CreateAssetMenu]
=======
[CreateAssetMenu(menuName = "ScriptableObjects/Spell", order = 1)]
>>>>>>> ee62877142338c7d80e7badf05f02c61c1e2a88c
public class Spell : ScriptableObject
{
    [Tooltip("What is the spell's name?")]
    public string spellName;
    [Tooltip("How long do you need to charge the spell before it can fire?"), Min(0)]
    public float castTime;
    [Tooltip("How much mana does it take to cast this spell?"), Min(0)]
    public float manaCost;

    [Tooltip("Sound Effect to play when casting the spell")]
    public AudioClip castSfx;
    [Tooltip("The image to display in the UI")]
    public Sprite spellIcon;
    [Tooltip("A description of what this spell does")]
    public string description;
    [Tooltip("Spell GameObject that is instantiated on cast")]
    public GameObject spellPrefab;
}
