using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Spell : ScriptableObject
{
    [Tooltip("What is the spell's name?")]
    public string spellName;
    [Tooltip("How long do you need to charge the spell before it can fire?"), Min(0)]
    public float castTime;
    [Tooltip("How much mana does it take to cast this spell?"), Min(0)]
    public float manaCost;
    [Tooltip("Visual Effect to display while charging")]
    public GameObject castVfx;
    [Tooltip("Sound Effect to play when casting the spell")]
    public AudioClip castSfx;
    [Tooltip("The image to display in the UI")]
    public Sprite spellIcon;
    [Tooltip("A description of what this spell does")]
    public string description;
}
