using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class enemyController : MonoBehaviour
{
    [SerializeField] private Transform collectableManaParent;
    public Transform unitSpawner;
    public GameObject enemyUnit;
    public GameObject enemyUnitParent;
    public int manaCostPerUnit;


    private float movementSpeed;
    private Rigidbody rb;
    private Animator animator;
    private float upperBoundX;
    private float lowerBoundX;
    private float upperBoundZ;
    private float lowerBoundZ;
    private unitMatcher unitMatcher;
    private int mana = 0;
    private bool spawningUnits = false;
    void Start()
    {
        //Set movement bounds
        playerController playerController = FindObjectOfType<playerController>();
        float[] borders = playerController.getBorders();

        upperBoundX = borders[0];
        upperBoundZ = borders[1];

        lowerBoundX = -1 * upperBoundX;
        lowerBoundZ = -1 * upperBoundZ;

        //Assign values
        rb = GetComponent<Rigidbody>();
        movementSpeed = playerController.movementSpeed;
        animator = transform.GetChild(0).GetComponent<Animator>();
        unitMatcher = FindObjectOfType<unitMatcher>();

        AIMovement();
        animator.SetTrigger("run");
    }


    private GameObject closestMana;

    private void AIMovement()
    {

        if (findClosestMana())
        {
            bool spawnUnits = (mana > manaCostPerUnit && UnityEngine.Random.Range(0, 100) > 60) ? true : false;
            if (spawnUnits)
                moveToSpawner();
            else
                patrol();

        }
            
    }

    private void moveToClosestMana()
    {
        DOTween.Kill(rb);
        Vector3 wayPoint = closestMana.transform.position;
        wayPoint.y = 0;
        transform.DOLookAt(wayPoint, 0f);
        rb.DOMove(wayPoint, movementSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => AIMovement());
    }

    private void patrol()
    {
        DOTween.Kill(rb);

        Vector3 waypoint = new Vector3(UnityEngine.Random.Range(lowerBoundX, upperBoundX), 0, UnityEngine.Random.Range(lowerBoundZ, upperBoundZ));
        transform.DOLookAt(waypoint, 0f);
        rb.DOMove(waypoint, movementSpeed).SetSpeedBased().SetEase(Ease.Linear).OnUpdate(() => { if (findClosestMana()) moveToClosestMana(); }).OnComplete(() => AIMovement());
        
    }

    private bool findClosestMana()
    {
        float minDistance = 9999;
        foreach(Transform mana in collectableManaParent)
        {
            Vector3 distanceVector = mana.transform.position - transform.position;
            float distance = distanceVector.sqrMagnitude;
            if(distanceVector.sqrMagnitude < minDistance)
            {
                closestMana = mana.gameObject;
                minDistance = distanceVector.sqrMagnitude;
            }
        }

        return minDistance != 9999;     

    }


    public void updateMana(int value)
    {
        mana += value;
    }

    private void moveToSpawner()
    {
        DOTween.Kill(rb);

        transform.DOLookAt(unitSpawner.position, 0f);
        rb.DOMove(unitSpawner.position, movementSpeed).SetSpeedBased().SetEase(Ease.Linear);

    }
    public async void spawnSkeletons(Vector3 spawnPosition)
    {
        DOTween.Kill(rb);

        while(mana >= manaCostPerUnit)
        {
            updateMana(-manaCostPerUnit);
            animator.SetTrigger("spawnSkeleton");
            await Task.Delay(System.TimeSpan.FromSeconds(1f));
            GameObject spawnedSkeleton = Instantiate(enemyUnit, spawnPosition, Quaternion.identity, enemyUnitParent.transform);
            unitMatcher.enemyUnitsList.Add(spawnedSkeleton);
            if (unitMatcher.enemyUnitsList.Count == 1)
                unitMatcher.setTargetForAllUnits(unitMatcher.enemyUnitsList);
            unitMatcher.setTarget(spawnedSkeleton);
        }

        AIMovement();
    }


}
