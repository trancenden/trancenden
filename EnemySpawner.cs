//This script creates waves of enemies.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    public float firtWaitTime;
    public float waitTimeBetweenWaves;
    public List<WaveConfig> waveConfigs;
    public bool looping;
    public bool randomize;

    List<WaveConfig> newList = new List<WaveConfig>();
    [HideInInspector]
    public bool isLastWave;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (randomize)
        {
            System.Random rnd = new System.Random();
            newList = waveConfigs.OrderBy(x => rnd.Next()).ToList();
        }
        else
        {
            newList = waveConfigs;
        }

        StartCoroutine(SpawnAllWaves(newList));        
    }

    IEnumerator SpawnAllWaves(List<WaveConfig> newWaveConfigs)
    {
        yield return new WaitForSeconds(firtWaitTime);
        do
        {
            for (int i = 0; i < newWaveConfigs.Count(); i++)
            {
                yield return new WaitForSeconds(waitTimeBetweenWaves);
                bool isLast = false;
                if (!looping && i == newWaveConfigs.Count() - 1)
                {
                    isLast = true;
                }
                yield return StartCoroutine(SpawnAllEnemies(newWaveConfigs[i], isLast));
            }
        } while (looping);
    }

    private IEnumerator SpawnAllEnemies(WaveConfig currentWave, bool isLast)
    {
        for (int enemyCount = 0; enemyCount < currentWave.GetNumberOfEnemies(); enemyCount++)
        {
            GameObject original;
            if (currentWave.isRandomEnemy)
            {
                original = currentWave.GetRandomEnemyPreFab();
            }
            else
            {
                original = currentWave.GetEnemyPrefab();
            }
            var newEnemy = Instantiate(original, currentWave.GetWayPoints()[0].transform.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(currentWave);
            
            var enemy = newEnemy.GetComponent<Enemy>();
            if (enemyCount == currentWave.GetNumberOfEnemies() - 1)
            {
                enemy.isLastShip = true;
            }
            else
            {
                enemy.isLastShip = false;
            }
           
            yield return new WaitForSeconds(currentWave.GetTimeBetweenSpawns());
           
        }
        if (isLast)
        {
            isLastWave = true;            
        }
    }
}
