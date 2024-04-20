using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrade", order = 3)]
public class UpgradeSO : ScriptableObject
{
    public enum UpgradeType
    {
        SpellUpgrade,
        PlayerBuff
    }
    
    [Header("Requirements")] 
    [Min(1)]
    [Tooltip("What is the minimum tier that this upgrade can appear in?")]
    public int requiredTier = 1;
    [Tooltip("Is another upgrade required for this one to appear?")]
    public UpgradeSO requiredUpgrade;

    [Header("Upgrade Info")] 
    [Tooltip("The upgrade's name")]
    public string upgradeName;
    [Tooltip("The upgrade's description")]
    public string upgradeDescription;
    [Tooltip("The upgrade's icon")]
    public Sprite upgradeIcon;
    [Tooltip("What type of upgrade is this?")]
    public UpgradeType upgradeType;
    
    // Spell Upgrade Information
    [HideInInspector]
    public Spell spellToRemove;
    [HideInInspector]
    public Spell spellToGain;

    // Player Buff Information
    [HideInInspector]
    public int maxHealthIncrease;
    [HideInInspector]
    public int maxManaIncrease;
    [HideInInspector]
    public float moveSpeedMultiplier;
}
