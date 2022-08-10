using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct spawnProperties
{
    public float minimumSpawnTime;
    public float maximumSpawnTime;
    public Transform spawnLocation;
}
public class enemySpawner : MonoBehaviour
{
    public List<unitStats> unitStats;
    public spawnProperties spawnIntervals;
    public int initialEnemyUnitCount;

    private float spawnTime;

    void Start()
    {
        Debug.Log(Random.Range(0, 0));
    }

    private float deltaTime = 0f;
    void Update()
    {
        deltaTime += Time.deltaTime;

    }


    private float generateRandomSpawnTime()
    {
        return Random.Range(spawnIntervals.minimumSpawnTime, spawnIntervals.maximumSpawnTime);
    }

    private void spawnUnits()
    {
        int unitLevel = Random.Range(0, 2);
    }
}
