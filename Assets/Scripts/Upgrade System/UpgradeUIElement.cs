using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIElement : MonoBehaviour
{
    public Image upgradeIcon;
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI upgradeDescriptionText;
    
    private Action<UpgradeSO> OnSelect;
    private UpgradeSO _upgrade;

    public void SetUpgrade(UpgradeSO upgrade, Action<UpgradeSO> OnSelect)
    {
        _upgrade = upgrade;

        upgradeIcon.sprite = upgrade.upgradeIcon;
        upgradeNameText.text = upgrade.upgradeName;
        upgradeDescriptionText.text = upgrade.upgradeDescription;

        this.OnSelect = OnSelect;
    }

    private void ChooseUpgrade()
    {
        OnSelect(_upgrade);
    }
}
