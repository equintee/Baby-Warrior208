using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class enemySpawner : MonoBehaviour
{
    private float minimumSpawnTime;
    private float maximumSpawnTime;
    private Transform enemyUnitsParent;
    private int maximumUnitPerSpawn;
    private List<unitStats> unitStats;
    
    private unitMatcher unitMatcher;
    private bool spawningUnits = false;
    private float spawnTime;
    private int spawnCount = 1;
    private void Awake()
    {
        unitMatcher = FindObjectOfType<unitMatcher>();
    }
    void Start()
    {
        spawnTime = generateRandomSpawnTime();
        spawnCount = generateSpawnCount();
    }

    private float deltaTime = 0f;
    async void Update()
    {
        if (spawningUnits)
            return;


        if (deltaTime >= spawnTime)
            await spawnUnit();
        else
            deltaTime += Time.deltaTime;


    }


    private float generateRandomSpawnTime()
    {
        return Random.Range(minimumSpawnTime, maximumSpawnTime);
    }

    private int generateSpawnCount()
    {
        return Random.Range(1, maximumUnitPerSpawn + 1);
    }

    public unitStats generateRandomUnit()
    {
        return unitStats[Random.Range(0, unitStats.Count)];
    }

    public async Task spawnUnit()
    {
        spawningUnits = true;
        for(int i = 0; i< spawnCount; i++) {
            unitStats unit = generateRandomUnit();
            GameObject spawnedUnit = Instantiate(unit.unitPrefab, transform.position, Quaternion.identity, enemyUnitsParent);
            spawnedUnit.tag = "enemyUnit";
            unitMatcher.addSkeletonToList(unitMatcher.enemyUnitsList, spawnedUnit);
            await Task.Delay(System.TimeSpan.FromSeconds(0.5f));
        }

        spawnTime = generateRandomSpawnTime();
        spawnCount = generateSpawnCount();
        spawningUnits = false;
        deltaTime = 0f;
    }

    public void initilizeSpawnProperties(spawnProperties spawnProperties, List<unitStats> unitStats)
    {
        minimumSpawnTime = spawnProperties.minimumSpawnTime;
        maximumSpawnTime = spawnProperties.maximumSpawnTime;
        enemyUnitsParent = spawnProperties.enemyUnitsParent;
        maximumUnitPerSpawn = spawnProperties.maximumUnitPerSpawn;
        this.unitStats = unitStats;
    }
}
