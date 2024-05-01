using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Enemy", order = 2)]
public class EnemySO : ScriptableObject
{
    [Tooltip("The enemy to instantiate")]
    public GameObject enemyPrefab;
    [Tooltip("How many points does it cost to instantiate this enemy?")]
    public int enemyCost;
    [Tooltip("What is the tier level of this enemy?")]
    [Min(1)]
    public int enemyTier = 1;
}
