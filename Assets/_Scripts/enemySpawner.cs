using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct spawnProperties
{
    public float minimumSpawnTime;
    public float maximumSpawnTime;
    public Transform spawnLocation;
    public Transform enemyUnitsParent;
}
public class enemySpawner : MonoBehaviour
{
    public List<unitStats> unitStats;
    public spawnProperties spawnProperties;
    public int initialEnemyUnitCount;

    private float spawnTime;
    private int maximumUnitLevel = 0;
    private unitMatcher unitMatcher;
    private bool spawningUnits = false;

    private void Awake()
    {
        unitMatcher = FindObjectOfType<unitMatcher>();
    }
    void Start()
    {
        spawnTime = generateRandomSpawnTime();
    }

    private float deltaTime = 0f;
    async void Update()
    {
        if (spawningUnits)
            return;


        if (deltaTime >= 1f)
            await spawnUnit(2);
        else
            deltaTime += Time.deltaTime;


    }


    private float generateRandomSpawnTime()
    {
        return Random.Range(spawnProperties.minimumSpawnTime, spawnProperties.maximumSpawnTime);
    }

    private async Task spawnUnit(int count)
    {
        spawningUnits = true;
        for(int i = 0; i< count; i++) { 
            int unitLevel = Random.Range(0, maximumUnitLevel);
            GameObject unit = Instantiate(unitStats[unitLevel].unitPrefab, spawnProperties.spawnLocation.position, Quaternion.identity, spawnProperties.enemyUnitsParent);
            unit.tag = "enemyUnit";
            unitMatcher.addSkeletonToList(unitMatcher.enemyUnitsList, unit);
            await Task.Delay(System.TimeSpan.FromSeconds(0.5f));
        }

        spawningUnits = false;
        deltaTime = 0f;
    }
}
