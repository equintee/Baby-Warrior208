using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{
    [SerializeField] private Transform collectableManaParent;
    
    private float movementSpeed;
    private Rigidbody rb;
    private Animator animator;
    private float upperBoundX;
    private float lowerBoundX;
    private float upperBoundZ;
    private float lowerBoundZ;
    private int mana;

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


        moveToMana();
        animator.SetTrigger("run");
    }


    private GameObject closestMana;
    private void moveToMana()
    {
        if (closestMana == null && !findClosestMana())
        {
            makeRandomMovement();
            return;
        }

        DOTween.Kill(rb);
        Vector3 wayPoint = closestMana.transform.position;
        wayPoint.y = 0;
        transform.DOLookAt(wayPoint, 0f);
        rb.DOMove(wayPoint, movementSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => moveToMana());



    }

    private void makeRandomMovement()
    {
        Vector3 waypoint = new Vector3(UnityEngine.Random.Range(lowerBoundX, upperBoundX), 0, UnityEngine.Random.Range(lowerBoundZ, upperBoundZ));
        transform.DOLookAt(waypoint, 0f);
        rb.DOMove(waypoint, movementSpeed).SetSpeedBased().SetEase(Ease.Linear).OnUpdate(() => { if (findClosestMana()) moveToMana(); }).OnComplete(() => moveToMana());
        
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



}
