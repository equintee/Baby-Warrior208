using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct spawnProperties
{
    public float minimumSpawnTime;
    public float maximumSpawnTime;
    public Transform enemyUnitsParent;
    public int maximumUnitPerSpawn;
}
public class enemyFieldController : MonoBehaviour
{
    public List<unitStats> unitStats;
    public spawnProperties spawnProperties;
    public int initialUnitCount;

    private Transform spawnerParentTransform;
    [HideInInspector] public List<GameObject> spawners;

    private void Awake()
    {
        spawnerParentTransform = transform.GetChild(0);
        spawners = new List<GameObject>();

        foreach (Transform spawner in spawnerParentTransform)
            spawners.Add(spawner.gameObject);

        foreach (GameObject spawner in spawners)
        {
            Destroy(spawner.GetComponent<playerSpawnPoint>());
            enemySpawner enemySpawner = spawner.AddComponent<enemySpawner>();
            enemySpawner.initilizeSpawnProperties(spawnProperties, unitStats);
            spawner.tag = "enemySpawner";
        }

        for (int i = 0; i < initialUnitCount; i++)
            spawners[Random.Range(0, spawners.Count)].GetComponent<enemySpawner>().spawnUnit();

    }

}
