using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeSO[] allUpgrades;
    public UpgradeSO[] startingSpellUpgrades;

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
    }

    private UpgradeSO[] GetAvailableUpgrades()
    {
        var availableUpgrades = new List<UpgradeSO>();

        foreach (var upgrade in availableUpgrades)
        {
            if (ownedUpgrades.Contains(upgrade)) continue;
            
            // If the player has the prereq, the upgrade is available
            bool hasPreReq = upgrade.requiredUpgrade == null || ownedUpgrades.Contains(upgrade.requiredUpgrade);
            if (GameManager.GetCurrentTier() >= upgrade.requiredTier && hasPreReq) availableUpgrades.Add(upgrade);
        }

        return availableUpgrades.ToArray();
    }
}
