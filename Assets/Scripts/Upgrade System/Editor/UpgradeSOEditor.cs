using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.Universal;

[CustomEditor(typeof(UpgradeSO))]
[CanEditMultipleObjects]
public class UpgradeSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UpgradeSO upgrade = (UpgradeSO)target;
        
        serializedObject.Update();
        
        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (upgrade.upgradeType == UpgradeSO.UpgradeType.SpellUpgrade)
        {
            upgrade.maxHealthIncrease = 0;
            upgrade.maxManaIncrease = 0;
            upgrade.moveSpeedMultiplier = 1f;
            
            EditorGUILayout.LabelField("Spell Upgrade");
            EditorGUILayout.BeginVertical();
            upgrade.spellToRemove = EditorGUILayout.ObjectField("Spell To Remove", upgrade.spellToRemove, typeof(Spell), false) as Spell;
            upgrade.spellToGain = EditorGUILayout.ObjectField("Spell To Gain", upgrade.spellToGain, typeof(Spell), false) as Spell;
            EditorGUILayout.EndVertical();
        }
        else if (upgrade.upgradeType == UpgradeSO.UpgradeType.PlayerBuff)
        {
            upgrade.spellToRemove = null;
            upgrade.spellToGain = null;

            EditorGUILayout.LabelField("Player Buff");
            // Health, mana, or speed

            EditorGUILayout.BeginVertical();
            upgrade.maxHealthIncrease = EditorGUILayout.IntField("Max Health Increase", upgrade.maxHealthIncrease);
            upgrade.maxManaIncrease = EditorGUILayout.IntField("Max Mana Increase", upgrade.maxManaIncrease);
            upgrade.moveSpeedMultiplier =
                EditorGUILayout.FloatField("Move Speed Multiplier", upgrade.moveSpeedMultiplier);
            EditorGUILayout.EndVertical();
        }
    }
}
