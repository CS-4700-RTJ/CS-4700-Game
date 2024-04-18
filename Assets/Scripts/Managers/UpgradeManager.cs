using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeSO[] allUpgrades;
    public UpgradeSO[] startingSpellUpgrades;

    public UpgradeUIElement newSpellUpgradeElement;
    public UpgradeUIElement spellEnhancementUpgradeElement;
    public UpgradeUIElement playerBuffUpgradeElement;
    
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

            // Set the New Spell upgrade
            if (newSpellUpgrades.Length > 0)
            {
                var newSpellUpgrade = newSpellUpgrades[Random.Range(0, newSpellUpgrades.Length)];
                newSpellUpgradeElement.SetUpgrade(newSpellUpgrade, ChooseUpgrade);
                newSpellUpgradeElement.gameObject.SetActive(true);
            }
            else
            {
                newSpellUpgradeElement.gameObject.SetActive(false);
            }

            // Set the Spell Enhancement Upgrade
            if (spellEnhancementUpgrades.Length > 0)
            {
                var spellEnhancementUpgrade = spellEnhancementUpgrades[Random.Range(0, spellEnhancementUpgrades.Length)];
                spellEnhancementUpgradeElement.SetUpgrade(spellEnhancementUpgrade, ChooseUpgrade);
                spellEnhancementUpgradeElement.gameObject.SetActive(true);
            }
            else
            {
                spellEnhancementUpgradeElement.gameObject.SetActive(false);
            }

            // Set the Player Buff Upgrade
            if (playerBuffUpgrades.Length > 0)
            {
                var playerBuffUpgrade = playerBuffUpgrades[Random.Range(0, playerBuffUpgrades.Length)];
                playerBuffUpgradeElement.SetUpgrade(playerBuffUpgrade, ChooseUpgrade);
                playerBuffUpgradeElement.gameObject.SetActive(true);
            }
            else
            {
                playerBuffUpgradeElement.gameObject.SetActive(false);
            }
        }
    }

    private void ChooseUpgrade(UpgradeSO chosenUpgrade)
    {
        print("Player Chose " + chosenUpgrade.upgradeName);
    }

   /// <summary>
   /// Gets the upgrades that are currently available to the player
   /// </summary>
   /// <param name="newSpellUpgrades">An array containing the available upgrades for new spells to learn</param>
   /// <param name="spellEnhancementUpgrades">An array containing the available upgrades for spells to enhance</param>
   /// <param name="playerBuffUpgrades">An array containing the available upgrade for player buffs</param>
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
