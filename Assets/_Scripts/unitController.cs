using DG.Tweening;
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
    [HideInInspector] public bool isTargetSpawner = false;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isLookingForTarget = true;
    [HideInInspector] public bool cancelAttack = false; 

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
            return;

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
        
        if (cancelAttack)
            return;
        if(target && (target.CompareTag("enemyUnit") || target.CompareTag("playerUnit")))
        {
            target.GetComponent<unitController>().decrementHp(damage);
            bool isTargetDead = target.GetComponent<unitController>().getHit();
            target = isTargetDead ? null : target;
        }
        else if(target && target.CompareTag("playerSpawner"))
        {
            explodeCapsules();
            unitMatcher.removeSpawnerFromList(target.gameObject);
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
        foreach(Transform spawnerPiece in target.transform.GetChild(0))
        {
            pieceRBList.Add(spawnerPiece.gameObject.AddComponent<Rigidbody>());
            spawnerPiece.gameObject.AddComponent<BoxCollider>();
        }

        foreach (Rigidbody rb in pieceRBList)
            rb.AddExplosionForce(1.5f, rb.position, 1);

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

        if (transform.CompareTag("playerUnit"))
            unitMatcher.playerUnitsList.Remove(gameObject);
        if (transform.CompareTag("enemyUnit"))
        {
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
}
