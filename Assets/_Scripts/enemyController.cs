using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
    public GameObject unitPrefab;
    public Transform enemyUnitsParent;
    public GameObject enemyField;
    public Transform enemySpawnersParent;
    public int maximumManaCount;
    public GameObject manaPrefab;

    private int spawnChance;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private List<GameObject> enemySpawners;
    private List<GameObject> enemyManaList;
    private int enemyMana;
    private unitMatcher unitMatcher;
    private Bounds manaSpawnBounds;
    private int manaCount;
    private int manaCost = 10;
    private int manaGain = 10;
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
            spawner.GetChild(1).tag = "enemySpawner";
            enemySpawners.Add(spawner.GetChild(1).gameObject);
        }
            
        foreach (GameObject spawner in enemySpawners)
            Destroy(spawner.GetComponent<playerSpawnPoint>());


        if (PlayerPrefs.HasKey("level"))
        {
            manaGain = PlayerPrefs.GetInt("level") + 13;
            spawnChance = PlayerPrefs.GetInt("level") * 10 + 80;
        }

        else
        {
            manaGain = 10;
            spawnChance = 80;
        }

        decrementManaGain();
        decrementSpawnChannce();
    }
    
    void Start()
    {
        spawnMana();
        AIMovement();
    }


    private async void decrementManaGain()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(5f));
        
        if (manaGain <= 5)
            return;

        manaGain--;
        decrementManaGain();
    }

    private async void decrementSpawnChannce()
    {
        await Task.Delay(System.TimeSpan.FromSeconds(5f));

        if (spawnChance <= 70)
            return;

        spawnChance -= 5;
        decrementSpawnChannce();
    }

    private float deltaTime = 0f;
    void Update()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime > 2f)
            spawnMana();
    }

    private void AIMovement()
    {
        resetPath();
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

    private void spawnUnit(GameObject spawner)
    {
        if(enemyMana >= manaCost)
        {
            updateMana(-manaCost);
            GameObject unit = Instantiate(unitPrefab, spawner.transform.parent.position, Quaternion.identity, enemyUnitsParent);
            unit.tag = "enemyUnit";
            unitMatcher.addSkeletonToList(unitMatcher.enemyUnitsList, unit);
        }

        AIMovement();
    }

    private void updateMana(int value)
    {
        enemyMana += value;
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
        GameObject mana = Instantiate(manaPrefab, spawnPoint, Quaternion.identity, enemyField.transform);
        mana.tag = "enemyMana";
        enemyManaList.Add(mana);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("enemyMana"))
        {
            enemyMana += manaGain;
            enemyManaList.Remove(other.gameObject);
            Destroy(other.gameObject);
            if (enemyManaList.Count == 0)
                spawnMana();
            AIMovement();
        }
            
    }
    private float spawnTime = 0f;
    private bool ignoreSpawner = false;
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("enemySpawner") && ignoreSpawner)
            return;

        if (manaCost > enemyMana)
        {
            ignoreSpawner = true;
            return;
        }
            

        spawnTime += Time.deltaTime;

        if (spawnTime > 0.5f)
            spawnUnit(other.gameObject);
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemySpawner"))
            return;
        ignoreSpawner = false;
        spawnTime = 0f;
        AIMovement();
    }

    //naming :/
    public void playerStoleMana(GameObject stolenMana)
    {
        enemyManaList.Remove(stolenMana);
        manaCount--;
        if (enemyManaList.Count == 0)
            spawnMana();
        AIMovement();
    }
}
