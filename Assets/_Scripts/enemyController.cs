using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
    public GameObject unitPrefab;
    public Transform enemyUnitsParent;
    [Range(0,100)]public int spawnChance;
    public int manaCost;
    public GameObject enemyField;
    public Transform enemySpawnersParent;
    public int maximumManaCount;
    public GameObject manaPrefab;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private List<GameObject> enemySpawners;
    private List<GameObject> enemyManaList;
    private int enemyMana;
    [SerializeField]private Transform target;
    private unitMatcher unitMatcher;
    private Bounds manaSpawnBounds;
    private int manaCount;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        unitMatcher = FindObjectOfType<unitMatcher>();
        manaSpawnBounds = enemyField.GetComponent<BoxCollider>().bounds;

        enemyManaList = new List<GameObject>();
        enemySpawners = new List<GameObject>();
        foreach (Transform spawner in enemySpawnersParent)
        {
            spawner.tag = "enemySpawner";
            enemySpawners.Add(spawner.gameObject);
        }
            
        foreach (GameObject spawner in enemySpawners)
            Destroy(spawner.GetComponent<playerSpawnPoint>());
        
    }
    
    void Start()
    {
        spawnMana();
        AIMovement();
    }


    private float deltaTime = 0f;
    void Update()
    {
        deltaTime = Time.deltaTime;
        if (deltaTime > 2f)
            spawnMana();
    }

    private void AIMovement()
    {
        animator.SetTrigger("run");
        if (enemyMana >= manaCost && Random.Range(0, 101) < spawnChance)
            moveToSpawner();
        else
            moveToMana();            
    }

    private void moveToSpawner()
    {
        Transform closestSpawnerTransform = findClosestObjectInArray(enemySpawners.ToArray());
        navMeshAgent.SetDestination(closestSpawnerTransform.position);
    }

    private void moveToMana()
    {
        navMeshAgent.SetDestination(findClosestObjectInArray(enemyManaList.ToArray()).position);
    }

    private async void spawnUnit(GameObject spawner)
    {
        while(enemyMana >= manaCost)
        {
            updateMana(-manaCost);
            animator.SetTrigger("spawnSkeleton");
            await Task.Delay(System.TimeSpan.FromSeconds(1f));
            GameObject unit = Instantiate(unitPrefab, spawner.transform.position, Quaternion.identity, enemyUnitsParent);
            unit.tag = "enemyUnit";
            unitMatcher.addSkeletonToList(unitMatcher.enemyUnitsList, unit);
        }

        AIMovement();
    }

    private void updateMana(int value)
    {
        enemyMana += value;
    }
    private bool isTargetReached()
    {
        return Vector3.Distance(transform.position, target.position) <= 3f;
    }

    public void resetPath()
    {
        navMeshAgent.ResetPath();
    }


    private Transform findClosestObjectInArray(GameObject[] arr)
    {
        float minimumDistance = float.MaxValue;
        Transform closestObjectTransform = null;
        foreach (GameObject currentObject in arr)
            if (Vector3.Distance(currentObject.transform.position, transform.position) < minimumDistance)
                closestObjectTransform = currentObject.transform;

        return closestObjectTransform;
    }

    public void addManaToList(GameObject mana)
    {
        enemyManaList.Add(mana);
    }

    private void spawnMana()
    {
        deltaTime = 0f;
        if (manaCount > maximumManaCount)
            return;
        Vector3 spawnPoint = new Vector3(
            Random.Range(manaSpawnBounds.min.x, manaSpawnBounds.max.x),
            1.5f,
            Random.Range(manaSpawnBounds.min.z, manaSpawnBounds.max.z)
            );

        enemyManaList.Add(Instantiate(manaPrefab, spawnPoint, Quaternion.identity, enemyField.transform));
    }

    private void OnTriggerEnter(Collider other)
    {
        resetPath();
        if (other.CompareTag("enemySpawner"))
            spawnUnit(other.gameObject);
        if (other.CompareTag("mana"))
        {
            enemyMana += 3;
            enemyManaList.Remove(other.gameObject);
            Destroy(other.gameObject);
            if (enemyManaList.Count == 0)
                spawnMana();

            AIMovement();
            
        }
            
    }
}
