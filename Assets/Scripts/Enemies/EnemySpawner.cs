using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Avaiable Enemies")] 
    public EnemySO[] enemies;
    
    [Header("Spawn Information")]
    public Transform[] spawnLocations;
    public int roundsPerTier = 3;
    public int pointsGainedPerRound = 1;
    public int initialPoints = 3;

    [Header("Spawn Timer")] 
    public float timeBetweenRounds = 30f;
    
    private int roundsToNextTier;

    private void Start()
    {
        roundsToNextTier = roundsPerTier;

        StartCoroutine(HandleSpawns());
    }

    public void SpawnEnemies()
    {
        // Formula:
        //  The spawner has a certain amount of points available and a tier level, and every enemy has a point cost.
        //  The spawner can choose any number of enemies to create, as long as it can afford them and they are
        //  in the appropriate tier.
        List<EnemySO> availableEnemies = new List<EnemySO>();

        foreach (var enemy in enemies)
        {
            if (enemy.enemyTier <= GameManager.GetCurrentTier()) availableEnemies.Add(enemy);
        }
        
        // choose which enemies to spawn (random)
        int points = initialPoints;
        var enemiesToSpawn = new List<GameObject>();

        while (points > 0)
        {
            // choose an enemy
            var chosenEnemy = availableEnemies[Random.Range(0, availableEnemies.Count)];
            enemiesToSpawn.Add(chosenEnemy.enemyPrefab);
            points -= chosenEnemy.enemyCost;
            
            // Remove unaffordable enemies and break loop if necessary
            for (int i = availableEnemies.Count - 1; i >= 0; i--)
            {
                if (availableEnemies[i].enemyCost > points)
                {
                    availableEnemies.RemoveAt(i);
                }
            }

            if (availableEnemies.Count == 0) break;
        }
        
        // Spawn the enemies
        foreach (var enemyToSpawn in enemiesToSpawn)
        {
            Transform spawnLocation = spawnLocations[Random.Range(0, spawnLocations.Length)];

            Instantiate(enemyToSpawn, spawnLocation.position, spawnLocation.rotation);
        }
        
        // Increase round counter, tier counter, and available point counter
        initialPoints += pointsGainedPerRound;
        roundsToNextTier--;

        if (roundsToNextTier == 0)
        {
            GameManager.IncreaseTier();
            roundsToNextTier = roundsPerTier;
        }
    }

    private IEnumerator HandleSpawns()
    {
        while (true)
        {
            SpawnEnemies();

            yield return new WaitForSeconds(timeBetweenRounds);
        }
    }
}
