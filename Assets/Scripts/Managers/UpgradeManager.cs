using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeSO[] allUpgrades;
    public UpgradeSO[] startingSpellUpgrades;

    public float timeBetweenUpgrades = 60f;

    [Header("UI")] public GameObject upgradePanel;
    public UpgradeUIElement newSpellUpgradeElement;
    public UpgradeUIElement spellEnhancementUpgradeElement;
    public UpgradeUIElement playerBuffUpgradeElement;
    
    private bool readyForUpgrade;
    
    private SpellHandler _spellHandler;
    private Animator _obeliskAnimator;
    
    private List<UpgradeSO> _ownedUpgrades;
    private static readonly int ObeliskActiveBool = Animator.StringToHash("Active");

    private PlayerHealth _playerHealth;
    private PlayerController _playerController;
    private PlayerControllerInput _playerInput;

    // audio
    private AudioSource _audiosource;
    public AudioClip panelPopUpSfx;
    public AudioClip upgradeSelectSfx;


    private void Start()
    {
        _spellHandler = PlayerHealth.PlayerTransform.GetComponent<SpellHandler>();
        _obeliskAnimator = GameObject.FindGameObjectWithTag("Obelisk").GetComponent<Animator>();
        _ownedUpgrades = new List<UpgradeSO>();
        _playerHealth = PlayerHealth.PlayerTransform.GetComponent<PlayerHealth>();
        _playerController = PlayerHealth.PlayerTransform.GetComponent<PlayerController>();
        _playerInput = PlayerHealth.PlayerTransform.GetComponent<PlayerControllerInput>();
        
        readyForUpgrade = false;
        _audiosource = GetComponent<AudioSource>();

        List<Spell> startingSpells = new List<Spell>();
        
        foreach (var upgrade in startingSpellUpgrades)
        {
            if (upgrade.upgradeType != UpgradeSO.UpgradeType.SpellUpgrade) continue;
            
            startingSpells.Add(upgrade.spellToGain);
            _ownedUpgrades.Add(upgrade);
        }

        _spellHandler.availableSpells = startingSpells.ToArray();
        _spellHandler.SelectStartingSpell();
        upgradePanel.SetActive(false);

        StartCoroutine(WaitForUpgrade());

        EventManager.OnPlayerDeath += StopUpgrades;
    }

    private void StopUpgrades()
    {
        StopAllCoroutines();
        EventManager.OnPlayerDeath -= StopUpgrades;
    }

    public bool GetUpgradeReady()
    {
        return _obeliskAnimator.GetBool(ObeliskActiveBool);
    }
    
    private void TriggerUpgrade()
    {
        GetAvailableUpgrades(out var newSpellUpgrades, out var spellEnhancementUpgrades, out var playerBuffUpgrades);
        _audiosource.PlayOneShot(panelPopUpSfx);

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

        // Activate the Obelisk
        _obeliskAnimator.SetBool(ObeliskActiveBool, true);
        if (_playerController.CanInteract()) GameManager.SetNotificationVisibility(true);
        // assume it opens right away, need to change
        // OpenUpgradeMenu();
    }

    public void OpenUpgradeMenu()
    {
        if(GetUpgradeReady()){
            upgradePanel.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            _playerInput.DisableInput();

            // select the first enabled upgrade
            if (newSpellUpgradeElement.gameObject.activeSelf) newSpellUpgradeElement.Select();
            else if (spellEnhancementUpgradeElement.gameObject.activeSelf) newSpellUpgradeElement.Select();
            else if (playerBuffUpgradeElement.gameObject.activeSelf) playerBuffUpgradeElement.Select();
            else
            {
                // no upgrades!
                _obeliskAnimator.SetBool(ObeliskActiveBool, false);
                upgradePanel.SetActive(false);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    
    private void ChooseUpgrade(UpgradeSO chosenUpgrade)
    {
        print("Player Chose " + chosenUpgrade.upgradeName);
        _ownedUpgrades.Add(chosenUpgrade);
        _audiosource.PlayOneShot(upgradeSelectSfx);
        
        // Apply the upgrade
        if (chosenUpgrade.upgradeType == UpgradeSO.UpgradeType.SpellUpgrade)
        {
            // Get a list of the current Spells
            List<Spell> currentSpells = new List<Spell>(_spellHandler.availableSpells);
            
            // Remove the spell if there is one
            if (chosenUpgrade.spellToRemove) currentSpells.Remove(chosenUpgrade.spellToRemove);
            
            // Add the spell to gain
            currentSpells.Add(chosenUpgrade.spellToGain);
            
            // set the available spells to the list
            _spellHandler.availableSpells = currentSpells.ToArray();
        }
        else
        {
            // Apply the player buffs
            _playerHealth.IncreaseMaxHealth(chosenUpgrade.maxHealthIncrease);
            _spellHandler.IncreaseMaxMana(chosenUpgrade.maxManaIncrease);
            _playerController.IncreaseMoveSpeed(chosenUpgrade.moveSpeedMultiplier);
        }
        
        _playerInput.EnableInput();
        Time.timeScale = 1;
        _obeliskAnimator.SetBool(ObeliskActiveBool, false);
        upgradePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        readyForUpgrade = true;
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
            if (_ownedUpgrades.Contains(upgrade)) continue;
            
            // If the player has the prereq, the upgrade is available
            bool hasPreReq = upgrade.requiredUpgrade == null || _ownedUpgrades.Contains(upgrade.requiredUpgrade);
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

   private IEnumerator WaitForUpgrade()
   {
       while (true)
       {
           yield return new WaitForSeconds(timeBetweenUpgrades);

            readyForUpgrade = false;
            TriggerUpgrade();
            
            while (!readyForUpgrade) yield return null;
       }
   }
}
