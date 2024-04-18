using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeSO[] allUpgrades;
    public UpgradeSO[] startingSpellUpgrades;

    public bool getAvailableUpgrades = false;
    
    private SpellHandler _spellHandler;
    
    private List<UpgradeSO> ownedUpgrades;

    private void Start()
    {
        _spellHandler = PlayerHealth.PlayerTransform.GetComponent<SpellHandler>();
        ownedUpgrades = new List<UpgradeSO>();
        
        List<Spell> startingSpells = new List<Spell>();
        
        foreach (var upgrade in startingSpellUpgrades)
        {
            if (upgrade.upgradeType != UpgradeSO.UpgradeType.SpellUpgrade) continue;
            
            startingSpells.Add(upgrade.spellToGain);
            ownedUpgrades.Add(upgrade);
        }

        _spellHandler.availableSpells = startingSpells.ToArray();
        _spellHandler.SelectStartingSpell();
    }

    private void Update()
    {
        if (getAvailableUpgrades)
        {
            getAvailableUpgrades = false;

            GetAvailableUpgrades(out var newSpellUpgrades, out var spellEnhancementUpgrades, out var playerBuffUpgrades);
            
            print("New Spell Upgrades: " + newSpellUpgrades.Length);
            print("Spell Enhancement Upgrades: " + spellEnhancementUpgrades.Length);
            print("Player Buff Upgrades: " + playerBuffUpgrades.Length);
        }
    }

    /// <summary>
    /// Gets an array of Upgrades that the player is able to choose from
    /// </summary>
    /// <returns>An array of available upgrades</returns>
    private void GetAvailableUpgrades(out UpgradeSO[] newSpellUpgrades, out UpgradeSO[] spellEnhancementUpgrades, out UpgradeSO[] playerBuffUpgrades)
    {
        var newSpellUpgradesList = new List<UpgradeSO>();
        var spellEnhancementUpgradesList = new List<UpgradeSO>();
        var playerBuffUpgradesList = new List<UpgradeSO>();

        foreach (var upgrade in allUpgrades)
        {
            if (ownedUpgrades.Contains(upgrade)) continue;
            
            // If the player has the prereq, the upgrade is available
            bool hasPreReq = upgrade.requiredUpgrade == null || ownedUpgrades.Contains(upgrade.requiredUpgrade);
            if (GameManager.GetCurrentTier() >= upgrade.requiredTier && hasPreReq)
            {
                if (upgrade.upgradeType == UpgradeSO.UpgradeType.PlayerBuff) playerBuffUpgradesList.Add(upgrade);
                else if (upgrade.spellToRemove == null) newSpellUpgradesList.Add(upgrade);
                else spellEnhancementUpgradesList.Add(upgrade);
            }
        }
        
        newSpellUpgrades = newSpellUpgradesList.ToArray();
        spellEnhancementUpgrades = spellEnhancementUpgradesList.ToArray();
        playerBuffUpgrades = playerBuffUpgradesList.ToArray();
    }
}
