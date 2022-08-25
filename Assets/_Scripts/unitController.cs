﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class unitController : MonoBehaviour
{
    public int hp;
    public int damage;
    public float damageAnimationLength;
    public float deathAnimationLength;
    public float gettingHitAnimationLength;
    public GameObject droppedGoldPrefab;
    [HideInInspector] public bool isTargetSpawner = false;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isLookingForTarget = true;
    public bool cancelAttack = false; 

    private float moveSpeed;
    private Rigidbody rb;
    private Animator animator;
    private Transform target;
    private unitMatcher unitMatcher;
    private NavMeshAgent navMeshAgent;
    
    private void Awake()
    {
        moveSpeed = FindObjectOfType<unitMatcher>().unitSpeed;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        unitMatcher = transform.parent.parent.GetComponent<unitMatcher>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;

        isTargetSpawner = false;
        isAlive = true;
        isLookingForTarget = true;

    }
    public void setTarget(Transform target)
    {
        this.target = target;
        moveToTarget();
    }

    private void Update()
    {

        if (!target)
        {
            target = null;
            isLookingForTarget = true;
            return;
        }

        if(isTargetReached() && isLookingForTarget)
        {
            animateHit(damage);
        }

    }

    private async void animateHit(int damage)
    {
        isLookingForTarget = false;
        cancelAttack = false;
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.DOLookAt(target.position, 0f);
        animator.SetTrigger("attack");
        await Task.Delay(System.TimeSpan.FromSeconds(damageAnimationLength));

        cancelAttack = !target ? true : false;

        if (cancelAttack)
        {
            isLookingForTarget = true;
            return;
        }
            
        if(target && (target.CompareTag("enemyUnit") || target.CompareTag("playerUnit")))
        {
            target.GetComponent<unitController>().decrementHp(damage);
            bool isTargetDead = target.GetComponent<unitController>().getHit();
            target = isTargetDead ? null : target;
        }
        else if(target && target.CompareTag("playerSpawner"))
        {
            unitMatcher.removeSpawnerFromList(target.gameObject);
            explodeCapsules();            
        }
        else if(target && target.CompareTag("enemySpawner"))
        {
            unitMatcher.enemySpawners.Remove(target.gameObject);
            explodeCapsules();
        }
        else if(target && target.CompareTag("powerUpSpawner"))
        {
            explodeCapsules();
            if (transform.CompareTag("enemyUnit"))
                FindObjectOfType<levelController>().endGame(false);
            else
            {
                FindObjectOfType<levelController>().moveCameraAtFinalPosition();
                unitMatcher.isEnemyPowerUpSpawnerAlive = false;
            }
                
        }
        if(rb)
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        isLookingForTarget = true;

    }

    public void decrementHp(int damage)
    {
        hp -= damage;
    }

    private void explodeCapsules()
    {
        List<Rigidbody> pieceRBList = new List<Rigidbody>();
        foreach(Transform spawnerPiece in target.transform.GetChild(0).GetChild(0))
        {
            pieceRBList.Add(spawnerPiece.gameObject.AddComponent<Rigidbody>());
            spawnerPiece.gameObject.AddComponent<BoxCollider>();
        }

        foreach (Rigidbody rb in pieceRBList)
            if (rb)
                rb.AddExplosionForce(1.5f, rb.position, 1);
            else
                return;

        //Kapsül içindeki spermin hemen kaybolması için satır 110. nun yorumunu kaldırabilirsin.
        Destroy(target.GetChild(2).gameObject);
        Destroy(target.gameObject, 2f);
    }

    public bool getHit()
    {
        resetPath();
        cancelAttack = true;
        isLookingForTarget = false;
        if (hp > 0)
            animateGettingHit();
        else if (isAlive)
            return animateDeath();

        return false;

    }
    public async void animateGettingHit()
    {
        animator.SetTrigger("getHit");
        await Task.Delay(System.TimeSpan.FromSeconds(gettingHitAnimationLength));
        isLookingForTarget = true;
    }
    public bool animateDeath()
    {
        isAlive = false;
        unitMatcher.moveSkeletonToCorpse(gameObject);
        cancelAttack = true;
        if (transform.CompareTag("playerUnit"))
            unitMatcher.playerUnitsList.Remove(gameObject);
        if (transform.CompareTag("enemyUnit"))
        {
            GameObject goldText = Instantiate(droppedGoldPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.Euler(0, 180, 0), null);
            goldText.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            goldText.transform.DOMoveY(goldText.transform.position.y + 1, 1.5f).SetEase(Ease.Linear);
            Destroy(goldText, 2f);
            unitMatcher.enemyUnitsList.Remove(gameObject);
            FindObjectOfType<playerController>().updateGold(unitMatcher.goldPerUnit);
        }
            

        navMeshAgent.enabled = false;
        animator.SetTrigger("death");
        this.enabled = false;
        Invoke("destroyUnit", deathAnimationLength + 2f);
        return true;
    }

    public void destroyUnit()
    {
        DOTween.Kill(rb);
        Destroy(gameObject);
    }

    public bool isTargetNullOrBoss()
    {
        return !target || isTargetSpawner;
    }

    public void moveToTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }

    public bool isTargetReached()
    {
        return Vector3.Distance(transform.position, target.position) <= 3.5f;
    }

    public void resetPath()
    {
        if (navMeshAgent.enabled == false)
            return;
        navMeshAgent.ResetPath();
    }

    public Transform getTarget()
    {
        return target;
    }
}
