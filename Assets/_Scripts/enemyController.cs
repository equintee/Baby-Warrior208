using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class enemyController : MonoBehaviour
{
    public GameObject unitPrefab;
    public Material enemyUnitMaterial;
    public GameObject babyPrefab;
    public Transform enemyBabyUnits;
    public GameObject enemyPowerUpSpawner;
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
            spawner.tag = "enemySpawner";
            enemySpawners.Add(spawner.GetChild(1).gameObject);
        }
            
        foreach (GameObject spawner in enemySpawners)
            Destroy(spawner.GetComponent<playerSpawnPoint>());


        if (PlayerPrefs.HasKey("level"))
        {
            manaGain = PlayerPrefs.GetInt("level") + 10;
            spawnChance = PlayerPrefs.GetInt("level") * 10 + 80;
        }

        else
        {
            manaGain = 10;
            spawnChance = 80;
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
            manaGain /= 2;

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
        if(unitMatcher.enemySpawners.Count == 0)
        {
            moveToMana();
            return;
        }

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
            DOTween.Kill(spawner.transform);
            spawner.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            await spawner.transform.GetChild(0).DOScale(new Vector3(1.30f, 0.70f, 1), 0.25f).AsyncWaitForCompletion();
            await spawner.transform.GetChild(0).DOScale(Vector3.one, 0.25f).AsyncWaitForCompletion();

            GameObject unit = Instantiate(unitPrefab, spawner.transform.position, Quaternion.identity, enemyUnitsParent);
            unit.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = enemyUnitMaterial;
            unit.tag = "enemyUnit";
            unitMatcher.addSkeletonToList(unitMatcher.enemyUnitsList, unit);

    }

    private async void spawnBaby(GameObject spawner)
    {
        if (enemyMana >= manaCost)
        {
            DOTween.Kill(spawner.transform);
            spawner.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            await spawner.transform.GetChild(0).DOScale(new Vector3(1.30f, 0.70f, 1), 0.25f).AsyncWaitForCompletion();
            await spawner.transform.GetChild(0).DOScale(Vector3.one, 0.25f).AsyncWaitForCompletion();
            updateMana(-manaCost);
            Vector3 spawnPoint = spawner.transform.position;
            spawnPoint.y = 1.5f;
            GameObject baby = Instantiate(babyPrefab, spawner.transform.position, Quaternion.identity, enemyBabyUnits);
            baby.transform.DOLookAt(enemyPowerUpSpawner.transform.position, 0f);
            baby.transform.DOMoveZ(enemyPowerUpSpawner.transform.position.z, 3f).SetSpeedBased().SetEase(Ease.Linear);
            await baby.transform.DOMoveX(enemyPowerUpSpawner.transform.position.x, 3f).SetSpeedBased().SetEase(Ease.Linear).AsyncWaitForCompletion();

            spawnUnit(enemyPowerUpSpawner);
            Destroy(baby.gameObject);

            
        }
            
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

        while(Physics.Raycast(spawnPoint, Vector3.down, 0.01f))
            spawnPoint = new Vector3(
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

        if (!other.gameObject.CompareTag("enemySpawner") || ignoreSpawner)
            return;

        if (manaCost > enemyMana)
        {
            ignoreSpawner = true;
            AIMovement();
            return;
        }
            

        spawnTime += Time.deltaTime;

        if (spawnTime > 0.5f)
        {
            spawnBaby(other.transform.parent.gameObject);
            spawnTime = 0f;
        }
            
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemySpawner"))
            return;
        ignoreSpawner = false;
        spawnTime = 0f;
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
