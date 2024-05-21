using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Difficulty", order = 3)]
public class DifficultyOptionSO : ScriptableObject
{
    [Tooltip("How many seconds should pass between upgrades?")]
    public float timeBetweenUpgrades = 30f;
    [Tooltip("How many points should the spawner initially have?")]
    public int startingSpawnPoints = 4;
    [Tooltip("How often should the next wave of enemies be spawned?")]
    public float timeBetweenSpawns = 30f;
    [Tooltip("Choosing this difficulty multiplies all points gained by this amount")]
    public float pointMultiplier = 1f;
}
